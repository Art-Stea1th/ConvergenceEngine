using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAM.Models.Map.BrutforceMapperResources {

    internal sealed class GradientDescent {

        public enum Direction { Forward, Backward }

        private Direction direction;

        private int maxIterations;
        private Func<bool> NeedImproved;

        

        public Func<bool> Improved;        

        public void Proceed() {

            while (true) {

                if (Improved()) {

                }
                else {
                    ChangeDirection();
                }
            }


            //while (true) { // r
            //    while (true) { // y
            //        while (true) { // x

            //        }
            //    }
            //}
        }

        private void ChangeDirection() {
            if (direction == Direction.Forward) {
                direction = Direction.Backward;
                return;
            }
            direction = Direction.Forward;
        }
    }
}