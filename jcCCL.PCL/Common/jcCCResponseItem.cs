namespace jcCCL.PCL.Common {
    public class jcCCResponseItem {
        public byte[] compressedData { get; set; }

        public double percentCompression { get; set; }

        public double OriginalSize { get; set; }

        public double NewSize { get; set; }

        public string OutputFile { get; set; }

        public string Hash { get; set; }
    }
}