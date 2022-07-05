using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLVolumeCalculator
{
    public struct Triangle
    {
        public Vector P1;
        public Vector P2;
        public Vector P3;
        public Vector Normal;

        public Triangle(Vector P1, Vector P2, Vector P3, Vector Normal)
        {
            this.P1 = P1;
            this.P2 = P2;
            this.P3 = P3;
            this.Normal = Normal;
        }
    }
}
