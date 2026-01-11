using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Tenant;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authenticator.API.Core.Application.Implementation;

/// <summary>
/// Serviço para geração e validação de tokens JWT
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings, ILogger<JwtTokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Gera um token de acesso JWT para o usuário e tenant informados
    /// </summary>
    /// <param name="user"></param>
    /// <param name="tenant"></param>
    /// <param name="roles"></param>
    /// <returns></returns>
    public string GenerateAccessToken(UserAccountEntity user, TenantEntity? tenant, IList<string> roles)
    {
        try
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new("full_name", $"{user.FirstName} {user.LastName}".Trim()),
                new("user_id", user.Id.ToString()),
                new("username", user.Username),
                new("email", user.Email)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            if (tenant != null)
            {
                claims.Add(new Claim("tenant_id", tenant.Id.ToString()));
                claims.Add(new Claim("tenant_slug", tenant.Slug));
                claims.Add(new Claim("tenant_name", tenant.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar token de acesso para usuário {UserId}", user.Id);
            throw;
        }
    }

    /// <summary>
    /// Gera um refresh token
    /// </summary>
    /// <returns></returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Obtém os claims de um token JWT válido
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao obter claims do token");
            return null;
        }
    }

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token inválido");
            return false;
        }
    }

    /// <summary>
    /// Obtém o tempo de expiração configurado para tokens (em segundos)
    /// </summary>
    /// <returns></returns>
    public int GetTokenExpirationTime() =>  _jwtSettings.AccessTokenExpirationMinutes * 60; 
}