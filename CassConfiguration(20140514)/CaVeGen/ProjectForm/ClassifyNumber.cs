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

namespace CaVeGen.ProjectControl
{
    public partial class ClassifyNumber : Form
    {
        public ClassifyNumber(string _savePath, string _programPath)
        {
            InitializeComponent();
            this.savePath = _savePath;
            this.programPath = _programPath;
            //初始化变量
            InitializeVariable();

        }
        public struct Numberborder      //识别区域的结构体
        {
            public Point start;
            public Size size;
        }
        public struct featherRgnstruct  //特征点的结构体
        {
            public int classifyNUM;   //当前点属于哪个数字
            public Rectangle rec;     //特征点矩形
            public int Pixel;         //图像像素点
            public int featherNum;
        }
        public struct Templet           //模板的结构体
        {
            public string templetName;
            public Point templetPoint;
            public Size templetSize;
            public Feather feathers;
        }
        public struct Feather           //模板内特征点存储结构
        {
            public featherRgnstruct[] featherPoints;
        }

        public struct ExitNumber
        {
            public int Number;
            public int count;
        }

        static int currentimageCount = 0;
        const int NumberPoints = 30;

        int CurrentStep = 0;
       // List<Record> RecordList;      //用于返回二值化前的图片

        List<int> ExistNumber;

        bool isMeasureLen = false;      //为true时进入测量状态

        private string savePath = "D:\\Patch";   //默认工程路径
        private string programPath = null;   //编程路径 ...\debug   20140218
        Bitmap bmp;                         //读入的图像变量
        bool isaddNumborder = false;        //为true时进入增加识别区域
        bool isSreenShot = false;           //为true进行截图状态
        bool isaddFeather = false;          //为true时进行增加特征点

        bool startdraw = false;              //为真时在拖拉截图时绘制矩形
        bool startMeasure = false;           //为true表示已经在测量过程中

        static int cameraImagecount = 0;     //用于保存图像

        public PictureBox picturebox = new PictureBox();  //主界面内用于显示图片的容器

        System.Diagnostics.Process p; //调用自动特征点exe进程

        List<Bitmap> btList;       //存在的数字图片

        int defaultValue = 1;               //特征点权重默认值

        int defaultRanNum = 10;            //随机产生特征点的个数

        string CurrentTempletName = null;  //当前设计模板的名字

        SerialPort serialport = new SerialPort();           
        string CameraData = "0,0,0,0,0,0,0,0,";         //对应Camera Set界面的参数
        Modbus modbus;

        bool DrawMeasure = false;     //为true时指示绘制测量的线条
        bool DrawBorder = false;      //为true时指示绘制边框的矩形

        bool DrawCurrentBorder = false;             //为true时绘制表格内选中的边框
        bool DrawCurrentFeather = false;           //为true时绘制表格内选中的特征点

        bool isDesignFeather = false;      //为true时进入特征点绘制状态
        bool isDesignBorder = false;       // 为true时进入边框绘制状态

        Templet templetFlag;
        bool isShowTemplateBorder = false;
        bool isDrawTemplate = false;     //是否处于绘制模板边框状态
        bool startDrawTemplet = false;   //开始绘制模板边框
        Point templetLoc;                //模板边框的左上角位置
        Point CurrentTemplet;            //当前模板的左上角位置

        Rectangle CurrentCellRec;       //鼠标点击表格需在picturebox上绘制的矩形

        Point startpoint;             //鼠标点击的开始位置
        Point endpoint;              //鼠标点击的结束位置
        Size featherSize = new Size(3, 3); //特征点的默认矩形大小 

        Point lefttop;                    //截图图片相对原始图片的左上角坐标
        Point rightbottom;                //截图图片相对原始图片的右下角坐标
        Point currentMouse;                //当前鼠标的位置
         
        Point measureStartPoint;          //测量时鼠标点击的起始坐标
        Point measureEndPoint;            //测量时鼠标点击的结束坐标

        Size normalSize;   //图片的原始大小

        int leftX = 0, leftY = 0, rightX = 0, rightY = 0;//当前图片相对于原始图像的坐标
        int SizeChangecount = 0; //图片放大缩小比

        List<featherRgnstruct> featherRegion = new List<featherRgnstruct>(); //特征点的后台数据
        List<featherRgnstruct> showfeature = new List<featherRgnstruct>();   //显示在图像上的特征点数据
        Bitmap showBmp; //当前在用图像
        List<Numberborder> borderRegion = new List<Numberborder>();      //识别区域的后台数据
        List<Numberborder> showborder = new List<Numberborder>();        //显示图像上的识别区域
        // int[] OriginalData = new int[1037]; 
        ToolTip tooltip;                                                //指示按钮的提示信息
        void InitializeVariable( )   //初始化变量
        {
            picturebox.MouseDown += new MouseEventHandler(pictureBox_MouseDown);
            picturebox.MouseMove += new MouseEventHandler(pictureBox_MouseMove);
            picturebox.MouseUp += new MouseEventHandler(pictureBox_MouseUp);
            picturebox.MouseLeave += new EventHandler(picturebox_MouseLeave);
            picturebox.Paint += new PaintEventHandler(AutoPaintRectangle);

            foreach (Button tmp in Designtools.Controls) //设计框内的所有按钮初始化为disable状态
            {
                tmp.Enabled = false;
            }

            ExistNumber = new List<int>();//初始化一个队列，记录哪些数字在模板识别中存在


            AddBorder.Enabled = false;
            CutNumberBmp.Enabled = false;
            
            MakeMod .Enabled = false;
            SetMod.Enabled = false; //初始化步骤按钮的使能为false

            tooltip = new ToolTip(); //初始化提示控件

            DatagridviewgroupBox.Text = null;
            //RecordList = new List<Record>();
            if (!File.Exists(savePath + "\\feather.h"))//若feather.h文件不存在，则新建一个空的文件
            {
                string InitFeather = null;
                for (int i = 0; i < 12053; i++) 
                {
                    InitFeather += "0,";
                }
                InitFeather += "0";
                string data = " image_data_packet_desc_type  image_data_packet_desc={" + InitFeather + "};";
                StreamWriter streamwriter = File.CreateText(savePath + "\\" + "feather.h");
                streamwriter.WriteLine(data);
                streamwriter.Close();
            }

            BorderdataGrid.ColumnHeadersVisible = false;
            BorderdataGrid.RowHeadersVisible = false;
            FeatherdataGrid.ColumnHeadersVisible = false;
            FeatherdataGrid.RowHeadersVisible = false;
          //  savePath = currentPath;

            templetFlag = new Templet();
            templetFlag.templetName = null;
            templetFlag.templetSize = new System.Drawing.Size(0, 0);
            templetFlag.templetPoint = new Point(-1, -1);
            templetFlag.feathers.featherPoints = new featherRgnstruct[NumberPoints*10];
            for (int i = 0; i < NumberPoints*10; i++)
            {
                templetFlag.feathers.featherPoints[i].featherNum = 0;
                templetFlag.feathers.featherPoints[i].rec = new Rectangle(0, 0, 0, 0);
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Point pt = new Point(e.X, e.Y);
            if (e.Button == MouseButtons.Left && isSreenShot)
            {
                startpoint = pt;
                startdraw = true;
            }
            else if (e.Button == MouseButtons.Left && isaddFeather)   //点击添加特征点状态
            {
                Rectangle rec = new Rectangle(pt.X, pt.Y, featherSize.Width, featherSize.Height);
                featherRgnstruct tmpfeather = new featherRgnstruct();
                tmpfeather.featherNum = defaultValue;
                tmpfeather.rec = rec;

                Graphics g = (sender as PictureBox).CreateGraphics();

                if (FeatherdataGrid.Rows.Count < NumberPoints*10)//特征点上限不超过300个
                {
                    g.DrawRectangle(Pens.Blue, new Rectangle(rec.X - (featherSize.Width - 1) / 2, rec.Y - (featherSize.Height - 1) / 2, featherSize.Width, featherSize.Height));
                    FeatherdataGrid.Rows.Add(tmpfeather.rec.X, tmpfeather.rec.Y, tmpfeather.featherNum);
                    showfeature.Add(tmpfeather);
                    tmpfeather.rec.X = (int)(tmpfeather.rec.X * Math.Pow(2, -SizeChangecount));
                    tmpfeather.rec.Y = (int)(tmpfeather.rec.Y * Math.Pow(2, -SizeChangecount));
                    featherRegion.Add(tmpfeather);
                    //UpdataTemplet();
                }
                g.Dispose();
            }
            else if (e.Button == MouseButtons.Left && isaddNumborder)  //点击添加识别区域
            {
                if (BorderdataGrid.Rows.Count < 10)
                {
                    Rectangle rec = new Rectangle(pt, new Size((int)(templetFlag.templetSize.Width * Math.Pow(2, SizeChangecount)), (int)(templetFlag.templetSize.Height * Math.Pow(2, SizeChangecount))));
                    Graphics g = (sender as PictureBox).CreateGraphics();
                    g.DrawRectangle(Pens.Blue, rec);

                    Numberborder tmpnumberborder = new Numberborder();
                    tmpnumberborder.start = new Point((int)(pt.X * Math.Pow(2, -SizeChangecount)), (int)(pt.Y * Math.Pow(2, -SizeChangecount)));
                    tmpnumberborder.size = templetFlag.templetSize;
                    //tmpnumberborder.FeatherName = CurrentTempletName;
                    borderRegion.Add(tmpnumberborder);
                    BorderdataGrid.Rows.Add(tmpnumberborder.start.X, tmpnumberborder.start.Y, CurrentTempletName);

                    tmpnumberborder.start = pt;
                    tmpnumberborder.size = rec.Size;  

                    showborder.Add(tmpnumberborder);   //识别区域模板
                    g.Dispose();
                }
            }
            else if (e.Button == MouseButtons.Left && isMeasureLen)
            {
                startMeasure = true;
                measureStartPoint = new Point((int)(e.X * Math.Pow(2, -SizeChangecount)), (int)(e.Y * Math.Pow(2, -SizeChangecount)));

            }
            else if (e.Button == MouseButtons.Right)
            {
                #region 右击改变特征点的值
                #endregion
            }
            if (isDrawTemplate)   //进入绘制模板边框状态
            {
                
                templetLoc = new Point(e.X, e.Y);   //记录模板开始的点
                SetTemplettextBox2.Text = e.X.ToString() + "," + e.Y.ToString();
                CurrentTemplet = new Point(e.X, e.Y);
                startDrawTemplet = true;   //正在进行模板绘制
            }
            picturebox.Invalidate();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            currentMouse = new Point(e.X, e.Y);
            tooltip.SetToolTip(picturebox, e.X.ToString() + "," + e.Y.ToString());
            if (bmp != null)
            {
                //Notice.Text = "picture size:" + bmp.Size.Width.ToString() + "," + bmp.Size.Height.ToString() + ";\r" + "current mouse position:" + e.X.ToString() + "," + e.Y.ToString() + ";";
            }
            //locationLabel.Text = currentMouse.X.ToString() + "," + currentMouse.Y.ToString();
            if (isSreenShot || isMeasureLen )
            {
                (sender as PictureBox).Invalidate();
            }
            if (startDrawTemplet)  //处于鼠标拖拉绘制模板边框
            {
                CurrentTemplet = new Point(e.X, e.Y);
                (sender as PictureBox).Invalidate();
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            #region 截图
            if (isSreenShot)
            {
                endpoint = new Point(e.X, e.Y);
                DialogResult result = MessageBox.Show("是否确定所取区域", "截图", MessageBoxButtons.OKCancel);
                if (result.Equals(DialogResult.OK))
                {
                    DrawBorder = true;
                
                        if (endpoint.X > picturebox.Size.Width)
                            endpoint.X = picturebox.Size.Width;
                        if (endpoint.Y > picturebox.Size.Height)
                            endpoint.Y = picturebox.Size.Height;
                    
   
                        leftX = (int)(startpoint.X * Math.Pow(2, -SizeChangecount));
                        leftY = (int)(startpoint.Y * Math.Pow(2, -SizeChangecount));
                        rightX = (int)(endpoint.X * Math.Pow(2, -SizeChangecount));
                        rightY = (int)(endpoint.Y * Math.Pow(2, -SizeChangecount));
                    picturebox.Invalidate();
                }
                else
                {
                    (sender as PictureBox).Invalidate();
                }
            }
            isSreenShot = false;

            #endregion

            #region 添加数字框

            #endregion

            if (isDrawTemplate)   //结束绘制模板边框，将模板边框信息保存模板队列中
            {
                SetTemplettextBox3.Text = (e.X-templetLoc.X).ToString() + "," + (e.Y-templetLoc.Y).ToString();
                isDrawTemplate = false;
                startDrawTemplet = false;
                isShowTemplateBorder = true;
                templetFlag.templetPoint = templetLoc;
                templetFlag.templetSize = new Size(e.X - templetLoc.X, e.Y - templetLoc.Y);
                NextStep.Enabled = true;
            }

            #region 测量
            if (isMeasureLen)
            {
                measureEndPoint = new Point((int)(e.X * Math.Pow(2, -SizeChangecount)), (int)(e.Y * Math.Pow(2, -SizeChangecount)));
                DrawMeasure = true;
                startMeasure = false;
                isMeasureLen = false;
                Notice.Text = "current length:" + Math.Sqrt(Math.Pow(e.Y - measureStartPoint.Y, 2) + Math.Pow(e.X - measureStartPoint.X, 2)).ToString();
            }
            startdraw = false;
            startpoint = new Point(0, 0);
            #endregion
            picturebox.Invalidate();
        }
        
        private void UpdataTemplet()    //更新模板内特征点数据
        {
            int flag = 0;
            int Piccount=1; //小图像在拼图中的序列
            int Feathercount = 0;
            Feather tmpfeather = new Feather();
            while (flag<10)//模板只有十个数字的特征点
            {
                if (ExistNumber.Contains(flag))      
                {
                    tmpfeather.featherPoints = new featherRgnstruct[NumberPoints];
                    foreach (featherRgnstruct tmp in featherRegion)
                    {
                        if ((Piccount - 1) * templetFlag.templetSize.Width < tmp.rec.X && tmp.rec.X < Piccount * templetFlag.templetSize.Width) //会导致横坐标为零或者等于宽度的点不被保存
                        {
                            if (Feathercount < NumberPoints)
                            {
                                tmpfeather.featherPoints[Feathercount] = tmp;
                                tmpfeather.featherPoints[Feathercount].rec.X = tmp.rec.X - (Piccount - 1) * templetFlag.templetSize.Width;
                                tmpfeather.featherPoints[Feathercount++].Pixel = ((Bitmap)(picturebox.Image)).GetPixel(tmp.rec.X, tmp.rec.Y).R;
                            }
                        }
                    }
                    for (int i = 0; i < NumberPoints; i++)
                    {
                        templetFlag.feathers.featherPoints[flag * NumberPoints + i] = tmpfeather.featherPoints[i];
                    }
                    Piccount++;
                }
                flag++;
                Feathercount = 0;
            }
        }

        private void picturebox_MouseLeave(object sender, EventArgs e)
        {
            //locationLabel.Text = "0,0";
            //Notice.Text = null;
        }

        private void AutoPaintRectangle(object sender, PaintEventArgs e)//picturebox重绘时调用 
        {
            int x, y, width, height;
            if (isDesignBorder)
            {
                foreach (Numberborder tmp in showborder)
                {

                    x = tmp.start.X;
                    y = tmp.start.Y;
                    Rectangle rec = new Rectangle(new Point(x, y), tmp.size);
                    e.Graphics.DrawRectangle(Pens.Blue, rec);
                }
            }
            else if (isDesignFeather)
            {
                foreach (featherRgnstruct tmpfeather in showfeature)
                {
                    Rectangle tmprec = tmpfeather.rec;
                    x = tmprec.X;
                    y = tmprec.Y;
                    width = tmprec.Width;
                    height = tmprec.Height;
                    Rectangle rec = new Rectangle(x - (width - 1) / 2, y - (height - 1) / 2, width, height);
                    e.Graphics.DrawRectangle(Pens.Blue, rec);
                }

            }

            if (DrawCurrentBorder && borderRegion.Count > 0)
            {
                CurrentCellRec = new Rectangle((int)(int.Parse(BorderdataGrid.Rows[BorderdataGrid.CurrentRow.Index].Cells[0].Value.ToString())*Math.Pow(2,SizeChangecount)),
               (int)(int.Parse(BorderdataGrid.Rows[BorderdataGrid.CurrentRow.Index].Cells[1].Value.ToString())*Math.Pow(2,SizeChangecount)),(int)(templetFlag.templetSize.Width*Math.Pow(2,SizeChangecount)),
                (int)(templetFlag.templetSize.Height*Math.Pow(2,SizeChangecount)));
               e.Graphics.DrawRectangle(Pens.Red, CurrentCellRec);
            }

            if (DrawCurrentFeather && featherRegion.Count > 0 && isDesignFeather)
            {
                CurrentCellRec = new Rectangle(int.Parse(FeatherdataGrid.Rows[FeatherdataGrid.CurrentRow.Index].Cells[0].Value.ToString()) - 1,
                    int.Parse(FeatherdataGrid.Rows[FeatherdataGrid.CurrentRow.Index].Cells[1].Value.ToString()) - 1,
                3, 3);
                e.Graphics.DrawRectangle(Pens.Red, CurrentCellRec);
            }

            //if (DrawBorder)
            //{
            //    e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)(leftX * Math.Pow(2, SizeChangecount)), (int)(leftY * Math.Pow(2, SizeChangecount)),
            //        (int)((rightX - leftX) * Math.Pow(2, SizeChangecount)), (int)((rightY - leftY) * Math.Pow(2, SizeChangecount))));
            //}
            if (isMeasureLen && startMeasure)
            {
                e.Graphics.DrawLine(Pens.Red, measureStartPoint, currentMouse);

            }
            if (DrawMeasure)
            {
                Point tmpStart = new Point((int)(measureStartPoint.X * Math.Pow(2, SizeChangecount)), (int)(measureStartPoint.Y * Math.Pow(2, SizeChangecount)));
                Point tmpEnd = new Point((int)(measureEndPoint.X * Math.Pow(2, SizeChangecount)), (int)(measureEndPoint.Y * Math.Pow(2, SizeChangecount)));
                e.Graphics.DrawLine(Pens.Red, tmpStart, tmpEnd);
            }
            if (isSreenShot && startdraw)
            {
                e.Graphics.DrawRectangle(Pens.Gray, new Rectangle(startpoint, new Size(currentMouse.X - startpoint.X, currentMouse.Y - startpoint.Y)));
            }
            if (startDrawTemplet && bmp != null)
            {
                Rectangle rec = new Rectangle(templetLoc.X, templetLoc.Y, CurrentTemplet.X - templetLoc.X, CurrentTemplet.Y - templetLoc.Y);
                e.Graphics.DrawRectangle(Pens.Red, rec);
            }
            if (isShowTemplateBorder && CurrentTempletName != null)
            {
               
                e.Graphics.DrawRectangle(Pens.Red, new Rectangle((int)(templetFlag.templetPoint.X* Math.Pow(2, SizeChangecount)),(int)(templetFlag.templetPoint.Y* Math.Pow(2, SizeChangecount)),
                   (int)(templetFlag.templetSize.Width * Math.Pow(2, SizeChangecount)), (int)(templetFlag.templetSize.Height * Math.Pow(2, SizeChangecount))));
                
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

            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();

                return bmpOut;
            }
            catch
            {
                return null;
            }
        }

        //保存模板和特征值数据
        private void SaveTemplet()
        {
            if (!Directory.Exists(savePath + "\\NumberBmp\\" + CurrentTempletName.ToString()))
            {
                Directory.CreateDirectory(savePath + "\\NumberBmp\\" + CurrentTempletName.ToString());
            }

            if (File.Exists(savePath + "\\NumberBmp\\"+CurrentTempletName.ToString()+"\\" + "templet.txt")) //如果存在templet.txt文件则先删除
            {
                File.Delete(savePath + "\\NumberBmp\\" + CurrentTempletName.ToString() + "\\" + "templet.txt");
            }
            StreamWriter streamwriter = File.CreateText(savePath + "\\NumberBmp\\" + CurrentTempletName.ToString() + "\\" + "templet.txt");
            string data = null;
            data+=templetFlag.templetName.ToString()+","+templetFlag.templetPoint.X.ToString()+","+templetFlag.templetPoint.Y.ToString()+","
                +templetFlag.templetSize.Width.ToString()+","+templetFlag.templetSize.Height.ToString();
            for (int i = 0; i < NumberPoints*10; i++)
            {
                data += "," + templetFlag.feathers.featherPoints[i].rec.X.ToString() + "," + templetFlag.feathers.featherPoints[i].rec.Y.ToString() +
                    "," + templetFlag.feathers.featherPoints[i].Pixel.ToString()+"," + templetFlag.feathers.featherPoints[i].featherNum.ToString();
            }
            streamwriter.WriteLine(data);
            streamwriter.Close();
        }

        private void SaveFeather()  //将特征点数据保存到feather.h文件中
        {
            StreamWriter streamwriter;
            string alltext = null;
            string data = null;
            int count = 0;
            string VersonNum = "0,";
            string IsGray = "0,";
            if (File.Exists(savePath + "\\feather.h"))  
                File.Delete(savePath + "\\feather.h");
            streamwriter = File.CreateText(savePath + "\\" + "feather.h");
            alltext += VersonNum;
            alltext += IsGray;
            alltext += CameraData;
            try
            {
                leftX = borderRegion.First().start.X;
                leftY = borderRegion.First().start.Y;
                rightX = borderRegion.First().start.X + borderRegion.First().size.Width;
                rightY = borderRegion.First().start.Y + borderRegion.First().size.Height;
                foreach (Numberborder tmp in borderRegion)
                {
                    if (tmp.start.Y + borderRegion.First().size.Height > rightY)
                    {
                        rightY = tmp.start.Y + borderRegion.First().size.Height;
                    }
                    if (tmp.start.X + borderRegion.First().size.Width > rightX)
                    {
                        rightX = tmp.start.X + borderRegion.First().size.Width;
                    }
                    if (tmp.start.X < leftX)
                    {
                        leftX = tmp.start.X;
                    }
                    if (tmp.start.Y < leftY)
                    {
                        leftY = tmp.start.Y;
                    }
                }
            }
            catch
            {
                if (bmp != null)
                {
                    leftX = 0;
                    leftY = 0;
                    rightX = bmp.Size.Width;
                    rightY = bmp.Size.Height;
                }
            }

            alltext += leftX.ToString() + ',' + leftY.ToString() + ',' + rightX.ToString() + ',' + rightY.ToString() ;
            while (count < 10)       //每次识别最多识别十个数字
            {
                Numberborder tmp = new Numberborder();
                //tmp.FeatherName = null;
                if (borderRegion.Count > 0)   //一次设计选择的识别区域均用一套模板来识别
                {
                    tmp = borderRegion.First();
                    borderRegion.Remove(tmp);
                    alltext +=","+ tmp.start.X.ToString()+"," + tmp.start.Y.ToString()+"," + tmp.size.Width.ToString()+"," + tmp.size.Height.ToString();
                }
                else
                {
                    alltext += ",0,0,0,0";

                }
                for (int i = 0; i < NumberPoints*10; i++)
                    alltext +=","+ templetFlag.feathers.featherPoints[i].rec.X.ToString()+","
                        + templetFlag.feathers.featherPoints[i].rec.Y.ToString() + ","+
                        templetFlag.feathers.featherPoints[i].Pixel.ToString()+","
                        + templetFlag.feathers.featherPoints[i].featherNum.ToString();

                count++;
            }
            data += " image_data_packet_desc_type  image_data_packet_desc={" + alltext + "};";
            streamwriter.WriteLine(data);
            streamwriter.Close();
        }

        private void MatchFM_FormClosed(object sender, FormClosedEventArgs e)
        {


        }

        private void Closebt_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否确定退出", "退出", MessageBoxButtons.OKCancel);
            if (result.Equals(DialogResult.OK))
            {
                this.Close();
            }
        }

        private void saveDatabt_Click(object sender, EventArgs e)
        {
            SaveFeather();
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "bmp files (*.bmp)|*.bmp";
            openFileDialog.FilterIndex = 2;
            openFileDialog.InitialDirectory = savePath;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)   //打开一张图片
            {
                DrawCurrentBorder = false;
                DrawCurrentFeather = false;
                isaddNumborder = false;
                OpenImgbutton1.Enabled = true;
                OpenImgtextBox1.Enabled = true;

                //AddBorder.Enabled = false;
                //Addfeather.Enabled = true;
                SizeChangecount = 0;

                DrawBorder = false;

                bmp = (Bitmap)Image.FromFile(openFileDialog.FileName);
                lefttop = new Point(0, 0);
                rightbottom = new Point(bmp.Size.Width, bmp.Size.Height);

                picturebox.Parent = Mainpanel;
                picturebox.Image = bmp;
                showBmp = bmp;
                //leftToptoolStripStatusLabel.Text = "0,0";
                //rightbottomtoolStripStatusLabel.Text = rightX.ToString() + "," + rightY.ToString();

                picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
                picturebox.Size = showBmp.Size;
                normalSize = showBmp.Size;
                RePlacebitmap(bmp);
                picturebox.Show();
                SizeChangecount = 0;
            }
            picturebox.Invalidate();
        }

        private void PicFormLocality_Click(object sender, EventArgs e)
        {
            OpenFile();
            if (bmp != null)
                NextStep.Enabled = true; //打开一张图片后 Next按钮使能置为true
        }

        private void amplification_Click(object sender, EventArgs e)
        {
            if (SizeChangecount < 3 && isDesignBorder)
            {
                Mainpanel.AutoScroll = true;
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
                SizeChangecount++;
               // BorderdataGrid.Rows.Clear();
                List<Numberborder> backupborder = new List<Numberborder>();
                foreach (Numberborder tmp in borderRegion)
                {
                    Numberborder border = tmp;
                    border.start = new Point((int)(tmp.start.X * Math.Pow(2, SizeChangecount)), (int)(tmp.start.Y * Math.Pow(2, SizeChangecount)));
                    border.size = new Size((int)(tmp.size.Width * Math.Pow(2, SizeChangecount)), (int)(tmp.size.Height * Math.Pow(2, SizeChangecount)));
                    backupborder.Add(border);
                    //BorderdataGrid.Rows.Add(border.start.X, border.start.Y, border.end.X, border.end.Y);
                }
                showborder = backupborder;
                picturebox.Invalidate();

            }
            else if (SizeChangecount < 3 && isDesignFeather)
            {
                Mainpanel.AutoScroll = true;
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
                SizeChangecount++;
                List<featherRgnstruct> backupFeather = new List<featherRgnstruct>();
                foreach (featherRgnstruct tmp in featherRegion)
                {
                    featherRgnstruct feather = tmp;
                    feather.rec.Location = new Point((int)(tmp.rec.Location.X * Math.Pow(2, SizeChangecount)), (int)(tmp.rec.Location.Y * Math.Pow(2, SizeChangecount)));
                    backupFeather.Add(feather);
                }
                showfeature = backupFeather;
                picturebox.Invalidate();
            }
        }

        private void lessen_Click(object sender, EventArgs e)
        {

            if (SizeChangecount > -3 && isDesignBorder)
            {
                picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
                picturebox.Parent = splitContainer2.Panel1;
                picturebox.Parent = Mainpanel;
                picturebox.Size = new Size(picturebox.Size.Width / 2, picturebox.Size.Height / 2);
                if (picturebox.Size.Width > Mainpanel.Width && picturebox.Size.Height > Mainpanel.Height)
                    picturebox.Location = new Point(0, 0);
                else if (picturebox.Size.Width > Mainpanel.Width)
                    picturebox.Location = new Point(0, (Mainpanel.Height - picturebox.Size.Height) / 2);
                else if (picturebox.Size.Height > Mainpanel.Height)
                    picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, 0);
                else
                    picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, (Mainpanel.Height - picturebox.Size.Height) / 2);
                SizeChangecount--;
                Notice.Text = "current bmp size:" + picturebox.Size.Width.ToString() + "," + picturebox.Size.Height.ToString();
                //BorderdataGrid.Rows.Clear();
                List<Numberborder> backupborder = new List<Numberborder>();
                foreach (Numberborder tmp in borderRegion)
                {
                    Numberborder border = tmp;
                    border.start = new Point((int)(tmp.start.X * Math.Pow(2, SizeChangecount)), (int)(tmp.start.Y * Math.Pow(2, SizeChangecount)));
                    border.size = new Size((int)(tmp.size.Width * Math.Pow(2, SizeChangecount)), (int)(tmp.size.Height * Math.Pow(2, SizeChangecount)));
                    backupborder.Add(border);
                    //BorderdataGrid.Rows.Add(border.start.X, border.start.Y, border.end.X, border.end.Y);
                }
                showborder = backupborder;
                picturebox.Invalidate();
            }
            else if (SizeChangecount > -3 && isDesignFeather)
            {
                picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
                picturebox.Parent = splitContainer2.Panel1;
                picturebox.Parent = Mainpanel;
                picturebox.Size = new Size(picturebox.Size.Width / 2, picturebox.Size.Height / 2);
                if (picturebox.Size.Width > Mainpanel.Width && picturebox.Size.Height > Mainpanel.Height)
                    picturebox.Location = new Point(0, 0);
                else if (picturebox.Size.Width > Mainpanel.Width)
                    picturebox.Location = new Point(0, (Mainpanel.Height - picturebox.Size.Height) / 2);
                else if (picturebox.Size.Height > Mainpanel.Height)
                    picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, 0);
                else
                    picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, (Mainpanel.Height - picturebox.Size.Height) / 2);
                SizeChangecount--;
                Notice.Text = "current bmp size:" + picturebox.Size.Width.ToString() + "," + picturebox.Size.Height.ToString();
                FeatherdataGrid.Rows.Clear();
                List<featherRgnstruct> backupFeather = new List<featherRgnstruct>();
                foreach (featherRgnstruct tmp in showfeature)
                {
                    featherRgnstruct feather = tmp;
                    feather.rec.Location = new Point(tmp.rec.Location.X / 2, tmp.rec.Location.Y / 2);
                    backupFeather.Add(feather);
                    FeatherdataGrid.Rows.Add(feather.rec.Location.X, feather.rec.Location.Y, feather.featherNum);
                }
                showfeature = backupFeather;
                picturebox.Invalidate();
            }
        }

        private void DefaultSize_Click(object sender, EventArgs e)
        {
            if (isDesignBorder)
            {
                picturebox.Parent = null;
                picturebox.Parent = splitContainer2.Panel1;
                picturebox.Location = new Point(0, 0);
                BorderdataGrid.Rows.Clear();
                foreach (Numberborder tmp in borderRegion)
                {
                    //FeatherdataGrid.Rows.Add(tmp.rec.X, tmp.rec.Y, tmp.featherNum);
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
                picturebox.Size = normalSize;
                showborder = borderRegion;
                Notice.Text = "current bmp size:" + picturebox.Size.Width.ToString() + "," + picturebox.Size.Height.ToString();

                //picturebox.Location = new Point((Mainpanel.Width - picturebox.Size.Width) / 2, (Mainpanel.Height - picturebox.Size.Height) / 2);
            }
            else if (isDesignFeather)
            {
                picturebox.Parent = null;
                picturebox.Parent = splitContainer2.Panel1;
                FeatherdataGrid.Rows.Clear();
                foreach (featherRgnstruct tmp in featherRegion)
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
            }
        }

        private void AddPoint_Click(object sender, EventArgs e)  //添加数字框
        {
            SizeChangecount = 0;
            isShowTemplateBorder = false;
            DatagridviewgroupBox.Text = "Border Data";  //切换表格
            isDesignBorder = true;            //进入选取识别框状态
            
            BorderdataGrid.BringToFront();    //将显示边框数据的表格显示到表面
            FeatherdataGrid.Rows.Clear();
            foreach (Button tmp in Designtools.Controls)
            {
                tmp.Enabled = false;
            }
            BasegroupBox.Controls.Clear();  //清空basegroupbox内控件
            CurrentStep = 4;
            AddFeather.Enabled = true;
            MakeMod.Enabled = false;
            picturebox.Invalidate();
        }


        private void Addfeather_Click(object sender, EventArgs e) //添加特征点
        {
            if (CurrentStep == 3)
            {
                DatagridviewgroupBox.Text = "Feather Data";
                FeatherdataGrid.BringToFront();
                isaddFeather = true;   //进入增加特征点状态
                picturebox.Invalidate();
            }
            else if(CurrentStep==4)
            {
                isaddNumborder = true;
                NextStep.Enabled = true;
            }
        }

        private void DeleteFeather_Click(object sender, EventArgs e)
        {
            isaddNumborder = false;
            BorderdataGrid.Rows.Clear();
            showborder.Clear();
            borderRegion.Clear();
            picturebox.Invalidate();
        }

        private void PicFromCamera_Click(object sender, EventArgs e)
        { 
            PicFromCamera.Enabled = false;   //摄像头读入图片时按钮使能设置为false

            ReadImageFromCamera();           //调用摄像头读图函数

            PicFromCamera.Enabled = true;
        }

        private void SavePic_Click(object sender, EventArgs e)  //绘制border
        {
            bmp.Save(savePath + "\\currentImage" + currentimageCount++.ToString() + ".bmp");
        }

        private void AutoFeatherPoint_Click(object sender, EventArgs e)//自动获取特征点
        {
            Formtimer.Stop();
            isaddNumborder = false;
            BasegroupBox.Controls.Clear();
            InitializeAutoFeather(BasegroupBox);

        }

        private void MeasureLen_Click(object sender, EventArgs e)
        {
            isaddNumborder = false;
            isaddFeather = false;
            BasegroupBox.Controls.Clear();
            isMeasureLen = true;
            isSreenShot = false;
            DrawMeasure = false;

            picturebox.Invalidate();
        }

        private void BinaryTool_Click(object sender, EventArgs e)
        {
            BasegroupBox.Controls.Clear();
            List<Bitmap> threshodbmp = new List<Bitmap>();
            foreach (Bitmap tmp in btList)
            {
                int threshod = AutoThroshod(tmp);
                threshodbmp.Add(BinaryImage(threshod,tmp));
            }
            PasteNumber(threshodbmp);
           //picturebox.Image = BinaryImage(threshod, (Bitmap)picturebox.Image);

            BinaryTool.Enabled = false;
            NextStep.Enabled = true;
            AddFeather.Enabled = true;
            AutoFeather.Enabled = true;
            showfeature.Clear();
            featherRegion.Clear();
            FeatherdataGrid.Rows.Clear();

            //while()
            for (int i = 0; i < NumberPoints*10; i++)
            {
                if(templetFlag.feathers.featherPoints[i].featherNum!=0)
                {
                    featherRegion.Add(templetFlag.feathers.featherPoints[i]);
                    FeatherdataGrid.Rows.Add(templetFlag.feathers.featherPoints[i].rec.X,
                        templetFlag.feathers.featherPoints[i].rec.Y,
                        templetFlag.feathers.featherPoints[i].featherNum
                        );
                    featherRgnstruct tmp=new featherRgnstruct();
                    tmp.rec=new Rectangle((int)(templetFlag.feathers.featherPoints[i].rec.X*Math.Pow(2,SizeChangecount)),
                        (int)(templetFlag.feathers.featherPoints[i].rec.Y*Math.Pow(2,SizeChangecount)),
                       (int) (templetFlag.feathers.featherPoints[i].rec.Width*Math.Pow(2,SizeChangecount)),
                       (int) (templetFlag.feathers.featherPoints[i].rec.Height*Math.Pow(2,SizeChangecount)));
                    tmp.classifyNUM = templetFlag.feathers.featherPoints[i].classifyNUM;
                    tmp.featherNum = templetFlag.feathers.featherPoints[i].featherNum;
                    showfeature.Add(tmp);
                }
            }
            string data = null;
            StreamWriter streamwriter = streamwriter = File.CreateText(savePath + "\\NumberBmp\\" + CurrentTempletName.ToString() + "\\" + "AutoFeatherConfig.txt");
            data += "0," + ExistNumber.Count;
            foreach(int tmp in ExistNumber)
            {
                data+=","+tmp.ToString();
            }
            streamwriter.Write(data);
            streamwriter.Close();
        }

        private void SetMod_Click(object sender, EventArgs e)
        {
            isDesignFeather = false;
            isDesignBorder = false;

            FeatherdataGrid.BringToFront();
            MakeMod.Enabled = false;                  //将下几部的按钮使能设置为false
            CutNumberBmp.Enabled = false;
            AddBorder.Enabled = false;
            foreach (Button tmp in Designtools.Controls) //设计框内的所有按钮初始化为disable状态
            {
                tmp.Enabled = false;
            }
            BasegroupBox.Controls.Clear();  //清空basegroupbox内控件
            CurrentStep = 1;
            InitializeSetMod(BasegroupBox); //初始化basegroupbox内用于增加一套模板的控件  
            DatagridviewgroupBox.Text = "Templet Data";
            FeatherdataGrid.Rows.Clear();
            FeatherdataGrid.Enabled = false;
            string numberBmpPath = savePath + "\\NumberBmp";
            if (!Directory.Exists(numberBmpPath))
            {
                Directory.CreateDirectory(numberBmpPath);
            }
            DirectoryInfo TheFolder = new DirectoryInfo(savePath + "\\NumberBmp");
            FileSystemInfo[] NextFile = TheFolder.GetDirectories(); //遍历文件夹内文件
            for (int i = 0; i < NextFile.Length;i++ )
                FeatherdataGrid.Rows.Add((i+1).ToString(),NextFile[i].Name, null); //将存在的模板显示到表格上
            SizeChangecount = 0;
            picturebox.Image = bmp;
            picturebox.Size = bmp.Size;
            RePlacebitmap(bmp);
        }

        private void MakeMod_Click(object sender, EventArgs e)
        {
            DatagridviewgroupBox.Text = "Feather Data";
            FeatherdataGrid.BringToFront();
            FeatherdataGrid.Enabled = true; //是用户可以改变表格内数据
            
            AddBorder.Enabled = false;      //将后一步的按钮使能设置为false
            FeatherdataGrid.Rows.Clear();   //清空表格内原有数据
            isDesignFeather = true;
            isDesignBorder = false;

            SetMod.Enabled = false;  //将新建模板按钮使能置为false，防止模板内图片在使用时进行覆盖(删除文件)操作使程序奔溃
            CutNumberBmp.Enabled = false;
            btList = new List<Bitmap>(); //建立队列存储存在的数字图片
            ExistNumber.Clear();         //清空原先存在的数字数据

            foreach (Button tmp in Designtools.Controls)
            {
                tmp.Enabled = false;
            }
            BasegroupBox.Controls.Clear();  //清空basegroupbox内控件

            DirectoryInfo TheFolder = new DirectoryInfo(savePath + "\\NumberBmp" + "\\" + CurrentTempletName);

            for (int flag = 0; flag < 10; flag++)
            {
                foreach (FileInfo NextFile in TheFolder.GetFiles())  //遍历文件夹内文件
                {
                    if (flag.ToString() + ".bmp" == NextFile.Name)
                    {
                        Bitmap bt = (Bitmap)Bitmap.FromFile(NextFile.FullName);
                        btList.Add(bt);
                        ExistNumber.Add(flag);
                        break;
                    }
                }
            }
            PasteNumber(btList);           //将截图保存下来的图片拼接成一张大图
            if (CurrentStep < 3)  //只有在前几部处于第三步之前时才进行特征点相对坐标转化
            {
                CalcExistFeather();
            }
            normalSize = picturebox.Size;
            BinaryTool.Enabled = true;
            CurrentStep = 3;    //将当前步骤设置为第三步
        }

        private void CalcExistFeather() //将读入的特征点坐标转化为合成的大图的坐标
        {
            int flag=0;
            int count=0;
            while (flag<10)
            {
                if (ExistNumber.Contains(flag))
                {
                    for (int i = 0; i < NumberPoints; i++)
                    {
                        if (templetFlag.feathers.featherPoints[flag * NumberPoints + i].rec.X != 0 || templetFlag.feathers.featherPoints[flag * NumberPoints + i].rec.Y != 0)
                            templetFlag.feathers.featherPoints[flag * NumberPoints + i].rec.X += count * templetFlag.templetSize.Width;
                    }
                    count++;
                }
                flag++;
            }
 
        }

        private void PasteNumber(List<Bitmap> tmpList)//将截图所得图片拼接成大图
        {
            try
            {
                byte[,] bitData;

                bitData = new byte[tmpList.First().Height, tmpList.First().Width * tmpList.Count]; //新建个大小为所有图片拼接起来大小的数组
                int flag = 0;
                foreach (Bitmap tmp in tmpList)   //将图像像素点填充到数组里
                {
                    for (int i = 0; i < tmp.Height; i++)
                    {
                        for (int j = 0; j < tmp.Width; j++)
                        {
                            bitData[i, j + tmp.Width * flag] = tmp.GetPixel(j, i).R;
                        }
                    }
                    flag++;
                }
                ByteToImage bytetoimage = new ByteToImage();   //将数组转化为图片
                Bitmap bitmap = bytetoimage.FromGray(bitData);
                picturebox.Size = new System.Drawing.Size(tmpList.First().Width * tmpList.Count, tmpList.First().Height);
                picturebox.Image = bitmap;
                RePlacebitmap(bitmap); //将图片放在panel中心位置
            }
            catch
            {
                MessageBox.Show("图片有误");
            }
        }

        private void savefeatherbt_Click(object sender, EventArgs e)
        {
            SaveFeather();
        }

        private void valueTextbox_TextChanged(object sender, EventArgs e)
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

        private void MatchFeather_Click(object sender, EventArgs e)
        {
        
        }

        private void SaveToh_Click(object sender, EventArgs e)
        {
            SaveFeather();
            SaveToh.Enabled = false;
        }

        private void BorderdataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DrawCurrentBorder = true;
            DrawCurrentFeather = false;
            int currentRow = BorderdataGrid.CurrentRow.Index;
            int startX = int.Parse(BorderdataGrid.Rows[currentRow].Cells[0].Value.ToString());
            int startY = int.Parse(BorderdataGrid.Rows[currentRow].Cells[1].Value.ToString());
            CurrentCellRec = new Rectangle(new Point(startX, startY),templetFlag.templetSize);
            picturebox.Invalidate();
        }

        private void BorderGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int flag = 0;
            int feathernum = borderRegion.Count;
            borderRegion.Clear();
            showborder.Clear();
            while (flag < feathernum)
            {
                Numberborder tmp = new Numberborder();
                int pointX = int.Parse(BorderdataGrid.Rows[flag].Cells[0].Value.ToString());
                int pointY = int.Parse(BorderdataGrid.Rows[flag].Cells[1].Value.ToString());
                tmp.start = new Point(pointX, pointY);
                tmp.size = templetFlag.templetSize;
                //tmp.FeatherName = BorderdataGrid.Rows[flag].Cells[3].Value.ToString();
                borderRegion.Add(tmp);
                
                Point tmpPoint = new Point((int)(pointX * Math.Pow(2, SizeChangecount)), (int)(pointY * Math.Pow(2, SizeChangecount)));
                tmp.start = tmpPoint;
                tmp.size = new Size((int)(templetFlag.templetSize.Width * Math.Pow(2, SizeChangecount)), (int)(templetFlag.templetSize.Height * Math.Pow(2, SizeChangecount)));
               // tmp.end = tmpPoint;
                showborder.Add(tmp);
                flag++;
            }
            picturebox.Invalidate();
        }

        private void FeatherdataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (CurrentStep == 3)
            {
                DrawCurrentFeather = true;
                DrawCurrentBorder = false;
                picturebox.Invalidate();
            }
        }

        #region 初始化右下角GroupBox控件
        #region 定义控件变量
        Button AutoFeatherbutton1 = new Button();
        Label AutoFeatherlabel1 = new Label();
        TextBox AutoFeathertextBox1 = new TextBox();
        ComboBox AutoAddFeathercomboBox = new ComboBox();
        Label AutoFeatherlabel2 = new Label();

        Button OpenImgbutton1 = new Button();
        Label OpenImglabel1 = new Label();
        TextBox OpenImgtextBox1 = new TextBox();


        private System.Windows.Forms.Label SetTempletlabel1 = new Label();
        private System.Windows.Forms.Label SetTempletlabel2 = new Label();
        private System.Windows.Forms.Label SetTempletlabel3 = new Label();
        private System.Windows.Forms.TextBox SetTemplettextBox1 = new TextBox();
        private System.Windows.Forms.TextBox SetTemplettextBox2 = new TextBox();
        private System.Windows.Forms.TextBox SetTemplettextBox3 = new TextBox();
        private System.Windows.Forms.Button SetTempletbutton1 = new Button();
        private System.Windows.Forms.Button SetTempletbutton2 = new Button();
        #endregion


        private void InitializeMeasureLen(GroupBox baseGroup)
        {

        }

        private void InitializeSetMod(GroupBox baseGroup)
        {
            baseGroup.Controls.Add(this.SetTempletbutton1);
            baseGroup.Controls.Add(this.SetTemplettextBox3);
            baseGroup.Controls.Add(this.SetTemplettextBox2);
            baseGroup.Controls.Add(this.SetTemplettextBox1);
            baseGroup.Controls.Add(this.SetTempletlabel3);
            baseGroup.Controls.Add(this.SetTempletlabel2);
            baseGroup.Controls.Add(this.SetTempletlabel1);
            baseGroup.Text = "Set Templet";

            // 
            // SetTempletbutton2
            // 

            // 
            // SetTempletlabel3
            // 
            this.SetTempletlabel3.AutoSize = true;
            this.SetTempletlabel3.Location = new System.Drawing.Point(15, 106);
            this.SetTempletlabel3.Name = "SetTempletlabel3";
            this.SetTempletlabel3.Size = new System.Drawing.Size(77, 12);
            this.SetTempletlabel3.TabIndex = 16;
            this.SetTempletlabel3.Text = "Templet Size";
            // 
            // SetTemplettextBox3
            // 
            this.SetTemplettextBox3.Location = new System.Drawing.Point(128, 103);
            this.SetTemplettextBox3.Name = "SetTemplettextBox3";
            this.SetTemplettextBox3.Size = new System.Drawing.Size(50, 21);
            this.SetTemplettextBox3.TabIndex = 15;
            SetTemplettextBox3.Enabled = false;
            // 
            // SetTempletlabel2
            // 
            this.SetTempletlabel2.AutoSize = true;
            this.SetTempletlabel2.Location = new System.Drawing.Point(15, 73);
            this.SetTempletlabel2.Name = "SetTempletlabel2";
            this.SetTempletlabel2.Size = new System.Drawing.Size(101, 12);
            this.SetTempletlabel2.TabIndex = 14;
            this.SetTempletlabel2.Text = "Templet Location";
            // 
            // SetTemplettextBox2
            // 
            this.SetTemplettextBox2.Location = new System.Drawing.Point(128, 70);
            this.SetTemplettextBox2.Name = "SetTemplettextBox2";
            this.SetTemplettextBox2.Size = new System.Drawing.Size(50, 21);
            this.SetTemplettextBox2.TabIndex = 13;
            SetTemplettextBox2.Enabled = false;
            // 
            // SetTempletbutton1
            // 
            this.SetTempletbutton1.Location = new System.Drawing.Point(142, 143);
            this.SetTempletbutton1.Name = "SetTempletbutton1";
            this.SetTempletbutton1.Size = new System.Drawing.Size(42, 37);
            this.SetTempletbutton1.TabIndex = 4;
            this.SetTempletbutton1.Text = "OK";
            this.SetTempletbutton1.UseVisualStyleBackColor = true;
            try
            {
                this.SetTempletbutton1.Click -= new EventHandler(SetTempletbutton1_Click);
            }
            catch
            { }
            this.SetTempletbutton1.Click += new EventHandler(SetTempletbutton1_Click);
            // 
            // SetTemplettextBox1
            // 
            this.SetTemplettextBox1.Location = new System.Drawing.Point(128, 35);
            this.SetTemplettextBox1.Name = "SetTemplettextBox1";
            this.SetTemplettextBox1.Size = new System.Drawing.Size(50, 21);
            this.SetTemplettextBox1.TabIndex = 3;
            // 
            // SetTempletlabel1
            // 
            this.SetTempletlabel1.AutoSize = true;
            this.SetTempletlabel1.Location = new System.Drawing.Point(15, 38);
            this.SetTempletlabel1.Name = "SetTempletlabel1";
            this.SetTempletlabel1.Size = new System.Drawing.Size(77, 12);
            this.SetTempletlabel1.TabIndex = 1;
            this.SetTempletlabel1.Text = "Templet Name";
        }

        private void SetTempletbutton1_Click(object sender, EventArgs e)
        {
            if (SetTemplettextBox1.Text != null)
            {
                if (!Directory.Exists(savePath + "\\NumberBmp" + "\\" + SetTemplettextBox1.Text))
                {
                    Directory.CreateDirectory(savePath + "\\NumberBmp" + "\\" + SetTemplettextBox1.Text);
                    Templet templet = new Templet(); 
                    templet.templetName = SetTemplettextBox1.Text;
                    CurrentTempletName = SetTemplettextBox1.Text;
                    SetTemplettextBox2.Text = null;
                    SetTemplettextBox3.Text = null;
                    //if(SetTemplettextBox3.)
                    templet.templetSize = new Size(0, 0);
                    templet.feathers.featherPoints = new featherRgnstruct[NumberPoints*10];
                    for (int i = 0; i < NumberPoints*10; i++)
                    {
                        templet.feathers.featherPoints[i].featherNum = 0;
                        templet.feathers.featherPoints[i].rec = new Rectangle(0, 0, 0, 0);
                    }
                    templetFlag = templet;
                }
                else
                {
                    DialogResult result= MessageBox.Show("该名字模板已存在，是否覆盖原有数据","提示",MessageBoxButtons.OKCancel);
                    if (result.Equals(DialogResult.OK))
                    {
                        if (borderRegion != null)
                        {
                            borderRegion.Clear();
                            showborder.Clear();
                            BorderdataGrid.Rows.Clear();
                        }
                        Directory.Delete(savePath + "\\NumberBmp" +"\\"+ SetTemplettextBox1.Text, true);
                        Directory.CreateDirectory(savePath + "\\NumberBmp" + "\\" + SetTemplettextBox1.Text);
                        Templet templet = new Templet();
                        templet.templetName = SetTemplettextBox1.Text;
                        //if(SetTemplettextBox3.)
                        SetTemplettextBox2.Text = "";
                        SetTemplettextBox3.Text = "";
                        templet.templetPoint = new Point(0, 0);
                        templet.templetSize = new Size(0, 0);

                        templet.feathers.featherPoints = new featherRgnstruct[NumberPoints*10];
                        for (int i = 0; i < NumberPoints*10; i++)
                        {
                            templet.feathers.featherPoints[i].featherNum = 0;
                            templet.feathers.featherPoints[i].rec = new Rectangle(0, 0, 0, 0);
                        }
                        CurrentTempletName = SetTemplettextBox1.Text;
                        templetFlag = templet;
                    }
                    else if (result.Equals(DialogResult.Cancel))
                    {
                        CurrentTempletName = SetTemplettextBox1.Text;
                        templetFlag.templetName = SetTemplettextBox1.Text;
                        if (File.Exists(savePath + "\\NumberBmp" + "\\" + SetTemplettextBox1.Text + "\\" + "templet.txt"))
                        {
                            string streamreader = File.ReadAllText(savePath + "\\NumberBmp" + "\\" + SetTemplettextBox1.Text + "\\" + "templet.txt");
                            string[] Templetdata = streamreader.Split(',');
                            templetFlag.templetPoint = new Point(int.Parse(Templetdata[1]), int.Parse(Templetdata[2]));
                            templetFlag.templetSize = new System.Drawing.Size(int.Parse(Templetdata[3]), int.Parse(Templetdata[4]));
                            SetTemplettextBox2.Text = Templetdata[1] + "," + Templetdata[2];  //将文件里存的模板大小位置信息给到截取模板文本框内
                            SetTemplettextBox3.Text = Templetdata[3] + "," + Templetdata[4];
                            isShowTemplateBorder = true; //绘制模板框

                            for (int i = 0; i < NumberPoints*10; i++)
                            {
                                featherRgnstruct tmp = new featherRgnstruct();
                                tmp.rec = new Rectangle(int.Parse(Templetdata[5 + 4 * i]), int.Parse(Templetdata[6 + 4 * i]), 3, 3);
                                tmp.Pixel = int.Parse(Templetdata[7 + 4 * i]);
                                tmp.featherNum = int.Parse(Templetdata[8 + 4 * i]);

                                if (tmp.featherNum != 0)
                                {
                                    tmp.classifyNUM = i / NumberPoints;
                                }
                                templetFlag.feathers.featherPoints[i] = tmp;

                            }

                            NextStep.Enabled = true;
                            SetTemplettextBox2.Text = Templetdata[1].ToString() + "," + Templetdata[2].ToString();
                            SetTemplettextBox3.Text = Templetdata[3].ToString() + "," + Templetdata[4].ToString();
                        }
                        else
                        {
                        }
                    }
                }
                picturebox.Invalidate(); 
                DrawTempleBorder.Enabled = true;
                SetTemplettextBox3.Enabled = true;
                SetTemplettextBox2.Enabled = true;
            }
            else
            {
                MessageBox.Show("请输入模板名");
            }
        }

        private void SetTempletbutton2_Click(object sender, EventArgs e)
        {
            if (SetTemplettextBox1.Text == null)
            {
                MessageBox.Show("请输入模板名");
            }
            else
            {
                isaddFeather = false;
                isDrawTemplate = true;
            }
        }
        
        private int AutoThroshod(Bitmap tmpbmp)
        {
            int piexlsum, thresh_value, greyscale;
            int[] piexlcount = new int[256];               //每个像素点个数
            double[] piexlpercent = new double[256];       //每个像素点所占比重
            double w0, w1, u0tmp, u1tmp, u0, u1, u, deltaTmp, deltaMax;


            piexlsum = tmpbmp.Height * tmpbmp.Width; //全图的像素总数
            thresh_value = 0;                              //最佳阈值
            greyscale = 255;                               //255个像素点
            deltaMax = 0;

            for (int i = 0; i < greyscale; i++)//置每个灰度的像素数为0,对应比率为0
            {
                piexlcount[i] = 0;
                piexlpercent[i] = 0;
            }

            for (int y = 0; y < tmpbmp.Height; y++)
                for (int x = 0; x < tmpbmp.Width; x++)
                {
                    piexlcount[tmpbmp.GetPixel(x, y).R]++;//每级灰度的像素个数
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
            return thresh_value;
        }

        private Bitmap BinaryImage(int threshod,Bitmap tmpbmp)
        {
            try
            {
                int y = 0, x = 0;
                int[,] dealtData = new int[tmpbmp.Width, tmpbmp.Height];
                for (y = 0; y < tmpbmp.Height; y++)
                {
                    for (x = 0; x < tmpbmp.Width; x++)
                    {
                        if (tmpbmp.GetPixel(x, y).R < threshod)
                            dealtData[x, y] = 0;
                        else
                            dealtData[x, y] = 255;
                    }
                }
                /*图像边界全涂为0*/
                for (x = 0; x < tmpbmp.Width; x++)
                {
                    dealtData[x, 0] = 255;

                    dealtData[x, tmpbmp.Height - 1] = 255;
                }
                for (y = 0; y < tmpbmp.Height; y++)
                {
                    dealtData[0, y] = 255;
                    dealtData[tmpbmp.Width - 1, y] = 255;
                }
                Bitmap bmpOut = new Bitmap(tmpbmp.Width, tmpbmp.Height, PixelFormat.Format24bppRgb);
                for (y = 0; y < tmpbmp.Height; y++)
                {
                    for (x = 0; x < tmpbmp.Width; x++)
                    {
                        Color color = Color.FromArgb(dealtData[x, y], dealtData[x, y], dealtData[x, y]);
                        bmpOut.SetPixel(x, y, color);
                    }
                }
                picturebox.Image = tmpbmp;
                return bmpOut;
                //Graphics g = Graphics.FromImage(bmpOut);
                //g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                //g.Dispose();

                //return bmpOut;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }

        }

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
            try
            {
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

        private void AutoFeatherButton_Click(object sender, EventArgs e)
        {
            if (int.Parse(AutoFeathertextBox1.Text) > 20)
            {
                AutoFeathertextBox1.Text = "20";
            }
            else if (int.Parse(AutoFeathertextBox1.Text) <= 0)
            {
                AutoFeathertextBox1.Text = "0";
            }
            defaultRanNum = int.Parse(AutoFeathertextBox1.Text);
            borderRegion.Clear();
            BorderdataGrid.Rows.Clear();
            showborder.Clear();
            switch (AutoAddFeathercomboBox.Text)
            {
                case "genetic algorithm":
                    {
                        ConfigGen configgen = new ConfigGen(savePath,programPath);
                        configgen.Show();

                        picturebox.Invalidate();
                    }
                    break;
                case "random":
                    {
                        Random random = new Random();
                        for (int i = 0; i < defaultRanNum; i++)
                        {

                   
                        }
                        picturebox.Invalidate();
                    }
                    break;
            }
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
            tooltip.SetToolTip((sender as Button), "click this button to Undo\n the last operation ");

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

        ushort VDOffset = 0;

        public struct VDaddress
        {
            public int Chromaadd;
            public int Brightnessadd;
            public int Contrastadd;
            public int Saturationadd;
            public int Exposureadd;

        };

        VDaddress vdaddress = new VDaddress();

        private void CommunicateBt_Click(object sender, EventArgs e)
        {
            if (ConncetioncomboBox.Text == "Serial port")
            {
                SetSeriport();

                try
                {
                    ushort reSetAddress = 164;   //切换成大图模式
                    byte[] WriteData = new byte[2] { 1, 1 };
                    modbus.WriteSingleRegister(reSetAddress, ref WriteData);
                }
                catch
                {
                    MessageBox.Show("串口未能成功设置");
                }
                string[] addString = new string[5];
                //string path = StringOperateHelp.LeftOfRightmostOf(savePath, '\\');
                FileStream fs = new FileStream(programPath + PublicVariable.HardwareConfigFileName, FileMode.Open, FileAccess.Read);//读取文件设定
                StreamReader m_streamReader = new StreamReader(fs);
                string strLine;
                int flag = 0;
                while ((strLine = m_streamReader.ReadLine()) != null)
                {
                    if (CommonOperation.StringOperateHelp.LeftOf(strLine, ' ') == "hardware")
                    {
                        VDOffset = ushort.Parse(CommonOperation.StringOperateHelp.RightOf(strLine, ':'));
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
                FileStream comfs = new FileStream(savePath + "\\" + PublicVariable.CompileFileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(comfs, Encoding.Default);
                string text = Com.Text + "\r" + BaudRate.Text + "\r" + Databits.Text + "\r" + Stopbits.Text + "\r" + ParityCheck.Text + "\r" + StreamControl.Text;
                sw.Write(text);
                sw.Close();
                fs.Close();
                UpdataCameraData();
            }
        }

        private void UpdataCameraData()
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

        void SetSeriport()
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
                //serialport.Open();

                //serialport.DiscardInBuffer();
                //serialport.DiscardOutBuffer();
                modbus = new Modbus(serialport);

                //serialport.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Serial_Comm_Receive_Data);

                //serialport.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        #endregion


        private void CameraBt_Click(object sender, EventArgs e)
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
                CameraData = ChromatrackBar.Value.ToString() + "," + BrightnesstrackBar.Value.ToString() +
                    "," + ContrasttrackBar.Value.ToString() + "," + SatureationtrackBar.Value.ToString() +
                    "," + "0,0,0,0,";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void ZOOMIN_Click(object sender, EventArgs e)
        {
            byte[] ZOOMData = new byte[2];
            //ushort ZOOMAddress = (ushort)(70 + VDOffset);
            //modbus.ReadOneRegister(ZOOMAddress, ZOOMData);
            //modbus.WriteSingleRegister
        }

        private void FeatherdataGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {


            }
        }

        private void dEleteToolStripMenuItem1_Click(object sender, EventArgs e)   //右击删除表格内数据
        {
            try
            {
                featherRgnstruct flagfeather = new featherRgnstruct();
                flagfeather.featherNum = int.Parse(FeatherdataGrid.CurrentRow.Cells[2].Value.ToString());
                flagfeather.rec = new Rectangle(int.Parse(FeatherdataGrid.CurrentRow.Cells[0].Value.ToString()), int.Parse(FeatherdataGrid.CurrentRow.Cells[1].Value.ToString()),3,3);
               
                List<featherRgnstruct> tmpfeatherList1 = new List<featherRgnstruct>();
                foreach (featherRgnstruct tmp in featherRegion)
                {
                    flagfeather.classifyNUM = tmp.classifyNUM;
                    if (flagfeather.Equals(tmp))
                    {

                    }
                    else
                    {
                        tmpfeatherList1.Add(tmp);
                    }
                }
                //borderRegion = tmpfeatherList;
                featherRegion = tmpfeatherList1;
                //tmpfeatherList.Clear();
                List<featherRgnstruct> tmpfeatherList2 = new List<featherRgnstruct>();
                featherRgnstruct featherstruct = new featherRgnstruct();
                
                foreach (featherRgnstruct tmp in showfeature)
                {
                    featherstruct.rec = new Rectangle((int)(tmp.rec.X * Math.Pow(2, -SizeChangecount)), (int)(tmp.rec.Y * Math.Pow(2, -SizeChangecount)), 3, 3); 
                    featherstruct.classifyNUM = tmp.classifyNUM;
                    featherstruct.featherNum = tmp.featherNum;
                    if (flagfeather.Equals(featherstruct))
                    {

                    }
                    else
                    {
                        tmpfeatherList2.Add(tmp);
                    }
                }
                showfeature = tmpfeatherList2;
                FeatherdataGrid.Rows.Remove(FeatherdataGrid.CurrentRow);



                picturebox.Invalidate();
            }
            catch (Exception ex)
            {

            }
        }
      
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)//右击删除表格内数据
        {
            try
            {
                Numberborder flagfeather = new Numberborder();
                flagfeather.start = new Point(int.Parse(BorderdataGrid.CurrentRow.Cells[0].Value.ToString()), int.Parse(BorderdataGrid.CurrentRow.Cells[1].Value.ToString()));

                List<Numberborder> tmpfeatherList1 = new List<Numberborder>();
                foreach (Numberborder tmp in showborder)
                {
                    if (flagfeather.start.Equals(tmp.start))
                    {

                    }
                    else
                    {
                        tmpfeatherList1.Add(tmp);
                    }
                }
                //borderRegion = tmpfeatherList;
                showborder = tmpfeatherList1;
                //tmpfeatherList.Clear();
                List<Numberborder> tmpfeatherList2 = new List<Numberborder>();
                foreach (Numberborder tmp in borderRegion)
                {
                    Numberborder numberborder = new Numberborder();
                    numberborder.start = new Point((int)(tmp.start.X * Math.Pow(2, -SizeChangecount)), (int)(tmp.start.Y * Math.Pow(2, SizeChangecount)));
                    //numberborder.end = new Point((int)(tmp.end.X * Math.Pow(2, -SizeChangecount)), (int)(tmp.end.Y * Math.Pow(2, SizeChangecount)));
                    if (flagfeather.Equals(numberborder))
                    {

                    }
                    else
                    {
                        tmpfeatherList2.Add(tmp);
                    }
                }
                borderRegion = tmpfeatherList2;
                BorderdataGrid.Rows.Remove(BorderdataGrid.CurrentRow);



                picturebox.Invalidate();
            }
            catch (Exception ex)
            {

            }
        }

        private void FeatherdataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (CurrentStep == 3) //当前处于设计特征点状态时
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
                    showfeature.Add(tmp);
                    Rectangle rec = new Rectangle((int)(tmp.rec.X * Math.Pow(2, -SizeChangecount)), (int)(tmp.rec.Y * Math.Pow(2, -SizeChangecount)), 3, 3);
                    tmp.rec = rec;
                    featherRegion.Add(tmp);
                    flag++;
                }
                picturebox.Invalidate();
            }
        }

        private void valueTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyCode)
            {
                BorderdataGrid.Focus();
            }
        }

        private void Undobutton_Click(object sender, EventArgs e)
        {

        }

        public void ReadImageFromCamera()   //从摄像头读取一张图片
        {
            try
            {

                ushort Caddress = 1;
                byte[] writedata = new byte[2] { 1, 1 };

                bmp = modbus.ReadBmpFromCamera(Caddress, writedata);
                bmp.Save(savePath + "\\camerabmp" + (cameraImagecount++).ToString() + ".bmp",ImageFormat.Bmp);

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
                //leftToptoolStripStatusLabel.Text = "0,0";
                //rightbottomtoolStripStatusLabel.Text = rightX.ToString() + "," + rightY.ToString();

                picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
                picturebox.Size = showBmp.Size;
                normalSize = showBmp.Size;
                RePlacebitmap(bmp);  //调整读入图片在panel上位置

                picturebox.Show();
                SizeChangecount = 0;
                Notice.Text = null;
                NextStep.Enabled = true;    //NextStep设置为true
            }
            catch (Exception ex)
            {
                NextStep.Enabled = false;
                MessageBox.Show("请检查下串口，并重新连接");
            }
            finally
            {
                Mainpanel.AutoScroll = true;
                PicFromCamera.Enabled = true;
            }
        }

        private void UpdataCamera_Click(object sender, EventArgs e)
        {
            UpdataCameraData(); //将摄像头参数设置到采集板上
        }

        private void TabControl_Selected(object sender, TabControlEventArgs e) //tabcontrol切换时，串口数据的初始化
        {
            if ((sender as TabControl).SelectedTab.Text.Equals("Communications"))
            {
                Com.Items.Clear();
                string[] spname = System.IO.Ports.SerialPort.GetPortNames();
                for (int i = 0; i < spname.Length; i++)
                {
                    Com.Items.Add(spname[i]);
                }
                if (File.Exists(savePath +"\\"+ PublicVariable.CompileFileName))
                {
                    FileStream fs = new FileStream(savePath +"\\"+ PublicVariable.CompileFileName, FileMode.Open, FileAccess.Read);//读取文件设定
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

        private void OpenBmp_Click(object sender, EventArgs e)
        {
            if (btList != null)
            {
                foreach (Bitmap tmp in btList)
                {
                    tmp.Dispose();
                }
            }
            //BorderdataGrid.Rows.Clear();
            //borderRegion.Clear();
            //showborder.Clear();
            FeatherdataGrid.Rows.Clear();
            SetMod.Enabled = false;  //将后续四部的按钮的使能设置为false
            CutNumberBmp.Enabled = false;
            MakeMod.Enabled = false;
            AddBorder.Enabled = false;
            picturebox.Parent = null; ;
            isDesignBorder = false;   //当用户放弃操作，从头开始设计时，需要置位初始化一系列变量
            isDesignFeather = false;

            foreach (Button tmp in Designtools.Controls) //设计框内的所有按钮初始化为disable状态
            {
                tmp.Enabled = false;
            }
            CurrentStep = 0;        //当前处于的设计步骤
            PicFromCamera.Enabled = true;    //当前步骤设计需要的按钮使能设置为true
            PicFromLocality.Enabled = true;
            BasegroupBox.Controls.Clear();   //清空groupbox
        }

        private void NextStep_Click(object sender, EventArgs e)
        {
            switch (CurrentStep)//下一步按钮根据
            {
                case 0:   
                    {
                        PicFromCamera.Enabled = false;
                        PicFromLocality.Enabled = false;   //将本步骤的按钮使能设置为false
                        NextStep.Enabled = false;         //将NextStep按钮使能设置为false
                        SetMod.Enabled = true;            //将设置模板按钮使能设置为true
                        break;
                    }
                case 1:
                    {
                        NextStep.Enabled = false;         
                        DrawTempleBorder.Enabled = false;
                        BasegroupBox.Controls.Clear();
                        CutNumberBmp.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        NextStep.Enabled = false;
                        PicFromLocality.Enabled = false;
                        PicFromCamera.Enabled = false;
                        isShowTemplateBorder = false;
                        BasegroupBox.Controls.Clear();
                        MakeMod.Enabled = true;

                        break;
                    }
                case 3:
                    {

                        NextStep.Enabled = false;
                        AddFeather.Enabled = false;
                        AutoFeather.Enabled = false;
                        isDesignFeather = false;
                        isaddFeather = false;
                        UpdataTemplet();//将模板的数据保存到变量TempletFlag中
                        SaveTemplet();
                        picturebox.Size = bmp.Size;//将图像恢复到特征点设计之前
                        picturebox.Image =bmp ;
                        normalSize = bmp.Size;
                        RePlacebitmap(bmp);
                        DrawCurrentFeather = false;
                        AddBorder.Enabled = true;
       
                        break;
                    }
                case 4:
                    {
                        foreach (Button tmp in Designtools.Controls) //设计框内的所有按钮初始化为disable状态
                        {
                            tmp.Enabled = false;
                        }
                        isaddNumborder = false;
                        SaveToh.Enabled = true;
                        break;
                    }
            }
        }

        private void DrawTempleBorder_Click(object sender, EventArgs e)  //截图按钮
        {
            if (CurrentStep == 1)
            {
                isDrawTemplate = true;
            }
            else if (CurrentStep == 4)
            {
                isSreenShot = true;
            }
        }

        private void CutNumberBmp_Click(object sender, EventArgs e)   //点击进入截数字模板图步骤
        {
            SaveTemplet();
            isDesignFeather = false;
            isDesignBorder = false;

            MakeMod.Enabled = false;
            AddBorder.Enabled = false;
            foreach (Button tmp in Designtools.Controls) //设计框内的所有按钮初始化为disable状态
            {
                tmp.Enabled = false;
            }
            FeatherdataGrid.Rows.Clear();
            CurrentStep = 2;                 //当前处于第二步状态
            PicFromCamera.Enabled = true;
            PicFromLocality.Enabled = true;
            BasegroupBox.Controls.Clear();  //清空basegroupbox内控件
            InitializeCutMod(BasegroupBox); //初始化basegroupbox内用于增加一套模板的控件  
            DatagridviewgroupBox.Text = "Exist Bitmap";
            FeatherdataGrid.Rows.Clear();
            DirectoryInfo TheFolder = new DirectoryInfo(savePath + "\\NumberBmp" + "\\" + CurrentTempletName);
            FileInfo[] NextFile = TheFolder.GetFiles(); //遍历文件夹内文件
            int count = 0;
            for (int i = 0; i < NextFile.Length; i++)
            {
                if (StringOperateHelp.RightOf( NextFile[i].Name,'.')=="bmp")
                {
                    count++;
                    FeatherdataGrid.Rows.Add(count.ToString(),StringOperateHelp.LeftOf( NextFile[i].Name,'.'), null);
                }
            }
        }
        private System.Windows.Forms.TextBox CutBmptextBox1;
        private System.Windows.Forms.Label CutBmplabel1;
        private System.Windows.Forms.Button CutBmpbutton1;

        private void InitializeCutMod(GroupBox baseGroup)  //初始化截图模板工具框
        {
            CutBmptextBox1 = new TextBox();
            CutBmplabel1 = new Label();
            CutBmpbutton1 = new Button();

            baseGroup.Controls.Add(this.CutBmptextBox1);
            baseGroup.Controls.Add(this.CutBmplabel1);
            baseGroup.Controls.Add(this.CutBmpbutton1);

            baseGroup.Text = "Get Number";
            // 
            // CutBmptextBox1
            // 
            this.CutBmptextBox1.Location = new System.Drawing.Point(129, 63);
            this.CutBmptextBox1.Name = "CutBmptextBox1";
            this.CutBmptextBox1.Size = new System.Drawing.Size(51, 21);
            this.CutBmptextBox1.TabIndex = 24;
            // 
            // CutBmplabel1
            // 
            this.CutBmplabel1.AutoSize = true;
            this.CutBmplabel1.Location = new System.Drawing.Point(15, 66);
            this.CutBmplabel1.Name = "CutBmplabel1";
            this.CutBmplabel1.Size = new System.Drawing.Size(95, 12);
            this.CutBmplabel1.TabIndex = 23;
            this.CutBmplabel1.Text = "Sequence number";
            // 
            // CutBmpbutton1
            // 
            this.CutBmpbutton1.Location = new System.Drawing.Point(151, 143);
            this.CutBmpbutton1.Name = "CutBmpbutton1";
            this.CutBmpbutton1.Size = new System.Drawing.Size(42, 37);
            this.CutBmpbutton1.TabIndex = 22;
            this.CutBmpbutton1.Text = "OK";
            this.CutBmpbutton1.UseVisualStyleBackColor = true;
            try
            {
                this.CutBmpbutton1.Click -= new EventHandler(CutBmpbutton1_Click);
            }
            catch
            { }
            this.CutBmpbutton1.Click += new EventHandler(CutBmpbutton1_Click);
        }

        private void CutBmpbutton1_Click(object sender, EventArgs e)    //从大图上截取模板大小的图片并保存到本地
        {
            int i;
            int j;
            try
            {
                if (CutBmptextBox1.Text != "") //当截图名不为空时
                {
               
                    Bitmap temBmp = CutImage(bmp, templetFlag.templetPoint.X, templetFlag.templetPoint.Y, templetFlag.templetSize.Width, templetFlag.templetSize.Height);
                    ByteToImage bytetoimage = new ByteToImage();
                    byte[,] temData = new byte[temBmp.Height,temBmp.Width];
                    for (i = 0; i < temBmp.Height; i++)
                    {
                        for (j = 0; j < temBmp.Width; j++)
                        {
                            temData[i, j] = temBmp.GetPixel(j, i).R;
                        }
                    }
                    bytetoimage.FromGray(temData).Save(savePath + "\\NumberBmp\\" + CurrentTempletName + "\\" + CutBmptextBox1.Text + ".bmp", ImageFormat.Bmp);//24位
                    FeatherdataGrid.Rows.Clear();
                    DirectoryInfo TheFolder = new DirectoryInfo(savePath + "\\NumberBmp" + "\\" + CurrentTempletName);
                    FileInfo[] NextFile = TheFolder.GetFiles(); //遍历文件夹内文件 将存在的图片名添加到表格内
                    int count = 0;
                    for (i = 0; i < NextFile.Length; i++)
                    {
                        if (NextFile[i].Name != "templet.txt")
                        {
                            count++;
                            FeatherdataGrid.Rows.Add(count.ToString(), StringOperateHelp.LeftOf(NextFile[i].Name, '.'), null);
                        }
                    }
                 }
            }
            catch
            {
 
            }
            NextStep.Enabled = true;
        }

        private void RePlacebitmap(Bitmap bt)   //调整改变后的图片在panel上的位置
        {
            if (Mainpanel.Width > bt.Width && Mainpanel.Height > bt.Height)
            {
                picturebox.Location = new Point((Mainpanel.Width - bt.Size.Width) / 2, (Mainpanel.Height - bt.Size.Height) / 2);

            }
            else if (Mainpanel.Width > bt.Width)
            {
                picturebox.Parent = null;
                picturebox.Parent = Mainpanel;
                picturebox.Location = new Point((Mainpanel.Width - bt.Size.Width) / 2, 0);

            }
            else if (Mainpanel.Height > bt.Height)
            {
                picturebox.Parent = null;
                picturebox.Parent = Mainpanel;
                picturebox.Location = new Point(0, (Mainpanel.Height - bt.Size.Height) / 2);

            }
            else
            {
                picturebox.Parent = null;
                picturebox.Parent = Mainpanel;
                picturebox.Location = new Point(0, 0);
            }
        }

        private void AutoFeather_Click(object sender, EventArgs e)  //智能算法添加特征点
        {
            picturebox.Size = picturebox.Image.Size;
            SizeChangecount = 0;
            string exeSavePath = programPath + PublicVariable.NumberConfigExePath;
            //string exeSavePath = savePath + "\\NumberBmp\\" + CurrentTempletName.ToString() + "\\NumberConfig.exe";
            if (File.Exists(savePath + "\\NumberBmp\\" + CurrentTempletName.ToString() + "\\points.txt"))
                File.Delete(savePath + "\\NumberBmp\\" + CurrentTempletName.ToString() + "\\points.txt");
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(exeSavePath);
            info.UseShellExecute = false;
            info.WorkingDirectory = savePath+"\\NumberBmp\\" + CurrentTempletName.ToString();
            p = System.Diagnostics.Process.Start(info);
            while (!p.HasExited) //等待exe程序结束
            {
 
            }
            Process_exit(); //exe退出后，将exe生成的特征点文档读入
            picturebox.Invalidate();
        }

        private void Process_exit()
        {
            featherRegion.Clear();//清空所有特征点数据
            showfeature.Clear();
            FeatherdataGrid.Rows.Clear();
            for (int i = 0; i < NumberPoints*10; i++)   //初始化模板变量
            {
                templetFlag.feathers.featherPoints[i].featherNum = 0;
                templetFlag.feathers.featherPoints[i].rec = new Rectangle(0, 0, 0, 0);

            }
            if (File.Exists(savePath + "\\NumberBmp\\" + CurrentTempletName.ToString() + "\\points.txt")) //删除原先生成的特征点文件
            {
                try
                {
                    string[] allData = File.ReadAllLines(savePath + "\\NumberBmp\\" + CurrentTempletName.ToString() + "\\points.txt");
                    for (int i = 0; i < allData.Count(); i++)
                    {
                        string[] everyNumber = allData[i].Split(',');
                        for (int j = 0; j < everyNumber.Count() - 1; j += 3)
                        {
                            if (int.Parse(everyNumber[j]) != 0 && int.Parse(everyNumber[j+1]) != 0)
                            {
                                featherRgnstruct tmp = new featherRgnstruct();
                                tmp.rec = new Rectangle(int.Parse(everyNumber[j]) + i * picturebox.Size.Width / ExistNumber.Count, int.Parse(everyNumber[j + 1]), 3, 3);
                                tmp.featherNum = int.Parse(everyNumber[j + 2]);
                                featherRegion.Add(tmp);   //将文件内读入的特征点显示到Panel和表格上
                                showfeature.Add(tmp);
                                FeatherdataGrid.Rows.Add(int.Parse(everyNumber[j]) + i * picturebox.Size.Width / ExistNumber.Count, everyNumber[j + 1], everyNumber[j + 2]);
                            }
                        }

                    }
                    UpdataTemplet(); //将生成的特征点数据更新到TempletFlag变量
                }
                catch
                {
                    MessageBox.Show("自动获取失败");
                }
            }
        }

        private void Mainpanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
