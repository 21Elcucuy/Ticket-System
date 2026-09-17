using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Infrastructure.Persistence;
using Wolverine.Http;

namespace TicketSystem.Feature.Auth.Logout;
 

 public record LogoutCommand(string RefreshToken);

 public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Access Token Is required");

    }
}

public class LogoutEndpoint
{
    [Authorize]
    [WolverinePost("api/logout")]
       public async Task<Results<NoContent, ProblemHttpResult>> Handle(LogoutCommand command , AppDbContext context,IHttpContextAccessor httpContext ,CancellationToken ct)
     {
         var token = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == command.RefreshToken , ct);
          var userId= int.Parse(httpContext.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value !);   
          if(token is null ||   token.UserId != userId)
        {
             return   TypedResults.Problem(
                title: "Invalid Token" ,
                detail : "Invalid  Refresh Token or Wrong User",
                statusCode: StatusCodes.Status401Unauthorized
                 );
        }
        token.Revoke();
        await context.SaveChangesAsync(ct);
           return TypedResults.NoContent();
    }
       
       
       }