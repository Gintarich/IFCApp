using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Services;
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
        TeklaBoundingBoxService teklaBoundingBoxService = new TeklaBoundingBoxService();
        TeklaWallService wallMaker = new TeklaWallService(teklaBoundingBoxService);
        var wall = wallMaker.GetWalls(["SIENAS PANELIS"]);
        TeklaGraphicsDrawerService gd = new TeklaGraphicsDrawerService();
        //gd.DrawBox(wall.GetBox());
    }
}
