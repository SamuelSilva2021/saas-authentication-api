using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.MultiTenant;

[Route("api/tenant-products")]
[ApiController]
public class TenantProductController(ITenantProductService productService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<TenantProductDTO>>> Create([FromBody] CreateTenantProductDTO dto)
    {
        var result = await productService.CreateAsync(dto);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<TenantProductDTO>>> Update(Guid id, [FromBody] UpdateTenantProductDTO dto)
    {
        var result = await productService.UpdateAsync(id, dto);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<bool>>> Delete(Guid id)
    {
        var result = await productService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ResponseDTO<TenantProductDTO>>> GetById(Guid id)
    {
        var result = await productService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDTO<List<TenantProductDTO>>>> GetAll()
    {
        var result = await productService.GetAllAsync();
        return StatusCode(result.Code, result);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseDTO<List<TenantProductSummaryDTO>>>> GetActive()
    {
        var result = await productService.GetActiveProductsAsync();
        return StatusCode(result.Code, result);
    }
}
