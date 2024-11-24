using System;
using System.Collections.Generic;
using System.Text;

namespace IFCApp.Core.Geometry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Vector3d"/> class.
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
    /// <param name="z">The Z component of the vector.</param>
    public struct Vector3d(double x = 0, double y = 0, double z = 0)
    {
        /// <summary>
        /// Gets or sets the X component of the vector.
        /// </summary>
        public double X { get; set; } = x;

        /// <summary>
        /// Gets or sets the Y component of the vector.
        /// </summary>
        public double Y { get; set; } = y;

        /// <summary>
        /// Gets or sets the Z component of the vector.
        /// </summary>
        public double Z { get; set; } = z;

        readonly public static Vector3d xAxis = new Vector3d(1, 0, 0);
        readonly public static Vector3d yAxis = new Vector3d(0, 1, 0);
        readonly public static Vector3d zAxis = new Vector3d(0, 0, 1);

        /// <summary>
        /// Calculates the magnitude (length) of the vector.
        /// </summary>
        /// <returns>The magnitude of the vector.</returns>
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Returns a normalized version of the vector (unit vector).
        /// </summary>
        /// <returns>A new <see cref="Vector3d"/> representing the normalized vector.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the vector is zero.</exception>
        public Vector3d Normalize()
        {
            double magnitude = Length();
            if (magnitude == 0)
                throw new InvalidOperationException("Cannot normalize a zero vector.");
            return new Vector3d(X / magnitude, Y / magnitude, Z / magnitude);
        }

        /// <summary>
        /// Calculates the dot product of this vector with another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The dot product as a double.</returns>
        public double Dot(Vector3d other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        /// <summary>
        /// Calculates the cross product of this vector with another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>A new <see cref="Vector3d"/> representing the cross product.</returns>
        public Vector3d Cross(Vector3d other)
        {
            return new Vector3d(
                Y * other.Z - Z * other.Y,
                Z * other.X - X * other.Z,
                X * other.Y - Y * other.X
            );
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The resulting vector after addition.</returns>
        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Subtracts one vector from another.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector to subtract from the first.</param>
        /// <returns>The resulting vector after subtraction.</returns>
        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            return new Vector3d(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <param name="scalar">The scalar value.</param>
        /// <returns>The resulting vector after multiplication.</returns>
        public static Vector3d operator *(Vector3d a, double scalar)
        {
            return new Vector3d(a.X * scalar, a.Y * scalar, a.Z * scalar);
        }

        /// <summary>
        /// Multiplies a scalar by a vector.
        /// </summary>
        /// <param name="scalar">The scalar value.</param>
        /// <param name="a">The vector.</param>
        /// <returns>The resulting vector after multiplication.</returns>
        public static Vector3d operator *(double scalar, Vector3d a)
        {
            return new Vector3d(a.X * scalar, a.Y * scalar, a.Z * scalar);
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <param name="scalar">The scalar value.</param>
        /// <returns>The resulting vector after division.</returns>
        /// <exception cref="DivideByZeroException">Thrown if the scalar is zero.</exception>
        public static Vector3d operator /(Vector3d a, double scalar)
        {
            if (scalar == 0)
                throw new DivideByZeroException("Cannot divide by zero.");
            return new Vector3d(a.X / scalar, a.Y / scalar, a.Z / scalar);
        }

        /// <summary>
        /// Returns a string representation of the vector.
        /// </summary>
        /// <returns>A string in the format "(X, Y, Z)".</returns>
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}
