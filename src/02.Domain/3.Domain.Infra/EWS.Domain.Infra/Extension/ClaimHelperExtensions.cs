using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using EWS.Application;
using EWS.Application.Const;
using EWS.Entity;
using EWS.Entity.Db;
using eXtensionSharp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.Extension;

/// <summary>
/// 
/// </summary>
public static class ClaimsHelperExtensions
{
    public static IdentityResult AddPermissionClaim(this EWSMsDbContext db, Role role, string permission)
    {
        var allClaims = db.RoleClaims.Where(m => m.TenantId == role.TenantId && m.RoleId == role.Id)
            .ToList();
        var list = new List<RoleClaim>();
        if (!allClaims.Any(a => a.ClaimType == ApplicationClaimTypes.Permission && a.ClaimValue == permission))
        {
            var claim = new Claim(ApplicationClaimTypes.Permission, permission);
            list.Add(new RoleClaim()
            {
                TenantId = role.TenantId,
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                CreatedBy = role.CreatedBy,
                CreatedOn = role.CreatedOn,
            });
        }

        if (list.xIsNotEmpty())
        {
            db.RoleClaims.AddRange(list);
            db.SaveChanges();
            return IdentityResult.Success;                
        }

        return IdentityResult.Failed();
    }
}