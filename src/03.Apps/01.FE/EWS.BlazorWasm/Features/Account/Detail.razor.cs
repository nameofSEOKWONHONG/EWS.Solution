using EWS.Domain.Account;
using eXtensionSharp;

namespace EWS.BlazorWasm.Features.Account;

public partial class Detail
{
    private readonly ILogger<Detail> _logger;
    protected override Task OnViewDataAsync()
    {
        if (this.SelectedItem.xIsEmpty())
            this.SelectedItem = new UserResult();
        return base.OnViewDataAsync();
    }

    protected override Task OnFinished()
    {
        _logger.LogDebug(this.SelectedItem.xToJson());
        return base.OnFinished();
    }
}