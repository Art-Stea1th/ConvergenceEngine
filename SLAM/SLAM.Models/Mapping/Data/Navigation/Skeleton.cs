using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping.Data.Navigation {

    using Extensions;
    using System.Runtime.CompilerServices;

    internal sealed class Skeleton {

        private Segmenter segmenter;

        private IList<Point> points;
        internal IList<IList<Point>> Segments { get { return segmenter.ToSegment(points); } }

        internal Skeleton(IList<Point> points) {
            segmenter = new Segmenter();
            this.points = points;
        }        
    }
}