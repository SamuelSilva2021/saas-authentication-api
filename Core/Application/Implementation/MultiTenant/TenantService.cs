using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Tenant;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using AutoMapper;
using System.Text.RegularExpressions;

namespace Authenticator.API.Core.Application.Implementation.MultiTenant
{
    public class TenantService(
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<TenantService> logger,
        IUserAccountsRepository userAccountsRepository,
        IJwtTokenService jwtTokenService
        ) : ITenantService
    {
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<TenantService> _logger = logger;
        private readonly IUserAccountsRepository _userAccountsRepository = userAccountsRepository;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<ResponseDTO<RegisterTenantResponseDTO>> AddTenantAsync(CreateTenantDTO tenant)
        {
            try
            {
                var existingUser = await _userAccountsRepository.GetByEmailAsync(tenant.Email);
                if (existingUser != null)
                    return ResponseBuilder<RegisterTenantResponseDTO>
                        .Fail(new ErrorDTO { Message = "Email já está em uso." }).WithCode(400).Build();

                var existingDocument = await _tenantRepository.GetByDocumentAsync(tenant.CnpjCpf!);
                if (existingDocument != null)
                    return ResponseBuilder<RegisterTenantResponseDTO>
                        .Fail(new ErrorDTO { Message = "CNPJ/CPF já está em uso." }).WithCode(400).Build();

                var tenantEntity = _mapper.Map<TenantEntity>(tenant);
                tenantEntity.Slug = await GenerateUniqueSlugAsync(tenant.CompanyName);

                var createdTenant = await _tenantRepository.AddAsync(tenantEntity);
                var tenantDTO = _mapper.Map<TenantDTO>(createdTenant);
                _logger.LogInformation("Tenant criado com sucesso: {TenantId}", createdTenant.Id);

                // 2. Criar o Usuário Administrador
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(tenant.Password);
                var userName = await GenerateUniqueUsernameAsync(tenant.Email);

                var adminUser = new UserAccountEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = createdTenant.Id,
                    Username = await GenerateUniqueUsernameAsync(tenant.Email),
                    Email = tenant.Email.ToLower().Trim(),
                    PasswordHash = passwordHash,
                    FirstName = tenant.FirstName.Trim(),
                    LastName = tenant.LastName.Trim(),
                    PhoneNumber = tenant.UserPhone?.Trim(),
                    IsActive = true,
                    IsEmailVerified = false
                };
                await _userAccountsRepository.AddAsync(adminUser);
                _logger.LogInformation("Usuário administrador criado com sucesso: {UserId}", adminUser.Id);

                var accessToken = _jwtTokenService.GenerateAccessToken(adminUser, createdTenant, new List<string>(), new List<string>(), new List<string>());
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var expiresIn = _jwtTokenService.GetTokenExpirationTime();

                if (!string.IsNullOrWhiteSpace(tenant.LeadSource))
                {
                    _logger.LogInformation("Lead registrado - Fonte: {LeadSource}, UTM: {UtmSource}/{UtmCampaign}/{UtmMedium}",
                        tenant.LeadSource, tenant.UtmSource, tenant.UtmCampaign, tenant.UtmMedium);
                }

                var dto = new RegisterTenantResponseDTO
                {
                    TenantId = createdTenant.Id,
                    UserId = adminUser.Id,
                    CompanyName = createdTenant.Name,
                    Slug = createdTenant.Slug,
                    Email = adminUser.Email,
                    FullName = $"{adminUser.FirstName} {adminUser.LastName}",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = expiresIn,
                    CreatedAt = createdTenant.CreatedAt,
                    Message = "Empresa e usuário administrador criados com sucesso!"
                };

                return ResponseBuilder<RegisterTenantResponseDTO>.Ok(dto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar novo tenant: {Message}", ex.Message);
                return ResponseBuilder<RegisterTenantResponseDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }

        private async Task<string> GenerateUniqueUsernameAsync(string email)
        {
            var baseUsername = email.Split('@')[0].ToLower();
            var username = baseUsername;
            var counter = 1;

            while (await _userAccountsRepository.AnyAsync(u => u.Username == username))
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
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
            while (await _tenantRepository.ExistingSlug(slug))
            {
                slug = $"{originalSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}
