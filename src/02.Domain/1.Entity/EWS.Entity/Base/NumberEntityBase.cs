using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWS.Entity.Base;

public abstract class NumberEntityBase : EntityBase
{
    [Key, Column(Order = 1)]
    public int Id { get; set; }

    protected NumberEntityBase()
    {
        
    }
}