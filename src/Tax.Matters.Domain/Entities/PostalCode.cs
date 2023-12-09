using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.Domain.Entities;

/// <summary>
/// Postal Code enitity
/// </summary>
public class PostalCode : Base
{
    [Display(Name = "Code")]
    public string Code { get; set; } = default!;
    public string IncomeTaxId { get; set; } = default!;

    [Display(Name = "Tax Calculation Type")]
    public IncomeTax IncomeTax { get; set; } = default!;
}
