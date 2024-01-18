using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

        static double SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
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

        public string ExportSTLA()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("solid ASCII");
            foreach (var triangle in Triangles)
            {
                stringBuilder.AppendFormat("\tfacet normal {0} {1} {2}\n\t\touter loop\n\t\t\tvertex {3} {4} {5}\n\t\t\tvertex {6} {7} {8}\n\t\t\tvertex {9} {10} {11}\n\t\tendloop\n\tendfacet\n",
                    triangle.Normal.X, triangle.Normal.Y, triangle.Normal.Z,
                    triangle.P1.X, triangle.P1.Y, triangle.P1.Z,
                    triangle.P2.X, triangle.P2.Y, triangle.P2.Z,
                    triangle.P3.X, triangle.P3.Y, triangle.P3.Z);
            }
            stringBuilder.Append("endsolid");
            return stringBuilder.ToString();
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
                    Vector3 norm = new Vector3(Xs[0], Ys[0], Zs[0]);
                    Vector3 P1 = new Vector3(Xs[1], Ys[1], Zs[1]);
                    Vector3 P2 = new Vector3(Xs[2], Ys[2], Zs[2]);
                    Vector3 P3 = new Vector3(Xs[3], Ys[3], Zs[3]);
                    mesh.Triangles.Add(new Triangle(P1, P2, P3, norm));
                }
            }
            else
            {
                using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(PathToSTL)))
                { 
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

                        Vector3 norm = new Vector3(Xs[0], Ys[0], Zs[0]);
                        Vector3 P1 = new Vector3(Xs[1], Ys[1], Zs[1]);
                        Vector3 P2 = new Vector3(Xs[2], Ys[2], Zs[2]);
                        Vector3 P3 = new Vector3(Xs[3], Ys[3], Zs[3]);
                        mesh.Triangles.Add(new Triangle(P1, P2, P3, norm));
                    }
                }
            }
            return mesh;
        }

    }
}
