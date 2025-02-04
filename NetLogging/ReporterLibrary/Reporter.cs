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

        public void Report(int numberOfItems)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                // Uncomment following lines to get structured logging
                // var car = new Car();
                // _logger.LogInformation("Logging information message number {Count} for {@car}", i, car);

                // comment following line to get structured logging
                _logger.LogInformation("Logging information message number {Count}", i);
            }
        }
    }
}