using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Authenticator.API.UserEntry.AccessControl.AccessGroup
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
        [SwaggerResponse(200, "Lista de grupos de acesso recuperada com sucesso", typeof(ResponseDTO<IEnumerable<AccessGroupDTO>>))]
        [SwaggerResponse(404, "Grupos de acesso não encontrado", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<IEnumerable<AccessGroupDTO>>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<IEnumerable<AccessGroupDTO>>))]
        public async Task<ActionResult<ResponseDTO<IEnumerable<AccessGroupDTO>>>> GetAllAccessGroups()
        {
            var accessGroups = await _accessGroupService.GetAllAsync();
            if (accessGroups.Data == null || !accessGroups.Data.Any())
                return NotFound(new ResponseDTO<IEnumerable<AccessGroupDTO>>());
            return Ok(accessGroups);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Grupo de acesso recuperado com sucesso", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(404, "Grupo de acesso não encontrado", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<AccessGroupDTO>))]
        public async Task<ActionResult<ResponseDTO<AccessGroupDTO>>> GetAccessGroupById([FromRoute] Guid id)
        {
            var accessGroup = await _accessGroupService.GetByIdAsync(id);
            if (accessGroup == null)
                return NotFound(new ResponseDTO<AccessGroupDTO>());
            return Ok(accessGroup);
        }

        [HttpGet("group-types")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Lista de tipos de grupo recuperada com sucesso", typeof(ResponseDTO<IEnumerable<GroupTypeDTO>>))]
        [SwaggerResponse(404, "Tipos de grupo não encontrado", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<IEnumerable<GroupTypeDTO>>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<IEnumerable<GroupTypeDTO>>))]
        public async Task<ActionResult<ResponseDTO<IEnumerable<GroupTypeDTO>>>> GetAllGroupTypes()
        {
            var groupTypes = await _groupTypeService.GetAllAsync();
            if (groupTypes.Data == null || !groupTypes.Data.Any())
                return NotFound(new ResponseDTO<IEnumerable<GroupTypeDTO>>());
            return Ok(groupTypes);

        }

        [HttpGet("group-types/{id:guid}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Tipo de grupo recuperado com sucesso", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(404, "Tipo de grupo não encontrado", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<GroupTypeDTO>))]
        public async Task<ActionResult<ResponseDTO<GroupTypeDTO>>> GetGroupTypeById([FromRoute] Guid id)
        {
            var groupType = await _groupTypeService.GetByIdAsync(id);
            if (groupType == null)
                return NotFound(new ResponseDTO<GroupTypeDTO>());
            return Ok(groupType);
        }

        #endregion GET

        #region POST
        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(200, "Grupo de acesso criado com sucesso", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<AccessGroupDTO>))]
        [ProducesResponseType(typeof(ResponseDTO<AccessGroupDTO>), 200)]
        [ProducesResponseType(typeof(ResponseDTO<AccessGroupDTO>), 400)]
        [ProducesResponseType(typeof(ResponseDTO<AccessGroupDTO>), 401)]
        public async Task<ActionResult<ResponseDTO<AccessGroupDTO>>> CreateAccessGroup([FromBody] CreateAccessGroupDTO createAccessGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var response = await _accessGroupService.CreateAsync(createAccessGroupDTO);
            if (!response.Succeeded)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("group-types")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Tipo de grupo criado com sucesso", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<GroupTypeDTO>))]
        [ProducesResponseType(typeof(ResponseDTO<GroupTypeDTO>), 200)]
        [ProducesResponseType(typeof(ResponseDTO<GroupTypeDTO>), 400)]
        [ProducesResponseType(typeof(ResponseDTO<GroupTypeDTO>), 401)]
        public async Task<ActionResult<ResponseDTO<GroupTypeDTO>>> CreateGroupType([FromBody] GroupTypeCreateDTO groupTypeCreateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _groupTypeService.CreateAsync(groupTypeCreateDTO);
            return StatusCode(response.Code, response);
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Grupo de acesso atualizado com sucesso", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(404, "Grupo de acesso não encontrado", typeof(ResponseDTO<AccessGroupDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<AccessGroupDTO>))]
        public async Task<ActionResult<ResponseDTO<AccessGroupDTO>>> UpdateAccessGroup([FromRoute] Guid id, [FromBody] UpdateAccessGroupDTO updateAccessGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _accessGroupService.UpdateAsync(id, updateAccessGroupDTO);
            return StatusCode(response.Code, response);
        }


        [HttpPut("group-types/{id:guid}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Tipo de grupo atualizado com sucesso", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(404, "Tipo de grupo não encontrado", typeof(ResponseDTO<GroupTypeDTO>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<GroupTypeDTO>))]
        public async Task<ActionResult<ResponseDTO<GroupTypeDTO>>> UpdateGroupType([FromRoute] Guid id, [FromBody] GroupTypeUpdateDTO groupTypeUpdateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _groupTypeService.UpdateAsync(id, groupTypeUpdateDTO);
            return StatusCode(response.Code, response);
        }

        #endregion PUT

        #region DELETE
        [HttpDelete("{id:guid}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Grupo de acesso deletado com sucesso", typeof(ResponseDTO<bool>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<bool>))]
        [SwaggerResponse(404, "Grupo de acesso não encontrado", typeof(ResponseDTO<bool>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<bool>))]
        public async Task<ActionResult<ResponseDTO<bool>>> DeleteAccessGroup([FromRoute] Guid id)
        {
            var response = await _accessGroupService.DeleteAsync(id);
            return StatusCode(response.Code, response);
        }

        [HttpDelete("group-types/{id:guid}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Tipo de grupo deletado com sucesso", typeof(ResponseDTO<bool>))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<bool>))]
        [SwaggerResponse(404, "Tipo de grupo não encontrado", typeof(ResponseDTO<bool>))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<bool>))]
        public async Task<ActionResult<ResponseDTO<bool>>> DeleteGroupType([FromRoute] Guid id)
        {
            var response = await _groupTypeService.DeleteAsync(id);
            return StatusCode(response.Code, response);
        }

        #endregion DELETE
    }
}
