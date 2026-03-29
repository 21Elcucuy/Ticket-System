using Microsoft.AspNetCore.Authentication.BearerToken;

namespace TicketSystem.Common.Dtos;

public sealed record TokenResponse
{
    public string? AccessToken;
    public string? RefreshToken;
    public DateTimeOffset ExpiresOnUtc;
    
}