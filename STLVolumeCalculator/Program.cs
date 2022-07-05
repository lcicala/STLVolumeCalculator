using STLVolumeCalculator;
using System.Diagnostics;

Stopwatch sw = new Stopwatch();
sw.Start();
Mesh mesh = Mesh.LoadFromSTL("prova.stl");

var volume = mesh.Volume;

sw.Stop();
Console.WriteLine("The volume is: {0:N2} mm2, calculated in {1} ms", volume, sw.ElapsedMilliseconds);
Console.ReadLine();