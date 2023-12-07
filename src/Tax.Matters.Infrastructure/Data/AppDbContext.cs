using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IHttpContextAccessor? httpContext = null) : DbContext(options)
{
    private readonly IHttpContextAccessor? _httpContext = httpContext;

    public DbSet<PostalCode> PostalCode => Set<PostalCode>();
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

    private void AddAuditTrail()
    {
        // Get all Added/Deleted/Modified entities (ingore Unmodified or Detached)
        foreach (var ent in base.ChangeTracker.Entries().Where<EntityEntry>(p => p.Entity is not Domain.Entities.AuditLog && (p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified)))
        {
            // For each changed record, get the audit record entries and add them
            foreach (AuditLog x in GetAuditRecordsForChange(ent /* , userId *TODO: get user id from httpContext */))
            {
                AuditLog.Add(x);
            }
        }
    }

    private List<AuditLog> GetAuditRecordsForChange(EntityEntry dbEntry, string userId = null)
    {
        List<AuditLog> result = [];

        DateTime auditTime = DateTime.Now;

        // Get table name
        string tableName = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), true).SingleOrDefault() is TableAttribute tableAttr ? tableAttr.Name : dbEntry.Entity.GetType().Name;

        // Get primary key value (If you have more than one key column, this will need to be adjusted)
        var keyNames = dbEntry.Entity.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0).ToList();

        string keyName = keyNames[0].Name;

        if (dbEntry.State == EntityState.Added)
        {
            // For Inserts, just add the whole record
            foreach (var propertyName in dbEntry.CurrentValues.Properties)
            {
                result.Add(new AuditLog()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    EventDate = auditTime,
                    EventType = "A",    // Added
                    TableName = tableName,
                    RecordId = dbEntry.CurrentValues.GetValue<object>(keyName).ToString()!,
                    ColumnName = propertyName.Name,
                    NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName)?.ToString()
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
                EventType = "D", // Deleted
                TableName = tableName,
                RecordId = dbEntry.OriginalValues.GetValue<object>(keyName).ToString()!,
                ColumnName = "*ALL",
                NewValue = dbEntry.OriginalValues.ToObject()?.ToString()
            });
        }
        else if (dbEntry.State == EntityState.Modified)
        {
            foreach (var propertyName in dbEntry.OriginalValues.Properties)
            {
                // For updates, we only want to capture the columns that actually changed
                if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                {
                    result.Add(new AuditLog()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        EventDate = auditTime,
                        EventType = "M",    // Modified
                        TableName = tableName,
                        RecordId = dbEntry.OriginalValues.GetValue<object>(keyName).ToString()!,
                        ColumnName = propertyName.Name,
                        OriginalValue = dbEntry.OriginalValues.GetValue<object>(propertyName)?.ToString(),
                        NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName)?.ToString()
                    });
                }
            }
        }

        // Otherwise, nothing changed

        return result;
    }
}
