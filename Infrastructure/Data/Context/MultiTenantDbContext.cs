using Authenticator.API.Core.Domain.MultiTenant.Plan;
using Authenticator.API.Core.Domain.MultiTenant.Subscriptions;
using Authenticator.API.Core.Domain.MultiTenant.Tenant;
using Authenticator.API.Core.Domain.MultiTenant.TenantProduct;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Authenticator.API.Infrastructure.Data;

/// <summary>
/// Contexto do banco de dados multi-tenant
/// Conecta ao banco multi_tenant
/// </summary>
public class MultiTenantDbContext : DbContext
{
    public MultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options) : base(options)
    {
    }

    // DbSets das entidades
    public DbSet<TenantEntity> Tenants { get; set; }
    public DbSet<TenantProductEntity> Products { get; set; }
    public DbSet<PlanEntity> Plans { get; set; }
    public DbSet<SubscriptionEnity> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração da tabela Tenants
        modelBuilder.Entity<TenantEntity>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.Domain).IsUnique();
            entity.HasIndex(e => e.Document).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.Domain).HasColumnName("domain");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.Property(e => e.Settings)
            .HasColumnName("settings").HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions()) ?? new Dictionary<string, object>());

            entity.Property(e => e.ActiveSubscriptionId).HasColumnName("active_subscription_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            // Campos corporativos
            entity.Property(e => e.Document).HasColumnName("document");
            entity.Property(e => e.RazaoSocial).HasColumnName("razao_social");
            entity.Property(e => e.InscricaoEstadual).HasColumnName("inscricao_estadual");
            entity.Property(e => e.InscricaoMunicipal).HasColumnName("inscricao_municipal");

            // Dados de contato
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Website).HasColumnName("website");

            // Endereço principal
            entity.Property(e => e.AddressStreet).HasColumnName("address_street");
            entity.Property(e => e.AddressNumber).HasColumnName("address_number");
            entity.Property(e => e.AddressComplement).HasColumnName("address_complement");
            entity.Property(e => e.AddressNeighborhood).HasColumnName("address_neighborhood");
            entity.Property(e => e.AddressCity).HasColumnName("address_city");
            entity.Property(e => e.AddressState).HasColumnName("address_state");
            entity.Property(e => e.AddressZipcode).HasColumnName("address_zipcode");
            entity.Property(e => e.AddressCountry).HasColumnName("address_country");

            // Endereço de cobrança
            entity.Property(e => e.BillingStreet).HasColumnName("billing_street");
            entity.Property(e => e.BillingNumber).HasColumnName("billing_number");
            entity.Property(e => e.BillingComplement).HasColumnName("billing_complement");
            entity.Property(e => e.BillingNeighborhood).HasColumnName("billing_neighborhood");
            entity.Property(e => e.BillingCity).HasColumnName("billing_city");
            entity.Property(e => e.BillingState).HasColumnName("billing_state");
            entity.Property(e => e.BillingZipcode).HasColumnName("billing_zipcode");
            entity.Property(e => e.BillingCountry).HasColumnName("billing_country");

            // Responsável legal
            entity.Property(e => e.LegalRepresentativeName).HasColumnName("legal_representative_name");
            entity.Property(e => e.LegalRepresentativeCpf).HasColumnName("legal_representative_cpf");
            entity.Property(e => e.LegalRepresentativeEmail).HasColumnName("legal_representative_email");
            entity.Property(e => e.LegalRepresentativePhone).HasColumnName("legal_representative_phone");

            // Relacionamento com assinatura ativa
            entity.HasOne(d => d.ActiveSubscription)
                .WithMany()
                .HasForeignKey(d => d.ActiveSubscriptionId);

            // Relacionamentos 1:N
            entity.HasMany(t => t.UserAccounts)
                .WithOne(u => u.Tenant)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(t => t.AccessGroups)
                .WithOne(g => g.Tenant)
                .HasForeignKey(g => g.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(t => t.Roles)
                .WithOne(r => r.Tenant)
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(t => t.Permissions)
                .WithOne(p => p.Tenant)
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        // Configuração da tabela Products
        modelBuilder.Entity<TenantProductEntity>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Version).HasColumnName("version");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.ConfigurationSchema).HasColumnName("configuration_schema");
            entity.Property(e => e.PricingModel).HasColumnName("pricing_model");
            entity.Property(e => e.BasePrice).HasColumnName("base_price").HasPrecision(10, 2);
            entity.Property(e => e.SetupFee).HasColumnName("setup_fee").HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
        });

        // Configuração da tabela Plans
        modelBuilder.Entity<PlanEntity>(entity =>
        {
            entity.ToTable("plans");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price).HasColumnName("price").HasPrecision(10, 2);
            entity.Property(e => e.BillingCycle).HasColumnName("billing_cycle");
            entity.Property(e => e.MaxUsers).HasColumnName("max_users");
            entity.Property(e => e.MaxStorageGb).HasColumnName("max_storage_gb");
            entity.Property(e => e.Features).HasColumnName("features");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
        });

        // Configuração da tabela Subscriptions
        modelBuilder.Entity<SubscriptionEnity>(entity =>
        {
            entity.ToTable("subscriptions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.ProductId }).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.PlanId).HasColumnName("plan_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TrialEndsAt).HasColumnName("trial_ends_at");
            entity.Property(e => e.CurrentPeriodStart).HasColumnName("current_period_start");
            entity.Property(e => e.CurrentPeriodEnd).HasColumnName("current_period_end");
            entity.Property(e => e.CancelAtPeriodEnd).HasColumnName("cancel_at_period_end");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CustomPricing).HasColumnName("custom_pricing").HasPrecision(10, 2);
            entity.Property(e => e.UsageLimits).HasColumnName("usage_limits");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            // Relacionamentos
            entity.HasOne(d => d.Tenant)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Product)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Plan)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}