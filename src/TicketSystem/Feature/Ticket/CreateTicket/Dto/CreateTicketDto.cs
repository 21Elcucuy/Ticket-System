using TicketSystem.Feature.Ticket.Model;

namespace TicketSystem.Feature.Ticket.CreateTicket.Dto;

public class CreateTicketDto
{
    public string? TicketSubject {get; set;}
    public string? FromEmail {get; set;}
    public TicketStatus TicketStatus {get; set;}


}