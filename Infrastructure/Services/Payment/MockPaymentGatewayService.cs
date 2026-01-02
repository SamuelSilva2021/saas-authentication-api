using Authenticator.API.Core.Application.Interfaces.Payment;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;

namespace Authenticator.API.Infrastructure.Services.Payment
{
    public class MockPaymentGatewayService : IPaymentGatewayService
    {
        public Task<string> CreateCheckoutSessionAsync(SubscriptionEntity subscription, decimal amount, string successUrl, string cancelUrl)
        {
            // Simula a criação de uma sessão de checkout retornando uma URL fictícia
            // Em produção, aqui chamaria Stripe/Pagar.me
            var sessionId = Guid.NewGuid().ToString();
            return Task.FromResult($"https://mock-payment-gateway.com/checkout/{sessionId}?success={successUrl}&cancel={cancelUrl}");
        }

        public Task<bool> VerifyPaymentAsync(string sessionId)
        {
            // Simula verificação de pagamento sempre bem sucedida
            return Task.FromResult(true);
        }
    }
}
