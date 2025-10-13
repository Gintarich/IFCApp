namespace IFC.App.Bom.Models
{
    public interface IElement
    {
        public string Nosaukums { get; set; }
        public ElementPosition Marka { get; set; }
        public int Skaits { get; set; }
    }
}
