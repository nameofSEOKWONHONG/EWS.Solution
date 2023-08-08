namespace EWS.BlazorWasm.Base;

public class SpinLayoutService : ISpinLayoutService
{
    private bool _loading = false;
    private Action _action;

    public bool Loading
    {
        get => _loading;
        set
        {
            _loading = value;
        }
    }
    public void ShowProgress()
    {
        _loading = true;
        _action();
    }

    public void CloseProgress()
    {
        _loading = false;
        _action();
    }

    public void OnStateChange(Action action)
    {
        _action = action;
    }
}