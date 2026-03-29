using TicketSystem.Common.Entity;

namespace TicketSystem.Feature.Employee.Model;

public class Department : AuditableEntity
{
    public int DepartmentId { get; set; }
    public string? Name {get ; set;}

    public virtual EmployeeProfile? Employee {get ; set ;}
}