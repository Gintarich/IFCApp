﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Geometry
{
    public class BBox
    {
        public Point3d Min { get => GetMin(); }
        public Point3d Max { get => GetMax(); }
        public Matrix4d CS { get => _coordinateSystem; }
        private Matrix4d _coordinateSystem = new();
        private Point3d _max;
        private Point3d _min;


        public BBox(List<Point3d> Points)
        {
            var minMax = CreateBox(Points);
            _min = minMax.Min;
            _max = minMax.Max;
        }
        public BBox(List<Point3d> Points, Matrix4d CoordinateSystem) : this(Points)
        {
            _coordinateSystem = CoordinateSystem;
        }
        public BBox TrimBoxWidth(double thickness)
        {
            var halfBox = thickness / 2;
            if (_min.Y < -halfBox)
            {
                _min = new Point3d(_min.X, -halfBox, _min.Z);
            }
            if (_max.Y > halfBox)
            {
                _max = new Point3d(_max.X, halfBox, _max.Z);
            }
            return this;
        }
        public BBox TrimBoxLength(double length)
        {
            var halfBox = length / 2;
            if (_min.X < -halfBox)
            {
                _min = new Point3d(-halfBox, _min.Y, _min.Z);
            }
            if (_max.X > halfBox)
            {
                _max = new Point3d(halfBox, _max.Y, _max.Z);
            }
            return this;
        }
        public BBox TrimBoxHeight(double height)
        {
            var halfBox = height / 2;
            if (_min.Z < -halfBox)
            {
                _min = new Point3d(_min.X, _min.Y, -halfBox);
            }
            if (_max.Z > halfBox)
            {
                _max = new Point3d(_min.X, _max.Y, halfBox);
            }
            return this;
        }
        public BBox SetHeight(double height)
        {
            _max.Z = height;
            return this;
        }

        /// <summary>
        /// Checks if this bounding box intersects or touches another bounding box.
        /// </summary>
        /// <param name="other">The other bounding box.</param>
        /// <returns>True if the boxes intersect or touch; otherwise, false.</returns>
        public bool Intersects(BBox other)
        {
            // Get the min and max points of both bounding boxes
            Point3d thisMin = this.Min;
            Point3d thisMax = this.Max;
            Point3d otherMin = other.Min;
            Point3d otherMax = other.Max;

            // Check for overlap in each dimension
            bool xOverlap = thisMin.X <= otherMax.X && thisMax.X >= otherMin.X;
            bool yOverlap = thisMin.Y <= otherMax.Y && thisMax.Y >= otherMin.Y;
            bool zOverlap = thisMin.Z <= otherMax.Z && thisMax.Z >= otherMin.Z;

            // Boxes intersect if they overlap in all three dimensions
            return xOverlap && yOverlap && zOverlap;
        }
        private (Point3d Min, Point3d Max) CreateBox(List<Point3d> Points)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double minZ = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            double maxZ = double.MinValue;

            foreach (var point in Points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                minZ = Math.Min(minZ, point.Z);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
                maxZ = Math.Max(maxZ, point.Z);
            }
            var min = new Point3d(minX, minY, minZ);
            var max = new Point3d(maxX, maxY, maxZ);
            return (min, max);
        }
        private Point3d GetMin()
        {
            var pt1 = _coordinateSystem.Apply(_min);
            var pt2 = _coordinateSystem.Apply(_max);
            return CreateBox([pt1, pt2]).Min;
        }
        private Point3d GetMax()
        {
            var pt1 = _coordinateSystem.Apply(_min);
            var pt2 = _coordinateSystem.Apply(_max);
            return CreateBox([pt1, pt2]).Max;
        }

        public bool IsParallel(BBox other)
        {
            var thisYAxis = _coordinateSystem.YAxis;
            var otherYAxis = other.CS.YAxis;
            return Math.Abs(thisYAxis.Dot(otherYAxis)) > 0.99;
        }
        public double OverlapVolume(BBox other)
        {
            // Get the transformed (global) min and max points for both boxes.
            Point3d thisMin = this.GetMin();
            Point3d thisMax = this.GetMax();
            Point3d otherMin = other.GetMin();
            Point3d otherMax = other.GetMax();


            // Calculate the overlapping extents along the X, Y, and Z axes.
            double overlapX = Math.Max(0, Math.Min(thisMax.X, otherMax.X) - Math.Max(thisMin.X, otherMin.X));
            double overlapY = Math.Max(0, Math.Min(thisMax.Y, otherMax.Y) - Math.Max(thisMin.Y, otherMin.Y));
            double overlapZ = Math.Max(0, Math.Min(thisMax.Z, otherMax.Z) - Math.Max(thisMin.Z, otherMin.Z));

            // The overlapping volume is the product of the overlaps in X, Y, and Z.
            return overlapX * overlapY * overlapZ;
        }
    }
}
