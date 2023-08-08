using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Entity.Base;

[Table("Tenants", Schema = "identity")]
public class Tenant
{
    /// <summary>
    /// 테넌트 ID
    /// </summary>
    [MaxLength(5)]
    [Column(Order = 0)]
    public string Id { get; set; }

    /// <summary>
    /// 테넌트 사용자명
    /// </summary>
    [MaxLength(64), Column(Order = 1)]
    public string Name { get; set; }

    /// <summary>
    /// redirect될 도메인명 (존 개념)
    /// </summary>
    [MaxLength(128), Column(Order = 2)]
    public string DomainName { get; set; }

    /// <summary>
    /// 테넌트 코드 (현재 사용 안함)
    /// </summary>
    [MaxLength(48), Column(Order = 3)]
    public string Code { get; set; }
    
    /// <summary>
    /// 테넌트별 TimeZone
    /// </summary>
    [MaxLength(64), Column(Order = 4)]
    public string TimeZone { get; set; }

    /// <summary>
    /// 기본 테넌트 여부 (시스템 루트)
    /// </summary>
    [DefaultValue(false), Column(Order = 5)]
    public bool IsDefault { get; set; }

    /// <summary>
    /// 활성화 여부
    /// </summary>
    [DefaultValue(true), Column(Order = 6)]
    public bool IsActive { get; set; }
    
    [MaxLength(100), Column(Order = 7), AllowNull]
    public string CreatedBy { get; set; }
    
    [Column(Order = 8), AllowNull]
    public DateTime? CreatedOn { get; set; }
    
    [MaxLength(100), Column(Order = 9), AllowNull]
    public string LastModifiedBy { get; set; }
    
    [Column(Order = 10), AllowNull]
    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }

    public Tenant()
    { }

    public Tenant(string id, string name, string domainName, string code, string timeZone)
    {
        Id = id;
        Name = name;
        DomainName = domainName;
        Code = code;
        TimeZone = timeZone.xValue<string>("Korea Standard Time");
    }

    public static void Builder(ModelBuilder modelBuilder)
    {
        
    }
}