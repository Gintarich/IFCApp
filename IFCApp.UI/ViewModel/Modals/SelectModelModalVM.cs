using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCApp.UI.ViewModel.Modals
{
    internal class SelectModelModalVM : ViewModelBase
    {
        ObservableCollection<ViewModelBase> _models = [];
        IEnumerable<ViewModelBase> Models => _models;
        public SelectModelModalVM()
        {
            
        }
    }
}
