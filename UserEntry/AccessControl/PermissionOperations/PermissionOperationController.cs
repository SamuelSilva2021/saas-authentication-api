using Authenticator.API.Core.Application.Interfaces.AccessControl.PermissionOperations;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.AccessControl.PermissionOperations
{
    /// <summary>
    /// Controller para gerenciamento de relações Permissão-Operação
    /// </summary>
    /// <param name="permissionOperationService"></param>
    [Route("api/permission-operations")]
    [ApiController]
    [Authorize]
    public class PermissionOperationController(IPermissionOperationService permissionOperationService) : ControllerBase
    {
        private readonly IPermissionOperationService _permissionOperationService = permissionOperationService;

        /// <summary>
        /// Busca todas as relações Permissão-Operação
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PermissionOperationDTO>>> GetAllPermissionOperationsAsync()
        {
            var response = await _permissionOperationService.GetAllPermissionOperationsAsync();
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Busca relações por ID da permissão
        /// </summary>
        /// <param name="permissionId">ID da permissão</param>
        /// <returns></returns>
        [HttpGet]
        [Route("by-permission/{permissionId:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PermissionOperationDTO>>> GetByPermissionIdAsync([FromRoute] Guid permissionId)
        {
            var response = await _permissionOperationService.GetByPermissionIdAsync(permissionId);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Busca relações por ID da operação
        /// </summary>
        /// <param name="operationId">ID da operação</param>
        /// <returns></returns>
        [HttpGet]
        [Route("by-operation/{operationId:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PermissionOperationDTO>>> GetByOperationIdAsync([FromRoute] Guid operationId)
        {
            var response = await _permissionOperationService.GetByOperationIdAsync(operationId);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Busca uma relação específica entre permissão e operação
        /// </summary>
        /// <param name="permissionId">ID da permissão</param>
        /// <param name="operationId">ID da operação</param>
        /// <returns></returns>
        [HttpGet]
        [Route("permission/{permissionId:guid}/operation/{operationId:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PermissionOperationDTO>> GetByPermissionAndOperationAsync([FromRoute] Guid permissionId, [FromRoute] Guid operationId)
        {
            var response = await _permissionOperationService.GetByPermissionAndOperationAsync(permissionId, operationId);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Cria uma nova relação Permissão-Operação
        /// </summary>
        /// <param name="permissionOperation">Dados da relação</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PermissionOperationDTO>> CreatePermissionOperationAsync([FromBody] PermissionOperationCreateDTO permissionOperation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _permissionOperationService.CreatePermissionOperationAsync(permissionOperation);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Cria múltiplas relações Permissão-Operação
        /// </summary>
        /// <param name="permissionOperations">Dados das relações</param>
        /// <returns></returns>
        [HttpPost]
        [Route("bulk")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PermissionOperationDTO>>> CreatePermissionOperationsBulkAsync([FromBody] PermissionOperationBulkDTO permissionOperations)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _permissionOperationService.CreatePermissionOperationsBulkAsync(permissionOperations);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Atualiza uma relação Permissão-Operação
        /// </summary>
        /// <param name="id">ID da relação</param>
        /// <param name="permissionOperation">Dados atualizados da relação</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PermissionOperationDTO>> UpdatePermissionOperationAsync([FromRoute] Guid id, [FromBody] PermissionOperationUpdateDTO permissionOperation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _permissionOperationService.UpdatePermissionOperationAsync(id, permissionOperation);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Remove uma relação Permissão-Operação (soft delete)
        /// </summary>
        /// <param name="id">ID da relação</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeletePermissionOperationAsync([FromRoute] Guid id)
        {
            var response = await _permissionOperationService.DeletePermissionOperationAsync(id);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Remove todas as relações de uma permissão (soft delete)
        /// </summary>
        /// <param name="permissionId">ID da permissão</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("permission/{permissionId:guid}/all")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteAllByPermissionIdAsync([FromRoute] Guid permissionId)
        {
            var response = await _permissionOperationService.DeleteAllByPermissionIdAsync(permissionId);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Remove relações específicas de uma permissão (soft delete)
        /// </summary>
        /// <param name="permissionId">ID da permissão</param>
        /// <param name="operationIds">Lista de IDs das operações</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("permission/{permissionId:guid}/operations")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteByPermissionAndOperationsAsync([FromRoute] Guid permissionId, [FromBody] IEnumerable<Guid> operationIds)
        {
            if (operationIds == null || !operationIds.Any())
            {
                return BadRequest("Lista de operações não pode estar vazia");
            }

            var response = await _permissionOperationService.DeleteByPermissionAndOperationsAsync(permissionId, operationIds);
            return StatusCode(response.Code, response);
        }
    }
}