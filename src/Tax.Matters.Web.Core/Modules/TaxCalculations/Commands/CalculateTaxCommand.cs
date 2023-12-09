using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Models;

namespace Tax.Matters.Web.Core.Modules.TaxCalculations.Commands
{
    /// <summary>
    /// Intializes a new instance of the <see cref="CalculateTaxCommand"/> command class
    /// </summary>
    /// <param name="model"></param>
    public class CalculateTaxCommand(TaxCalculationInputModel model) : IRequest<IResponse<TaxCalculation>>
    {
        public TaxCalculationInputModel Model { get; } = model;
    }
}
