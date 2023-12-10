using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxCalculations.Queries;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="GetTaxCalculationQueryHandler"/> handler class
/// </summary>
/// <param name="context"></param>
public class GetTaxCalculationQueryHandler(AppDbContext context) : IRequestHandler<GetTaxCalculationQuery, IResponse<TaxCalculation>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<TaxCalculation>> Handle(
        GetTaxCalculationQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            throw new ArgumentNullException(nameof(request), "Request id can not be null");
        }

        var response = await _context.TaxCalculation.Where(m => m.Id == request.Id).Select(m => new TaxCalculation
        {
            Id = m.Id,
            AnnualIncome = m.AnnualIncome,
            TaxAmount = m.TaxAmount,
            DateUpdated = m.DateUpdated,
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


        if (response == null)
        {
            return new Response<TaxCalculation>(
                raw: null,
                reason: "entity not found",
                statusCode: HttpStatusCode.NotFound);
        }

        var result = new Response<TaxCalculation>(
            response,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}
