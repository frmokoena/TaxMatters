using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.Domain.Entities;

public class TaxCalculation : Auditable
{
    [Display(Name = "Annual Income")]
    [DisplayFormat(DataFormatString = "{0:### ### ###}")]
    public decimal AnnualIncome { get; set; }

    [Display(Name = "Tax Amount")]
    [DisplayFormat(DataFormatString = "{0:### ### ###.#0}")]
    public decimal TaxAmount { get; set; }
    
    public string PostalCodeId { get; set; } = default!;

    [Display(Name = "Postal Code")]
    public PostalCode PostalCode { get; set; } = default!;
}
