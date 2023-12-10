using System.ComponentModel.DataAnnotations;
using Tax.Matters.Domain.Enums;

namespace Tax.Matters.Domain.Entities
{
    /// <summary>
    /// Models the tax entity
    /// </summary>
    public class IncomeTax : Auditable
    {
        [Display(Name = "Tax Calculation Type")]
        public TaxCalculationType TypeName { get; set; }

        [Display(Name = "Flat Rate")]
        public decimal? FlatRate { get; set; }
        public FlatValueIncomeTax? FlatValue { get; set; }
        public ICollection<ProgressiveIncomeTax> ProgressiveTaxTable { get; set; } = [];
    }
}
