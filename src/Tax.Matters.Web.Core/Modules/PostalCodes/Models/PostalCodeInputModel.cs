using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.Web.Core.Modules.PostalCodes.Models
{
    public class PostalCodeInputModel
    {
        [Required(ErrorMessage = "Required")]
        public string Code { get; set; } = default!;

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Tax Calculation Type")]
        public string IncomeTaxId { get; set; } = default!;
    }
}
