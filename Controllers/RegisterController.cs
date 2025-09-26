using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Authenticator.API.Data;
using Authenticator.API.Entities;
using Authenticator.API.Models;
using Authenticator.API.Services;

namespace Authenticator.API.Controllers;

/// <summary>
/// Controller para registro de novos tenants (clientes/empresas)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly MultiTenantDbContext _tenantContext;
    private readonly AccessControlDbContext _accessContext;
    private readonly IAuthenticationService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(
        MultiTenantDbContext tenantContext,
        AccessControlDbContext accessContext,
        IAuthenticationService authService,
        IJwtTokenService jwtTokenService,
        ILogger<RegisterController> logger)
    {
        _tenantContext = tenantContext;
        _accessContext = accessContext;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Registra um novo tenant (cliente/empresa) com usuário administrador
    /// </summary>
    /// <param name="request">Dados do tenant e usuário administrador</param>
    /// <returns>Informações do tenant criado com tokens de autenticação</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RegisterTenantResponse>>> Register([FromBody] RegisterTenantRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando processo de registro para empresa: {CompanyName}", request.CompanyName);

            var existingUser = await _accessContext.UserAccounts
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                _logger.LogWarning("Tentativa de registro com email já existente: {Email}", request.Email);
                return BadRequest(new ApiResponse<RegisterTenantResponse>
                {
                    Success = false,
                    Message = "Este email já está sendo utilizado por outro usuário.",
                    Data = null
                });
            }

            // Validar CNPJ/CPF se fornecido
            if (!string.IsNullOrWhiteSpace(request.CnpjCpf))
            {
                var existingTenant = await _tenantContext.Tenants
                    .FirstOrDefaultAsync(t => t.CnpjCpf == request.CnpjCpf);

                if (existingTenant != null)
                {
                    _logger.LogWarning("Tentativa de registro com CNPJ/CPF já existente: {CnpjCpf}", request.CnpjCpf);
                    return BadRequest(new ApiResponse<RegisterTenantResponse>
                    {
                        Success = false,
                        Message = "Este CNPJ/CPF já está cadastrado no sistema.",
                        Data = null
                    });
                }
            }

            // Usar transações em ambos os contextos
            using var tenantTransaction = await _tenantContext.Database.BeginTransactionAsync();
            using var accessTransaction = await _accessContext.Database.BeginTransactionAsync();
            
            try
            {
                // 1. Criar o Tenant
                var tenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    Name = request.CompanyName,
                    Slug = await GenerateUniqueSlugAsync(request.CompanyName),
                    CnpjCpf = request.CnpjCpf?.Trim(),
                    RazaoSocial = request.RazaoSocial?.Trim(),
                    Phone = request.Phone?.Trim(),
                    Email = request.CompanyEmail,
                    Website = request.Website?.Trim(),
                    AddressStreet = request.AddressStreet?.Trim(),
                    AddressNumber = request.AddressNumber?.Trim(),
                    AddressComplement = request.AddressComplement?.Trim(),
                    AddressNeighborhood = request.AddressNeighborhood?.Trim(),
                    AddressCity = request.AddressCity?.Trim(),
                    AddressState = request.AddressState?.Trim()?.ToUpper(),
                    AddressZipcode = request.AddressZipcode?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    Status = "active",
                    Settings = new Dictionary<string, object>()
                };

                _tenantContext.Tenants.Add(tenant);
                await _tenantContext.SaveChangesAsync();

                _logger.LogInformation("Tenant criado com sucesso. ID: {TenantId}, Nome: {CompanyName}", tenant.Id, tenant.Name);

                // 2. Criar o Usuário Administrador
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var username = await GenerateUniqueUsernameAsync(request.Email);

                var adminUser = new UserAccount
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant.Id,
                    Username = username,
                    Email = request.Email.ToLower().Trim(),
                    PasswordHash = passwordHash,
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    PhoneNumber = request.UserPhone?.Trim(),
                    IsActive = true,
                    IsEmailVerified = false,
                    CreatedAt = DateTime.UtcNow
                };

                _accessContext.UserAccounts.Add(adminUser);
                await _accessContext.SaveChangesAsync();

                _logger.LogInformation("Usuário administrador criado com sucesso. ID: {UserId}, Email: {Email}", adminUser.Id, adminUser.Email);

                var accessToken = _jwtTokenService.GenerateAccessToken(adminUser, tenant, new List<string>(), new List<string>(), new List<string>());
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var expiresIn = _jwtTokenService.GetTokenExpirationTime();

                if (!string.IsNullOrWhiteSpace(request.LeadSource))
                {
                    _logger.LogInformation("Lead registrado - Fonte: {LeadSource}, UTM: {UtmSource}/{UtmCampaign}/{UtmMedium}", 
                        request.LeadSource, request.UtmSource, request.UtmCampaign, request.UtmMedium);
                }

                await tenantTransaction.CommitAsync();
                await accessTransaction.CommitAsync();

                // 5. Preparar resposta
                var response = new RegisterTenantResponse
                {
                    TenantId = tenant.Id,
                    UserId = adminUser.Id,
                    CompanyName = tenant.Name,
                    Slug = tenant.Slug,
                    Email = adminUser.Email,
                    FullName = $"{adminUser.FirstName} {adminUser.LastName}",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = expiresIn,
                    CreatedAt = tenant.CreatedAt,
                    Message = "Empresa e usuário administrador criados com sucesso!"
                };

                _logger.LogInformation("Registro concluído com sucesso para {CompanyName} - TenantId: {TenantId}", 
                    tenant.Name, tenant.Id);

                return Ok(new ApiResponse<RegisterTenantResponse>
                {
                    Success = true,
                    Message = "Registro realizado com sucesso!",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                await tenantTransaction.RollbackAsync();
                await accessTransaction.RollbackAsync();
                _logger.LogError(ex, "Erro durante a transação de registro para {CompanyName}: {Error}", 
                    request.CompanyName, ex.Message);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no processo de registro: {Error}", ex.Message);
            return StatusCode(500, new ApiResponse<RegisterTenantResponse>
            {
                Success = false,
                Message = "Erro interno do servidor. Tente novamente mais tarde.",
                Data = null
            });
        }
    }

    /// <summary>
    /// Gera um slug único para o tenant baseado no nome da empresa
    /// </summary>
    private async Task<string> GenerateUniqueSlugAsync(string companyName)
    {
        // Remover caracteres especiais e normalizar
        var slug = Regex.Replace(companyName.ToLower(), @"[^a-z0-9\s-]", "")
                       .Trim()
                       .Replace(' ', '-')
                       .Substring(0, Math.Min(companyName.Length, 50));

        var originalSlug = slug;
        var counter = 1;

        // Verificar se o slug já existe
        while (await _tenantContext.Tenants.AnyAsync(t => t.Slug == slug))
        {
            slug = $"{originalSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    /// <summary>
    /// Gera um username único baseado no email
    /// </summary>
    private async Task<string> GenerateUniqueUsernameAsync(string email)
    {
        var baseUsername = email.Split('@')[0].ToLower();
        var username = baseUsername;
        var counter = 1;

        while (await _accessContext.UserAccounts.AnyAsync(u => u.Username == username))
        {
            username = $"{baseUsername}{counter}";
            counter++;
        }

        return username;
    }
}