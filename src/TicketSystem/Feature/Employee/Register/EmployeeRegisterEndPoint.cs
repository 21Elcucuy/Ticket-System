using System.Security.Claims;
using FluentValidation;
using ImTools;
using JasperFx.Events.Daemon;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using TicketSystem.Common.Dtos;
using TicketSystem.Common.Entity.Enum;
using TicketSystem.Common.Model;
using TicketSystem.Feature.Employee.Model;
using TicketSystem.Infrastructure.Persistence;
using Wolverine;
using Wolverine.Http;

namespace TicketSystem.Feature.Auth.Register;


public record EmployeeRegisterCommand(string Email , string UserName , int DepartmentId ,string FirstName, string LastName , string Password ,string PhoneNumber,double Salary);

public class EmployeeRegisterCommandValidator : AbstractValidator<EmployeeRegisterCommand>
{
    public EmployeeRegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotNull().WithMessage("Email Is Required").EmailAddress();
        RuleFor(x => x.UserName).NotNull().WithMessage("UserName Is Required");
        RuleFor(x => x.FirstName).NotNull().WithMessage("FirstName  Is Required");
        RuleFor(x => x.LastName).NotNull().WithMessage("LastName  Is Required");
        RuleFor(x => x.Password).NotNull().WithMessage("Password  Is Required").MinimumLength(6);
        RuleFor(x => x.DepartmentId).NotNull().WithMessage("DepartmentId is Required");
        RuleFor(x => x.PhoneNumber).Matches( @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");
        RuleFor(x => x.Salary).NotNull();
    }
}

[Tags("EmployeeRegister")]
// [Authorize(policy : "CanRegisterEmployee")]
   
public class EmployeeRegisterEndPoint(AppDbContext context , UserManager<AppUser> userManager)
{
    private readonly AppDbContext context = context;
    private readonly UserManager<AppUser> userManager = userManager;
   

    public   async Task<ProblemDetails> ValidateAsync(EmployeeRegisterCommand command , CancellationToken ct)
    {
        var IsEmailExist = await context.Users.AnyAsync(x => x.Email == command.Email ,ct);
        if (IsEmailExist)
        {
            return new  ProblemDetails{
            Status = StatusCodes.Status409Conflict,
            Title ="Invalid Email",
            Detail ="Email is Already Exist"
            };
        }
       var  IsUserNameExist =await context.Users.AnyAsync(x => x.UserName == command.UserName ,ct);
        if (IsUserNameExist)
        {
            return new  ProblemDetails{
            Status = StatusCodes.Status409Conflict,
            Title ="Invalid UserName",
            Detail ="UserName is Used"
            };
        }
        var IsDepartmentExist= await context.Departments.AnyAsync(x => x.DepartmentId == command.DepartmentId , ct);
        if (!IsDepartmentExist)
        {
            return new  ProblemDetails{
            Status = StatusCodes.Status404NotFound,
            Title ="Wrong Department Id",
            Detail ="The Department Id is not Exist"
            };
        }
         return WolverineContinue.NoProblems;
    }
     
        [WolverinePost("/api/Employee-Register")]
        [Authorize(Roles = "Staff")]
         public async Task<Results<Created, ProblemHttpResult>> Handle(EmployeeRegisterCommand command , CancellationToken ct)
         {
            var user = new AppUser
            {
               Email =command.Email,
               UserName = command.UserName,
               FirstName = command.FirstName,
               LastName = command.LastName,
               PhoneNumber = command.PhoneNumber,
               Role = Role.Staff
            };
             user.PasswordHash = userManager.PasswordHasher.HashPassword(user , command.Password);
             user.NormalizedUserName = user.UserName.ToUpper();
             user.NormalizedEmail = user.Email.ToUpper();
            using var transaction = context.Database.BeginTransaction();
            
                
            var ResultUser = context.Users.Add(user);
            await context.SaveChangesAsync(ct);   
        
             var employee = new EmployeeProfile
             {
                 DepartmentId = command.DepartmentId,
                 EmployeeId = ResultUser.Entity.Id,
                 Salary = command.Salary
             };
        
           var addEmployeeResult =  context.Employees.Add(employee);
             
           await context.SaveChangesAsync(ct);   
        
            await transaction.CommitAsync(ct);
           return TypedResults.Created();
            
       
        }
        }
    

