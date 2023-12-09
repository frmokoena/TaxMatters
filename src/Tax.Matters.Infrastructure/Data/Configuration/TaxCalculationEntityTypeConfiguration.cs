using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data.Configuration;

public class TaxCalculationEntityTypeConfiguration : IEntityTypeConfiguration<TaxCalculation>
{
    public void Configure(EntityTypeBuilder<TaxCalculation> builder)
    {
        builder
            .Property(m => m.AnnualIncome)
            .HasPrecision(18, 2);

        builder
            .Property(m => m.TaxAmount)
            .HasPrecision(18, 2);

        builder
            .HasOne(m => m.PostalCode)
            .WithMany()
            .HasForeignKey(m => m.PostalCodeId);

        builder
            .ToTable(b => b.HasCheckConstraint("CK_TaxCalculation_AnnualIncome", "[AnnualIncome] >= 0.00"));
    }
}
