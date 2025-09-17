using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using IFCApp.UI.Stores;
using IFCApp.UI.Core;
using IFCApp.Core.Geometry;
using IFCApp.IFCServices.Utils;
using IFCApp.Core.Elements;

namespace IFCApp.UI.ViewModel.Modals
{
    public class CoordinateInputViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly ModelStore _modelStore;
        private double _north;
        private double _east;
        private double _height;
        private double _angle;

        public double North
        {
            get => _north;
            set { _north = value; OnPropertyChanged(nameof(North)); }
        }
        public double East
        {
            get => _east;
            set { _east = value; OnPropertyChanged(nameof(East)); }
        }
        public double Height
        {
            get => _height;
            set { _height = value; OnPropertyChanged(nameof(Height)); }
        }
        public double Angle
        {
            get => _angle;
            set { _angle = value; OnPropertyChanged(nameof(Angle)); }
        }

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }

        public CoordinateInputViewModel(MainViewModel vm, ModelStore store)
        {
            Coordinates cs = store.GetCoordinates();
            North = cs.North;
            East = cs.East;
            Height = cs.Elevation;
            Angle = cs.Angle;
            _mainViewModel = vm;
            _modelStore = store;
            CloseModalCommand = new RelayCommand(CloseModal);
            OkCommand = new RelayCommand(Ok);
        }

        private void Ok()
        {
            _modelStore.SetCoordinateSystem(new Coordinates(North, East, Height, Angle));
            CloseModal();
        }

        private void CloseModal()
        {
            _mainViewModel.IsOpen = false;
        }
    }
}
