using IFCApp.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IFCApp.TeklaServices.Services;
using Tekla.Structures.Model;

namespace IFCApp.UI.ViewModel.Sections
{
    class ParametersVM : SectionVMBase
    {
        //Parameters
        private string _modelName;

        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; OnPropertyChanged(nameof(ModelName)); }
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
        public ICommand ChangeViewCommand { get; set; }
        public ParametersVM(string name, ModelManagerVM vm) : base(name)
        {
            _parentViewModel = vm;
            ChangeViewCommand = new RelayCommand(ChangeView);
            AddParametersCommand = new RelayCommand(AddParameters);
            ModelName = ModelAttributeServer.GetModelName();
            Errors = "TEST123";
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
    }
}
