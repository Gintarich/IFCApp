using IFCApp.Core;
using IFCApp.UI.Core;
using IFCApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.IO;
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
            var path = _modelStore.GetFolderPath();
            if (Directory.Exists(path))
            {
                var jsonFiles = Directory.GetFiles(path, "*.json").ToList();
                if (jsonFiles.Count == 0)
                {
                    MessageBox.Show("No models found in directory.");
                    return;
                }
                else if (jsonFiles.Count == 1)
                {
                    _modelStore.LoadModel(Path.GetFileNameWithoutExtension(jsonFiles[0]));
                    _mainViewModel.IsOpen = false;
                    return;
                }
                else
                {
                    var fileNames = jsonFiles.Select(Path.GetFileNameWithoutExtension).ToList();
                    _mainViewModel.SelectedModal = new SelectModelModalVM(fileNames, _modelStore, _mainViewModel);
                }
            }
            else
            {
                MessageBox.Show("Directory does not exist.");
            }
        }

        private void CreateModel()
        {
            _modelStore.CreateModel();
            _mainViewModel.IsOpen = false;
        }
    }
}
