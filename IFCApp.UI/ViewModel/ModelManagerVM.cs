using IFCApp.UI.Core;
using IFCApp.UI.Stores;
using IFCApp.UI.ViewModel.Sections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IFCApp.UI.ViewModel
{
    public class ModelManagerVM : ViewModelBase
    {
        private ObservableCollection<SectionVMBase> _sections = new ObservableCollection<SectionVMBase>();
        public IEnumerable<SectionVMBase> Sections => _sections;
        public ICommand ChangeToProjectSectionCommand;
        public ICommand ChangeToDrawingSectionCommand;
        private SectionVMBase _selectedSection;
        public SectionVMBase SelectedSection
        {
            get { return _selectedSection; }
            set
            {
                _selectedSection = value;
                OnPropertyChanged(nameof(SelectedSection));
            }
        }

        //public property here ?

        public ModelManagerVM(ModelStore modelStore, MainViewModel mainvm, ConfigStore cfgStore)
        {
            _sections.Add(new ParametersVM("Parameters", this, modelStore, mainvm, cfgStore));
            _sections.Add(new OpeningsVM("Openings", this, modelStore, mainvm, cfgStore));
            _sections.Add(new WallsWM("Walls", this, modelStore, mainvm, cfgStore));
            _sections.Add(new SlabsVM("Slabs", this, modelStore, mainvm, cfgStore));
            _selectedSection = _sections.FirstOrDefault();
            ChangeToProjectSectionCommand = new RelayCommand(
                () => SelectedSection = _sections.First(x => x.Name == "Project parameters"));
            ChangeToProjectSectionCommand = new RelayCommand(
                () => SelectedSection = _sections.First(x => x.Name == "Drawing parameters"));
            //OnPropertyChanged(nameof(Sections));
        }
    }
}
