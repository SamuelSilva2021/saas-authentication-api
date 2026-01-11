//using Authenticator.API.Core.Application.Interfaces.Auth;
//using Authenticator.API.Core.Application.Interfaces.MultiTenant;
//using Authenticator.API.Core.Application.Interfaces.Payment;
//using Authenticator.API.Core.Domain.Api;
//using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;
//using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;
//using Authenticator.API.Core.Domain.MultiTenant.Tenant; // For ETenantStatus if needed
//using AutoMapper;

//namespace Authenticator.API.Core.Application.Implementation.MultiTenant;

//public class SubscriptionService(
//    ISubscriptionRepository subscriptionRepository,
//    ITenantRepository tenantRepository,
//    IPlanRepository planRepository,
//    IPaymentGatewayService paymentGatewayService,
//    IUserContext userContext,
//    IMapper mapper,
//    ILogger<SubscriptionService> logger) : ISubscriptionService
//{
//    public async Task<ResponseDTO<string>> CreateCheckoutSessionAsync()
//    {
//        try
//        {
//            var tenantId = userContext.CurrentUser?.TenantId;
//            if (tenantId == null || tenantId == Guid.Empty)
//            {
//                return ResponseBuilder<string>.Fail("Tenant não identificado no contexto do usuário");
//            }

//            var subscription = await subscriptionRepository.GetByTenantIdAsync(tenantId.Value);
//            if (subscription == null)
//            {
//                return ResponseBuilder<string>.Fail("Assinatura não encontrada");
//            }

//            var plan = await planRepository.GetByIdAsync(subscription.PlanId);
//            if (plan == null)
//            {
//                return ResponseBuilder<string>.Fail("Plano não encontrado");
//            }

//            var checkoutUrl = await paymentGatewayService.CreateCheckoutSessionAsync(
//                subscription, 
//                plan.Price, 
//                $"http://localhost:5173/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
//                $"http://localhost:5173/payment/cancel"
//            );

//            return ResponseBuilder<string>.Ok(checkoutUrl);
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "Erro ao criar sessão de checkout");
//            return ResponseBuilder<string>.Fail("Erro interno ao processar checkout");
//        }
//    }

//    public async Task<ResponseDTO<string>> ActivateTrialAsync()
//    {
//        try
//        {
//            var tenantId = userContext.CurrentUser?.TenantId;
//            if (tenantId == null || tenantId == Guid.Empty)
//            {
//                return ResponseBuilder<string>.Fail("Tenant não identificado no contexto do usuário");
//            }

//            var subscription = await subscriptionRepository.GetByTenantIdAsync(tenantId.Value);
//            if (subscription == null)
//            {
//                return ResponseBuilder<string>.Fail("Assinatura não encontrada");
//            }

//            if (subscription.TrialEndsAt == null || subscription.TrialEndsAt < DateTime.UtcNow)
//            {
//                return ResponseBuilder<string>.Fail("Período de trial expirado ou inválido");
//            }

//            // Ativar Tenant
//            var tenant = await tenantRepository.GetByIdAsync(tenantId.Value);
//            if (tenant != null)
//            {
//                tenant.Status = ETenantStatus.Active;
//                await tenantRepository.UpdateAsync(tenant);
//            }
            
//            // Atualizar status da assinatura se necessário
//            subscription.Status = "active"; // Ou enum correspondente
//            await subscriptionRepository.UpdateAsync(subscription);

//            return ResponseBuilder<string>.Ok("Trial ativado com sucesso", "Trial ativado com sucesso");
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "Erro ao ativar trial");
//            return ResponseBuilder<string>.Fail("Erro interno ao ativar trial");
//        }
//    }

//    public async Task<ResponseDTO<SubscriptionDTO>> GetCurrentSubscriptionAsync()
//    {
//        var tenantId = userContext.CurrentUser?.TenantId;
//        if (tenantId == null || tenantId == Guid.Empty)
//            return ResponseBuilder<SubscriptionDTO>.Fail("Tenant não identificado");

//        var subscription = await subscriptionRepository.GetByTenantIdAsync(tenantId.Value);
//        if (subscription == null)
//            return ResponseBuilder<SubscriptionDTO>.Fail("Assinatura não encontrada");

//        return ResponseBuilder<SubscriptionDTO>.Ok(mapper.Map<SubscriptionDTO>(subscription));
//    }

//    public async Task<ResponseDTO<SubscriptionDTO>> CreateAsync(CreateSubscriptionDTO dto)
//    {
//        try 
//        {
//            // Lógica de criação manual (ex: por admin)
//            var entity = mapper.Map<SubscriptionEntity>(dto);
//            entity.CreatedAt = DateTime.UtcNow;
            
//            var created = await subscriptionRepository.AddAsync(entity);
//            return ResponseBuilder<SubscriptionDTO>.Ok(mapper.Map<SubscriptionDTO>(created));
//        }
//        catch(Exception ex)
//        {
//            logger.LogError(ex, "Erro ao criar assinatura");
//            return ResponseBuilder<SubscriptionDTO>.Fail("Erro ao criar assinatura");
//        }
//    }

//    public async Task<ResponseDTO<SubscriptionDTO>> UpdateAsync(Guid id, UpdateSubscriptionDTO dto)
//    {
//        try
//        {
//            var entity = await subscriptionRepository.GetByIdAsync(id);
//            if (entity == null) return ResponseBuilder<SubscriptionDTO>.Fail("Assinatura não encontrada");

//            mapper.Map(dto, entity);
//            entity.UpdatedAt = DateTime.UtcNow;
            
//            await subscriptionRepository.UpdateAsync(entity);
//            return ResponseBuilder<SubscriptionDTO>.Ok(mapper.Map<SubscriptionDTO>(entity));
//        }
//        catch(Exception ex)
//        {
//            logger.LogError(ex, "Erro ao atualizar assinatura");
//            return ResponseBuilder<SubscriptionDTO>.Fail("Erro ao atualizar assinatura");
//        }
//    }

//    public async Task<ResponseDTO<bool>> CancelAsync(Guid id)
//    {
//        try
//        {
//            var entity = await subscriptionRepository.GetByIdAsync(id);
//            if (entity == null) return ResponseBuilder<bool>.Fail("Assinatura não encontrada");

//            entity.Status = "canceled";
//            entity.CanceledAt = DateTime.UtcNow;
//            entity.UpdatedAt = DateTime.UtcNow;

//            await subscriptionRepository.UpdateAsync(entity);
//            return ResponseBuilder<bool>.Ok(true, "Assinatura cancelada com sucesso");
//        }
//        catch(Exception ex)
//        {
//            logger.LogError(ex, "Erro ao cancelar assinatura");
//            return ResponseBuilder<bool>.Fail("Erro ao cancelar assinatura");
//        }
//    }
//}
