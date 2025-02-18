using IFCApp.Core;
using IFCApp.Core.DetailComponents;
using IFCApp.Core.Elements;
using IFCApp.Core.Services;
using IFCApp.TeklaServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model.UI;
using TSM = Tekla.Structures.Model;

namespace IFCApp.Tests.TeklaTests
{
    [TestClass]
    public class WallSeamTests
    {

        [TestMethod]
        public void MustCreateSeams()
        {
            TSM.Model tModel = new();
            var pic = new Picker();
            //var part1 = pic.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick slab") as TSM.Part;
            //var part2 = pic.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick wall") as TSM.Part;
            //var guid1 = part1 is not null ? part1.Identifier.GUID : new Guid();
            //var guid2 = part2 is not null ? part2.Identifier.GUID : new Guid();
            //var guid1 =new Guid("084bcf98-070a-40e4-b250-f43752407b59");
            //var guid2 =new Guid("24c581da-1606-484a-8323-349f09f182ce");
            var guid1 = new Guid("084bcf98-070a-40e4-b250-f43752407b59");
            var guid2 = new Guid("3d02e2ba-4eae-4b60-bf7d-da5812921c52");
            JsonModelSerializationService jSer = new JsonModelSerializationService(@"Z:\BCD projekti\Eduards Beernaerds_7AM\VUGD Depo kuldiga\Teklas modeli\KUL-7AM-00-00-M3-BK-0001\Automation\KUL-7AM-00-00-M3-BK-0001.json");
            Model model = jSer.Read();
            TeklaBoundingBoxService bbService = new TeklaBoundingBoxService();
            TeklaSlabService sService = new TeklaSlabService(bbService);
            TeklaDowelService dService = new TeklaDowelService();
            DowelComponent dc = new DowelComponent(guid1, guid2 );
            dc.Run(model,dService.GenerateDowels);
        }

        [TestMethod]
        public void MustCreateAllSeams()
        {
            TSM.Model tModel = new();
            var guid1 = new Guid("084bcf98-070a-40e4-b250-f43752407b59");
            JsonModelSerializationService jSer = new JsonModelSerializationService(@"Z:\BCD projekti\Eduards Beernaerds_7AM\VUGD Depo kuldiga\Teklas modeli\KUL-7AM-00-00-M3-BK-0001\Automation\KUL-7AM-00-00-M3-BK-0001.json");
            Model model = jSer.Read();
            var walls = model.Elements.Where(x => x is Wall).Cast<Wall>().ToList();
            TeklaBoundingBoxService bbService = new TeklaBoundingBoxService();
            TeklaSlabService sService = new TeklaSlabService(bbService);
            TeklaDowelService dService = new TeklaDowelService();
            Model modelOut = new Model();
            foreach (var wall in walls)
            {
                Guid guid2 = wall.ID;
                DowelComponent dc = new DowelComponent(guid1, guid2);
                modelOut.Insert(dc);
                dc.Run(model,dService.GenerateDowels);
            }
            JsonModelSerializationService jser2 = new(@"C:\Users\User\Documents\Test123.json");
            jser2.Write(modelOut);
            var mod = jser2.Read();
            var lst = mod.Elements.Where(x => x is Component).ToList();
        }

        [TestMethod]
        public void MustMakeDowelComponents()
        {
            Dictionary<Slab, List<Wall>> firstFloor = [];
            Dictionary<Wall, List<Wall>> secondFloor = [];

            JsonModelSerializationService jSer = new JsonModelSerializationService("KUL-7AM-00-00-M3-BK-0001.json");
            Model model = jSer.Read();
            var walls = model.Elements.Where(x=>x is Wall).Cast<Wall>().ToList();
            var ffWalls = walls.Where(w=>w.Box.GetMin().Z <30).ToList();
            var slabs = model.Elements.Where(x=>x is Slab).Cast<Slab>().ToList();
            foreach (var slab in slabs)
            {
                
            }
        }
    }
}
