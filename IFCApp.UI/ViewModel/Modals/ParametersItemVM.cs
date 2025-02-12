using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.UI.ViewModel.Modals
{
    public class ParametersItemVM : ViewModelBase
    {
        private string _boxName;
        public string BoxName
        {
            get { return _boxName; }
            set { _boxName = value; OnPropertyChanged(nameof(BoxName)); }
        }

        private string _parameterName;
        public string ParameterName
        {
            get { return _parameterName; }
            set { _parameterName = value; OnPropertyChanged(nameof(ParameterName)); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(nameof(Value)); }
        }

        private string _partName;
        public string PartName
        {
            get { return _partName; }
            set { _partName = value; OnPropertyChanged(nameof(PartName)); }
        }


        public ParametersItemVM(string parameterName, string value, string partName="", string boxName = "")
        {
            Value = value;
            BoxName = boxName;
            ParameterName = parameterName;
            PartName = partName;
        }
    }
}
