using CalculatorService;
using Grpc.Core;

namespace ProtoService.Services;

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