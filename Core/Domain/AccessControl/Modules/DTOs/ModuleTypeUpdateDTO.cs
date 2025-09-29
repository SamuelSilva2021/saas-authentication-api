using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Modules.DTOs
{
    /// <summary>
    /// DTO para atualização de tipo de módulo
    /// </summary>
    public class ModuleTypeUpdateDTO
    {
        /// <summary>
        /// ID único do tipo de módulo
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do tipo de módulo ex: "Administração", "Relatórios", etc.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do tipo de módulo ex: "Módulo responsável pela administração do sistema"
        /// </summary>
        [MaxLength(255)]
        public string? Description { get; set; }

        /// <summary>
        /// Código do tipo de módulo ex: "ADMIN", "REPORTS", etc.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Se o tipo de módulo está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
