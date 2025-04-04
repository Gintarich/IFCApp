using IFCApp.UI.Stores;
using IFCApp.UI.ViewModel;
using System.Windows;

namespace IFCApp.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ModelStore _modelStore;
        private readonly ConfigStore _configStore;
        public App()
        {
            _modelStore = new ModelStore();
            _configStore = new ConfigStore();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            ModelStore modelStore = _modelStore;
            ConfigStore configStore = _configStore;
            var mainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(modelStore, configStore)
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
                _configStore.Save();
            }
            base.OnExit(e);
        }
    }

}
