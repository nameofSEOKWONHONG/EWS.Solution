using eXtensionSharp;

namespace EWS.WebApi.Server.ApplicationInitializer.Tenant;

public class TenantParser
{
    private TenantParser()
    {
    }

    public TenantCreatorModel Parse(string[] args)
    {
        if (args.Length < 9) args = new[] { "", "", "", "", "", "", "", "", "", "" };
        return new TenantCreatorModel()
        {
            Id = args[1].xValue<string>("00000"),
            Name = args[2].xValue<string>("jiu-production"),
            DomainName = args[3].xValue<string>("jres.com"),
            Code = args[4].xValue<string>("jres"),
            TimeZone = args[5].xValue<string>("Korea Standard Time"),
            Email = args[6].xValue<string>("h20913@gmail.com"),
            IsActive = true,
            IsDefault = args[7].xValue<bool>(true)
        };
    }

    public static TenantParser Create()
    {
        return new TenantParser();
    }
}

/// <summary>
/// 
/// </summary>
public class TenantCreatorModel
{
    /// <summary>
    /// 테넌트 ID
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 테넌트 사용자명
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// redirect될 도메인명 (존 개념) (현재 사용 안함)
    /// </summary>
    public string DomainName { get; set; }

    /// <summary>
    /// 테넌트 코드 (현재 사용 안함)
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// 테넌트별 TimeZone
    /// </summary>
    public string TimeZone { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 기본 테넌트 여부 (시스템 루트)
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 활성화 여부
    /// </summary>
    public bool IsActive { get; set; }
    
    public string CreateBy { get; set; }
    public DateTime CreateOn { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime LastModifiedOn { get; set; }
}