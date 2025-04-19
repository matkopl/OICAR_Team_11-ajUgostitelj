using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Services
{
    public class JwtDecodeService
    {
        public static string GetUserRole(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var role = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role")?.Value;
            return role ?? "User";
        }
    }
}
