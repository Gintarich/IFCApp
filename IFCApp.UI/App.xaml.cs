using IFCApp.UI.Stores;
using IFCApp.UI.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace IFCApp.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ModelStore _modelStore;
        public App()
        {
            _modelStore = new ModelStore();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            ModelStore modelStore = _modelStore;
            var mainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(modelStore)
            };
            mainWindow.Show();
            base.OnStartup(e);
        }
        override protected void OnExit(ExitEventArgs e)
        {
            var result = MessageBox.Show("Do you want to save the model?", "Save Model", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _modelStore.SaveModel();
            }
            base.OnExit(e);
        }
    }

}
