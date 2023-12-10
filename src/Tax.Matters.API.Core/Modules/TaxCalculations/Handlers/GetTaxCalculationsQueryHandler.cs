using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Net;
using Tax.Matters.API.Core.Extensions;
using Tax.Matters.API.Core.Modules.TaxCalculations.Queries;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="GetTaxCalculationsQueryHandler"/> handler class
/// </summary>
/// <param name="context"></param>
public class GetTaxCalculationsQueryHandler(AppDbContext context) : IRequestHandler<GetTaxCalculationsQuery, IResponse<PageList<TaxCalculation>>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<PageList<TaxCalculation>>> Handle(
        GetTaxCalculationsQuery request, CancellationToken cancellationToken)
    {
        if (request.Model == null)
        {
            throw new ArgumentNullException(nameof(request), "Request filter can not be null");
        }

        IQueryable<TaxCalculation> query = _context.TaxCalculation;

        Expression<Func<TaxCalculation, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.Model.Keyword))
        {
            predicate = DefaultQuery.True<TaxCalculation>();

            var keyword = Uri.UnescapeDataString(request.Model.Keyword);

            predicate = predicate.And(m => EF.Functions.Like(m.PostalCode.Code, $"%{keyword}%"));
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        string? sortOrder = request.Model.SortOrder;

        query = sortOrder switch
        {
            "income_desc" => query.OrderByDescending(m => m.AnnualIncome),
            "Income" => query.OrderBy(m => m.AnnualIncome),
            "date_asc" => query.OrderBy(m => m.DateUpdated),
            _ => query.OrderByDescending(s => s.DateUpdated),
        };

        IQueryable<TaxCalculation> resultQuery = query.Select(m => new TaxCalculation
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
        });

        var response = await PageList<TaxCalculation>.CreateAsync(
            resultQuery,
            request.Model.PageNumber,
            request.Model.Limit);

        var result = new Response<PageList<TaxCalculation>>(
            response,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}