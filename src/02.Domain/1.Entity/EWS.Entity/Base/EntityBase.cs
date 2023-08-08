using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EWS.Entity.Base;
using Microsoft.EntityFrameworkCore;

namespace EWS.Entity;


[Index(nameof(IsActive), IsUnique = false, Name = "IX_BASE_3")]
[Index(nameof(LastModifiedOn), IsUnique = false, Name = "IX_BASE_2")]
[Index(nameof(CreatedOn), IsUnique = false, Name = "IX_BASE_1")]
public abstract class EntityBase : IEntityBase
{
    [Key, Column(Order = 0)]
    public string TenantId { get; set; }
    
    [MaxLength(400)]
    public string CreatedBy { get; set; }
    
    public DateTime? CreatedOn { get; set; }
    
    [MaxLength(100)]
    public string CreatedName { get; set; }
    
    [MaxLength(400)]
    public string LastModifiedBy { get; set; }
    
    public DateTime? LastModifiedOn { get; set; }
    
    [MaxLength(100)]
    public string LastModifiedName { get; set; }

    [DefaultValue(true)]
    public bool IsActive { get; set; }

    protected EntityBase()
    {
        
    }

    protected EntityBase(string tenantId, string createdBy, DateTime? createdOn)
    {
        this.TenantId = tenantId;
        this.CreatedBy = createdBy;
        this.CreatedOn = createdOn;
    }
}