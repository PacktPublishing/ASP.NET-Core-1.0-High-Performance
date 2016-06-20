using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelBenchmarking
{
    public class Program
    {
        private const int runs = 1000;
        private const int length = 100000;

        private const int hashRuns = 10;
        private const int iterations = 10000;

        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Testing over {runs} runs summing {length} integers...");
            var array = new int[length];
            var rng = new Random();
            for (int i = 0; i < length; i++)
            {
                array[i] = rng.Next(10);
            }
            var list = array.ToList();
            GC.Collect();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" Simple for loops");
            Console.WriteLine("  - Array");
            RunTest(() =>
            {
                var sum = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    sum += array[i];
                }
                return sum;
            });
            Console.WriteLine("  - List");
            RunTest(() =>
            {
                var sum = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    sum += list[i];
                }
                return sum;
            });

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Simple foreach loops:");
            Console.WriteLine("  - Array");
            RunTest(() =>
            {
                var sum = 0;
                foreach (var element in array)
                {
                    sum += element;
                }
                return sum;
            });
            Console.WriteLine("  - List");
            RunTest(() =>
            {
                var sum = 0;
                foreach (var element in list)
                {
                    sum += element;
                }
                return sum;
            });

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" Bad Parallel.Foreach (without interlocked add)");
            Console.WriteLine("  - Array");
            RunTest(() =>
            {
                var sum = 0;
                Parallel.ForEach(array, element =>
                {
                    sum += element; // do not do this!
                });
                return sum;
            });

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Parallel.Foreach");
            Console.WriteLine("  - Array");
            RunTest(() =>
            {
                var sum = 0;
                Parallel.ForEach(array, element =>
                {
                    Interlocked.Add(ref sum, element);
                });
                return sum;
            });
            Console.WriteLine("  - List");
            RunTest(() =>
            {
                var sum = 0;
                Parallel.ForEach(list, element =>
                {
                    Interlocked.Add(ref sum, element);
                });
                return sum;
            });

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" Parallel.For");
            Console.WriteLine("  - Array");
            RunTest(() =>
            {
                var sum = 0;
                Parallel.For(0, array.Length, i =>
                {
                    Interlocked.Add(ref sum, array[i]);
                });
                return sum;
            });
            Console.WriteLine("  - List");
            RunTest(() =>
            {
                var sum = 0;
                Parallel.For(0, list.Count, i =>
                {
                    Interlocked.Add(ref sum, list[i]);
                });
                return sum;
            });


            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            var passwords = new List<string> { "password", "cohobast", "changeme", "qwert123" };
            Console.WriteLine($"PBKDF2 over {hashRuns} runs hashing {passwords.Count} passwords ({iterations} iterations)...");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" Simple foreach loop");
            RunHashTest(() =>
            {
                foreach (var password in passwords)
                {
                    var pbkdf2 = new Rfc2898DeriveBytes(password, 256, iterations);
                    Convert.ToBase64String(pbkdf2.GetBytes(256));
                }
            });

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" Parallel.ForEach");
            RunHashTest(() =>
            {
                Parallel.ForEach(passwords, password =>
                {
                    var pbkdf2 = new Rfc2898DeriveBytes(password, 256, iterations);
                    Convert.ToBase64String(pbkdf2.GetBytes(256));
                });
            });

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" AsParallel().ForAll");
            RunHashTest(() =>
            {
                passwords.AsParallel().ForAll(password =>
                {
                    var pbkdf2 = new Rfc2898DeriveBytes(password, 256, iterations);
                    Convert.ToBase64String(pbkdf2.GetBytes(256));
                });
            });

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" AsParallel().WithDegreeOfParallelism(2).ForAll");
            RunHashTest(() =>
            {
                passwords.AsParallel().WithDegreeOfParallelism(2).ForAll(password =>
                {
                    var pbkdf2 = new Rfc2898DeriveBytes(password, 256, iterations);
                    Convert.ToBase64String(pbkdf2.GetBytes(256));
                });
            });

            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }

        private static void RunTest(Func<int> func)
        {
            var result = 0;
            var s = Stopwatch.StartNew();
            for (int j = 0; j < runs; j++)
            {
                result = func();
            }
            s.Stop();
            Console.WriteLine($"      {s.ElapsedMilliseconds:0000} ms total time, last sum = {result}");
            GC.Collect();
        }
        private static void RunHashTest(Action func)
        {
            var s = Stopwatch.StartNew();
            for (int j = 0; j < hashRuns; j++)
            {
                func();
            }
            s.Stop();
            Console.WriteLine($"  {s.ElapsedMilliseconds:0000} ms total time");
            GC.Collect();
        }
    }
}
