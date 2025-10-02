using Authenticator.API.Core.Domain.AccessControl.AccessGroup.Entities;
using Authenticator.API.Core.Domain.AccessControl.AccountAccessGroups.Etities;
using Authenticator.API.Core.Domain.AccessControl.Applications;
using Authenticator.API.Core.Domain.AccessControl.Modules;
using Authenticator.API.Core.Domain.AccessControl.Operations;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations;
using Authenticator.API.Core.Domain.AccessControl.Permissions;
using Authenticator.API.Core.Domain.AccessControl.RoleAccessGroups;
using Authenticator.API.Core.Domain.AccessControl.Roles;
using Authenticator.API.Core.Domain.AccessControl.Roles.Entities;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Authenticator.API.Infrastructure.Data.Interfaces;

namespace Authenticator.API.Infrastructure.Data.Context;

/// <summary>
/// Contexto do banco de dados de controle de acesso
/// Conecta ao banco access_control_db
/// </summary>
public class AccessControlDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    // Construtor utilizado pelos testes e cenários sem injeção de ITenantContext
    public AccessControlDbContext(DbContextOptions<AccessControlDbContext> options) : base(options)
    {
        _tenantContext = new DefaultTenantContext();
    }

    // Construtor utilizado em execução normal com ITenantContext resolvido pelo DI
    public AccessControlDbContext(DbContextOptions<AccessControlDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<UserAccountEntity> UserAccounts { get; set; }
    public DbSet<AccessGroupEntity> AccessGroups { get; set; }
    public DbSet<GroupTypeEntity> GroupTypes { get; set; }
    public DbSet<AccountAccessGroupEntity> AccountAccessGroups { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<RoleAccessGroupEntity> RoleAccessGroups { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<OperationEntity> Operations { get; set; }
    public DbSet<PermissionOperationEntity> PermissionOperations { get; set; }
    public DbSet<ApplicationEntity> Applications { get; set; }
    public DbSet<ModuleEntity> Modules { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccountEntity>(entity =>
        {
            entity.ToTable("user_account");
            entity.HasKey(e => e.Id);
            entity.Property(u => u.Id).HasColumnName("id").IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.TenantId);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(20);
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified").HasDefaultValue(false);
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.PasswordResetToken).HasColumnName("password_reset_token").HasMaxLength(500);
            entity.Property(e => e.PasswordResetExpiresAt).HasColumnName("password_reset_expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp without time zone");

            entity.HasMany(u => u.AccountAccessGroups)
                .WithOne(aag => aag.UserAccount)
                .HasForeignKey(aag => aag.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(u => u.Tenant)
            .WithMany(t => t.UserAccounts)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.AccountAccessGroups)
                .WithOne(ag => ag.UserAccount)
                .HasForeignKey(ag => ag.UserAccountId);

        });

        modelBuilder.Entity<AccessGroupEntity>(entity =>
        {
            entity.ToTable("access_group");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.GroupTypeId).HasColumnName("group_type_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasOne(e => e.GroupType)
                .WithMany(gt => gt.AccessGroups)
                .HasForeignKey(e => e.GroupTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.Tenant)
                .WithMany(t => t.AccessGroups)
                .HasForeignKey(g => g.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(g => g.AccountAccessGroups)
                .WithOne(ag => ag.AccessGroup)
                .HasForeignKey(ag => ag.AccessGroupId);

            entity.HasMany(g => g.RoleAccessGroups)
                .WithOne(gr => gr.AccessGroup)
                .HasForeignKey(gr => gr.AccessGroupId);
        });

        modelBuilder.Entity<AccessGroupRoleEntity>(entity =>
        {
            entity.ToTable("access_group_roles");
            entity.HasKey(e => new { e.GroupId, e.RoleId });
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
        });

        modelBuilder.Entity<GroupTypeEntity>(entity =>
        {
            entity.ToTable("group_type");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasIndex(gt => gt.Code).IsUnique();

            entity.HasMany(gt => gt.AccessGroups)
                .WithOne(ag => ag.GroupType)
                .HasForeignKey(ag => ag.GroupTypeId);
        });

        modelBuilder.Entity<AccountAccessGroupEntity>(entity =>
        {
            entity.ToTable("account_access_group");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserAccountId, e.AccessGroupId }).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserAccountId).HasColumnName("user_account_id");
            entity.Property(e => e.AccessGroupId).HasColumnName("access_group_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.GrantedBy).HasColumnName("granted_by");
            entity.Property(e => e.GrantedAt).HasColumnName("granted_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasOne(x => x.UserAccount)
                .WithMany(x => x.AccountAccessGroups)
                .HasForeignKey(x => x.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.AccessGroup)
                .WithMany(p => p.AccountAccessGroups)
                .HasForeignKey(d => d.AccessGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);


        });

        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.ToTable("role");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasOne(r => r.Tenant)
            .WithMany(t => t.Roles)
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(r => r.AccessGroupRoles)
                .WithOne(gr => gr.Role)
                .HasForeignKey(gr => gr.RoleId);

            entity.HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId);
        });

        modelBuilder.Entity<RoleAccessGroupEntity>(entity =>
        {
            entity.ToTable("role_access_group");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.AccessGroupId, e.RoleId }).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessGroupId).HasColumnName("access_group_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            // Relacionamentos
            entity.HasOne(d => d.Role)
                .WithMany(p => p.RoleAccessGroups)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.AccessGroup)
                .WithMany(p => p.RoleAccessGroups)
                .HasForeignKey(d => d.AccessGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolePermissionEntity>(entity =>
        {
            entity.ToTable("role_permission");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<ApplicationEntity>(entity =>
        {
            entity.ToTable("application");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.SecretKey).HasColumnName("secret_key").HasMaxLength(255);
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.AuxiliarSchema).HasColumnName("auxiliar_schema");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Visible).HasColumnName("visible");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<ModuleEntity>(entity =>
        {
            entity.ToTable("module");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ModuleKey);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.ModuleKey).HasColumnName("module_key").HasMaxLength(100);
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Application)
                .WithMany(p => p.Modules)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PermissionEntity>(entity =>
        {
            entity.ToTable("permission");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoleId, e.ModuleId }).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            // Relacionamentos
            entity.HasOne(p => p.Tenant)
            .WithMany(t => t.Permissions)
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId);
        });

        modelBuilder.Entity<OperationEntity>(entity =>
        {
            entity.ToTable("operation");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Value).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Value).HasColumnName("value");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<PermissionOperationEntity>(entity =>
        {
            entity.ToTable("permission_operation");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PermissionId, e.OperationId }).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.OperationId).HasColumnName("operation_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Permission)
                .WithMany(p => p.PermissionOperations)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Operation)
                .WithMany(p => p.PermissionOperations)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);

        // Filtros globais por tenant (aplicados quando há tenant no contexto)
        // Quando não há tenant (ex.: endpoints públicos ou login), os filtros não bloqueiam os dados
        modelBuilder.Entity<UserAccountEntity>().HasQueryFilter(e => !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<AccessGroupEntity>().HasQueryFilter(e => !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<RoleEntity>().HasQueryFilter(e => !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<PermissionEntity>().HasQueryFilter(e => !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);
    }
}

/// <summary>
/// Implementação padrão para cenários sem tenant (ex.: testes)
/// </summary>
internal class DefaultTenantContext : ITenantContext
{
    public Guid? TenantId => null;
    public string? TenantSlug => null;
    public string? TenantName => null;
    public bool HasTenant => false;
    public void SetTenant(Guid? tenantId, string? tenantSlug, string? tenantName) { }
}