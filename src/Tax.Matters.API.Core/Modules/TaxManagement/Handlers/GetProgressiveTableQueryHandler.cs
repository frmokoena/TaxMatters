using MediatR;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxManagement.Queries;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.TaxManagement.Handlers;

public class GetProgressiveTableQueryHandler(AppDbContext context) : IRequestHandler<GetProgressiveTableQuery, IResponse<PageList<ProgressiveIncomeTax>>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<PageList<ProgressiveIncomeTax>>> Handle(
        GetProgressiveTableQuery request, CancellationToken cancellationToken)
    {
        if (request.Model == null)
        {
            throw new ArgumentNullException(nameof(request), "Request filter can not be null");
        }

        if (string.IsNullOrWhiteSpace(request.Model.IncomeTaxId))
        {
            throw new ArgumentNullException(nameof(request), "Tax Id can not be empty");
        }

        var query = _context.ProgressiveIncomeTax.Where(m => m.IncomeTaxId == request.Model.IncomeTaxId).OrderBy(m => m.MinimumIncome).Select(m => new ProgressiveIncomeTax
        {
            Id = m.Id,
            Rate = m.Rate,
            MinimumIncome = m.MinimumIncome,
            MaximumIncome = m.MaximumIncome,
            Version = m.Version
        });

        var response = await PageList<ProgressiveIncomeTax>.CreateAsync(
            query,
            request.Model.PageNumber,
            request.Model.Limit);

        var result = new Response<PageList<ProgressiveIncomeTax>>(
            response,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}