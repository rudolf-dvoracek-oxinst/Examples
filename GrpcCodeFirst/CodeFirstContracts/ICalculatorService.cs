using System.ServiceModel;

namespace CodeFirstContracts;

[ServiceContract]
public interface ICalculatorService
{
    [OperationContract]
    ValueTask<SumResult> Sum(SumRequest sumRequest);
}