using CalculatorService;
using GreeterService;
using Grpc.Net.Client;

namespace ProtoClient;

internal class Program
{
    private static void Main(string[] args)
    {
        var uriAddress = "http://localhost:5069";
        var grpcChannel = GrpcChannel.ForAddress(uriAddress);

        var helloRequest = new HelloRequest
        {
            Name = "customer",
            Number = 2
        };

        using (grpcChannel)
        {
            var greeterClient = new Greeter.GreeterClient(grpcChannel);
            Console.WriteLine("Calling SayHello method");
            var helloReply = greeterClient.SayHello(helloRequest);
            Console.WriteLine($"{helloReply}");

            Console.WriteLine();
            Console.WriteLine("Calling JustSayHello with empty result");
            greeterClient.JustSayHello(helloRequest);

            var calculatorClient = new Calculator.CalculatorClient(grpcChannel);
            var numbers = new[] {3.0f, 2.0f};
            var sumRequest = new SumRequest();
            sumRequest.Numbers.AddRange(numbers);
            Console.WriteLine();
            Console.WriteLine($"Summing numbers {sumRequest}");
            var sumResult = calculatorClient.Sum(sumRequest);
            Console.WriteLine($"Result is {sumResult}");
        }
    }
}