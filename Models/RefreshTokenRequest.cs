using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Models;

/// <summary>
/// Modelo para requisição de renovação de token
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Token de renovação
    /// </summary>
    [Required(ErrorMessage = "Refresh token é obrigatório")]
    public string RefreshToken { get; set; } = string.Empty;
}