using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FCMS.Auth
{
    public class JWTManager : IJWTManager
    {
        public string CreateToken(string key, string issuer, JwtTokenRequestModel model)
        {
            _ = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, model.Id),
                 new Claim(ClaimTypes.Name, model.FirstName),
                 new Claim(ClaimTypes.Role, model.Role.ToLower()),
                 new Claim(ClaimTypes.Email, model.Email),
                 new Claim(ClaimTypes.UserData, model.ProfilePicture),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
             expires: DateTime.Now.AddHours(5), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }

        public static bool IsTokenValid(string key, string issuer, string token)
        {
            var mySecret = Encoding.UTF8.GetBytes(key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static Guid GetLoginId(string? token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim != null)
            {
                string userId = idClaim.Value;
                return Guid.Parse(userId);
            }
            else
            {
                return Guid.Empty;
            }
        }
    }
}
