using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Services;
using IFCApp.UI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IFCApp.UI.ViewModel.Sections
{
    public class WallsWM : SectionVMBase
    {
        public ICommand LoadWallsCommand { get; set; }
        public WallsWM(string name) : base(name)
        {
            LoadWallsCommand = new RelayCommand(LoadWalls);
        }

        private void LoadWalls()
        {
            TeklaBoundingBoxService boxService = new();
            TeklaWallService wService = new(boxService);

        }
    }
}
