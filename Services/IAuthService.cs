using finansee_api.DTOs;

namespace finansee_api.Services;

public interface IAuthService
{
    AuthResponse Register(RegisterRequest request);
    AuthResponse Login(LoginRequest request);
    UsuarioResponse? GetUsuario(int usuarioId);
}
