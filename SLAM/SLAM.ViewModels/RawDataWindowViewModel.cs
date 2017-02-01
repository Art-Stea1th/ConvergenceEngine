using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SLAM.ViewModels {

    public class RawDataWindowViewModel : ViewModelBase {

        private WriteableBitmap frontDepthtData;

        public ImageSource FrontDepthData {
            get { return frontDepthtData; }
            set { Set(ref frontDepthtData, (WriteableBitmap)value); }
        }
    }
}