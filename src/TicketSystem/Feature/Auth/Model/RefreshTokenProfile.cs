using TicketSystem.Common.Entity;
using TicketSystem.Common.Entity.Results;
using TicketSystem.Common.Model;

namespace TicketSystem.Feature.Auth.Model;


public class RefreshTokenProfile : AuditableEntity
{
    public int Id {get ;private  set; }
    public int UserId { get;private  set; }
    public string Token {get; private set;}
    public DateTimeOffset ExpiresAtUtc {get ;private  set;}
    public bool IsRevoked {get ;  set;}
    
    public AppUser? User  {get ; set;}

    private RefreshTokenProfile(string token ,int userId, DateTimeOffset expiresAtUtc)
    {
        Token = token;
        UserId = userId ;
        ExpiresAtUtc = expiresAtUtc;
    }
    public static  Result<RefreshTokenProfile> Create(int userId, DateTimeOffset expiresAtUtc)
    {
        if(expiresAtUtc <= DateTimeOffset.UtcNow)
        {return Error.Validation("Invalid Date" , "The Date you made not in same time") ;}
         var token =  Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        return new RefreshTokenProfile(token,userId,expiresAtUtc);
    }
    public void Revoke() => IsRevoked =true;
}