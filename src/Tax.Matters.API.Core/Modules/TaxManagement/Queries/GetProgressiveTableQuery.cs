using MediatR;
using Tax.Matters.API.Core.Modules.TaxManagement.Models;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.TaxManagement.Queries;

/// <summary>
/// Initializes a new instance of the <see cref="GetProgressiveTableQuery"/> query class
/// </summary>
/// <param name="model"></param>
public class GetProgressiveTableQuery(ProgressiveTableFilteringModel model) : IRequest<IResponse<PageList<ProgressiveIncomeTax>>>
{
    public ProgressiveTableFilteringModel Model { get; } = model;
}
