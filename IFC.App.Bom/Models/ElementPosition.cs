using System;

namespace IFC.App.Bom.Models
{
    public class ElementPosition
    {
        public string Prefix { get; set; }
        public int Number { get; set; }
        public bool NeedsModification { get; set; }

        public ElementPosition(string prefix, int elementNumber)
        {
            Prefix = prefix;
            Number = elementNumber;
        }
        public ElementPosition(string position)
        {
            if(string.IsNullOrWhiteSpace(position))
            {
                throw new ArgumentException("Position cannot be null or empty", nameof(position));
            }
            if(position.Contains("(?)"))
            {
                position = position.Replace("(?)", string.Empty).Trim();
                NeedsModification = true;
            }
            (var prefix,var nr) = ValidateAndSplitPostition(position);
            if ( !int.TryParse(nr, out int number))
            {
                Prefix = position;
                Number = 0;
                Console.WriteLine("Position must be in the format 'Prefix/Number'");
                Console.WriteLine($"Position is {position} for element");
                // throw new FormatException("Position must be in the format 'Prefix/Number'");
            }
            else
            {
                Prefix = prefix;
                Number = number;
            }
        }

        private (string, string) ValidateAndSplitPostition(string str)
        {
            var tempStr = str;
            var nrString = "";
            var lastChar = tempStr[^1];
            while(char.IsDigit(lastChar))
            {
                nrString = lastChar + nrString;
                tempStr = tempStr![..^1];
                if (tempStr.Length == 0) break;
                lastChar = tempStr[^1];
            }
            return (tempStr![..^1], nrString);
        }

        public override string ToString()
        {
            return $"{Prefix}/{Number}";
        }
    }
}
