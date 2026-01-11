using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.MultiTenant;

[Route("api/plans")]
[ApiController]
public class PlanController(IPlanService planService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")] // Ajuste conforme suas roles
    public async Task<ActionResult<ResponseDTO<PlanDTO>>> Create([FromBody] CreatePlanDTO dto)
    {
        var result = await planService.CreateAsync(dto);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<PlanDTO>>> Update(Guid id, [FromBody] UpdatePlanDTO dto)
    {
        var result = await planService.UpdateAsync(id, dto);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<bool>>> Delete(Guid id)
    {
        var result = await planService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ResponseDTO<PlanDTO>>> GetById(Guid id)
    {
        var result = await planService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous] // Planos geralmente são públicos para venda
    public async Task<ActionResult<ResponseDTO<PlanDTO>>> GetBySlug(string slug)
    {
        var result = await planService.GetBySlugAsync(slug);
        return StatusCode(result.Code, result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDTO<PagedResponseDTO<PlanDTO>>>> GetAll([FromQuery] PlanFilterDTO filter)
    {
        var result = await planService.GetAllAsync(filter);
        return StatusCode(result.Code, result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDTO<List<PlanSummaryDTO>>>> GetActive()
    {
        var result = await planService.GetActivePlansAsync();
        return StatusCode(result.Code, result);
    }
}
