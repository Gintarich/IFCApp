using IFCApp.Core;
using IFCApp.Core.DetailComponents;
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
            var part1 = pic.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick slab") as TSM.Part;
            var part2 = pic.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick wall") as TSM.Part;
            var guid1 = part1 is not null ? part1.Identifier.GUID : new Guid();
            var guid2 = part2 is not null ? part2.Identifier.GUID : new Guid();

            JsonSerializationService jSer = new JsonSerializationService(@"Z:\BCD projekti\Eduards Beernaerds_7AM\VUGD Depo kuldiga\Teklas modeli\KUL-7AM-00-00-M3-BK-0001\Automation\KUL-7AM-00-00-M3-BK-0001.json");
            Model model = jSer.Read();

            TeklaBoundingBoxService bbService = new TeklaBoundingBoxService();
            TeklaSlabService sService = new TeklaSlabService(bbService);
            TeklaDowelService dService = new TeklaDowelService();
            DowelComponent dc = new DowelComponent(guid1,guid2,dService.GenerateDowels);
            dc.Run(model);
        }
    }
}
