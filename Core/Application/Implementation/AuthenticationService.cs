using Authenticator.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using BCrypt.Net;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Tenant;
using Authenticator.API.Core.Application.Interfaces;

namespace Authenticator.API.Core.Application.Implementation;

/// <summary>
/// Implementação do serviço de autenticação
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly AccessControlDbContext _accessControlContext;
    private readonly MultiTenantDbContext _multiTenantContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AuthenticationService> _logger;

    // Chaves para cache
    private const string REFRESH_TOKENS_CACHE_KEY = "refresh_tokens";
    private const string USER_CACHE_KEY_PREFIX = "user_";

    public AuthenticationService(
        AccessControlDbContext accessControlContext,
        MultiTenantDbContext multiTenantContext,
        IJwtTokenService jwtTokenService,
        IMemoryCache cache,
        ILogger<AuthenticationService> logger)
    {
        _accessControlContext = accessControlContext;
        _multiTenantContext = multiTenantContext;
        _jwtTokenService = jwtTokenService;
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ApiResponse<LoginResponse>> LoginAsync(string usernameOrEmail, string password)
    {
        try
        {
            _logger.LogInformation("Tentativa de login para: {UsernameOrEmail}", usernameOrEmail);

            var user = await _accessControlContext.UserAccounts
                .Where(u => (u.Username == usernameOrEmail || u.Email == usernameOrEmail)
                           && u.IsActive && u.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado: {UsernameOrEmail}", usernameOrEmail);
                return ApiResponse<LoginResponse>.ErrorResult("Credenciais inválidas");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Senha incorreta para usuário: {UserId}", user.Id);
                return ApiResponse<LoginResponse>.ErrorResult("Credenciais inválidas");
            }

            var tenant = _multiTenantContext.Tenants.Where(t => t.Id == user.TenantId).FirstOrDefault();

            var accessGroups = await GetUserAccessGroupsAsync(user.Id);
            var roles = await GetUserRolesAsync(accessGroups);
            var permissions = await GetUserPermissionsAsync(roles);

            var accessToken = _jwtTokenService.GenerateAccessToken(user, tenant, accessGroups, roles, permissions);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            await StoreRefreshTokenAsync(refreshToken, user.Id, tenant?.Id);

            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _accessControlContext.SaveChangesAsync();

            var loginResponse = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtTokenService.GetTokenExpirationTime(),
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    AccessGroups = accessGroups,
                    Roles = roles,
                    Permissions = permissions,
                    Tenant = tenant != null ? new TenantInfo
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        Slug = tenant.Slug,
                        CustomDomain = tenant.Domain
                    } : null
                }
            };

            _logger.LogInformation("Login bem-sucedido para usuário: {UserId}", user.Id);
            return ApiResponse<LoginResponse>.SuccessResult(loginResponse, "Login realizado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login para: {UsernameOrEmail}", usernameOrEmail);
            return ApiResponse<LoginResponse>.ErrorResult("Erro interno do servidor");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            _logger.LogInformation("Tentativa de renovação de token");

            var tokenData = await GetRefreshTokenDataAsync(refreshToken);
            if (tokenData == null)
            {
                _logger.LogWarning("Refresh token inválido ou expirado");
                return ApiResponse<LoginResponse>.ErrorResult("Token de renovação inválido");
            }

            var user = await _accessControlContext.UserAccounts
                .Where(u => u.Id == tokenData.UserId && u.IsActive && u.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado para refresh token: {UserId}", tokenData.UserId);
                return ApiResponse<LoginResponse>.ErrorResult("Usuário não encontrado");
            }

            TenantEntity? tenant = null;
            if (tokenData.TenantId.HasValue)
            {
                tenant = await _multiTenantContext.Tenants
                    .Where(t => t.Id == tokenData.TenantId && t.Status == "active" && t.DeletedAt == null)
                    .FirstOrDefaultAsync();
            }

            var accessGroups = await GetUserAccessGroupsAsync(user.Id);
            var roles = await GetUserRolesAsync(accessGroups);
            var permissions = await GetUserPermissionsAsync(roles);

            var newAccessToken = _jwtTokenService.GenerateAccessToken(user, tenant, accessGroups, roles, permissions);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            await RevokeRefreshTokenAsync(refreshToken);
            await StoreRefreshTokenAsync(newRefreshToken, user.Id, tenant?.Id);

            var loginResponse = new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = _jwtTokenService.GetTokenExpirationTime(),
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    AccessGroups = accessGroups,
                    Roles = roles,
                    Permissions = permissions,
                    Tenant = tenant != null ? new TenantInfo
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        Slug = tenant.Slug,
                        CustomDomain = tenant.Domain
                    } : null
                }
            };

            _logger.LogInformation("Token renovado com sucesso para usuário: {UserId}", user.Id);
            return ApiResponse<LoginResponse>.SuccessResult(loginResponse, "Token renovado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante renovação de token");
            return ApiResponse<LoginResponse>.ErrorResult("Erro interno do servidor");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken)
    {
        try
        {
            await RevokeRefreshTokenAsync(refreshToken);
            _logger.LogInformation("Refresh token revogado com sucesso");
            return ApiResponse<bool>.SuccessResult(true, "Logout realizado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao revogar token");
            return ApiResponse<bool>.ErrorResult("Erro ao realizar logout");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResponse<UserInfo>> GetUserInfoAsync(Guid userId, string? tenantSlug = null)
    {
        try
        {
            // Busca no cache primeiro
            var cacheKey = $"{USER_CACHE_KEY_PREFIX}{userId}_{tenantSlug ?? "no_tenant"}";
            if (_cache.TryGetValue(cacheKey, out UserInfo? cachedUserInfo))
            {
                return ApiResponse<UserInfo>.SuccessResult(cachedUserInfo!, "Informações do usuário obtidas com sucesso");
            }

            var user = await _accessControlContext.UserAccounts
                .Where(u => u.Id == userId && u.IsActive && u.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return ApiResponse<UserInfo>.ErrorResult("Usuário não encontrado");
            }

            TenantEntity? tenant = null;
            if (!string.IsNullOrEmpty(tenantSlug))
            {
                tenant = await _multiTenantContext.Tenants
                    .Where(t => t.Slug == tenantSlug && t.Status == "active" && t.DeletedAt == null)
                    .FirstOrDefaultAsync();
            }

            var accessGroups = await GetUserAccessGroupsAsync(user.Id);
            var roles = await GetUserRolesAsync(accessGroups);
            var permissions = await GetUserPermissionsAsync(roles);

            var userInfo = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                AccessGroups = accessGroups,
                Roles = roles,
                Permissions = permissions,
                Tenant = tenant != null ? new TenantInfo
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Slug = tenant.Slug,
                    CustomDomain = tenant.Domain
                } : null
            };

            // Armazena no cache por 15 minutos
            _cache.Set(cacheKey, userInfo, TimeSpan.FromMinutes(15));

            return ApiResponse<UserInfo>.SuccessResult(userInfo, "Informações do usuário obtidas com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações do usuário: {UserId}", userId);
            return ApiResponse<UserInfo>.ErrorResult("Erro ao obter informações do usuário");
        }
    }

    /// <inheritdoc />
    public async Task<bool> ValidateTokenAsync(string token)
    {
        return await Task.FromResult(_jwtTokenService.ValidateToken(token));
    }

    #region Métodos auxiliares privados

    private async Task<List<string>> GetUserAccessGroupsAsync(Guid userId)
    {
        return await _accessControlContext.AccountAccessGroups
            .Where(aag => aag.UserAccountId == userId && aag.IsActive)
            .Include(aag => aag.AccessGroup)
            .Where(aag => aag.AccessGroup.IsActive && aag.AccessGroup.DeletedAt == null)
            .Select(aag => aag.AccessGroup.Name)
            .ToListAsync();
    }

    private async Task<List<string>> GetUserRolesAsync(List<string> accessGroups)
    {
        return await _accessControlContext.RoleAccessGroups
            .Where(rag => accessGroups.Contains(rag.AccessGroup.Name) && rag.IsActive)
            .Include(rag => rag.Role)
            .Where(rag => rag.Role.IsActive && rag.Role.DeletedAt == null)
            .Select(rag => rag.Role.Name)
            .Distinct()
            .ToListAsync();
    }

    private async Task<List<string>> GetUserPermissionsAsync(List<string> roles)
    {
        return await _accessControlContext.Permissions
            .Where(p => roles.Contains(p.Role!.Name) && p.IsActive && p.DeletedAt == null)
            .Include(p => p.Role)
            .Where(p => p.Role!.IsActive && p.Role.DeletedAt == null)
            .Select(p => p.Name)
            .Distinct()
            .ToListAsync();
    }

    private async Task StoreRefreshTokenAsync(string refreshToken, Guid userId, Guid? tenantId)
    {
        var tokenData = new RefreshTokenData
        {
            UserId = userId,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7) // 7 dias de validade
        };

        if (!_cache.TryGetValue(REFRESH_TOKENS_CACHE_KEY, out Dictionary<string, RefreshTokenData>? tokens))
        {
            tokens = new Dictionary<string, RefreshTokenData>();
        }

        tokens![refreshToken] = tokenData;
        _cache.Set(REFRESH_TOKENS_CACHE_KEY, tokens, TimeSpan.FromDays(7));

        await Task.CompletedTask;
    }

    private async Task<RefreshTokenData?> GetRefreshTokenDataAsync(string refreshToken)
    {
        if (_cache.TryGetValue(REFRESH_TOKENS_CACHE_KEY, out Dictionary<string, RefreshTokenData>? tokens))
        {
            if (tokens!.TryGetValue(refreshToken, out RefreshTokenData? tokenData))
            {
                if (tokenData.ExpiresAt > DateTime.UtcNow)
                {
                    return await Task.FromResult(tokenData);
                }

                // Remove token expirado
                tokens.Remove(refreshToken);
                _cache.Set(REFRESH_TOKENS_CACHE_KEY, tokens, TimeSpan.FromDays(7));
            }
        }

        return await Task.FromResult<RefreshTokenData?>(null);
    }

    private async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        if (_cache.TryGetValue(REFRESH_TOKENS_CACHE_KEY, out Dictionary<string, RefreshTokenData>? tokens))
        {
            tokens!.Remove(refreshToken);
            _cache.Set(REFRESH_TOKENS_CACHE_KEY, tokens, TimeSpan.FromDays(7));
        }

        await Task.CompletedTask;
    }

    #endregion
}

/// <summary>
/// Dados do refresh token para cache
/// </summary>
internal class RefreshTokenData
{
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
