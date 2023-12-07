using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data.Configuration;

public class AuditLogEntityTypeConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder
            .HasKey(e => e.Id);
    }
}