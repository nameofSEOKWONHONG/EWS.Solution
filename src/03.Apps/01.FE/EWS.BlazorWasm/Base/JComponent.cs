using AntDesign;
using AntDesign.TableModels;
using EWS.Application.Wrapper;
using EWS.Domain.Common;
using eXtensionSharp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace EWS.BlazorWasm.Base;

/// <summary>
/// 기본 화면 Component Base
/// 로그인 이후 화면은 모두 Component화 하고 각 Component에 상속하여 개발한다.
/// 화면과 관련된 사항은 모두 아래 클래스에서 구현한다.
/// </summary>
/// <typeparam name="TSelfComponent"></typeparam>
/// <typeparam name="TDlgOptions"></typeparam>
/// <typeparam name="TDlgResult"></typeparam>
public abstract class JComponent<TSelfComponent, TDlgOptions, TDlgResult> : JFeedbackComponent<TSelfComponent, TDlgOptions, TDlgResult>
    where TDlgOptions : JDlgOptionsBase, new()
{
    [Parameter] public Action<TDlgResult> OnTabChange { get; set; }
    [Parameter] public TDlgResult SelectedItem { get; set; }
    
    protected ColLayoutParam LabelCol = new ColLayoutParam 
    { 
        Xs = new EmbeddedProperty { Span = 24 },
        Sm = new EmbeddedProperty { Span = 6 },
    };

    protected ColLayoutParam WrapperCol = new ColLayoutParam
    {
        Xs = new EmbeddedProperty { Span = 24 },
        Sm = new EmbeddedProperty { Span = 14 },
    };  
    
    protected TDlgOptions DlgOptions { get; set; }

    private const int INTERVAL = 500;
    protected int DefaultPageSize = 10;
    protected int Total = 0;
    protected IEnumerable<TDlgResult> SelectedRows;

    //string[] positions = new[] { "none", "topLeft", "topCenter", "topRight", "bottomLeft", "bottomCenter", "bottomRight" };
    protected string DefaultPagingPosition = "bottomRight";
    
    protected override void OnInitialized()
    {
        if (this.Options.xIsEmpty())
        {
            this.DlgOptions = new TDlgOptions();
        }
        else
        {
            this.DlgOptions = this.Options;
        }
        this.SpinLayoutService.OnStateChange(this.StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        this.ShowProgress();
        await base.OnInitializedAsync();
        await Task.Delay(INTERVAL);
        this.CloseProgress();
    }

    protected async Task ShowDialog<TComponent, TDlgOptions1, TDlgResult1>(TDlgOptions1 dlgOptions, Action<TDlgResult1> action)
        where TComponent : FeedbackComponent<TDlgOptions1, TDlgResult1>
        where TDlgOptions1 : JDlgOptionsBase
    {
        var options = new ConfirmOptions()
        {
            Width = "100%",
            OnOk = e =>
            {
                return Task.CompletedTask;
            },
            OnCancel = e =>
            {
                return Task.CompletedTask;
            },
        };
        if (dlgOptions.xIsNotEmpty())
        {
            dlgOptions.SelectRowType = "radio";
        }
        var confirmRef = await ModalService.CreateConfirmAsync<TComponent, TDlgOptions1, TDlgResult1>(options, dlgOptions);
        confirmRef.OnOpen = () =>
        {
            return Task.CompletedTask;
        };
    
        confirmRef.OnClose = () =>
        {
            return Task.CompletedTask;
        };
    
        confirmRef.OnOk = result =>
        {
            action(result);
            return Task.CompletedTask;
        };
    }

    protected virtual async Task ShowMessageAsync(string message, ENUM_MESSAGE_TYPE type = ENUM_MESSAGE_TYPE.Info)
    {
        switch (type)
        {
            case ENUM_MESSAGE_TYPE.Info:
                await this.NotificationService.Info(new NotificationConfig()
                {
                    Message = "Information",
                    Description = message
                });
                break;
            case ENUM_MESSAGE_TYPE.Success:
                await this.NotificationService.Success(new NotificationConfig()
                {
                    Message = "Success",
                    Description = message
                });
                break;
            case ENUM_MESSAGE_TYPE.Warning:
                await this.NotificationService.Warning(new NotificationConfig()
                {
                    Message = "Warning",
                    Description = message
                });
                break;
            case ENUM_MESSAGE_TYPE.Error:
                await this.NotificationService.Error(new NotificationConfig()
                {
                    Message = "Error",
                    Description = message
                });
                break;
        }
    }

    protected virtual async Task ShowConfirmAsync(string title, string message, ConfirmIcon icon = ConfirmIcon.Info)
    {
        await ConfirmService.Show(
            message,
            title,
            ConfirmButtons.OK,
            icon);
    }

    protected virtual async Task<bool> ShowConfirmOkCancelAsync(string title, string message, ConfirmIcon icon = ConfirmIcon.Info)
    {
        var result = await ConfirmService.Show(
            message,
            title,
            ConfirmButtons.OKCancel,
            icon);
        
        return result == ConfirmResult.OK || result == ConfirmResult.Yes;
    }

    protected virtual void ShowProgress()
    {
        SpinLayoutService.ShowProgress();
        this.StateHasChanged();
    }
    
    protected virtual void CloseProgress()
    {
        SpinLayoutService.CloseProgress();
        this.StateHasChanged();
    }
    
    /// <summary>
    /// 단축키, 실제 기능은 추가되어야 함.
    /// </summary>
    /// <param name="e"></param>
    protected async Task OnKeyUp(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await this.ShowMessageAsync(e.Code);       
        }
        else if (e.AltKey == true && e.Code == "F1")
        {
            await this.ShowMessageAsync(e.Code);
        }
        else if (e.AltKey && e.Code == "F2")
        {
            await this.ShowMessageAsync(e.Code);
        }
    }

    #region [ui crud events]

    protected virtual async Task OnSearchPaging<T>(QueryModel<T> query, Func<int, int, string, string, Task<IPaginatedResult>> callback)
    {
        if(callback.xIsEmpty()) return;
        this.ShowProgress();
        
        await Task.Delay(INTERVAL);
        
        IPaginatedResult result = null;
        if (query.SortModel.xIsNotEmpty())
        {
            var sort = query.SortModel.FirstOrDefault(m => m.Sort.xIsNotEmpty());
            if (sort.xIsNotEmpty())
            {
                if (sort.Sort == "ascend")
                {
                    result = await callback(query.PageIndex, query.PageSize, sort!.FieldName, "ASC");    
                }
                else
                {
                    result = await callback(query.PageIndex, query.PageSize, sort!.FieldName, "DESC");
                }
            }
            else
            {
                result = await callback(query.PageIndex, query.PageSize, string.Empty, string.Empty);    
            }
        }
        else
        {
            result = await callback(query.PageIndex, query.PageSize, string.Empty, string.Empty);
        }

        if (result.Succeeded)
        {
            Total = result.TotalCount;
        }
        
        await this.StateHasChangedAsync();
        this.CloseProgress();
    }
    
    protected virtual async Task OnChange<T>(QueryModel<T> query)
    {
        if (IsInitialized.xIsFalse()) return;

        await OnSearchPaging(query, null);
        await this.StateHasChangedAsync();
    }

    protected virtual async Task OnSearch(Func<Task<IResultBase>> callback)
    {
        if (callback.xIsEmpty()) return;

        this.ShowProgress();
        var result = await callback();
        this.CloseProgress();
        await this.ShowMessageAsync(result.Messages.xFirst());
    }

    protected virtual async Task OnSave(Func<Task<IResultBase>> callback)
    {
        if(callback.xIsEmpty()) return;
        
        this.ShowProgress();
        var result = await callback();
        this.CloseProgress();
        await this.ShowMessageAsync(result.Messages.xFirst());
    }

    protected virtual async Task OnSaveBack(Func<Task<IResultBase>> callback)
    {
        await OnSave(callback);
        await JsRuntime.InvokeVoidAsync("history.back");
    }

    protected virtual async Task OnExport(Func<Task<IResultBase<ExportOption>>> callback)
    {
        if (callback.xIsEmpty())
        {
            await this.ShowMessageAsync("Not support function", ENUM_MESSAGE_TYPE.Error);
            return;
        }
            
        this.ShowProgress();
        var result = await callback();
        if (result.xIsNotEmpty())
        {
            if (result.Succeeded)
            {
                await JsRuntime.InvokeVoidAsync("download", new
                {
                    mimeType = result.Data.MimeType,
                    byteArray = Convert.ToBase64String(result.Data.Bytes),
                    fileName = result.Data.FileName
                });
                await this.ShowMessageAsync("Export success");
            }
            else
            {
                if (result.Messages.xIsNotEmpty())
                {
                    await this.ShowMessageAsync($"Export failed: {result.Messages.xJoin()}");    
                }
                else
                {
                    await this.ShowMessageAsync($"Export failed");
                }
            }            
        }
        else
        {
            await this.ShowMessageAsync("Export failed: Data is empty.");
        }
        this.CloseProgress();
    }

    protected virtual async Task OnImport(Func<Task<IResultBase>> callback)
    {
        if (callback.xIsEmpty())
        {
            await this.ShowMessageAsync("not support function", ENUM_MESSAGE_TYPE.Error);
            return;
        }

        this.ShowProgress();
        var result = await callback();
        await this.ShowMessageAsync(result.Messages.xFirst());
        this.CloseProgress();
    }

    protected void OnRowClick(RowData<TDlgResult> row)
    {
        if (OnTabChange.xIsNotEmpty())
        {
            OnTabChange(row.Data);
        }
    }

    protected virtual Task OnFinished()
    {
        return Task.CompletedTask;
    }
    
    protected virtual async Task OnFinish(EditContext editContext)
    {
        var vaild = editContext.Validate();
        if(vaild.xIsFalse()) return;
        this.ShowProgress();
        await Task.Delay(3000);
        await OnFinished();
        this.CloseProgress();
        await this.ShowMessageAsync("처리 되었습니다.");
    }

    protected virtual void OnFinishFailed(EditContext editContext)
    {
         
    }
    #endregion
    
}

/// <summary>
/// 메세지 타입
/// </summary>
public enum ENUM_MESSAGE_TYPE
{
    Info,
    Success,
    Error,
    Warning
}