namespace EWS.Application.Wrapper;

public interface IResultBase
{
    List<string> Messages { get; set; }

    bool Succeeded { get; set; }
}

public interface IResultBase<out T> : IResultBase
{
    T Data { get; }
}