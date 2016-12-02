using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SLAM.Models {

    using Data;
    using Map;

    public delegate void ModelUpdatedEvent();

    public class Model {

        public event ModelUpdatedEvent OnModelUpdated;

        private IDataProvider  dataProvider;
        private IMaper         mapper;
        private FramesProvider framesProvider;



    }
}