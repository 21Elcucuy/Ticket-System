using TicketSystem.Common.Entity;
using TicketSystem.Common.Model;
using TicketSystem.Feature.Ticket.Model;

namespace TicketSystem.Feature.Employee.Model;

public class EmployeeProfile  : AuditableEntity
{
    
    public int EmployeeId {get ;set;} 
    public int DepartmentId {get ;set;}
    public double Salary {get; set;}
    public virtual Department? Department {get; set;}
    public virtual AppUser? AppUser {get; set;}
    public virtual TicketProfile? Ticket {get;set;}

}