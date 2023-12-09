using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.Domain.Entities;

public class ProgressiveIncomeTax : Auditable
{
    [Display(Name = "From")]
    public decimal MinimumIncome {  get; set; }

    [Display(Name = "To")]
    public decimal? MaximumIncome { get; set; }
    public decimal Rate {  get; set; }
    public string IncomeTaxId { get; set; } = default!;
    public IncomeTax IncomeTax { get; set; } = default!;
}
