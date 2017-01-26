using System.Collections.Generic;


namespace SLAM.Models.MapModel.ComplexMapperResources {

    using DataModel.Structures;

    internal sealed class Beautifier {


        public IEnumerable<SPoint> Beautify(IEnumerable<SPoint> points) {
            return null;
        }

        public IEnumerable<SPoint> RemoveIsolatedArtifacts(IList<SPoint> points, int maxArtifactsLength, float distanceThreshold) {

            int currentSequenceLength = 0;

            for (int i = 0; i < points.Count - 1; ++i) {
                
            }

            return null;
        }

    }
}