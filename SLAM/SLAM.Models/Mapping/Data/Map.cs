using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAM.Models.Mapping.Data {

    using Navigation;

    internal sealed class Map {

        private Segmenter segmenter;
        private List<PointSequence> frames;

        internal void AddFrame(PointSequence frame) {

        }

        internal Map() {
            frames = new List<PointSequence>();
        }
    }
}