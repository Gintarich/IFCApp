using IFCApp.Core.Elements;
using IFCApp.IFCServices;
using IFCApp.IFCServices.Services;
using IFCApp.IFCServices.Utils;
using IFCApp.TeklaServices;
using IFCApp.TeklaServices.Services;
using IFCApp.TeklaServices.Utils;
using IFCApp.UI.Core;
using IFCApp.UI.Stores;
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
        private ModelStore _modelStore;
        private List<Wall> _walls = [];
        public ICommand LoadOpeningsCommand { get; set; }
        public ICommand ShowOpeningsCommand { get; set; }
        public ICommand InsertOpeningsCommand { get; set; }
        public ICommand ChangeViewCommand { get; set; }
        public OpeningsVM(string name, ModelManagerVM vm, ModelStore modelStore, MainViewModel mainvm, ConfigStore cfgStore) : base(name)
        {
            _parentViewModel = vm;
            _modelStore = modelStore;
            ChangeViewCommand = new RelayCommand(ChangeView);
            LoadOpeningsCommand = new RelayCommand(LoadOpenings);
            ShowOpeningsCommand = new RelayCommand(ShowOpenings);
            InsertOpeningsCommand = new RelayCommand(InsertOpenings);
            _modelStore.ModelChanged += UpdateWalls;
        }
        private void ChangeView()
        {
            _parentViewModel.SelectedSection = this;
        }
        public void UpdateWalls()
        {
            _walls = _modelStore.Model.Elements.Where(x => x is Wall).Cast<Wall>().ToList();
        }
        private void LoadOpenings()
        {
            if (_walls.Count == 0)
            {
                _walls = _modelStore.Model.Elements.Where(x => x is Wall).Cast<Wall>().ToList();
            }
            //_modelStore.Model.CS = VUGDCoordinateSystems.InverseKul;
            //Dependencies
            BBoxService bBoxService = new BBoxService();
            TransformationService transformationService = new TransformationService(_modelStore.Model.CS);
            TeklaBoundingBoxService teklaBoundingBoxService = new TeklaBoundingBoxService();
            //Script

            //Get Windows
            IFCModel model = new IFCModel();
            IfcDoorService doorServ = new IfcDoorService(model, transformationService, bBoxService);
            var doors = doorServ.GetDoors();

            IfcWindowService windowService = new IfcWindowService(model, transformationService, bBoxService);
            var windows = windowService.GetWindows();

            //Get Walls
            //List<Wall> walls = new TeklaWallService(teklaBoundingBoxService).GetWalls(["TRĪSSLĀŅU SIENAS PANELIS", "VIENSLĀŅU SIENAS PANELIS"]);

            //Add doors to walls
            foreach (Wall wall in _walls)
            {
                foreach (var door in doors)
                {
                    wall.TryToAddOpening(door);
                }
                foreach (var win in windows)
                {
                    wall.TryToAddOpening(win);
                }
            }
            _modelStore.Update();
        }
        private void ShowOpenings()
        {
            if (_walls.Count == 0)
            {
                _walls = _modelStore.Model.Elements.Where(x => x is Wall).Cast<Wall>().ToList();
            }
            TeklaGraphicsDrawerService drawerService = new TeklaGraphicsDrawerService();
            drawerService.DrawOpenings(_walls);
        }
        private void InsertOpenings()
        {
            if (_walls.Count == 0)
            {
                _walls = _modelStore.Model.Elements.Where(x => x is Wall).Cast<Wall>().ToList();
            }
            TeklaDoorConfig dCfng = new TeklaDoorConfig();
            TeklaWindowConfig wCfig = new TeklaWindowConfig();
            TeklaOpeningMaker wm = new(_walls, wCfig, dCfng);
            wm.GenerateOpenings();
        }
    }
}
