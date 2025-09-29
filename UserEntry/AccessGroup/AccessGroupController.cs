using Authenticator.API.Core.Application.Interfaces.AccessGroup;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Authenticator.API.UserEntry.AccessGroup
{
    /// <summary>
    /// Controlador para gerenciar grupos de acesso
    /// </summary>
    [Route("api/access-group")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Grupos de Acesso")]
    [Authorize]
    public class AccessGroupController(
        ILogger<AccessGroupController> logger,
        IGroupTypeService groupTypeService,
        IAccessGroupService accessGroupService
        ) : ControllerBase
    {
        private readonly ILogger<AccessGroupController> _logger = logger;
        private readonly IGroupTypeService _groupTypeService = groupTypeService;
        private readonly IAccessGroupService _accessGroupService = accessGroupService;

        #region GET
        [HttpGet]
        [SwaggerResponse(200, "Lista de grupos de acesso recuperada com sucesso", typeof(ApiResponse<IEnumerable<AccessGroupDTO>>))]
        [SwaggerResponse(404, "Grupos de acesso não encontrado", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<IEnumerable<AccessGroupDTO>>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<IEnumerable<AccessGroupDTO>>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<AccessGroupDTO>>>> GetAllAccessGroups()
        {
            var accessGroups = await _accessGroupService.GetAllAsync();
            if (accessGroups.Data == null || !accessGroups.Data.Any())
                return NotFound(new ApiResponse<IEnumerable<AccessGroupDTO>>());
            return Ok(accessGroups);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Grupo de acesso recuperado com sucesso", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(404, "Grupo de acesso não encontrado", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<AccessGroupDTO>))]
        public async Task<ActionResult<ApiResponse<AccessGroupDTO>>> GetAccessGroupById([FromRoute] Guid id)
        {
            var accessGroup = await _accessGroupService.GetByIdAsync(id);
            if (accessGroup == null)
                return NotFound(new ApiResponse<AccessGroupDTO>());
            return Ok(accessGroup);
        }

        [HttpGet("group-types")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Lista de tipos de grupo recuperada com sucesso", typeof(ApiResponse<IEnumerable<GroupTypeDTO>>))]
        [SwaggerResponse(404, "Tipos de grupo não encontrado", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<IEnumerable<GroupTypeDTO>>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<IEnumerable<GroupTypeDTO>>))]
        public async Task<ActionResult<ApiResponse<IEnumerable<GroupTypeDTO>>>> GetAllGroupTypes()
        {
            var groupTypes = await _groupTypeService.GetAllAsync();
            if (groupTypes.Data == null || !groupTypes.Data.Any())
                return NotFound(new ApiResponse<IEnumerable<GroupTypeDTO>>());
            return Ok(groupTypes);

        }

        [HttpGet("group-types/{id:guid}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Tipo de grupo recuperado com sucesso", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(404, "Tipo de grupo não encontrado", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<GroupTypeDTO>))]
        public async Task<ActionResult<ApiResponse<GroupTypeDTO>>> GetGroupTypeById([FromRoute] Guid id)
        {
            var groupType = await _groupTypeService.GetByIdAsync(id);
            if (groupType == null)
                return NotFound(new ApiResponse<GroupTypeDTO>());
            return Ok(groupType);
        }

        #endregion GET

        #region POST
        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(200, "Grupo de acesso criado com sucesso", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<AccessGroupDTO>))]
        [ProducesResponseType(typeof(ApiResponse<AccessGroupDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<AccessGroupDTO>), 400)]
        [ProducesResponseType(typeof(ApiResponse<AccessGroupDTO>), 401)]
        public async Task<ActionResult<ApiResponse<AccessGroupDTO>>> CreateAccessGroup([FromBody] CreateAccessGroupDTO createAccessGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<AccessGroupDTO> { Success = false, Message = "Dados de entrada inválidos." });
            var response = await _accessGroupService.CreateAsync(createAccessGroupDTO);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("group-types")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Tipo de grupo criado com sucesso", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<GroupTypeDTO>))]
        [ProducesResponseType(typeof(ApiResponse<GroupTypeDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<GroupTypeDTO>), 400)]
        [ProducesResponseType(typeof(ApiResponse<GroupTypeDTO>), 401)]
        public async Task<ActionResult<ApiResponse<GroupTypeDTO>>> CreateGroupType([FromBody] GroupTypeCreateDTO groupTypeCreateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<GroupTypeDTO> { Success = false, Message = "Dados de entrada inválidos." });
            var createdGroupType = await _groupTypeService.CreateAsync(groupTypeCreateDTO);
            if (!createdGroupType.Success)
                return BadRequest(createdGroupType);
            return Ok(createdGroupType);
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Grupo de acesso atualizado com sucesso", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(404, "Grupo de acesso não encontrado", typeof(ApiResponse<AccessGroupDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<AccessGroupDTO>))]
        public async Task<ActionResult<ApiResponse<AccessGroupDTO>>> UpdateAccessGroup([FromRoute] Guid id, [FromBody] UpdateAccessGroupDTO updateAccessGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<AccessGroupDTO> { Success = false, Message = "Dados de entrada inválidos." });
            var response = await _accessGroupService.UpdateAsync(id, updateAccessGroupDTO);
            if (!response.Success)
            {
                if (response.Message == "Grupo de acesso não encontrado")
                    return NotFound(response);
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpPut("group-types/{id:guid}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Tipo de grupo atualizado com sucesso", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(404, "Tipo de grupo não encontrado", typeof(ApiResponse<GroupTypeDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<GroupTypeDTO>))]
        public async Task<ActionResult<ApiResponse<GroupTypeDTO>>> UpdateGroupType([FromRoute] Guid id, [FromBody] GroupTypeUpdateDTO groupTypeUpdateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<GroupTypeDTO> { Success = false, Message = "Dados de entrada inválidos." });
            var response = await _groupTypeService.UpdateAsync(id, groupTypeUpdateDTO);

            if (!response.Success)
            {
                if (response.Message == "Tipo de grupo não encontrado")
                    return NotFound(response);
                return BadRequest(response);
            }
            return Ok(response);
        }

        #endregion PUT

        #region DELETE
        [HttpDelete("{id:guid}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Grupo de acesso deletado com sucesso", typeof(ApiResponse<bool>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<bool>))]
        [SwaggerResponse(404, "Grupo de acesso não encontrado", typeof(ApiResponse<bool>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<bool>))]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAccessGroup([FromRoute] Guid id)
        {
            var response = await _accessGroupService.DeleteAsync(id);
            if (!response.Success)
            {
                if (response.Message == "Grupo de acesso não encontrado")
                    return NotFound(response);
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("group-types/{id:guid}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Tipo de grupo deletado com sucesso", typeof(ApiResponse<bool>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ApiResponse<bool>))]
        [SwaggerResponse(404, "Tipo de grupo não encontrado", typeof(ApiResponse<bool>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ApiResponse<bool>))]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteGroupType([FromRoute] Guid id)
        {
            var response = await _groupTypeService.DeleteAsync(id);
            if (!response.Success)
            {
                if (response.Message == "Tipo de grupo não encontrado")
                    return NotFound(response);
                return BadRequest(response);
            }
            return Ok(response);
        }

        #endregion DELETE
    }
}
