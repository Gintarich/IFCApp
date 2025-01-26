using System;
using System.Collections.Generic;
using System.Text;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Services
{
    public class ModelAttributeServer
    {
        private Model _model;

        public ModelAttributeServer()
        {
            _model = new Model();
        }
        public ModelAttributeServer(Model model)
        {
            _model = model;
        }
        public string GetModelName()
        {
            var model = new Model();
            return model.GetInfo().ModelName.Split('.')[0];
        }
        public string GetFilePath()
        {
            var model = new Model();
            return model.GetInfo().ModelPath;
        }
    }
}
