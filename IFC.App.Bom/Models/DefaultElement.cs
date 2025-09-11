namespace IFC.App.Bom.Models
{
    public class DefaultElement : IElement, IEquatable<DefaultElement>
    {
        public string Nosaukums { get; set; }
        public ElementPosition Marka { get; set; }
        public double Tilpums { get; set; }
        public int Count { get; set; }


        public bool Equals(DefaultElement other)
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
    }
}
