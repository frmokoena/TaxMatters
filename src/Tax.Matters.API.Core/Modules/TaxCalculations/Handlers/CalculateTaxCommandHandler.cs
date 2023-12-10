using MediatR;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Domain.Enums;
using Tax.Matters.Infrastructure.Data.Repositories;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Handlers;

/// <summary>
/// Intialiazes a new instance of the <see cref="CalculateTaxCommandHandler"/> handler class 
/// </summary>
/// <param name="repository"></param>
public class CalculateTaxCommandHandler(ICalculationRepository repository) : IRequestHandler<CalculateTaxCommand, IResponse<TaxCalculation>>
{
    private readonly ICalculationRepository _repository = repository;

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

        var postalCode = await _repository.GetPostalCodeAsync(request.Model.PostalCode, cancellationToken);

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
                var result = await CalculateProgressiveTaxAsync(request.Model.AnnualIncome, postalCode, cancellationToken);

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

        await _repository.AddAsync(calculation, cancellationToken);

        return new Response<TaxCalculation>(
                    calculation,
                    raw: string.Empty,
                    HttpStatusCode.OK);
    }


    private async Task<Response<TaxCalculation>> CalculateProgressiveTaxAsync(
        decimal annualIncome,
        PostalCode postalCode,
        CancellationToken cancellationToken)
    {
        // Sanity check
        if (postalCode?.IncomeTax?.TypeName != TaxCalculationType.Progressive)
        {
            return new Response<TaxCalculation>(
                raw: null,
                reason: "Invalid calculation type",
                statusCode: HttpStatusCode.BadRequest);
        }

        var table = await _repository.GetProgressiveIncomeTaxTableAsync(
            postalCode.IncomeTax.Id,
            annualIncome,
            cancellationToken);

        if (table.Count == 0)
        {
            return new Response<TaxCalculation>(
                raw: null,
                reason: "Progressive tax table is not defined",
                statusCode: HttpStatusCode.Conflict);
        }

        // Sanity check
        var firstBracket = table[0];
        if (firstBracket.MinimumIncome > annualIncome)
        {
            return new Response<TaxCalculation>(
                raw: null,
                reason: "Progressive tax table is not applicable to the provided income",
                statusCode: HttpStatusCode.Conflict);
        }

        decimal taxAmount = 0;
        var residual = annualIncome;

        ProgressiveIncomeTax? bracket, nextBracket;

        decimal bracketShare, bracketTaxAmount;

        for (var i = 0; i < table.Count; i++)
        {
            if (residual == 0)
            {
                break;
            }

            bracket = table[i];

            // Sanity check
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

            // Sanity check
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
                bracketTaxAmount = bracket.Rate / 100 * residual;
                taxAmount += bracketTaxAmount;
                residual = 0;
            }
            else
            {
                bracketShare = bracket.MinimumIncome == 0 ?
                    bracket.MaximumIncome.Value - bracket.MinimumIncome : bracket.MaximumIncome.Value - bracket.MinimumIncome + 1;

                if (bracketShare > residual)
                {
                    bracketTaxAmount = bracket.Rate / 100 * residual;
                    taxAmount += bracketTaxAmount;
                    residual = 0;
                }
                else
                {
                    bracketTaxAmount = bracket.Rate / 100 * bracketShare;
                    taxAmount += bracketTaxAmount;
                    residual -= bracketShare;
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