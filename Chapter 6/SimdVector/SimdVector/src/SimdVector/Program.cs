using System;
using System.Numerics;

namespace SimdVector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (Vector.IsHardwareAccelerated)
            {
                Console.WriteLine("Vector IS hardware accelerated!");
                Console.WriteLine();
            }

            var vectorf = new Vector<float>(11f);
            Console.WriteLine(vectorf);
            Console.WriteLine(Vector.SquareRoot(vectorf));

            var vectord = new Vector<double>(11f);
            Console.WriteLine(vectord);
            Console.WriteLine(Vector.SquareRoot(vectord));
            Console.WriteLine();

            var vector3d = new Vector3(0f, 1f, 2f);
            Console.WriteLine(vector3d);
            Console.WriteLine(Vector3.SquareRoot(vector3d));
            Console.WriteLine();

            // N.B. Not all operations are equivalent between complex and vector maths
            var complex = new Complex(1d, 2d);
            Console.WriteLine(complex);
            Console.WriteLine(Complex.Add(complex, complex));

            var vectorComplexSingle = new Vector2(1f, 2f);
            Console.WriteLine(vectorComplexSingle);
            Console.WriteLine(Vector2.Add(vectorComplexSingle, vectorComplexSingle));

            var vectorComplexDouble = new Vector<double>(new[] { 1d, 2d });
            Console.WriteLine(vectorComplexDouble);
            Console.WriteLine(Vector.Add(vectorComplexDouble, vectorComplexDouble));

            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }
    }
}
