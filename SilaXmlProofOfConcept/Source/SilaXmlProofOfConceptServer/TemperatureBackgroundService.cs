namespace SilaXmlProofOfConceptServer;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sila2.Examples.Temperature.V1;
using SilaXmlProofOfConceptServer.Services;

public sealed class TemperatureBackgroundService : BackgroundService
{
    private readonly ILogger<TemperatureBackgroundService> logger;
    private readonly ITemperatureBroadcaster broadcaster;
    private readonly Random random = new();

    private double currentTemperature = 20.0;

    public TemperatureBackgroundService(
        ILogger<TemperatureBackgroundService> logger,
        ITemperatureBroadcaster broadcaster)
    {
        this.logger = logger;
        this.broadcaster = broadcaster;
    }

    public double CurrentTemperature => this.currentTemperature;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("TemperatureBackgroundService executed.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Simulate drift
                this.currentTemperature += (this.random.NextDouble() - 0.5) * 0.2;

                var reading = new TemperatureReading { ValueCelsius = Math.Round(this.currentTemperature, 3), TimestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };

                this.broadcaster.Broadcast(reading);
                await Task.Delay(2000, stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
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