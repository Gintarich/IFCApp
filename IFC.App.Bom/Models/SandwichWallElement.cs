using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;
using System.Runtime.InteropServices;

namespace IFC.App.Bom.Models
{
    public class SandwichWallElement : IElement, IEquatable<SandwichWallElement>
    {
        public List<SandwichWallLayer> Layers { get; set; }
        public ElementPosition Marka { get; set; }
        public string Nosaukums { get; set; }
        public int Skaits { get; set; }
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
            return $"{Nosaukums}, Marka: {Marka}, Tilpums: {Math.Round(Tilpums, 3)}, Count: {Skaits}";
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
                var checkedName = CheckWall.CheckName(secPart);
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
                        Tilpums = secPart.GetDoubleProp("VOLUME") / 1e9,
                        Weight = secPart.GetDoubleProp("WEIGHT") / 1e3,
                        BrutoLaukums = secPart.GetDoubleProp("AREA_PROJECTION_XY_GROSS") / 1e6,
                        NetoLaukums = secPart.GetDoubleProp("AREA_PROJECTION_XY_NET") / 1e6
                    };
                    //if (layer.Nosaukums == "NESOŠAIS SLĀNIS" && layer.Tilpums < 0.045) continue;
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
                Tilpums = assembly.GetDoubleProp("VOLUME") / 1e9, // Set default or calculate based on assembly properties
                Svars = assembly.GetDoubleProp("WEIGHT") / 1e3, // Set default or calculate based on assembly properties
                BrutoLaukums = assembly.GetDoubleProp("AREA_PROJECTION_XY_GROSS") / 1e6, // Set default or calculate based on assembly properties
                NetoLaukums = assembly.GetDoubleProp("AREA_PROJECTION_XY_NET") / 1e6 // Set default or calculate based on assembly properties
            };
            return element;
        }
        public void MergeEqualLayers()
        {
            var openingInsulation = Layers.Where(l => l.Nosaukums == "IZOLĀCIJA").ToList();
            var filteredIns = openingInsulation.GroupBy(l => new
            {
                Nosaukums = l.Nosaukums,
                Biezums = Math.Round(l.Biezums, 2),
                Augstums = Math.Round(l.Augstums, 2),
                Garums = Math.Round(l.Garums, 2)
            }).Select(g =>
            {
                var el = g.First();
                el.Skaits = g.Count();
                return el;
            })
            .ToList();

            var byName =
                Layers
                .Where(l => l.Nosaukums != "IZOLĀCIJA")
                .GroupBy(l => l.Nosaukums ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    // choose the representative (largest volume in the group)
                    var rep = g
                        .OrderByDescending(x => x.Tilpums)   // tie-breakers optional below
                                                             //.ThenByDescending(x => x.NetoLaukums)
                                                             //.ThenBy(x => x.Materiāls)
                        .First();
                    var count = 1;
                    if (g.Key == "IZOLĀCIJA") { count = g.Count(); }

                    // build a result that keeps rep's other props,
                    // but sums Tilpums and Weight across the group
                    return new SandwichWallLayer
                    {
                        Nosaukums = rep.Nosaukums,
                        Materiāls = rep.Materiāls,
                        Biezums = rep.Biezums,
                        Augstums = rep.Augstums,
                        Garums = rep.Garums,
                        BrutoLaukums = rep.BrutoLaukums,
                        NetoLaukums = rep.NetoLaukums,
                        Skaits = count,
                        Tilpums = g.Sum(x => x.Tilpums), // total per name
                        Weight = g.Sum(x => x.Weight),  // total per name
                    };
                })
                //.OrderByDescending(x => x.Tilpums) // sort by total volume
                .ToList();
            byName.AddRange(filteredIns);
            this.Layers = byName;
        }
    }
}
