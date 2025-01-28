using IFCApp.Core.Geometry;
using IFCApp.TeklaServices.Utils;
using IFCApp.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IFCApp.UI.ViewModel.Modals;

public class BoxItemVM
{
    public string Name { get; set; } = "";
    public string MinPt { get; set; }
    public string MaxPt { get; set; }
    private BBox _bBox;
    private Point3d _p1;
    private Point3d _p2;
    public ICommand ShowBoxCommand { get; set; }

    public BoxItemVM(string name, Point3d p1, Point3d p2)
    {
        _p1 = p1;
        _p2 = p2;
        Name = name;
        BBox b = new BBox([p1, p2]);
        MinPt = $"({b.Min.X}, {b.Min.Y}, {b.Min.Z})";
        MaxPt = $"({b.Max.X}, {b.Max.Y}, {b.Max.Z})";
        _bBox = b;
        ShowBoxCommand = new RelayCommand(ShowBox);
    }

    private void ShowBox()
    {
        TeklaGraphicsDrawerService dr = new();
        dr.DrawBox(_bBox);
    }
    public List<Point3d> GetPoints() 
    {
        return [_p1, _p2];
    }
}
