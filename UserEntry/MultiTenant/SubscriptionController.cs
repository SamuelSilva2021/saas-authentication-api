using Microsoft.AspNetCore.Mvc;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Authenticator.API.UserEntry.MultiTenant
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController(
        ISubscriptionService subscriptionService
        ) : ControllerBase
    {
        [HttpPost("checkout")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO<string>>> CreateCheckoutSession()
        {
            var result = await subscriptionService.CreateCheckoutSessionAsync();
            return StatusCode(result.Code, result);
        }

        [HttpPost("activate-trial")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO<string>>> ActivateTrial()
        {
            var result = await subscriptionService.ActivateTrialAsync();
            return StatusCode(result.Code, result);
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<ActionResult<ResponseDTO<SubscriptionDTO>>> GetCurrent()
        {
            var result = await subscriptionService.GetCurrentSubscriptionAsync();
            return StatusCode(result.Code, result);
        }

        // Endpoints administrativos podem ser adicionados aqui (Create, Update, Cancel)
    }
}
