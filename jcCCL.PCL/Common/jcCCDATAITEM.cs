using System.Runtime.Serialization;

namespace jcCCL.PCL.Common {
    [DataContract]
    public class jcCCDATAITEM {
        [DataMember(Name = "I")]
        public int Index { get; set; }

        [DataMember(Name = "D")]
        public byte[] Data { get; set; }

        public string Hash { get; set; }
    }
}