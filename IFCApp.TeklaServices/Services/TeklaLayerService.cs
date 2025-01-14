using IFCApp.Core.Elements;
using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.Model;
using TS = Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Services;

public class TeklaLayerService
{
    private Assembly _assembly;
    private Component _component;
    private List<int> _layers = new List<int>();
    public TeklaLayerService(Assembly assembly)
    {
        _assembly = assembly;
        var mainPart = _assembly.GetMainPart();
        _component = mainPart.GetFatherComponent() as Component;
        if (_component is null)
        {
            _layers.Add(0);
            return;
        }
        var str = string.Empty;
        _component.GetUserProperty("ActiveLayers", ref str);
        _layers = str.Split(';').Select(x => Int32.Parse(x)).ToList();
    }

    public int GetLayerCount()
    {
        return _layers.Count;
    }
    public Layers GetLayers()
    {
        var createdObjects = _component.GetChildren().ToList().Where(x => x is TS.Part).Cast<Part>().ToList();
        var bearingLayer = createdObjects.Where(x => x.Name == "NESOŠAIS SLĀNIS").FirstOrDefault();
        var insulationLayer = createdObjects.Where(x => x.Name == "SILTUMIZOLĀCIJA").FirstOrDefault();
        var outerLayer = createdObjects.Where(x => x.Name == "APDARES SLĀNIS").FirstOrDefault();
        var bearingThickness = bearingLayer.GetDoubleProp("WIDTH");
        var insulationThickness = insulationLayer.GetDoubleProp("WIDTH");
        var outerThickness = outerLayer.GetDoubleProp("WIDTH");

        var layers = new Layers();
        layers.OuterLayerThickness = outerThickness;
        layers.InnerLayerThickness = bearingThickness;
        layers.InsulationThickness = insulationThickness;
        return layers;
    }
}
