using Grpc.Net.Client;
using ProtoService;

namespace ProtoClient;

internal class Program
{
    private static void Main(string[] args)
    {
        var uriAddress = "http://localhost:5069";
        var grpcChannel = GrpcChannel.ForAddress(uriAddress);

        var helloRequest = new HelloRequest();
        helloRequest.Name = "customer";
        helloRequest.Number = 2;
        using (grpcChannel)
        {
            var greeterClient = new Greeter.GreeterClient(grpcChannel);
            var helloReply = greeterClient.SayHello(helloRequest);
            Console.WriteLine($"{helloReply}");

            var calculatorClient = new Calculator.CalculatorClient(grpcChannel);
            var sumRequest = new SumRequest();
            sumRequest.Numbers.AddRange(new[] { 3.0f, 2.0f });
            var sumResult = calculatorClient.Sum(sumRequest);
            Console.WriteLine($"Result is {sumResult}");
        }
    }
}