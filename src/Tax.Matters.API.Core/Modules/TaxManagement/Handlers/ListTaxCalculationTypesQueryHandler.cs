using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxManagement.Queries;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.TaxManagement.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="ListTaxCalculationTypesQueryHandler"/> handler class
/// </summary>
/// <param name="client"></param>
/// <param name="optionsAccessor"></param>
public class ListTaxCalculationTypesQueryHandler(AppDbContext context) : IRequestHandler<ListTaxCalculationTypesQuery, IResponse<IEnumerable<IncomeTax>>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<IEnumerable<IncomeTax>>> Handle(
        ListTaxCalculationTypesQuery request, CancellationToken cancellationToken)
    {
        var response = await _context.IncomeTax.Select(m => new IncomeTax
        {
            Id = m.Id,
            TypeName = m.TypeName
        }).ToListAsync(cancellationToken: cancellationToken);

        var result = new Response<IEnumerable<IncomeTax>>(
            response,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}
