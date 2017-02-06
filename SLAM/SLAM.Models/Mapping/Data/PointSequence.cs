using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SLAM.Models.Mapping.Data {

    using Navigation;

    internal sealed class PointSequence : IList<Point>, ICollection<Point>, IEnumerable<Point> {

        private List<Point> sequence;
        private Skeleton navSkeleton;                

        internal Skeleton GetNavSkeleton() {
            return navSkeleton;
        }

        public static explicit operator List<Point>(PointSequence frame) {
            return frame.sequence.ToList();
        }

        public static explicit operator Point[] (PointSequence frame) {
            return frame.sequence.ToArray();
        }

        public static implicit operator PointSequence(List<Point> sequence) {
            return CreateFrom(sequence);
        }

        public static implicit operator PointSequence(Point[] sequence) {
            return CreateFrom(sequence);
        }

        public static PointSequence CreateFrom(IEnumerable<Point> sequence) {
            return new PointSequence(sequence);
        }

        private PointSequence(IEnumerable<Point> sequence) {
            this.sequence = new List<Point>(sequence);
        }

        #region IList, ICollection, IEnumerable

        public bool IsReadOnly { get { return false; } }
        public int Count { get { return sequence.Count; } }
        public Point this[int index] { get { return sequence[index]; } set { sequence[index] = value; } }

        public void Add(Point item) {
            sequence.Add(item);
        }

        public void Clear() {
            sequence.Clear();
        }

        public bool Contains(Point item) {
            return sequence.Contains(item);
        }

        public void CopyTo(Point[] array, int arrayIndex) {
            sequence.CopyTo(array, arrayIndex);
        }

        public bool Remove(Point item) {
            return sequence.Remove(item);
        }

        public int IndexOf(Point item) {
            return sequence.IndexOf(item);
        }

        public void Insert(int index, Point item) {
            sequence.Insert(index, item);
        }

        public void RemoveAt(int index) {
            sequence.RemoveAt(index);
        }

        public IEnumerator<Point> GetEnumerator() {
            return sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return sequence.GetEnumerator();
        }
        #endregion
    }
}