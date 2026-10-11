using JasperFx.CodeGeneration.Frames;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Engines;
using TicketSystem.Common.Entity.Results;
using TicketSystem.Common.Interface;
using TicketSystem.Feature.Ticket.Model;
using TicketSystem.Infrastructure.Persistence;

namespace TicketSystem.Feature.Ticket.ProcessCreatedTicket;


public class ProcessCreatedTicketWorker(ILogger<ProcessCreatedTicketWorker> logger ,IServiceScopeFactory scopeFactory) : BackgroundService 
{
    private readonly ILogger<ProcessCreatedTicketWorker> logger = logger;
    private readonly IServiceScopeFactory scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var Timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
      do{


        
           logger.LogInformation("Background Service is working");
            try
            {
              
            await Process(stoppingToken);
            }
            catch (Exception ex)when(ex is not OperationCanceledException)
            {
                logger.LogError(ex.Message);
              
            } 
        }
        while(await Timer.WaitForNextTickAsync(stoppingToken));
    }
    private async Task<bool> Process(CancellationToken ct)
    {
      using var scope =  scopeFactory.CreateScope();   
      scope.ServiceProvider.GetRequiredService<ISystemScope>().Use(-1);
      var context = scope.ServiceProvider.GetRequiredService<AppDbContext>(); 
    if(!await context.Tickets.AnyAsync(x => x.TicketStatus == TicketStatus.Unssigned,ct))
        {
            return true; 
        }
    if(!await context.Employees.AnyAsync(x => x.AppUser!.IsOnline ==true , ct))
    {
      return  true ; 
    }
      var ticket = await context.Tickets.FirstOrDefaultAsync(x => x.TicketStatus == TicketStatus.Unssigned,ct);
      var employee = await context.Employees.FirstOrDefaultAsync(x => x.AppUser!.IsOnline == true,ct);
      ticket!.TicketStatus = TicketStatus.pending;
      ticket!.EmployeeId = employee!.EmployeeId;
       await context.SaveChangesAsync();
      return true ;  
    }
}