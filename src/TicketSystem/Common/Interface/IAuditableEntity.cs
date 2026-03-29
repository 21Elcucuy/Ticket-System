

public interface IAuditableEntity
{
    public DateTime CreatedAtUtc{get; set;}
    public DateTime LastModifiedAtUtc {get; set;}
    public int ? CreatedBy {get; set;}  
    public int ? LastModifiedBy {get;set;} 
    public bool IsDeleted  {get;set;} 
    public bool IsActive  {get;set;} 

}