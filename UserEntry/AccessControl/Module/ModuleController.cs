using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.AccessControl.Module
{
    [Route("api/modules")]
    [ApiController]
    public class ModuleController(IModuleService moduleTypeService) : ControllerBase
    {
        private readonly IModuleService _moduleTypeService = moduleTypeService;

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ModuleDTO>>> GetAllModuleAsync()
        {
            var response = await _moduleTypeService.GetAllModuleAsync();
            return StatusCode(response.Code, response);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> GetModuleByIdAsync([FromRoute] Guid id)
        {
            var response = await _moduleTypeService.GetModuleByIdAsync(id);
            return StatusCode(response.Code, response);
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> AddModuleTypeAsync([FromBody] ModuleCreateDTO moduleType)
        {
            var response = await _moduleTypeService.AddModuleAsync(moduleType);
            return StatusCode(response.Code, response);
        }
        [HttpPut]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> UpdateModuleTypeAsync([FromRoute] Guid id, [FromBody] ModuleUpdateDTO moduleType)
        {
            var response = await _moduleTypeService.UpdateModuleAsync(id, moduleType);
            return StatusCode(response.Code, response);
        }
        [HttpDelete]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteModuleTypeAsync([FromRoute] Guid id)
        {
            var response = await _moduleTypeService.DeleteModuleAsync(id);
            return StatusCode(response.Code, response);
        }
    }
}
