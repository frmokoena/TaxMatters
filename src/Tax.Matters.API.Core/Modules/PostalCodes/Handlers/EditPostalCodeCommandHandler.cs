using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tax.Matters.API.Core.Modules.PostalCodes.Commands;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="EditPostalCodeCommandHandler"/> handler class
/// </summary>
/// <param name="context"></param>
public class EditPostalCodeCommandHandler(AppDbContext context) : IRequestHandler<EditPostalCodeCommand, IResponse<PostalCode>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<PostalCode>> Handle(
        EditPostalCodeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            throw new ArgumentNullException(nameof(request), "Request id can not be null");
        }

        if (request.Model == null)
        {
            throw new ArgumentNullException(nameof(request), "Request filter can not be null");
        }

        var entity = new PostalCode
        {
            Id = request.Id,
            Version = request.Model.Version
        };

        _context.PostalCode.Attach(entity);

        entity.Code = request.Model.Code;
        entity.IncomeTaxId = request.Model.IncomeTaxId;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return new Response<PostalCode>(
                entity,
                raw: string.Empty,
                HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateConcurrencyException)
            {
                return new Response<PostalCode>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Entity was updated since it was last requested");
            }
            else if (ex.InnerException?.Message?.Contains("duplicate", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                return new Response<PostalCode>(
                    raw: null,
                    HttpStatusCode.Conflict,
                    reason: "Entity exists");
            }

            return new Response<PostalCode>(
                raw: null,
                HttpStatusCode.InternalServerError,
                reason: "Failed to save entity");
        }

    }
}