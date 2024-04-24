using FCMS.Model.DTOs;

namespace FCMS.Auth
{
    public interface IJWTManager
    {
        string CreateToken(string key, string issuer, JwtTokenRequestModel model);
        //bool IsTokenValid(string key, string issuer, string token);
    }
}
