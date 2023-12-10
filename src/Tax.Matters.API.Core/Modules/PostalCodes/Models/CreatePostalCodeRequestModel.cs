using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Models;

public class CreatePostalCodeRequestModel
{
    [Required(ErrorMessage = "Required")]
    public string Code { get; set; } = default!;

    [Required(ErrorMessage = "Required")]
    public string IncomeTaxId { get; set; } = default!;
}