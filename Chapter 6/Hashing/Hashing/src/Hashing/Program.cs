using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Hashing
{
    public class Program
    {
        private const int runs = 100000;
        private const string hashingData = "My short string of test data for hashing! ";
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Over {runs} runs:");

            var key = new byte[256];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);

            var algos = new SortedList<string, HashAlgorithm>
            {
                {"1.          MD5", MD5.Create()},      // MD5 is not secure!
                {"2.        SHA-1", SHA1.Create()},     // SHA-1 is known to be weak
                {"3.      SHA-256", SHA256.Create()},
                {"4.   HMAC SHA-1", new HMACSHA1(key)},
                {"5. HMAC SHA-256", new HMACSHA256(key)},
            };
            foreach (var algo in algos)
            {
                Console.ForegroundColor++;
                HashAlgorithmTest(algo);
            }
            Console.ForegroundColor++;

            // PBKDF2 is so slow that 100,000 runs would take some time
            var slowRuns = 10;
            var pbkdf2 = new Rfc2898DeriveBytes(hashingData, key, 10000);
            var pbkdf2Ms = RunTest(() =>
            {
                pbkdf2.GetBytes(256);
            }, slowRuns);
            Console.WriteLine($"6.       PBKDF2 average time {pbkdf2Ms / slowRuns} ms (over {slowRuns} runs)");

            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey(true); // don't display the pressed key (especially the 'any' key)
        }

        private static void HashAlgorithmTest(KeyValuePair<string, HashAlgorithm> algo)
        {
            var smallBytes = Encoding.UTF8.GetBytes(hashingData);
            var largeBytes = new byte[smallBytes.Length * 100];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(largeBytes);

            var smallTimeMs = RunTest(() =>
            {
                algo.Value.ComputeHash(smallBytes);
            }, runs);
            var largeTimeMs = RunTest(() =>
            {
                algo.Value.ComputeHash(largeBytes);
            }, runs);
            var avgSmallTimeMs = (double)smallTimeMs / runs;
            var avgLargeTimeMS = (double)largeTimeMs / runs;

            Console.WriteLine($"{algo.Key} average time {smallBytes.Length}B {avgSmallTimeMs:0.00000} ms {largeBytes.Length}B {avgLargeTimeMS:0.00000} ms");
        }
        private static long RunTest(Action func, int runs = 1000)
        {
            var s = Stopwatch.StartNew();
            for (int j = 0; j < runs; j++)
            {
                func();
            }
            s.Stop();
            return s.ElapsedMilliseconds;
        }
    }
}
