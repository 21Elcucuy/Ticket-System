using TicketSystem.Common.Entity;
using TicketSystem.Feature.Employee.Model;

namespace TicketSystem.Feature.Ticket.Model;

public  class TicketProfile : AuditableEntity
{
    public int TicketId {get; set;}
    public TicketStatus TicketStatus {get; set;}
    public string? TicketSubject {get; set;}
    public string? FromEmail {get; set;}
    public int EmployeeId {get ; set;}
    public virtual EmployeeProfile? EmployeeProfile {get; set; }

}