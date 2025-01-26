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
        private ModelStore _modelStore;
        public ICommand ChangeViewCommand { get; set; }

        public ParametersVM(string name, ModelManagerVM vm, ModelStore modelStore) : base(name)
        {
            _modelStore = modelStore;
            _parentViewModel = vm;
            ChangeViewCommand = new RelayCommand(ChangeView);
            AddParametersCommand = new RelayCommand(AddParameters);
            //ModelName = ModelAttributeServer.GetModelName();
            Errors = "";
            _modelStore.ModelChanged += ModelChanged;
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
            new Model().CommitChanges();
        }

        public override void Dispose()
        {
            _modelStore.ModelChanged -= ModelChanged;
            base.Dispose();
        }

    }
}
