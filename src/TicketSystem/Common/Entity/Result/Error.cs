using System.Runtime.CompilerServices;

namespace TicketSystem.Common.Entity.Result;


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
     public static Error Validation(string Code = nameof(Validation) ,ErrorKind Error =ErrorKind.Validation ,string  Description ="Validation Error") =>
     new(Code ,Error ,Description);
     public static Error Failure(string Code = nameof(Failure) ,ErrorKind Error =ErrorKind.Failure ,string  Description ="Failure Error") =>
     new(Code ,Error , Description);
     public static Error Unexpected(string Code = nameof(Unexpected) ,ErrorKind Error =ErrorKind.Unexpected ,string  Description ="Unexpected Error") =>
     new(Code ,Error , Description);
     public static Error Conflict(string Code = nameof(Conflict) ,ErrorKind Error =ErrorKind.Conflict ,string  Description ="Conflict Error") =>
     new(Code ,Error , Description);
     public static Error NotFound(string Code = nameof(NotFound) ,ErrorKind Error =ErrorKind.NotFound ,string  Description ="NotFound Error") =>
     new(Code ,Error , Description);
     public static Error Unauthorized(string Code = nameof(NotFound) ,ErrorKind Error =ErrorKind.Unauthorized ,string  Description ="Unauthorized Error") =>
     new(Code ,Error , Description);
     public static Error Forbidden(string Code = nameof(Forbidden) ,ErrorKind Error =ErrorKind.Forbidden ,string  Description ="Forbidden Error") =>
     new(Code ,Error , Description);
    

}