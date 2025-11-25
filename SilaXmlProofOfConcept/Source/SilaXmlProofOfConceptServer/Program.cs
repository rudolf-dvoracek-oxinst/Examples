namespace SilaXmlProofOfConceptServer;

using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SiLA2.AspNetCore;
using SiLA2.Commands;
using SiLA2.Network.Discovery;
using SiLA2.Network.Discovery.mDNS;
using Sila2.Oxfordinstruments.Examples.Temperaturefeature.V1;
using SiLA2.Server;
using SiLA2.Server.Services;
using Sila2.Utils;
using SiLA2.Utils.Config;
using SiLA2.Utils.gRPC;
using SiLA2.Utils.Network;
using SiLA2.Utils.Security;
using SilaXmlProofOfConceptServer.GrpcServices;
using SilaXmlProofOfConceptServer.Services;

public class Program
{
    public static void Main(string[] args)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder(args)
            .UseWindowsService(options => options.ServiceName = "SiLA Proof Of Concept Server")
            .ConfigureServices(services =>
            {
                services.AddSingleton<MetadataManager>();
                services.AddSingleton(typeof(IObservableCommandManager<,>), typeof(ObservableCommandManager<,>));
                services.AddTransient<INetworkService, NetworkService>();
                services.AddSingleton<ServiceDiscoveryInfo>();
                services.AddSingleton<ServerInformation>();
                services.AddTransient<IServiceAnnouncer, ServiceAnnouncer>();
                services.AddSingleton<ISiLA2Server, SiLA2Server>();
                services.AddSingleton<IGrpcChannelProvider, GrpcChannelProvider>();
                services.AddScoped<IServerDataProvider, ServerDataProvider>();
                services.AddSingleton<ICertificateProvider, CertificateProvider>();
                services.AddSingleton<ICertificateContext, CertificateContext>();
                services.AddSingleton<ICertificateRepository, CertificateRepository>();
                ServerConfig serverConfig = new("Sila example server",
                    Guid.Parse("94511C6F-77E7-400F-8D8D-5E86B87F53CE"),
                    "localhost",
                    5069,
                    "Ethernet",
                    "SiLA2ExampleServer");
                services.AddSingleton<IServerConfig>(serverConfig);
                services.AddSingleton(x =>
                {
                    var channel = x.GetRequiredService<IGrpcChannelProvider>().GetChannel(serverConfig.FQHN, serverConfig.Port, true);
                    return new TemperatureFeature.TemperatureFeatureClient(channel.Result);
                });

                // gRPC feature services
                services.AddSingleton<GreetingFeatureService>();
                services.AddSingleton<TemperatureFeatureService>();

                // Broadcaster registrations
                services.AddSingleton<ITemperatureBroadcaster, TemperatureBroadcaster>();
                services.AddSingleton<TemperatureBackgroundService>();
                services.AddSingleton<ITemperatureState>(sp => sp.GetRequiredService<TemperatureBackgroundService>());
                services.AddHostedService(sp => sp.GetRequiredService<TemperatureBackgroundService>());
            })
            .ConfigureWebHostDefaults(hostBuilder =>
            {
                hostBuilder.Configure((context, applicationBuilder) =>
                {
                    applicationBuilder.UseRouting();
                    applicationBuilder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<SiLAService>();
                        endpoints.MapGrpcService<GreetingFeatureService>();
                        endpoints.MapGrpcService<TemperatureFeatureService>();
                    });
                });

                hostBuilder.ConfigureKestrel(options =>
                {
                    var certificateProvider = options.ApplicationServices.GetService(typeof(ICertificateProvider)) as ICertificateProvider;
                    var certificate = certificateProvider.GetServerCertificate(safeIfNotExists: true);

                    var serverConfig = options.ApplicationServices.GetService(typeof(IServerConfig)) as IServerConfig;

                    args.ParseServerCmdLineArgs<CmdLineServerArgs>(serverConfig);

                    IPAddress ipAddress = IPAddress.Any;
                    options.Listen(
                        ipAddress, serverConfig.Port, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            listenOptions.UseHttps(certificate);
                        });
                });

                hostBuilder.ConfigureServices(services =>
                {
                    services.AddGrpc(o => o.EnableDetailedErrors = true);
                });
            });

        IHost host = builder.Build();

        var siLA2Server = host.Services.GetService<ISiLA2Server>();
        var logger = host.Services.GetService<ILogger<Program>>();

        host.InitializeSiLA2Features(siLA2Server);
        logger.LogInformation("{Info}", siLA2Server.ServerInformation);
        logger.LogInformation("Starting SiLA2 server announcement...");
        siLA2Server.Start();

        host.Run();
    }
}