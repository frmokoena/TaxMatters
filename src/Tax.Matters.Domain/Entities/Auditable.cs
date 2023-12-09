using System.ComponentModel.DataAnnotations;

namespace Tax.Matters.Domain.Entities;

public abstract class Auditable : Base
{
    [Display(Name = "Date Created")]
    public DateTime DateCreated { get; set; }

    [Display(Name = "Date Updated")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
    public DateTime DateUpdated { get; set; }
}
