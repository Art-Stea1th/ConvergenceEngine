using ConvergenceEngine.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvergenceEngine.ViewModels {

    public abstract class ApplicationViewModel : ViewModelBase {        

        protected IDataProvider DataProvider { get; set; }
        protected IMappep Mapper { get; set; }
        protected IMap Map { get; set; }

        public abstract void Update(IMap map);
    }
}