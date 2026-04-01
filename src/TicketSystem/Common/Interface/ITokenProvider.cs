using TicketSystem.Common.Dtos;
using TicketSystem.Common.Entity.Results;
using TicketSystem.Common.Model;

namespace TicketSystem.Common.Interface;

public interface ITokenProvider
{
     public Task<Result<TokenResponse>> GenerateAsync(AppUser user , CancellationToken ct = default);
}