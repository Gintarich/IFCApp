using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;

namespace IFC.App.Bom.Models
{
    public class PrecastHcsElement : IElement, IEquatable<PrecastHcsElement>
    {
        public ElementPosition Marka { get; set; }
        public string Nosaukums { get; set; }
        public int Skaits { get; set; }
        public string Materiāls { get; set; }
        public double Biezums { get; set; }
        public double Augstums { get; set; }
        public double Garums { get; set; }
        public double Tilpums { get; set; }
        public double Svars { get; set; }
        public double BrutoLaukums { get; set; }
        public double NetoLaukums { get; set; }
        public double TilpumsKopā { get { return Tilpums * Skaits; } }
        public double BrutoLaukumsKopā { get { return BrutoLaukums * Skaits; } }
        public double NetoLaukumsKopā { get { return NetoLaukums * Skaits; } }

        public void Print()
        {
            Console.WriteLine(this);
        }

        public override string ToString()
        {
            return $"{Nosaukums}, Marka: {Marka}, Tilpums: {Math.Round(Tilpums, 3)}, Count: {Skaits}";
        }

        public static PrecastHcsElement CreateFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null");
            }
            var pt = assembly.GetMainPart() as Part;

            var element = new PrecastHcsElement
            {
                Marka = new ElementPosition(assembly.GetStringProp("ASSEMBLY_POS")),
                Nosaukums = assembly.Name,
                Materiāls = pt is null ? "N/A" : CheckWall.CheckMaterial(pt),
                Biezums =  assembly.GetDoubleProp("WIDTH"),
                Augstums = assembly.GetDoubleProp("HEIGHT"), 
                Garums = assembly.GetDoubleProp("LENGTH"),
                Tilpums = assembly.GetDoubleProp("VOLUME")/1e9,
                Svars = assembly.GetDoubleProp("WEIGHT")/1e3,
                BrutoLaukums = assembly.GetDoubleProp("AREA_PROJECTION_XZ_GROSS")/1e6,
                NetoLaukums = assembly.GetDoubleProp("AREA_PROJECTION_XZ_NET") / 1e6
            };
            return element;
        }

        public bool Equals(PrecastHcsElement other)
        {
            return Nosaukums == other.Nosaukums &&
                Marka.Prefix == other.Marka.Prefix &&
                Marka.Number == other.Marka.Number;
        }
    }
}
