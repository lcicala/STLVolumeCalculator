﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STLVolumeCalculator
{
    public struct Vector
    {
        public double X;
        public double Y;
        public double Z;

        public Vector(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
}
