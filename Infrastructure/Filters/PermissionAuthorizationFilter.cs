using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Infrastructure.Configurations;
using Authenticator.API.Infrastructure.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace Authenticator.API.Infrastructure.Filters
{
    /// <summary>
    /// Filtro global de autorização baseado em permissões.
    /// Lê o atributo [MapPermission(module, operation)] aplicado em actions ou controladores
    /// e valida se o usuário autenticado possui a permissão necessária.
    /// 
    /// Estratégia:
    /// 1) Busca o usuário e suas permissões via cache usando IAuthenticationService.GetUserInfoAsync
    /// 2) Caso o cache/serviço falhe, realiza fallback para os claims do JWT ("permission")
    /// 3) Atualmente as permissões no token representam o nome do módulo (Permission.Name).
    ///    Assim, a validação considera o módulo informado em [MapPermission]. A operação
    ///    é registrada para auditoria, mas não influencia na decisão enquanto o token não
    ///    incluir operações por permissão.
    /// </summary>
    public class PermissionAuthorizationFilter : IAsyncActionFilter
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ITenantContext _tenantContext;
        private readonly ILogger<PermissionAuthorizationFilter> _logger;

        public PermissionAuthorizationFilter(
            IAuthenticationService authenticationService,
            ITenantContext tenantContext,
            ILogger<PermissionAuthorizationFilter> logger)
        {
            _authenticationService = authenticationService;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor == null)
            {
                await next();
                return;
            }

            // Tenta obter o atributo [MapPermission] da action; se não houver, verifica no controller
            var mapAttr = descriptor.MethodInfo
                .GetCustomAttributes(typeof(MapPermission), inherit: true)
                .OfType<MapPermission>()
                .FirstOrDefault()
                ?? descriptor.ControllerTypeInfo
                    .GetCustomAttributes(typeof(MapPermission), inherit: true)
                    .OfType<MapPermission>()
                    .FirstOrDefault();

            // Se não houver mapeamento de permissão, segue o fluxo normal
            if (mapAttr == null)
            {
                await next();
                return;
            }

            // Garante que está autenticado
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Obtém UserId do claim (preferência pelo ClaimTypes.NameIdentifier)
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? context.HttpContext.User.FindFirst("user_id")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("UserId inválido nos claims ao validar permissão {Module}/{Operation}", mapAttr.Module, mapAttr.Operation);
                context.Result = new UnauthorizedResult();
                return;
            }

            var tenantSlug = _tenantContext.TenantSlug; // populado pelo TenantContextMiddleware
            var requiredModuleName = mapAttr.Module?.Trim();
            var requiredOperation = mapAttr.Operation?.Trim();

            try
            {
                // 1) Tenta obter permissões via serviço (com cache interno)
                var userInfoResponse = await _authenticationService.GetUserInfoAsync(userId, tenantSlug);
                List<string> permissions = [];
                IEnumerable<ModuleBasicDTO> modules = [];

                if (userInfoResponse.Succeeded && userInfoResponse.Data != null)
                {
                    var accessGroup = userInfoResponse.Data.Permissions.AccessGroups;
                    var roles = accessGroup.SelectMany(ag => ag.Roles).ToList();
                    modules = roles.SelectMany(r => r.Modules).DistinctBy(m => m.Key).ToList();
                }
                else
                {
                    // 2) Fallback: extrai permissões dos claims do JWT
                    permissions = context.HttpContext.User.FindAll("permission")
                        .Select(c => c.Value)
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }

                var moduleToValidate = modules.FirstOrDefault(m => string.Equals(m.Key, requiredModuleName, StringComparison.OrdinalIgnoreCase));
                var operationsToModule = moduleToValidate.Operations;

                var hasPermissionModule = modules.Any(p => string.Equals(p.Key, requiredModuleName, StringComparison.OrdinalIgnoreCase));
                var hasPermissionOperation = operationsToModule.Any(o => string.Equals(o, requiredOperation, StringComparison.OrdinalIgnoreCase));

                if (!hasPermissionModule)
                {
                    _logger.LogWarning(
                        "Acesso negado. Usuário {UserId} não possui permissão para {Module}/{Operation} no tenant {Tenant}",
                        userId, requiredModuleName, requiredOperation, tenantSlug);
                    context.Result = new ForbidResult();
                    return;
                }

                if(!hasPermissionOperation)
                {
                    _logger.LogWarning(
                        "Acesso negado. Usuário {UserId} não possui permissão para operação {Operation} no módulo {Module} no tenant {Tenant}",
                        userId, requiredOperation, requiredModuleName, tenantSlug);
                    context.Result = new ForbidResult();
                    return;
                }

                // Log informativo (auditoria)
                _logger.LogInformation(
                    "Acesso permitido. Usuário {UserId} possui permissão para {Module}/{Operation} no tenant {Tenant}",
                    userId, requiredModuleName, requiredOperation, tenantSlug);

                await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar permissão {Module}/{Operation} para usuário {UserId}", requiredModuleName, requiredOperation, userId);
                context.Result = new ObjectResult(ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Erro na validação de permissão" })
                        .WithException(ex)
                        .WithCode(500)
                        .Build())
                {
                    StatusCode = 500
                };
            }
        }
    }
}