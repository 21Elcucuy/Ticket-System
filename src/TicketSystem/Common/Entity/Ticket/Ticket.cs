using TicketSystem.Common.Entity.Results;
using TicketSystem.Common.Entity.Ticket.Enums;

namespace TicketSystem.Common.Entity.Ticket;

public sealed class Ticket : AuditableEntity
{
  public int TicketId { get; private set; }
  public int CustomerId {get ;private set;}
  public string NatureOfRequest {get; private set;} = string.Empty; //In near future this need to update to addable enum 
  public TicketStatus TicketStatus {get ; private set;}
  public string Subject {get ;private set;} = string.Empty;

  private Ticket(int customerId , string natureOfRequest,TicketStatus ticketStatus,string subject):base(customerId)
  {
    CustomerId = customerId;
    NatureOfRequest = natureOfRequest;
    TicketStatus = ticketStatus;
    Subject =subject;
  }
  
  public Result<Ticket> Create(int customerId , string natureOfRequest,string subject)
  {
        if(customerId<0)
        {
        return TicketError.InvalidCustomerId();
       }
       if(string.IsNullOrWhiteSpace(natureOfRequest))
        {
      return TicketError.NatureOfRequestIsRequired();
    }
      if(string.IsNullOrWhiteSpace(subject))
    {
      return TicketError.SubjectIsRequired(); 
    }
    return new Ticket(customerId,natureOfRequest,TicketStatus.Available,subject);
  }
   
  public Result<Updated> Update(string natureOfRequest,string subject ,TicketStatus ticketStatus)
  {
      if(string.IsNullOrWhiteSpace(natureOfRequest))
        {
      return TicketError.NatureOfRequestIsRequired();
    }
      if(string.IsNullOrWhiteSpace(subject))
    {
      return TicketError.SubjectIsRequired(); 
    }
    if(Enum.IsDefined(ticketStatus))
    { 
      return TicketError.EnumIsNotDefined();  
    }

     return  Result.Updated;
  }
  
}