using finansee_api.Data;
using finansee_api.DTOs;
using finansee_api.Models;
using Microsoft.EntityFrameworkCore;

namespace finansee_api.Services;

public class AuthService(
    ApplicationDbContext dbContext,
    IJwtTokenService jwtTokenService) : IAuthService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

    public AuthResponse Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var alreadyExists = _dbContext.Usuarios
            .Any(x => x.Email == email);

        if (alreadyExists)
        {
            throw new InvalidOperationException("Já existe um usuário cadastrado com este e-mail.");
        }

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = email,
            Perfil = string.IsNullOrWhiteSpace(request.Perfil) ? "usuario" : request.Perfil.Trim().ToLowerInvariant(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _dbContext.Usuarios.Add(usuario);
        _dbContext.SaveChanges();

        return BuildAuthResponse(usuario);
    }

    public AuthResponse? Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var usuario = _dbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefault(x => x.Email == email && x.Ativo);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.SenhaHash))
        {
            return null;
        }

        return BuildAuthResponse(usuario);
    }

    public UsuarioResponse? GetUsuario(int usuarioId)
    {
        return _dbContext.Usuarios
            .AsNoTracking()
            .Where(x => x.Id == usuarioId)
            .Select(x => new UsuarioResponse
            {
                Id = x.Id,
                Nome = x.Nome,
                Email = x.Email,
                Perfil = x.Perfil,
                Ativo = x.Ativo
            })
            .FirstOrDefault();
    }

    private AuthResponse BuildAuthResponse(Usuario usuario)
    {
        var (token, expiresAt) = _jwtTokenService.GenerateToken(usuario);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                Ativo = usuario.Ativo
            }
        };
    }
}
