namespace EWS.Application.Language;

public interface ILocalizer
{
    string this[string name] { get; }
}