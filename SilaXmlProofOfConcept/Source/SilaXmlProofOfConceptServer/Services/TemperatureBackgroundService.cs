namespace SilaXmlProofOfConceptServer.Services;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class TemperatureBackgroundService : BackgroundService, ITemperatureState
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
        this.logger.LogInformation("TemperatureBackgroundService started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Simulate drift
                this.currentTemperature += (this.random.NextDouble() - 0.5) * 0.2;
                this.currentTemperature = Math.Round(this.currentTemperature, 3);

                var sample = new TemperatureSample(
                    this.currentTemperature,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

                this.broadcaster.Broadcast(sample);
                await Task.Delay(2000, stoppingToken);
            }
        }
        catch (TaskCanceledException) { }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Fatal error in TemperatureBackgroundService");
            Environment.Exit(1);
        }
    }
}