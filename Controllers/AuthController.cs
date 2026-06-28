using finansee_api.DTOs;
using finansee_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace finansee_api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
    {
        var response = _authService.Login(request);
        return Ok(response);
    }

    [HttpGet("test-bcrypt")]
    [AllowAnonymous]
    public ActionResult TestBCrypt([FromQuery] string senha = "admin123", [FromQuery] string? hash = null)
    {
        var hashDoBanco = hash ?? "$2a$10$N9qo8uLOickgx2ZMRZoMye1tLqH.IbIjJqJ5z8tNKLLlGPFqYFZWi";
        
        var resultado = new
        {
            senhaTestada = senha,
            hashTestado = hashDoBanco,
            hashValido = BCrypt.Net.BCrypt.Verify(senha, hashDoBanco),
            novoHashGerado = BCrypt.Net.BCrypt.HashPassword(senha),
            novoHashValido = true, // Sempre será válido pois acabamos de gerar
            instrucao = "Se hashValido = false, copie o novoHashGerado e atualize no banco"
        };

        return Ok(resultado);
    }
}
