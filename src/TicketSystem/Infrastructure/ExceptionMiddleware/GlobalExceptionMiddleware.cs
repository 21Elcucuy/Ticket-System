using System;
using Microsoft.AspNetCore.Mvc;

namespace TicketSystem.Infrastructure.ExceptionMiddleware ;

public class GlobalExceptionMiddleware(RequestDelegate next )
{
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch(Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await  context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Something Went wrong",
                Detail =ex.Message,
            });
        }
    }
}