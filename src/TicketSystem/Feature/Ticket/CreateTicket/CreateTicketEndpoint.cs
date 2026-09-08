using System.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Feature.Ticket.CreateTicket.Dto;
using TicketSystem.Feature.Ticket.Model;
using TicketSystem.Infrastructure.Persistence;
using Wolverine.Http;

namespace TicketSystem.Feature.Ticket.CreateTicket;

public record CreateTicketCommand(string TikcetSubject , string FromEmail , int EmployeeId) ;// The EmployeeId in the near future should remove it from command 

public class CreateTikcetCommandValidator : AbstractValidator<CreateTicketCommand>
{
     public CreateTikcetCommandValidator()
    {
        RuleFor(x => x.TikcetSubject).NotEmpty().WithMessage("The Subject is required");
        RuleFor(x => x.FromEmail).EmailAddress();
    } 
}
[Tags("Ticket")]

public class CreateTicketEndpoint(AppDbContext context )
{
    private readonly AppDbContext context = context;
    public async Task<ProblemDetails> ValidateAsync(CreateTicketCommand command , CancellationToken ct)
    {
        if(! await context.Employees.AnyAsync(x => x.EmployeeId == command.EmployeeId, cancellationToken: ct))
         {
            return new ProblemDetails
            {
                Title = "Invalid Employee",
                Status = StatusCodes.Status404NotFound,
                Detail = "Employee Not Found" 

            };

         }
         return WolverineContinue.NoProblems;
    }
     [WolverinePost("/api/Create-Ticket")]
     [AllowAnonymous]
    public async Task<Results<Ok<CreateTicketDto> ,BadRequest>> Handle(CreateTicketCommand command , CancellationToken ct  = default)
    {
        var Ticket = new TicketProfile
        {
            EmployeeId = command.EmployeeId,
            TicketStatus = TicketStatus.pending,
            TicketSubject = command.TikcetSubject,
            FromEmail = command.FromEmail
        };
        var Result =  context.Tickets.Add(Ticket);
        
        if(Result.State == EntityState.Added)
        {
            await context.SaveChangesAsync(ct);
            var TicketDto = new CreateTicketDto()
            {
                FromEmail = Ticket.FromEmail,
                TicketStatus = Ticket.TicketStatus,
                TicketSubject = Ticket.TicketSubject
            };
            return TypedResults.Ok(TicketDto);
        }
        
        return TypedResults.BadRequest();

    }


}