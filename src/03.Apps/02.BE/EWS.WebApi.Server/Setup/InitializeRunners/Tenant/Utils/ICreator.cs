namespace EWS.WebApi.Server.ApplicationInitializer.Tenant;

public interface ICreator
{
    void Execute(string[] args);
}