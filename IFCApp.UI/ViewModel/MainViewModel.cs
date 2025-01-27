using IFCApp.UI.Stores;
using IFCApp.UI.ViewModel.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.UI.ViewModel;
public class MainViewModel : ViewModelBase
{
    private ViewModelBase _selectedViewModel;
    public ViewModelBase SelectedViewModel
    {
        get { return _selectedViewModel; }
        set { _selectedViewModel = value; OnPropertyChanged(nameof(SelectedViewModel)); }
    }

    private ViewModelBase _selectedModal;
    public ViewModelBase SelectedModal
    {
        get { return _selectedModal; }
        set { _selectedModal = value; OnPropertyChanged(nameof(SelectedModal)); }
    }

    private bool _isOpen;
    public bool IsOpen
    {
        get { return _isOpen; }
        set { _isOpen = value; OnPropertyChanged(nameof(IsOpen)); }
    }

    public MainViewModel(ModelStore modelStore)
    {
        _selectedViewModel = new ModelManagerVM(modelStore, this);
        _selectedModal = new StartAppModalVM(this,modelStore);
        IsOpen = true;
    }
}
