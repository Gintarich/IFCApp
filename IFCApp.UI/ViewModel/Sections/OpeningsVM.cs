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
        private ConfigStore _configStore;
        private List<Wall> _walls = [];

        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged(nameof(Path)); }
        }

        public ICommand LoadOpeningsCommand { get; set; }
        public ICommand ShowOpeningsCommand { get; set; }
        public ICommand InsertOpeningsCommand { get; set; }
        public ICommand ChangeViewCommand { get; set; }
        public OpeningsVM(string name, ModelManagerVM vm, ModelStore modelStore, MainViewModel mainvm, ConfigStore cfgStore) : base(name)
        {
            _parentViewModel = vm;
            _modelStore = modelStore;
            _configStore = cfgStore;
            ChangeViewCommand = new RelayCommand(ChangeView);
            LoadOpeningsCommand = new RelayCommand(LoadOpenings);
            ShowOpeningsCommand = new RelayCommand(ShowOpenings);
            InsertOpeningsCommand = new RelayCommand(InsertOpenings);
            _modelStore.ModelChanged += UpdateWalls;
            _modelStore.ModelLoaded += OnModelLoaded;
        }
        private void ChangeView()
        {
            _parentViewModel.SelectedSection = this;
        }
        public void OnModelLoaded()
        {
            var archModelName = _modelStore.Model.ModelName.Replace("BK", "AR");
            Path = _modelStore.GetFolderPath() + $"\\{archModelName}.ifc";
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
            var guids = _walls.Select(x => x.ID).ToList();
            //Dependencies
            BBoxService bBoxService = new BBoxService();
            TransformationService transformationService = new TransformationService(_modelStore.Model.CS);
            TeklaBoundingBoxService teklaBoundingBoxService = new TeklaBoundingBoxService();
            //Script

            //Get Windows
            IFCModel model = new IFCModel(Path);
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
                    guids.Remove(door.ID);
                    wall.TryToAddOpening(door);
                }
                foreach (var win in windows)
                {
                    guids.Remove(win.ID);
                    wall.TryToAddOpening(win);
                }
            }
            if (guids.Count > 0)
            {
                foreach (var wall in _walls)
                {
                    foreach (var opening in wall.Openings)
                    {
                        if (guids.Contains(opening.ID))
                        {
                            wall.Openings.Remove(opening);
                        }
                    }
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
