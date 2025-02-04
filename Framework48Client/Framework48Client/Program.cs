using System;
using System.Net.Http;
using CodeFirstContracts;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using ProtoBuf.Grpc.Client;

namespace Framework48Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var httpClientHandler = new HttpClientHandler();
            var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, httpClientHandler);

            var grpcChannelOptions = new GrpcChannelOptions
            {
                HttpHandler = grpcWebHandler,
            };
            var uriAddress = "https://localhost:5069";
            var grpcChannel = GrpcChannel.ForAddress(uriAddress, grpcChannelOptions);

            var helloRequest = new HelloRequest
            {
                Name = "customer",
                Number = 2
            };

            using (grpcChannel)
            {
                var greeterClient = grpcChannel.CreateGrpcService<IGreeterService>();
                Console.WriteLine("Calling SayHello method");
                var helloReply = greeterClient.SayHello(helloRequest);
                Console.WriteLine($"{helloReply.Result.Message}");

                Console.WriteLine();
                Console.WriteLine("Calling JustSayHello with empty result");
                greeterClient.JustSayHello(helloRequest);

                var calculatorClient = grpcChannel.CreateGrpcService<ICalculatorService>();
                var numbers = new[] {3.0, 2.0};
                var sumRequest = new SumRequest
                {
                    Numbers = numbers
                };
                Console.WriteLine();
                Console.WriteLine($"Summing numbers {sumRequest}");
                var sumResult = calculatorClient.Sum(sumRequest);
                Console.WriteLine($"Result is {sumResult.Result.Sum}");
                Console.ReadLine();
            }
        }
    }
}
