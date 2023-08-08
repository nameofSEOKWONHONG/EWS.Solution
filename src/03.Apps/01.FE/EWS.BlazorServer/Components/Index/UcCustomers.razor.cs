using AntDesign;
using EWS.BlazorServer.Components.FetchData;
using EWS.BlazorShared.Base;
using EWS.Domain.Example;
using eXtensionSharp;
using Microsoft.AspNetCore.Components;

namespace EWS.BlazorServer.Components.Index;

public partial class UcCustomers
{
    [Parameter] public Action OnChange { get; set; }
    
    private ITable _table;
    private Column[] _data;
    private IEnumerable<Column> _selectedRows;
    private string _selectionType = "checkbox";
    private bool _expand = false;
    private AntDesign.Internal.IForm _form;
    private Dictionary<string, FormValue> _formData = new();
    private string _value;
    private List<UncertainCategoryOption> _options = new();    

    protected override Task OnPermissionAsync()
    {
        return base.OnPermissionAsync();
    }

    protected override Task OnViewDataAsync()
    {
        for (var i = 0; i < 10; i++)
        {
            _formData.Add($"field-{i}", new FormValue());
        }
        return base.OnViewDataAsync();
    }

    protected override Task OnPageDataAsync()
    {
        _data = new Column[]
        {
            new Column()
            {
                Name = "John Brown",
                Age = 32,
                Address = "New York No. 1 Lake Park",
            },
            new Column()
            {
                Name = "Jim Green",
                Age = 42,
                Address = "London No. 1 Lake Park",
            },
            new Column()
            {
                Name = "Joe Black",
                Age = 32,
                Address = "Sidney No. 1 Lake Park",
            },
            new Column()
            {
                Name = "Disabled User",
                Age = 99,
                Address = "Sidney No. 1 Lake Park",
            }
        };
        
        return base.OnPageDataAsync();
    }

    #region [function]

    private void RemoveSelection(string key)
    {
        var selected = _selectedRows.Where(x => x.Name != key).ToList();
        _table.SetSelection(selected.Select(x => x.Name).ToArray());
    }

    private void Callback(string[] keys)
    {
        Logger.LogInformation("Callback : {Result}", keys.xJoin());
    }

    private void OnInput(ChangeEventArgs e)
    {
        var v = e.Value.ToString();

        var r = new Random();
        var i = 0;
        this._options = new int[7].Select(x => new UncertainCategoryOption()
        {
            Title = v,
            Category = $"{v}{i++}",
            Count = r.Next(100, 200),
        }).ToList();
    }

    private async Task OnDialogSearch()
    {
        await this.ShowDialog<UcFetchData, JDlgOptions<bool>, IEnumerable<WeatherForecastResult>>(new JDlgOptions<bool>(), result =>
        {
            Logger.LogInformation("Result : {Result}", result.xToJson());
        });        
    }

    #endregion
}

internal class Column
{
    public string Name { get; set; }

    public int Age { get; set; }

    public string Address { get; set; }
}

internal class UncertainCategoryOption
{
    public string Title { get; set; }

    public string Category { get; set; }
    public int Count { get; set; }
}    

internal class FormValue
{
    public string Value { get; set; }
}