using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using jcCCL.PCL.Common;
using System.Runtime.InteropServices;

namespace jcCCL.PCL {
    public class JCCDATAITEM {
        public int Index { get; set; }

        public byte[] Data { get; set; }
    }

    public class jcCCLMain {
        public jcCCLMain()
        {
           
        }

        
    }

    public struct HEADER {
        public int VersionNumber { get; set; }

        public string Filename { get; set; }
    }


    [DataContract]
        public class jcCCLData {
            private readonly List<JCCDATAITEM> _bytes;
            private readonly List<int> _dataList;

        

        public jcCCLData() {
                _bytes = new List<JCCDATAITEM>();
                _dataList = new List<int>();
            }

            public void AddData(byte[] data) {
                var item = _bytes.FirstOrDefault(a => a.Data == data);

                if (item == null) {
                    _bytes.Add(new JCCDATAITEM { Index = _bytes.Count(), Data = data });
                    _dataList.Add(_bytes.Count());
                } else {
                    _dataList.Add(item.Index);
                }
            }

            public byte[] GetBytes(HEADER headerItem)
            {
                using (var ms = new MemoryStream()) {
                    var sw = new BinaryWriter(ms);

                    sw.Write(headerItem.Filename);
                sw.Write(headerItem.VersionNumber);

                foreach (var key in _bytes) {
                        sw.Write(key.Index);
                        sw.Write(key.Data);
                    }

                    foreach (var index in _dataList) {
                        sw.Write(index);
                    }

                    sw.Flush();

                    ms.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    return ms.ToArray();
                }
            }

            public jcCCLResponseItem CompressFile(string fileName, byte[] uncompressedData, int numberOfSlices)
            {
                var headerItem = new HEADER() {Filename = fileName, VersionNumber = Common.Constants.VERSION_NUMBER};

                var response = new jcCCLResponseItem();

                var data = new jcCCLData();

                var dictionarySize = uncompressedData.Length/numberOfSlices;

                for (var x = 0; x < uncompressedData.Length; x += dictionarySize) {
                    var tmpData = new List<byte>();

                    for (var y = x; y < dictionarySize; y++) {
                        tmpData.Add(uncompressedData[y]);
                    }

                    data.AddData(tmpData.ToArray());
                }

                response.compressedData = data.GetBytes(headerItem);
                response.percentCompression = ((double)response.compressedData.Length/uncompressedData.Length);

                return response;
            }

            public jcCCLResponseItem DecompressFile(byte[] compressedData)
            {
                var response = new jcCCLResponseItem();

                

                return response;
            }
        }
    }

