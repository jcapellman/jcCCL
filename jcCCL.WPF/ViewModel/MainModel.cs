using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

using jcCCL.PCL;
using jcCCL.PCL.Common;

namespace jcCCL.WPF.ViewModel {
    public class MainModel : INotifyPropertyChanged {
        private Visibility _pgVisibility;

        public Visibility PG_Visibility {
            get { return _pgVisibility; }
            set { _pgVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _btnProcessVisibility;

        public Visibility BUTTON_Visibility {
            get { return _btnProcessVisibility; }
            set { _btnProcessVisibility = value; OnPropertyChanged(); }
        }

        public MainModel() {
            BUTTON_Visibility = Visibility.Visible;
            PG_Visibility = Visibility.Collapsed;
        }

        public List<jcCCResponseItem> ProcessFiles(string[] files) {
            BUTTON_Visibility = Visibility.Collapsed;
            PG_Visibility = Visibility.Visible;

            var results = new List<jcCCResponseItem>();

            foreach (var file in files) {
                if (file.EndsWith("." + PCL.Common.Constants.FILE_EXTENSION)) {
                    results.Add(uncompress(file));
                } else {
                    results.Add(compress(file));
                }
            }

            PG_Visibility = Visibility.Collapsed;
            BUTTON_Visibility = Visibility.Visible; 

            return results;
        }

        private jcCCResponseItem compress(string fileName, bool generateHash = true) {
            var response = new jcCCManager().CompressFile(fileName, File.ReadAllBytes(fileName), PCL.Common.Constants.DEFAUlT_SLICES, generateHash);

            File.WriteAllBytes(fileName + "." + PCL.Common.Constants.FILE_EXTENSION, response.compressedData);

            if (generateHash) {
                File.WriteAllText(fileName + "." + PCL.Common.Constants.FILE_EXTENSION + ".md5", response.Hash);
            }

            return response;
        }

        private jcCCResponseItem uncompress(string fileName) {
            var response = new jcCCManager().DecompressFile(File.ReadAllBytes(fileName));

            File.WriteAllBytes(response.OutputFile + ".tmp", response.compressedData);

            return response;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}