using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Domain.Enums;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Handlers;

public class CalculateTaxCommandHandler(AppDbContext context) : IRequestHandler<CalculateTaxCommand, IResponse<TaxCalculation>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<TaxCalculation>> Handle(
        CalculateTaxCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Model?.PostalCode))
        {
            throw new ArgumentNullException(nameof(request), "Request model can not be null");
        }

        if (request.Model.AnnualIncome < 0)
        {
            throw new InvalidOperationException("Income can never be less than 0");
        }

        var taxCalculation = await _context.TaxCalculation
            .Where(
                m =>
                m.AnnualIncome == request.Model.AnnualIncome
                && m.PostalCode.Code == request.Model.PostalCode).Select(m => new TaxCalculation
                {
                    Id = m.Id,
                    AnnualIncome = m.AnnualIncome,
                    TaxAmount = m.TaxAmount,
                    DateCreated = m.DateCreated,
                    PostalCode = new PostalCode
                    {
                        Code = m.PostalCode.Code,
                        IncomeTax = new IncomeTax
                        {
                            TypeName = m.PostalCode.IncomeTax.TypeName
                        }
                    },
                    Version = m.Version
                })
        .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        // return existing if available
        if (taxCalculation != null)
        {
            return new Response<TaxCalculation>(
                        taxCalculation,
                        raw: string.Empty,
                        HttpStatusCode.OK);
        }

        var postalCode = await _context.PostalCode
            .Where(m => m.Code == request.Model.PostalCode)
            .Select(m => new PostalCode
            {
                Id = m.Id,
                Code = m.Code,
                IncomeTax = new IncomeTax
                {
                    TypeName = m.IncomeTax.TypeName,
                    FlatRate = m.IncomeTax.FlatRate,
                    FlatValue = m.IncomeTax.FlatValue == null ? null : new FlatValueIncomeTax
                    {
                        Amount = m.IncomeTax.FlatValue.Amount,
                        Threshold = m.IncomeTax.FlatValue.Threshold,
                        ThresholdRate = m.IncomeTax.FlatValue.ThresholdRate
                    },
                    ProgressiveTaxTable = m.IncomeTax.ProgressiveTaxTable.Select(m => new ProgressiveIncomeTax
                    {
                        MinimumIncome = m.MinimumIncome,
                        MaximumIncome = m.MaximumIncome,
                        Rate = m.Rate
                    }).ToList()
                }
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (postalCode == null)
        {
            return new Response<TaxCalculation>(
                raw: null,
                reason: $"{nameof(PostalCode)} not found",
                statusCode: HttpStatusCode.NotFound);
        }

        decimal taxAmount;

        switch (postalCode.IncomeTax.TypeName)
        {
            case TaxCalculationType.FlatRate:
                taxAmount = postalCode.IncomeTax.FlatRate!.Value / 100 * request.Model.AnnualIncome;

                break;
            case TaxCalculationType.FlatValue:
                taxAmount = postalCode.IncomeTax.FlatValue!.Amount;
                if (request.Model.AnnualIncome < postalCode.IncomeTax.FlatValue!.Threshold)
                {
                    taxAmount = postalCode.IncomeTax.FlatValue.ThresholdRate / 100 * request.Model.AnnualIncome;
                }

                break;
            case TaxCalculationType.Progressive:
                var result = CalculateProgressiveTax(request.Model.AnnualIncome, postalCode);

                if (result.IsError)
                {
                    return result;
                }
                taxAmount = result.Content!.TaxAmount;
                break;
            default:
                return new Response<TaxCalculation>(
                    raw: null,
                    reason: $"Tax calculation type is not supported",
                    statusCode: HttpStatusCode.BadRequest);
        }

        var calculation = new TaxCalculation
        {
            AnnualIncome = request.Model.AnnualIncome,
            TaxAmount = taxAmount,
            PostalCodeId = postalCode.Id
        };

        _context.TaxCalculation.Add(calculation);

        await _context.SaveChangesAsync(cancellationToken);

        return new Response<TaxCalculation>(
                    calculation,
                    raw: string.Empty,
                    HttpStatusCode.OK);
    }


    private static Response<TaxCalculation> CalculateProgressiveTax(decimal annualIncome, PostalCode postalCode)
    {
        if (postalCode.IncomeTax.ProgressiveTaxTable.Count == 0)
        {
            return new Response<TaxCalculation>(
                raw: null,
                reason: "Progressive tax table is not defined",
                statusCode: HttpStatusCode.BadRequest);
        }

        var table = postalCode.IncomeTax.ProgressiveTaxTable.OrderBy(m => m.MinimumIncome).ToList();

        // sanity check
        var firstBracket = table[0];
        if (firstBracket.MinimumIncome > annualIncome)
        {
            return new Response<TaxCalculation>(
                raw: null,
                reason: "Progressive tax table is not applicable to the provided income",
                statusCode: HttpStatusCode.BadRequest);
        }

        decimal taxAmount = 0;
        var remaining = annualIncome;

        ProgressiveIncomeTax? bracket, nextBracket;

        decimal bracketShare;

        for (var i = 0; i < table.Count; i++)
        {
            if (remaining == 0)
            {
                break;
            }

            bracket = table[i];

            // sanity check
            if (bracket.MaximumIncome != null && bracket.MinimumIncome > bracket.MaximumIncome)
            {
                return new Response<TaxCalculation>(
                    raw: null,
                    reason: $"Progressive tax table is not valid",
                    statusCode: HttpStatusCode.Conflict);
            }

            if (table.Count > i + 1)
            {
                nextBracket = table[i + 1];
            }
            else
            {
                nextBracket = null;
            }

            // sanity check
            if (nextBracket != null)
            {
                if (bracket.MaximumIncome == null || nextBracket.MinimumIncome - bracket.MaximumIncome != 1)
                {
                    return new Response<TaxCalculation>(
                        raw: null,
                        reason: $"Progressive tax table is not valid",
                        statusCode: HttpStatusCode.Conflict);
                }
            }

            if (bracket.MaximumIncome == null)
            {
                taxAmount += bracket.Rate / 100 * remaining;
                remaining = 0;
            }
            else
            {
                bracketShare = bracket.MaximumIncome.Value - bracket.MinimumIncome;

                if (bracketShare > remaining)
                {
                    taxAmount += bracket.Rate / 100 * remaining;
                    remaining = 0;
                }
                else
                {
                    taxAmount += bracket.Rate / 100 * bracketShare;
                    remaining -= bracketShare;
                }
            }
        }

        var calculation = new TaxCalculation
        {
            AnnualIncome = annualIncome,
            TaxAmount = taxAmount,
            PostalCodeId = postalCode.Id
        };

        return new Response<TaxCalculation>(
                    calculation,
                    raw: string.Empty,
                    HttpStatusCode.OK);
    }
}