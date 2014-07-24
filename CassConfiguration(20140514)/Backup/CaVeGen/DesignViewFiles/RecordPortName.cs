using System;
using System.Collections.Generic;
using System.Text;

namespace CaVeGen.DesignViewFiles
{
    public class RecordPortName
    {
        private string originalName = "";

        private string portName = "";
        public string PortName
        {
            get
            {
                return this.portName;
            }
            set
            {
                this.portName = value;
            }
        }

        public RecordPortName()
        {
        }

        public RecordPortName(string str)
        {
            this.originalName = str;
        }
    }
}
