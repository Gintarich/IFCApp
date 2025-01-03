using IFCApp.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core
{
    public class Model
    {
        public List<ElementBase> Elements = new();
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
