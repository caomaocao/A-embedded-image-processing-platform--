using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace CaVeGen.CommonOperation
{
    class ByteToImage
    {
        public Image AddHeader(byte[] imageDataDetails)
        {
            System.Drawing.Image img;
            try
            {

                MemoryStream ms = new MemoryStream();
                ms = new System.IO.MemoryStream(imageDataDetails);
                ms.Position = 0;
                img = System.Drawing.Image.FromStream(ms);
            }
            catch (Exception ex)
            {

            }
            /* if (img.Width > Convert.ToInt32(_width))
             {
                 _width = 500;
                 _height = (img.Height * Convert.ToInt32(_width)) / img.Width;
             }
             else
             {
                 _width = img.Width;
                 _height = img.Height;
             }*/
            // return img;
            return null;
            //  img.Dispose(); 
        }

        public Bitmap FromGray(byte[,] Gray)
        {

            int iWidth = Gray.GetLength(1);
            int iHeight = Gray.GetLength(0);
            Bitmap dstBitmap = CreateGrayscaleImage(iWidth, iHeight);
            BitmapData gbmData = dstBitmap.LockBits(new Rectangle(0, 0, iWidth, iHeight),
            ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            System.IntPtr ScanG = gbmData.Scan0;
            int posScan = 0;
            int gOffset = gbmData.Stride - iWidth;
            int scanBytes = gbmData.Stride*iHeight;
            byte[] pixel = new byte[scanBytes];
             for (int i = 0; i < iHeight; i++)
             {
                    for (int j = 0; j < iWidth; j++)
                    {
                        pixel[posScan++] = Gray[i, j];
                    }
                    posScan +=  gOffset;
             }
            /*
            {
                byte* g = (byte*)(void*)ScanG;
                // 注意这个地方图像的两维方向与数组两维的方向是转置的关系 
                for (int i = 0; i < iHeight; i++)
                {
                    for (int j = 0; j < iWidth; j++)
                    {
                        *g = (byte)Gray[i, j];
                        g++;
                    }
                    g += gOffset;
                }
            }
            */
            System.Runtime.InteropServices.Marshal.Copy(pixel,0,ScanG,scanBytes);
            dstBitmap.UnlockBits(gbmData);
            return dstBitmap;

        } //#

        /*********************************补充******************************************/

        /// <summary>

        /// Create and initialize grayscale image

        /// </summary>

        public static Bitmap CreateGrayscaleImage(int width, int height)
        {

            // create new image

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // set palette to grayscale

            SetGrayscalePalette(bmp);

            // return new image

            return bmp;

        }//#



        /// <summary>

        /// Set pallete of the image to grayscale

        /// </summary>

        public static void SetGrayscalePalette(Bitmap srcImg)
        {

            // check pixel format

            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)

                throw new ArgumentException();



            // get palette

            ColorPalette cp = srcImg.Palette;

            // init palette

            for (int i = 0; i < 256; i++)
            {

                cp.Entries[i] = Color.FromArgb(i, i, i);

            }

            // set palette back

            srcImg.Palette = cp;

        }//#



    }
}
