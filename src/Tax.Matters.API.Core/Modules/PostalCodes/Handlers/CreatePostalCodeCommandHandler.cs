using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tax.Matters.API.Core.Modules.PostalCodes.Commands;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="CreatePostalCodeCommandHandler"/> handler classes
/// </summary>
/// <param name="client"></param>
/// <param name="optionsAccessor"></param>
public class CreatePostalCodeCommandHandler(AppDbContext context) : IRequestHandler<CreatePostalCodeCommand, IResponse<PostalCode>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<PostalCode>> Handle(
        CreatePostalCodeCommand request, CancellationToken cancellationToken)
    {
        var entity = new PostalCode
        {
            Code = request.Model.Code,
            IncomeTaxId = request.Model.IncomeTaxId
        };

        _context.Add(entity);

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
            if (ex.InnerException?.Message?.Contains("duplicate", StringComparison.InvariantCultureIgnoreCase) ?? false)
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
