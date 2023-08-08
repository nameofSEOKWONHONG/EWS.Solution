namespace EWS.BlazorWasm.Base;

public interface ISpinLayoutService
{
    bool Loading { get; set; }
    void ShowProgress();
    void CloseProgress();
    void OnStateChange(Action action);
}