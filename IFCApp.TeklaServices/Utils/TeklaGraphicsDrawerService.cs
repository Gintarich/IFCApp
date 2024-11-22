using System;
using System.Collections.Generic;
using System.Text;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;

namespace IFCApp.TeklaServices.Utils;

public class TeklaGraphicsDrawerService
{
    public void DrawCube(Point center)
    {
        GraphicsDrawer gd = new GraphicsDrawer();
        var cube = CreateCube(new Point(0, 0, 0), 100);
        gd.DrawMeshSurface(cube,new Color(1,0,0) );
    }

    public Mesh CreateCube(Point center, double edgeLength)
    {
        // Half of the edge length
        double halfEdge = edgeLength / 2.0;

        // Define the 8 corner points of the cube
        var vertices = new List<Point>
        {
            new Point(center.X - halfEdge, center.Y - halfEdge, center.Z - halfEdge), // Point 0
            new Point(center.X + halfEdge, center.Y - halfEdge, center.Z - halfEdge), // Point 1
            new Point(center.X + halfEdge, center.Y + halfEdge, center.Z - halfEdge), // Point 2
            new Point(center.X - halfEdge, center.Y + halfEdge, center.Z - halfEdge), // Point 3
            new Point(center.X - halfEdge, center.Y - halfEdge, center.Z + halfEdge), // Point 4
            new Point(center.X + halfEdge, center.Y - halfEdge, center.Z + halfEdge), // Point 5
            new Point(center.X + halfEdge, center.Y + halfEdge, center.Z + halfEdge), // Point 6
            new Point(center.X - halfEdge, center.Y + halfEdge, center.Z + halfEdge)  // Point 7
        };

        var edges = new List<(int, int)>
        {
            // Bottom edges
            (0, 1), (1, 2), (2, 3), (3, 0),
            // Top edges
            (4, 5), (5, 6), (6, 7), (7, 4),
            // Vertical edges
            (0, 4), (1, 5), (2, 6), (3, 7)
        };

        // Define the faces of the cube using vertex indices
        // Each face is a triangle defined by three vertices
        var triangles = new List<int[]>
        {
            // Bottom face (0, 1, 2, 3)
            new[] {2, 1, 0 }, new[] {3, 2, 0 },
            // Top face (4, 5, 6, 7)
            new[] { 4, 5, 6 }, new[] { 4, 6, 7 },
            // Front face (0, 1, 5, 4)
            new[] { 0, 1, 5 }, new[] { 0, 5, 4 },
            // Back face (2, 3, 7, 6)
            new[] { 2, 3, 7 }, new[] { 2, 7, 6 },
            // Left face (0, 3, 7, 4)
            new[] { 7, 3, 0 }, new[] { 4, 7, 0 },
            // Right face (1, 2, 6, 5)
            new[] { 1, 2, 6 }, new[] { 1, 6, 5 }
        };

        // Create the Mesh object
        var mesh = new Mesh();

        // Add vertices to the mesh
        foreach (var vertex in vertices)
        {
            mesh.AddPoint(vertex);
        }

        // Add triangular faces to the mesh
        foreach (var triangle in triangles)
        {
            mesh.AddTriangle(triangle[0], triangle[1], triangle[2]);
        }

        foreach (var edge in edges)
        {
            mesh.AddLine(edge.Item1, edge.Item2);
        }

        return mesh; // Successfully created
    }
}
