using IFCApp.Core;
using IFCApp.Core.Geometry;
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

        private ModelAttributeServer _attrServer = new ModelAttributeServer();
        private Model _model;
        public Model Model
        {
            get { return _model; }
            set { _model = value; ModelChanged?.Invoke(); }
        }

        public event Action ModelChanged;
        public ModelStore()
        {
            _model = new Model();
        }

        public string GetFolderPath()
        {
            return Path.Combine(_attrServer.GetFilePath(), "Automation");
        }
        public void SetModelName(string name)
        {
            Model.ModelName = name;
            ModelChanged?.Invoke();
        }
        public void CreateModel()
        {
            _model = new Model();
            _model.ModelName = _attrServer.GetModelName();
            var path = _attrServer.GetFilePath();
            var modelPath = Path.Combine(path, "Automation");
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
            if (!Directory.Exists(GetFolderPath())) { return; }
            var path = Path.Combine(GetFolderPath(), _model.ModelName + ".json");
            var json = JsonSerializer.Serialize(_model);
            File.WriteAllText(path, json);
        }
        public void LoadModel()
        {
            ModelChanged?.Invoke();
        }
        public void LoadModel(string name)
        {
            string path = Path.Combine(GetFolderPath(), name + ".json");
            var json = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(json))
            {
                var model = JsonSerializer.Deserialize<Model>(json);
                if (model != null)
                {
                    _model = model;
                }
            }
            ModelChanged?.Invoke();
        }
        public Dictionary<string, BBox> GetBoxes()
        {
            return _model.BBoxes;
        }
        public void SetBoxes(Dictionary<string, BBox> boxes)
        {
            ModelChanged?.Invoke(); 
            _model.BBoxes = boxes;
        }
    }
}
