using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Domain.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Authenticator.API.UserEntry.Auth;

/// <summary>
/// Controller de autenticação e autorização
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("🔐 Autenticação")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authenticationService, ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    /// <summary>
    /// Autentica um usuário e retorna o token JWT
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <returns>Token JWT e informações do usuário</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Fazer login",
        Description = "Autentica usuário com email/username e senha, retornando tokens JWT",
        OperationId = "Login",
        Tags = new[] { "🔐 Autenticação" }
    )]
    [SwaggerResponse(200, "Login realizado com sucesso", typeof(ApiResponse<LoginResponse>))]
    [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ApiResponse<LoginResponse>))]
    [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<LoginResponse>))]
    [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<LoginResponse>))]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 401)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorResponse = ApiResponse<LoginResponse>.ErrorResult("Dados inválidos", errors);
                return BadRequest(errorResponse);
            }

            var result = await _authenticationService.LoginAsync(
                request.UsernameOrEmail,
                request.Password);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            _logger.LogInformation("Login realizado com sucesso para usuário: {Username}", request.UsernameOrEmail);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login");
            var errorResponse = ApiResponse<LoginResponse>.ErrorResult("Erro interno do servidor");
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Renova o token de acesso usando o refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>Novos tokens</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 400)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), 401)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorResponse = ApiResponse<LoginResponse>.ErrorResult("Dados inválidos", errors);
                return BadRequest(errorResponse);
            }

            var result = await _authenticationService.RefreshTokenAsync(request.RefreshToken);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            _logger.LogInformation("Token renovado com sucesso");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante renovação de token");
            var errorResponse = ApiResponse<LoginResponse>.ErrorResult("Erro interno do servidor");
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Realiza logout revogando o refresh token
    /// </summary>
    /// <param name="request">Refresh token para revogar</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorResponse = ApiResponse<bool>.ErrorResult("Dados inválidos", errors);
                return BadRequest(errorResponse);
            }

            var result = await _authenticationService.RevokeTokenAsync(request.RefreshToken);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Logout realizado com sucesso para usuário: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout");
            var errorResponse = ApiResponse<bool>.ErrorResult("Erro interno do servidor");
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Busca as permissões do usuário autenticado
    /// </summary>
    /// <param name="tenantSlug">Slug do tenant (opcional)</param>
    /// <returns>Informações completas do usuário incluindo permissões</returns>
    [HttpGet("permissions")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Obter permissões do usuário",
        Description = "Retorna todas as permissões e informações detalhadas do usuário autenticado",
        OperationId = "GetUserPermissions",
        Tags = new[] { "🔐 Autenticação" }
    )]
    [SwaggerResponse(200, "Permissões obtidas com sucesso", typeof(ApiResponse<UserInfo>))]
    [SwaggerResponse(401, "Token JWT inválido ou expirado", typeof(ApiResponse<UserInfo>))]
    [SwaggerResponse(404, "Usuário não encontrado", typeof(ApiResponse<UserInfo>))]
    [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<UserInfo>))]
    [ProducesResponseType(typeof(ApiResponse<UserInfo>), 200)]
    [ProducesResponseType(typeof(ApiResponse<UserInfo>), 401)]
    [ProducesResponseType(typeof(ApiResponse<UserInfo>), 404)]
    public async Task<ActionResult<ApiResponse<UserInfo>>> GetUserPermissions([FromQuery] string? tenantSlug = null)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                var errorResponse = ApiResponse<UserInfo>.ErrorResult("Usuário não encontrado no token");
                return Unauthorized(errorResponse);
            }

            var result = await _authenticationService.GetUserInfoAsync(userId, tenantSlug);

            if (!result.Success)
            {
                return NotFound(result);
            }

            _logger.LogInformation("Permissões obtidas com sucesso para usuário: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter permissões do usuário");
            var errorResponse = ApiResponse<UserInfo>.ErrorResult("Erro interno do servidor");
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Valida se o token atual é válido
    /// </summary>
    /// <returns>Informações básicas do token</returns>
    [HttpGet("validate")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public ActionResult<ApiResponse<object>> ValidateToken()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            var tenantSlugClaim = User.FindFirst("tenant_slug")?.Value;

            var tokenInfo = new
            {
                UserId = userIdClaim,
                Username = usernameClaim,
                Email = emailClaim,
                TenantId = tenantIdClaim,
                TenantSlug = tenantSlugClaim,
                IsValid = true,
                ValidatedAt = DateTime.UtcNow
            };

            var response = ApiResponse<object>.SuccessResult(tokenInfo, "Token válido");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar token");
            var errorResponse = ApiResponse<object>.ErrorResult("Erro interno do servidor");
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Obtém informações básicas do usuário autenticado
    /// </summary>
    /// <returns>Informações do usuário</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserInfo>), 200)]
    [ProducesResponseType(typeof(ApiResponse<UserInfo>), 401)]
    [ProducesResponseType(typeof(ApiResponse<UserInfo>), 404)]
    public async Task<ActionResult<ApiResponse<UserInfo>>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tenantSlugClaim = User.FindFirst("tenant_slug")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                var errorResponse = ApiResponse<UserInfo>.ErrorResult("Usuário não encontrado no token");
                return Unauthorized(errorResponse);
            }

            var result = await _authenticationService.GetUserInfoAsync(userId, tenantSlugClaim);

            if (!result.Success)
            {
                return NotFound(result);
            }

            _logger.LogInformation("Informações do usuário obtidas com sucesso: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações do usuário");
            var errorResponse = ApiResponse<UserInfo>.ErrorResult("Erro interno do servidor");
            return StatusCode(500, errorResponse);
        }
    }
}