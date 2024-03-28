using FCMS.Model.DTOs;

namespace FCMS.Auth
{
    public interface IJWTManager
    {
        string CreateToken(JwtTokenRequestModel model);
    }
}
