
using System;
using System.Drawing;

namespace Qoden.Util
{
    /// <summary>
    /// This is a matrix limited to 2D coordinates.
    /// A matrix represents a coordinate system.
    /// Different kinds of transformations can be combined with multiplication.
    /// It has only 6 entries because the last row with 0 0 1 is constant.
    /// </summary>
    public class Matrix2d
    {
        public double[] Elements = new double[6];

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix2d"/> class.
        /// </summary>
        public Matrix2d()
        {
            Elements[0] = 1;
            Elements[4] = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix2d"/> class.
        /// </summary>
        /// <param name='m11'>
        /// The element in 1st row and 1st column.
        /// </param>
        /// <param name='m12'>
        /// The element in 1st row and 2nd column.
        /// </param>
        /// <param name='m13'>
        /// The element in 1st row and 3rd column.
        /// This element contains the translation in the x-direction.
        /// </param>
        /// <param name='m21'>
        /// The element in 2nd row and 1st column.
        /// </param>
        /// <param name='m22'>
        /// The element in 2nd row and 2nd column.
        /// </param>
        /// <param name='m23'>
        /// The element in 2nd row nad 3rd column.
        /// This element contains the translation in the y-direction.
        /// </param>
        public Matrix2d(double m11, double m12, double m13, double m21, double m22, double m23)
        {
            Elements[0] = m11;
            Elements[1] = m12;
            Elements[2] = m13;
            Elements[3] = m21;
            Elements[4] = m22;
            Elements[5] = m23;
        }

        /// <summary>
        /// Determines whether this matrix is identity.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this matrix is identity; otherwise, <c>false</c>.
        /// </returns>
        public bool IsIdentity()
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            return Elements[0] == 1 &&
                    Elements[1] == 0 &&
                    Elements[2] == 0 &&
                    Elements[3] == 0 &&
                    Elements[4] == 1 &&
                    Elements[5] == 0;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
        }

        // Creates a translation matrix.
        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name='x'>
        /// How much to translate in the x direction.
        /// </param>
        /// <param name='y'>
        /// How much to translate in the y direction.
        /// </param>
        public static Matrix2d Translation(double x, double y)
        {
            var mat = new Matrix2d();
            mat.Elements[2] = x;
            mat.Elements[5] = y;
            return mat;
        }

        // Creates a rotation matrix.
        /// <summary>
        /// Creates a rotation matrix in degrees specified by angle.
        /// </summary>
        /// <param name='angle'>
        /// The angle of rotation in degrees.
        /// </param>
        public static Matrix2d Rotation(double angle)
        {
            var mat = new Matrix2d();
            double cos = Math.Cos(angle * Math.PI / 180.0);
            double sin = Math.Sin(angle * Math.PI / 180.0);
            mat.Elements[0] = cos;
            mat.Elements[1] = -sin;
            mat.Elements[3] = sin;
            mat.Elements[4] = cos;
            return mat;
        }

        // Creates a scale matrix with one factor.
        /// <summary>
        /// Creates a scaling matrix with equal stretching in x and y direction.
        /// For direction dependent scaling, use "Stretch".
        /// </summary>
        /// <param name='s'>
        /// The scaling factor.
        /// </param>
        public static Matrix2d Scale(double s)
        {
            return new Matrix2d(s, 0, 0, 0, s, 0);
        }

        /// <summary>
        /// Stretch the specified matrix by sx and sy.
        /// </summary>
        /// <param name='sx'>
        /// Stretching in the x direction.
        /// </param>
        /// <param name='sy'>
        /// Stretching in the y direction.
        /// </param>
        public static Matrix2d Stretch(double sx, double sy)
        {
            return new Matrix2d(sx, 0, 0, 0, sy, 0);
        }

        /// <summary>
        /// Creates a rotation matrix by radians.
        /// </summary>
        /// <returns>
        /// Returns a rotation matrix.
        /// </returns>
        /// <param name='angle'>
        /// The angle of rotation in radians.
        /// </param>
        public static Matrix2d RotationByRadians(double angle)
        {
            var mat = new Matrix2d();
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            mat.Elements[0] = cos;
            mat.Elements[1] = -sin;
            mat.Elements[3] = sin;
            mat.Elements[4] = cos;
            return mat;
        }

        /// <summary>
        /// Composes two matrices together to form a new matrix.
        /// This equals combining two coordinate systems into a new one.
        /// </summary>
        /// <param name='a'>
        /// The child matrix.
        /// </param>
        /// <param name='b'>
        /// The parent matrix.
        /// </param>
        public static Matrix2d Multiply(Matrix2d child, Matrix2d parent)
        {
            var mat = new Matrix2d();
            mat.Elements[0] = child.Elements[0] * parent.Elements[0] + child.Elements[1] * parent.Elements[3];
            mat.Elements[1] = child.Elements[0] * parent.Elements[1] + child.Elements[1] * parent.Elements[4];
            mat.Elements[2] = child.Elements[0] * parent.Elements[2] + child.Elements[1] * parent.Elements[5] + child.Elements[2];
            mat.Elements[3] = child.Elements[3] * parent.Elements[0] + child.Elements[4] * parent.Elements[3];
            mat.Elements[4] = child.Elements[3] * parent.Elements[1] + child.Elements[4] * parent.Elements[4];
            mat.Elements[5] = child.Elements[3] * parent.Elements[2] + child.Elements[4] * parent.Elements[5] + child.Elements[5];
            return mat;
        }

        public static Matrix2d operator *(Matrix2d a, Matrix2d b)
        {
            return Matrix2d.Multiply(a, b);
        }

        /// <summary>
        /// Returns the determinant of the matrix.
        /// If non-zero, the matrix is invertible.
        /// </summary>
        public double Determinant()
        {
            return Elements[0] * Elements[4] - Elements[1] * Elements[3];
        }

        /// <summary>
        /// Finds out if the matrix is invertible or not.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this matrix is invertible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInvertible()
        {
            return Determinant() != 0;
        }

        /// <summary>
        /// Creates an inverted matrix that transforms coordinates the opposite way.
        /// </summary>
        public Matrix2d Inverted()
        {
            Matrix2d mat = new Matrix2d();
            double det = this.Determinant();

            mat.Elements[0] = Elements[4] / det;
            mat.Elements[1] = -Elements[1] / det;
            mat.Elements[2] = (Elements[1] * Elements[5] - Elements[4] * Elements[2]) / det;
            mat.Elements[3] = -Elements[3] / det;
            mat.Elements[4] = Elements[0] / det;
            mat.Elements[5] = (Elements[2] * Elements[3] - Elements[0] * Elements[5]) / det;

            return mat;
        }

        /// <summary>
        /// Finds out whether the matrix is a translating matrix or not.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this matrix is translating; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTranslating()
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            return Elements[0] == 1
                && Elements[1] == 0
                && Elements[3] == 0
                && Elements[4] == 1;
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
        }

        /// <summary>
        /// Writes the elements of matrix into a float array.
        /// </summary>
        /// <param name='destination'>
        /// The array to write to.
        /// </param>
        public void ToFloatArray(float[] destination)
        {
            destination[0] = (float)Elements[0];
            destination[1] = (float)Elements[1];
            destination[2] = (float)Elements[2];
            destination[3] = (float)Elements[3];
            destination[4] = (float)Elements[4];
            destination[5] = (float)Elements[5];
        }

        public PointF Transform(PointF point)
        {
            return new PointF((float)(Elements[0] * point.X + Elements[1] * point.Y + Elements[2]),
                              (float)(Elements[3] * point.X + Elements[4] * point.Y + Elements[5]));
        }

        public RectangleF Transform(RectangleF rect)
        {
            var lt = Transform(new PointF(rect.Left, rect.Top));
            var rb = Transform(new PointF(rect.Right, rect.Bottom));

            PointF newLt, newRb;
            if (Math.Abs(lt.X) < Math.Abs(rb.X))
            {
                newLt.X = lt.X;
                newRb.X = rb.X;
            }
            else
            {
                newLt.X = rb.X;
                newRb.X = lt.X;
            }
            if (Math.Abs(lt.Y) < Math.Abs(rb.Y))
            {
                newLt.Y = lt.Y;
                newRb.Y = rb.Y;
            }
            else
            {
                newLt.Y = rb.Y;
                newRb.Y = lt.Y;
            }
            var width = Math.Abs(newLt.X - newRb.X);
            var height = Math.Abs(newLt.Y - newRb.Y);
            return new RectangleF(newLt.X, newLt.Y, width, height);
        }
    }
}