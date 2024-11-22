using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFCApp.IFCServices;

namespace IFCApp.Tests.IFCTests;
[TestClass]
public class IfcConnection
{
    [TestMethod]
    public void MustGetWindows()
    {
        IfcTester tester = new IfcTester();
        tester.GetWalls();
    }
}
