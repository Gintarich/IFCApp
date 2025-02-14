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
        public ICommand LoadWallsCommand { get; set; }
        public ICommand ChangeViewCommand { get; set; }
        public WallsWM(string name, ModelManagerVM vm, Stores.ModelStore modelStore, MainViewModel mainvm, Stores.ConfigStore cfgStore) : base(name)
        {
            LoadWallsCommand = new RelayCommand(LoadWalls);
            ChangeViewCommand = new RelayCommand(ChangeView);
            _parentViewModel = vm;
            _modelStore = modelStore;
        }

        private void ChangeView()
        {
            _parentViewModel.SelectedSection = this;
        }

        private void LoadWalls()
        {
            TeklaBoundingBoxService boxService = new();
            TeklaWallService wService = new(boxService);
            var walls = wService.GetWalls(["TRĪSSLĀŅU SIENAS PANELIS", "VIENSLĀŅU SIENAS PANELIS"]);
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
