using IFCApp.Core.Elements;
using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Services;
using IFCApp.UI.Core;
using IFCApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IFCApp.UI.ViewModel.Sections
{
    public class WallsWM : SectionVMBase
    {
        private ModelManagerVM _parentViewModel;
        private ModelStore _modelStore;
        private ConfigStore _configStore;
        //private string _wallNames;

        public string WallNames
        {
            get { return _configStore.Cfg.WallNames; }
            set { _configStore.SetWallNames(value); OnPropertyChanged(nameof(WallNames)); }
        }

        public ICommand LoadWallsCommand { get; set; }
        public ICommand ChangeViewCommand { get; set; }
        public WallsWM(string name, ModelManagerVM vm, Stores.ModelStore modelStore, MainViewModel mainvm, Stores.ConfigStore cfgStore) : base(name)
        {
            LoadWallsCommand = new RelayCommand<string>(LoadWalls);
            ChangeViewCommand = new RelayCommand(ChangeView);
            _parentViewModel = vm;
            _modelStore = modelStore;
            _configStore = cfgStore;
            WallNames = cfgStore.Cfg?.WallNames is null ? "" : cfgStore.Cfg.WallNames;
        }

        private void ChangeView()
        {
            _parentViewModel.SelectedSection = this;
        }

        private void LoadWalls(string WallNames)
        {
            var splitNames = WallNames.Split(',').Select(x=>x.Trim()).ToList();
            TeklaBoundingBoxService boxService = new();
            TeklaWallService wService = new(boxService);
            var walls = wService.GetWalls(splitNames);
            foreach (var wall in walls)
            {
                var model = _modelStore.Model;
                if (model.TryGetValue(wall.ID, out var el))
                {
                    if (el is Wall wallEl)
                    {
                        wallEl.Box = wall.Box;
                        wallEl.ID = wall.ID;
                        wallEl.Matrix = wall.Matrix;
                        wallEl.Openings = wall.Openings;
                        wallEl.ShouldHaveOpening = wall.ShouldHaveOpening;
                        wallEl.UserData = wall.UserData;
                    }
                }
                else
                {
                    model.Insert(wall);
                }
            }
            _modelStore.Update();
            //TODO: Remove Unused walls ??
        }
    }
}
