namespace EWS.Domain.Common;

public class ExportOption
{
    /// <summary>
    /// MimeType
    /// </summary>
    public string MimeType { get; set; }
        
    /// <summary>
    /// byte array
    /// </summary>
    public byte[] Bytes { get; set; }
        
    /// <summary>
    /// file name
    /// </summary>
    public string FileName { get; set; }
}