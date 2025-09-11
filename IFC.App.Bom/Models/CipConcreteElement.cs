using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;

namespace IFC.App.Bom.Models
{
    public class CipConcreteElement : IElement, IEquatable<CipConcreteElement>
    {
        public ElementPosition Marka { get; set; }
        public string Nosaukums { get; set; }
        public int Count { get; set; }
        public string Materiāls { get; set; }
        public double Tilpums { get; set; }
        public double Stiegrojums { get; set; }

        public void Print()
        {
            Console.WriteLine(this);
        }
        public override string ToString()
        {
            return $"{Nosaukums}, Marka: {Marka}, Count: {Count}";
        }

        public static CipConcreteElement CreateFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null");
            }
            var pt = assembly.GetMainPart() as Part;
            var element = new CipConcreteElement
            {
                Marka = new ElementPosition(assembly.GetStringProp("ASSEMBLY_POS")),
                Nosaukums = assembly.Name,
                Materiāls = CheckWall.CheckMaterial(pt),
                Tilpums = assembly.GetDoubleProp("VOLUME") / 1e9, // Convert to m³
                Stiegrojums = CheckWall.CheckRebarWeight(assembly),
            };
            return element;
        }

        public bool Equals(CipConcreteElement other)
        {
            return Nosaukums == other.Nosaukums &&
                Marka.Prefix == other.Marka.Prefix &&
                Marka.Number == other.Marka.Number;
        }
    }
}
