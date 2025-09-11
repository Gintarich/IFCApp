namespace IFC.App.Bom.Models
{
    public static class CheckSteel
    {
        public static string CheckMaterial(string material)
        {
            if (string.IsNullOrEmpty(material)) return "NEZINĀMS MATERIĀLS";
            if (material == "Steel_Undefined") material = "SKATĪT RAŽ. NOR.";
            return material;
        }
    }
}
