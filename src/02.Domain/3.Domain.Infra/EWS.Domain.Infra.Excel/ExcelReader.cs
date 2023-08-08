using ClosedXML.Excel;
using eXtensionSharp;

namespace EWS.Domain.Infra.Excel;

public class ExcelReader
{
    private readonly string _filename;
    private readonly Stream _stream;
    private ExcelReader(string filename)
    {
        _filename = filename;
    }

    private ExcelReader(Stream stream)
    {
        _stream = stream;
    }

    public void Read(Action<int, IXLRangeRow> rowHandler, string sheetName)
    {
        XLWorkbook wb = null;
        if (_filename.xIsNotEmpty())
        {
            wb = new XLWorkbook(_filename);    
        }
        else
        {
            wb = new XLWorkbook(_stream);
        }
        
        var rows= wb.Worksheet(sheetName).RangeUsed().RowsUsed().Skip(1);
        foreach (var row in rows)
        {
            var rowNumber  = row.RowNumber();
            rowHandler(rowNumber, row);
        }
        wb.Dispose();
    }
    
    public void Read(Action<int, IXLRangeRow> rowHandler, int sheetNo = 1)
    {
        XLWorkbook wb = null;
        if (_filename.xIsNotEmpty())
        {
            wb = new XLWorkbook(_filename);    
        }
        else
        {
            wb = new XLWorkbook(_stream);
        }
        
        var rows= wb.Worksheet(sheetNo).RangeUsed().RowsUsed().Skip(1);
        foreach (var row in rows)
        {
            var rowNumber  = row.RowNumber();
            rowHandler(rowNumber, row);
        }
        wb.Dispose();
    }
    

    public static ExcelReader Create(string filename)
    {
        return new ExcelReader(filename);
    }

    public static ExcelReader Create(Stream stream)
    {
        return new ExcelReader(stream);
    }
}