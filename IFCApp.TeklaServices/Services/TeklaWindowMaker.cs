using IFCApp.TeklaServices.Utils;
using IFCApp.Core.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.Geometry3d;
using IFCApp.Core.Geometry;
using Tekla.Structures;
using TS = Tekla.Structures.Model;
using Tekla.Structures.Model;
using System.Data;
using Tekla.Structures.ModelInternal;

namespace IFCApp.TeklaServices;

public class TeklaWindowMaker
{
    private TS.Component _windowComponent;
    public List<Wall> Walls { get; }
    public TS.Model Model { get; }

    public TeklaWindowMaker(List<Wall> walls)
    {
        Walls = walls;
        Model = new TS.Model();
    }

    public void GenerateOpenings()
    {
        Clear();
        Model.CommitChanges();
        foreach (Wall wall in Walls)
        {
            if (wall is SandwichPanel sandwichPanel)
            {
                _windowComponent = SetupComponent(sandwichPanel);
            }
            else
            {
                _windowComponent = SetupComponent();
            }
            List<Window> windows = wall.GetWindows();
            foreach (Window window in windows)
            {
                InsertWindow(window);
            }
        }
        Model.CommitChanges();
    }

    private void InsertWindow(Window window)
    {
        Point startPoint = window.GetStartPoint().TeklaPoint();
        Point endPoint = window.GetEndPoint().TeklaPoint();
        Identifier identifier = new Identifier(window.FatherID);
        var father = Model.SelectModelObject(identifier) as TS.Beam;
        if (father == null) return;

        //var mainpart = father.GetChildren()
        //    .ToList()
        //    .Where(x => x is TS.Beam b)
        //    .Cast<TS.Beam>()
        //    .Where(x => x.Name == "NESOŠAIS SLĀNIS").First();

        //if (mainpart == null) return;

        TS.ComponentInput input = new TS.ComponentInput();
        input.AddInputObject(father);
        input.AddOneInputPosition(startPoint);
        input.AddOneInputPosition(endPoint);
        _windowComponent.SetComponentInput(input);
        _windowComponent.Insert();
        _windowComponent.SetUserProperty("Inserted", 1);
        _windowComponent.Modify();
    }

    private TS.Component SetupComponent(SandwichPanel sandwichPanel)
    {
        Layers layers = sandwichPanel.Layers;
        var insulationPart = 60;
        double thickPartWidth = layers.InsulationThickness + layers.InnerLayerThickness - insulationPart;
        double insulationOffset = layers.InsulationThickness - insulationPart;
        TS.Component opening = new TS.Component();
        opening.Name = "SandwichWallWindow";
        opening.Number = TS.Component.PLUGIN_OBJECT_NUMBER;

        opening.SetAttribute("insul_class_list", "76");
        opening.SetAttribute("out_class_list", "29");
        opening.SetAttribute("pict_input_type", 0);
        opening.SetAttribute("pict_pt0_off_x", 0.0);
        opening.SetAttribute("pict_pt0_off_z", 0.0);
        opening.SetAttribute("pict_pt1_off_x", 0.0);
        opening.SetAttribute("pict_pt1_off_z", 0.0);
        //TOP
        //Iekšējais
        opening.SetAttribute("T_inside_cut_y", 10.0);
        opening.SetAttribute("T_inside_cut_z", 10.0);
        opening.SetAttribute("T_inside_cb", 2); // Otrais tips nesošajā slānī
        opening.SetAttribute("T_inside_ext_h", 100.0); // Pabiezinājuma augstums
        opening.SetAttribute("T_inside_ext_l", thickPartWidth); // Pabiezinājuma garums
        //Izoācija
        opening.SetAttribute("T_insul_off", 100.0); // Izolācijas offsets
        //Apdares slānis
        opening.SetAttribute("T_outside_off", -30.0); // Apdares slāņa offsets
        //Koka elements 
        opening.SetAttribute("T_wood.profile", "BL100*60"); // Profils 
        opening.SetAttribute("T_wood.material", "KOOLTHERM K20");
        opening.SetAttribute("T_wood.name", "IZOLĀCIJA");
        opening.SetAttribute("T_wood_offset_y", insulationOffset); // Profila offsets
        //Lāsenis
        opening.SetAttribute("T_drip_cut_L", 10);
        opening.SetAttribute("T_drip_cut_length", 20.0);
        opening.SetAttribute("T_drip_cut_R", 10.0);
        opening.SetAttribute("T_drip_cut_z", 10.0);
        opening.SetAttribute("T_drip_mold_cb", 2);

        //BOTTOM
        opening.SetAttribute("B_inside_cut_y", 10.0);
        opening.SetAttribute("B_inside_cut_z", 10.0);
        opening.SetAttribute("B_con_slope_cb", 0);
        opening.SetAttribute("b_create_ext_foil", 0);
        opening.SetAttribute("B_fillers_cb", 0);
        opening.SetAttribute("B_inside_cb", 2);
        opening.SetAttribute("B_inside_ext_h", 90.0);
        opening.SetAttribute("B_inside_ext_l", thickPartWidth);
        opening.SetAttribute("B_insul_cb", 0);
        opening.SetAttribute("B_insul_foil_cb", 0);
        opening.SetAttribute("B_insul_off", 90.0);
        opening.SetAttribute("B_outside_cb", 0);
        opening.SetAttribute("B_outside_cut_y", layers.OuterLayerThickness);
        opening.SetAttribute("B_outside_cut_z", 20.0);
        opening.SetAttribute("B_screws_def_type", 0);
        opening.SetAttribute("B_wood.profile", "BL90*60");
        opening.SetAttribute("B_wood.material", "KOOLTHERM K20");
        opening.SetAttribute("B_wood.name", "IZOLĀCIJA");
        opening.SetAttribute("B_wood_cb", 0);
        opening.SetAttribute("B_wood_offset_y", insulationOffset);

        opening.SetAttribute("L_con_slope_cb", 0);
        opening.SetAttribute("L_fillers_cb", 0);
        opening.SetAttribute("L_inside_cb", 2);
        opening.SetAttribute("L_inside_cut_y", 10.0);
        opening.SetAttribute("L_inside_cut_z", 10.0);
        opening.SetAttribute("L_inside_ext_h", 90.0);
        opening.SetAttribute("L_inside_ext_l", thickPartWidth);
        opening.SetAttribute("L_insul_cb", 0);
        opening.SetAttribute("L_insul_foil_cb", 0);
        opening.SetAttribute("L_insul_off", 90.0);
        opening.SetAttribute("L_outside_cb", 0);
        opening.SetAttribute("L_outside_off", -30.0);
        opening.SetAttribute("L_screws_def_type", 0);
        opening.SetAttribute("L_wood.material", "KOOLTHERM K20");
        opening.SetAttribute("L_wood.name", "IZOLĀCIJA");
        opening.SetAttribute("L_wood.profile", "BL90*60");
        opening.SetAttribute("L_wood_cb", 0);
        opening.SetAttribute("L_wood_offset_y", insulationOffset);
        opening.SetAttribute("right_same_as_left", 0);

        opening.SetAttribute("create_screws", 2);

        return opening;
    }

    private TS.Component SetupComponent()
    {
        TS.Component opening = new TS.Component();
        opening.Name = "SandwichWallWindow";
        opening.Number = TS.Component.PLUGIN_OBJECT_NUMBER;
        return opening;
    }
    private void Clear()
    {
        var selector = Model.GetModelObjectSelector();
        var components = selector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.COMPONENT)
            .ToList().Cast<Component>();
        var openings = components.
            Where(x => x.Name == "SandwichWallWindow" && x.IsUserCreated()).ToList();
        foreach (var opening in openings)
        {
            opening.Delete();
        }
    }
}
