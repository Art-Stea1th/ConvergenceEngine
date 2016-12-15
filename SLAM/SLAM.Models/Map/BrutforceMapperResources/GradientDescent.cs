using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SLAM.Models.Map.BrutforceMapperResources {

    internal sealed class GradientDescent {

        public enum Direction { Forward, Backward }

        //private Direction direction;

        private int maxIterations;
        //private Func<bool> NeedImproved;
        //public Func<bool> Improved;

        public void Proceed(Func<bool> NeedImproved, Func<Direction, bool> Improved) {

            Direction direction = Direction.Forward;

            while (NeedImproved()) {

                if (!Improved(direction)) {

                    direction = SwitchedDirection(direction);

                }
                else {
                    
                }
            }


            //while (true) { // r
            //    while (true) { // y
            //        while (true) { // x

            //        }
            //    }
            //}
        }

        private void MoveNext(Direction direction) {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Direction SwitchedDirection(Direction direction) {
            return direction == Direction.Forward
                ? Direction.Backward
                : Direction.Forward;
        }
    }
}