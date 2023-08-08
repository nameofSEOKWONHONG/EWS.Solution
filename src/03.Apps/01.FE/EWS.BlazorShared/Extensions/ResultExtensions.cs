using System.Text.Json;
using System.Text.Json.Serialization;
using EWS.Application.Wrapper;
using EWS.Domain.Base;

namespace EWS.BlazorShared;

public static class ResultExtensions
{
    public static async Task<IResultBase<T>> vToResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JResult<T>>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject;
    }

    public static async Task<IResultBase> vToResult(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JResult>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject;
    }

    public static async Task<JPaginatedResult<T>> vToPaginatedResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JPaginatedResult<T>>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return responseObject;
    }
}