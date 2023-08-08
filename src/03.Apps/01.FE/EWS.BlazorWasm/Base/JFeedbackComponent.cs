using AntDesign;
using EWS.Application.Language;
using eXtensionSharp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace EWS.BlazorWasm.Base;

/// <summary>
/// 기본 Component Base
/// 모든 Component는 Popup 화면이 될 수 있다.
/// Page와 Component를 구분한다.
/// Component는 User Control의 개념.
/// 화면 구현 이외에 사항에 대하여 처리한다.
/// </summary>
/// <typeparam name="TSelfComponent"></typeparam>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResult"></typeparam>
public abstract class JFeedbackComponent<TSelfComponent, TRequest, TResult> : FeedbackComponent<TRequest, TResult>
{
    [Parameter] public Action<int> OnTabChanged { get; set; }

    #region [inject]
    
    [Inject] protected ILocalizer Localizer { get; set; }
    [Inject] protected ModalService ModalService { get; set; }
    [Inject] protected IConfirmService ConfirmService { get; set; }
    [Inject] protected ISpinLayoutService SpinLayoutService { get; set; }
    [Inject] protected IJSRuntime JsRuntime { get; set; }
    
    [Inject] protected IMessageService MessageService { get; set; }    
    [Inject] protected NavigationManager NavigationManager { get; set; }
    
    [Inject] protected NotificationService NotificationService { get; set; }
    [Inject] protected HttpClient HttpClient { get; set; }

    #endregion
    
    #region [role]

    protected bool IsAdmin { get; set; }
    protected bool IsView { get; set; }
    protected bool IsRead { get; set; }
    protected bool IsWrite { get; set; }
    protected bool IsExport { get; set; }
    protected bool IsImport { get; set; }    

    #endregion

    protected string MenuCode { get; set; }
    protected bool IsInitialized;
    protected EditContext EditContext { get; set; }

    protected override void OnInitialized()
    {
        // MessageService.Config(new MessageGlobalConfig {
        //     Top = 24,
        //     Duration = 2,
        //     MaxCount = 3,
        //     Rtl = false,
        // });
        base.OnInitialized();
    }

    protected override async Task OnInitializedAsync()
    {
        await OnPermissionAsync();
        await OnViewDataAsync();
    }

    #region [init life cycle]

    protected virtual Task OnPermissionAsync()
    {
        if (RoleState.RoleStates.TryGetValue(this.MenuCode.xValue(""), out string[] roles))
        {
            //todo : check roles
        }
        
        return Task.CompletedTask;
    }

    protected virtual Task OnViewDataAsync()
    {
        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {            
            await OnPageDataAsync();
            await this.StateHasChangedAsync();
            IsInitialized = true;
        }
    }

    protected virtual Task OnPageDataAsync()
    {
        return Task.CompletedTask;
    }

    #endregion

    #region [function]

    protected async Task StateHasChangedAsync() => await this.InvokeAsync(StateHasChanged);
    
    protected async void GoBack()
    {
        await JsRuntime.InvokeVoidAsync("history.back");
    }
    
    protected void NavigateTo(string url, string state = "", bool forceLoad = false, bool replaceHistoryEntry = false)
    {
        NavigationManager.NavigateTo(url, new NavigationOptions
        {
            HistoryEntryState = state,
            ForceLoad = forceLoad,
            ReplaceHistoryEntry = replaceHistoryEntry
        });
    }    
    
    protected string GetHistoryEntryState()
    {
        return NavigationManager.HistoryEntryState;
    }
    
    protected T GetHistoryEntryState<T>()
    {
        var hes = NavigationManager.HistoryEntryState;
        if (hes.xIsNotEmpty())
        {
            return hes.xToEntity<T>();
        }

        return default;
    }

    protected async Task InvokeVoidAsync(string command)
    {
        await JsRuntime.InvokeVoidAsync(command);
    }

    protected async Task InvokeVoidAsync(string command, string args)
    {
        await JsRuntime.InvokeVoidAsync(command, args);
    }

    protected async Task<T> InvokeAsync<T>(string command, string args)
    {
        return await JsRuntime.InvokeAsync<T>(command, args);
    }

    #endregion
}