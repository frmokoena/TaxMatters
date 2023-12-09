using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data.Configuration;

public class IncomeTaxEntityTypeConfiguration : IEntityTypeConfiguration<IncomeTax>
{
    public void Configure(EntityTypeBuilder<IncomeTax> builder)
    {
        builder
            .Property(m => m.TypeName)
            .HasMaxLength(256)
            .HasConversion<string>();

        builder
            .Property(m => m.FlatRate)
            .HasPrecision(5, 2);

        builder
            .ToTable(b => b.HasCheckConstraint("CK_IncomeTax_FlatRate", "[FlatRate] <= 100.00"));
    }
}