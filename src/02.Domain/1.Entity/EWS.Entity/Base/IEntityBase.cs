namespace EWS.Entity.Base;

public interface IEntityBase
{
    string TenantId { get; set; }
    
    string CreatedBy { get; set; }
    
    DateTime? CreatedOn { get; set; }
    
    string CreatedName { get; set; }
    
    string LastModifiedBy { get; set; }
    
    DateTime? LastModifiedOn { get; set; }
    
    string LastModifiedName { get; set; }
}