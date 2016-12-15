using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.Map.BrutforceMapperResources {

    internal sealed class VirtualOdometry {

        private Point previousOffset;
        private double previousAngle;

        private Point currentOffset;
        private double currentAngle;

        private Point expectedOffset;
        private double expectedAngle;        

        public double ExpectedX { get { return expectedOffset.X; } }
        public double ExpectedY { get { return expectedOffset.Y; } }
        public double ExpectedA { get { return expectedAngle; } }

        public void SetLastMove(Point lastOffset, double lastAngle) {
            ShiftInternalData();
            currentOffset = lastOffset;
            currentAngle = lastAngle;
            CalculateExpectedMove();
        }

        public void SetLastMove(double lastOffsetX, double lastOffsetY, double lastAngle) {
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
            expectedAngle = currentAngle * accelerationFactorA;
        }
    }
}