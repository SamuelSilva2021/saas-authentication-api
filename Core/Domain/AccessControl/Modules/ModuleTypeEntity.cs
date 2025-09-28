using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Modules
{
    /// <summary>
    /// Tipo de módulo/funcionalidade do sistema
    /// </summary>
    public class ModuleTypeEntity
    {
        /// <summary>
        /// ID único do tipo de módulo
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Nome do tipo de módulo
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Descrição do tipo de módulo
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Código do tipo de módulo
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? Code { get; set; } = string.Empty;
        /// <summary>
        /// Se o tipo de módulo está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Data de criação do tipo de módulo
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        /// <summary>
        /// Data da última atualização do tipo de módulo
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>
        /// Data de exclusão do tipo de módulo
        /// </summary>
        public DateTime? DeletedAt { get; set; }
        /// <summary>
        /// Módulos associados a este tipo
        /// </summary>
        public ICollection<ModuleEntity> Modules { get; set; } = new List<ModuleEntity>();
    }
}
