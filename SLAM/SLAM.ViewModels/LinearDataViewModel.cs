using SLAM.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SLAM.ViewModels {

    public class LinearDataViewModel : ViewModelBase {

        private string testData = "Hello!";

        public string TestData {
            get { return testData; }
            set { Set(ref testData, value); }
        }
    }
}