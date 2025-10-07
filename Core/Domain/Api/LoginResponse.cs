namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// Modelo para resposta de login
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Token JWT de acesso
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token de renovação
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do token (normalmente "Bearer")
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Tempo de expiração do token em segundos
    /// </summary>
    public int ExpiresIn { get; set; }
}