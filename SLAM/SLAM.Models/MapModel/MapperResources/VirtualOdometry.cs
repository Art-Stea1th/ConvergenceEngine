using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.MapModel.MapperResources {

    internal sealed class VirtualOdometry {

        private Point previousOffset;
        private float previousAngle;

        private Point currentOffset;
        private float currentAngle;

        private Point expectedOffset;
        private float expectedAngle;        

        public float ExpectedX { get { return (float)expectedOffset.X; } }
        public float ExpectedY { get { return (float)expectedOffset.Y; } }
        public float ExpectedA { get { return expectedAngle; } }

        public bool Zero() {
            return expectedOffset.X == 0.0f && expectedOffset.Y == 0.0f && expectedAngle == 0.0f;
        }

        public void SetLastMove(Point lastOffset, float lastAngle) {
            ShiftInternalData();
            currentOffset = lastOffset;
            currentAngle = lastAngle;
            CalculateExpectedMove();
        }

        public void SetLastMove(float lastOffsetX, float lastOffsetY, float lastAngle) {
            ShiftInternalData();
            currentOffset.X = lastOffsetX;
            currentOffset.Y = lastOffsetY;
            currentAngle = lastAngle;
            CalculateExpectedMove();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShiftInternalData() {
            previousOffset = currentOffset;
            previousAngle = currentAngle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CalculateExpectedMove() {

            var accelerationFactorX = previousOffset.X == 0.0 ? 1.0 : (currentOffset.X / previousOffset.X);
            var accelerationFactorY = previousOffset.Y == 0.0 ? 1.0 : (currentOffset.Y / previousOffset.Y);
            var accelerationFactorA = previousAngle == 0.0 ? 1.0 : (currentAngle / previousAngle);

            expectedOffset.X = currentOffset.X * accelerationFactorX;
            expectedOffset.Y = currentOffset.Y * accelerationFactorY;
            expectedAngle = (float)(currentAngle * accelerationFactorA);
        }
    }
}