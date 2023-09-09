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

public class GetClaimsService : ScopeServiceImpl<GetClaimsService, User, IEnumerable<Claim>>, IGetClaimsService
{
    public GetClaimsService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(DbContext dbContext, ISessionContext context)
    {
        var tenantDb = dbContext.Set<Tenant>();
        var userDb = dbContext.Set<User>();
        var userRoleDb = dbContext.Set<UserRole>();
        var roleDb = dbContext.Set<Role>();
        var roleClaimDb = dbContext.Set<RoleClaim>();
        
        var userClaims = new List<Claim>();
        var tenant = await tenantDb.FirstOrDefaultAsync(m => m.Id == this.Request.TenantId);
        var userRole = await userRoleDb.FirstOrDefaultAsync(m => m.TenantId == this.Request.TenantId && m.UserId == this.Request.Id);
        var roles = await roleDb.Where(m => m.TenantId == this.Request.TenantId && m.Id == userRole.RoleId).ToListAsync();
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role.Name));
            var results = await roleClaimDb.Where(m => m.TenantId == this.Request.TenantId && m.RoleId == role.Id)
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