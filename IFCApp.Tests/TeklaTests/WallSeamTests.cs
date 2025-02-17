using IFCApp.Core.DetailComponents;
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
            TSM.Model model = new();
            var pic = new Picker();
            DowelComponent dc = new DowelComponent();
        }
    }
}
