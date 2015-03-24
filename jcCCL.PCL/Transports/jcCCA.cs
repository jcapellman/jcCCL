using System.Collections.Generic;
using System.Runtime.Serialization;

using jcCCL.PCL.Common;

namespace jcCCL.PCL.Transports {
    [DataContract]
    public class jcCCA {
        [DataMember]
        public int VersionNumber { get; set; }

        [DataMember]
        public string Filename { get; set; }

        [DataMember]
        public List<jcCCDATAITEM> Data { get; set; }
        
        [DataMember]
        public List<int> Patterns { get; set; }  
    }
}