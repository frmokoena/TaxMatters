namespace Tax.Matters.Domain.Entities;

public class AuditLog
{
    public string Id { get; set; } = default!;
    public string? UserId { get; set; }
    public DateTime EventDate { get; set; }
    public string EventType { get; set; } = default!;
    public string TableName { get; set; } = default!;
    public string RecordId { get; set; } = default!;
    public string ColumnName {  get; set; } = default!;
    public string? OriginalValue {  get; set; }
    public string? NewValue { get; set; }
}
