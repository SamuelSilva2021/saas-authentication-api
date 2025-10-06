using Authenticator.API.Core.Domain.AccessControl.AccountAccessGroups.Etities;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.AccountAccessGroups
{
    /// <summary>
    /// Repositório para gerenciar vínculos de grupos de acesso com contas de usuário
    /// </summary>
    public interface IAccountAccessGroupRepository : IBaseRepository<AccountAccessGroupEntity>
    {
        /// <summary>
        /// Lista vínculos ativos de grupos de acesso de um usuário
        /// </summary>
        Task<IEnumerable<AccountAccessGroupEntity>> GetByUserAsync(Guid userId);

        /// <summary>
        /// Atribui (ou reativa) grupos de acesso a um usuário
        /// </summary>
        Task AssignGroupsAsync(Guid userId, IEnumerable<Guid> accessGroupIds, Guid? grantedBy, DateTime? expiresAt);

        /// <summary>
        /// Revoga (desativa) um vínculo de grupo de acesso de um usuário
        /// </summary>
        Task<bool> RevokeAsync(Guid userId, Guid accessGroupId);
    }
}