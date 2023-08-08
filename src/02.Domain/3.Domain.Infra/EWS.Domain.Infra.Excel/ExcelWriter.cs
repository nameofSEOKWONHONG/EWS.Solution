using ClosedXML.Excel;

namespace EWS.Domain.Infra.Excel;

public class ExcelWriter
{
    private ExcelWriter()
    {
    }

    public void Write<T>(IList<T> list, string filename, string sheetname)
        where T : ExcelWriteBase
    {
        using var wb = new XLWorkbook();
        wb.AddWorksheet(sheetname).FirstCell().InsertTable(list, false);
        wb.SaveAs(filename);
    }

    public byte[] Write<T>(IList<T> list, string sheetname)
        where T : ExcelWriteBase
    {
        using var ms = new MemoryStream();
        using var wb = new XLWorkbook();
        wb.AddWorksheet(sheetname).FirstCell().InsertTable(list, false);
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public static ExcelWriter Create()
    {
        return new ExcelWriter();
    }
}