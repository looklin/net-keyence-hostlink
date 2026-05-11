using System;
using System.Threading.Tasks;

namespace Keyence.HostLink.ConsoleExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Keyence HostLink Console Example ===");
            Console.WriteLine();

            var options = new HostLinkOptions
            {
                Host = "192.168.3.100",
                Port = 8501,
                ConnectTimeout = TimeSpan.FromSeconds(5),
                ReadTimeout = TimeSpan.FromSeconds(3),
                AutoReconnect = true
            };

            using var client = new HostLinkClient(options);

            client.Connected += (s, e) => Console.WriteLine("[Event] Connected to PLC");
            client.Disconnected += (s, e) => Console.WriteLine("[Event] Disconnected from PLC");

            try
            {
                Console.WriteLine($"Connecting to {options.Host}:{options.Port}...");
                await client.ConnectAsync();
                Console.WriteLine("Connected successfully.");
                Console.WriteLine();

                Console.WriteLine("Reading DM0...");
                var readResult = await client.ReadItemAsync("DM0");

                if (readResult.IsSuccess)
                {
                    Console.WriteLine($"Raw response: {readResult.RawResponse}");
                    Console.WriteLine($"Int16 value: {readResult.ToInt16()}");
                }
                else
                {
                    Console.WriteLine($"Read failed: {readResult.Error?.Message}");
                }

                Console.WriteLine();
                Console.WriteLine("Writing 100 to DM0...");
                await client.WriteItemAsync("DM0", "100");
                Console.WriteLine("Write completed.");

                Console.WriteLine();
                Console.WriteLine("Reading DM0-DM9 (continuous)...");
                var contResult = await client.ReadContinuousAsync("DM0", 10);

                if (contResult.IsSuccess)
                {
                    Console.WriteLine($"Raw response: {contResult.RawResponse}");
                    var values = contResult.ToIntArray();
                    for (int i = 0; i < values.Length; i++)
                    {
                        Console.WriteLine($"  DM{i}: {values[i]}");
                    }
                }
                else
                {
                    Console.WriteLine($"Continuous read failed: {contResult.Error?.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Disconnect();
                Console.WriteLine("Disconnected.");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
