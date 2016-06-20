using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace BenchmarkingDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var pbkdf2 = new Rfc2898DeriveBytes("password", 64, 256);
            SingleTest(pbkdf2);
            SingleTest(pbkdf2);
            SingleTest(pbkdf2);

            Console.WriteLine();
            var tests = 1000;
            AvgTest(pbkdf2, tests);
            AvgTest(pbkdf2, tests);
            AvgTest(pbkdf2, tests);

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }

        private static void SingleTest(Rfc2898DeriveBytes pbkdf2)
        {
            var s = Stopwatch.StartNew();
            pbkdf2.GetBytes(2048);
            s.Stop();
            Console.WriteLine($"Test duration = {s.ElapsedMilliseconds} ms");
        }

        private static void AvgTest(Rfc2898DeriveBytes pbkdf2, int tests)
        {
            // Prevent needless work and potential divide by zero later
            if (tests < 1)
            {
                return;
            }

            var s = Stopwatch.StartNew();
            for (var ii = 0; ii < tests; ii++) // ii is easier to grep than i
            {
                pbkdf2.GetBytes(2048);
            }
            s.Stop();
            var mean = s.ElapsedMilliseconds / tests;
            Console.WriteLine($"{tests} runs mean duration = {mean} ms");
        }
    }
}
