using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CassControls
{
    public partial class BaseControl : UserControl
    {
        public BaseControl()
        {
            InitializeComponent();
        }
        public Bitmap inputbitmap = null;
        public Bitmap InputPic
        {
            set
            {

                inputbitmap = value;
            }
            get
            {
                return inputbitmap;
            }
        }
        public  Bitmap showImage = null;
        public Bitmap OutputPic
        {
            get
            {

                return showImage;
            }
            set
            {
                showImage = value;
            }
        }

        private void BaseControl_Resize(object sender, EventArgs e)
        {
            this.Size = new Size(80,50);
        }

     
    }
}
