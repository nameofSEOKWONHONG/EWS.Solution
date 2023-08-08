namespace EWS.Domain.Infra.Abstract;

public interface IRedisSubscriberWorkerImplBase
{
    Task ExecuteAsync();
}
