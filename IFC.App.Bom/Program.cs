using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;
using IFC.App.Bom.Models;

namespace IFC.App.Bom;
public class Program
{
    private static void Main(string[] args)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        Application app = new Application();
        app.Run();

        sw.Stop();
        Console.WriteLine($"Elapsed time: {sw.Elapsed} elements in BOM: {0000}");
    }
}