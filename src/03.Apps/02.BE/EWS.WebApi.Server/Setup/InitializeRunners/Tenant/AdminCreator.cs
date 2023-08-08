using EWS.Application;
using EWS.Application.Const;
using EWS.Domain;
using EWS.Domain.Infra.Extension;
using EWS.Entity;
using EWS.Entity.Db;
using eXtensionSharp;
using Microsoft.AspNetCore.Identity;

namespace EWS.WebApi.Server.ApplicationInitializer.Tenant;

/// <summary>
/// 
/// </summary>
public class AdminCreator
{
    private readonly EWSMsDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="passwordHasher"></param>
    public AdminCreator(EWSMsDbContext dbContext,
        IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Execute(string[] args)
    {
        var tenant = TenantParser.Create().Parse(args);
        var adminRole = new Role()
        {
            TenantId = tenant.Id,
            Name = RoleConstants.AdministratorRole,
            Description = "Administrator role with full permissions",
            RoleClaims = new HashSet<RoleClaim>(),
            IsActive = true,
            NormalizedName = $"{tenant.Id}_{RoleConstants.AdministratorRole.ToUpper()}",
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "SYSTEM"
        };

        var adminRoleInDb =
            _dbContext.Roles.FirstOrDefault(m => m.TenantId == tenant.Id && m.Name == RoleConstants.AdministratorRole);
        if (adminRoleInDb.xIsEmpty())
        {
            _dbContext.Roles.Add(adminRole);
            _dbContext.SaveChanges();
        }

        //Check if User Exists
        var adminUser = new User
        {
            TenantId = tenant.Id,
            Id = Guid.NewGuid().ToString(),
            FirstName = "admin",
            LastName = "administrator",
            Email = "admin@gmail.com",
            UserName = "admin",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
        };

        var passwordHash = _passwordHasher.HashPassword(adminUser, RoleConstants.AdministratorPassword);
        adminUser.PasswordHash = passwordHash;
        adminUser.SecurityStamp = SecurityStamp.CreateSecurityStamp(adminUser);
        var adminUserInDb = _dbContext.Users.FirstOrDefault(m => m.TenantId == tenant.Id && m.Email == adminUser.Email);
        if (adminUserInDb.xIsEmpty())
        {
            _dbContext.Users.Add(adminUser);
            _dbContext.SaveChanges();

            var userRole = new UserRole()
            {
                TenantId = tenant.Id,
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            };

            _dbContext.UserRoles.Add(userRole);
            _dbContext.SaveChanges();

            foreach (var permission in JPermissions.GetRegisteredPermissions())
            {
                _dbContext.AddPermissionClaim(adminRoleInDb, permission);
            }
        }
    }
}