namespace IFC.App.Bom.Models
{
    public class SandwichWallLayer
    {
        public string Nosaukums { get; set; }
        public string Materiāls { get; set; }
        public double Biezums { get; set; }
        public double Augstums { get; set; } 
        public double Garums { get; set; }
        public double Tilpums { get; set; }
        public double Weight { get; set; }
        public double BrutoLaukums { get; set; }
        public double NetoLaukums { get; set; }
        public int Skaits { get; set; } = 1;
    }
}
