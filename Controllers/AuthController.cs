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
}
