using System.Text.Json;
using AntDesign;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using EWS.Application.Language;
using EWS.Domain.Example;
using eXtensionSharp;

namespace EWS.BlazorServer.Components.Index;

public partial class UcCustomerDetail1
{
    private ColLayoutParam _labelCol = new ColLayoutParam 
    { 
        Xs = new EmbeddedProperty { Span = 24 },
        Sm = new EmbeddedProperty { Span = 6 },
    };

    private ColLayoutParam _wrapperCol = new ColLayoutParam
    {
        Xs = new EmbeddedProperty { Span = 24 },
        Sm = new EmbeddedProperty { Span = 14 },
    };
    private CustomerDetailModel _model = new CustomerDetailModel();
    private List<CascaderNode> _districts;
    private List<string> _autoCompleteOptions;

    record Person(string Name);

    private List<Person> _persons;

    protected override Task OnViewDataAsync()
    {
        _districts = new List<CascaderNode>
        {
            new CascaderNode()
            {
                Value = "1",
                Label = "Zhejianng",
                Children = new []
                {
                    new CascaderNode {Value = "11", Label = "Hangzhou"},
                    new CascaderNode {Value = "12", Label = "Wenzhou"},
                }
            },
            new CascaderNode()
            {
                Value = "2",
                Label = "Shanghai",
            }
        };      
        _autoCompleteOptions = new List<string> { "Primary", "Junior", "Senior", "Undergraduate", "Master", "Doctor" };
        _persons= new List<Person>
        {
            new Person("John"),
            new Person("Lucy"),
            new Person("Jack"),
            new Person("Emily"),
        };
        return base.OnViewDataAsync();
    }

    #region [function]

    private async Task OnFinish(EditContext editContext)
    {
        var vaild = editContext.Validate();
        if(vaild.xIsFalse()) return;
        
        this.ShowProgress();
        await Task.Delay(5000);
        Logger.LogInformation("Success:{Result}", _model.xToJson());
        this.CloseProgress();
        
        await this.ShowMessageAsync("저장 되었습니다.");
    }

    private void OnFinishFailed(EditContext editContext)
    {
        Logger.LogInformation("Failed:{Result}", _model.xToJson());
    }    

    #endregion
}