using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VioVid.Core.Identity;
using VioVid.Core.Models;

namespace VioVid.Core.ServiceContracts
{
    public interface IJwtService
    {
        JwtToken CreateJwtToken(ApplicationUser user);
        ClaimsPrincipal? GetPrincipalFromJwtToken(string token);
    }
}
