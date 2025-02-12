using IFCApp.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IFCApp.TeklaServices.Services;
using Tekla.Structures.Model;
using IFCApp.UI.Stores;
using IFCApp.UI.ViewModel.Modals;

namespace IFCApp.UI.ViewModel.Sections
{
    class ParametersVM : SectionVMBase
    {
        //Parameters
        public string ModelName
        {
            get { return _modelStore.Model?.ModelName; }
            set {  _modelStore.SetModelName(value); }
        }

        private string _errors;
        public string Errors
        {
            get { return _errors; }
            set { _errors = value; OnPropertyChanged(nameof(Errors)); }
        }

        public ICommand AddParametersCommand { get; set; }

        //Boilerplate
        private ModelManagerVM _parentViewModel;
        private MainViewModel _mainViewModel;
        private ModelStore _modelStore;
        private ConfigStore _configStore;
        public ICommand ChangeViewCommand { get; set; }
        public ICommand OpenBoxModalCommand { get; set; }
        public ICommand OpenParametersModalCommand { get; set; }

        public ParametersVM(string name, ModelManagerVM vm, ModelStore modelStore, MainViewModel mainvm, ConfigStore cfgStore) : base(name)
        {
            _mainViewModel = mainvm;
            _modelStore = modelStore;
            _configStore = cfgStore;
            _parentViewModel = vm;
            ChangeViewCommand = new RelayCommand(ChangeView);
            AddParametersCommand = new RelayCommand(AddParameters);
            OpenBoxModalCommand = new RelayCommand(OpenBoxModal);
            OpenParametersModalCommand = new RelayCommand(OpenParametersModal);
            Errors = "";
            _modelStore.ModelChanged += ModelChanged;
        }

        private void OpenParametersModal()
        {
            _mainViewModel.SelectedModal = new ParametersModalVM(_mainViewModel, _modelStore, _configStore);
            _mainViewModel.IsOpen = true;
        }

        private void OpenBoxModal()
        { 
            _mainViewModel.SelectedModal = new BoxModalVM(_mainViewModel, _modelStore);
            _mainViewModel.IsOpen = true;
        }

        private void ModelChanged()
        {
            OnPropertyChanged(nameof(ModelName));   
        }

        
        private void ChangeView()
        {
            _parentViewModel.SelectedSection = this;
        }
        private void AddParameters()
        {
            NVAAtributeCreator atrCreator  = new NVAAtributeCreator();
            Errors = atrCreator.CreateClassificationForAllParts();
            atrCreator.CreateAttributesForAllParts();

            ParameterCreator pCreator = new ParameterCreator();
            foreach (var param in _configStore.Cfg.Parameters)
            {
                var boxName = "";
                if (param.BoxName != null)  boxName = param.BoxName;
                var boxes = _modelStore.Model.BBoxes;
                if(boxes.TryGetValue(boxName, out var box))
                {
                    pCreator.CreateParameter(param.ParameterName, param.ParameterValue, param.PartName, box);
                }
                else
                {
                    pCreator.CreateParameter(param.ParameterName, param.ParameterValue, param.PartName);
                }
            }
            new Model().CommitChanges();
        }

        public override void Dispose()
        {
            _modelStore.ModelChanged -= ModelChanged;
            base.Dispose();
        }

    }
}
