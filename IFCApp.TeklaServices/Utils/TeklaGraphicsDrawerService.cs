using IFCApp.Core.Elements;
using IFCApp.Core.Geometry;
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
        var cube = CreateCube(center, 100);
        gd.DrawMeshSurface(cube, new Color(1, 0, 0));
    }
    public void DrawBox(BBox box)
    {
        GraphicsDrawer gd = new GraphicsDrawer();
        var bbox = CreateBox(box);
        var result = gd.DrawMeshSurface(bbox, new Color(1, 0, 0));
    }
    public void DrawCylinder(Point startPoint, Point endPoint, double diameter)
    {
        GraphicsDrawer gd = new GraphicsDrawer();
        var cylinder = CreateCylinder(startPoint, endPoint, diameter);
        var result = gd.DrawMeshSurface(cylinder, new Color(1, 0, 0));
    }
    public void DrawBox(BBox box, Color c)
    {
        GraphicsDrawer gd = new GraphicsDrawer();
        var bbox = CreateBox(box);
        var result = gd.DrawMeshSurface(bbox, c);
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

    public Mesh CreateBox(BBox box)
    {
        // Define the 8 corner points of the cube
        var vertices = new List<Point>
        {
            new Point(box.GetMin().X,box.GetMin().Y,box.GetMin().Z), // Point 0
            new Point(box.GetMax().X, box.GetMin().Y,box.GetMin().Z), // Point 1
            new Point(box.GetMax().X, box.GetMax().Y, box.GetMin().Z), // Point 2
            new Point(box.GetMin().X,box.GetMax().Y,box.GetMin().Z), // Point 3
            new Point(box.GetMin().X,box.GetMin().Y,box.GetMax().Z), // Point 4
            new Point(box.GetMax().X,box.GetMin().Y,box.GetMax().Z), // Point 5
            new Point(box.GetMax().X,box.GetMax().Y,box.GetMax().Z), // Point 6
            new Point(box.GetMin().X,box.GetMax().Y,box.GetMax().Z)  // Point 7
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
    public Mesh CreateCylinder(Point startPoint, Point endPoint, double diameter)
    {
        int segments = 36; // Number of segments to approximate the circle
        double radius = diameter / 2.0;
        double height = Distance(startPoint, endPoint);

        // Calculate the direction vector from startPoint to endPoint
        Vector direction = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z);
        direction.Normalize();

        // Calculate the orthogonal vectors for the circle
        Vector ortho1 = new Vector(-direction.Y, direction.X, 0);
        if (ortho1.GetLength() == 0)
        {
            ortho1 = new Vector(0, -direction.Z, direction.Y);
        }
        ortho1.Normalize();
        Vector ortho2 = direction.Cross(ortho1);
        ortho2.Normalize();

        // Create the vertices for the top and bottom circles
        List<Point> vertices = new List<Point>();
        for (int i = 0; i < segments; i++)
        {
            double angle = 2 * Math.PI * i / segments;
            double x = radius * Math.Cos(angle);
            double y = radius * Math.Sin(angle);

            Point bottomVertex = new Point(
                startPoint.X + x * ortho1.X + y * ortho2.X,
                startPoint.Y + x * ortho1.Y + y * ortho2.Y,
                startPoint.Z + x * ortho1.Z + y * ortho2.Z
            );
            Point topVertex = new Point(
                endPoint.X + x * ortho1.X + y * ortho2.X,
                endPoint.Y + x * ortho1.Y + y * ortho2.Y,
                endPoint.Z + x * ortho1.Z + y * ortho2.Z
            );

            vertices.Add(bottomVertex);
            vertices.Add(topVertex);
        }

        // Create the mesh
        Mesh mesh = new Mesh();

        // Add vertices to the mesh
        foreach (var vertex in vertices)
        {
            mesh.AddPoint(vertex);
        }

        // Add triangles for the sides
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            mesh.AddTriangle(i * 2, next * 2, i * 2 + 1);
            mesh.AddTriangle(next * 2, next * 2 + 1, i * 2 + 1);
        }

        // Add triangles for the top and bottom faces
        for (int i = 1; i < segments - 1; i++)
        {
            mesh.AddTriangle(0, (i + 1) * 2, i * 2);
            mesh.AddTriangle(1, i * 2 + 1, (i + 1) * 2 + 1);
        }

        return mesh;
    }
    private double Distance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2) + Math.Pow(p2.Z - p1.Z, 2));
    }

    public void DrawOpening(Wall wall)
    {
        var openings = wall.Openings;

        foreach (var opening in openings)
        {
            var box = opening.Box;
            DrawBox(box);
        }
    }
    public void DrawOpenings(List<Wall> walls)
    {
        foreach (var wall in walls)
        {
            DrawOpening(wall);
        }
    }
}

