using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Common.Entity.Results;
using TicketSystem.Common.Interface;
using TicketSystem.Feature.Ticket.Model;
using TicketSystem.Infrastructure.Persistence;
using Wolverine.Http;

namespace TicketSystem.Feature.Ticket.UpdateTicket;

public record UpdateTicketCommand(int TicketId , TicketStatus TicketStatus ,string ResponseMessage);

public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
         RuleFor(x => x.TicketStatus).IsInEnum().Must(status => status != TicketStatus.pending).WithMessage("pending is not allowed");
         RuleFor(x => x.ResponseMessage).NotEmpty();
    }
}

[Tags("Ticket")]
public class UpdateTicketEndpoint(AppDbContext context , IEmailSerivces emailSerivces)
{
    private readonly AppDbContext context = context;
    private readonly IEmailSerivces emailSerivces = emailSerivces;

    public async Task<ProblemDetails> ValidateAsync(UpdateTicketCommand command ,CancellationToken ct)
    {
        if(! await context.Tickets.AnyAsync(x => x.TicketId  == command.TicketId , ct))
        {
            return new ProblemDetails
            {
                Title="Invalid Ticket",
                Status = StatusCodes.Status404NotFound,
                Detail = "Ticket is Not Exist"
                
            };

        }
        return WolverineContinue.NoProblems;
    }
    [WolverinePut("/api/update-ticket")]
    [AllowAnonymous]
    public async Task<Results<NoContent , BadRequest>>Handle(UpdateTicketCommand command , CancellationToken ct  = default)
    {
        var Ticket = await context.Tickets.FirstOrDefaultAsync(x => x.TicketId == command.TicketId ,ct);
        
        if (Ticket is null)
        {
            return TypedResults.BadRequest();
        }
        
        Ticket!.TicketStatus = command.TicketStatus;
         
        var result= context.Tickets.Update(Ticket);
       
        if(result.State == EntityState.Modified)
        {
            await context.SaveChangesAsync(ct);
            await emailSerivces.SendEmailAaync(Ticket.FromEmail!,"Damn" , command.ResponseMessage);
          return TypedResults.NoContent();

        }
       return  TypedResults.BadRequest();
     
    }
}
