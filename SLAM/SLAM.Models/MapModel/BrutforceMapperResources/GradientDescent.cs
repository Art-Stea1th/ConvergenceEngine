using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SLAM.Models.MapModel.BrutforceMapperResources {

    internal sealed class GradientDescent {

        public enum Direction { Forward, Backward }

        private int maxIterations;

        public void Proceed(Func<bool> NeedImproved, Func<Direction, bool> Improved) {

            throw new NotImplementedException();

            Direction direction = Direction.Forward;

            while (NeedImproved()) {

                if (!Improved(direction)) {

                    direction = SwitchedDirection(direction);

                }
                else {
                    
                }
            }
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