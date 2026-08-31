using GrpcAutthorizationService.Entitites;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GrpcAutthorizationService.Services
{
    public class GenerateJwt
    {

        static public string Generate(User user, string role)
        {
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                        new Claim(JwtRegisteredClaimNames.Email,user.Email),
                        new Claim("role", role)
                    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Секретный ключ fdisfjosfjosjfiodsfiodsjifdsiofjidsfjiosjdfisfjidjsfjdfjs"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "AuthService",
                audience: "AllServices",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        static public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
