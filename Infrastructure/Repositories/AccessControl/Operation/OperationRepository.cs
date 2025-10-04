using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using Authenticator.API.Core.Domain.AccessControl.Operations;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.Operation
{
    public class OperationRepository(
        IDbContextProvider dbContextProvider,
        ILogger<OperationRepository> logger
        ) : BaseRepository<OperationEntity>(dbContextProvider), IOperationRepository
    {
        private readonly ILogger<OperationRepository> _logger = logger;
    }
}
