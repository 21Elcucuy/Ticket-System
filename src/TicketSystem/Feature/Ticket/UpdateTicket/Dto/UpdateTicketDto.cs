using TicketSystem.Feature.Ticket.Model;

namespace TicketSystem.Feature.Ticket.UpdateTicket.Dto;

public class UpdateTicketDto
{
    public TicketStatus TicketStatus {get; set;}
    public string? ResponseMessage {get; set;}


}