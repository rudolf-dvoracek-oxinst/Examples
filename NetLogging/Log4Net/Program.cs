using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Log4Net
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Set up Log4Net configuration
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            // Set up DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddLog4Net();
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .AddSingleton<Reporter.Reporter>()
                .BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();
        
            // Create an instance of Reporter
            var reporter = serviceProvider.GetRequiredService<Reporter.Reporter>();

            // Measure execution time
            var stopwatch = Stopwatch.StartNew();
            reporter.Report();
            stopwatch.Stop();

            logger.LogInformation("Report method took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        }
    }
}
