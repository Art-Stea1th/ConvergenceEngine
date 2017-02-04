using System;
using SLAM.Models;
using System.Windows;
using System.Collections.Generic;

namespace SLAM.ViewModels.AppWindows {

    public class LinearDataWindowViewModel : ViewportWindowViewModel {

        List<List<Point>> segments;

        public List<List<Point>> Segments {
            get { return segments; }
            set { Set(ref segments, value); }
        }

        private bool currentA = true;

        internal LinearDataWindowViewModel() {
            Title = "Line-Segments Data";
            Initialize();
        }

        public override void Initialize() {
            Segments = null;
        }

        public override void UpdateFrom(Model model) {



            if (currentA) {
                Segments = new List<List<Point>> {
                    new List<Point> { new Point(0, 0), new Point(0, 0) },
                    new List<Point> { new Point(76, 145), new Point(54, 265) },
                    new List<Point> { new Point(43, 234), new Point(53, 344) },
                    new List<Point> { new Point(2, 16), new Point(4, 77) },
                    new List<Point> { new Point(126, 311), new Point(54, 200) }
                };
            }
            else {
                Segments = new List<List<Point>> {
                    new List<Point> { new Point(0, 0), new Point(0, 0) },
                    new List<Point> { new Point(53, 244), new Point(63, 354) },
                    new List<Point> { new Point(86, 155), new Point(64, 275) },
                    new List<Point> { new Point(136, 321), new Point(64, 210) },
                    new List<Point> { new Point(12, 26), new Point(14, 87) }
                };
            }
            currentA = !currentA;
        }
    }
}