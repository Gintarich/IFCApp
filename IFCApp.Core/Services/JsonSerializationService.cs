using IFCApp.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IFCApp.Core.Services
{
    public class JsonSerializationService
    {
        private readonly string _path;

        public JsonSerializationService(string path)
        {
            _path = path;
        }
        public void Write(Model model)
        {
            if (!Directory.Exists(_path)) { return; }
            var json = JsonSerializer.Serialize(model);
            File.WriteAllText(_path, json);
        }
        public Model Read()
        {
            var ModelOut = new Model();
            string path = _path;
            var json = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(json))
            {
                var model = JsonSerializer.Deserialize<Model>(json);
                if (model != null)
                {
                    return model;
                }
            }
            return null;
        }
    }

}
