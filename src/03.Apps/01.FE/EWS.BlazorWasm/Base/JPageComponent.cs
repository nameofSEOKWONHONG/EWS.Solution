using Microsoft.AspNetCore.Components;

namespace EWS.BlazorWasm.Base;

public class JPageComponent<TRequest, TResponse> : ComponentBase
{
    protected AntDesign.Tabs Tabs { get; set; }
    protected TResponse SelectedItem { get; set; }

    protected virtual void OnTabChanged(string tabNum, TResponse item)
    {
        Tabs.ActiveKey = tabNum;
        SelectedItem = item;
    }
}