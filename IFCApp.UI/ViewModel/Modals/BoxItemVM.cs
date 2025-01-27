using IFCApp.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.UI.ViewModel.Modals
{
    public class BoxItemVM
    {
        public string Name { get; set; }
        public string MinPt { get; set; }
        public string MaxPt { get; set; }
        public BoxItemVM(string name, Point3d p1, Point3d p2)
        {
            Name = name;
            BBox b = new BBox([p1, p2]);
            MinPt = $"({b.Min.X}, {b.Min.Y}, {b.Min.Z})";
            MaxPt = $"({b.Max.X}, {b.Max.Y}, {b.Max.Z})";
        }
    }
}
