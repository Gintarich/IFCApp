using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Geometry;

public class Matrix4d
{
    private readonly double[,] _matrix;

    public Matrix4d()
    {
        _matrix = new double[,] {
            { 1, 0, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 1, 0 },
            { 0, 0, 0, 1 },
        };
    }
    // Constructor: Initializes with a custom matrix
    public Matrix4d(double[,] customMatrix)
    {
        if (customMatrix.GetLength(0) != 4 || customMatrix.GetLength(1) != 4)
            throw new ArgumentException("Matrix must be 4x4.");
        _matrix = customMatrix;
    }

    public Matrix4d(Matrix4d mat) : this(mat.GetData()) { }

    public double[,] GetData() { return _matrix; }

    /// <summary>
    /// Creates a transformation matrix for rotation around the X-axis.
    /// </summary>
    /// <param name="angle">The rotation angle in degrees.</param>
    /// <returns>A transformation matrix for the specified rotation.</returns>
    public static Matrix4d RotationX(double angle)
    {
        double c = Math.Cos(angle);
        double s = Math.Sin(angle);

        return new Matrix4d(new double[,]
        {
            { 1, 0,  0, 0 },
            { 0, c, -s, 0 },
            { 0, s,  c, 0 },
            { 0, 0,  0, 1 }
        });
    }

    
    /// <summary>
    /// Creates a transformation matrix for rotation around the X-axis.
    /// </summary>
    /// <param name="angle">The rotation angle in radians.</param>
    /// <returns>A transformation matrix for the specified rotation.</returns>
    public static Matrix4d RotationY(double angle)
    {
        double c = Math.Cos(angle);
        double s = Math.Sin(angle);

        return new Matrix4d(new double[,]
        {
            {  c, 0, s, 0 },
            {  0, 1, 0, 0 },
            { -s, 0, c, 0 },
            {  0, 0, 0, 1 }
        });
    }

    /// <summary>
    /// Creates a transformation matrix for rotation around the Z-axis.
    /// </summary>
    /// <param name="angle">The rotation angle in degrees.</param>
    /// <returns>A transformation matrix for the specified rotation.</returns>
    public static Matrix4d RotationZ(double angle)
    {
        var radians = angle * Math.PI / 180.0; 
        double c = Math.Cos(radians);
        double s = Math.Sin(radians);

        return new Matrix4d(new double[,]
        {
            { c, -s, 0, 0 },
            { s,  c, 0, 0 },
            { 0,  0, 1, 0 },
            { 0,  0, 0, 1 }
        });
    }

    // Static Method: Translation matrix
    public static Matrix4d Translation(double tx, double ty, double tz)
    {
        return new Matrix4d(new double[,]
        {
            { 1, 0, 0, tx },
            { 0, 1, 0, ty },
            { 0, 0, 1, tz },
            { 0, 0, 0,  1 }
        });
    }

    // Apply the transformation to a 3D vector
    public double[] Apply(double[] vector)
    {
        if (vector.Length != 3 && vector.Length != 4)
            throw new ArgumentException("Vector must have 3 or 4 elements.");

        double[] extendedVector = vector.Length == 3
            ? new[] { vector[0], vector[1], vector[2], 1.0 }
            : vector;

        double[] result = new double[4];
        for (int i = 0; i < 4; i++)
        {
            result[i] = 0;
            for (int j = 0; j < 4; j++)
            {
                result[i] += _matrix[i, j] * extendedVector[j];
            }
        }

        // Return the first three elements as a 3D vector
        return new[] { result[0], result[1], result[2] };
    }

    public Point3d Apply(Point3d point)
    {
        var vArray = this.Apply([point.X,point.Y,point.Z]);
        return new Point3d(vArray[0], vArray[1], vArray[2]);
    }

    // Combine this transformation with another
    public Matrix4d Combine(Matrix4d other)
    {
        double[,] result = new double[4, 4];

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                result[i, j] = 0;
                for (int k = 0; k < 4; k++)
                {
                    result[i, j] += this._matrix[i, k] * other.GetData()[k, j];
                }
            }
        }

        return new Matrix4d(result);
    }

    // Override ToString() for better debugging output
    public override string ToString()
    {
        string output = "";
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                output += $"{_matrix[i, j]:F2}\t";
            }
            output += "\n";
        }
        return output;
    }

};