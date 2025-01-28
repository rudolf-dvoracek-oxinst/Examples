using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ProtoService.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            HelloReply reply =  new HelloReply();
            reply.Message = "Hello " + request.Name;
            if (request.Number > 1)
            {
                reply.Message += "s";
            }
            return Task.FromResult(reply);
        }

        public override Task<Empty> JustSayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }
    }

    public class CalculatorService : Calculator.CalculatorBase
    {
        public override Task<SumResult> Sum(SumRequest request, ServerCallContext context)
        {
            var sum = request.Numbers.Aggregate((a, b) => a + b);

            var sumResult = new SumResult
            {
                Sum = sum
            };
            return Task.FromResult(sumResult);
        }
    }
}
