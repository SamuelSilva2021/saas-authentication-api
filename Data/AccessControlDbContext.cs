using Authenticator.API.Core.Domain.AccessControl.AccessGroups;
using Authenticator.API.Core.Domain.AccessControl.AccountAccessGroups;
using Authenticator.API.Core.Domain.AccessControl.Applications;
using Authenticator.API.Core.Domain.AccessControl.Modules;
using Authenticator.API.Core.Domain.AccessControl.Operations;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations;
using Authenticator.API.Core.Domain.AccessControl.Permissions;
using Authenticator.API.Core.Domain.AccessControl.RoleAccessGroups;
using Authenticator.API.Core.Domain.AccessControl.Roles;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Data;

/// <summary>
/// Contexto do banco de dados de controle de acesso
/// Conecta ao banco access_control_db
/// </summary>
public class AccessControlDbContext : DbContext
{
    public AccessControlDbContext(DbContextOptions<AccessControlDbContext> options) : base(options)
    {
    }

    public DbSet<UserAccountEntity> UserAccounts { get; set; }
    public DbSet<AccessGroupEntity> AccessGroups { get; set; }
    public DbSet<AccountAccessGroupEntity> AccountAccessGroups { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<RoleAccessGroupEntity> RoleAccessGroups { get; set; }
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
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(100);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100);
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(20);
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.PasswordResetToken).HasColumnName("password_reset_token").HasMaxLength(500);
            entity.Property(e => e.PasswordResetExpiresAt).HasColumnName("password_reset_expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        });

        modelBuilder.Entity<AccessGroupEntity>(entity =>
        {
            entity.ToTable("access_group");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.GroupTypeId).HasColumnName("group_type_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
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
            entity.Property(e => e.GrantedAt).HasColumnName("granted_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            entity.HasOne(d => d.UserAccount)
                .WithMany(p => p.AccountAccessGroups)
                .HasForeignKey(d => d.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.AccessGroup)
                .WithMany(p => p.AccountAccessGroups)
                .HasForeignKey(d => d.AccessGroupId)
                .OnDelete(DeleteBehavior.Cascade);
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
            entity.Property(e => e.RoleTypeId).HasColumnName("role_type_id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
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
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

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


        modelBuilder.Entity<ApplicationEntity>(entity =>
        {
            entity.ToTable("application");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.AuxiliarSchema).HasColumnName("auxiliar_schema");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Visible).HasColumnName("visible");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
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
            entity.Property(e => e.ModuleTypeId).HasColumnName("module_type_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            // Relacionamentos
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
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

            // Relacionamentos
            entity.HasOne(d => d.Role)
                .WithMany(p => p.Permissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Module)
                .WithMany(p => p.Permissions)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
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
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
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
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");

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
    }
}