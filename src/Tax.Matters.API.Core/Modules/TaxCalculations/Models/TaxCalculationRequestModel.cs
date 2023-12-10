using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Models;

/// <summary>
/// Class <c>TaxCalculationRequestModel</c> models a request object to calculate the tax for the provided postal code and annual income
/// </summary>
public class TaxCalculationRequestModel
{
    [Required(ErrorMessage = "Required")]
    public string PostalCode { get; set; } = default!;
    public decimal AnnualIncome { get; set; }
}
