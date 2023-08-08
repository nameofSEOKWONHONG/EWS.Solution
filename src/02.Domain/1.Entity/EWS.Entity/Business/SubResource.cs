using System.ComponentModel.DataAnnotations.Schema;
using EWS.Entity.Base;

namespace EWS.Entity;

[Table("SubResources", Schema = "Business")]
public class SubResource : NumberEntityBase
{
    public string ResourceUid { get; set; }
    public Resource Resource { get; set; }
    
    public string LangType { get; set; }
    public string Name { get; set; }
}