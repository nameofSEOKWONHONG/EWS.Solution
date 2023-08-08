using System.Net.Http.Headers;
using EWS.BlazorShared.Base.Abstract;
using Microsoft.AspNetCore.Components;
using Toolbelt.Blazor;

namespace EWS.BlazorShared.Base;

public class JHttpInterceptor : IHttpInterceptor
{
    private readonly HttpClientInterceptor _interceptor;
    private readonly IAuthentication _authentication;
    private readonly NavigationManager _navigationManager;

    public JHttpInterceptor(HttpClientInterceptor interceptor,
        IAuthentication authentication,
        NavigationManager navigationManager)
    {
        _interceptor = interceptor;
        _authentication = authentication;
        _navigationManager = navigationManager;
    }
    
    public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;

    public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        var absPath = e.Request.RequestUri.AbsolutePath;
        if (!absPath.Contains("token") && !absPath.Contains("accounts"))
        {
            try
            {
                var token = await _authentication.TryRefreshToken();
                if (!string.IsNullOrEmpty(token))
                {
                    e.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                await _authentication.Logout();
                _navigationManager.NavigateTo("/Login");
            }
        }
    }

    public void DisposeEvent() => _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
}