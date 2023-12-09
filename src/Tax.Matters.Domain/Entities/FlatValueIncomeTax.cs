using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.Domain.Entities;

/// <summary>
/// Models the flat value tax calculation type
/// </summary>
public class FlatValueIncomeTax : Auditable
{
    [Display(Name = "Annual Tax")]
    public decimal Amount {  get; set; }

    [Display(Name = "Threshold Income")]
    public decimal Threshold {  get; set; }

    [Display(Name = "Threshold Rate")]
    public decimal ThresholdRate { get; set; }
    public string IncomeTaxId { get; set; } = default!;
    public IncomeTax IncomeTax { get; set; } = default!;
}
