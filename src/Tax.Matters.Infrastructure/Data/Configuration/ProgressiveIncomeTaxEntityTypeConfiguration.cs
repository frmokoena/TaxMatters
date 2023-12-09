using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data.Configuration;

public class ProgressiveIncomeTaxEntityTypeConfiguration : IEntityTypeConfiguration<ProgressiveIncomeTax>
{
    public void Configure(EntityTypeBuilder<ProgressiveIncomeTax> builder)
    {
        builder
            .Property(m => m.MinimumIncome)
            .HasPrecision(18, 2);

        builder
            .Property(m => m.MaximumIncome)
            .HasPrecision(18, 2);

        builder
            .Property(m => m.Rate)
            .HasPrecision(5, 2);

        builder
            .HasOne(m => m.IncomeTax)
            .WithMany(m => m.ProgressiveTaxTable)
            .HasForeignKey(m => m.IncomeTaxId);

        builder
            .HasIndex(m => new { m.MinimumIncome, m.IncomeTaxId })
            .IsUnique();

        builder
            .ToTable(b => b.HasCheckConstraint("CK_ProgressiveIncomeTax_Rate", "[Rate] <= 100.00"));

        builder
            .ToTable(b => b.HasCheckConstraint("CK_ProgressiveIncomeTax_MinimumIncome_vs_MaximumIncome", "[MaximumIncome] IS NULL OR ([MinimumIncome] < [MaximumIncome])"));
    }
}