namespace SilaProofOfConceptServer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class HelloWorldBackgroundService : BackgroundService
    {
        private readonly ILogger<HelloWorldBackgroundService> logger;

        public HelloWorldBackgroundService(ILogger<HelloWorldBackgroundService> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("HelloWorldBackgroundService is executed.");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            // ReSharper disable once CatchAllClause
            catch (Exception ex)
            {
                this.logger.LogError(ex, "General error when starting HelloWorld Service");

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }
    }
}
