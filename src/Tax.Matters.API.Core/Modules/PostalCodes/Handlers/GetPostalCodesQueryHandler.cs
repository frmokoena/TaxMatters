using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using Tax.Matters.API.Core.Extensions;
using Tax.Matters.API.Core.Modules.PostalCodes.Queries;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Handlers;

public class GetPostalCodesQueryHandler(AppDbContext context) : IRequestHandler<GetPostalCodesQuery, IResponse<PageList<PostalCode>>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<PageList<PostalCode>>> Handle(
        GetPostalCodesQuery request, CancellationToken cancellationToken)
    {
        if (request.Model == null)
        {
            throw new ArgumentNullException(nameof(request), "Request filter can not be null");
        }

        IQueryable<PostalCode> query = _context.PostalCode;

        Expression<Func<PostalCode, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.Model.Keyword))
        {
            predicate = DefaultQuery.True<PostalCode>();

            var keyword = Uri.UnescapeDataString(request.Model.Keyword);

            predicate = predicate.And(m => EF.Functions.Like(m.Code, $"%{keyword}%"));
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        IQueryable<PostalCode> resultQuery = query.Select(m => new PostalCode
        {
            Id = m.Id,
            Code = m.Code,
            IncomeTax = new IncomeTax
            {
                TypeName = m.IncomeTax.TypeName
            },
            Version = m.Version
        });

        var response = await PageList<PostalCode>.CreateAsync(
            resultQuery,
            request.Model.PageNumber,
            request.Model.Limit);

        var result = new Response<PageList<PostalCode>>(
            response,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}