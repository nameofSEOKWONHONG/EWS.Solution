using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EWS.Entity.Base;

namespace EWS.Entity;

[Table("Resouces", Schema = "Business")]
public class Resource : CodeEntityBase
{
    [MaxLength(2000)]
    public string Name { get; set; }
    [MaxLength(4000)]
    public string Description { get; set; }
    public virtual ICollection<SubResource> MultiResources { get; set; }
}

