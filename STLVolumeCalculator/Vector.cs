using LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLVolumeCalculator
{
    public class Vector3 : Vector
    {
        public Vector3() : base(3)
        {

        }

        public Vector3(double X, double Y, double Z) : base(3)
        {
            this[0] = X;
            this[1] = Y;
            this[2] = Z;
        }

        public double X { get => m_Values[0]; }
        public double Y { get => m_Values[1]; }
        public double Z { get => m_Values[2]; }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            Vector3 result = new Vector3();
            for (int i = 0; i < v1.Dimension; i++)
            {
                result[i] = v1[i] - v2[i];
            }
            return result;
        }

        // Cross product
        public static Vector3 operator |(Vector3 u, Vector3 v)
        {
            return new Vector3(u.Y * v.Z - (v.Y * u.Z), -(u.X * v.Z - v.X * u.Z), u.X * v.Y - v.X * u.Y);
        }
    }
}
