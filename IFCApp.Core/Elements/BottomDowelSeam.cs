
using IFCApp.Core.Geometry;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Elements;

public class BottomDowelSeam
{
    public List<Guid> RelatingElements { get; set; } = new List<Guid>();
    public List<Point3d> Points { get; set; } = new List<Point3d>();
    public BottomDowelSeam() { }
}
