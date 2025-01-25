using IFCApp.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IFCApp.UI.ViewModel.Modals
{
    public class StartAppModalVM : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        public ICommand CloseModalCommand { get; set; }
        public StartAppModalVM(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            CloseModalCommand = new RelayCommand(CloseModal);
        }
        private void CloseModal()
        {
            _mainViewModel.IsOpen = false;
        }
    }
}
