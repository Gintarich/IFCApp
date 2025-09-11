using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;

namespace IFC.App.Bom.Models
{
    public class SandwichWallElement : IElement, IEquatable<SandwichWallElement>
    {
        public List<SandwichWallLayer> Layers { get; set; }
        public ElementPosition Marka { get; set; }
        public string Nosaukums { get; set; }
        public int Count { get; set; }
        public double Biezums { get; set; }
        public double Augstums { get; set; }
        public double Garums { get; set; }
        public double Tilpums { get; set; }
        public double Svars { get; set; }
        public double BrutoLaukums { get; set; }
        public double NetoLaukums { get; set; }
        public double TilpumsKopā { get { return Tilpums * Count; } }
        public double BrutoLaukumsKopā { get { return BrutoLaukums * Count; } }
        public double NetoLaukumsKopā { get { return NetoLaukums * Count; } }



        public bool Equals(SandwichWallElement other)
        {
            return Nosaukums == other.Nosaukums &&
                Marka.Prefix == other.Marka.Prefix &&
                Marka.Number == other.Marka.Number;
        }

        public void Print()
        {
            Console.WriteLine(this);
        }

        public override string ToString()
        {
            return $"{Nosaukums}, Marka: {Marka}, Tilpums: {Math.Round(Tilpums, 3)}, Count: {Count}";
        }

        public static SandwichWallElement CreateFromAssembly(Assembly assembly)
        {
            List<string> layerNames = new List<string>
            {
                "APDARES SLĀNIS",
                "SILTUMIZOLĀCIJA",
                "NESOŠAIS SLĀNIS",
            };

            List<SandwichWallLayer> layers = new List<SandwichWallLayer>();

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null");
            }

            //Create Layers based on assembly secondary parts : NESOŠAIS SLĀNIS, SILTUMIZOLĀCIJA AND APDARES SLĀNIS
            var secondaries = assembly.GetSecondaries();
            secondaries.Add(assembly.GetMainPart());
            foreach (var part in secondaries)
            {
                if (part is not Part secPart) continue;
                var checkedName= CheckWall.CheckName(secPart);
                var checkedMaterial = CheckWall.CheckMaterial(secPart);

                if (checkedName.IsValid)
                {
                    SandwichWallLayer layer = new SandwichWallLayer()
                    {
                        Nosaukums = checkedName.Name,
                        Materiāls = checkedMaterial,
                        Biezums = secPart.GetDoubleProp("WIDTH"),
                        Augstums = secPart.GetDoubleProp("HEIGHT"),
                        Garums = secPart.GetDoubleProp("LENGTH"),
                        Tilpums = secPart.GetDoubleProp("VOLUME")/1e9,
                        Weight = secPart.GetDoubleProp("WEIGHT")/1e3,
                        BrutoLaukums = secPart.GetDoubleProp("AREA_PROJECTION_XY_GROSS")/1e6,
                        NetoLaukums = secPart.GetDoubleProp("AREA_PROJECTION_XY_NET")/1e6
                    };
                    if (layer.Nosaukums == "NESOŠAIS SLĀNIS" && layer.Tilpums < 0.045) continue;
                    layers.Add(layer);
                }

                //sort layers in order : "NESOŠAIS SLĀNIS", "SILTUMZIOLĀCIJA", "APDARES SLĀNIS" 
                layers = layers.OrderBy(l => layerNames.IndexOf(l.Nosaukums)).ToList();
            }

            var element = new SandwichWallElement
            {
                Layers = layers,
                Marka = new ElementPosition(assembly.GetStringProp("ASSEMBLY_POS")),
                Nosaukums = assembly.Name,
                Biezums = assembly.GetDoubleProp("WIDTH"), // Set default or calculate based on assembly properties
                Augstums = assembly.GetDoubleProp("HEIGHT"), // Set default or calculate based on assembly properties
                Garums = assembly.GetDoubleProp("LENGTH"), // Set default or calculate based on assembly properties
                Tilpums = assembly.GetDoubleProp("VOLUME")/1e9, // Set default or calculate based on assembly properties
                Svars = assembly.GetDoubleProp("WEIGHT")/1e3, // Set default or calculate based on assembly properties
                BrutoLaukums = assembly.GetDoubleProp("AREA_PROJECTION_XY_GROSS")/1e6, // Set default or calculate based on assembly properties
                NetoLaukums = assembly.GetDoubleProp("AREA_PROJECTION_XY_NET") / 1e6 // Set default or calculate based on assembly properties
            };
            return element;
        }
    }
}
