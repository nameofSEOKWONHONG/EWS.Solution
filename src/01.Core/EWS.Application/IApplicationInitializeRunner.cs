namespace EWS.Application;

public interface IApplicationInitializeRunner
{
    bool Filter(string[] args);
    void Execute(string[] args);
}