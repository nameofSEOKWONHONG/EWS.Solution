using EWS.Application.Wrapper;

namespace EWS.Domain.Base;

public class JPaginatedResult<T> : IPaginatedResult
{
    public JPaginatedResult(List<T> data)
    {
        Data = data;
    }

    public List<T> Data { get; set; }

    internal JPaginatedResult(bool succeeded, List<T> data = default, List<string> messages = null, int count = 0, int page = 1, int pageSize = 10)
    {
        Data = data;
        CurrentPage = page;
        Succeeded = succeeded;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
    }

    public static JPaginatedResult<T> Failure(List<string> messages)
    {
        return new JPaginatedResult<T>(false, default, messages);
    }

    public static JPaginatedResult<T> Success(List<T> data, int totalCount, int currentPage, int pageSize)
    {
        var result = new JPaginatedResult<T>(true, data, null, totalCount, currentPage, pageSize);
        result.Messages = new List<string>() { "Success." };
        return result;
    }

    public static Task<JPaginatedResult<T>> SuccessAsync(List<T> data, int totalCount, int currentPage,
        int pageSize)
    {
        return Task.FromResult(Success(data, totalCount, currentPage, pageSize));
    }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }
    public int PageSize { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
    public List<string> Messages { get; set; } = new List<string>();
    public bool Succeeded { get; set; }
}