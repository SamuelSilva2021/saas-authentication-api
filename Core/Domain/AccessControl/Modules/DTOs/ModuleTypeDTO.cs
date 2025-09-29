namespace Authenticator.API.Core.Domain.AccessControl.Modules.DTOs
{
    /// <summary>
    /// Data Transfer Object para representar o tipo de módulo.
    /// </summary>
    public class ModuleTypeDTO
    {
        /// <summary>
        /// Identificador único do tipo de módulo.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do tipo de módulo. ex: "User Management", "Reporting", "Analytics"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do tipo de módulo. ex: "Módulo responsável pela gestão de usuários", "Módulo de geração de relatórios"
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Código único do tipo de módulo. ex: "USER_MGMT", "REPORTING", "ANALYTICS"
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o tipo de módulo está ativo.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Data e hora de criação do tipo de módulo.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data e hora da última atualização do tipo de módulo.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Data e hora da exclusão do tipo de módulo (se aplicável).
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
