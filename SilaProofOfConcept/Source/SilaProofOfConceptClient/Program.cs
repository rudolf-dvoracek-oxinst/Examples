namespace SilaProofOfConceptClient
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Grpc.Net.Client;
    using Google.Protobuf.WellKnownTypes;
    using Sila2.Examples.Greeting.V1;
    using Sila2.Examples.Temperature.V1;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var uriAddress = "http://localhost:5069";
            using var channel = GrpcChannel.ForAddress(uriAddress);

            var greetingClient = new GreetingFeature.GreetingFeatureClient(channel);
            var tempClient = new TemperatureFeature.TemperatureFeatureClient(channel);

            Console.WriteLine("Discover GreetingFeature:");
            var greetingDef = greetingClient.GetFeatureDefinition(new Empty());
            Console.WriteLine($"Feature: {greetingDef.FeatureName} Commands={string.Join(",", greetingDef.Commands)}");

            var exec = greetingClient.SayHello(new SayHelloParameters { Name = "John" });
            Console.WriteLine($"SayHello execution: {exec.ExecutionUuid} State={exec.State} Message={exec.Result?.Message}");

            Console.WriteLine("LastGreeting property:");
            var lastGreeting = greetingClient.GetLastGreeting(new Empty());
            Console.WriteLine($"LastGreeting: {lastGreeting.Message}");

            Console.WriteLine("Discover TemperatureFeature:");
            var tempDef = tempClient.GetFeatureDefinition(new Empty());
            Console.WriteLine($"Feature: {tempDef.FeatureName} Properties={string.Join(",", tempDef.Properties)}");

            Console.WriteLine("MeasureTemperature command:");
            var tempExec = tempClient.MeasureTemperature(new Empty());
            Console.WriteLine($"Temperature: {tempExec.Reading.ValueCelsius:F3} °C @ {tempExec.Reading.TimestampUnixMs}");

            Console.WriteLine("CurrentTemperature property:");
            var current = tempClient.GetCurrentTemperature(new Empty());
            Console.WriteLine($"CurrentTemperature: {current.ValueCelsius:F3} °C");

            Console.WriteLine("Subscribe to TemperatureUpdated events (5 samples)...");
            using var cts = new CancellationTokenSource();
            var stream = tempClient.SubscribeTemperatureUpdated(new Empty(), cancellationToken: cts.Token);

            int received = 0;
            try
            {
                await foreach (var reading in stream.ResponseStream.ReadAllAsync(cts.Token))
                {
                    Console.WriteLine($"[Event] {reading.ValueCelsius:F3} °C");
                    if (++received >= 5) break;
                }
            }
            finally
            {
                cts.Cancel();
            }

            Console.WriteLine("Done. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}