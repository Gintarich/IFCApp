using IFCApp.Core.Elements;
using IFCApp.IFCServices.Services;
using IFCApp.IFCServices.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;

namespace IFCApp.IFCServices
{
    public class IFCModel
    {

        private readonly string _fileName;
        private readonly IfcStore _model;
        private readonly TransformationService _tService = new TransformationService();
        private readonly BBoxService _bBoxService = new BBoxService();

        public IFCModel(string filePath)
        {
            _model = IfcStore.Open(filePath);
            _fileName = filePath;
        }
        public IFCModel(string filePath, TransformationService tService, BBoxService bService)
        {
            _model = IfcStore.Open(filePath);
            _fileName = filePath;
            _tService = tService;
            _bBoxService = bService;
        }
        public IfcStore GetModel()
        {
            return _model;
        }
        public void GetTransformation()
        {
        }
        public List<Wall> GetWalls()
        {
            var ifcWalls = _model.Instances.OfType<IfcWall>().ToList();
            var walls = new List<Wall>();
            foreach (var ifcWall in ifcWalls)
            {
                var tform = _tService.GetTransformation(ifcWall.ObjectPlacement);
                var box = _bBoxService.GetBBox(ifcWall.Representation, tform);
                var wallPanel = new Wall(box, tform);
                wallPanel.ID = ifcWall.GlobalId;
                walls.Add(wallPanel);
            }
            return walls;
        }
        public List<HvacOpening> GetHvacOpenings(List<string> names)
        {
            var ifcOpenings = _model.Instances.OfType<IfcOpeningElement>();
            var openings = new List<HvacOpening>();
            foreach (var opening in ifcOpenings)
            {
                bool found = false;
                foreach (var name in names)
                {
                    if (opening.Name.ToString().Contains(name))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) continue;

                var wallPlacement = opening.VoidsElements.RelatingBuildingElement.ObjectPlacement;
                var wallTform = _tService.GetTransformation(wallPlacement);

                var item = opening.Representation.Representations[0].Items[0];
                bool isCircle = false;
                if (item is IfcFacetedBrep brep)
                {
                    isCircle = brep.Outer.CfsFaces.Count > 20;
                }
                var tform = _tService.GetTransformation(opening.ObjectPlacement);
                var box = _bBoxService.GetGlobalBox(opening.Representation, tform);

                var localBox = wallTform.ToLocal(box);

                var hvacOpening = new HvacOpening(localBox);
                hvacOpening.IsCircle = isCircle;
                hvacOpening.ID = opening.GlobalId;
                openings.Add(hvacOpening);
            }

            return openings;
        }
    }
}
