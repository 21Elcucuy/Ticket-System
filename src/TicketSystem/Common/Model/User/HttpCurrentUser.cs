using System;
using System.Security.Claims;
using TicketSystem.Common.Interface;

namespace TicketSystem.Common.Model.User;

public class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser ,ISystemScope
{
    private int _systemid;
    public void Use(int systemId) => _systemid = systemId;
    
    public int UserId {
        get
        {
            if(_systemid is int systemId)
            {
                return _systemid;
            }
            var claim = accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if(int.TryParse(claim,out var userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("no authnticated user for this");
        }
    }

  
}
