
using IFCApp.Core.Geometry;
using IFCApp.Core.Services;
using System.Text.Json.Serialization;

namespace IFCApp.Core.Elements;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Wall), typeDiscriminator: "Wall")]
[JsonDerivedType(typeof(SandwichPanel), typeDiscriminator: "SandwichPanel")]
[JsonDerivedType(typeof(WallPanel), typeDiscriminator: "WallPanel")]
public class Wall : ElementBase
{
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

    public Wall()
    {
        _box = new BBox();
        _matrix = new Matrix4d();
    }
    public Wall(BBox box, Matrix4d cs)
    {
        _box = box;
        _matrix = cs;
    }
    public Wall(BBox box) : this(box, new Matrix4d()) { }

    public Wall TryToAddOpening(Opening opening)
    {
        if (!ShouldHaveOpening) return this;

        var matchingOpeningList = GetOpenings().Where(x => x.ID == opening.ID);
        bool IsParallel = _box.IsParallel(opening.GetBox());
        bool colides = _colider.Colides(this, opening);

        if (matchingOpeningList.Count() > 0 && (IsParallel && colides))
        {
            var overlap = _box.OverlapVolume(opening.GetBox());
            if (overlap < 0.001) { return this; } // If overlap is small then dont add opening
            var matchingOpening = matchingOpeningList.FirstOrDefault();
            matchingOpening.Box = opening.Box;
            matchingOpening.FatherID = this.ID;
            matchingOpening.UserData = opening.UserData;
            return this;
        }

        if (colides && IsParallel)
        {
            var overlap = _box.OverlapVolume(opening.GetBox());
            if (overlap < 0.001) { return this; } // If overlap is small then dont add opening
            opening.FatherID = this.ID;
            GetOpenings().Add(opening);
        }
        return this;
    }

    public List<Window> GetWindows()
    {
        return GetOpenings().Where(x => x is Window).Cast<Window>().ToList();
    }
    public List<Door> GetDoors()
    {
        return GetOpenings().Where(x => x is Door).Cast<Door>().ToList();
    }

    public BBox GetBox()
    {
        return _box;
    }

    public List<Opening> GetOpenings() { return _openings; }

    public List<Domain> GetLowerDomains(double threshold)
    { 
        var domains = new List<Domain>();
        var openingBoxes = GetOpenings().Select(x=>x.Box.ToOtherCS(Box.CS))
            .Where(x=>x.Min.Z<threshold).ToList();
        if (!openingBoxes.Any())
        {
            double left = Box.Min.X;
            double right = Box.Max.X;
            domains.Add(new Domain(left, right));
            return domains;
        }
        openingBoxes = openingBoxes.OrderBy(x => x.Min.X).ToList();
        double lhs = Box.Min.X;
        for (int i = 0; i < openingBoxes.Count; i++)
        {
            double rhs = openingBoxes[i].Min.X;
            domains.Add(new Domain(lhs, rhs));
            lhs = openingBoxes[i].Max.X;
        }
        domains.Add(new Domain(lhs, Box.Max.X));
        return domains;
    }

}