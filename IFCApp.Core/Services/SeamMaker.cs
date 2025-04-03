using IFCApp.Core.DetailComponents;
using IFCApp.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Services
{
    public class SeamMaker
    {
        private readonly Model _model;
        public double Anchorage { get; set; }
        public DowelPointGenerator Dpg { get; set; } = new DowelPointGenerator();
        public SeamMaker(Model model)
        {
            _model = model;
        }

        public void GenerateSeams()
        {
            var walls = _model.Elements.Where(x => x is Wall).Cast<Wall>().ToList();
            var slabs = _model.Elements.Where(x => x is Slab).Cast<Slab>().ToList();
            foreach (var wall in walls)
            {
                FindNeighborWalls(wall, walls);
                FindNeighborSlabs(wall, slabs);
                GeneratePoints(wall);
            }
            foreach (var wall in walls)
            {
                GenerateAnchors(wall);
            }
        }


        public void ClearSeams()
        {

        }

        /// <summary>
        /// Adds anchor points to wall
        /// </summary>
        /// <param name="wall">The wall,which will have anchors added</param>
        /// <param name="refWall">The wall to use as reference</param>
        private void AddAnchorsToWall(Wall wall, Wall refWall)
        {
            if (wall.TopSeam is null) wall.TopSeam = new TopDowelSeam();
            var pts = refWall.BottomSeam.Points;
            var box = wall.Box;
            var min = box.Min;
            var max = box.Max;
            var midy = (box.Max.Y + box.Min.Y) / 2;
            var inverse = wall.Box.CS.Inverse();
            foreach (var pt in pts)
            {
                var globalPt = refWall.Box.CS.Apply(pt);
                globalPt.Z = wall.Box.GetMax().Z;
                if (box.ContainsXY(globalPt))
                {
                    var local = inverse.Apply(globalPt);
                    local.Y = midy;
                    wall.TopSeam.Points.Add(local);
                }
            }
        }

        /// <summary>
        /// Adds anchor points to wall
        /// </summary>
        /// <param name="slab">The slab,which will have anchors added</param>
        /// <param name="wall">The wall to use as reference</param>
        private void AddAnchorsToSlab(Wall wall, Slab slab)
        {
            var pts = wall.BottomSeam.Points;
            var box = slab.Box;
            slab.Anchors.Anchorage = 150;
            slab.Anchors.FreeLength = 275;
            foreach (var pt in pts)
            {
                var globalPt = wall.Box.CS.Apply(pt);
                if (box.ContainsXY(globalPt))
                {
                    slab.Anchors.Points.Add(globalPt);
                    
                }
            }
        }

        private void GenerateAnchors(Wall wall)
        {
            var elementIds = wall.BottomSeam.RelatingElements;
            foreach (var elementId in elementIds)
            {
                if (!_model.TryGetValue(elementId, out var element)) continue;
                if (element is Wall otherWall)
                {
                    AddAnchorsToWall(otherWall, wall);
                }
                else if (element is Slab otherSlab)
                {
                    AddAnchorsToSlab(wall, otherSlab);
                }
            }
        }

        private void GeneratePoints(Wall wall)
        {
            var pts = Dpg.Generate(wall);
            wall.BottomSeam.Points = pts;
        }

        private void FindNeighborSlabs(Wall currentWall, List<Slab> slabs)
        {

            if (currentWall.BottomSeam == null) currentWall.BottomSeam = new BottomDowelSeam();
            foreach (var slab in slabs)
            {
                if (slab.Box.IsUnderNotParallel(currentWall.Box, 300))
                {
                    currentWall.BottomSeam.RelatingElements.Add(slab.ID);
                }
            }
        }

        private void FindNeighborWalls(Wall currentWall, List<Wall> walls)
        {
            if (currentWall.BottomSeam == null) currentWall.BottomSeam = new BottomDowelSeam();
            foreach (var wall in walls)
            {
                if (wall.Box.IsUnder(currentWall.Box, 300))
                {
                    currentWall.BottomSeam.RelatingElements.Add(wall.ID);
                }
            }
        }
    }
}
