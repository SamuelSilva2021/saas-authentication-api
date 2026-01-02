using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.Enum;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Tenant;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using AutoMapper;
using System.Text.RegularExpressions;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;

namespace Authenticator.API.Core.Application.Implementation.MultiTenant
{
    /// <summary>
    /// Serviço para gerenciamento de tenants (empresas)
    /// </summary>
    /// <param name="tenantRepository"></param>
    /// <param name="mapper"></param>
    /// <param name="logger"></param>
    /// <param name="userAccountsRepository"></param>
    /// <param name="jwtTokenService"></param>
    public class TenantService(
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<TenantService> logger,
        IUserAccountsRepository userAccountsRepository,
        IJwtTokenService jwtTokenService,
        ITenantBusinessRepository tenantBusinessRepository,
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository,
        ITenantProductRepository tenantProductRepository
        ) : ITenantService
    {
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<TenantService> _logger = logger;
        private readonly IUserAccountsRepository _userAccountsRepository = userAccountsRepository;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly ITenantBusinessRepository _tenantBusinessRepository = tenantBusinessRepository;
        private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
        private readonly IPlanRepository _planRepository = planRepository;
        private readonly ITenantProductRepository _tenantProductRepository = tenantProductRepository;

        /// <summary>
        /// Adiciona um novo tenant e cria o usuário administrador associado
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<RegisterTenantResponseDTO>> AddTenantAsync(CreateTenantDTO tenant)
        {
            try
            {
                var existingUser = await _userAccountsRepository.GetByEmailAsync(tenant.Email);
                if (existingUser != null)
                    return ResponseBuilder<RegisterTenantResponseDTO>
                        .Fail(new ErrorDTO { Message = "Email já está em uso." }).WithCode(400).Build();

                var existingDocument = await _tenantRepository.GetByDocumentAsync(tenant.Document!);
                if (existingDocument != null)
                    return ResponseBuilder<RegisterTenantResponseDTO>
                        .Fail(new ErrorDTO { Message = "CNPJ/CPF já está em uso." }).WithCode(400).Build();

                // Busca Produto e Plano Padrão
                var product = await _tenantProductRepository.GetDefaultProductAsync();
                if (product == null)
                    return ResponseBuilder<RegisterTenantResponseDTO>
                        .Fail(new ErrorDTO { Message = "Produto padrão não configurado no sistema." }).WithCode(500).Build();

                var plan = await _planRepository.GetDefaultPlanAsync();
                if (plan == null)
                    return ResponseBuilder<RegisterTenantResponseDTO>
                        .Fail(new ErrorDTO { Message = "Plano padrão não configurado no sistema." }).WithCode(500).Build();

                var tenantEntity = _mapper.Map<TenantEntity>(tenant);
                tenantEntity.Slug = await GenerateUniqueSlugAsync(tenant.CompanyName);
                // Define como pendente até que o fluxo de pagamento/trial seja resolvido
                // Se o plano for gratuito ou trial, poderia ser Active. Vamos assumir Pending para forçar verificação.
                tenantEntity.Status = ETenantStatus.Pending; 

                var createdTenant = await _tenantRepository.AddAsync(tenantEntity);

                var tenantBusinessEntity = new TenantBusinessEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = createdTenant.Id,
                };

                await _tenantBusinessRepository.AddAsync(tenantBusinessEntity);

                // Criar Assinatura (Subscription)
                var now = DateTime.UtcNow;
                var subscription = new SubscriptionEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = createdTenant.Id,
                    ProductId = product.Id,
                    PlanId = plan.Id,
                    Status = "active", // Assinatura criada como ativa (Trial) inicialmente
                    TrialEndsAt = now.AddDays(14), // 14 dias de Trial
                    CurrentPeriodStart = now,
                    CurrentPeriodEnd = now.AddDays(30),
                };
                await _subscriptionRepository.AddAsync(subscription);

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
                    Status = EUserAccountStatus.Ativo,
                    IsEmailVerified = false
                };
                await _userAccountsRepository.AddAsync(adminUser);
                _logger.LogInformation("Usuário administrador criado com sucesso: {UserId}", adminUser.Id);

                var accessToken = _jwtTokenService.GenerateAccessToken(adminUser, createdTenant);
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

        /// <summary>
        /// Gera um nome de usuário único baseado no email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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
                           .Replace(' ', '_')
                           .Substring(0, Math.Min(companyName.Length, 50));

            var originalSlug = slug;
            var counter = 1;

            // Verificar se o slug já existe
            while (await _tenantRepository.ExistingSlug(slug))
            {
                slug = $"{originalSlug}_{counter}";
                counter++;
            }

            return slug;
        }
    }
}
