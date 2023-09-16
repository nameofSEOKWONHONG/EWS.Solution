using System.Security.Claims;
using EWS.Application.Const;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Infra.Extension;
using EWS.Entity;
using EWS.Entity.Base;
using EWS.Entity.Db;
using EWS.Infrastructure.Extentions;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Account.Identity;

public class GetClaimsService : ServiceImplBase<GetClaimsService, User, IEnumerable<Claim>>, IGetClaimsService
{
    public GetClaimsService(DbContext dbContext) : base(dbContext, null)
    {
        
    }
    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var tenantDb = Db.Set<Tenant>();
        var userDb = Db.Set<User>();
        var userRoleDb = Db.Set<UserRole>();
        var roleDb = Db.Set<Role>();
        var roleClaimDb = Db.Set<RoleClaim>();
        
        var userClaims = new List<Claim>();
        var tenant = await tenantDb.AsNoTracking().FirstOrDefaultAsync(m => m.Id == this.Request.TenantId);
        var userRole = await userRoleDb.AsNoTracking().FirstOrDefaultAsync(m => m.TenantId == this.Request.TenantId && m.UserId == this.Request.Id);
        var roles = await roleDb.Where(m => m.TenantId == this.Request.TenantId && m.Id == userRole.RoleId).ToListAsync();
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role.Name));
            var results = await roleClaimDb.AsNoTracking().Where(m => m.TenantId == this.Request.TenantId && m.RoleId == role.Id)
                .ToListAsync();
            var allPermissionsForThisRoles = new List<Claim>();
            results.ForEach(item =>
            {
                allPermissionsForThisRoles.Add(new Claim(item.ClaimType, item.ClaimValue));
            });
            permissionClaims.AddRange(allPermissionsForThisRoles);
        }

        var claims = new List<Claim>
            {
                new(ApplicationClaimTypeConstants.TenantId, this.Request.TenantId),
                new(ApplicationClaimTypeConstants.TimeZone, tenant.TimeZone),
                new(ClaimTypes.NameIdentifier, this.Request.Id),
                new(ClaimTypes.Email, this.Request.Email),
                new(ClaimTypes.Name, this.Request.UserName),
                // new(ApplicationClaimTypeConstant.Depart, user.DeptName.xValue()),
                // new(ApplicationClaimTypeConstant.Level, user.LvlName.xValue()),
                new(ClaimTypes.MobilePhone, this.Request.PhoneNumber.xIsNotEmpty() ? this.Request.PhoneNumber.vToAESDecrypt() : string.Empty),
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

        this.Result = claims;
    }
}