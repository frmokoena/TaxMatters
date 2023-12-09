using MediatR;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxManagement.Queries;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.TaxManagement.Handlers;

public class GetTaxCalculationTypesQueryHandler(AppDbContext context) : IRequestHandler<GetTaxCalculationTypesQuery, IResponse<PageList<IncomeTax>>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<PageList<IncomeTax>>> Handle(
        GetTaxCalculationTypesQuery request, CancellationToken cancellationToken)
    {
        if (request.Model == null)
        {
            throw new ArgumentNullException(nameof(request), "Request filter can not be null");
        }

        var query = _context.IncomeTax.Select(m => new IncomeTax
        {
            Id = m.Id,
            TypeName = m.TypeName,
            FlatRate = m.FlatRate,
            FlatValue = m.FlatValue == null? null: new FlatValueIncomeTax
            {
                Amount = m.FlatValue.Amount,
                Threshold = m.FlatValue.Threshold,
                ThresholdRate = m.FlatValue.ThresholdRate
            },
            Version = m.Version
        });

        var response = await PageList<IncomeTax>.CreateAsync(
            query,
            request.Model.PageNumber,
            request.Model.Limit);

        var result = new Response<PageList<IncomeTax>>(
            response,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}