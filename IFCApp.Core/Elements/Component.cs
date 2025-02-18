using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements
{
    public abstract class Component : ElementBase
    {
        public Guid FirstPart { get; set; }
        public Guid SecondPart { get; set; }
        public abstract void Run(Model model,Action<List<Point3d>, Guid, Guid> func);
    }
}
