using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SLAM.Models.MapModel.ComplexMapperResources {

    internal sealed class VirtualOdometry {

        private List<Point>  offsetHistory;
        private List<double> rotateHistory;

        public VirtualOdometry() {
            offsetHistory = new List<Point>();
            rotateHistory = new List<double>();
        }

        public Tuple<Point, double> this[int index] { get { return GetMove(index); } }

        public int Length { get { return offsetHistory.Count; } }

        public void AddMove(double offsetX, double offsetY, double rotateAngle) {
            offsetHistory.Add(new Point(offsetX, offsetY));
            rotateHistory.Add(rotateAngle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Tuple<Point, double> GetMove(int index) {
            return
                new Tuple<Point, double>(
                        new Point(offsetHistory[index].X, offsetHistory[index].Y),
                        rotateHistory[index]);
        }
    }
}