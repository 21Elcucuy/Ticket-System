using System.Data;
using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Common.Entity.Results;
using TicketSystem.Infrastructure.Persistence;
using Wolverine.Http;

namespace TicketSystem.Feature.Teams.AddToTeam;

public record AddToTeamCommand(int EmployeeId ,int TeamId );

public class AddToTeamValidator : AbstractValidator<AddToTeamCommand>
{
    public AddToTeamValidator()
    {
        RuleFor(x => x.EmployeeId).NotNull().WithMessage("Employee Id is Required");
        RuleFor(x => x.TeamId).NotNull().WithMessage("Team Id is required");
    }

}

public class AddToTeamEndpoint(AppDbContext context)
{
    private readonly AppDbContext context = context;

    public async Task<ProblemDetails> ValidateAsync(AddToTeamCommand command , CancellationToken ct)
    {
        if(await context.Employees.AnyAsync(x => x.EmployeeId ==command.EmployeeId && x.TeamId == command.TeamId))
        {
            return new ProblemDetails()
            {
                Title="Cant Add Employee",
                Detail = "The Employee is Already in the Team",
                Status =  StatusCodes.Status409Conflict,

            };
        }
        if(! await context.Employees.AnyAsync(x => x.EmployeeId == command.EmployeeId,ct))
        {
            return new ProblemDetails()
            {
                Title="Invalid EmployeeId",
                Detail = "Employeee Id is Not Found",
                Status =  StatusCodes.Status404NotFound,

            };
        }
           if(! await context.Team.AnyAsync(x => x.TeamId == command.TeamId,ct))
        {
            return new ProblemDetails()
            {
                Title="Invalid TeamId",
                Detail = "Team Id is Not Found",
                Status =  StatusCodes.Status404NotFound,

            };
        }
        return WolverineContinue.NoProblems;
    }
     [WolverinePut("api/team/member")]
     [AllowAnonymous]
    public async Task<Results<Ok,BadRequest<string>>> Handle(AddToTeamCommand command , CancellationToken ct)
    {
      var employee = await context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == command.EmployeeId,ct);
      employee!.TeamId =command.TeamId;
        context.Employees.Update(employee);
        if(await context.SaveChangesAsync(ct) > 0)
        {
            return TypedResults.Ok();
        }
        return TypedResults.BadRequest("Something Went Wrong");

    }
 
}