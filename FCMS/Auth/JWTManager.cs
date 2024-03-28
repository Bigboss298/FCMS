using FCMS.Model.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FCMS.Auth
{
    public class JWTManager : IJWTManager
    {
        private const int ExpirationMinutes = 30;
        public string CreateToken(JwtTokenRequestModel model)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(CreateJwtToken(CreateClaims(model), CreateSigningCredentials(), expiration));
        }

        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration) => new("FCMSAuthIssuer", "FCMSAuthAudience", claims, expires: expiration, signingCredentials: credentials);

        private List<Claim> CreateClaims(JwtTokenRequestModel model)
        {
            try
            {
                var claims = new List<Claim>
                 {
                     new Claim(JwtRegisteredClaimNames.Sub, "TokenForTheApiWithAuth"),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                     new Claim(ClaimTypes.NameIdentifier, model.Id),
                     new Claim(ClaimTypes.Name, model.FirstName),
                     new Claim(ClaimTypes.Role, model.Role.ToLower()),
                     new Claim(ClaimTypes.Email, model.Email),
                     new Claim(ClaimTypes.UserData, model.ProfilePicture),
                 };
                return claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private SigningCredentials CreateSigningCredentials()
        {
            var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("!Iwantthistobeextremelysecretivesopleasetreatwithcare!")), SecurityAlgorithms.HmacSha256);
            return signingCredentials;
        }

    }
}
