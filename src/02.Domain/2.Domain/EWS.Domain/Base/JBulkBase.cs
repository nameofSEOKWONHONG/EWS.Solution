namespace EWS.Domain.Base;

/// <summary>
/// 다량 INSERT에 사용되는 기초 클래스로 상속받는 클래스는 테이블 컬럼과 1:1 맵핑 되어야 한다.
/// </summary>
public abstract class JBulkBase
{
    public string TenantId { get; set; }
    
    public string CreatedBy { get; set; }
    
    public string CreatedName { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; } = true;
}