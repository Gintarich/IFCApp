using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.UI.Models;

public class ParametersItemModel
{
    public string BoxName { get; set; }
    public string ParameterName { get; set; }
    public string ParameterValue { get; set; }
    public string PartName { get; set; }
    public ParametersItemModel() { }
}
