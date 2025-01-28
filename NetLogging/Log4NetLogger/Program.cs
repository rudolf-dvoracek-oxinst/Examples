using System.Diagnostics;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReporterLibrary;

namespace Log4NetLogger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Set up DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddLog4Net("log4net.config");
                })
                .AddSingleton<Reporter>()
                .BuildServiceProvider();

            // Create an instance of Reporter
            var reporter = serviceProvider.GetRequiredService<Reporter>();

            // Measure execution time
            var stopwatch = Stopwatch.StartNew();
            reporter.Report();
            stopwatch.Stop();

            Console.WriteLine($"Report method took {stopwatch.Elapsed} ms");
            Console.Read();
        }
    }
}
