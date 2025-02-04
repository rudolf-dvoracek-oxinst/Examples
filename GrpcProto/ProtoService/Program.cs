namespace ProtoService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();

        // Configure Kestrel to listen on specific port
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(5069);
        });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<Services.GreeterService>();
        app.MapGrpcService<Services.CalculatorService>();
        app.MapGet("/",
            () =>
                "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}