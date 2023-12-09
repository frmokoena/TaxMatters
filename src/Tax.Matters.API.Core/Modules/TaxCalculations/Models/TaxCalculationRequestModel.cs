using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Models;

public class TaxCalculationRequestModel
{
    [Required(ErrorMessage = "Required")]
    public string PostalCode { get; set; } = default!;
    public decimal AnnualIncome { get; set; }
}
