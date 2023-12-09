using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;

namespace Tax.Matters.Web.Core.Modules.TaxCalculations.Queries;

public class GetTaxCalculationsQuery(
    string? keyword,
    string? sortOrder,
    int pageNumber = 1,
    int limit = 10) : IRequest<IResponse<PageListDto<TaxCalculation>>>
{
    public int PageNumber { get; } = pageNumber;
    public int Limit { get; } = limit;
    public string? Keyword { get; } = keyword;
    public string? SortOrder { get; } = sortOrder;
}
