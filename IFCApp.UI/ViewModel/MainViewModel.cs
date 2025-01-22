using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.UI.ViewModel;
class MainViewModel : ViewModelBase
{
    private ViewModelBase _selectedViewModel;
    public ViewModelBase SelectedViewModel
    {
        get { return _selectedViewModel; }
        set
        {
            _selectedViewModel = value;
            OnPropertyChanged(nameof(SelectedViewModel));
        }
    }
}
