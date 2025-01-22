using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.UI.ViewModel.Sections;

class SectionVMBase : ViewModelBase
{
    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }
    public SectionVMBase(string name)
    {
        _name = name;
        OnPropertyChanged(nameof(Name));
    }
}
