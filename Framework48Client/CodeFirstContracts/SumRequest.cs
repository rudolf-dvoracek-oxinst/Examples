using System.Runtime.Serialization;

namespace CodeFirstContracts
{
    [DataContract]
    public sealed class SumRequest
    {
        [DataMember(Order=1)]
        public double[] Numbers { get; set; }
    }
}