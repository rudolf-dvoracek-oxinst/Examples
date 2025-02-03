using System.ServiceModel;

namespace CodeFirstContracts;

[ServiceContract]
public interface IGreeterService
{
    [OperationContract]
    ValueTask<HelloReply> SayHello(HelloRequest helloRequest);


    [OperationContract]
    ValueTask JustSayHello(HelloRequest helloRequest);
}