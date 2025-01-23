using IFCApp.TeklaServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace IFCApp.Tests.TeklaTests;

[TestClass]
public class GetPropsTests
{
    [TestMethod]
    public void ShouldGetProp()
    {
        var picker = new Picker();  

        var ass = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT) as Assembly;
        int number = -1;
        ass.GetUserProperty("ShouldHaveOpening", ref number);
        string text = string.Empty;
        ass.GetUserProperty("ShouldHaveOpening", ref text);
        double floating = -1;
        ass.GetUserProperty("ShouldHaveOpening", ref floating);
    }
    [TestMethod]
    public void ShouldGetName()
    {
        var name = ModelAttributeServer.GetModelName();
        var path = ModelAttributeServer.GetFilePath();
    }
}
