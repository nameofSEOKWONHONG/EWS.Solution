using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using EWS.Entity.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EWS.Entity;

[Table("Roles", Schema = "identity")]
public class Role : IdentityRole, IEntityBase
{
    [Column(Order = 1)]
    public string TenantId { get; set; }
    public Tenant Tenant { get; set; }
    
    [MaxLength(4000), Column(Order = 2)]
    public string Description { get; set; }
    
    /// <summary>
    /// 활성 여부
    /// </summary>
    [DefaultValue(true), Column(Order = 6)]
    public bool IsActive { get; set; }
    
    [MaxLength(100), Column(Order = 7), AllowNull]
    public string CreatedBy { get; set; }
    
    [Column(Order = 8), AllowNull]
    public DateTime? CreatedOn { get; set; }

    [MaxLength(100), Column(Order = 9)]
    public string CreatedName { get; set; }

    [MaxLength(100), Column(Order = 10), AllowNull]
    public string LastModifiedBy { get; set; }
    
    [Column(Order = 11), AllowNull]
    public DateTime? LastModifiedOn { get; set; }

    [MaxLength(100), Column(Order = 12)]
    public string LastModifiedName { get; set; }
    
    public virtual ICollection<RoleClaim> RoleClaims { get; set; }

    public static void Builder(ModelBuilder modelBuilder)
    {
            
    }
}