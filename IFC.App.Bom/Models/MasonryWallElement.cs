using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace IFC.App.Bom.Models
{
    public class MasonryWallElement : IElement, IEquatable<MasonryWallElement>
    {
        public ElementPosition Marka { get; set; }
        public string Nosaukums { get; set; }
        public int Skaits { get; set; }
        public string Materiāls { get; set; }
        public double Biezums { get; set; }
        public double Tilpums { get; set; }
        public double TilpumsKopā { get { return Tilpums * Skaits; } }

        public void Print()
        {
            Console.WriteLine(this);
        }

        public override string ToString()
        {
            return $"{Nosaukums}, Marka: {Marka}, Tilpums: {Math.Round(Tilpums, 3)}, Count: {Skaits}";
        }

        public static MasonryWallElement CreateFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null");
            }
            var pt = assembly.GetMainPart() as Part;

            var element = new MasonryWallElement
            {
                Marka = new ElementPosition(assembly.GetStringProp("ASSEMBLY_POS")),
                Nosaukums = assembly.Name,
                Materiāls = pt is null ? "N/A" : CheckWall.CheckMaterial(pt),
                Biezums = assembly.GetDoubleProp("WIDTH"),
                Tilpums = assembly.GetDoubleProp("VOLUME") / 1e9,
            };
            return element;
        }

        public bool Equals(MasonryWallElement other)
        {
            return Nosaukums == other.Nosaukums &&
            Marka.Prefix == other.Marka.Prefix &&
            Marka.Number == other.Marka.Number;
        }
    }
}
