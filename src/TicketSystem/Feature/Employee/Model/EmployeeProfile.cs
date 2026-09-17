using TicketSystem.Common.Entity;
using TicketSystem.Common.Entity.Enum;
using TicketSystem.Common.Model;
using TicketSystem.Common.Model.Department;
using TicketSystem.Feature.Teams.Models;
using TicketSystem.Feature.Ticket.Model;

namespace TicketSystem.Feature.Employee.Model;

public class EmployeeProfile  : AuditableEntity
{
    
    public int EmployeeId {get ;set;} 
    
    public int? TeamId {get; set;}
    public Role role {get ;set;}
    public virtual TeamProfile? Team {get; set;}

    public virtual AppUser? AppUser {get; set;}

}