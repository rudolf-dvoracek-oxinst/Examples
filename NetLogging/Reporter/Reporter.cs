namespace Reporter
{
    using System;
    using Microsoft.Extensions.Logging;

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
                _logger.LogInformation("Logging information message number {Count}", i);
            }
        }
    }
}