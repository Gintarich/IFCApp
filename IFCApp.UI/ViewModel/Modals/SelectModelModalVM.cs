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
    internal class SelectModelModalVM : ViewModelBase
    {
        ObservableCollection<string> _models = [];
        public IEnumerable<string> Models => _models;
        private ModelStore _modelStore;
        private readonly MainViewModel _mainViewModel;
        private string _selectedItem;
        public string SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); }
        }

        public ICommand OpenModel { get; set; }

        public SelectModelModalVM(List<string> models, ModelStore modelStore, MainViewModel mainViewModel)
        {
            _models = new ObservableCollection<string>(models);
            OpenModel = new RelayCommand(OpenSelectedModel);
            _modelStore = modelStore;
            _mainViewModel = mainViewModel;
        }

        private void OpenSelectedModel()
        {
            _modelStore.LoadModel(SelectedItem);
            _mainViewModel.IsOpen = false;
        }
    }
}
