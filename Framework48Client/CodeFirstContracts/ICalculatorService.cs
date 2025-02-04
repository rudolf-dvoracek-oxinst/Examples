using System.ServiceModel;
using System.Threading.Tasks;

namespace CodeFirstContracts
{
    [ServiceContract]
    public interface ICalculatorService
    {
        [OperationContract]
        ValueTask<SumResult> Sum(SumRequest sumRequest);
    }
}