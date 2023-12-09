using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data.Configuration;

public class FlatValueIncomeTaxEntityTypeConfiguration : IEntityTypeConfiguration<FlatValueIncomeTax>
{
    public void Configure(EntityTypeBuilder<FlatValueIncomeTax> builder)
    {
        builder
            .Property(m => m.Amount)
            .HasPrecision(18, 2);

        builder
            .Property(m => m.Threshold)
            .HasPrecision(18, 2);

        builder
            .Property(m => m.ThresholdRate)
            .HasPrecision(5, 2);

        builder
            .HasOne(m => m.IncomeTax)
            .WithOne(m => m.FlatValue)
            .HasForeignKey<FlatValueIncomeTax>(m => m.IncomeTaxId);

        builder
            .ToTable(b => b.HasCheckConstraint("CK_FlatValueIncomeTax_ThresholdRate", "[ThresholdRate] <= 100.00"));
    }
}