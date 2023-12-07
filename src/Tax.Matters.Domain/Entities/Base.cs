using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tax.Matters.Domain.Entities;

public abstract class Base
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = default!;

    // Concurrency token
    [Timestamp]
    public byte[] Version { get; set; } = default!;
}
