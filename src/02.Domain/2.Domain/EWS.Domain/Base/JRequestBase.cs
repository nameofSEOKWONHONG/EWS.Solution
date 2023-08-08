namespace EWS.Domain.Base;

public class JRequestBase
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    /// <summary>
    /// AES, DESC
    /// </summary>
    public string Sort { get; set; }
    public string Orderby { get; set; }
}

public class JCodeRequestBase : JRequestBase
{
    public string Code { get; set; }
}