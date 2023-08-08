using System.Net.Http.Json;
using EWS.Application.Wrapper;
using EWS.BlazorWasm;
using EWS.BlazorWasm.Base;
using EWS.BlazorWasm.Setup;
using EWS.Domain.Base;
using EWS.Domain.Identity;

namespace EWS.BlazorWasm.Pages.User.Manager;

public class TokenManager : JManagerBase, ITokenManager, IScopeManager
{
    private readonly IStateHandler _stateHandler;
    private readonly JAuthenticationStateProvider _authenticationState;
    public TokenManager(HttpClient httpClient, 
        ILogger<TokenManager> logger,
        IStateHandler stateHandler,
        JAuthenticationStateProvider authenticationState) : base(httpClient, logger)
    {
        _stateHandler = stateHandler;
        _authenticationState = authenticationState;
    }
    
    public async Task<IResultBase> Login(TokenRequest model)
    {
        var response = await this.Client.PostAsJsonAsync<TokenRequest>("api/Token", model);
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
    
            await _authenticationState.StateChangedAsync();
            return await JResult.SuccessAsync();
        }
        else
        {
            return await JResult.FailAsync(result.Messages);
        }
    }
}