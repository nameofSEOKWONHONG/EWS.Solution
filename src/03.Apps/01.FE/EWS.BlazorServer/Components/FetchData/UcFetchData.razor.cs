using AntDesign;
using AntDesign.TableModels;
using EWS.Application.Wrapper;
using EWS.Domain.Account.Users;
using EWS.Domain.Base;
using EWS.Domain.Example;
using eXtensionSharp;
using Microsoft.AspNetCore.Components;
using OneOf.Types;

namespace EWS.BlazorServer.Components.FetchData;

public partial class UcFetchData
{
    [Inject] private IFetchDataManager Manager { get; set; }
    private Table<WeatherForecastResult> _table;
    private WeatherForecastResult[] forecasts;
    private IEnumerable<WeatherForecastResult> _selectedRows;

    protected override Task OnViewDataAsync()
    {
        Manager.Write();
        return base.OnViewDataAsync();
    }
    
    protected override async Task OnPageDataAsync()
    {
        await OnSearchPaging<WeatherForecastResult>(new QueryModel<WeatherForecastResult>(1, 10, 1, null, null), null);
    }

    public override async Task OnFeedbackOkAsync(ModalClosingEventArgs args)
    {
        if (FeedbackRef is ConfirmRef confirmRef)
        {
            confirmRef.Config.OkButtonProps.Loading = true;
            await confirmRef.UpdateConfigAsync();
        }
        else if (FeedbackRef is ModalRef modalRef)
        {
            modalRef.Config.ConfirmLoading = true;
            await modalRef.UpdateConfigAsync();
        }
        
        await Task.Delay(1000);
        // only the input's value equals the initialized value, the OK button will close the confirm dialog box

        if (_selectedRows?.Count() <= 0)
        {
            args.Cancel = true;
        }
        else
        {
            // method 1(not recommended): await (FeedbackRef as ConfirmRef<string>)!.OkAsync(value);
            // method 2: await (FeedbackRef as IOkCancelRef<string>)!.OkAsync(value);
            await (FeedbackRef as IOkCancelRef<IEnumerable<WeatherForecastResult>>)!.OkAsync(_selectedRows);            
        }
        
        await base.OnFeedbackOkAsync(args);
    }

    #region [functions]

    private bool Filter(object v, object n)
    {
        return true;
    }

    protected override Task OnSearchPaging<T>(QueryModel<T> query, Func<int, int, string, string, Task<IPaginatedResult>> callback)
    {
        callback = async (index, size, sort, orderBy ) =>
        {
            Logger.LogInformation("sort:{sort}, orderby:{orderby}", sort, orderBy);
            var result = await this.Manager.GetAll<WeatherForecastResult>($"WeatherForecast/GetAll?pageSize={size}&pageNumber={index}&sort={sort}&orderby={orderBy}");
            forecasts = result.Data.ToArray();
            return result;
        };
        
        return base.OnSearchPaging(query, callback);
    }

    #endregion
}