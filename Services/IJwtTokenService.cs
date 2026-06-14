using finansee_api.Models;

namespace finansee_api.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(Usuario usuario);
}
