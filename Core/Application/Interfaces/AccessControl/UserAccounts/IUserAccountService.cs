using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.UserAccounts
{
    /// <summary>
    /// Serviço para gerenciamento de contas de usuário
    /// </summary>
    public interface IUserAccountService
    {
        /// <summary>
        /// Lista usuários (do tenant atual) com paginação
        /// </summary>
        Task<ResponseDTO<PagedResponseDTO<UserAccountDTO>>> GetAllUserAccountsPagedAsync(int page, int limit);

        /// <summary>
        /// Lista todos os usuários ativos do tenant atual
        /// </summary>
        Task<ResponseDTO<IEnumerable<UserAccountDTO>>> GetAllActiveUsersAsync();

        /// <summary>
        /// Obtém detalhes de um usuário por ID (escopo do tenant atual)
        /// </summary>
        Task<ResponseDTO<UserAccountDTO>> GetUserAccountByIdAsync(Guid id);

        /// <summary>
        /// Cria um novo usuário (no tenant atual)
        /// </summary>
        Task<ResponseDTO<UserAccountDTO>> AddUserAccountAsync(UserAccountCreateDTO dto);

        /// <summary>
        /// Atualiza dados de um usuário
        /// </summary>
        Task<ResponseDTO<UserAccountDTO>> UpdateUserAccountAsync(Guid id, UserAccountUpdateDTO dto);

        /// <summary>
        /// Remove (hard delete) um usuário
        /// </summary>
        Task<ResponseDTO<bool>> DeleteUserAccountAsync(Guid id);

        /// <summary>
        /// Altera a senha do usuário autenticado
        /// </summary>
        //Task<ResponseDTO<bool>> ChangePasswordAsync(UserAccountChangePasswordDTO dto);

        /// <summary>
        /// Inicia fluxo de esqueci a senha (gera token)
        /// </summary>
        Task<ResponseDTO<bool>> ForgotPasswordAsync(UserAccountForgotPasswordDTO dto);

        /// <summary>
        /// Reseta a senha usando token de reset
        /// </summary>
        Task<ResponseDTO<bool>> ResetPasswordAsync(UserAccountResetPasswordDTO dto);
    }
}