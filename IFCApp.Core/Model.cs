using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core
{
    public class Model
    {
        public string ModelName { get; set; }
        public Matrix4d CS { get; set; }

        public List<ElementBase> Elements = new();
        public Model() { }
        public void Insert(ElementBase element)
        {
            Elements.Add(element);
        }
        public void Insert(IEnumerable<ElementBase> element)
        {
            Elements.AddRange(element);
        }
    }
}
