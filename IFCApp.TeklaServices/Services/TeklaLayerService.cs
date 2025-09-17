using IFCApp.Core.Elements;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.Model;
using TS = Tekla.Structures.Model;
using Component = Tekla.Structures.Model.Component;

namespace IFCApp.TeklaServices.Services;

public class TeklaLayerService
{
    private Assembly _assembly;
    private Component _component;
    private bool _isComponent;
    private List<int> _layers = new List<int>();
    public TeklaLayerService(Assembly assembly)
    {
        _assembly = assembly;
        var mainPart = _assembly.GetMainPart();
        _component = mainPart.GetFatherComponent() as Component;
        if (_component is null) _isComponent = false;
        else _isComponent = true;
        if (_isComponent)
        {
            var str = string.Empty;
            _component.GetUserProperty("ActiveLayers", ref str);

            if(str == null || str == string.Empty)
            {
                var layerCount = GetLayerCount(_assembly);
                for (int i = 0; i < layerCount; i++)
                {
                    _layers.Add(i);
                }
                return;
            }
            _layers = str.Split(';').Select(x => Int32.Parse(x)).ToList();
        }
        else
        {
            var layerCount = GetLayerCount(_assembly) ;
            for (int i = 0; i < layerCount; i++)
            {
                _layers.Add(i);
            }
        }
    }

    public int GetLayerCount()
    {
        return _layers.Count;
    }

    public int GetLayerCount(Assembly ass)
    {
        int layers = 0;
        var secondaries = _assembly.GetSecondaries();
        secondaries.Add(_assembly.GetMainPart());
        var createdObjects = new List<TS.Part>();
        foreach (var secondary in secondaries)
        {
            if (secondary is not Part secPart) continue;
            createdObjects.Add(secPart);
        }
        var bearingLayer = createdObjects.Where(x => x.Name == "NESOŠAIS SLĀNIS" && x.IsValidLayer()).FirstOrDefault();
        var insulationLayer = createdObjects.Where(x => x.Name == "SILTUMIZOLĀCIJA" && x.IsValidLayer()).FirstOrDefault();
        var outerLayer = createdObjects.Where(x => x.Name == "APDARES SLĀNIS" && x.IsValidLayer()).FirstOrDefault();
        var finishLayer = createdObjects.Where(x => x.Name == "APDARES ĶIEĢELIS" && x.IsValidLayer()).FirstOrDefault();
        if (bearingLayer != null) layers++;
        if (insulationLayer != null) layers++;
        if (outerLayer != null) layers++;
        if (finishLayer != null) layers++;
        return layers;
    }

    public Layers GetLayers()
    {
        if (_isComponent)
        {
            var createdObjects = _component.GetChildren().ToList().Where(x => x is TS.Part).Cast<Part>().ToList();
            var bearingLayer = createdObjects.Where(x => x.Name == "NESOŠAIS SLĀNIS").FirstOrDefault();
            var insulationLayer = createdObjects.Where(x => x.Name == "SILTUMIZOLĀCIJA").FirstOrDefault();
            var outerLayer = createdObjects.Where(x => x.Name == "APDARES SLĀNIS").FirstOrDefault();
            var bearingThickness = bearingLayer.GetDoubleProp("WIDTH");
            var insulationThickness = insulationLayer.GetDoubleProp("WIDTH");
            var outerThickness = outerLayer.GetDoubleProp("WIDTH");
            var layers = new Layers();
            layers.OuterLayerThickness = (int)Math.Round(outerThickness);
            layers.InnerLayerThickness = (int)Math.Round(bearingThickness);
            layers.InsulationThickness = (int)Math.Round(insulationThickness);
            return layers;
        }
        else
        {

            var secondaries = _assembly.GetSecondaries();
            secondaries.Add(_assembly.GetMainPart());
            var createdObjects  = new List<TS.Part>();

            foreach (var secondary in secondaries)
            {
                if (secondary is not Part secPart) continue;
                createdObjects.Add(secPart);
            }
            var bearingLayer = createdObjects.Where(x => x.Name == "NESOŠAIS SLĀNIS" && x.IsValidLayer()).FirstOrDefault();
            var insulationLayer = createdObjects.Where(x => x.Name == "SILTUMIZOLĀCIJA" && x.IsValidLayer()).FirstOrDefault();
            var outerLayer = createdObjects.Where(x => x.Name == "APDARES SLĀNIS" && x.IsValidLayer()).FirstOrDefault();
            var bearingThickness = bearingLayer.GetDoubleProp("WIDTH");
            var insulationThickness = insulationLayer.GetDoubleProp("WIDTH");
            var outerThickness = outerLayer.GetDoubleProp("WIDTH");
            var layers = new Layers();
            layers.OuterLayerThickness = (int)Math.Round(outerThickness);
            layers.InnerLayerThickness = (int)Math.Round(bearingThickness);
            layers.InsulationThickness = (int)Math.Round(insulationThickness);
            return layers;
        }
    }
}
