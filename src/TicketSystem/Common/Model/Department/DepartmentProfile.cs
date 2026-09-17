using TicketSystem.Common.Entity;
using TicketSystem.Feature.Employee.Model;

namespace TicketSystem.Common.Model.Department;

public class DepartmentProfile : AuditableEntity
{
    public int DepartmentId { get; set; }
    public string? Name {get ; set;}

    public string? Location {get ;set;}
    // public virtual List<EmployeeProfile>? Employee {get ; set ;}
}