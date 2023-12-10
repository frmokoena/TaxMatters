using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.Web.Core.Modules.TaxCalculations.Models;

/// <summary>
/// Class <c>TaxCalculationInputModel</c> models the request object for tax calculation
/// </summary>
public class TaxCalculationInputModel
{
    [Required(ErrorMessage = "Required")]
    [Display(Name = "Postal Code", Prompt = "Enter postal code..")]
    public string PostalCode { get; set; } = default!;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Annual Income", Prompt = "Enter annual taxable income...")]
    public decimal AnnualIncome {  get; set; }
}
