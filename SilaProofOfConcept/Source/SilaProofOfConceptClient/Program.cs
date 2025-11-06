namespace SilaProofOfConceptClient
{
    using System;
    using Grpc.Net.Client;
    using HelloWorldService;

    internal class Program
    {
        static void Main(string[] args)
        {
            // Create a new gRPC channel
            var uriAddress = "http://localhost:5069";
            var grpcChannel = GrpcChannel.ForAddress(uriAddress);

            // Create a new HelloRequest object
            var helloRequest = new HelloRequest
            {
                Name = "John",
            };

            using (grpcChannel)
            {
                // Create a new HelloWorldClient object
                var greeterClient = new HelloWorld.HelloWorldClient(grpcChannel);
                Console.WriteLine($"Calling SayHello method with {helloRequest.Name} as name");

                // Call the SayHello method on the server
                var reply = greeterClient.SayHello(helloRequest);

                // Print the reply from the server
                Console.WriteLine($"Reply from server: {reply.Message}");

                // Call the JustSayHello method on the server
                Console.WriteLine($"Calling JustSayHello method with {helloRequest.Name} as name");
                greeterClient.JustSayHello(helloRequest);
                Console.WriteLine($"JustSayHello method called successfully");
            }

            // Wait for user input before closing the console
            Console.ReadLine();
        }
    }
}
