using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Entity.Base;

public abstract class YearEntityBase : EntityBase
{
    [Key, Column(Order = 1)]
    public int Year { get; set; }
    
    [Key, Column(Order = 2)]
    public string UId { get; set; }

    protected YearEntityBase()
    {
        
    }
}