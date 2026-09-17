namespace TicketSystem.Common.Dtos;

public sealed record EmployeeDTO
{
    public int EmployeeId {get;set;}
    public string? EmployeeName { get; set; }
}