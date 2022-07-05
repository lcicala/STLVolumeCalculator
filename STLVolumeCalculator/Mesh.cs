using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace STLVolumeCalculator
{
    public class Mesh
    {
        public List<Triangle> Triangles = new List<Triangle>();

        double _volume = double.NaN;

        public double Volume
        {
            get
            {
                if (double.IsNaN(_volume))
                    _volume = CalculateVolume(this);
                return _volume;
            }
        }

        static double SignedVolumeOfTriangle(Vector p1, Vector p2, Vector p3)
        {
            var v321 = p3.X * p2.Y * p1.Z;
            var v231 = p2.X * p3.Y * p1.Z;
            var v312 = p3.X * p1.Y * p2.Z;
            var v132 = p1.X * p3.Y * p2.Z;
            var v213 = p2.X * p1.Y * p3.Z;
            var v123 = p1.X * p2.Y * p3.Z;
            return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
        }

        static double CalculateVolume(Mesh mesh)
        {
            var vols = from t in mesh.Triangles
                       select SignedVolumeOfTriangle(t.P1, t.P2, t.P3);
            return Math.Abs(vols.Sum());
        }

        public static Mesh LoadFromSTL(string PathToSTL)
        {
            Mesh mesh = new Mesh();
            var STL = File.ReadAllText(PathToSTL);
            Regex ascii = new Regex("solid ASCII");

            if (ascii.IsMatch(STL))
            {
                Regex facet = new Regex("facet.*\\n.*\\n.*\\n.*\\n.*\\n.*\\n.*endfacet");
                Regex vertex = new Regex("vertex\\s*([0-9.e+\\-]+) ([0-9.e+\\-]+) ([0-9.e+\\-]+)");
                Regex normal = new Regex("normal\\s*([0-9.e+\\-]+) ([0-9.e+\\-]+) ([0-9.e+\\-]+)");

                var facets = facet.Matches(STL);
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                foreach (Match f in facets)
                {
                    // [0] Normal
                    // [1][2][3] P1 P2 P3
                    double[] Xs = new double[4];
                    double[] Ys = new double[4];
                    double[] Zs = new double[4];

                    Match n = normal.Match(f.Value);
                    byte i = 0;
                    Xs[i] = double.Parse(n.Groups[1].Value);
                    Ys[i] = double.Parse(n.Groups[2].Value);
                    Zs[i] = double.Parse(n.Groups[3].Value);
                    i++;

                    foreach (Match v in vertex.Matches(f.Value))
                    {
                        Xs[i] = double.Parse(v.Groups[1].Value);
                        Ys[i] = double.Parse(v.Groups[2].Value);
                        Zs[i] = double.Parse(v.Groups[3].Value);
                        i++;
                    }
                    Vector norm = new Vector(Xs[0], Ys[0], Zs[0]);
                    Vector P1 = new Vector(Xs[1], Ys[1], Zs[1]);
                    Vector P2 = new Vector(Xs[2], Ys[2], Zs[2]);
                    Vector P3 = new Vector(Xs[3], Ys[3], Zs[3]);
                    mesh.Triangles.Add(new Triangle(P1, P2, P3, norm));
                }
            }
            else
            {
                BinaryReader binaryReader = new BinaryReader(File.OpenRead(PathToSTL));
                binaryReader.ReadBytes(80);
                var numberOfTriangles = binaryReader.ReadInt32();
                double[] Xs = new double[4];
                double[] Ys = new double[4];
                double[] Zs = new double[4];
                for (int i = 0; i < numberOfTriangles; i++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Xs[k] = binaryReader.ReadSingle();
                        Ys[k] = binaryReader.ReadSingle();
                        Zs[k] = binaryReader.ReadSingle();
                    }
                    binaryReader.ReadUInt16();

                    Vector norm = new Vector(Xs[0], Ys[0], Zs[0]);
                    Vector P1 = new Vector(Xs[1], Ys[1], Zs[1]);
                    Vector P2 = new Vector(Xs[2], Ys[2], Zs[2]);
                    Vector P3 = new Vector(Xs[3], Ys[3], Zs[3]);
                    mesh.Triangles.Add(new Triangle(P1, P2, P3, norm));
                }
            }
            return mesh;
        }

    }
}
