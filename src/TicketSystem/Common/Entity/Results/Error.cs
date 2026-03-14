using System.Runtime.CompilerServices;

namespace TicketSystem.Common.Entity.Results;


public  class Error
{
   private  string? _Code;
   private  ErrorKind _Error;
   private  string? _Description; 

   private Error(string Code , ErrorKind Error , string Description)
    {
        _Code =Code;
        _Error = Error ;
        _Description =Description;
    }
     public static Error Validation(string Code = nameof(Validation)  ,string  Description ="Validation Error",ErrorKind Error =ErrorKind.Validation) =>
     new(Code ,Error ,Description);
     public static Error Failure(string Code = nameof(Failure) ,string  Description ="Failure Error",ErrorKind Error =ErrorKind.Failure ) =>
     new(Code ,Error , Description);
     public static Error Unexpected(string Code = nameof(Unexpected),string  Description ="Unexpected Error" ,ErrorKind Error =ErrorKind.Unexpected ) =>
     new(Code ,Error , Description);
     public static Error Conflict(string Code = nameof(Conflict) ,string  Description ="Conflict Error" ,ErrorKind Error =ErrorKind.Conflict) =>
     new(Code ,Error , Description);
     public static Error NotFound(string Code = nameof(NotFound)  ,string  Description ="NotFound Error",ErrorKind Error =ErrorKind.NotFound) =>
     new(Code ,Error , Description);
     public static Error Unauthorized(string Code = nameof(NotFound) ,string  Description ="Unauthorized Error" ,ErrorKind Error =ErrorKind.Unauthorized) =>
     new(Code ,Error , Description);
     public static Error Forbidden(string Code = nameof(Forbidden) ,string  Description ="Forbidden Error",ErrorKind Error =ErrorKind.Forbidden ) =>
     new(Code ,Error , Description);
    

}