namespace SilaProofOfConceptServer;

using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilaProofOfConceptServer.GrpcServices;
using SilaProofOfConceptServer.Services;

public class Program
{
    public static void Main(string[] args)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder(args)
            .UseWindowsService(options => options.ServiceName = @"Hello World Service")
            .ConfigureServices(services =>
            {
                // gRPC feature services
                services.AddSingleton<GreetingFeatureService>();
                services.AddSingleton<TemperatureFeatureService>();

                // Broadcaster registrations
                services.AddSingleton<IGreetingBroadcaster>(sp => sp.GetRequiredService<GreetingFeatureService>());
                services.AddSingleton<ITemperatureBroadcaster>(sp => sp.GetRequiredService<TemperatureFeatureService>());

                // Temperature background producer
                services.AddSingleton<TemperatureBackgroundService>();
                services.AddHostedService(sp => sp.GetRequiredService<TemperatureBackgroundService>());

            })
            .ConfigureWebHostDefaults(hostBuilder =>
            {
                hostBuilder.Configure((context, applicationBuilder) =>
                {
                    applicationBuilder.UseRouting();
                    applicationBuilder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<GreetingFeatureService>();
                        endpoints.MapGrpcService<TemperatureFeatureService>();
                    });
                });

                hostBuilder.ConfigureKestrel(options =>
                    options.Listen(IPAddress.Loopback, 5069, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    }));

                hostBuilder.ConfigureServices(services =>
                {
                    services.AddGrpc(o => o.EnableDetailedErrors = true);
                });
            });

        IHost app = builder.Build();
        app.Run();
    }
}