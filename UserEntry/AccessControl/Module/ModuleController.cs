using Authenticator.API.Core.Application.Interfaces.Module;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.AccessControl.Module
{
    [Route("api/modules")]
    [ApiController]
    public class ModuleController(IModuleTypeService moduleTypeService) : ControllerBase
    {
        private readonly IModuleTypeService _moduleTypeService = moduleTypeService;

        [HttpGet("module-types")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ModuleTypeDTO>>> GetAllModuleTypesAsync()
        {
            var response = await _moduleTypeService.GetAllModuleTypesAsync();
            return StatusCode(response.Code, response);
        }

        [HttpPost("module-types")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleTypeDTO>> AddModuleTypeAsync([FromBody] ModuleTypeCreateDTO moduleType)
        {
            var response = await _moduleTypeService.AddModuleTypeAsync(moduleType);
            return StatusCode(response.Code, response);
        }
    }
}
