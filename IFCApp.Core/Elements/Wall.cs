
using IFCApp.Core.Geometry;
using IFCApp.Core.Services;

namespace IFCApp.Core.Elements;

public class Wall
{
    public int TeklaIdentifier { get; set; }

    private List<Opening> _openings = new List<Opening>();
    private BBox _box;
    private Matrix4d _matrix;
    private Colider _colider = new Colider();

    public Wall(BBox box, Matrix4d cs)
    {
        _box = box;
        _matrix = cs;
    }
    public Wall(BBox box):this(box,new Matrix4d()) { }

    public Wall TryToAddOpening(Opening opening)
    {
        bool IsParallel = _box.IsParallel(opening.GetBox());
        bool colides = _colider.Colides(this, opening);
        if (colides && IsParallel)
        {
            opening.FatherID = TeklaIdentifier;
            _openings.Add(opening);
        }
        return this;
    }

    public List<Opening> GetWindows()
    {
        return _openings;
    }
    public BBox GetBox()
    {
        return _box;
    }
}