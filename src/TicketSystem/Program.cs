using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TicketSystem.Application.Auth;
using TicketSystem.Common.Model;
using TicketSystem.Infrastructure.Persistence;
using Wolverine;
using Wolverine.Http;
using Wolverine.Http.Transport;
using Wolverine.FluentValidation;
using Wolverine.Http.FluentValidation;
using TicketSystem.Infrastructure.ExceptionMiddleware;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services.AddSqlite<AppDbContext>("Data Source =AppDb.db");

builder.Services.AddScoped<TokenProvider>();
builder.Services.AddScoped<AppDbContext>();





builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme  = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(op =>
{
  op.TokenValidationParameters = new TokenValidationParameters
  {
      ValidateIssuer = true,
      ValidateAudience =true,
      ValidateLifetime = true, 
      ValidateIssuerSigningKey = true,
      ValidIssuer = builder.Configuration["Jwt:Issuer"],
      ValidAudience =builder.Configuration["Jwt:Audience"],
      IssuerSigningKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))

  }  ;
});

builder.Services.Configure<IdentityOptions>(options =>
{
    
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
}).AddIdentity<AppUser , IdentityRole<int>>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
// builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
// .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWolverineHttp();
builder.Services.AddWolverine(op =>
{
    op.UseFluentValidation();
    
});





var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapWolverineEndpoints(opts =>
{
     opts.UseFluentValidationProblemDetailMiddleware();
});
 app.MapWolverineHttpTransportEndpoints();
app.Run();

