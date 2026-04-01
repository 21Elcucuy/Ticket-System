using Wolverine;
using Wolverine.Http;
using Wolverine.Http.Transport;
using Wolverine.FluentValidation;
using Wolverine.Http.FluentValidation;
using TicketSystem.Infrastructure.ExceptionMiddleware;
using FluentValidation;
using TicketSystem.Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddInfrastructure(builder.Configuration);




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWolverineHttp();
builder.Services.AddWolverine(op =>
{
    op.UseFluentValidation();
   
});

builder.Services.AddProblemDetails(op =>
{
    op.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId" ,context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId",activity?.Id);
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseExceptionHandler();

app.UseRouting();
app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapWolverineEndpoints(opts =>
{
    opts.WarmUpRoutes = RouteWarmup.Eager;
     opts.UseFluentValidationProblemDetailMiddleware();
     opts.RequireAuthorizeOnAll();
     
});
 app.MapWolverineHttpTransportEndpoints();
app.Run();

