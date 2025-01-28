using IFCApp.Core.Geometry;
using IFCApp.TeklaServices.Utils;
using IFCApp.UI.Core;
using IFCApp.UI.Stores;
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
        private readonly ModelStore _modelStore;

        public ObservableCollection<BoxItemVM> BoxItems { get; set; } = [];
        public ICommand RemoveItemCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }
        public ICommand AddBoxCommand { get; set; }

        private string _boxName;

        public string BoxName
        {
            get { return _boxName; }
            set { _boxName = value; OnPropertyChanged(nameof(BoxName)); }
        }


        public BoxModalVM(MainViewModel mainViewModel, Stores.ModelStore modelStore)
        {
            var boxes = modelStore.GetBoxes();
            foreach (var box in boxes)
            {
                BoxItems.Add(new BoxItemVM(box.Key, box.Value.Min, box.Value.Max));
            }
            _mainViewModel = mainViewModel;
            _modelStore = modelStore;
            RemoveItemCommand = new RelayCommand<object>(RemoveItem);
            CloseModalCommand = new RelayCommand(CloseModal);
            AddBoxCommand = new RelayCommand(AddItem);
        }
        private void CloseModal()
        {
            _modelStore.SetBoxes(BoxItems.ToDictionary(x => x.Name, x => new BBox(x.GetPoints())));
            _mainViewModel.IsOpen = false;
        }
        private void RemoveItem(object parameter)
        {
            if (parameter is BoxItemVM item && BoxItems.Contains(item))
            {
                BoxItems.Remove(item);
            }
        }
        private void AddItem()
        {
            TeklaInteraction teklaInteraction = new TeklaInteraction();
            var pt1 = teklaInteraction.PickPoint();
            var pt2 = teklaInteraction.PickPoint();
            var box = new BBox([pt1, pt2]);
            BoxItems.Add(new BoxItemVM(BoxName, pt1, pt2));
        }
    }
}
