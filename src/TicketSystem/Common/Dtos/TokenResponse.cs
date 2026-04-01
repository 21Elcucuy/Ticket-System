using Microsoft.AspNetCore.Authentication.BearerToken;

namespace TicketSystem.Common.Dtos;

public sealed record TokenResponse
{
    public string? AccessToken {get;set;}
    public string? RefreshToken {get;set;}
    public DateTimeOffset ExpiresOnUtc {get;set;}
    
}