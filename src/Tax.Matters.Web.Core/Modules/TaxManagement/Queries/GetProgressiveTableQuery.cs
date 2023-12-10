using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;

namespace Tax.Matters.Web.Core.Modules.TaxManagement.Queries;

/// <summary>
/// Initializes a new instance of the <see cref="GetProgressiveTableQuery"/> query class
/// </summary>
/// <param name="incomeTaxId"></param>
/// <param name="pageNumber"></param>
/// <param name="limit"></param>
public class GetProgressiveTableQuery(
    string incomeTaxId,
    int pageNumber = 1,
    int limit = 10) : IRequest<IResponse<PageListDto<ProgressiveIncomeTax>>>
{
    public string IncomeTaxId { get; } = incomeTaxId;
    public int PageNumber { get; } = pageNumber;
    public int Limit { get; } = limit;
}