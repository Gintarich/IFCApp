using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;

namespace IFC.App.Bom.Models
{
    public class SteelElement : IElement, IEquatable<SteelElement>
    {
        public ElementPosition Marka { get; set; }
        public string Nosaukums { get; set; }
        public int Skaits { get; set; }
        public string Materiāls { get; set; }
        public string Profils { get; set; }
        public double Garums { get; set; }
        public double Svars { get; set; }
        public double Laukums { get; set; }
        public string Piezīmes { get; set; }
        public double SvarsKopā { get { return Svars * Skaits; } }
        public double LaukumsKopā { get { return Laukums * Skaits; } }

        public void Print()
        {
            Console.WriteLine(this);
        }

        public override string ToString()
        {
            return $"{Nosaukums}, Marka: {Marka}, Profils: {Profils}, Count: {Skaits}";
        }

        public static SteelElement CreateFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null");
            }
            var pt = assembly.GetMainPart() as Part;

            var element = new SteelElement
            {
                Marka = new ElementPosition(assembly.GetStringProp("ASSEMBLY_POS")),
                Nosaukums = assembly.Name,
                Materiāls = CheckSteel.CheckMaterial(pt.GetStringProp("MATERIAL")),
                Profils = pt.GetStringProp("PROFILE"),
                Garums = assembly.GetDoubleProp("LENGTH"),
                Svars = assembly.GetDoubleProp("WEIGHT") / 1e3,
                Laukums = assembly.GetDoubleProp("AREA") / 1e6,
                Piezīmes = assembly.GetStringProp("comment")
            };
            return element;
        }
        public bool Equals(SteelElement other)
        {
            return Nosaukums == other.Nosaukums &&
                Marka.Prefix == other.Marka.Prefix &&
                Marka.Number == other.Marka.Number;
        }
    }
}
