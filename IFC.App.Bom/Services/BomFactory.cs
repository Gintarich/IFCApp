using IFC.App.Bom.Creators;
using Tekla.Structures.Model;

namespace IFC.App.Bom.Services
{
    public class BomFactory
    {
        private List<IBomCreator> _bomCreators = [];

        public BomFactory(List<IBomCreator> creators)
        {
            _bomCreators = creators;
            _bomCreators.Add(new DefaultBomCreator());
        }

        public List<IBomCreator> GetBomCreators()
        {
            return _bomCreators;
        }

        public void GetElements(List<Assembly> assemblies)
        {
            foreach (var creator in _bomCreators)
            {
                creator.TakeElements(assemblies);
            }
            if (assemblies.Count() > 0)
            {
                throw new Exception("Daži elementi netika pievienoti specifikācijai");
            }
        }

        public void PrintElements()
        {
            foreach (var creator in _bomCreators)
            {
                var model = new Model();
                var modelPath = model.GetInfo().ModelPath;
                var excelPath = Path.Combine(modelPath, "Automation", "BOM.xlsx");
                creator.PrintElements();
            }
        }
    }
}
