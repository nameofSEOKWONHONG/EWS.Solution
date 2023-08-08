namespace EWS.BlazorShared.Base;

public interface ISpinLayoutService
{
    bool Loading { get; set; }
    void ShowProgress();
    void CloseProgress();
    void OnStateChange(Action action);
}