using IFCApp.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IFCApp.UI.ViewModel.Sections
{
    class OpeningsVM : SectionVMBase
    {
        private ModelManagerVM _parentViewModel;
        public ICommand ChangeViewCommand { get; set; }
        public OpeningsVM(string name, ModelManagerVM vm) : base(name)
        {
            _parentViewModel = vm;
            ChangeViewCommand = new RelayCommand(ChangeView);
        }
        private void ChangeView()
        {
            _parentViewModel.SelectedSection = this;
        }
    }
}
