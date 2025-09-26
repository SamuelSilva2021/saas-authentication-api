using Authenticator.API.Core.Application.Implementation;
using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;

namespace Authenticator.API.Infrastructure.Repositories
{
    public class UserAccountsRepository(
        AccessControlDbContext accessControlContext, 
        MultiTenantDbContext multiTenantContext, 
        IJwtTokenService jwtTokenService, 
        IMemoryCache cache, 
        ILogger<AuthenticationService> logger)
    {
        private readonly AccessControlDbContext _accessControlContext = accessControlContext;
        private readonly MultiTenantDbContext _multiTenantContext = multiTenantContext;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly IMemoryCache _cache = cache;
        private readonly ILogger<AuthenticationService> _logger = logger;


    }
}
