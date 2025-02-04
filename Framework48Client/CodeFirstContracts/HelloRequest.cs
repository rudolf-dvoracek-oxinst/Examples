using System.Runtime.Serialization;

namespace CodeFirstContracts
{
    [DataContract]
    public sealed class HelloRequest
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }
        
        [DataMember(Order = 2)]
        public int Number { get; set; }
    }
}