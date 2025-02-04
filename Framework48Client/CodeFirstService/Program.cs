using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Server;

namespace CodeFirstService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddCodeFirstGrpc(options => options.EnableDetailedErrors = true);

            // Configure Kestrel to support HTTP/1.1 and HTTP/2
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenLocalhost(5069, listenOptions =>
                {
                    listenOptions.UseHttps();
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                });
            });
            var app = builder.Build();

            app.UseRouting();
            app.UseGrpcWeb();

            // Configure the HTTP request pipeline.

            app.MapGrpcService<Services.GreeterService>().EnableGrpcWeb();
            app.MapGrpcService<Services.CalculatorService>().EnableGrpcWeb();
            app.MapGet("/",
                () =>
                    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}
