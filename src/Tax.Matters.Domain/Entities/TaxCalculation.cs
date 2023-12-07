using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.Domain.Entities;

public class TaxCalculation : Auditable
{
    [Display(Name = "Postal Code")]
    public string PostalCodeId { get; set; } = default!;
    public PostalCode PostalCode { get; set; } = default!;

    [Display(Name = "Annual Income")]
    public decimal AnnualIncome { get; set; }

    [Display(Name = "Tax Amount")]
    public decimal TaxAmount { get; set;}
}
