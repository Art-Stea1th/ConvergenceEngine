using System.Windows;


namespace SLAM.Models.Map.BrutforceMapperResources {

    internal sealed class VirtualOdometry {

        private Point oldestOffset;
        private double oldestAngle;

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

        public void SetLastMove(double lastX, double lastY, double lastAngle) {
            ShiftInternalData();
            currentOffset.X = lastX;
            currentOffset.Y = lastY;
            currentAngle = lastAngle;
            CalculateExpectedMove();
        }

        private void ShiftInternalData() {
            oldestOffset = previousOffset;
            oldestAngle = previousAngle;
            previousOffset = currentOffset;
            previousAngle = currentAngle;
        }

        private void CalculateExpectedMove() {
            var firstOffsetDiff = previousOffset - oldestOffset;
            var firstAngleDiff = previousAngle - oldestAngle;
            var secondOffsetDiff = currentOffset - previousOffset;
            var secondAngleDiff = currentAngle - previousAngle;

            var accelerationPercentX = firstOffsetDiff.X == 0.0 ? 100.0 : (secondOffsetDiff.X / firstOffsetDiff.X) * 100.0;
            var accelerationPercentY = firstOffsetDiff.Y == 0.0 ? 100.0 : (secondOffsetDiff.Y / firstOffsetDiff.Y) * 100.0;
            var accelerationPercentA = firstAngleDiff == 0.0 ? 100.0 : (secondAngleDiff / firstAngleDiff) * 100.0;

            expectedOffset.X = currentOffset.X * 0.01 * accelerationPercentX;
            expectedOffset.Y = currentOffset.Y * 0.01 * accelerationPercentY;
            expectedAngle = currentAngle * 0.01 * accelerationPercentA;
        }
    }
}