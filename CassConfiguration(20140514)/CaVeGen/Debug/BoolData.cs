using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaVeGen.Debug
{
    class BoolData:DataSturctBase
    {
        public string TargetValue;
        public string ActualValue;
        public string Result;

        public BoolData()
        {
          // this.Result =  base.result;
        }

        public override string GetResult()
        {
            return base.result;
        }


    }
}
