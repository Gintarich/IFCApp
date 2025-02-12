using IFCApp.Core.Geometry;
using IFCApp.TeklaServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace IFCApp.Tests.TeklaTests
{
    [TestClass]
    public class ParameterCreatorTests
    {
        [TestMethod]
        public void ShouldSetParameters()
        {
            BBox box = new BBox([new Point3d(79150,22600,-400.00), new Point3d(44300,6000,8100)]);
            var creator = new ParameterCreator();
            creator.CreateParameter("Finish", "XC1", "NESOŠAIS SLĀNIS" );
            creator.CreateParameter("Finish", "XC1", "VIENSLĀŅU SIENAS PANELIS" );
            creator.CreateParameter( "Finish", "XC3", "VIENSLĀŅU SIENAS PANELIS", box);
            creator.CreateParameter("Finish", "XC3", "NESOŠAIS SLĀNIS", box);
            creator.CreateParameter("Finish", "XC4, XF1", "APDARES SLĀNIS" );
            new Model().CommitChanges();
        }
    }
}
