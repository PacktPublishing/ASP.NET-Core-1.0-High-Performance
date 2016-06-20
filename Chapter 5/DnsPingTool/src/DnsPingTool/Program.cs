using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace DnsPingTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter a hostname or IP address: ");
            var host = Console.ReadLine();
            Console.WriteLine();

            DnsLookup(host).Wait();
            PingHost(host).Wait();

            Console.ResetColor();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        private static async Task DnsLookup(string host)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"Performing DNS lookup of {host}");
                // Results include hosts file overrides not just DNS
                var results = await Dns.GetHostAddressesAsync(host);
                foreach (var result in results)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Complete, {host} = {result}");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"Performing reverse DNS lookup of {result}");
                    var revDns = await Dns.GetHostEntryAsync(result);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Complete, {result} = {revDns.HostName}");
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async Task PingHost(string host)
        {
            var attempts = 4;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Pinging {host} {attempts} times");
            var ping = new Ping();
            for (var ii = 0; ii < attempts; ii++)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Ping attempt #{ii + 1} of {attempts}");
                var result = await ping.SendPingAsync(host);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(result.Status);
                Console.WriteLine($"{result.RoundtripTime} ms");
            }
            Console.WriteLine();
        }
    }
}
