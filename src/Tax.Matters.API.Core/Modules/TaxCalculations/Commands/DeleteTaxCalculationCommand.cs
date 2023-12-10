using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Commands;

/// <summary>
/// Initializes a new instance of <see cref="DeleteTaxCalculationCommand"/> command class
/// </summary>
/// <param name="id"></param>
public class DeleteTaxCalculationCommand(string id) : IRequest<IResponse<TaxCalculation>>
{
    public string Id { get; } = id;
}
