using System.Runtime.Serialization;

namespace CodeFirstContracts
{
    [DataContract]
    public sealed class HelloReply
    {
        [DataMember(Order = 1)] public string Message { get; set; }
    }
}