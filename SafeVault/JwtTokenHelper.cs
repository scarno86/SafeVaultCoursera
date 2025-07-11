using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SafeVault
{
    public static class JwtTokenHelper
    {

        public static string GenerateToken(string username, string secretKey, long expirationTime)
        {
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenHanlder = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddSeconds(expirationTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHanlder.CreateToken(tokenDescriptor);  
            return tokenHanlder.WriteToken(token);  
        }
    }
}
