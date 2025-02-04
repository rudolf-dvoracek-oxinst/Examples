using System.Runtime.Serialization;

namespace CodeFirstContracts
{
    [DataContract]
    public sealed class SumResult
    {
        [DataMember(Order = 1)]
        public double Sum { get; set; }
    }
}