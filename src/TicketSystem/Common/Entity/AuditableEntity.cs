namespace TicketSystem.Common.Entity;

public class AuditableEntity :Entity
{
    public DateTime CreatedAtUtc{get; set;}
    public DateTime LastModifiedAtUtc {get; set;}
    public int CreatedBy {get; set;} 
    
    public AuditableEntity(int Id)
    {
        CreatedAtUtc = DateTime.UtcNow;
        LastModifiedAtUtc = DateTime.UtcNow;
        CreatedBy = Id;
    }
    
}