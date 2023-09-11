using System.Globalization;
using System.Net;
using EWS.Domain.Base;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Infra.Middleware;

public sealed class JErrorHandleMiddleware
{
    private readonly RequestDelegate _next;

    public JErrorHandleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var responseModel = await JResult.FailAsync(error.Message);

            switch (error)
            {
                case ApiException e:
                    // custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case KeyNotFoundException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.OK;
                    break;
            }

            var result = responseModel.xToJson();
            await response.WriteAsync(result);
        }
    }
}

public class ApiException : Exception
{
    public ApiException() : base()
    {
    }

    public ApiException(string message) : base(message)
    {
    }

    public ApiException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}