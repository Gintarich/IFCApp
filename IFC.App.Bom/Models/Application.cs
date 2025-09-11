using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;
using IFC.App.Bom.Services;
using IFC.App.Bom.Creators;

namespace IFC.App.Bom.Models
{
    public class Application
    {
        List<Assembly> _assemblies = new List<Assembly>();
        BomFactory _bomFactory;
        ExcelService _excelService;
        public Application()
        {
            _excelService = new ExcelService(GetFilePath());
            _assemblies = GetElements();
            _bomFactory = new BomFactory(new List<IBomCreator>
            {
                new PrecastSandwichWallCreator(),
                new PrecastWallCreator(),
                new PrecastHcsCreator(),
                new PrecastBomCreator(),
                new ConcreteBomCreator(),
                new SteelBomCreator(),
            });
        }

        public void Run()
        { 
            _bomFactory.GetElements(_assemblies);
            var creators = _bomFactory.GetBomCreators();
            _excelService.ExportToExcel(creators, PrintType.ToSingleFile);
        }
        private string GetFilePath()
        {
            var model = new Model();
            var modelPath = model.GetInfo().ModelPath;
            var scriptPath = Path.Combine(modelPath, "Automation");
            if (!Directory.Exists(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }
            var excelPath = Path.Combine(scriptPath, "BOM.xlsx");
            return excelPath;
        }

        public List<Assembly> GetElements()
        {
            Model model = new Model();
            var mos = model.GetModelObjectSelector();
            return mos.GetAllObjectsWithType(ModelObject.ModelObjectEnum.ASSEMBLY).ToList().Cast<Assembly>()
                .OrderBy(x => x.GetAssemblyType())
                .ThenBy(x => x.Name)
                .Where(x => x.GetIntProp("HIERARCHY_LEVEL") == 0 && x.Name != "SMALKGRAUDAINS BETONS")
                .ToList();
        }
    }
}
