using System.Reflection.Metadata;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TicketSystem.Common.Dtos;
using TicketSystem.Common.Entity.Results;
using TicketSystem.Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JasperFx.CodeGeneration.Model;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Feature.Auth.Model;
using TicketSystem.Common.Model;
using TicketSystem.Common.Interface;

namespace TicketSystem.Application.Auth;


public class TokenProvider(AppDbContext context , IConfiguration configuration) :ITokenProvider
{
    private readonly AppDbContext context = context;
    private readonly IConfiguration configuration = configuration;

    public async Task<Result<TokenResponse>> GenerateAsync(AppUser user , CancellationToken ct = default)
    {
        var JwtSettings = configuration.GetSection("JwtSettings");
        var issuer = JwtSettings["Issuer"];
        var audience =  JwtSettings["Audience"];
        var key = JwtSettings["Key"];
        var expires  = DateTime.UtcNow.AddMinutes(int.Parse(JwtSettings["TokenExpirationInMinutes"]!));
        var claims = new List<Claim>()
        {
            new (JwtRegisteredClaimNames.Sub , $"{user.Id}"),
            new (JwtRegisteredClaimNames.Email , user.Email!),
            new (ClaimTypes.Role , user.Role.ToString())
        };
      
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var SecurityToken = tokenHandler.CreateToken(descriptor);
         
        var oldRefreshToken = await context.RefreshTokens.Where(x => x.UserId == user.Id).ExecuteDeleteAsync(ct);

        var refreshTokenResult = RefreshTokenProfile.Create(user.Id , DateTime.UtcNow.AddDays(7));
        if(refreshTokenResult.IsError)
        {
            return refreshTokenResult.Errors;
        }
        var refreshToken =refreshTokenResult.Value;
        
        context.RefreshTokens.Add(refreshToken);
        
        await context.SaveChangesAsync(ct);
        return new TokenResponse
        {
            
            AccessToken = tokenHandler.WriteToken(SecurityToken),
            RefreshToken = refreshToken.Token,
            ExpiresOnUtc = expires
        };
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
         var JwtSettings = configuration.GetSection("JwtSettings");
        var issuer = JwtSettings["Issuer"];
        var audience =  JwtSettings["Audience"];
        var key = JwtSettings["Key"];
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
            ValidateIssuer   = true,
            ValidIssuer      = issuer,
            ValidateAudience = true,
            ValidAudience    = audience,
            ValidateLifetime = false
        };

        var handler   = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validationParams, out var securityToken);

        if (securityToken is not JwtSecurityToken jwt ||
            !jwt.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            return null;

        return principal;
    }
}