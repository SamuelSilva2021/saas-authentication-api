using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Modules.DTOs
{
    /// <summary>
    /// DTO para criação de um novo tipo de módulo
    /// </summary>
    public class ModuleTypeCreateDTO
    {
        /// <summary>
        /// Nome do tipo de módulo ex: "Relatórios", "Usuários"
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do tipo de módulo ex: "Módulo responsável por gerenciar relatórios do sistema"
        /// </summary>
        [MaxLength(255)]
        public string? Description { get; set; }

        /// <summary>
        /// Código do tipo de módulo ex: "REPORTS", "USERS"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
    }
}
