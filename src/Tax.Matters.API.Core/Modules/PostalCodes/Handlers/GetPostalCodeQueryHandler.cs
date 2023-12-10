using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tax.Matters.API.Core.Modules.PostalCodes.Queries;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Handlers;

/// <summary>
/// Intializes a new instance of the <see cref="GetPostalCodeQueryHandler"/> handler class
/// </summary>
/// <param name="context"></param>
public class GetPostalCodeQueryHandler(AppDbContext context) : IRequestHandler<GetPostalCodeQuery, IResponse<PostalCode>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<PostalCode>> Handle(
        GetPostalCodeQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            throw new ArgumentNullException(nameof(request), "Request id can not be null");
        }

        var entity = await _context.PostalCode.Where(m => m.Id == request.Id).Select(m => new PostalCode
        {
            Id = m.Id,
            Code = m.Code,
            IncomeTaxId = m.IncomeTaxId,
            Version = m.Version
        }).FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            return new Response<PostalCode>(
                raw: null,
                HttpStatusCode.NotFound,
                reason: "Entity not found");
        }

        return new Response<PostalCode>(
            entity,
            raw: string.Empty,
            HttpStatusCode.OK);
    }
}