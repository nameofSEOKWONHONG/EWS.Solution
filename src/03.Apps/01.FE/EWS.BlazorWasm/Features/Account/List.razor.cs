using AntDesign;
using AntDesign.TableModels;
using EWS.Application.Wrapper;
using EWS.BlazorWasm.Features.Account.Manager;
using EWS.Domain.Account;
using EWS.Domain.Account.Users;
using EWS.Domain.Base;
using Microsoft.AspNetCore.Components;

namespace EWS.BlazorWasm.Features.Account;

public partial class List
{
    [Inject] private IAccountManager _manager { get; set; }
    private IEnumerable<UserResult> _users;
    private Table<UserResult> _table;

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
        await OnSearchPaging<UserResult>(new QueryModel<UserResult>(1, 10, 1, null, null), null);
    }

    protected override Task OnSearchPaging<T>(QueryModel<T> query, Func<int, int, string, string, Task<IPaginatedResult>> callback)
    {
        callback = async (index, size, sort, orderBy ) =>
        {
            var result = await _manager.GetAll<UserResult>("api/user/getall", new GetAllUsersRequest()
            {
                UserName = string.Empty,
                PageNumber = index,
                PageSize = size,
            });
            _users = result.Data;
            return result;
        };
         
        return base.OnSearchPaging(query, callback);
    }
}