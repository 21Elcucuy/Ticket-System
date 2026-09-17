
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Common.Dtos;
using TicketSystem.Infrastructure.Persistence;
using Wolverine.Http;

namespace TicketSystem.Feature.Teams.GetTeamDetails;

// public record GetTeamDetailsQuery(
//    [FromRoute]
//     int TeamId
// );

// public class GetTeamDetailsValidation :AbstractValidator<GetTeamDetailsQuery> 
// {
//     public GetTeamDetailsValidation()
//     {
//         RuleFor(x => x.TeamId).NotNull();
//     }
// }
public class GetTeamDetailsEndpoint(AppDbContext context)
{
    private readonly AppDbContext context = context;

    public async Task<ProblemDetails> ValidateAsync(int id , CancellationToken ct)
    {
        if(! await context.Team.AnyAsync(x => x.TeamId == id))
        {
            return new ProblemDetails
            {
                Title ="Invalid TeamId",
                Detail ="Team Id is not Found",
                Status = StatusCodes.Status404NotFound,
            };
        }
        return WolverineContinue.NoProblems;

    }
    [WolverineGet("/api/team/{id}")]
    [AllowAnonymous]
    public async Task<Results<Ok<GetTeamDetailsDTO>, BadRequest<string>>> Handle([FromRoute]int id , CancellationToken ct)
    {
        var getteam = await context.Team.FirstOrDefaultAsync(x => x.TeamId == id,ct);
        
        var GetManagerName = await context.Users.FirstOrDefaultAsync(x => x.Id == getteam!.ManagerId);
        if(GetManagerName is null)
        {
            return TypedResults.BadRequest("Manager Is not Found");
        }
        var EmployeeList = await context.Employees.Where(t => t.TeamId == id).Select(x => new EmployeeDTO
        {
           EmployeeId = x.EmployeeId,
           EmployeeName = x.AppUser != null ? x.AppUser!.FullName :"",
        }).ToListAsync();
        var team = new GetTeamDetailsDTO()
        {
          TeamId= getteam!.TeamId,
          ManagerId = getteam.ManagerId,
          TeamName = getteam.TeamName,
          ManagerName  = GetManagerName!.FullName,
          Employee= EmployeeList
        };
        return TypedResults.Ok(team);
    }
}
