using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tekla.Structures.Model.UI;

namespace IFCApp.Tests.TeklaTests;
[TestClass]
public class WallMakerTests
{
    [TestMethod]
    public void MustGetWalls()
    {
        //Picker picker = new Picker();
        //Tekla.Structures.Model.ModelObject obj = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT);
        TeklaWallService wallMaker = new TeklaWallService();
        var wall = wallMaker.GetWall(1056597);
        TeklaGraphicsDrawerService gd = new TeklaGraphicsDrawerService();
        gd.DrawBox(wall.GetBox());
    }
}
