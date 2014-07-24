using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using CaVeGen;
using CaVeGen.CommonOperation;
using System.Text.RegularExpressions;
using CassGraphicsSystem.Project;
using System.Threading;
using System.Reflection;
using System.Resources;


namespace CaVeGen.ProjectControl
{
    public partial class ClassifyWorkpiece : Form
    {
        public ClassifyWorkpiece(string _savePath, string _programPath)
        {
            InitializeComponent();

            this.savePath = _savePath;
            this.programPath = _programPath;
            //初始化变量
            InitializeVariable();
          
        }

        public struct Record
        {
            public string recordName;           //操作名称
        //    public List< featherRgnstruct >feathervalue;  
        //    public int RowIndex;
        //    public int currentSizechange;
            public Bitmap bitmap;
        }
       // List<Record> featherRecordList;//用于特征点Undo
        List<Record> RecordList;      //用于回退二值化后的图片
        bool isMeasureLen = false;    //是否处于连线状态
 
        public string savePath = "D:\\Patch";   //默认的用户工程路径
        public string programPath = null;   //编程系统路径 ...\debug   20140218
        Bitmap bmp;
        bool isaddFeather = false;         //为true时表示进入特征点设计状态
        bool isSreenShot = false;           //为true时表示进入截图状态

        bool startdraw = false; //为真时在拖拉截图时绘制矩形
        bool startMeasure = false;  //开始测量

        static int cameraImagecount = 0;   //保存图片的序号
        
        public PictureBox picturebox = new PictureBox();  //主面板上显示图片的容器

        SerialPort serialport = new SerialPort();  //串口
        string CameraData = "0,0,0,0,0,0,0,0,0,";//是否是灰度图、色度亮度对比度饱和度 四个预留位
        Modbus modbus;                       //声明Modbus对象

        bool DrawMeasure = false;            //是否已经测量
        bool DrawBorder = false;              //是否已经截框

        bool DrawCurrentCellRec = false;      //是否绘制当前选中的特征点
        Rectangle CurrentCellRec;             //绘制当前选中框的矩形

        Point startpoint;                //有mousedown事件触发时，记录鼠标坐标
        Point endpoint;                   //mouseup事件触发时，记录鼠标坐标
        Size featherSize = new Size(3, 3);   //显示特征点矩形的大小

        Point lefttop;              //图像左上角坐标
        Point rightbottom;        //图像右下角坐标
        Point currentMouse;        //鼠标在图像中的位置

        Point measureStartPoint;   //测量时开始点
        Point measureEndPoint;      //测量时结束点

        Size normalSize;

        int leftX = 0, leftY = 0, rightX=0, rightY=0;//当前图片相对于原始图像的坐标
        int SizeChangecount = 0; //图片放大缩小比
        public struct featherRgnstruct     //特征点结构体
        {
            public int classifyNUM;
            public Rectangle rec;
            public int featherNum;
        }
        Bitmap showBmp; //存储当前图像
        List<featherRgnstruct> featherRegion = new List<featherRgnstruct>();
        List<featherRgnstruct> showfeature = new List<featherRgnstruct>();

        int[] OriginalData = new int[1038];

        //double ScreenSizePer;
        ToolTip tooltip;
        void InitializeVariable()   //初始化变量
        {
            picturebox.MouseDown += new MouseEventHandler(pictureBox_MouseDown);   //鼠标点击事件
            picturebox.MouseMove += new MouseEventHandler(pictureBox_MouseMove);
            picturebox.MouseUp += new MouseEventHandler(pictureBox_MouseUp);
            picturebox.MouseLeave += new EventHandler(picturebox_MouseLeave);
            picturebox.Paint += new PaintEventHandler(AutoPaintRectangle);

            tooltip = new ToolTip();   //提示信息
            
            //featherRecordList = new List<Record>();
            RecordList = new List<Record>();   //用于保存二值化图片
            #region
            //if (!File.Exists(currentPath + "\\feather.h"))
            //{
            //    string InitFeather = null;
            //    for (int i = 0; i < 1037; i++)
            //    {
            //        InitFeather += "0,";
            //        OriginalData[i] = 0;
            //    }
            //    InitFeather += "0";
            //    string data = " image_data_packet_desc_type  image_data_packet_desc={" + InitFeather + "};";
            //    StreamWriter streamwriter = File.CreateText(currentPath + "\\" + "feather.h");
            //    streamwriter.WriteLine(data);
            //    streamwriter.Close();
            //}
            //else
            //{
            //    try
            //    {
            //        string streamreader = File.ReadAllText(currentPath + "\\" + "feather.h");
            //        string dealtdata = CommonOperation.StringOperateHelp.RightOf(streamreader, '{');
            //        dealtdata = CommonOperation.StringOperateHelp.LeftOf(dealtdata, '}');
            //        string[] featherdata = dealtdata.Split(',');
            //        for (int i = 0; i < 1038; i++)
            //        {
            //            if (featherdata[i] != "")
            //            {
            //                OriginalData[i] = int.Parse(featherdata[i]);

            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        MessageBox.Show(e.ToString());
            //    }
            //}
            #endregion
            FeatherdataGrid.ColumnHeadersVisible = false;
            FeatherdataGrid.RowHeadersVisible = false;
          //  savePath = currentPath;

        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            // fm.Refresh();
            Point pt = new Point(e.X, e.Y);
            List<featherRgnstruct> backupfeathureRegion = new List<featherRgnstruct>();
            if (e.Button == MouseButtons.Left && isSreenShot)
            {
                startpoint = pt;
                startdraw = true;
                //isDraw = true;
            }
            else if (e.Button == MouseButtons.Left && isaddFeather)
            {
                Rectangle rec = new Rectangle(pt.X, pt.Y, featherSize.Width, featherSize.Height);
                featherRgnstruct tmpfeather = new featherRgnstruct();
                tmpfeather.featherNum = defaultValue;
                tmpfeather.rec = rec;
                Graphics g = (sender as PictureBox).CreateGraphics();

                if (FeatherdataGrid.Rows.Count < 31)
                {
                    g.DrawRectangle(Pens.Blue, new Rectangle(rec.X - (featherSize.Width-1) / 2 , rec.Y - (featherSize.Height-1) / 2 , featherSize.Width, featherSize.Height));
                    
                    showfeature.Add(tmpfeather);
  
                    tmpfeather.rec.X = (int)(tmpfeather.rec.X * Math.Pow(2, -SizeChangecount));
                    tmpfeather.rec.Y = (int)(tmpfeather.rec.Y * Math.Pow(2, -SizeChangecount));
                    FeatherdataGrid.Rows.Add(tmpfeather.rec.X, tmpfeather.rec.Y, tmpfeather.featherNum);
                    featherRegion.Add(tmpfeather);
                }
                else
                {
                   //特征点上限不超过30
                    MessageBox.Show("绘制的特征点数超过限制");
                }

            }
            else if (e.Button == MouseButtons.Left && isMeasureLen)
            {
                startMeasure = true;
                measureStartPoint = new Point((int)(e.X*Math.Pow(2,-SizeChangecount )), (int)(e.Y*Math.Pow(2,-SizeChangecount )));
               
            }
            else if (e.Button == MouseButtons.Right)//&&!isaddFeather&&!isSreenShot)
            {

            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            currentMouse = new Point(e.X, e.Y);
            tooltip.SetToolTip(picturebox,e.X.ToString()+","+e.Y.ToString());
            if (bmp != null)
            {
                //Notice.Text = "picture size:" + bmp.Size.Width.ToString() + "," + bmp.Size.Height.ToString() + ";\r" + "current mouse position:" + e.X.ToString() + "," + e.Y.ToString() + ";";
            }
            //locationLabel.Text = currentMouse.X.ToString() + "," + currentMouse.Y.ToString();
            if (isSreenShot||isMeasureLen)
            {
                (sender as PictureBox).Invalidate();
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            #region 截图
            if (isSreenShot)
            {
                endpoint = new Point(e.X, e.Y);
                DrawBorder = true;

                try
                {
                    CutGrouptextBox1.Text = (startpoint.X).ToString() + "," + (startpoint.Y).ToString();
                    CutGrouptextBox2.Text = (endpoint.X).ToString() + "," + (endpoint.Y).ToString();
                }
                catch (Exception ex2)
                {

                }
                leftX = (int)(startpoint.X * Math.Pow(2, -SizeChangecount));
                leftY = (int)(startpoint.Y * Math.Pow(2, -SizeChangecount));
                rightX = (int)(endpoint.X * Math.Pow(2, -SizeChangecount));
                rightY = (int)(endpoint.Y * Math.Pow(2, -SizeChangecount));

                picturebox.Invalidate();
            }

            #endregion

            #region 测量
            if (isMeasureLen)
            {
                measureEndPoint = new Point((int)(e.X*Math.Pow(2,-SizeChangecount )), (int)(e.Y*Math.Pow(2,-SizeChangecount )));
                DrawMeasure = true;
                startMeasure = false;
                isMeasureLen = false;
                Notice.Text = "current length:" + Math.Sqrt(Math.Pow(e.Y - measureStartPoint.Y, 2) + Math.Pow(e.X - measureStartPoint.X, 2)).ToString();
            }
            startdraw = false;
            startpoint = new Point(0, 0);
            #endregion

        }

        private void picturebox_MouseLeave(object sender, EventArgs e)
        {

        }

        private void AutoPaintRectangle(object sender, PaintEventArgs e)
        {
            int x, y, width, height;
            //(sender as ).Invalidate();

            foreach (featherRgnstruct tmpfeather in showfeature)   //绘制特征点
            {
                 Rectangle tmprec = tmpfeather.rec;
                    x = tmprec.X;
                    y = tmprec.Y;
                    width = tmprec.Width;
                    height = tmprec.Height;
                    Rectangle rec = new Rectangle(x - (width - 1) / 2, y - (height - 1) / 2, width, height);
                  e.Graphics.DrawRectangle(Pens.Blue, rec);
            }
            
            if (DrawCurrentCellRec&&featherRegion.Count>0&&showfeature.Count!=0&&isaddFeather) //绘制表格中选中的特征点
            {
                CurrentCellRec = new Rectangle((int)(int.Parse(FeatherdataGrid.Rows[FeatherdataGrid.CurrentRow.Index].Cells[0].Value.ToString())*Math.Pow(2,SizeChangecount)) - 1,
              (int)(int.Parse(FeatherdataGrid.Rows[FeatherdataGrid.CurrentRow.Index].Cells[1].Value.ToString()) * Math.Pow(2, SizeChangecount)) - 1, 3, 3);
                e.Graphics.DrawRectangle(Pens.Red, CurrentCellRec);
            }

            if (DrawBorder)                   //绘制框架
            {
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)(leftX * Math.Pow(2, SizeChangecount)), (int)(leftY * Math.Pow(2, SizeChangecount)),
                    (int)((rightX - leftX) * Math.Pow(2, SizeChangecount)), (int)((rightY - leftY) * Math.Pow(2, SizeChangecount))));
            }
            if (isMeasureLen&&startMeasure)       //实时绘制尺寸的线
            {
                  e.Graphics.DrawLine(Pens.Red, measureStartPoint, currentMouse);
            
            }
            if (DrawMeasure)                    //绘制尺寸的线
            {
                Point tmpStart = new Point((int)(measureStartPoint.X * Math.Pow(2, SizeChangecount)), (int)(measureStartPoint.Y * Math.Pow(2, SizeChangecount)));
                Point tmpEnd = new Point((int)(measureEndPoint.X * Math.Pow(2, SizeChangecount)), (int)(measureEndPoint.Y * Math.Pow(2, SizeChangecount)));
                e.Graphics.DrawLine(Pens.Red, tmpStart, tmpEnd);
            }
            if (isSreenShot && startdraw)        //实时绘制截图边框
            {
                e.Graphics.DrawRectangle(Pens.Gray, new Rectangle(startpoint, new Size(currentMouse.X - startpoint.X, currentMouse.Y - startpoint.Y)));
            }
        }

        public static Bitmap CutImage(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)  //截图函数
        {
            if (b == null)
            {
                return null;
            }

            int w = b.Width;
            int h = b.Height;
            byte[,] data = new byte[iHeight,iWidth];

            if (StartX >= w || StartY >= h)
            {
                return null;
            }

            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }

            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }
            ByteToImage bytetoimage=new ByteToImage();
            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);

                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();

                for (int i = 0; i < iHeight; i++)
                    for (int j = 0; j < iWidth; j++)
                    {
                        data[i, j] =(byte)bmpOut.GetPixel(j, i).R;
                    }
                return bytetoimage.FromGray(data);
            }
            catch
            {
                return null;
            }
        }

        //保存边框数据和特征值数据
        private void SaveFeather()
        {
            StreamWriter streamwriter;
            string alltext = null;
            string data = null;
            string VersonNum = "0,";
            if (File.Exists(savePath + "\\feather.h"))
                File.Delete(savePath + "\\feather.h");
            streamwriter = File.CreateText(savePath + "\\" + "feather.h");
            alltext += VersonNum;
            alltext += CameraData;//6个数字（是否是灰度图，色度，亮度，对比度，饱和度 外加四个预留位）
            alltext += leftX.ToString() + ',' + leftY.ToString() + ',' + rightX.ToString() + ',' + rightY.ToString()+',';//由于只进行单工件的识别，识别区域和截图区域参数相同
            alltext += leftX.ToString() + ',' + leftY.ToString() + ',' + Math.Abs(rightX-leftX).ToString() + ',' + Math.Abs(rightY-leftY).ToString() ;

            Bitmap tmpbmp = ((Bitmap)picturebox.Image);
            foreach (featherRgnstruct feather in featherRegion)
            {
                Rectangle rec = feather.rec;

                alltext += ","+rec.X.ToString() + "," + rec.Y.ToString() + "," + tmpbmp.GetPixel(rec.X, rec.Y).R.ToString() + "," + feather.featherNum;

            }
            for (int i = 0; i < 300 - featherRegion.Count; i++)
            {
                alltext += ",0,0,0,0";
            }
            for (int i = 0; i < 2709; i++)
            {
                alltext += ",0,0,0,0";
            }

                data += " image_data_packet_desc_type  image_data_packet_desc={" + alltext + "};";
            streamwriter.WriteLine(data);
            streamwriter.Close();
        }

        private void MatchFM_Resize(object sender, EventArgs e) //
        {

        }

        private void MatchFM_FormClosed(object sender, FormClosedEventArgs e)
        {
         
        }

        private void Closebt_Click(object sender, EventArgs e)   //点击关闭按钮
        {
            //DialogResult result = MessageBox.Show("是否确定退出", "退出", MessageBoxButtons.OKCancel);
            //if (result.Equals(DialogResult.OK))
            //{
            //    this.Close();
            //}
            DialogResult result = MessageBox.Show("是否保存特征和边框信息", "保存", MessageBoxButtons.YesNoCancel);
            if (result.Equals(DialogResult.Yes))  //点击yes保存信息并退出
            {
                SaveFeather();
                this.Close();
            }
            else if (result.Equals(DialogResult.No))
            {
                this.Close();
            }
        }

        private void PicFromLocality_Click(object sender, EventArgs e)  //打开本地图片
        {
            featherRegion.Clear();     
            FeatherdataGrid.Rows.Clear();
            showfeature.Clear();
            leftX = 0;
            leftY = 0;
            rightX = 0;
            rightY = 0;             //打开本地图片前，先清空截图信息以及特征点信息

            picturebox.Parent = Mainpanel;          //把picturebox绑定到主panel上

            OpenFileDialog openFileDialog = new OpenFileDialog();   //弹出openfiledialog
            openFileDialog.Filter = "bmp files (*.bmp)|*.bmp";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)    //点击确定按钮读入一张图片
            {
                bmp = (Bitmap)Image.FromFile(openFileDialog.FileName);
                lefttop = new Point(0, 0);                  //设置图片参数
                rightbottom = new Point(bmp.Size.Width, bmp.Size.Height);  

                picturebox.Image = bmp;    
                showBmp = bmp;
                if (rightX >= bmp.Width || rightY >= bmp.Height)
                {
                    rightX = bmp.Width;
                    rightY = bmp.Height;
                }

                picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
                picturebox.Size = showBmp.Size;
                normalSize = showBmp.Size;
                if (Mainpanel.Width > bmp.Width&&Mainpanel.Height>bmp.Height)
                {
                    picturebox.Location = new Point((Mainpanel.Width - bmp.Size.Width) / 2, (Mainpanel.Height - bmp.Size.Height) / 2);

                }
                else if (Mainpanel.Width > bmp.Width)
                {
                    picturebox.Location = new Point((Mainpanel.Width - bmp.Size.Width) / 2, 0);

                }
                else if (Mainpanel.Height > bmp.Height)
                {
                    picturebox.Location = new Point(0, (Mainpanel.Height - bmp.Size.Height) / 2);

                }
                else
                {
                    picturebox.Location = new Point(0, 0);
                }
                picturebox.Show();                               
                SizeChangecount = 0;
                Notice.Text = null;

            }
        }

        private void amplification_Click(object sender, EventArgs e)  //点击按钮将图片放大两倍
        {
            #region
            //if (isAsScreemsize)
            //{
            //    FeatherdataGrid.Rows.Clear();
            //    foreach (featherRgnstruct tmp in featherRegion)
            //    {
            //        FeatherdataGrid.Rows.Add(tmp.rec.X, tmp.rec.Y, tmp.featherNum);
            //    }
            //    picturebox.Parent = Mainpanel;
            //    SizeChangecount = 0;
            //    picturebox.Size = normalSize;
            //    showfeature = featherRegion;
            //    if (Mainpanel.Width - picturebox.Size.Width > 0)
            //    {
            //        picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, (Mainpanel.Height - picturebox.Size.Height) / 2);

            //    }
            //    else//若图像大于form，则图像位置为（0,0）
            //    {
            //        picturebox.Location = new Point(0, 0);
            //    }
            //    isAsScreemsize = false;
            //    Notice.Text = "current bmp size:" + picturebox.Size.Width.ToString() + "," + picturebox.Size.Height.ToString();
            //}
            #endregion
            if (SizeChangecount < 3)
            {
                Mainpanel.AutoScroll = true;              //校准图片左上角位置，使图片与panel左上角对其
                picturebox.Parent = splitContainer2.Panel1;
                picturebox.Parent = Mainpanel;
                //MainPanel.AutoScrollOffset = new Point(0, 0);
                picturebox.Size = new Size(picturebox.Size.Width * 2, picturebox.Size.Height * 2);
                if (Mainpanel.Width - picturebox.Size.Width > 0)
                {
                    picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, (Mainpanel.Height - picturebox.Size.Height) / 2);

                }
                else//若图像大于form，则图像位置为（0,0）
                {
                    picturebox.Location = new Point(0, 0);
                }
                Notice.Text = "current bmp size:" + picturebox.Size.Width.ToString() + "," + picturebox.Size.Height.ToString();
                if (isSreenShot)
                {
                    CutGrouptextBox1.Text = (int.Parse(StringOperateHelp.LeftOf(CutGrouptextBox1.Text, ',')) * 2).ToString() + "," +
                        (int.Parse(StringOperateHelp.RightOf(CutGrouptextBox1.Text, ',')) * 2).ToString();
                    CutGrouptextBox2.Text = (int.Parse(StringOperateHelp.LeftOf(CutGrouptextBox2.Text, ',')) * 2).ToString() + "," +
                        (int.Parse(StringOperateHelp.RightOf(CutGrouptextBox2.Text, ',')) * 2).ToString();
                }
                SizeChangecount++;
                if (isaddFeather)
                {
                    //FeatherdataGrid.Rows.Clear();
                    List<featherRgnstruct> backupFeather = new List<featherRgnstruct>();
                    foreach (featherRgnstruct tmp in featherRegion)
                    {
                        featherRgnstruct feather = tmp;
                        feather.rec.Location = new Point((int)(tmp.rec.Location.X * Math.Pow(2, SizeChangecount)), (int)(tmp.rec.Location.Y * Math.Pow(2, SizeChangecount)));
                        backupFeather.Add(feather);
                    }
                    showfeature = backupFeather;
                }
                picturebox.Invalidate();

            }
        }

        private void lessen_Click(object sender, EventArgs e)   //点击缩小图片按钮
        {
            if (SizeChangecount > -3)
            {
                picturebox.SizeMode = PictureBoxSizeMode.StretchImage;    
                picturebox.Parent = splitContainer2.Panel1;
                picturebox.Parent = Mainpanel;                   //校准图片，使图片与panel左上角对齐
                picturebox.Size = new Size(picturebox.Size.Width / 2, picturebox.Size.Height / 2);
                if (picturebox.Size.Width > Mainpanel.Width && picturebox.Size.Height > Mainpanel.Height)  //使图片保持在panel正中心或者左上角位置
                    picturebox.Location = new Point(0, 0);
                else if (picturebox.Size.Width > Mainpanel.Width)
                    picturebox.Location = new Point(0, (Mainpanel.Height - picturebox.Size.Height) / 2);
                else if (picturebox.Size.Height > Mainpanel.Height)
                    picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, 0);
                else
                    picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, (Mainpanel.Height - picturebox.Size.Height) / 2);
                
                SizeChangecount--;

                Notice.Text = "current bmp size:" + picturebox.Size.Width.ToString() + "," + picturebox.Size.Height.ToString();

                if (isSreenShot)
                {
                    CutGrouptextBox1.Text = (int.Parse(StringOperateHelp.LeftOf(CutGrouptextBox1.Text, ',')) / 2).ToString() + "," +
                            (int.Parse(StringOperateHelp.RightOf(CutGrouptextBox1.Text, ',')) / 2).ToString();
                    CutGrouptextBox2.Text = (int.Parse(StringOperateHelp.LeftOf(CutGrouptextBox2.Text, ',')) / 2).ToString() + "," +
                            (int.Parse(StringOperateHelp.RightOf(CutGrouptextBox2.Text, ',')) / 2).ToString();
                }
                if (isaddFeather)
                {
                    //FeatherdataGrid.Rows.Clear();
                    List<featherRgnstruct> backupFeather = new List<featherRgnstruct>();
                    foreach (featherRgnstruct tmp in showfeature)
                    {
                        featherRgnstruct feather = tmp;
                        feather.rec.Location = new Point(tmp.rec.Location.X / 2, tmp.rec.Location.Y / 2);
                        backupFeather.Add(feather);
                    }
                    showfeature = backupFeather;
                }
                    picturebox.Invalidate();
            }
        }

        private void DefaultSize_Click(object sender, EventArgs e)    //将图片返回到原始大小
        {
            picturebox.Parent = null;                //校准图片
            picturebox.Parent = splitContainer2.Panel1;
            FeatherdataGrid.Rows.Clear();
            foreach (featherRgnstruct tmp in featherRegion)            //校准图片数据
            {
                FeatherdataGrid.Rows.Add(tmp.rec.X, tmp.rec.Y, tmp.featherNum);
            }
            picturebox.Size = normalSize;

            if (picturebox.Size.Width > Mainpanel.Width && picturebox.Size.Height > Mainpanel.Height)
                picturebox.Location = new Point(0, 0);
            else if (picturebox.Size.Width > Mainpanel.Width)
                picturebox.Location = new Point(0, (Mainpanel.Height - picturebox.Size.Height) / 2);
            else if (picturebox.Size.Height > Mainpanel.Height)
                picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, 0);
            else
                picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, (Mainpanel.Height - picturebox.Size.Height) / 2);

            //isAsScreemsize = false;
            picturebox.Parent = Mainpanel;
            SizeChangecount = 0;

            showfeature = featherRegion;
            Notice.Text = "current bmp size:" + picturebox.Size.Width.ToString() + "," + picturebox.Size.Height.ToString();

            //picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, (Mainpanel.Height - picturebox.Size.Height) / 2);
            if (isSreenShot)
            {
                CutGrouptextBox1.Text = leftX.ToString() + "," + leftY.ToString();
                CutGrouptextBox2.Text = rightX.ToString() + "," + rightY.ToString();
            }
        }

        private void AddPoint_Click(object sender, EventArgs e)
        {

            Formtimer.Stop();        
            if (isaddFeather == true)   //点击退出增加特征点状态
            {
                FeatherdataGrid.ReadOnly = true;      

                BinaryTool.Enabled = true;
                PicFromCamera.Enabled = true;
                PicFromLocality.Enabled = true;
                //SavePic.Enabled = true;
               // ReSet.Enabled = true;
                CutImage2.Enabled = true;
                AutoFeatherPoint.Enabled = true;
                DeleteFeather.Enabled = true;
                MainUndobutton.Enabled = true;
                CancleCut.Enabled = true;
                //MatchFeather.Enabled = true;
                OpenFeather.Enabled = true;

                isaddFeather = false;
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClassifyWorkpiece));
                (sender as Button).Image = ((System.Drawing.Image)(resources.GetObject("AddPoint.Image")));
                DialogResult result = MessageBox.Show("是否需要保存特征点"," ", MessageBoxButtons.OKCancel);
                if (result.Equals(DialogResult.OK))
                {
                    SaveFeather();
                }
            }
            else                          //点击进入特征点状态，关闭其他功能
            {
                FeatherdataGrid.ReadOnly = false;
                BinaryTool.Enabled = false;
                PicFromCamera.Enabled = false;
                PicFromLocality.Enabled = false;
                //SavePic.Enabled = false;
                CutImage2.Enabled = false;
                AutoFeatherPoint.Enabled = false;
                //ReSet.Enabled = false;
                DeleteFeather.Enabled = false;
                MainUndobutton.Enabled = false;
                CancleCut.Enabled = false;
               // MatchFeather.Enabled = false;
                OpenFeather.Enabled = false;

                isaddFeather = true;
                (sender as Button).Image = null;
            }
            picturebox.Invalidate();
        }

        private void DeleteFeather_Click(object sender, EventArgs e)   //点击删除特征点
        {
            FeatherdataGrid.Rows.Clear();
            showfeature.Clear();
            featherRegion.Clear();
            picturebox.Invalidate();
        }
        ProgressBar progressbar;
        private void PicFromCamera_Click(object sender, EventArgs e)   //点击通过串口获取一张图片，并关闭其他无关的按钮
        {
            featherRegion.Clear();
            FeatherdataGrid.Rows.Clear();
            showfeature.Clear();
            leftX = 0;
            leftY = 0;
            rightX = 0;
            rightY = 0;

            
            PicFromLocality.Enabled = false;
           // SavePic.Enabled = false;
            CutImage2.Enabled = false;
            AutoFeatherPoint.Enabled = false;
            //ReSet.Enabled = false;
            DeleteFeather.Enabled = false;
            MainUndobutton.Enabled = false;
            CancleCut.Enabled = false;
            //MatchFeather.Enabled = false;
            OpenFeather.Enabled = false;
            AddPoint.Enabled = false;
            BinaryTool.Enabled = false;

            PicFromCamera.Enabled = false;
            ReadImageFromCamera();
            PicFromCamera.Enabled = true;

            PicFromLocality.Enabled = true;
            //SavePic.Enabled = true;
            CutImage2.Enabled = true;
            AutoFeatherPoint.Enabled = true;
            //ReSet.Enabled = true;
            DeleteFeather.Enabled = true;
            MainUndobutton.Enabled = true;
            CancleCut.Enabled = true;
           // MatchFeather.Enabled = true;
            OpenFeather.Enabled = true;
            AddPoint.Enabled = true;
            BinaryTool.Enabled = true;
        }
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            progressbar.PerformStep();
        }
        static int currentimageCount = 0;
        private void SavePic_Click(object sender, EventArgs e)   //保存当前界面上的图片
        {
            bmp.Save(savePath + "\\currentImage" + currentimageCount++.ToString() + ".bmp",ImageFormat.Bmp);
        }

        private void CutImage2_Click(object sender, EventArgs e) //点击截图 清空所有之前的特征点
        {
            FeatherdataGrid.Rows.Clear();
            showfeature.Clear();
            featherRegion.Clear();

            DrawCurrentCellRec = false;

            BasegroupBox.Controls.Clear();
            InitializeCutImage(BasegroupBox);

            BinaryTool.Enabled = false;
            PicFromCamera.Enabled = false;
            PicFromLocality.Enabled = false;
            AutoFeatherPoint.Enabled = false;
            DeleteFeather.Enabled = false;
            MainUndobutton.Enabled = false;
            CancleCut.Enabled = false;
            OpenFeather.Enabled = false;
            AddPoint.Enabled = false;

            isSreenShot = true;

            DrawBorder=false;

            picturebox.Invalidate();
            
        }

        private void AutoFeatherPoint_Click(object sender, EventArgs e)//自动获取特征点
        {
            Formtimer.Stop();

            BinaryTool.Enabled = false;
            PicFromCamera.Enabled = false;
            PicFromLocality.Enabled = false;
            CutImage2.Enabled = false;
            DeleteFeather.Enabled = false;
            MainUndobutton.Enabled = false;
            CancleCut.Enabled = false;
            OpenFeather.Enabled = false;
            AddPoint.Enabled = false;
        
            BasegroupBox.Controls.Clear();
            InitializeAutoFeather(BasegroupBox);
            try
            {
                Bitmap img = CutImage((Bitmap)picturebox.Image, leftX, leftY, rightX - leftX, rightY - leftY);
             //   string currentSavePath = StringOperateHelp.LeftOfRightmostOf(savePath, '\\') + "\\Debug";
                if (File.Exists(savePath + "\\BmpforGenetic.bmp"))
                {
                    File.Delete(savePath + "\\BmpforGenetic.bmp");
                }

                img.Save(savePath + "\\BmpforGenetic1.bmp",ImageFormat.Bmp);
                img.Save(savePath + "\\BmpforGenetic2.bmp",ImageFormat.Bmp);
            }
            catch
            {
 
            }
        }

        private void MeasureLen_Click(object sender, EventArgs e)
        {
            isaddFeather = false;
            BasegroupBox.Controls.Clear();
            isMeasureLen = true;
            isSreenShot = false;
            DrawMeasure = false;

            picturebox.Invalidate();
        }

        private void CancleCut_Click(object sender, EventArgs e)     //取消截图，回退到最原始图片状态
        {
            Formtimer.Stop();
            showfeature.Clear();
            featherRegion.Clear();
            FeatherdataGrid.Rows.Clear();


            DrawBorder = false;
            DrawMeasure = false;
            DrawCurrentCellRec = false;
            //leftToptoolStripStatusLabel.Text = "0,0";
            //rightbottomtoolStripStatusLabel.Text = bmp.Width.ToString() + "," + bmp.Height.ToString();
            showBmp = bmp;

            leftX = 0;
            leftY = 0;
            rightX = bmp.Size.Width;
            rightY = bmp.Size.Height;

            picturebox.Image = bmp;
            normalSize = bmp.Size;
            lefttop = new Point(0, 0);
            rightbottom = new Point(bmp.Width, bmp.Height);
            picturebox.Size = showBmp.Size;

            if (Mainpanel.Width > bmp.Width && Mainpanel.Height > bmp.Height)
            {
                picturebox.Location = new Point((Mainpanel.Width - bmp.Size.Width) / 2, (Mainpanel.Height - bmp.Size.Height) / 2);

            }
            else if (Mainpanel.Width > bmp.Width)
            {
                picturebox.Parent = null;
                picturebox.Parent = Mainpanel;
                picturebox.Location = new Point((Mainpanel.Width - bmp.Size.Width) / 2, 0);

            }
            else if (Mainpanel.Height > bmp.Height)
            {
                picturebox.Parent = null;
                picturebox.Parent = Mainpanel;
                picturebox.Location = new Point(0, (Mainpanel.Height - bmp.Size.Height) / 2);

            }
            else
            {
                picturebox.Parent = null;
                picturebox.Parent = Mainpanel;
                picturebox.Location = new Point(0, 0);
            }
        }

        int Threshod;          //二值化阈值
        private void BinaryTool_Click(object sender, EventArgs e)   //进入二值化状态，关闭其他按钮
        {
            PicFromCamera.Enabled = false;
            PicFromLocality.Enabled = false;
            CutImage2.Enabled = false;
            AutoFeatherPoint.Enabled = false;
            DeleteFeather.Enabled = false;
            MainUndobutton.Enabled = false;
            CancleCut.Enabled = false;
            OpenFeather.Enabled = false;
            AddPoint.Enabled = false;

            Formtimer.Stop();
            BasegroupBox.Controls.Clear();
            InitializeBinary(BasegroupBox);

        }


        int defaultValue = 1;  //特征点权重默认为1；
        private void valueTextbox_TextChanged(object sender, EventArgs e) //改变特征点的权重
        {

            //Regex r = new Regex(@"\d");
            //if (!r.IsMatch(valueTextbox.Text))
            //{
            //    MessageBox.Show("请输入数字");
            //}
            try
            {
                defaultValue = int.Parse(valueTextbox.Text);
            }
            catch (Exception ex)
            {
                defaultValue = 1;
            }

        }

        private void MatchFeather_Click(object sender, EventArgs e) //点击识别图片
        {
            if(Formtimer.Enabled)
            {
                Formtimer.Enabled=false;
                BasegroupBox.Controls.Clear();
            }
            else
            {
                BasegroupBox.Controls.Clear();
                InitializeMatch(BasegroupBox);
            }
        }

        private void MainUndobutton_Click(object sender, EventArgs e)  //回退二值化图片操作
        {
            try
            {
                Record record = RecordList.Last();
                if (record.recordName == "binaryBmp")
                {
                    picturebox.Image = record.bitmap;
                }
                RecordList.Remove(record);

            }
            catch (Exception ex)
            {

            }
        }

        private void FeatherdataGrid_CellClick(object sender, DataGridViewCellEventArgs e)  //点击表格单元格绘制特征点框
        {
            picturebox.Invalidate();
            DrawCurrentCellRec = true;
            int currentRow = FeatherdataGrid.CurrentRow.Index;
            int locX = int.Parse(FeatherdataGrid.Rows[currentRow].Cells[0].Value.ToString());
            int locY = int.Parse(FeatherdataGrid.Rows[currentRow].Cells[1].Value.ToString());
            CurrentCellRec = new Rectangle(locX - 1, locY - 1, 3, 3);
        }
        
        #region 初始化右下角GroupBox控件
        Button AutoFeatherbutton1 = new Button();
        Label AutoFeatherlabel1 = new Label();
        TextBox AutoFeathertextBox1 = new TextBox();
        ComboBox AutoAddFeathercomboBox = new ComboBox();
        Label AutoFeatherlabel2 = new Label();

        Button OpenImgbutton1 = new Button();
        Label OpenImglabel1 = new Label();
        TextBox OpenImgtextBox1 = new TextBox();

        Button Binarybutton1 = new Button();
        Button Binarybutton2 = new Button();
        Label Binarylabel1 = new Label();
        TextBox BinarytextBox1 = new TextBox();

        Label CutGrouplabel1 = new Label();
        Button CutGroupbutton1 = new Button();
        TextBox CutGrouptextBox2 = new TextBox();
        TextBox CutGrouptextBox1 = new TextBox();
        Label CutGrouplabel2 = new Label();

        Button Matchbutton1 = new Button();
        Label Matchlabel1 = new Label();
        TextBox Matchtextbox1 = new TextBox();

        private void InitializeCutImage(GroupBox baseGroup)
        {

            CutGrouptextBox2.Text = (rightX * Math.Pow(2, SizeChangecount)).ToString() + "," + (rightY * Math.Pow(2, -SizeChangecount)).ToString();
            CutGrouptextBox1.Text = (leftX * Math.Pow(2, -SizeChangecount)).ToString() + "," + (leftY * Math.Pow(2, -SizeChangecount)).ToString();
            //}
            baseGroup.Controls.Add(CutGroupbutton1);
            baseGroup.Controls.Add(CutGrouptextBox2);
            baseGroup.Controls.Add(CutGrouptextBox1);
            baseGroup.Controls.Add(CutGrouplabel2);
            baseGroup.Controls.Add(CutGrouplabel1);
            baseGroup.Text = "Cut Image";
            // 
            // CutGrouplabel1
            // 
            CutGrouplabel1.AutoSize = true;
            CutGrouplabel1.Location = new System.Drawing.Point(16, 29);
            CutGrouplabel1.Name = "CutGrouplabel1";
            CutGrouplabel1.Size = new System.Drawing.Size(89, 12);
            CutGrouplabel1.TabIndex = 0;
            CutGrouplabel1.Text = "Left/Top Value";
            // 
            // CutGrouplabel2
            // 
            CutGrouplabel2.AutoSize = true;
            CutGrouplabel2.Location = new System.Drawing.Point(16, 86);
            CutGrouplabel2.Name = "CutGrouplabel2";
            CutGrouplabel2.Size = new System.Drawing.Size(113, 12);
            CutGrouplabel2.TabIndex = 1;
            CutGrouplabel2.Text = "Right/Bottom Value";
            // 
            // CutGrouptextBox1
            // 
            CutGrouptextBox1.Location = new System.Drawing.Point(60, 53);
            CutGrouptextBox1.Name = "CutGrouptextBox1";
            CutGrouptextBox1.Size = new System.Drawing.Size(100, 21);
            CutGrouptextBox1.TabIndex = 2;
            // 
            // CutGrouptextBox2
            // 
            CutGrouptextBox2.Location = new System.Drawing.Point(60, 114);
            CutGrouptextBox2.Name = "CutGrouptextBox2";
            CutGrouptextBox2.Size = new System.Drawing.Size(100, 21);
            CutGrouptextBox2.TabIndex = 3;
            // 
            // CutGroupbutton1
            // 
            CutGroupbutton1.Location = new System.Drawing.Point(151, 143);
            CutGroupbutton1.Name = "CutGroupbutton1";
            CutGroupbutton1.Size = new System.Drawing.Size(42, 37);
            CutGroupbutton1.TabIndex = 4;
            CutGroupbutton1.Text = "OK";
            CutGroupbutton1.UseVisualStyleBackColor = true;
            try
            {
                CutGroupbutton1.Click -= new EventHandler(CutImage_ByButton);
            }
            catch
            { }
            CutGroupbutton1.Click += new EventHandler(CutImage_ByButton);
        }

        private void InitializeMeasureLen(GroupBox baseGroup)
        {
            
        }

        private void CutImage_ByButton(object sender, EventArgs e)//修改数值改变截图所得的图片大小
        {
            try
            {
                BinaryTool.Enabled = true;
                PicFromCamera.Enabled = true;
                PicFromLocality.Enabled = true;
                AutoFeatherPoint.Enabled = true;
                DeleteFeather.Enabled = true;
                MainUndobutton.Enabled = true;
                CancleCut.Enabled = true;
                OpenFeather.Enabled = true;
                AddPoint.Enabled = true;

                leftX = (int)(int.Parse(CommonOperation.StringOperateHelp.LeftOf(CutGrouptextBox1.Text, ',')) * Math.Pow(2, -SizeChangecount));
                leftY = (int)(int.Parse(CommonOperation.StringOperateHelp.RightOf(CutGrouptextBox1.Text, ',')) * Math.Pow(2, -SizeChangecount));
                rightX = (int)(int.Parse(CommonOperation.StringOperateHelp.LeftOf(CutGrouptextBox2.Text, ',')) * Math.Pow(2, -SizeChangecount));
                rightY = (int)(int.Parse(CommonOperation.StringOperateHelp.RightOf(CutGrouptextBox2.Text, ',')) * Math.Pow(2, -SizeChangecount));
                
                DrawBorder = true;
                isSreenShot = false;
                picturebox.Invalidate();
                BasegroupBox.Controls.Clear();
                BasegroupBox.Text = null;
            }
            catch (Exception ex)
            { 
            }
        }

        private void InitializeBinary(GroupBox baseGroup)
        {
            baseGroup.Text = "Binary Image";



            baseGroup.Controls.Add(Binarybutton1);
            baseGroup.Controls.Add(Binarybutton2);
            baseGroup.Controls.Add(Binarylabel1);
            baseGroup.Controls.Add(BinarytextBox1);
                // 
            // Binarybutton1
            // 
            Binarybutton1.Location = new System.Drawing.Point(151, 143);
            Binarybutton1.Name = "Binarybutton1";
            Binarybutton1.Size = new System.Drawing.Size(42, 37);
            Binarybutton1.TabIndex = 4;
            Binarybutton1.Text = "OK";
            Binarybutton1.UseVisualStyleBackColor = true;
            
            try {
                Binarybutton1.Click -= new EventHandler(BinaryButton_Click);
            }
            catch
            { }
            Binarybutton1.Click += new EventHandler(BinaryButton_Click);

            // Binarybutton2
            Binarybutton2.Location = new System.Drawing.Point(6, 143);
            Binarybutton2.Name = "Autobutton2";
            Binarybutton2.Size = new System.Drawing.Size(64, 37);
            Binarybutton2.TabIndex = 5;
            Binarybutton2.Text = "Auto Threshod";
            Binarybutton2.UseVisualStyleBackColor = true;
            try {
                Binarybutton2.Click -= new EventHandler(AutoThroshod_Click);
            }
            catch
            { }
            Binarybutton2.Click += new EventHandler(AutoThroshod_Click);
            // 
            // Binarylabel1
            // 
            Binarylabel1.AutoSize = true;
            Binarylabel1.Location = new System.Drawing.Point(21, 54);
            Binarylabel1.Name = "Binarylabel1";
            Binarylabel1.Size = new System.Drawing.Size(89, 12);
            Binarylabel1.TabIndex = 0;
            Binarylabel1.Text = "Threshod Value";
            // 
            // BinarytextBox1
            // 
            BinarytextBox1.Location = new System.Drawing.Point(75, 88);
            BinarytextBox1.Text = "128";
            BinarytextBox1.Name = "BinarytextBox1";
            BinarytextBox1.Size = new System.Drawing.Size(100, 21);
            BinarytextBox1.TabIndex = 3;
        }

        private void BinaryButton_Click(object sender, EventArgs e)
        {
            try
            {
                Record record = new Record();
                record.recordName = "binaryBmp";
                record.bitmap = bmp;
                RecordList.Add(record);
                Threshod = int.Parse(BinarytextBox1.Text);
                if (picturebox.Image != null)
                {
                    BinaryImage(Threshod);
                }
                PicFromCamera.Enabled = true;
                PicFromLocality.Enabled = true;
                CutImage2.Enabled = true;
                AutoFeatherPoint.Enabled = true;
                DeleteFeather.Enabled = true;
                MainUndobutton.Enabled = true;
                CancleCut.Enabled = true;
                OpenFeather.Enabled = true;
                AddPoint.Enabled = true;
                BasegroupBox.Controls.Clear();
                BasegroupBox.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AutoThroshod_Click(object sender, EventArgs e)
        {
            int piexlsum, thresh_value, greyscale;
            int[] piexlcount = new int[256];               //每个像素点个数
            double[] piexlpercent = new double[256];       //每个像素点所占比重
            double w0, w1, u0tmp, u1tmp, u0, u1, u, deltaTmp, deltaMax;

            if (rightX == 0 && rightY == 0)//若图片没有进行截图操作
                piexlsum = bmp.Size.Height * bmp.Size.Width;//全图的像素总数
            else
            {
                piexlsum = (rightY - leftY) * (rightX - leftX);
            }
            thresh_value = 0;                              //最佳阈值
            greyscale = 255;                               //255个像素点
            deltaMax = 0;

            for (int i = 0; i < greyscale; i++)//置每个灰度的像素数为0,对应比率为0
            {
                piexlcount[i] = 0;
                piexlpercent[i] = 0;
            }
            if (rightX == 0 && rightY == 0)//若图片没有进行截图操作
            {
                for (int y = leftY; y < bmp.Size.Height; y++)
                {
                    for (int x = leftX; x < bmp.Size.Width; x++)
                    {
                        piexlcount[bmp.GetPixel(x, y).R]++;//每级灰度的像素个数
                    }
                }
            }
            else
            {
                for (int y = leftY; y < rightY; y++)
                {
                    for (int x = leftX; x < rightX; x++)
                    {
                        piexlcount[bmp.GetPixel(x, y).R]++;//每级灰度的像素个数
                    }
                }
            }
            for (int i = 0; i < greyscale; i++)
            {
                piexlpercent[i] = (double)piexlcount[i] / piexlsum;//每级灰度的像素占总像素数比率
            }

            for (int i = 0; i < greyscale; i++)//阈值扫描
            {
                w0 = w1 = u0tmp = u1tmp = u0 = u1 = u = deltaTmp = 0;
                for (int j = 0; j < greyscale; j++)
                {
                    if (j < i)
                    {
                        w0 += piexlpercent[j];//前景像素占总像素比率
                        u0tmp += j * piexlpercent[j];//前景像素灰度平均值
                    }
                    else
                    {
                        w1 += piexlpercent[j];//背景像素占总像素比率
                        u1tmp += j * piexlpercent[j];//背景像素灰度平均值
                    }
                }
                u0 = u0tmp / w0;//前景像素灰度平均值
                u1 = u1tmp / w1;//背景像素灰度平均值
                u = u0tmp + u1tmp;//所有像素灰度平均值
                deltaTmp = w0 * Math.Pow((u0 - u), 2) + w1 * Math.Pow((u1 - u), 2);//前景背景差距=w0(u0-u)^2+w1(u1-u)^2
                if (deltaTmp > deltaMax)//最大的deltaMax时，前景背景差异最大
                {
                    deltaMax = deltaTmp;
                    thresh_value = i;
                }
            }
            BinarytextBox1.Text = thresh_value.ToString();
        }

        void BinaryImage(int threshod)
        {
            try
            {
                int y = 0, x = 0;
                int[,] dealtData = new int[showBmp.Width, showBmp.Height];
                for (y = 0; y < showBmp.Height; y++)
                {
                    for (x = 0; x < showBmp.Width; x++)
                    {
                        if (showBmp.GetPixel(x, y).R < threshod)
                            dealtData[x, y] = 0;
                        else
                            dealtData[x, y] = 255;
                    }
                }
                /*图像边界全涂为0*/
                for (x = 0; x < showBmp.Width; x++)
                {
                    dealtData[x, 0] = 255;

                    dealtData[x, showBmp.Height - 1] = 255;
                }
                for (y = 0; y < showBmp.Height; y++)
                {
                    dealtData[0, y] = 255;
                    dealtData[showBmp.Width - 1, y] = 255;
                }
                Bitmap bmpOut = new Bitmap(showBmp.Width, showBmp.Height, PixelFormat.Format24bppRgb);
                for (y = 0; y < showBmp.Height; y++)
                {
                    for (x = 0; x < showBmp.Width; x++)
                    {
                        Color color = Color.FromArgb(dealtData[x, y], dealtData[x, y], dealtData[x, y]);
                        bmpOut.SetPixel(x, y, color);
                    }
                }
                picturebox.Image = bmpOut;
                showBmp = bmpOut;
                //Graphics g = Graphics.FromImage(bmpOut);
                //g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                //g.Dispose();

                //return bmpOut;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void InitializeOpenImg(GroupBox baseGroup)
        {
            baseGroup.Text = "Open Image";

            baseGroup.Controls.Add(OpenImgbutton1);
            baseGroup.Controls.Add(OpenImglabel1);
            baseGroup.Controls.Add(OpenImgtextBox1);
            // 
            // Binarybutton1
            // 
            OpenImgbutton1.Location = new System.Drawing.Point(151, 143);
            OpenImgbutton1.Name = "Binarybutton1";
            OpenImgbutton1.Size = new System.Drawing.Size(42, 37);
            OpenImgbutton1.TabIndex = 4;
            OpenImgbutton1.Text = "OK";
            OpenImgbutton1.UseVisualStyleBackColor = true;
            try
            {
                OpenImgbutton1.Click -= new EventHandler(OpenImgButton_Click);
            }
            catch
            { }
            OpenImgbutton1.Click += new EventHandler(OpenImgButton_Click);
            // 
            // Binarylabel1
            // 
            OpenImglabel1.AutoSize = true;
            OpenImglabel1.Location = new System.Drawing.Point(21, 54);
            OpenImglabel1.Name = "Binarylabel1";
            OpenImglabel1.Size = new System.Drawing.Size(89, 12);
            OpenImglabel1.TabIndex = 0;
            OpenImglabel1.Text = "Classify Number";
            // 
            // BinarytextBox1
            // 
            OpenImgtextBox1.Location = new System.Drawing.Point(75, 88);
            OpenImgtextBox1.Name = "BinarytextBox1";
            OpenImgtextBox1.Size = new System.Drawing.Size(100, 21);
            OpenImgtextBox1.TabIndex = 3;
        }

        private void OpenImgButton_Click(object sender, EventArgs e)
        {
          
        }

        int defaultRanNum = 10;
       
        private void InitializeAutoFeather(GroupBox baseGroup)
        {
            baseGroup.Text = "Get Auto Feather";

            baseGroup.Controls.Add(AutoFeatherlabel2);
            baseGroup.Controls.Add(AutoAddFeathercomboBox);
            baseGroup.Controls.Add(AutoFeatherbutton1);
            baseGroup.Controls.Add(AutoFeatherlabel1);
            baseGroup.Controls.Add(AutoFeathertextBox1);

            // 
            // Binarylabel2
            // 
            AutoFeatherlabel2.AutoSize = true;
            AutoFeatherlabel2.Location = new System.Drawing.Point(16, 29);
            AutoFeatherlabel2.Name = "AutoFeatherlabel2";
            AutoFeatherlabel2.Size = new System.Drawing.Size(89, 12);
            AutoFeatherlabel2.TabIndex = 0;
            AutoFeatherlabel2.Text = "Feather Algorithm";

            // 
            // AutoAddFeathercomboBox
            // 
            AutoAddFeathercomboBox.FormattingEnabled = true;
            if (AutoAddFeathercomboBox.Items.Count == 0)
            {
                AutoAddFeathercomboBox.Items.AddRange(new object[] {
            "genetic algorithm",
            "random"});
            }
            AutoAddFeathercomboBox.Location = new System.Drawing.Point(60, 54);
            AutoAddFeathercomboBox.Name = "AutoAddFeathercomboBox";
            AutoAddFeathercomboBox.Size = new System.Drawing.Size(100, 20);
            AutoAddFeathercomboBox.TabIndex = 5;
            AutoAddFeathercomboBox.Text = "genetic algorithm";
            // 
            // Binarybutton1
            // 
            AutoFeatherbutton1.Location = new System.Drawing.Point(151, 143);
            AutoFeatherbutton1.Name = "AutoFeatherbutton1";
            AutoFeatherbutton1.Size = new System.Drawing.Size(42, 37);
            AutoFeatherbutton1.TabIndex = 4;
            AutoFeatherbutton1.Text = "OK";
            AutoFeatherbutton1.UseVisualStyleBackColor = true;
            try {
                AutoFeatherbutton1.Click -= new EventHandler(AutoFeatherButton_Click);
            }
            catch
            { }
            AutoFeatherbutton1.Click += new EventHandler(AutoFeatherButton_Click);
            // 
            // Binarylabel1
            // 
            AutoFeatherlabel1.AutoSize = true;
            AutoFeatherlabel1.Location = new System.Drawing.Point(16, 86);
            AutoFeatherlabel1.Name = "AutoFeatherlabel1";
            AutoFeatherlabel1.Size = new System.Drawing.Size(89, 12);
            AutoFeatherlabel1.TabIndex = 0;
            AutoFeatherlabel1.Text = "Feather Number";
            // 
            // BinarytextBox1
            // 
            AutoFeathertextBox1.Location = new System.Drawing.Point(60, 114);
            AutoFeathertextBox1.Text = "10";
            AutoFeathertextBox1.Name = "AutoFeathertextBox1";
            AutoFeathertextBox1.Size = new System.Drawing.Size(100, 21);
            AutoFeathertextBox1.TabIndex = 3;
        }

        int pointNum = 60;
        private void AutoFeatherButton_Click(object sender, EventArgs e)
        {
            BinaryTool.Enabled = true;
            PicFromCamera.Enabled = true;
            PicFromLocality.Enabled = true;
            CutImage2.Enabled = true;
            DeleteFeather.Enabled = true;
            MainUndobutton.Enabled = true;
            CancleCut.Enabled = true;
            OpenFeather.Enabled = true;
            AddPoint.Enabled = true;

            if (int.Parse(AutoFeathertextBox1.Text) > 20)
            {
                AutoFeathertextBox1.Text = "20";
            }
            else if (int.Parse(AutoFeathertextBox1.Text) <= 0)
            {
                AutoFeathertextBox1.Text = "0";
            }
            defaultRanNum = int.Parse(AutoFeathertextBox1.Text);
            featherRegion.Clear();
            FeatherdataGrid.Rows.Clear();
            showfeature.Clear();  //清空数据

            SizeChangecount = 0;   //将图片变为原始大小
            if (bmp != null)
            {
                picturebox.Size = bmp.Size;

                if (Mainpanel.Width > bmp.Width && Mainpanel.Height > bmp.Height)
                {
                    picturebox.Location = new Point((Mainpanel.Width - bmp.Size.Width) / 2, (Mainpanel.Height - bmp.Size.Height) / 2);

                }
                else if (Mainpanel.Width > bmp.Width)
                {
                    picturebox.Location = new Point((Mainpanel.Width - bmp.Size.Width) / 2, 0);

                }
                else if (Mainpanel.Height > bmp.Height)
                {
                    picturebox.Location = new Point(0, (Mainpanel.Height - bmp.Size.Height) / 2);

                }
                else
                {
                    picturebox.Location = new Point(0, 0);
                }
            }
            switch (AutoAddFeathercomboBox.Text)
            {
                case "genetic algorithm":
                    {
                        ConfigGen configgen = new ConfigGen(savePath,programPath);
                      
                        //configgen.Show();
                        if (configgen.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                string[] data = configgen.data;
                           //     string DebugPath = StringOperateHelp.LeftOfRightmostOf(savePath, '\\');
                                if (File.Exists(programPath +PublicVariable.NegativePointsFileName))
                                {
                                    FileStream fs = new FileStream(programPath + PublicVariable.NegativePointsFileName, FileMode.Open);
                                    StreamReader streamreader = new StreamReader(fs);
                                    data = streamreader.ReadLine().Split(',');
                                    for (int i = 0; i < pointNum; i++)
                                    {
                                        featherRgnstruct tmp = new featherRgnstruct();
                                        tmp.rec = new Rectangle(int.Parse(data[i * 3+1])+leftX, int.Parse(data[i * 3])+leftY, 3, 3);
                                        tmp.featherNum = int.Parse(data[i * 3+2]);
                                        featherRegion.Add(tmp);
                                        showfeature.Add(tmp);
                                        FeatherdataGrid.Rows.Add(tmp.rec.X,tmp.rec.Y,tmp.featherNum);
                                    }
                                    picturebox.Invalidate();
                                }
                            }
                            catch
                            {
 
                            }
 
                        }
                        picturebox.Invalidate();
                    }
                    break;
                case "random":
                    {
                        Random random = new Random();
                      
                        for (int i = 0; i < defaultRanNum; i++)
                        {
                            featherRgnstruct tmp = new featherRgnstruct();
                            tmp.rec = new Rectangle(random.Next(rightX-leftX)+leftX, random.Next(rightY-leftY)+leftY, 3, 3);
                            tmp.featherNum = 1;
                            featherRegion.Add(tmp);
                            showfeature.Add(tmp);
                            FeatherdataGrid.Rows.Add(tmp.rec.X, tmp.rec.Y, 1);

  
                            
                        }
                        picturebox.Invalidate();
                    }
                    break;
            }
            BasegroupBox.Controls.Clear();
            BasegroupBox.Text = null;
        }

        private void InitializeMatch(GroupBox baseGroup)
        {
            baseGroup.Text = "Match pictures";

            baseGroup.Controls.Add(Matchbutton1);
            baseGroup.Controls.Add(Matchlabel1);
            baseGroup.Controls.Add(Matchtextbox1);

            Matchbutton1.Location = new System.Drawing.Point(151, 143);
            Matchbutton1.Name = "Matchbutton1";
            Matchbutton1.Size = new System.Drawing.Size(42, 37);
            Matchbutton1.TabIndex = 4;
            Matchbutton1.Text = "OK";
            Matchbutton1.UseVisualStyleBackColor = true;
            try
            {
                Matchbutton1.Click -= new EventHandler(Matchbutton_Click);
            }
            catch
            { }
            Matchbutton1.Click += new EventHandler(Matchbutton_Click);

            Matchlabel1.AutoSize = true;
            Matchlabel1.Location = new System.Drawing.Point(21, 54);
            Matchlabel1.Name = "Matchlabel1";
            Matchlabel1.Size = new System.Drawing.Size(89, 12);
            Matchlabel1.TabIndex = 0;
            Matchlabel1.Text = "time interval";

            Matchtextbox1.Location = new System.Drawing.Point(75, 88);
            Matchtextbox1.Text = "5";
            Matchtextbox1.Name = "Matchtextbox1";
            Matchtextbox1.Size = new System.Drawing.Size(100, 21);
            Matchtextbox1.TabIndex = 3;
        }

        private void Matchbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(Matchtextbox1.Text) < 5)
                {
                    MessageBox.Show("输入时间不得小于五秒");
                    return;
                }
                Formtimer.Interval = int.Parse(Matchtextbox1.Text);
                //timer = new System.Timers.Timer();
                //timer.Interval = int.Parse(Matchtextbox1.Text);
                //timer.Elapsed+=new System.Timers.ElapsedEventHandler(MatchPicture);
                //timer.Start();
                Formtimer.Start();

                //Notice.Text = "matching";
            }
            catch
            {
 
            }
 
        }

        private void MatchPicture(object sender,EventArgs e)
        {
            Formtimer.Stop();
            ReadImageFromCamera();
           // ReadImageFromCamera();// 读出图片
            //Notice.Text = "matching: match the feather point with the image; ";
            MatchtheImage();
            Formtimer.Start();
        }

        private void MatchtheImage()
        {
            int[,] bmpData = new int[bmp.Height, bmp.Width];
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (bmp.GetPixel(x, y).R > Threshod)
                    {
                        bmpData[y, x] = 255;
                    }
                    else
                    {
                        bmpData[y, x] = 0;
                    }
                }
            int sum=0,count=0;
            foreach (featherRgnstruct tmp in featherRegion)
            {
                sum += tmp.featherNum;

                if (bmpData[tmp.rec.Height, tmp.rec.Width] > 0)
                {
                    count += tmp.featherNum;

                }
                else
                {
                    count -= tmp.featherNum;
                }
            }
            Notice.Text = ((float)(count * 100 / sum)).ToString() + "feathers matchs ";
 
        }
        #endregion

        #region   按钮作用的提示信息
        private void savefeatherbt_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to save\n the feather points to the file ");
           // Notice.Text = "click this button to save the feather points to the file";
        }

        private void savefeatherbt_MouseLeave(object sender, EventArgs e)
        {
            //Notice.Text = null;
        }

        private void MainUndobutton_MouseEnter(object sender, EventArgs e)
        {
            //Notice.Text = "click this button to Undo the last operation";
            tooltip.SetToolTip((sender as Button), "click this button to Undo\n the binary operation ");

        }
        private void OpenFeather_MouseLeave(object sender, EventArgs e)
        {
            
        }

        private void OpenFeather_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to read \n the feathers saved in the file");
        }
        private void MainUndobutton_MouseLeave(object sender, EventArgs e)
        {
           // Notice.Text = null;
        }
        private void AddPoint_MouseEnter(object sender, EventArgs e)
        {
            Notice.Text = "click picture to add feather point";
        }
        private void PicFromCamera_MouseLeave(object sender, EventArgs e)
        {
            //Notice.Text = null;
        }
        private void PicFromCamera_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip(sender as Button, "click this button to get an image from the camera");
        }


        private void CutImage2_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip(sender as Button, "drag on the picture to\n cut the need location");
           // Notice.Text = "drag on the picture to cut the need location";
        }

        private void CancleCut_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip(sender as Button, "click this button to get\n the original bmp picture");
            //Notice.Text = "click this button to get the original bmp picture";
        }
        private void BinaryTool_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip(sender as Button, "click to binary\n the picture");
        }
        private void lessen_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip(sender as Button, "click to shrink\n the picture");
        }

        private void amplification_Leave(object sender, EventArgs e)
        {
            
        }

        private void AutoFeatherPoint_Leave(object sender, EventArgs e)
        {
           // Notice.Text = null;
        }

        private void AutoFeatherPoint_Enter(object sender, EventArgs e)
        {
            tooltip.SetToolTip(sender as Button, "click this button to get the\n feather points by the computer");
            //Notice.Text = "click this button to get the feather point by the computer";
        }

        private void PicFormLocality_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to get\n an image from the computer");
        }

        private void SavePic_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to save\n an image to the computer");
        }

        private void DefaultSize_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to get\n the default size image");
        }

        private void BinaryTool_MouseEnter_1(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button\n to binary the image");
        }

        private void AddPoint_MouseEnter_1(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to set the\n feather point on the image");
        }

        private void AutoFeatherPoint_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to set the\n feather points automaticly ");
        }

        private void DeleteFeather_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to delete\n all the feather points");
        }

        private void MatchFeather_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to match the feather\n points with the picture from the camera");
        }

        private void MeasureLen_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip((sender as Button), "click this button to measure\n the points in the image");
        }
        private void amplification_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip(sender as Button, "click to magnify\n the picture");
        }
        private void Close_MouseEnter(object sender, EventArgs e)
        {
            tooltip.SetToolTip(sender as Button, "click to exit");
        }
        #endregion

        #region  通信设置

        ushort VDOffset = 0;   //VD区偏移坐标
 
        public struct VDaddress
        {
            public  int Chromaadd;
            public int Brightnessadd;
            public int Contrastadd;
            public int Saturationadd;
            public int Exposureadd;
 
        };

        VDaddress vdaddress = new VDaddress();

        private void CommunicateBt_Click(object sender, EventArgs e) //当鼠标点击切入到端口设置面板时，更新下当前数据
        {
            if (ConncetioncomboBox.Text == "Serial port")
            {
                SetSeriport();
                
                string[] addString=new string[5];
              //  string path=StringOperateHelp.LeftOfRightmostOf(savePath,'\\');
                FileStream fs = new FileStream(programPath + PublicVariable.HardwareConfigFileName, FileMode.Open, FileAccess.Read);//读取文件设定
                StreamReader m_streamReader = new StreamReader(fs);
                string strLine;
                int flag=0;
                while ((strLine=m_streamReader.ReadLine()) != null)
                {
                    if (CommonOperation.StringOperateHelp.LeftOf(strLine, ' ') == "hardware")
                    {
                        VDOffset = ushort.Parse(CommonOperation.StringOperateHelp.RightOf(strLine,':'));
                    }
                    else
                    {
                        addString[flag] = strLine;
                        flag++;
                    }
                   
                    
                }
                vdaddress.Chromaadd = int.Parse(addString[0]);
                vdaddress.Brightnessadd = int.Parse(addString[1]);
                vdaddress.Contrastadd = int.Parse(addString[2]);
                vdaddress.Saturationadd = int.Parse(addString[3]);
                vdaddress.Exposureadd = int.Parse(addString[4]);

                //将串口信息写入文件
                FileStream comfs = new FileStream(savePath +"\\"+ PublicVariable.CompileFileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(comfs, Encoding.Default);
                string text = Com.Text + "\r" + BaudRate.Text + "\r" + Databits.Text + "\r" + Stopbits.Text + "\r" + ParityCheck.Text + "\r" + StreamControl.Text;
                sw.Write(text);
                sw.Close();
                fs.Close();
                UpdataCameraData();
            }
        }

        private void UpdataCameraData()          //将摄像头内亮度，对比度等数据更新到平台上
        {
                byte[] chromaValue = new byte[2];
                byte[] contrastValue = new byte[2];
                byte[] exposureValue = new byte[2];
                modbus.ReadOneRegister((ushort)(vdaddress.Chromaadd + VDOffset), chromaValue);
                modbus.ReadOneRegister((ushort)(vdaddress.Contrastadd + VDOffset), contrastValue);
                modbus.ReadOneRegister((ushort)(vdaddress.Exposureadd + VDOffset), exposureValue);

                //ChromatrackBar.Value = (int)chromaValue[0];
                //BrightnesstrackBar.Value = (int)chromaValue[1];
                //ContrasttrackBar.Value = (int)contrastValue[0];
                //SatureationtrackBar.Value = (int)contrastValue[1];
                //ExposuretrackBar.Value = (int)exposureValue[0];
        }

        void SetSeriport()      //设置端口信息
        {
            try
            {

                serialport.PortName = Com.Text;
                serialport.BaudRate = int.Parse(BaudRate.Text);
                serialport.DataBits = int.Parse(Databits.Text);
                if (ParityCheck.Text.ToUpper() == "NONE")
                    serialport.Parity = Parity.None;
                else if (ParityCheck.Text.ToUpper() == "ODD")
                    serialport.Parity = Parity.Odd;
                else
                    serialport.Parity = Parity.Even;
                if (Stopbits.Text == "1")
                    serialport.StopBits = StopBits.One;
                else if (Stopbits.Text == "1.5")
                    serialport.StopBits = StopBits.OnePointFive;

                serialport.ReadTimeout = 100;
                serialport.WriteTimeout = 100;

                modbus = new Modbus(serialport);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        #endregion


        private void CameraBt_Click(object sender, EventArgs e)  //将设置完的参数通过modbus写到VD区
        {
            try
            {
                byte[] chromaAndBrightData = new byte[2];

                byte[] contrastAndSatureData = new byte[2];

                byte[] ExposureData = new byte[2];

                ushort ExposureAddress = (ushort)(vdaddress.Exposureadd + VDOffset);

                ushort ChromatAddress = (ushort)(vdaddress.Chromaadd + VDOffset);
                ushort ContrastAddress = (ushort)(vdaddress.Contrastadd + VDOffset);

                chromaAndBrightData[0] = (byte)ChromatrackBar.Value;
                chromaAndBrightData[1] = (byte)BrightnesstrackBar.Value;

                contrastAndSatureData[0] = (byte)ContrasttrackBar.Value;
                contrastAndSatureData[1] = (byte)SatureationtrackBar.Value;
                modbus.WriteSingleRegister(ChromatAddress, ref chromaAndBrightData);
                modbus.WriteSingleRegister(ContrastAddress, ref contrastAndSatureData);

                // modbus.ReadOneRegister(ExposureAddress, ExposureData);
                ExposureData[0] = (byte)ExposuretrackBar.Value;
                ExposureData[1] = 0;
                modbus.WriteSingleRegister(ExposureAddress, ref ExposureData);
                CameraData += "0";//0表示灰度图，1表示彩色图
                CameraData = ChromatrackBar.Value.ToString() + "," + BrightnesstrackBar.Value.ToString() +
                    "," + ContrasttrackBar.Value.ToString() + "," + SatureationtrackBar.Value.ToString() +
                    "," + "0,0,0,0,";
            }
            catch
            {
 
            }


        }

        private void ZOOMIN_Click(object sender, EventArgs e)
        {
            byte[] ZOOMData = new byte[2];
            //ushort ZOOMAddress = (ushort)(70 + VDOffset);
            //modbus.ReadOneRegister(ZOOMAddress, ZOOMData);
            //modbus.WriteSingleRegister
        }

        private void FeatherdataGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

         
        }

        private void FeatherdataGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                

            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)//右键表格删除当前行
        {

            try
            {
                featherRgnstruct flagfeather = new featherRgnstruct();


                flagfeather.rec = new Rectangle((int)(int.Parse(FeatherdataGrid.CurrentRow.Cells[0].Value.ToString())), int.Parse(FeatherdataGrid.CurrentRow.Cells[1].Value.ToString()), 3, 3);
                flagfeather.featherNum = int.Parse(FeatherdataGrid.CurrentRow.Cells[2].Value.ToString());

                List<featherRgnstruct> tmpfeatherList = new List<featherRgnstruct>();
                foreach (featherRgnstruct tmp in showfeature)
                {
                    if (flagfeather.Equals(tmp))
                    {

                    }
                    else
                    {
                        tmpfeatherList.Add(tmp);
                    }
                }
                showfeature = tmpfeatherList;


                List<featherRgnstruct> tmpfeatherList2 = new List<featherRgnstruct>();
                foreach (featherRgnstruct tmp in featherRegion)
                {
                    featherRgnstruct numberborder = new featherRgnstruct();
                    numberborder.rec = new Rectangle((int)(tmp.rec.X * Math.Pow(2, SizeChangecount)), (int)(tmp.rec.Y * Math.Pow(2, SizeChangecount)),3,3);
                    numberborder.featherNum = tmp.featherNum;
                    if (flagfeather.Equals(numberborder))
                    {

                    }
                    else
                    {
                        tmpfeatherList2.Add(tmp);
                    }
                }
                featherRegion = tmpfeatherList2;
                showfeature = tmpfeatherList;
                FeatherdataGrid.Rows.Remove(FeatherdataGrid.CurrentRow);



                picturebox.Invalidate();
            }
            catch (Exception ex)
            {

            }
        }

        private void FeatherdataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) //当改变表格内数据时
        {
            int flag = 0;
            int feathernum = featherRegion.Count;
            featherRegion.Clear();
            showfeature.Clear();
            while (flag < feathernum)
            {
                featherRgnstruct tmp = new featherRgnstruct();
                int pointX = int.Parse(FeatherdataGrid.Rows[flag].Cells[0].Value.ToString());
                int pointY = int.Parse(FeatherdataGrid.Rows[flag].Cells[1].Value.ToString());
                tmp.rec = new Rectangle(pointX, pointY, 3, 3);
                tmp.featherNum = int.Parse(FeatherdataGrid.Rows[flag].Cells[2].Value.ToString());
                featherRegion.Add(tmp);
                tmp.rec=new Rectangle((int)(pointX*Math.Pow(2,SizeChangecount)),(int)(pointY*Math.Pow(2,SizeChangecount)),3,3);
                showfeature.Add(tmp);
                flag++;
            }
            picturebox.Invalidate();
        }

        private void valueTextbox_KeyDown(object sender, KeyEventArgs e)  //当特征点默认值改变时，点击enter按钮，切换焦点
        {
            if (Keys.Enter == e.KeyCode)
            {
                FeatherdataGrid.Focus();
            }
        }

        private void Undobutton_Click(object sender, EventArgs e)
        {
          
        }
       
        public void ReadImageFromCamera()   //读上来一张图片
        {
            try
            {
                //ushort reSetAddress = 164;
                byte[] WriteData = new byte[2] { 1, 1 };
                //modbus.WriteSingleRegister(reSetAddress, ref WriteData);

                ushort Caddress = 1;
                byte[] writedata = new byte[2] { 1, 1 };

                bmp = modbus.ReadBmpFromCamera(Caddress, writedata);
                bmp.Save(savePath + "\\camerabmp" + (cameraImagecount++).ToString() + ".bmp");

                picturebox.Parent = Mainpanel;

                lefttop = new Point(0, 0);
                rightbottom = new Point(bmp.Size.Width, bmp.Size.Height);

                picturebox.Image = bmp;
                showBmp = bmp;

                if (rightX == 0 && rightY == 0)
                {
                    rightX = bmp.Width;
                    rightY = bmp.Height;
                }

                picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
                picturebox.Size = showBmp.Size;
                normalSize = showBmp.Size;
                if (Mainpanel.Width > bmp.Width && Mainpanel.Height > bmp.Height)
                {
                    picturebox.Location = new Point((Mainpanel.Width - bmp.Size.Width) / 2, (Mainpanel.Height - bmp.Size.Height) / 2);

                }
                else if (Mainpanel.Width > bmp.Width)
                {
                    picturebox.Location = new Point((Mainpanel.Width - bmp.Size.Width) / 2, 0);

                }
                else if (Mainpanel.Height > bmp.Height)
                {
                    picturebox.Location = new Point(0, (Mainpanel.Height - bmp.Size.Height) / 2);

                }
                else
                {
                    picturebox.Location = new Point(0, 0);
                }
                SizeChangecount = 0;
                picturebox.Show();
                Notice.Text = null;
            }
            catch (Exception ex)
            {
                Notice.Text = null;
                MessageBox.Show("请检查下串口，并重新连接");
            }
            finally
            {
                Mainpanel.AutoScroll = true;
               // PicFromCamera.Enabled = true;
                timer1.Stop();
            }
        }

        private void UpdataCamera_Click(object sender, EventArgs e)
        {
            UpdataCameraData();
        }

        private void TabControl_Selected(object sender, TabControlEventArgs e)   //切换工具框
        {
            if((sender as TabControl).SelectedTab.Text.Equals("Communications"))
            {
                Com.Items.Clear();
                string[] spname = System.IO.Ports.SerialPort.GetPortNames();
                for (int i = 0; i < spname.Length; i++)
                {
                    Com.Items.Add(spname[i]);
                }
                if (File.Exists(savePath +"\\"+PublicVariable.CompileFileName))
                {
                    FileStream fs = new FileStream(savePath + "\\" + PublicVariable.CompileFileName, FileMode.Open, FileAccess.Read);//读取文件设定
                     StreamReader m_streamReader = new StreamReader(fs);
                     Com.Text = m_streamReader.ReadLine();
                     BaudRate.Text = m_streamReader.ReadLine();
                     Databits.Text = m_streamReader.ReadLine();
                     Stopbits.Text = m_streamReader.ReadLine();
                     ParityCheck.Text = m_streamReader.ReadLine();
                     StreamControl.Text = m_streamReader.ReadLine();
                     fs.Close();
                     m_streamReader.Close();
                }
            }
        }

        private void OpenFeather_Click(object sender, EventArgs e)   //打开并读入工程内的feather.h文档
        {
            featherRegion.Clear();
            showfeature.Clear();
            FeatherdataGrid.Rows.Clear();
            Openfeather();

        }
       
        private void Openfeather()   //打开feather.h文档
        {
            try
            {
                string streamreader = File.ReadAllText(savePath + "\\" + "feather.h");
                string dealtdata = CommonOperation.StringOperateHelp.RightOf(streamreader, '{');
                dealtdata = CommonOperation.StringOperateHelp.LeftOf(dealtdata, '}');
                string[] featherdata = dealtdata.Split(',');
                for (int i = 0; i < 1038; i++)
                {
                    OriginalData[i] = int.Parse(featherdata[i]);
                }
            }
            catch
            {
            }
            leftX = OriginalData[11];
            leftY = OriginalData[10];
            rightX = OriginalData[13];
            rightY = OriginalData[12];
            DrawBorder = true;    //显示边框
            for (int i = 0; i < 255; i += 4)
            {

                if (OriginalData[i + 14] != 0 || OriginalData[i + 15] != 0 ||
                    OriginalData[i + 16] != 0 || OriginalData[i + 17] != 0)
                {
                    featherRgnstruct featherstruct = new featherRgnstruct();
                    featherstruct.rec = new Rectangle(OriginalData[i + 15], OriginalData[i + 14], 3, 3);
                    featherstruct.featherNum = OriginalData[i + 17];
                    featherRegion.Add(featherstruct);
                    FeatherdataGrid.Rows.Add(featherstruct.rec.X, featherstruct.rec.Y, featherstruct.featherNum);
                    showfeature.Add(featherstruct);
                }
                else
                {
                    break;
                }

            }
            picturebox.Invalidate();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void ReSet_Click(object sender, EventArgs e)
        {
            try
            {
                ushort reSetAddress = 164;
                byte[] WriteData = new byte[2] { 1, 1 };
                modbus.WriteSingleRegister(reSetAddress, ref WriteData);
            }
            catch
            {
                MessageBox.Show("复位失败");
            }
        }

    }
}
