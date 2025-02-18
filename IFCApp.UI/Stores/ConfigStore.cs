using IFCApp.TeklaServices.Services;
using IFCApp.UI.Models;
using IFCApp.UI.Models;
using IFCApp.UI.ViewModel.Modals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IFCApp.UI.Stores;

public class ConfigStore
{
    private ModelAttributeServer _attrServer = new ModelAttributeServer();
    public Config Cfg { get; set; } = new Config();
    public event Action ConfigChanged;
    public ConfigStore() { }

    public void AddParameters(ObservableCollection<ParametersItemVM> parametersItems)
    {
        Cfg.Parameters.Clear();
        foreach (var p in parametersItems)
        {
            Cfg.Parameters.Add(new ParametersItemModel() 
                { BoxName = p.BoxName, ParameterName = p.ParameterName, ParameterValue = p.Value, PartName = p.PartName });
        }
    }

    public string GetFolderPath()
    {
        return Path.Combine(_attrServer.GetFilePath(), "Automation");
    }

    public void SetWallNames(string val)
    {
        Cfg.WallNames = val;
        ConfigChanged?.Invoke();
    }
    public void Save()
    {
        if (!Directory.Exists(GetFolderPath())) { return; }
        var path = Path.Combine(GetFolderPath(), "config.json");
        var json = JsonSerializer.Serialize(Cfg);
        File.WriteAllText(path, json);
    }
    public void Load()
    {
        string path = Path.Combine(GetFolderPath(), "config.json");
        if (!File.Exists(path))
        {
            Cfg = new Config();
        }
        else
        {
            var json = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(json))
            {
                var cfg = JsonSerializer.Deserialize<Config>(json);
                if (cfg != null)
                {
                    Cfg = cfg;
                }
            }
        }
        ConfigChanged?.Invoke();
    }
}
