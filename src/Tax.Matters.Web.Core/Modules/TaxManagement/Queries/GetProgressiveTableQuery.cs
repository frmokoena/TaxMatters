using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;

namespace Tax.Matters.Web.Core.Modules.TaxManagement.Queries;

public class GetProgressiveTableQuery(
    string incomeTaxId,
    int pageNumber = 1,
    int limit = 10) : IRequest<IResponse<PageListDto<ProgressiveIncomeTax>>>
{
    public string IncomeTaxId { get; } = incomeTaxId;
    public int PageNumber { get; } = pageNumber;
    public int Limit { get; } = limit;
}