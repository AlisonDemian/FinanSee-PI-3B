using finansee_api.Data;
using finansee_api.DTOs;
using finansee_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace finansee_api.Services;

public class AuthService(
    ApplicationDbContext dbContext,
    IJwtTokenService jwtTokenService,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly ILogger<AuthService> _logger = logger;

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

    public AuthResponse Login(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Tentativa de login para o e-mail: {Email}", request.Email);

            var email = request.Email.Trim().ToLowerInvariant();

            _logger.LogDebug("Buscando usuário no banco de dados com e-mail: {Email}", email);

            Usuario? usuario;
            try
            {
                usuario = _dbContext.Usuarios
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Email == email && x.Ativo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário no banco de dados. E-mail: {Email}", email);
                throw new InvalidOperationException($"Erro ao acessar o banco de dados durante o login: {ex.Message}", ex);
            }

            if (usuario is null)
            {
                var usuarioInativo = _dbContext.Usuarios
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Email == email);

                if (usuarioInativo is not null && !usuarioInativo.Ativo)
                {
                    _logger.LogWarning("Tentativa de login com usuário inativo. E-mail: {Email}, UsuarioId: {UsuarioId}", email, usuarioInativo.Id);
                    throw new UnauthorizedAccessException($"Usuário inativo. E-mail: {email}");
                }

                _logger.LogWarning("Tentativa de login com e-mail não cadastrado: {Email}", email);
                throw new UnauthorizedAccessException($"E-mail não encontrado: {email}");
            }

            _logger.LogDebug("Usuário encontrado. UsuarioId: {UsuarioId}, Nome: {Nome}", usuario.Id, usuario.Nome);

            bool senhaValida;
            try
            {
                senhaValida = BCrypt.Net.BCrypt.Verify(request.Password, usuario.SenhaHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar senha do usuário. UsuarioId: {UsuarioId}", usuario.Id);
                throw new InvalidOperationException($"Erro ao verificar senha: {ex.Message}", ex);
            }

            if (!senhaValida)
            {
                _logger.LogWarning("Senha inválida para o usuário. E-mail: {Email}, UsuarioId: {UsuarioId}", email, usuario.Id);
                throw new UnauthorizedAccessException($"Senha inválida para o e-mail: {email}");
            }

            _logger.LogInformation("Login realizado com sucesso. UsuarioId: {UsuarioId}, Email: {Email}", usuario.Id, email);

            return BuildAuthResponse(usuario);
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante o login. E-mail: {Email}", request.Email);
            throw new InvalidOperationException($"Erro inesperado durante o login: {ex.Message}", ex);
        }
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
