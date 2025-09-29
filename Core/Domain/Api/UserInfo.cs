using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;

namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// Informações básicas do usuário autenticado
/// </summary>
public class UserInfo
{
    /// <summary>
    /// ID do usuário
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome de usuário
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Grupos de acesso do usuário
    /// </summary>
    public List<string> AccessGroups { get; set; } = new();

    /// <summary>
    /// Roles do usuário
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Permissões do usuário
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// Informações do tenant (se aplicável)
    /// </summary>
    public TenantInfoDTO? Tenant { get; set; }
}