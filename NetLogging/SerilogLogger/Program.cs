using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using ReporterLibrary;
using Serilog;

namespace SerilogLogger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main Thread";

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();


            // Set up DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddSerilog(logger);
                })
                .AddSingleton<Reporter>()
                .BuildServiceProvider();

            // Create an instance of Reporter
            var reporter = serviceProvider.GetRequiredService<Reporter>();

            // Measure execution time
            var stopwatch = Stopwatch.StartNew();
            int numberOfItems = 25000;
            reporter.Report(numberOfItems);
            stopwatch.Stop();

            Console.WriteLine($"Report method took {stopwatch.Elapsed} s");
            Console.WriteLine($"One item latency {stopwatch.Elapsed.TotalMilliseconds/numberOfItems} ms");

            Console.Read();
        }
    }
}
