using EWS.Application.Language;
using EWS.Domain.Base;
using FluentValidation;

namespace EWS.Domain.Example;

public class CustomerDetailModel
{
    public string Size { get; set; } = "small";
    public string Input { get; set; } = "input";
    public string InputArea { get; set; } = "inputArea";
    public string Cascader { get; set; } = "11";
    public DateTime? DatePicker { get; set; } = DateTime.Now;
    public DateTime?[] RangePicker { get; set; } = new DateTime?[] { DateTime.Now, DateTime.Now.AddDays(10) };
    public double Number { get; set; } = 1;
    public bool Switch { get; set; } = true;
    public string Radio { get; set; } = "Beijing";
    public string AutoComplete { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Names { get; set; } = Array.Empty<string>();
}

public class CustomerDetailModelValidatorBase : JValidatorBase<CustomerDetailModel>
{
    public CustomerDetailModelValidatorBase(ILocalizer localizer) : base(localizer)
    {       
        NotEmpty(m => m.Input);
        MinLength(m => m.Input, 5);
        MaxLength(m => m.Input, 10);
        //MinMaxLength(m => m.Input, 5, 10);
        GreaterThan(m => m.Number, 0);
        LessThan(m => m.Number, 5);
    }
}