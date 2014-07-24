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
    public partial class ErodeControl : BaseControl
    {
        public ErodeControl()
        {
            InitializeComponent();
        }

        //Bitmap inputbitmap = null;
        //public Bitmap InputPic
        //{
        //    get
        //    {
        //        return inputbitmap;
        //    }
        //    set
        //    {

        //        inputbitmap = value;
                
        //    }
        //}
        ////int[,] dealtData;
        //Bitmap showImage = null;
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
        //int[,] showData;
        //public void Erode()
        //{
        //    int height = inputbitmap.Height;
        //    int width = inputbitmap.Width;
        //    showData = new int[width, height];
        //    int flag;
        //    int []pix=new int[5];

        //    for(int i=0;i<height;i++)
        //    {
        //        for (int j = 0; j < width; j++)
        //        {
        //            showData[j, i] = inputbitmap.GetPixel(j,i).G;
        //        }
        //    }

        //    for(int i=0;i<height-1;i++)                                      //edge problem...
        //    {
        //        for(int j=2;j<width-2;j++)
        //        {
        //            flag=1;
        //            pix[0] = inputbitmap.GetPixel(j, i).G;
        //            pix[1] = inputbitmap.GetPixel(j+1, i).G;
        //            pix[2] = inputbitmap.GetPixel(j+2, i).G;
        //            pix[3] = inputbitmap.GetPixel(j-1, i).G;
        //            pix[4] = inputbitmap.GetPixel(j-2, i).G; 
        //            for(int k=0;k<5;k++)
        //            {
        //                if((pix[k]==255))
        //                {
        //                    flag=0;
        //                    break;
        //                }
        //            }
        //            if(flag==0)
        //            {
        //                pix[0]=255;
        //                showData[j,i]=255;
        //            }
        //            else
        //            {
        //                pix[0]=0;
        //                showData[j, i] = 0;
        //            }
        //        }
        //    }
        //}
    }
}
