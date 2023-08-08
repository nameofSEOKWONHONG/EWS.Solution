using EWS.BlazorWasm.Base;

namespace EWS.BlazorWasm.Features.Account.Manager;

public interface IAccountManager : IManager
{
    
}

public class AccountManager : JManagerBase, IScopeManager, IAccountManager
{
    public AccountManager(HttpClient client, ILogger<AccountManager> logger) : base(client, logger)
    {
    }
}