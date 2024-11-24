using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VioVid.Core.Identity;

namespace VioVid.Core.ServiceContracts;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user);
    (string refreshToken, DateTime expirationDateTime) GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromJwtToken(string token);
}
