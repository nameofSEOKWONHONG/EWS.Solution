using System.Text.Json;
using System.Text.Json.Serialization;
using EWS.Application.Wrapper;
using EWS.BlazorShared;
using EWS.BlazorShared.Base;
using EWS.BlazorShared.Setup;
using EWS.Domain.Base;
using EWS.Domain.Identity;
using Microsoft.AspNetCore.Components.Authorization;

namespace EWS.BlazorServer.Shared.Authentication.Manager;

public class TokenManager : JManagerBase, ITokenManager, IScopeManager
{
    private readonly IStateHandler _stateHandler;
    private readonly AuthenticationStateProvider _authenticationState;
    public TokenManager(HttpClient httpClient,
        IStateHandler stateHandler,
        AuthenticationStateProvider authenticationState) : base(httpClient, null)
    {
        _stateHandler = stateHandler;
        _authenticationState = authenticationState;
    }
    
    public async Task<IResultBase> Login(TokenRequest model)
    {
        var response = await this.Client.PostAsJsonAsync<TokenRequest>("api/Token", model);
        var result = await ToResult<TokenResponse>(response);
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
    
            await ((JAuthenticationStateProvider)_authenticationState).StateChangedAsync();
            return await JResult.SuccessAsync();
        }
        else
        {
            return await JResult.FailAsync(result.Messages);
        }
    }
    
    private async Task<IResultBase<T>> ToResult<T>(HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JResult<T>>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject;
    }    
}