namespace EWS.Domain.Infra.Service;

public interface INodeHostService
{
    Task NpmVoidRun(string javascript);
    Task<T> NpmRun<T>(string javascript);
}