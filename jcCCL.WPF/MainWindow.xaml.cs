using System.Linq;
using System.Windows;

using MahApps.Metro.Controls;
using jcCCL.WPF.ViewModel;

namespace jcCCL.WPF {
    public partial class MainWindow : MetroWindow {

        public MainWindow() {
            InitializeComponent();

            DataContext = new MainModel();
        }

        private MainModel viewModel { get { return (MainModel)DataContext; } }
        
        private void spMain_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var response = viewModel.ProcessFiles((string[])e.Data.GetData(DataFormats.FileDrop));

                if (response.Any()) {
                    foreach (var result in response) {
                        MessageBox.Show(this, result.OriginalSize + " = " + result.NewSize);
                    }
                }
            }
        }
    }
}