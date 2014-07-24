using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CaVeGen.DesignViewFiles.CodeEditor
{
    public partial class ConfigurationCodeEdit : UserControl
    {
        public ConfigurationCodeEdit()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            PLCCodeEditor.GetInfomation();
            PLCCodeEditor.SetPLCLanguage();
            this.plcCodeEditor1.Language = "PLCLan";
        }

         public PLCCodeEditor CodeEditor
        {
            get
            {
                return this.plcCodeEditor1;
            }
            set
            {
                plcCodeEditor1 = value;
            }
        }

    }
}
