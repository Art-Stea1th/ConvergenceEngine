using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvergenceEngine.Models.Mapping {

    using Infrastructure.Interfaces;

    public sealed class Map : IMap {

        public IEnumerable<ISegment> AllSegments { get; internal set; }
        public IEnumerable<ISegment> CurrentSegments { get; internal set; }

        public IEnumerable<INavigationInfo> CameraPath { get; internal set; }
    }
}