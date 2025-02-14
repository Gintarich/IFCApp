using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core
{
    public class Model
    {
        public string ModelName { get; set; } = "";
        public string ModelPath { get; set; } = "";
        public Dictionary<string, BBox> BBoxes { get; set; } = new();
        public Matrix4d CS { get; set; }

        public List<ElementBase> Elements { get; set; } = new();
        public Dictionary<Guid, int> ElementMap { get; set; } = new();
        public Model() { }

        public void Insert(ElementBase element)
        {
            ElementMap[element.ID] = Elements.Count;
            Elements.Add(element);

        }
        public void Insert(IEnumerable<ElementBase> elements)
        {
            foreach (var el in elements)
            {
                Insert(el);
            }
        }
        public void Remove(ElementBase element)
        {
            var idx = ElementMap[element.ID];
            Elements[idx] = null;
        }
        public void CleanModel()
        {
            for (var i = 0; i < Elements.Count; i++)
            {
                if (Elements[i] == null)
                {
                    Elements.RemoveAt(i);
                }
            }
            for (var i = 0; i < Elements.Count; i++)
            {
                Dictionary<Guid, int> newMap = new Dictionary<Guid, int>();
                newMap[Elements[i].ID] = i;
                ElementMap = newMap;
            }
        }
        public bool TryGetValue(Guid id, out ElementBase value)
        {
            if (ElementMap.TryGetValue(id, out int index))
            {
                value = Elements[index];
                return true;
            }
            value = default;
            return false;
        }
    }
}
