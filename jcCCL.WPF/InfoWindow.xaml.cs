using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MahApps.Metro.Controls;

using jcCCL.WPF.ViewModel;

namespace jcCCL.WPF {
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : MetroWindow {
        public InfoWindow() {
            InitializeComponent();

            DataContext = new InfoWindowModel();
        }

        private InfoWindowModel viewModel { get { return (InfoWindowModel)DataContext; } }
        
    }
}
