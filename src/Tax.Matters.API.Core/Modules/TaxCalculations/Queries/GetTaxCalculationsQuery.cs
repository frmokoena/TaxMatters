using MediatR;
using Tax.Matters.API.Core.Modules.TaxCalculations.Models;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Queries;

public class GetTaxCalculationsQuery(TaxCalculationsFilteringModel model) : IRequest<IResponse<PageList<TaxCalculation>>>
{
    public TaxCalculationsFilteringModel Model { get; } = model;
}
