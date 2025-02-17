using IFCApp.Core.Elements;
using IFCApp.TeklaServices.Services;
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
    internal class SlabsVM : SectionVMBase
    {
        private ModelManagerVM _parentViewModel;
        private ModelStore _modelStore;
        public ICommand LoadSlabsCommand { get; set; }
        public ICommand ChangeViewCommand { get; set; }
        public SlabsVM(string name, ModelManagerVM vm, Stores.ModelStore modelStore, MainViewModel mainvm, Stores.ConfigStore cfgStore) : base(name)
        {
            LoadSlabsCommand = new RelayCommand(LoadSlabs);
            ChangeViewCommand = new RelayCommand(ChangeView);
            _parentViewModel = vm;
            _modelStore = modelStore;
        }

        private void ChangeView()
        {
            _parentViewModel.SelectedSection = this;
        }

        private void LoadSlabs()
        {
            TeklaBoundingBoxService boxService = new();
            TeklaSlabService sService = new(boxService);
            var slabs = sService.GetSlabs(["PAMATU PLĀTNE"]);
            foreach (var slab in slabs)
            {
                var model = _modelStore.Model;
                if (model.TryGetValue(slab.ID, out var el))
                {
                    if (el is Slab wallEl)
                    {
                        wallEl.ID = slab.ID;
                        wallEl.UserData = slab.UserData;
                    }
                }
                else
                {
                    model.Insert(slab);
                }
            }
            _modelStore.Update();
            //TODO: Remove Unused walls ??
        }
    }
}
