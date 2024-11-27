using IFCApp.TeklaServices.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace IFCApp.TeklaServices.Elements
{
    public class TeklaWindow
    {
        int PanelID { get; set; }
        Point MinPoint { get; set; }
        Point MaxPoint { get; set; }

        public void Insert()
        {

            Model model = new Model();

            Console.WriteLine(model.GetConnectionStatus());

            var wallLayout = model.GetModelObjectSelector()
                .GetAllObjectsWithType(ModelObject.ModelObjectEnum.COMPONENT)
                .ToList().Cast<Component>().Where(x => x.Identifier.ID == 241555).First();

            var mainpart = wallLayout.GetChildren()
                .ToList()
                .Where(x => x is Beam b)
                .Cast<Beam>()
                .Where(x => x.Name == "NESOŠAIS SLĀNIS").First();

            Component opening = new Component();
            opening.Name = "SandwichWallWindow";
            opening.Number = Component.PLUGIN_OBJECT_NUMBER;

            opening.SetAttribute("insul_class_list", "76");
            opening.SetAttribute("out_class_list", "29");
            opening.SetAttribute("pict_input_type", 0);
            opening.SetAttribute("pict_pt0_off_x", 0.0);
            opening.SetAttribute("pict_pt0_off_z", 0.0);
            opening.SetAttribute("pict_pt1_off_x", 0.0);
            opening.SetAttribute("pict_pt1_off_z", 0.0);
            //TOP
            //Iekšējais
            opening.SetAttribute("T_inside_cb", 2); // Otrais tips nesošajā slānī
            opening.SetAttribute("T_inside_ext_h", 100.0); // Pabiezinājuma augstums
            opening.SetAttribute("T_inside_ext_l", 320.0); // Pabiezinājuma garums
            //Izoācija
            opening.SetAttribute("T_insul_off", 100.0); // Izolācijas offsets
            //Apdares slānis
            opening.SetAttribute("T_outside_off", -30.0); // Apdares slāņa offsets
            //Koka elements 
            opening.SetAttribute("T_wood.profile", "BL100*60"); // Profils 
            opening.SetAttribute("T_wood_offset_y", 120.0); // Profila offsets
            //Lāsenis
            opening.SetAttribute("T_drip_cut_L", 10);
            opening.SetAttribute("T_drip_cut_length", 20.0);
            opening.SetAttribute("T_drip_cut_R", 10.0);
            opening.SetAttribute("T_drip_cut_z", 10.0);
            opening.SetAttribute("T_drip_mold_cb", 2);


            ComponentInput input = new ComponentInput();
            input.AddInputObject(mainpart);
            input.AddOneInputPosition(new Point(68000, 0, 1000));
            input.AddOneInputPosition(new Point(72000, 0, 3000));

            opening.SetComponentInput(input);
            var isInserted = opening.Insert();
            model.CommitChanges();
        }
    }
}
