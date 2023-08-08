namespace EWS.BlazorShared.Base;

public abstract class JDlgOptionsBase
{
    /// <summary>
    /// checkbox, radio
    /// </summary>
    public string SelectRowType { get; set; } = "checkbox";
}

public class JDlgOptions : JDlgOptionsBase
{
    
}

public class JDlgOptions<T> : JDlgOptions
{
    public T Param { get; set; }
}