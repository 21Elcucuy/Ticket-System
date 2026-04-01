using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ImTools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SQLitePCL;
using TicketSystem.Application.Auth;
using TicketSystem.Common.Entity.Enum;
using TicketSystem.Common.Interface;
using TicketSystem.Common.Model;
using TicketSystem.Feature.Auth.Register;
using TicketSystem.Infrastructure.Persistence;
using Wolverine;

namespace TicketSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection Services , IConfiguration Configuration)
    {


        Services.AddSqlite<AppDbContext>("Data Source =AppDb.db");
        Services.AddScoped<AppDbContext>();
        Services.AddScoped<ITokenProvider,TokenProvider>();
        Services.AddAuthentication(op =>
        {
           op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
           op.DefaultChallengeScheme =  JwtBearerDefaults.AuthenticationScheme;
           op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
          
    
        }).AddJwtBearer(op =>
{
  op.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
  {
      
      RoleClaimType = ClaimTypes.Role,
      ValidateIssuer = false,
      ValidateAudience =false,
      ValidateLifetime = true, 
      ValidateIssuerSigningKey = true,
      ValidIssuer = Configuration["Jwt:Issuer"],
      ValidAudience = Configuration["Jwt:Audience"],
      IssuerSigningKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"]!))
     
  };
  op.IncludeErrorDetails =true;
    });

Services.AddIdentityCore<AppUser>(options =>
            {
        options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});
 Services.AddHttpContextAccessor();
 Services.AddAuthorization(op =>
 {
     op.AddPolicy("CanRegisterEmployee" , policy => policy.RequireClaim(ClaimTypes.Role,nameof(Role.Manager)));
     op.AddPolicy("Test",policy => policy.RequireClaim(ClaimTypes.Role,nameof(Role.Staff)));
 });

 

    return Services;

}




}