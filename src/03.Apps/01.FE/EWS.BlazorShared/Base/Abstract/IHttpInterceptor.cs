using Toolbelt.Blazor;

namespace EWS.BlazorShared.Base.Abstract;

public interface IHttpInterceptor
{
    void RegisterEvent();

    Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

    void DisposeEvent();
}