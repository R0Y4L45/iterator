using ArrayToPdf;
using System.Data;

namespace Pdf;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class PdfFile<T>
{
    private readonly List<T> _list;
    public string FileName => $"{typeof(T).Name}.pdf";

    public PdfFile(List<T> list)
       => _list = list;

    public MemoryStream Create()
        => GetTable().ToPdfStream();

    private DataTable GetTable()
    {
        var table = new DataTable();

        var type = typeof(T);

        type.GetProperties()
            .ToList()
            .ForEach(x => table.Columns.Add(x.Name, x.PropertyType));

        _list.ForEach(x =>
        {
            var values = type.GetProperties()
                                .Select(properyInfo => properyInfo
                                .GetValue(x, null))
                                .ToArray();

            table.Rows.Add(values);
        });

        return table;
    }

}

public interface ITableActionCommand
{
    void Execute();
}


public class CreatePdfTableActionCommand<T> : ITableActionCommand
{
    private readonly PdfFile<T> _pdfFile;

    public CreatePdfTableActionCommand(PdfFile<T> pdfFile)
        => _pdfFile = pdfFile;
    public void Execute()
    {
        MemoryStream excelMemoryStream = _pdfFile.Create();
        File.WriteAllBytes(_pdfFile.FileName, excelMemoryStream.ToArray());
    }
}


class FileCreateInvoker
{
    private ITableActionCommand _tableActionCommand;
    private List<ITableActionCommand> tableActionCommands = new();

    public void SetCommand(ITableActionCommand tableActionCommand)
    {
        _tableActionCommand = tableActionCommand;
    }

    public void AddCommand(ITableActionCommand tableActionCommand)
    {
        tableActionCommands.Add(tableActionCommand);
    }

    public void CreateFile()
    {
        _tableActionCommand.Execute();
    }

}

class Program
{
    static void Main()
    {
        var products = Enumerable.Range(1, 30).Select(index =>
            new Product
            {
                Id = index,
                Name = $"Product {index}",
                Price = index + 100,
                Stock = index
            }
        ).ToList();

        PdfFile<Product> receiver = new(products);

        FileCreateInvoker invoker = new();
        invoker.SetCommand(new CreatePdfTableActionCommand<Product>(receiver));
        invoker.CreateFile();
    }
}