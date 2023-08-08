using EWS.BlazorServer.Components.FetchData;
using EWS.BlazorShared.Base;
using EWS.Domain.Example;
using eXtensionSharp;

namespace EWS.BlazorServer.Components.Index;

public partial class UcCustomerStatus
{
    private IDictionary<string, (bool edit, ItemData data)> _editCache = new Dictionary<string, (bool edit, ItemData data)>();
    private List<ItemData> _listOfData = new();
    private string _editId;
    private string _title = "BasicModal";
    private bool _visible1 = false;
    private bool _visible2 = false;
    private int _i = 0;
    

    protected override Task OnPermissionAsync()
    {
        return base.OnPermissionAsync();
    }

    protected override Task OnViewDataAsync()
    {
        return base.OnViewDataAsync();
    }

    protected override async Task OnPageDataAsync()
    {
        this.ShowProgress();
        await Task.Delay(1000);
        _listOfData = Enumerable.Range(0, 100).Select(_i => new ItemData { Id = $"{_i}", Name = $"Edrward {_i}", Age = 32, Address = $"London Park no. {_i}", Date = DateTime.Now}).ToList();
        _listOfData.ForEach(item =>
        {
            _editCache[item.Id] = (false, item);
        });
        this.CloseProgress();
    }

    #region [functions]

    private void StartEdit(string id)
    {
        var data = _editCache[id];
        _editCache[id] = (true, data.data with { }); // add a copy in cache
    }

    private void CancelEdit(string id)
    {
        var data = _listOfData.FirstOrDefault(item => item.Id == id);
        _editCache[id] = (false, data); // recovery
    }

    private void SaveEdit(string id)
    {
        var index = _listOfData.FindIndex(item => item.Id == id);
        _listOfData[index] = _editCache[id].data; // apply the copy to data source
        _editCache[id] = (false, _listOfData[index]); // don't affect rows in editing
    }
    
    private void HideModal1()
    {
        if (_visible1)
        {
            _visible1 = false;
        }
    }

    private void HideModal2()
    {
        if (_visible2)
        {
            _visible2 = false;
        }
    }

    private void SetModal1Visible()
    {
        _visible1 = true;
    }
    
    private async Task SetModal2Visible()
    {
        //_visible2 = true;
        await Click();
    }
    
    private async Task Click()
    { 
        await this.ShowDialog<UcFetchData, JDlgOptions<bool>, IEnumerable<WeatherForecastResult>>(new JDlgOptions<bool>(), result =>
        {
            Logger.LogInformation("UcFetchData result : {Result}", result.xToJson());
        });
    }

    #endregion
}

internal record ItemData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
    public DateTime Date { get; set; }
};