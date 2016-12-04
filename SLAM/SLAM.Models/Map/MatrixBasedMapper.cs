using System;


namespace SLAM.Models.Map {

    using Data.Readers;
    using Data.Adapters;
    

    internal sealed class MatrixBasedMapper : BaseMaper {

        public MatrixBasedMapper(DataProvider dataProvider) : base(dataProvider) { }

        protected override void NextFrameProceed() {
            //throw new NotImplementedException();
        }
    }
}