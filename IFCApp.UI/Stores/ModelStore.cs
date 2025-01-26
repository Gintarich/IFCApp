using IFCApp.Core;
using IFCApp.TeklaServices.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IFCApp.UI.Stores
{
    public class ModelStore
    {

        private Model _model;

        public Model Model
        {
            get { return _model; }
            set { _model = value; ModelChanged?.Invoke(); }
        }

        public event Action ModelChanged;
        public void SetModelName(string name)
        {
            Model.ModelName = name;
            ModelChanged?.Invoke();
        }
        public void CreateModel()
        {
            _model = new Model();
            ModelAttributeServer atrServer = new ModelAttributeServer();
            _model.ModelName = atrServer.GetModelName();
            var path = atrServer.GetFilePath();
            var modelPath= Path.Combine(path, "Automation");
            if (!Directory.Exists(modelPath))
            {
                Directory.CreateDirectory(modelPath);
            }
            _model.ModelPath = modelPath;
            SaveModel();
            ModelChanged?.Invoke();
        }
        public void SaveModel()
        {
            if(!Directory.Exists(_model.ModelPath)) { return; }
            var path = Path.Combine(_model.ModelPath, _model.ModelName + ".json");
            var json = JsonSerializer.Serialize(_model);
            File.WriteAllText(path, json);
        }
        public void LoadModel()
        {
            throw new NotImplementedException();
            ModelChanged?.Invoke();
        }
    }
}
