namespace SilaProofOfConceptServer;

using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilaProofOfConceptServer.GrpcServices;

public class Program
{
    public static void Main(string[] args)
    {
        // Create a new host builder
        var builder = Host.CreateDefaultBuilder(args)
            // Configure the host to use Windows services
            .UseWindowsService(options => options.ServiceName = @"Hello World Service")
            // Configure the host to use a windows service hosted
            .ConfigureServices(services =>
            {
                services.AddHostedService<HelloWorldBackgroundService>();
            })
            .ConfigureWebHostDefaults(hostBuilder =>
            {
                hostBuilder.Configure((context, applicationBuilder) =>
                    {
                        applicationBuilder.UseRouting();
                        applicationBuilder.UseEndpoints(routeBuilder => routeBuilder.MapGrpcService<HelloWorldService>());
                    });

                hostBuilder.ConfigureKestrel(options =>
                    options.Listen(IPAddress.Loopback, 5069, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    }));

                hostBuilder.ConfigureServices(collection =>
                {
                    collection.AddGrpc(options =>
                    {
                        options.EnableDetailedErrors = true;
                    });
                });
            });

        var app = builder.Build();
        app.Run();
    }
}