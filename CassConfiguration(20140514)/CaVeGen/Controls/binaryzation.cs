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
    //[ToolboxBitmapAttribute(typeof(U), "Beeline.BMP")]
    public partial class binaryzation : BaseControl
    {
        public binaryzation()
        {
            InitializeComponent();
            //base.inputbitmap
        }

        public int threshold=0;
        public string type = "binaryzation";

        [Category("控件属性")]
        [Description("二值化阈值设置")]
        public int Threshold
        {
            set
            {
                threshold = value;
            }
            get
            {
                return threshold;
            }
        }

        public string Type
        {
            get {
                return type;
            }
        }
        //Bitmap inputbitmap;
       
        //Bitmap showImage=null;
       
        //int[,] dealtData;
        public void Binaryzation()
        {
            //if (inputbitmap != null)
            //{
            //    dealtData=new int[inputbitmap.Width,inputbitmap.Height];
            //    for (int y = 0; y < inputbitmap.Height; y++)
            //    {
            //        for (int x = 0; x < inputbitmap.Width; x++)
            //        {
            //            if (dealtData[y, x] < threshold)
            //                dealtData[y, x] = 0;
            //            else
            //                dealtData[y, x] = 255;
            //        }
            //    }

            //    /*图像边界全涂为0*/
            //    for (int x = 0; x < inputbitmap.Width; x++)
            //    {
            //        dealtData[0, x] = 255;

            //        dealtData[inputbitmap.Height - 1, x] = 255;
            //    }
            //    for (int y = 0; y < inputbitmap.Height; y++)
            //    {
            //        dealtData[y, 0] = 255;
            //        dealtData[y, inputbitmap.Width - 1] = 255;
            //    }

            //}
        }
    }
}
