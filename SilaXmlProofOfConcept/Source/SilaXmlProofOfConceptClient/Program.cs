namespace SilaXmlProofOfConceptClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Microsoft.Extensions.Configuration;
    // Replace direct using to avoid type name collision with System.String
    using SiLAString = Sila2.Org.Silastandard.String;
    using SiLAReal = Sila2.Org.Silastandard.Real;
    using SiLAInteger = Sila2.Org.Silastandard.Integer;
    using Sila2.Oxfordinstruments.Examples.Greetingfeature.V1;
    using Sila2.Oxfordinstruments.Examples.Temperaturefeature.V1;
    using SiLA2.Client;
    using SiLA2.Network.Discovery.mDNS;
    using SiLA2.Utils.Extensions;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var configurator = new Configurator(configurationRoot, args);
            configurator.UpdateServiceProvider();
            IDictionary<Guid, ConnectionInfo>? serverMap = await configurator.SearchForServers();

            ConnectionInfo? connectionInfo = serverMap.Values.FirstOrDefault(info => info.ServerType == "SiLA2ExampleServer");

            if (connectionInfo == null)
            {
                Console.WriteLine("No SiLA2ExampleServer found.");
            }
            else
            {
                Console.WriteLine($"Found SiLA2ExampleServer at {connectionInfo.Address}:{connectionInfo.Port}");
                Console.WriteLine("Connecting to server...");
                var grpcChannel = await configurator.GetChannel(
                    connectionInfo.Address,
                    connectionInfo.Port,
                    false,
                    connectionInfo.SilaCA.GetCaFromFormattedCa());

                var temperatureFeatureClient = new TemperatureFeature.TemperatureFeatureClient(grpcChannel);
                var greetingClient = new GreetingFeature.GreetingFeatureClient(grpcChannel);

                // GreetingFeature usage
                Console.WriteLine("Invoke SayHello command:");
                var sayHelloResponse = greetingClient.SayHello(new SayHello_Parameters
                {
                    Name = new SiLAString { Value = "John" }
                });
                Console.WriteLine("SayHello executed (response empty by definition).");

                Console.WriteLine("Subscribe to LastGreeting stream (3 samples)...");
                using (var greetingCts = new CancellationTokenSource())
                {
                    int count = 0;
                    var greetingStream = greetingClient.Subscribe_LastGreeting(new Subscribe_LastGreeting_Parameters(), cancellationToken: greetingCts.Token);
                    try
                    {
                        await foreach (var update in greetingStream.ResponseStream.ReadAllAsync(greetingCts.Token))
                        {
                            var msg = update.LastGreeting?.Message?.Value ?? "<empty>";
                            var ts = update.LastGreeting?.TimestampUnixMs?.Value ?? 0;
                            var dt = ts > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(ts).ToString("O") : "-";
                            Console.WriteLine($"[LastGreeting] Message='{msg}' Timestamp='{dt}'");
                            if (++count >= 3) break;
                        }
                    }
                    finally
                    {
                        greetingCts.Cancel();
                    }
                }

                // TemperatureFeature usage
                Console.WriteLine("Invoke MeasureTemperature command:");
                var measureResponse = temperatureFeatureClient.MeasureTemperature(new MeasureTemperature_Parameters());
                Console.WriteLine("MeasureTemperature executed (response empty).");

                Console.WriteLine("Subscribe to CurrentTemperature stream (5 samples)...");
                using (var tempCts = new CancellationTokenSource())
                {
                    int received = 0;
                    var tempStream = temperatureFeatureClient.Subscribe_CurrentTemperature(new Subscribe_CurrentTemperature_Parameters(), cancellationToken: tempCts.Token);
                    try
                    {
                        await foreach (var reading in tempStream.ResponseStream.ReadAllAsync(tempCts.Token))
                        {
                            var value = reading.CurrentTemperature?.ValueCelsius?.Value ?? double.NaN;
                            var ts = reading.CurrentTemperature?.TimestampUnixMs?.Value ?? 0;
                            var dt = ts > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(ts).ToString("O") : "-";
                            Console.WriteLine($"[Temp] {value:F3} °C @ {dt}");
                            if (++received >= 5) break;
                        }
                    }
                    finally
                    {
                        tempCts.Cancel();
                    }
                }
            }

            Console.WriteLine("Done. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}