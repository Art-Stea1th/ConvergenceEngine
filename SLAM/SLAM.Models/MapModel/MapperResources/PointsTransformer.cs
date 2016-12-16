using System.Windows;
using System.Windows.Media;

namespace SLAM.Models.MapModel.MapperResources {

    internal sealed class PointsTransformer {               

        public void Transform(Point[] points, float x, float y, float angle) {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            matrix.Translate(x, y);
            matrix.Transform(points);
        }

        public void Rotate(Point[] points, float angle) {
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            matrix.Transform(points);
        }

        public void ShiftXY(Point[] points, float x, float y) {
            Matrix matrix = new Matrix();
            matrix.Translate(x, y);
            matrix.Transform(points);
        }

        public void ShiftX(Point[] points, float x) {
            Matrix matrix = new Matrix();
            matrix.Translate(x, 0.0);
            matrix.Transform(points);
        }

        public void ShiftY(Point[] points, float y) {
            Matrix matrix = new Matrix();
            matrix.Translate(0.0, y);
            matrix.Transform(points);
        }        
    }
}