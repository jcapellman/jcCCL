using System;
using System.IO;
using System.Linq;
using System.Reflection;

using jcCCL.lib.Algorithms;
using jcCCL.lib.Common;
using jcCCL.lib.Transports;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace jcCCL.lib {
    public class jcCCManager {
        private Assembly CurrentAssembly {
            get {
                var fullAssembly = GetType().AssemblyQualifiedName;

                return Assembly.Load(new AssemblyName(fullAssembly.Split(',')[1]));
            }
        }

        public BaseJCCC GetVersion(int version = Common.Constants.VERSION_NUMBER) {
            var assemblies = CurrentAssembly.DefinedTypes.Where(a => !a.IsAbstract && a.IsClass && a.BaseType == typeof (BaseJCCC)).ToList();

            return assemblies.Select(assembly => (BaseJCCC) Activator.CreateInstance(assembly.AsType())).FirstOrDefault(cAssembly => cAssembly.GetVersion() == version);
        }

        public jcCCResponseItem DecompressFile(byte[] compressedData) {
            var versionNumber = 0;

            var ms = new MemoryStream(compressedData);

            using (var reader = new BsonDataReader(ms)) {
                var serializer = new JsonSerializer();
                
                var obj = serializer.Deserialize<jcCCA>(reader);

                versionNumber = obj.VersionNumber;
            }

            if (versionNumber == 0) {
                throw new Exception("Could not obtain version number from header");
            }

            return GetVersion(versionNumber).DecompressFile(compressedData);
        }

        public jcCCResponseItem CompressFile(string fileName, byte[] uncompressedData, int numberOfSlices, bool generateHash = true) {
            return GetVersion().CompressFile(fileName, uncompressedData, numberOfSlices, generateHash);
        }
    }
}