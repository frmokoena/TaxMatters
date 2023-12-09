using MediatR;
using Tax.Matters.API.Core.Modules.TaxManagement.Models;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.TaxManagement.Queries;

public class GetTaxCalculationTypesQuery(TaxCalculationTypesFilteringModel model) : IRequest<IResponse<PageList<IncomeTax>>>
{
    public TaxCalculationTypesFilteringModel Model { get; } = model;
}
