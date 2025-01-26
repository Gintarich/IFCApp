using IFCApp.Core;
using IFCApp.UI.Core;
using IFCApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TS=Tekla.Structures.Model;

namespace IFCApp.UI.ViewModel.Modals
{
    public class StartAppModalVM : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        public ICommand CreateNewModelCommad { get; set; }
        public ICommand LoadModelCommand { get; set; }

        private bool _isTeklaOpen;
        public bool IsTeklaOpen
        {
            get { return _isTeklaOpen; }
            set { _isTeklaOpen = value; OnPropertyChanged(nameof(IsTeklaOpen)); }
        }

        private ModelStore _modelStore;
        private TS.Model _tsModel;
        public StartAppModalVM(MainViewModel mainViewModel, Stores.ModelStore modelStore)
        {
            _modelStore = modelStore;
            _mainViewModel = mainViewModel;
            CreateNewModelCommad = new RelayCommand(CreateModel,TeklaOpen);
            LoadModelCommand = new RelayCommand(LoadModel,TeklaOpen);
            _tsModel= new TS.Model();
            IsTeklaOpen = _tsModel.GetConnectionStatus();
        }

        private bool TeklaOpen()
        {
            return IsTeklaOpen;
        }

        private void LoadModel()
        {
            _modelStore.LoadModel();
            _mainViewModel.IsOpen = false;
        }

        private void CreateModel()
        {
            _modelStore.CreateModel();
            _mainViewModel.IsOpen = false;
        }
    }
}
