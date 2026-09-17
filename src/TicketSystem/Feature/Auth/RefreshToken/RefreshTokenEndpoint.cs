using System.Security.Claims;
using FastExpressionCompiler;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Engines;
using TicketSystem.Common.Dtos;
using TicketSystem.Common.Interface;
using TicketSystem.Infrastructure.Persistence;
using Wolverine.Http;

namespace TicketSystem.Feature.Auth.RefreshToken;
 

 public record RefreshTokenCommand(string AccessToken , string RefreshToken );


public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Access Token Is required");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Access Token Is required");

    }
}

public class RefreshTokenEndpoint
{
    [WolverinePost("api/refresh-token")]
    public async Task<Results<Ok<TokenResponse>, ProblemHttpResult>> Handle(RefreshTokenCommand command , AppDbContext context, ITokenProvider tokenProvider , CancellationToken ct)
    {
        var principal = tokenProvider.GetPrincipalFromExpiredToken(command.AccessToken);
        if(principal is null)
        {
            return   TypedResults.Problem(
                title: "Invalid Token" ,
                detail : "Access Token is Invalid",
                statusCode: StatusCodes.Status401Unauthorized
                 );
        }
        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!int.TryParse(userIdClaim , out var userId))
        {
                return   TypedResults.Problem(
                title: "Invalid Token" ,
                detail : "Access Token is Invalid",
                statusCode: StatusCodes.Status401Unauthorized
                 );
        }
        
      var result = await context.RefreshTokens.Where(x => x.Token == command.RefreshToken && x.UserId == userId ).FirstOrDefaultAsync(ct);
      if(result?.Token is null)
        {
             return   TypedResults.Problem(
                title: "Invalid Token" ,
                detail : "Refresh Token is not found",
                statusCode: StatusCodes.Status401Unauthorized
                 );
        }
        if(result?.UserId is null)
        {
             return   TypedResults.Problem(
                title: "Invalid Token" ,
                detail : "User is not found",
                statusCode: StatusCodes.Status401Unauthorized
                 );
        }
        var revokeToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == command.RefreshToken , ct);
        revokeToken!.Revoke();
        await context.SaveChangesAsync();
        var user = context.Users.FirstOrDefault(x => x.Id == userId);
        var token =await  tokenProvider.GenerateAsync(user! , ct);
        if(token.IsError)
        {
               return TypedResults.Problem(
                title: "Something Went wrong",
                detail: token.Errors.ToString(),
                statusCode: StatusCodes.Status500InternalServerError);
        }
          return TypedResults.Ok(token.Value);
    }
}