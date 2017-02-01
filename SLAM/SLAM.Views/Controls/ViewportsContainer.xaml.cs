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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SLAM.Views.Controls {
    /// <summary>
    /// Interaction logic for ViewportsContainer.xaml
    /// </summary>
    public partial class ViewportsContainer : UserControl {

        public static readonly DependencyProperty DataAProperty;
        public static readonly DependencyProperty DataBProperty;
        public static readonly DependencyProperty DataCProperty;
        public static readonly DependencyProperty DataDProperty;

        public ViewportsContainer() {
            InitializeComponent();
        }
    }
}
