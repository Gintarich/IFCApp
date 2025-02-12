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
    public class ParametersModalVM : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ModelStore _modelStore;
        private readonly ConfigStore _configStore;

        public ObservableCollection<ParametersItemVM> ParameterItems { get; set; } = [];
        public ICommand RemoveItemCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }
        public ICommand AddItemCommand { get; set; }

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

        private string _parameterValue;
        public string ParameterValue
        {
            get { return _parameterValue; }
            set { _parameterValue = value; OnPropertyChanged(nameof(ParameterValue)); }
        }

        private string _partName;
        public string PartName
        {
            get { return _partName; }
            set { _partName = value; OnPropertyChanged(nameof(PartName)); }
        }

        public ParametersModalVM(MainViewModel mainViewModel, Stores.ModelStore modelStore, ConfigStore configStore)
        {
            _mainViewModel = mainViewModel;
            _modelStore = modelStore;
            _configStore = configStore;
            foreach (var p in _configStore.Cfg.Parameters)
            {
                ParameterItems.Add(new ParametersItemVM(p.ParameterName,p.ParameterValue,p.PartName,p.BoxName));
            }
            CloseModalCommand = new RelayCommand(CloseModal);
            AddItemCommand = new RelayCommand(AddItem);
            RemoveItemCommand = new RelayCommand<object>(RemoveItem);
        }
        private void CloseModal()
        {
            //_modelStore.SetBoxes(BoxItems.ToDictionary(x => x.Name, x => new BBox(x.GetPoints())));
            _configStore.AddParameters(ParameterItems);
            _mainViewModel.IsOpen = false;
        }
        private void RemoveItem(object parameter)
        {
            if (parameter is ParametersItemVM item && ParameterItems.Contains(item))
            {
                ParameterItems.Remove(item);
            }
        }
        private void AddItem()
        {
            if(ParameterName=="") return;
            ParameterItems.Add(new ParametersItemVM(ParameterName, ParameterValue, PartName, BoxName));
        }
    }
}
