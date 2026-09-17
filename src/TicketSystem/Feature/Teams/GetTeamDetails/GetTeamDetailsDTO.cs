using TicketSystem.Common.Dtos;

namespace TicketSystem.Feature.Teams.GetTeamDetails;

public  sealed record GetTeamDetailsDTO
{
    public int TeamId { get; set; }
    public int ManagerId {get ;set;}
    public string? ManagerName {get;set;}
    public string? TeamName {get; set;}
    public int DepartmentId {get ;set;}
    public List<EmployeeDTO> Employee {get;set;} = new();


}