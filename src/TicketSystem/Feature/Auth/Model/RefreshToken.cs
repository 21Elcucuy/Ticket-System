using TicketSystem.Common.Entity.Results;

namespace TicketSystem.Feature.Auth.Model;


public class RefreshToken
{
    public int Id {get ;private  set; }
    public int UserId { get;private  set; }
    public string Token {get; private set;}
    public DateTimeOffset ExpiresAtUtc {get ;private  set;}
    public bool IsRevoked {get ; private set;}

    private RefreshToken(string token ,int userId, DateTimeOffset expiresAtUtc)
    {
        Token = token;
        UserId = userId ;
        ExpiresAtUtc = expiresAtUtc;
    }
    public static  Result<RefreshToken> Create(int userId, DateTimeOffset expiresAtUtc)
    {
        if(expiresAtUtc <= DateTimeOffset.UtcNow)
        {return Error.Validation("Invalid Date" , "The Date you made not in same time") ;}
         var token =  Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        return new RefreshToken(token,userId,expiresAtUtc);
    }
    public void Revoke() => IsRevoked =true;
}