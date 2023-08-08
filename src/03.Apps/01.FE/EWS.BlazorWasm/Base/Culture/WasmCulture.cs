using System.Globalization;
using EWS.BlazorWasm.Base;
using Microsoft.AspNetCore.Components;

namespace EWS.BlazorWasm;

public class WasmCulture : IWasmCulture
{
    private readonly NavigationManager _navigationManager;
    private readonly IStateHandler _state;
    
    private string _selectedLanguageCode;

    public string SelectedLanguageCode
    {
        get => _selectedLanguageCode;
        set
        {
            _selectedLanguageCode = value;
            _state.SetStateAsync(AppConstants.WasmHost.CULTURE_NAME, value);
            this.Culture = new CultureInfo(_selectedLanguageCode);
        }
    }
    
    public WasmCulture(IStateHandler state, NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _state = state;
    }
    
    public CultureInfo Culture
    {
        get => CultureInfo.CurrentCulture;
        set
        {
            if (CultureInfo.CurrentCulture != value)
            {
                _state.SetStateAsync(AppConstants.WasmHost.CULTURE_NAME, value.Name);
                _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
            }
        }
    }

    public async Task ChangeLanguageAsync(string languageCode)
    {
        await Task.Run(() => this.Culture = new CultureInfo(languageCode));
    }
}