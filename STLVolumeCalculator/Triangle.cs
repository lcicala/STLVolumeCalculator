using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLVolumeCalculator
{
    public struct Triangle
    {
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;
        public Vector3 Normal;

        public Triangle(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 Normal)
        {
            this.P1 = P1;
            this.P2 = P2;
            this.P3 = P3;
            this.Normal = Normal;
        }
    }
}
