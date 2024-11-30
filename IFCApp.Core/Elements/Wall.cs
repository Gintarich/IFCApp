
using IFCApp.Core.Geometry;
using IFCApp.Core.Services;

namespace IFCApp.Core.Elements;

public class Wall
{
    public int TeklaIdentifier { get; set; }

    private List<Window> _windows = new List<Window>();
    private BBox _box;
    private Matrix4d _matrix;
    private Colider _colider = new Colider();

    public Wall(BBox box, Matrix4d cs)
    {
        _box = box;
        _matrix = cs;
    }
    public Wall(BBox box):this(box,new Matrix4d()) { }

    public Wall TryToAddWindow(Window window)
    {
        if (_colider.Colides(this, window))
        {
            window.FatherID = TeklaIdentifier;
            _windows.Add(window);
        }
        return this;
    }

    public List<Window> GetWindows()
    {
        return _windows;
    }
    public BBox GetBox()
    {
        return _box;
    }
}