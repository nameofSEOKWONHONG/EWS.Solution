using Toolbelt.Blazor;

namespace EWS.BlazorWasm.Base;

public interface IHttpInterceptor
{
    void RegisterEvent();

    Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

    void DisposeEvent();
}