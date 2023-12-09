using MediatR;
using Tax.Matters.API.Core.Modules.TaxCalculations.Models;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Commands;

public class CalculateTaxCommand(TaxCalculationRequestModel model) : IRequest<IResponse<TaxCalculation>>
{
    public TaxCalculationRequestModel Model { get; } = model;
}
