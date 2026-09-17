using TicketSystem.Common.Entity;
using TicketSystem.Common.Model.Department;
using TicketSystem.Feature.Employee.Model;

namespace TicketSystem.Feature.Teams.Models;

public class TeamProfile :AuditableEntity 
{
    public int TeamId { get; set; }
    public int ManagerId {get ;set;}
    public string? TeamName {get; set;}
    public int DepartmentId {get ;set;}
    public EmployeeProfile?  Employee {get ;set;}
    public DepartmentProfile? Department {get ;set;}
}