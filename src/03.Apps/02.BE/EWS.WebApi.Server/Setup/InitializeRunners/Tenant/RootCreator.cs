using EWS.Application;
using EWS.Application.Const;
using EWS.Domain;
using EWS.Domain.Infra.Extension;
using EWS.Entity;
using EWS.Entity.Db;
using eXtensionSharp;
using Microsoft.AspNetCore.Identity;

namespace EWS.WebApi.Server.ApplicationInitializer.Tenant;

public class RootCreator
{
    private readonly EWSMsDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    
    public RootCreator(EWSMsDbContext dbContext,
        IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }
    
    public void Execute(string[] args)
    {
        var tenant = TenantParser.Create().Parse(args);
        var rootRole = new Role()
        {
            TenantId = tenant.Id,
            Name = RoleConstants.RootRole,
            Description = "Root role with full permissions",
            RoleClaims = new HashSet<RoleClaim>(),
            IsActive = true,
            NormalizedName = $"{tenant.Id}_{RoleConstants.RootRole.ToUpper()}",
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "SYSTEM"
        };

        var rootRoleInDb =
            _dbContext.Roles.FirstOrDefault(m => m.TenantId == tenant.Id && m.Name == RoleConstants.RootRole);
        if (rootRoleInDb.xIsEmpty())
        {
            _dbContext.Roles.Add(rootRole);
            _dbContext.SaveChanges();
        }

        //Check if User Exists
        var rootUser = new User
        {
            TenantId = tenant.Id,
            Id = Guid.NewGuid().ToString(),
            FirstName = "root",
            LastName = "root",
            Email = tenant.Email,
            UserName = "root",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            IsActive = true,
        };

        var passwordHash = _passwordHasher.HashPassword(rootUser, RoleConstants.RootPassword);
        rootUser.PasswordHash = passwordHash;
        rootUser.SecurityStamp = SecurityStamp.CreateSecurityStamp(rootUser);
        var rootUserInDb = _dbContext.Users.FirstOrDefault(m => m.TenantId == tenant.Id && m.Email == rootUser.Email);
        if (rootUserInDb.xIsEmpty())
        {
            _dbContext.Users.Add(rootUser);
            _dbContext.SaveChanges();

            var userRole = new UserRole()
            {
                TenantId = tenant.Id,
                UserId = rootUser.Id,
                RoleId = rootRole.Id,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            };

            _dbContext.UserRoles.Add(userRole);
            _dbContext.SaveChanges();

            foreach (var permission in JPermissions.GetRegisteredPermissions())
            {
                _dbContext.AddPermissionClaim(rootRoleInDb, permission);
            }
        }
    }
}