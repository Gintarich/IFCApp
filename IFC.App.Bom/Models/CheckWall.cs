using Tekla.Structures.Model;
using IFCApp.TeklaServices.Utils;

namespace IFC.App.Bom.Models
{
    public static class CheckWall
    {
        private static readonly List<string> _names = new List<string>
            {
                "NESOŠAIS SLĀNIS",
                "SILTUMIZOLĀCIJA",
                "APDARES SLĀNIS",
                "APDARES ĶIEĢELIS",
                "IZOLĀCIJA"
            };
        private static readonly List<string> _englishNames = new List<string>
            {
                "INSIDESHELL",
                "INSULATION",
                "OUTSIDESHELL"
            };
        private static readonly Dictionary<string,string> Translate = new Dictionary<string, string>
        {
            {"INSIDESHELL", "NESOŠAIS SLĀNIS"},
            {"INSULATION", "SILTUMIZOLĀCIJA"},
            {"OUTSIDESHELL", "APDARES SLĀNIS"}
        };

        public static (bool IsValid ,string Name) CheckName(Part mo)
        {
            var name = mo.GetStringProp("NAME");
            if (!(_names.Contains(name) || _englishNames.Contains(name))) return (false,name);
            if (_englishNames.Contains(name))
            {
                return (true, Translate[name]);
            }

            return (true, name);
        }
        public static string CheckMaterial(Part mo)
        {
            var material = mo.GetStringProp("MATERIAL");
            var finish = mo.GetStringProp("FINISH");
            var comment = mo.GetStringProp("comment");
            if (material == "Insulation_hard") material = "SKATĪT AR";
            if (string.IsNullOrEmpty(finish) && string.IsNullOrEmpty(comment)) return material;
            if (string.IsNullOrEmpty(finish)) return $"{material} {comment}";
            if (string.IsNullOrEmpty(comment)) return $"{material} {finish}";
            return $"{material} {finish} {comment}";
        }
        public static double CheckRebarWeight(Assembly ass)
        {
            var rebarWeight = ass.GetDoubleProp("CAST_UNIT_REBAR_WEIGHT");
            var volume = ass.GetDoubleProp("VOLUME") / 1e9;
            var mpMaterial = ass.GetMainPart() as Part;
            var isFibConcrete = mpMaterial.Material.MaterialString.Contains("FIBROBETONS");
            if (isFibConcrete) return 0;
            var rebarWeightIndex = rebarWeight/volume;
            if (rebarWeightIndex > 50) return rebarWeight;
            else return volume * 120;
        }
    }
}
