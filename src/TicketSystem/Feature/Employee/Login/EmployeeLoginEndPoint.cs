using System.Linq.Expressions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.Auth;
using TicketSystem.Common.Dtos;
using TicketSystem.Common.Entity.Enum;
using TicketSystem.Common.Interface;
using TicketSystem.Common.Model;
using TicketSystem.Infrastructure.Persistence;
using Wolverine.Http;

namespace TicketSystem.Feature.Employee.Login;

public sealed record EmployeeLoginCommand(string Email , string Password);

public sealed class EmployeeLoginValidation : AbstractValidator<EmployeeLoginCommand>
{
    public EmployeeLoginValidation()
    {
        RuleFor(x => x.Email).EmailAddress().NotNull().WithMessage("Email Is Required");
        RuleFor(x => x.Password).NotNull().WithMessage("Password is  Required").MinimumLength(6);
    }

}
[AllowAnonymous]
[Tags("EmployeeLogin")]
public class EmployeeLoginEndPoint(AppDbContext context , UserManager<AppUser> userManager,ITokenProvider tokenProvider) 
{
    private readonly AppDbContext context = context;
    private readonly UserManager<AppUser> userManager = userManager;
    private readonly ITokenProvider tokenProvider = tokenProvider;

    public   async Task<ProblemDetails> ValidateAsync(EmployeeLoginCommand command , CancellationToken ct)
    {
        var IsEmailExist = await  context.Users.AnyAsync(x => x.Email ==command.Email , ct);
        if(!IsEmailExist)
        {
            return new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Email is Not Found",
                Detail = "Email is Not Exist"
            };
        }
        return WolverineContinue.NoProblems;
    } 
    [WolverinePost("/api/Employee-Login")]
    
    public async Task<Results<Ok<TokenResponse>, ProblemHttpResult>> Handle(EmployeeLoginCommand command , CancellationToken ct)
    {
        var user = context.Users.FirstOrDefault(x => x.Email == command.Email)!;
        if(! await userManager.CheckPasswordAsync(user,command.Password))
        {
            return TypedResults.Problem(
            
                statusCode : StatusCodes.Status400BadRequest,
                title: "Cant Login",
                detail: "Incorrect Password"
            );
        }
       var tokens = await tokenProvider.GenerateAsync(user);
        if(tokens.IsError)
        {
             return TypedResults.Problem(
            
                statusCode : StatusCodes.Status500InternalServerError,
                title: "Something Went Wrong",
                detail: string.Join(" ", tokens.Errors)
            );
        }
       return TypedResults.Ok(tokens.Value);
        
    }
}