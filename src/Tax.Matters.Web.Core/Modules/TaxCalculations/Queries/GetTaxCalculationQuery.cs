using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Web.Core.Modules.TaxCalculations.Queries;

/// <summary>
/// Intializes a new instance of the <see cref="GetTaxCalculationQuery"/> query class
/// </summary>
/// <param name="id"></param>
public class GetTaxCalculationQuery(string id) : IRequest<IResponse<TaxCalculation>>
{
    public string Id { get; } = id;
}
