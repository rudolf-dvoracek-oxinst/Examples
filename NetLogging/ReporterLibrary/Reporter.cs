using Microsoft.Extensions.Logging;

namespace ReporterLibrary
{
    public class Reporter
    {
        private readonly ILogger<Reporter> _logger;

        public Reporter(ILogger<Reporter> logger)
        {
            _logger = logger;
        }

        public void Report()
        {
            for (int i = 0; i < 25000; i++)
            {
                // Uncomment following lines to get structured logging
                // var car = new Car();
                // _logger.LogInformation("Logging information message number {Count} for {@car}", i, car);
                _logger.LogInformation("Logging information message number {Count}", i);
            }
        }
    }
}