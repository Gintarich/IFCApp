
using IFCApp.Core.Geometry;

namespace IFCApp.Core.Elements;

public class Wall
{
    private List<Window> _windows = new List<Window>();
    private BBox _box;
    private Matrix4d _matrix;

    public Wall(BBox box, Matrix4d cs)
    {
        _box = box;
        _matrix = cs;
    }

    public Wall AddWindow(Window window)
    {
        _windows.Add(window);
        return this;
    }

    public List<Window> GetWindows()
    {
        return _windows;
    }
}