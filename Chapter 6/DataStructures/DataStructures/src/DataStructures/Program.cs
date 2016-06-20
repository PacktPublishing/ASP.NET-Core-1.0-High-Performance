using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataStructures
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rng = new Random();
            var elements = 100000;
            var randomDic = new Dictionary<int, double>();
            for (int i = 0; i < elements; i++)
            {
                randomDic.Add(i, rng.NextDouble());
            }
            var randomList = randomDic.ToList();
            var randomArray = randomList.ToArray();
            var runs = 1000;
            Console.WriteLine($"{runs} runs over {elements} elements...");
            Console.WriteLine();
            GC.Collect();

            var afems = RunTest(() =>
            {
                var sum = 0d;
                foreach (var element in randomArray)
                {
                    sum += element.Value;
                }
                return sum;
            }, runs);
            Console.WriteLine($"Array foreach {afems} ms");
            GC.Collect();

            var lfems = RunTest(() =>
            {
                var sum = 0d;
                foreach (var element in randomList)
                {
                    sum += element.Value;
                }
                return sum;
            }, runs);
            Console.WriteLine($"List foreach {lfems} ms");
            GC.Collect();

            var dfems = RunTest(() =>
            {
                var sum = 0d;
                foreach (var element in randomDic)
                {
                    sum += element.Value;
                }
                return sum;
            }, runs);
            Console.WriteLine($"Dict foreach {dfems} ms");
            GC.Collect();

            Console.WriteLine();


            var afms = RunTest(() =>
            {
                var sum = 0d;
                for (int i = 0; i < randomArray.Length; i++)
                {
                    sum += randomArray[i].Value;
                }
                return sum;
            }, runs);
            Console.WriteLine($"Array for {afms} ms");
            GC.Collect();

            var lfms = RunTest(() =>
            {
                var sum = 0d;
                for (int i = 0; i < randomList.Count; i++)
                {
                    sum += randomList[i].Value;
                }
                return sum;
            }, runs);
            Console.WriteLine($"List for {lfms} ms");
            GC.Collect();

            var dfms = RunTest(() =>
            {
                var sum = 0d;
                for (int i = 0; i < randomDic.Count; i++)
                {
                    sum += randomDic[i];
                }
                return sum;
            }, runs);
            Console.WriteLine($"Dict for {dfms} ms");
            GC.Collect();

            Console.WriteLine();


            var lastKey = randomList.Last().Key;

            var asms = RunTest(() =>
            {
                return randomArray.FirstOrDefault(r => r.Key == lastKey).Value;
            }, runs);
            Console.WriteLine($"Array select {asms} ms");
            GC.Collect();

            var lsms = RunTest(() =>
            {
                return randomList.FirstOrDefault(r => r.Key == lastKey).Value;
            }, runs);
            Console.WriteLine($"List select {lsms} ms");
            GC.Collect();

            // really quick
            var dsms = RunTest(() =>
            {
                double result;
                if (randomDic.TryGetValue(lastKey, out result))
                {
                    return result;
                }
                return 0d;
            }, runs * 10000);
            Console.WriteLine($"Dict select {(double)dsms / 10000} ms");
            GC.Collect();

            Console.WriteLine();


            var aams = RunTest(() =>
            {
                var array = new int[elements];
                for (int i = 0; i < elements; i++)
                {
                    array[i] = i;
                }
                return 0;
            }, runs);
            Console.WriteLine($"Array add {aams} ms");
            GC.Collect();

            var lams = RunTest(() =>
            {
                var list = new List<int>();
                for (int i = 0; i < elements; i++)
                {
                    list.Add(i);
                }
                return 0;
            }, runs);
            Console.WriteLine($"List add {lams} ms");
            GC.Collect();

            var dams = RunTest(() =>
            {
                var dic = new Dictionary<int, int>();
                for (int i = 0; i < elements; i++)
                {
                    dic.Add(i, i);
                }
                return 0;
            }, runs);
            Console.WriteLine($"Dict add {dams} ms");
            GC.Collect();

            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        private static long RunTest(Func<double> func, int runs = 1000)
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
