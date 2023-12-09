using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations.Schema;
using Tax.Matters.Client.Extensions;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data;

/// <summary>
/// Initializes a new instance of the <see cref="AppDbContext"/> context
/// </summary>
/// <param name="options"></param>
/// <param name="httpContext"></param>
public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IHttpContextAccessor httpContext) : DbContext(options)
{
    private readonly IHttpContextAccessor? _httpContext = httpContext;

    public DbSet<PostalCode> PostalCode => Set<PostalCode>();
    public DbSet<IncomeTax> IncomeTax => Set<IncomeTax>();
    public DbSet<FlatValueIncomeTax> FlatValueIncomeTax => Set<FlatValueIncomeTax>();
    public DbSet<ProgressiveIncomeTax> ProgressiveIncomeTax => Set<ProgressiveIncomeTax>();
    public DbSet<TaxCalculation> TaxCalculation => Set<TaxCalculation>();

    public DbSet<AuditLog> AuditLog => Set<AuditLog>();

    public override int SaveChanges()
    {
        AddAuditTrail();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddAuditTrail();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private void AddAuditTrail()
    {
        // Get all Added/Deleted/Modified entities (ingore Unmodified or Detached)
        foreach (var ent in ChangeTracker.Entries().Where<EntityEntry>(p => p.Entity is Auditable t && (p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified)).ToList())
        {
            // For each changed record, get the audit record entries and add them
            foreach (AuditLog x in GetAuditRecordsForChange(ent /* , userId *TODO: get user id from httpContext */))
            {
                AuditLog.Add(x);
            }
        }
    }

    private List<AuditLog> GetAuditRecordsForChange(EntityEntry dbEntry, string? userId = null)
    {
        List<AuditLog> result = [];

        DateTime auditTime = DateTime.Now;

        // Get table name
        string tableName = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault() is TableAttribute tableAttr ? tableAttr.Name : dbEntry.Entity.GetType().Name;
              
        string keyValue             
            = string.Join(", ",dbEntry.Properties.Where(m => m.Metadata.IsPrimaryKey()).Select(m => $"{m.Metadata.Name}:{m.CurrentValue?.ToString()}"));

        if (dbEntry.State == EntityState.Added)
        {
           if (dbEntry.Entity is Auditable auditable)
            {
                auditable.DateCreated = auditTime;
                auditable.DateUpdated = auditTime;
            }

            // For Inserts, just add the whole record
            foreach (var property in dbEntry.CurrentValues.Properties)
            {
                var current = dbEntry.CurrentValues[property.Name]?.ToString();

                result.Add(new AuditLog()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    EventDate = auditTime,
                    EventType = "Added",
                    TableName = tableName,
                    RecordId = keyValue,
                    ColumnName = property.Name,
                    NewValue = current
                });
            }
        }
        else if (dbEntry.State == EntityState.Deleted)
        {
            // Same with deletes, do the whole record
            result.Add(new AuditLog()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                EventDate = auditTime,
                EventType = "Deleted",
                TableName = tableName,
                RecordId = keyValue,
                ColumnName = "*ALL",
                NewValue = dbEntry.OriginalValues.ToObject()?.ToJsonString()
            }); ;
        }
        else if (dbEntry.State == EntityState.Modified)
        {
            if (dbEntry.Entity is Auditable auditable)
            {
                auditable.DateUpdated = auditTime;
            }

            foreach (var property in dbEntry.OriginalValues.Properties)
            {
                // For updates, we only want to capture the columns that actually changed
                var current = dbEntry.CurrentValues[property.Name];
                var original = dbEntry.OriginalValues[property.Name];

                if (!Equals(current, original))
                {
                    result.Add(new AuditLog()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        EventDate = auditTime,
                        EventType = "Modified",
                        TableName = tableName,
                        RecordId = keyValue,
                        ColumnName = property.Name,
                        OriginalValue = original?.ToString(),
                        NewValue = current?.ToString()
                    });                    
                }
            }
        }

        // Otherwise, nothing changed

        return result;
    }
}
