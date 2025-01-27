using IFCApp.Core.Geometry;
using IFCApp.UI.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IFCApp.UI.ViewModel.Modals
{
    public class BoxModalVM : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        public ObservableCollection<BoxItemVM> BoxItems { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }
        public BoxModalVM(MainViewModel mainViewModel)
        {
            BoxItems = new ObservableCollection<BoxItemVM>()
            {
                new BoxItemVM("Box 1", new Point3d(0, 0, 0), new Point3d(1, 1, 1)),
                new BoxItemVM("Box 2", new Point3d(1, 1, 1), new Point3d(2, 2, 2)),
                new BoxItemVM("Box 3", new Point3d(2, 2, 2), new Point3d(3, 3, 3)),
            };
            _mainViewModel = mainViewModel;
            RemoveItemCommand = new RelayCommand<object>(RemoveItem);
            CloseModalCommand = new RelayCommand(CloseModal);
        }
        private void CloseModal()
        {
            _mainViewModel.IsOpen = false;
        }
        private void RemoveItem(object parameter)
        {
            if (parameter is BoxItemVM item && BoxItems.Contains(item))
            {
                BoxItems.Remove(item);
            }
        }
    }
}
