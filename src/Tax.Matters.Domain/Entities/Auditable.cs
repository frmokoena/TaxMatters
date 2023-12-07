namespace Tax.Matters.Domain.Entities;

public abstract class Auditable : Base
{
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}
