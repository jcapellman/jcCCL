using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using jcCCL.lib.Common;
using jcCCL.lib.Extensions;
using jcCCL.lib.Transports;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace jcCCL.lib.Algorithms {
    public abstract class BaseJCCC {
        private readonly ConcurrentQueue<jcCCDATAITEM> _bytes;
        private readonly ConcurrentQueue<int> _dataList;

        protected BaseJCCC() {
            _bytes = new ConcurrentQueue<jcCCDATAITEM>();
            _dataList = new ConcurrentQueue<int>();
        }

        public abstract int GetVersion();

        private void addData(byte[] data) {
            if (data.Length == 0) {
                return;
            }

            var hash = data.ComputeMD5();

            var item = _bytes.FirstOrDefault(a => a.Hash == hash);

            if (item == null) {
                _bytes.Enqueue(new jcCCDATAITEM { Hash = hash, Index = _bytes.Count(), Data = data });
                _dataList.Enqueue(_bytes.Count());
            } else {
                _dataList.Enqueue(item.Index);
            }
        }

        private byte[] getBytes(string argFileName) {
            using (var ms = new MemoryStream()) {
                using (var writer = new BsonDataWriter(ms)) {
                    var file = new jcCCA {
                        Filename = argFileName,
                        VersionNumber = Common.Constants.VERSION_NUMBER,
                        Data = new List<jcCCDATAITEM>()
                    };

                    foreach (var item in _bytes.Select(key => new jcCCDATAITEM {Index = key.Index, Data = key.Data})) {
                        file.Data.Add(item);
                    }

                    file.Patterns = _dataList.ToList();

                    var serializer = new JsonSerializer();
                    serializer.Serialize(writer, file);

                    ms.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    return ms.ToArray();
                }
            }
        }

        private void processFaster(byte[] uncompressedData, int numberOfSlices) {
            var dictionarySize = uncompressedData.Length / numberOfSlices;

            for (var x = 0; x < uncompressedData.Length; x += dictionarySize) {
                var idx = x + dictionarySize;

                if (idx > uncompressedData.Length) {
                    idx = uncompressedData.Length - x;
                }

                var tmpData = new List<byte>();

                for (var y = x; y < idx; y++) {
                    tmpData.Add(uncompressedData[y]);
                }

                addData(tmpData.ToArray());
            }
        }

        private void processSlower(byte[] uncompressedData, int numberOfSlices) {
            for (var x = 0; x < uncompressedData.Length; x += numberOfSlices) {
                var idx = x + numberOfSlices;

                if (idx > uncompressedData.Length) {
                    idx = uncompressedData.Length - x;
                }

                var tmpData = new List<byte>();

                for (var y = x; y < idx; y++) {
                    tmpData.Add(uncompressedData[y]);
                }

                addData(tmpData.ToArray());
            }
        }

        public jcCCResponseItem CompressFile(string fileName, byte[] uncompressedData, int numberOfSlices, bool useSlowerMethod = true, bool generateHash = true) {
            var response = new jcCCResponseItem { OriginalSize = uncompressedData.Length };

            if (useSlowerMethod) {
                processSlower(uncompressedData, numberOfSlices);
            } else {
                processFaster(uncompressedData, numberOfSlices);
            }
          
            response.compressedData = getBytes(fileName);
            response.percentCompression = ((double)response.compressedData.Length / uncompressedData.Length);
            response.NewSize = response.compressedData.Length;

            if (generateHash)
            {
                response.Hash = response.compressedData.ComputeMD5();
            }

            return response;
        }


        public jcCCResponseItem DecompressFile(byte[] compressedData) {
            var response = new jcCCResponseItem();

            var ms = new MemoryStream(compressedData);

            using (var reader = new BsonDataReader(ms)) {
                var serializer = new JsonSerializer();

                var obj = serializer.Deserialize<jcCCA>(reader);

                response.OutputFile = obj.Filename + ".org";

                response.compressedData = new byte[0];

                var data = new List<byte>();

                foreach (var patternID in obj.Patterns) {
                    data.AddRange(obj.Data.FirstOrDefault(a => a.Index == (patternID - 1))?.Data ?? throw new InvalidOperationException());
                }

                response.compressedData = data.ToArray();
            }

            return response;
        } 
    }
}