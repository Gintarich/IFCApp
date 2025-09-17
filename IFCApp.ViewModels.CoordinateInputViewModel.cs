using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace IFCApp.ViewModels
{
    public class CoordinateInputViewModel : INotifyPropertyChanged
    {
        private double _north;
        private double _east;
        private double _height;
        private double _angle;

        public double North
        {
            get => _north;
            set { _north = value; OnPropertyChanged(); }
        }
        public double East
        {
            get => _east;
            set { _east = value; OnPropertyChanged(); }
        }
        public double Height
        {
            get => _height;
            set { _height = value; OnPropertyChanged(); }
        }
        public double Angle
        {
            get => _angle;
            set { _angle = value; OnPropertyChanged(); }
        }

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}