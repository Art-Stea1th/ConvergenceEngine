namespace SLAM.Models.Map {

    using Data.Readers;

    internal abstract class BaseMaper {

        protected DataProvider DataProvider { get; private set; }      

        internal BaseMaper(DataProvider dataProvider) {
            DataProvider = dataProvider;
            DataProvider.OnNextFrameReady += NextFrameProceed;
        }

        protected abstract void NextFrameProceed();
    }
}