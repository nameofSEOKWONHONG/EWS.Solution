namespace EWS.Domain.Base;

public abstract class JBulkBase
{
    public string TenantId { get; set; }
    
    public string CreatedBy { get; set; }
    
    public string CreatedName { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; } = true;
}