using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EWS.Entity.Base;

[Index(nameof(Name), IsUnique = false, Name = "IX_CODE_BASE_1")]
public abstract class CodeEntityBase : EntityBase
{
    [Key, Column(Order = 1), MaxLength(100)]
    public string UId { get; set; }
    
    [Key, Column(Order = 2), MaxLength(100)]
    public string Code { get; set; }
    
    [Column(Order = 3), MaxLength(400)]
    public string Name { get; set; }
    
    [Column(Order = 4)]
    public bool IsSaveState { get; set; }

    protected CodeEntityBase()
    {
        
    }

    protected CodeEntityBase(string tenantId, string code, string name, string createdBy, DateTime createdOn) : base(tenantId, createdBy, createdOn)
    {
        this.Code = code;
        this.Name = name;
    }
}