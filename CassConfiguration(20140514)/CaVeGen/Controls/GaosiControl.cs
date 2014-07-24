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
    public partial class GaosiControl : BaseControl
    {
        public GaosiControl()
        {
            InitializeComponent();
        }


        //static Bitmap inputbitmap = null;
        //public Bitmap InputPic
        //{
        //    set
        //    {

        //        inputbitmap = value;

        //    }
        //    get
        //    {
        //        return inputbitmap;
        //    }
        //}
        //static Bitmap showImage = null;
        //public Bitmap OutputPic
        //{
        //    get
        //    {
        //        return showImage;
        //    }
        //    set
        //    {
        //        showImage = value;
        //    }
        //}
        //int[,] orignalData;
        //int[,] dealtData;
        //public void Gaosi()
        //{
        //    int cur_value;
        //    int[] v = new int[9];
        //    int[] win = { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
        //    orignalData = new int[inputbitmap.Width, inputbitmap.Height];
        //    dealtData = new int[inputbitmap.Width, inputbitmap.Height];
        //    for (int y = 1; y < inputbitmap.Height - 1; y++)
        //    {
        //        for (int x = 1; x < inputbitmap.Width - 1; x++)
        //        {
        //            v[0] = orignalData[y - 1, x - 1];
        //            v[1] = orignalData[y - 1, x];
        //            v[2] = orignalData[y - 1, x + 1];
        //            v[3] = orignalData[y, x - 1];
        //            v[4] = orignalData[y, x];
        //            v[5] = orignalData[y, x + 1];
        //            v[6] = orignalData[y + 1, x - 1];
        //            v[7] = orignalData[y + 1, x];
        //            v[8] = orignalData[y + 1, x + 1];

        //            cur_value = win[0] * v[0] + win[1] * v[1] + win[2] * v[2]
        //                      + win[3] * v[3] + win[4] * v[4] + win[5] * v[5]
        //                      + win[6] * v[6] + win[7] * v[7] + win[8] * v[8];

        //            cur_value = cur_value / 16;

        //            dealtData[y, x] = cur_value;
        //        }
        //    }
        //}

    }
}
