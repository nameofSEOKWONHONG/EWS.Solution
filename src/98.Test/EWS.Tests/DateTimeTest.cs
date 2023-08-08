using eXtensionSharp;

namespace EWS.Tests;

public class DateTimeTest
{
    [Test]
    public void datetime_test()
    {
        var dts = new List<DateTime>();
        Enumerable.Range(1, 2000).ToList().ForEach(i =>
        {
            dts.Add(DateTime.Now.AddMilliseconds(i));
        });
        
        TestContext.Out.WriteLine(dts.First().ToString(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS_FFF));
    }
}