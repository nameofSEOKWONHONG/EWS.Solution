using AntDesign;

namespace EWS.BlazorServer.Components.Index;

public partial class UcCustomerDetail2
{
    private ColLayoutParam _labelCol = new() 
    { 
        Xs = new EmbeddedProperty { Span = 24 },
        Sm = new EmbeddedProperty { Span = 6 },
    };

    private ColLayoutParam _wrapperCol = new()
    {
        Xs = new EmbeddedProperty { Span = 24 },
        Sm = new EmbeddedProperty { Span = 14 },
    };

    private Model _model = new Model();

    private List<string> _options;
    private List<string> _options2;
    private IEnumerable<CascaderNode> _cascaderOptions = new[] { new CascaderNode { Label = "xx", Value = "xx" } };
    private TreeNode<string>[] _nodes = new TreeNode<string>[] { new() { Title = "xx", Key = "xx" } };

    protected override Task OnViewDataAsync()
    {
        _options = new List<string>
        {
            "Option 1", "Option 2", "Option 3"
        };
        _options2 = new List<string>
        {
            "Option 1", "Option 2", "Option 3"
        };
        
        return base.OnViewDataAsync();
    }
    
    internal class Model
    {
        public string Username { get; set; }
        public DateTime? DatePicker { get; set; }
        public DateTime? TimePicker { get; set; }
        public string SelectedOption { get; set; }
        public string SelectedOption2 { get; set; }
        public DateTime?[] RangePicker { get; set; }
        public DateTime? DatePicker2 { get; set; }
        public DateTime? DatePicker3 { get; set; }
        public string TreeSelect { get; set; }
        public int? InputNumber { get; set; }
        public string Input { get; set; }
        public string Password { get; set; }
    }
}

