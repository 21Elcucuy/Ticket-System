using Microsoft.AspNetCore.Identity;
using TicketSystem.Common.Entity;
using TicketSystem.Common.Entity.Enum;
using TicketSystem.Feature.Employee.Model;
namespace TicketSystem.Common.Model ; 

public class AppUser : IdentityUser<int> ,IAuditableEntity
{
    public string? FirstName {get ;set;}
    public string? LastName {get ;set; }  
    public string FullName => FirstName + " " + LastName;
    public Role Role {get;set;} 
    public DateTime CreatedAtUtc {get ;set; } 
    public DateTime LastModifiedAtUtc {get ;set; } 
    public int? CreatedBy {get ;set; }
    public int? LastModifiedBy {get ;set; }
    public bool IsDeleted {get ;set;}
    public bool IsActive {get; set;}

    public List<string> Roles =  [] ;
   
    public virtual EmployeeProfile? Employee {get ;set ;}


}
