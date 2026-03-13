namespace TicketSystem.Common.Entity;

public sealed class AuditableEntity :Entity
{
    public DateTime CreatedAtUtc{get; set;}
    public DateTime LastModifiedAtUtc {get; set;}
    public string CreatedBy {get; set;} = string.Empty;
    
    
    
}