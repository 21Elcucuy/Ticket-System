using FluentValidation;
using JasperFx.Events.Daemon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using TicketSystem.Feature.Teams.Models;
using TicketSystem.Infrastructure.Persistence;
using Wolverine.Http;

namespace TicketSystem.Feature.Teams.CreateTeam;


public record CreateTeamCommand(string TeamName,int ManagerId ,int DepartmentId);

public class CreateTeamValidation :AbstractValidator<CreateTeamCommand> 
{
    public CreateTeamValidation()
    {
        RuleFor(x => x.DepartmentId).NotNull();
        RuleFor(x => x.ManagerId).NotNull();
        RuleFor(x => x.TeamName).NotNull();
    }
}
public class CreateTeamEndPoint(AppDbContext context)
{
    private readonly AppDbContext context = context;

    public  async Task<ProblemDetails> ValidateAsync(CreateTeamCommand command,CancellationToken ct = default)
    {
        if(!await context.Employees.AnyAsync(x => x.EmployeeId == command.ManagerId))
        {
            return new ProblemDetails
            {
                Title= "Invalid Manager Id ",
                Detail= "Wrong Manager Id",
                Status = StatusCodes.Status404NotFound,
            };
        }
         if(!await context.Departments.AnyAsync(x => x.DepartmentId == command.DepartmentId))
        {
            return new ProblemDetails
            {
                Title= "Invalid Department Id",
                Detail= "Wrong Department Id",
                Status = StatusCodes.Status404NotFound,
            };
        }
           if(await context.Team.AnyAsync(x => x.TeamName == command.TeamName))
        {
            return new ProblemDetails
            {
                Title= "Invalid Team Name ",
                Detail= "Team Name is Already taken",
                Status = StatusCodes.Status400BadRequest,
            };
        } 
         return WolverineContinue.NoProblems;
    }
     [WolverinePost("api/team")]
      [AllowAnonymous]
     public async Task<Ok> Handle(CreateTeamCommand command , CancellationToken ct )
     {
        var Team = new TeamProfile
        {
            ManagerId = command.ManagerId,
            DepartmentId = command.DepartmentId,
            TeamName = command.TeamName
        };
         context.Team.Add(Team);
         await context.SaveChangesAsync(ct); 
         return TypedResults.Ok();

     }

}