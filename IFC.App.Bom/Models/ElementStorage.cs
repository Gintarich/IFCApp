namespace IFC.App.Bom.Models
{
    public class ElementStorage<T>  where T : IElement, IEquatable<T>
    {
        private List<T> _elements = [];

        public void Add(T element)
        {
            if (_elements.Contains(element))
            {
                _elements.FirstOrDefault(e=>e.Equals(element)).Count++;
            }
            else
            {
                element.Count = 1;
                _elements.Add(element);
            }
        }
        public List<T> GetElements()
        {
            return _elements;
        }

        public void SortByMark()
        {
            _elements = _elements.OrderBy(e => e.Marka.Prefix).ThenBy(e => e.Marka.Number).ToList();
        }
        public void SortByName()
        {
            _elements = _elements.OrderBy(e => e.Nosaukums).ToList();
        }
        public bool IsEmpty()
        {
            return _elements.Count == 0;
        }
        public int GetCount()
        {
            return _elements.Count;
        }
    }
}
