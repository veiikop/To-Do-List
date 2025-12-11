using To_Do_List.Models.DTO;

namespace To_Do_List.Services
{
    public interface IAuthService
    {
        AuthResponseDTO Login(LoginRequestDTO loginRequest);
        AuthResponseDTO Register(RegisterRequestDTO registerRequest);
        AuthResponseDTO RefreshToken(string token, string refreshToken); 
        bool ValidateToken(string token);
    }
}
