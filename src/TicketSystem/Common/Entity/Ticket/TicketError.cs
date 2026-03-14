using TicketSystem.Common.Entity.Results;

namespace TicketSystem.Common.Entity.Ticket;

public static  class TicketError
{
    public static Error InvalidCustomerId()=>Error.Validation("InvalidCustomerId" ,"Invalid Customer Id"); 
    public static Error NatureOfRequestIsRequired()=>Error.Validation("NatureOfRequestIsRequired" ,"Nature Of Request Is Required"); 
    public static Error SubjectIsRequired()=>Error.Validation("SubjectIsRequired" ,"Subject Is Required"); 
    public static Error EnumIsNotDefined()=> Error.NotFound("EnumIsNotDefined","Enum Is Not Defined"); 
}