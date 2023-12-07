using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data.Configuration;

public class PostalCodeEntityTypeConfiguration : IEntityTypeConfiguration<PostalCode>
{
    public void Configure(EntityTypeBuilder<PostalCode> builder)
    {
        builder
            .Property(m => m.Code)
            .HasMaxLength(256);
    }
}
