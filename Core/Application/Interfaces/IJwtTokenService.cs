using Authenticator.API.Core.Domain.AccessControl.UserAccounts;
using Authenticator.API.Core.Domain.MultiTenant.Tenant;
using System.Security.Claims;

namespace Authenticator.API.Core.Application.Interfaces;

/// <summary>
/// Interface para o serviço de tokens JWT
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Gera um token JWT para o usuário
    /// </summary>
    /// <param name="user">Usuário autenticado</param>
    /// <param name="tenant">Tenant (opcional)</param>
    /// <param name="accessGroups">Grupos de acesso do usuário</param>
    /// <param name="roles">Roles do usuário</param>
    /// <param name="permissions">Permissões do usuário</param>
    /// <returns>Token JWT gerado</returns>
    string GenerateAccessToken(UserAccountEntity user, TenantEntity? tenant, List<string> accessGroups, List<string> roles, List<string> permissions);

    /// <summary>
    /// Gera um refresh token
    /// </summary>
    /// <returns>Refresh token gerado</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Obtém os claims de um token JWT válido
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Claims do token</returns>
    ClaimsPrincipal? GetPrincipalFromToken(string token);

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Se o token é válido</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Obtém o tempo de expiração configurado para tokens (em segundos)
    /// </summary>
    /// <returns>Tempo de expiração em segundos</returns>
    int GetTokenExpirationTime();
}