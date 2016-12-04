using System.Windows;


namespace SLAM.Models.Map {

    using Data.Readers;

    internal abstract class BaseMaper {

        protected DataProvider DataProvider { get; private set; }

        public int ActualWidth { get; protected set; }
        public int ActualHeight { get; protected set; }
        public Point[] ResultMap { get; protected set; }        

        internal BaseMaper(DataProvider dataProvider) {
            DataProvider = dataProvider;
            DataProvider.OnNextFrameReady += NextFrameProceed;
        }

        protected abstract void NextFrameProceed();
    }
}