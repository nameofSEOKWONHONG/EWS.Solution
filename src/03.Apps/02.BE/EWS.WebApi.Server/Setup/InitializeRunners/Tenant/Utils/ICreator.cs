namespace EWS.WebApi.Server.ApplicationInitializer.Tenant;

/// <summary>
/// 
/// </summary>
public interface ICreator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    void Execute(string[] args);
}