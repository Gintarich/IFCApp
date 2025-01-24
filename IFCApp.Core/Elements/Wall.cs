
using IFCApp.Core.Geometry;
using IFCApp.Core.Services;
using System.Text.Json.Serialization;

namespace IFCApp.Core.Elements;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SandwichPanel), typeDiscriminator: "SandwichPanel")]
[JsonDerivedType(typeof(WallPanel),     typeDiscriminator: "WallPanel")]
public class Wall : ElementBase
{
    public int TeklaIdentifier { get; set; }
    public bool ShouldHaveOpening { get; set; }
    public List<Opening> Openings
    {
        get { return _openings; }
        set { _openings = value; }
    }
    public BBox Box
    {
        get { return _box; }
        set { _box = value; }
    }
    public Matrix4d Matrix
    {
        get { return _matrix; }
        set { _matrix = value; }
    }

    private BBox _box;
    private Matrix4d _matrix;
    private List<Opening> _openings = new List<Opening>();
    private Colider _colider = new Colider();

    public Wall(BBox box, Matrix4d cs)
    {
        _box = box;
        _matrix = cs;
    }
    public Wall(BBox box):this(box,new Matrix4d()) { }

    public Wall TryToAddOpening(Opening opening)
    {
        if (!ShouldHaveOpening) return this;
        bool IsParallel = _box.IsParallel(opening.GetBox());
        bool colides = _colider.Colides(this, opening);
        if (colides && IsParallel)
        {
            var overlap = _box.OverlapVolume(opening.GetBox());
            if (overlap<1000) { return this; } // If overlap is small then dont add opening
            opening.FatherID = TeklaIdentifier;
            _openings.Add(opening);
        }
        return this;
    }

    public List<Window> GetWindows()
    {
        return _openings.Where(x=>x is Window).Cast<Window>().ToList();
    }
    public List<Door> GetDoors()
    {
        return _openings.Where(x=>x is Door).Cast<Door>().ToList();
    }
    public BBox GetBox()
    {
        return _box;
    }

}