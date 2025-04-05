using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Models;

namespace WebAPI.Security
{
    public static class JwtTokenProvider
    {
        public static string CreateToken(string secureKey, int expirationMinutes, User user)
        {
            var tokenKey = Encoding.UTF8.GetBytes(secureKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username), 
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), 
                new Claim(ClaimTypes.Email, user.Email), 
                new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : "User") 
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); 
        }
    }
}
