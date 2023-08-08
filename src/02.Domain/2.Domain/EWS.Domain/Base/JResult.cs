using EWS.Application.Wrapper;

namespace EWS.Domain.Base;

public class JResult : IResultBase
{
    public JResult()
    {
    }

    public List<string> Messages { get; set; } = new List<string>();

    public bool Succeeded { get; set; }

    public static IResultBase Fail()
    {
        return new JResult { Succeeded = false };
    }

    public static IResultBase Fail(string message)
    {
        return new JResult { Succeeded = false, Messages = new List<string> { message } };
    }

    public static IResultBase Fail(List<string> messages)
    {
        return new JResult { Succeeded = false, Messages = messages };
    }

    public static Task<IResultBase> FailAsync()
    {
        return Task.FromResult(Fail());
    }

    public static Task<IResultBase> FailAsync(string message)
    {
        return Task.FromResult(Fail(message));
    }

    public static Task<IResultBase> FailAsync(List<string> messages)
    {
        return Task.FromResult(Fail(messages));
    }

    public static IResultBase Success()
    {
        return new JResult { Succeeded = true, Messages = new List<string>() { "Success." } };
    }

    public static IResultBase Success(string message)
    {
        return new JResult { Succeeded = true, Messages = new List<string> { "Success.", message } };
    }

    public static Task<IResultBase> SuccessAsync()
    {
        return Task.FromResult(Success());
    }

    public static Task<IResultBase> SuccessAsync(string message)
    {
        return Task.FromResult(Success(message));
    }
}

public class JResult<T> : JResult, IResultBase<T>
{
    public JResult()
    {
    }

    public T Data { get; set; }

    public new static JResult<T> Fail()
    {
        return new JResult<T> { Succeeded = false };
    }

    public new static JResult<T> Fail(string message)
    {
        return new JResult<T> { Succeeded = false, Messages = new List<string> { message } };
    }

    public new static JResult<T> Fail(List<string> messages)
    {
        return new JResult<T> { Succeeded = false, Messages = messages };
    }

    public new static Task<JResult<T>> FailAsync()
    {
        return Task.FromResult(Fail());
    }

    public new static Task<JResult<T>> FailAsync(string message)
    {
        return Task.FromResult(Fail(message));
    }

    public new static Task<JResult<T>> FailAsync(List<string> messages)
    {
        return Task.FromResult(Fail(messages));
    }

    public new static JResult<T> Success()
    {
        return new JResult<T> { Succeeded = true, Messages = new List<string>() { "Success." } };
    }

    public new static JResult<T> Success(string message)
    {
        return new JResult<T> { Succeeded = true, Messages = new List<string> { "Success.", message } };
    }

    public static JResult<T> Success(T data)
    {
        return new JResult<T> { Succeeded = true, Data = data, Messages = new List<string>() { "Success." } };
    }

    public static JResult<T> Success(T data, string message)
    {
        return new JResult<T> { Succeeded = true, Data = data, Messages = new List<string> { "Success.", message } };
    }

    public static JResult<T> Success(T data, List<string> messages)
    {
        messages.Insert(0, "Search Success.");
        return new JResult<T> { Succeeded = true, Data = data, Messages = messages };
    }

    public new static Task<JResult<T>> SuccessAsync()
    {
        return Task.FromResult(Success());
    }

    public new static Task<JResult<T>> SuccessAsync(string message)
    {
        return Task.FromResult(Success(message));
    }

    public static Task<JResult<T>> SuccessAsync(T data)
    {
        return Task.FromResult(Success(data));
    }

    public static Task<JResult<T>> SuccessAsync(T data, string message)
    {
        return Task.FromResult(Success(data, message));
    }
}