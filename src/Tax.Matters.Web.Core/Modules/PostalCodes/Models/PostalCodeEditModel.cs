using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.Web.Core.Modules.PostalCodes.Models;

public class PostalCodeEditModel
{
    [Required(ErrorMessage = "Required")]
    public string Code { get; set; } = default!;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Tax Calculation Type")]
    public string IncomeTaxId { get; set; } = default!;

    [Required]
    public byte[] Version { get; set; } = default!;
}