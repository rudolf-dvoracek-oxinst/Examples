using CodeFirstContracts;

namespace CodeFirstService.Services;

public sealed class CalculatorService : ICalculatorService
{
    public ValueTask<SumResult> Sum(SumRequest sumRequest)
    {
        var sum = sumRequest.Numbers.Aggregate((a, b) => a + b);

        var sumResult = new SumResult
        {
            Sum = sum
        };
        return ValueTask.FromResult(sumResult);
    }
}