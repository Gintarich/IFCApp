using IFC.App.Bom.Models;
using Tekla.Structures.Model;

namespace IFC.App.Bom.Creators
{
    public interface IBomCreator
    {
        public void TakeElements(List<Assembly> assemblies);
        public void PrintElements();
        public List<IElement> GetElements();
        public SheetInfo GetSheetInfo();
        public List<List<List<string>>> GetData();
        public int GetCount();
    }
}
