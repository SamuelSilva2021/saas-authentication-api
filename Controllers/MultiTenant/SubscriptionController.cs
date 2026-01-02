using Microsoft.AspNetCore.Mvc;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Application.Interfaces.Payment;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Tenant;
using Microsoft.AspNetCore.Authorization;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;
using Authenticator.API.Core.Application.Interfaces.Auth;

namespace Authenticator.API.Controllers.MultiTenant
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController(
        ISubscriptionRepository subscriptionRepository,
        ITenantRepository tenantRepository,
        IPlanRepository planRepository,
        IPaymentGatewayService paymentGatewayService,
        IUserContext userContext,
        ILogger<SubscriptionController> logger
        ) : ControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly IPlanRepository _planRepository = planRepository;
        private readonly IPaymentGatewayService _paymentGatewayService = paymentGatewayService;
        private readonly IUserContext _userContext = userContext;
        private readonly ILogger<SubscriptionController> _logger = logger;

        [HttpPost("checkout")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO<string>>> CreateCheckoutSession()
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (tenantId == null || tenantId == Guid.Empty)
                {
                    return BadRequest(ResponseBuilder<string>.Fail(new ErrorDTO { Message = "Tenant não identificado no contexto do usuário" }).Build());
                }

                var subscription = await _subscriptionRepository.GetByTenantIdAsync(tenantId.Value);
                if (subscription == null)
                {
                    return NotFound(ResponseBuilder<string>.Fail(new ErrorDTO { Message = "Assinatura não encontrada" }).Build());
                }

                var plan = await _planRepository.GetByIdAsync(subscription.PlanId);
                if (plan == null)
                {
                    return NotFound(ResponseBuilder<string>.Fail(new ErrorDTO { Message = "Plano não encontrado" }).Build());
                }

                var checkoutUrl = await _paymentGatewayService.CreateCheckoutSessionAsync(
                    subscription, 
                    plan.Price, 
                    $"http://localhost:5173/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
                    $"http://localhost:5173/payment/cancel"
                );

                return Ok(ResponseBuilder<string>.Ok(checkoutUrl).Build());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar sessão de checkout");
                return StatusCode(500, ResponseBuilder<string>.Fail(new ErrorDTO { Message = "Erro interno" }).Build());
            }
        }

        [HttpPost("activate-trial")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO<string>>> ActivateTrial()
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (tenantId == null || tenantId == Guid.Empty)
                {
                    return BadRequest(ResponseBuilder<string>.Fail(new ErrorDTO { Message = "Tenant não identificado no contexto do usuário" }).Build());
                }

                var subscription = await _subscriptionRepository.GetByTenantIdAsync(tenantId.Value);
                if (subscription == null)
                {
                    return NotFound(ResponseBuilder<string>.Fail(new ErrorDTO { Message = "Assinatura não encontrada" }).Build());
                }

                if (subscription.TrialEndsAt == null || subscription.TrialEndsAt < DateTime.UtcNow)
                {
                    return BadRequest(ResponseBuilder<string>.Fail(new ErrorDTO { Message = "Período de trial expirado ou inválido" }).Build());
                }

                // Ativar Tenant
                var tenant = await _tenantRepository.GetByIdAsync(tenantId.Value);
                if (tenant != null)
                {
                    tenant.Status = ETenantStatus.Active;
                    await _tenantRepository.UpdateAsync(tenant);
                }

                subscription.Status = "active";
                await _subscriptionRepository.UpdateAsync(subscription);

                return Ok(ResponseBuilder<string>.Ok("Trial ativado com sucesso").Build());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ativar trial");
                return StatusCode(500, ResponseBuilder<string>.Fail(new ErrorDTO { Message = "Erro interno" }).Build());
            }
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> Webhook([FromBody] dynamic payload)
        {
            // Em uma implementação real, verificar assinatura do webhook do gateway
            try
            {
                // Simulação: Recebe payload com status "paid" e tenantId
                // Exemplo payload: { "type": "payment_intent.succeeded", "data": { "object": { "metadata": { "tenantId": "..." } } } }
                
                // Mock implementation for demo purposes
                string eventType = payload.GetProperty("type").ToString();
                
                if (eventType == "payment_success")
                {
                    string tenantIdStr = payload.GetProperty("data").GetProperty("tenantId").ToString();
                    if (Guid.TryParse(tenantIdStr, out var tenantId))
                    {
                        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
                        if (tenant != null)
                        {
                            tenant.Status = ETenantStatus.Active;
                            await _tenantRepository.UpdateAsync(tenant);
                            
                            var subscription = await _subscriptionRepository.GetByTenantIdAsync(tenantId);
                            if (subscription != null)
                            {
                                subscription.Status = "active";
                                subscription.CurrentPeriodStart = DateTime.UtcNow;
                                subscription.CurrentPeriodEnd = DateTime.UtcNow.AddMonths(1); // Assumindo mensal
                                await _subscriptionRepository.UpdateAsync(subscription);
                            }
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar webhook");
                return BadRequest();
            }
        }
        
        // Endpoint para simular sucesso de pagamento (útil para testes manuais sem gateway real)
        [HttpPost("simulate-success/{tenantId}")]
        [Authorize] // Restringir a admins
        public async Task<ActionResult<ResponseDTO<bool>>> SimulatePaymentSuccess(Guid tenantId)
        {
             var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            if (tenant == null) return NotFound(ResponseBuilder<bool>.Fail(new ErrorDTO { Message = "Tenant não encontrado" }).Build());

            tenant.Status = ETenantStatus.Active;
            await _tenantRepository.UpdateAsync(tenant);
            
            var subscription = await _subscriptionRepository.GetByTenantIdAsync(tenantId);
            if (subscription != null)
            {
                subscription.Status = "active";
                subscription.CurrentPeriodStart = DateTime.UtcNow;
                subscription.CurrentPeriodEnd = DateTime.UtcNow.AddMonths(1);
                await _subscriptionRepository.UpdateAsync(subscription);
            }

            return Ok(ResponseBuilder<bool>.Ok(true).Build());
        }
    }
}
