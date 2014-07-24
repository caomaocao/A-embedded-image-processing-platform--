using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaVeGen.Debug
{
    class DataSturctBase
    {
        public string result;

        public DataSturctBase()
        {

        }

        public virtual string GetResult()
        {
           return  this.result;
        }

    }
}
