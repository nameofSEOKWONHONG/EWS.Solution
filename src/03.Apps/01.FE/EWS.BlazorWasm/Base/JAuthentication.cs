using System.Net.Http.Headers;
using System.Net.Http.Json;
using EWS.Application.Wrapper;
using EWS.BlazorWasm.Setup;
using EWS.Domain.Base;
using EWS.Domain.Identity;
using Microsoft.AspNetCore.Components.Authorization;

namespace EWS.BlazorWasm.Base;

public class JAuthentication : IAuthentication
{
    private readonly HttpClient _httpClient;
    private readonly IStateHandler _stateHandler;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly WasmSessionProvider _wasmSessionProvider;

    public JAuthentication(
        HttpClient httpClient,
        IStateHandler stateHandler,
        AuthenticationStateProvider authenticationStateProvider,
        WasmSessionProvider wasmSessionProvider)
    {
        _httpClient = httpClient;
        _stateHandler = stateHandler;
        _authenticationStateProvider = authenticationStateProvider;
        _wasmSessionProvider = wasmSessionProvider;
    }

    public async Task<IResultBase> Login(TokenRequest model)
    {
        var response = await this._httpClient.PostAsJsonAsync<TokenRequest>("api/Token", model);
        var result = await response.vToResult<TokenResponse>();
        if (result.Succeeded)
        {
            var token = result.Data.Token;
            var refreshToken = result.Data.RefreshToken;
            var userImageURL = result.Data.UserImageURL;
            await _stateHandler.SetStateAsync(StorageConstants.Local.AuthToken, token);
            await _stateHandler.SetStateAsync(StorageConstants.Local.RefreshToken, refreshToken);
            if (!string.IsNullOrEmpty(userImageURL))
            {
                await _stateHandler.SetStateAsync(StorageConstants.Local.UserImageURL, userImageURL);
            }

            await ((JAuthenticationStateProvider)_authenticationStateProvider).StateChangedAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await JResult.SuccessAsync();
        }
        else
        {
            return await JResult.FailAsync(result.Messages);
        }
    }

    public async Task<IResultBase> Logout()
    {
        var removeTargets = new[]
        {
            StorageConstants.Local.AuthToken,
            StorageConstants.Local.RefreshToken,
            StorageConstants.Local.UserImageURL,
        };

        foreach (var target in removeTargets)
        {
            await _stateHandler.RemoveStateAsync(target);
        }

        ((JAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        return await JResult.SuccessAsync();
    }

    public async Task<string> RefreshToken()
    {
        var token = await _stateHandler.GetStateAsync(StorageConstants.Local.AuthToken);
        var refreshToken = await _stateHandler.GetStateAsync(StorageConstants.Local.RefreshToken);

        var response = await _httpClient.PostAsJsonAsync("",
            new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });

        var result = await response.vToResult<TokenResponse>();

        if (!result.Succeeded)
        {
            throw new ApplicationException("Something went wrong during the refresh token action");
        }

        token = result.Data.Token;
        refreshToken = result.Data.RefreshToken;
        await _stateHandler.SetStateAsync(StorageConstants.Local.AuthToken, token);
        await _stateHandler.SetStateAsync(StorageConstants.Local.RefreshToken, refreshToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return token;
    }

    public async Task<string> TryRefreshToken()
    {
        //check if token exists
        var availableToken = await _stateHandler.GetStateAsync(StorageConstants.Local.RefreshToken);
        if (string.IsNullOrEmpty(availableToken)) return string.Empty;
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
        var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
        var timeUTC = DateTime.UtcNow;
        var diff = expTime - timeUTC;
        if (diff.TotalMinutes <= 1)
            return await RefreshToken();
        return string.Empty;
    }

    public async Task<string> TryForceRefreshToken()
    {
        return await RefreshToken();
    }

    public Task<IWasmSessionContext> CurrentUser()
    {
        return _wasmSessionProvider.GetSessionAsync();
    }
}