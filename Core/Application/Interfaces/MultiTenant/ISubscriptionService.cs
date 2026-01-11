using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant;

public interface ISubscriptionService
{
    Task<ResponseDTO<string>> CreateCheckoutSessionAsync();
    Task<ResponseDTO<string>> ActivateTrialAsync();
    Task<ResponseDTO<SubscriptionDTO>> GetCurrentSubscriptionAsync();
    Task<ResponseDTO<SubscriptionDTO>> CreateAsync(CreateSubscriptionDTO dto);
    Task<ResponseDTO<SubscriptionDTO>> UpdateAsync(Guid id, UpdateSubscriptionDTO dto);
    Task<ResponseDTO<bool>> CancelAsync(Guid id);
}
