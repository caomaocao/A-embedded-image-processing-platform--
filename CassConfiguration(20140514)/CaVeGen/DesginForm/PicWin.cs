using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using CaVeGen.Debug;
namespace  CaVeGen.DesignViewFiles
{
    public partial class PicWin : UserControl
    {
        //private int count_Pass = 0;  //pass个数
        //private int count_Fail = 0;  //fail个数
     //   private bool isStepDebug = false; //标记单步调试，防止对同一个控件累加计数
        private string curInputIndex = null;
        private ResultHelper resHelper;
     
        public PicWin()
        {
            InitializeComponent();
        }

        //当前输入图片编号
        public string InputPicIndex
        {
            get
            {
                return this.curInputIndex;
            }
            set
            {
                this.curInputIndex = value;
            }
        }
      
 
        #region  匹配结果参数 （pass/fail个数， 匹配率）

        /// <summary>
        /// 参数初始化
        /// </summary>
        public void Init()
        {
           // this.ClearCounter();
            this.ClearInspectionStatus();
          //  this.SetDebugStatus("null", false);
           // this.isStepDebug = false;//待定
        }
        ////设置pass个数
        //private void setPassCount()
        //{
        //    this.rs_pass.Text = this.count_Pass.ToString();
        //}
        ////设置fail个数
        //private void setFailCount()
        //{
        //    this.rs_fail.Text = this.count_Fail.ToString();
        //}
        
        ///// <summary>
        ///// 累加计数器
        ///// </summary>
        ///// <param name="type">pass or fail</param>
        //private void AccCounter(string type,bool flag)
        //{
        //    switch (type)
        //    {
        //        case "pass":
        //            if (flag)
        //                this.count_Pass++;
        //            else
        //                this.count_Pass = 1;
        //            break;
        //        case "fail":
        //            if (flag)
        //                this.count_Fail++;
        //            else
        //                this.count_Fail = 1;
        //            break;
        //    }
       
        //}
        ///// <summary>
        ///// 设置匹配率，外部接口
        ///// </summary>
        ///// <param name="percent"></param>
        //public void setMatch(int percent)
        //{
        //    this.rs_match.Text = percent.ToString() + "%";   
        // }
        ///// <summary>
        ///// 显示计数器结果，外部接口
        ///// </summary>
        //public void ShowCounter()
        //{
        //    this.setPassCount();
        //    this.setFailCount();
        //}

        ///// <summary>
        ///// 清空描述状态
        ///// </summary>
        public void ClearInspectionStatus()
        {
            //this.rs_pass.Text = "";
            //this.rs_fail.Text = "";
            //this.rs_match.Text = "";
            //  this.statusPic

            this.statusPic.Visible = false;
            this.tBox_Result.Text = "";
          

        }
        ///// <summary>
        ///// 清空计数器
        ///// </summary>
        //public void ClearCounter()
        //{
        //    this.count_Pass = 0;
        //    this.count_Fail = 0;
        //}

        #endregion

        #region 调试 2014.1.11
        /// <summary>
        /// 根据PictureBox类型，对相应的Picbox填充图片
        /// </summary>
        /// <param name="type">input/output</param>
        /// <param name="filePath"></param>
        //public bool SetPicBox(string type, string filePath)
        //{
        //    if (filePath == null && type == "output")  //单步调试初始化/停止调试
        //    {
        //        this.OutputPic.Image = null;
        //        this.OutputPic.Show();
        //    //    this.SetDebugStatus("null", false);
        //        return true;
        //    }
        //    switch (type) //图片填充并显示
        //    {
        //        case"input":
        //            this.InputPic.Image = OpenPicture("input", filePath);
        //            this.InputPic.Show();
        //            break;
        //        case "output":
        //            this.OutputPic.Image = OpenPicture("output", filePath);
        //            this.OutputPic.Show();
        //            break;
        //    }
        //    return true;
        //}


        /// <summary>
        /// 设置输入图片
        /// </summary>
        /// <param name="filePath"></param>
        public void SetInputPic(string filePath)
        {
            this.InputPic.Image = OpenPicture("input", filePath);
            this.InputPic.Show();
        }

        /// <summary>
        /// 设置输出图片
        /// </summary>
        /// <param name="filePath"></param>
        public void SetOutputPic(string filePath)
        {
            if (filePath == null)
            {
                this.OutputPic.Image = null;
                this.OutputPic.Show();
              //  this.SetDebugStatus("null", false);
                return ;
            }
            this.OutputPic.Image = OpenPicture("output", filePath);
            this.OutputPic.Show();
        }

        public void SetResultInspection(DataType type)
        {

        }
        /// <summary>
        /// 释放picbox的图片资源
        /// </summary>
        public void FreePicBox()
        {
            if (this.InputPic.Image != null)
            {
                this.InputPic.Image.Dispose();
            }
            if (this.OutputPic.Image != null)
            {
                this.OutputPic.Image.Dispose();

            }
            this.InputPic.Image = null;
            this.OutputPic.Image = null;

        }
        
        /// <summary>
        /// 打开图片
        /// </summary>
        /// <param name="type">string</param>
        /// <param name="filePath">string</param>
        /// <returns></returns>
        private Image OpenPicture(string type, string filePath)
        {
            
            try
            {
                  //打开图片
                Image myImage = null;
                //if (isAsync)//异步操作时，需要锁定图片
                //{   //此方法读取图片时，会锁定图片,导致调用exe时，打开初始图片失败
                //    myImage = Image.FromFile(filePath);
                //}
                //else
                //{   //此方法读取图片时，实现不锁定Image文件，到时可以让多用户同时访问Image文件。
                    FileStream fs = File.OpenRead(filePath); //OpenRead
                    int filelength = 0;
                    filelength = (int)fs.Length; //获得文件长度 
                    Byte[] image = new Byte[filelength]; //建立一个字节数组 
                    fs.Read(image, 0, filelength); //按字节流读取 
                    myImage = Image.FromStream(fs);
                    fs.Close();
              //  }
                //设置调试状态栏  pass
                // this.SetDebugStatus("pass");
                //返回图片
                return myImage;
            }
            catch (FileNotFoundException)
            {
                if (type == "input")
                {
                    MessageBox.Show(" Sorry,输入文件打开失败！\n不存在文件：" + filePath + "!   ");
                    //  this.SetDebugStatus("fail");
                    //return null;
                }
                //if (type == "output")
                //{
                    // MessageBox.Show("Sorry,输出文件打开失败！\n 不存在文件：" + filePath + "! ");
                    //   this.OutputPic.Image = null;
                    //  this.SetDebugStatus("fail");
                    //return null;
                //}
                return null;
            }
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.InnerException.StackTrace.ToString());
            //    return null;
            //}
            //finally
            //{
            //    
            //  //  return myImage;

            //}
            
        
        }

        /// <summary>
        /// 设置调试状态栏
        /// </summary>
        /// <param name="status">状态："null" or "pass" or "fail"</param>
        /// <param name="isLoop">bool</param>
        //public void SetDebugStatus(string status,bool isLoop)
        //{
        //    switch (status)
        //    {
        //        case"null":
        //            this.statusPic.Visible = false;
        //            break;
        //        case"pass":
        //             this.statusPic.Image = global::CaVeGen.Properties.Resources.pass;
        //            this.statusPic.Visible = true;
        //            this.AccCounter("pass", isLoop);
        //          //  this.isStepDebug = isStepDebug;
        //            break;
        //        case"fail":
        //            this.statusPic.Image = global::CaVeGen.Properties.Resources.fail;
        //            this.statusPic.Visible = true;
        //            this.AccCounter("fail", isLoop);
        //           // this.isStepDebug = isStepDebug;
        //            break;
        //    }
        //}
        #endregion



        /// <summary>
        /// 控件大小调整时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbxWin_Resize(object sender, EventArgs e)
        {
            //InputPic.Size = new Size((int)(this.Size.Width * 0.4), (int)(this.Size.Height - 25));
            //OutputPic.Location = new Point((int)(this.Size.Width * 0.6),24);
            //OutputPic.Size = new Size((int)(this.Size.Width * 0.4), (int)(this.Size.Height - 25));
            int width = this.Size.Width / 10;
            int pWidth = width * 5 / 2;    //图片picturebox的宽度
            int height = this.Size.Height / 6;
            int pHeight = height * 9 / 2;    //图片picturebox的高度
            //设置输入输出图片的大小
            InputPic.Size = new Size(pWidth, pHeight);
            OutputPic.Size = new Size(pWidth, pHeight);
            //设置输入输出图片在控件的位置
            InputPic.Location = new Point(width / 3 * 2, height);
            OutputPic.Location = new Point(width / 3 * 5 + pWidth, height);

            //设置输入输出标签位置
            label_Input.Location = new Point(width / 3 * 2, height / 3);
            label_Output.Location = new Point(width / 3 * 5 + pWidth, height / 3);

            //设置状态groupbox的位置和大小
            gBox_status.Location = new Point(width / 3 * 7 + pWidth * 2, height);
            gBox_status.Size = new Size(pWidth - 20, pHeight);
            gBox_status.BackColor = SystemColors.Control;
            //设置控件背景色
            this.BackColor = SystemColors.Control;


            //gBox_status中各控件的间距
            int gWidth = this.gBox_status.Width;
            int gHeight = (this.gBox_status.Height - this.lb_Result.Height - this.tBox_Result.Height - this.statusPic.Height) / 6;


            this.lb_Result.Location = new Point(10, gHeight * 2);
            this.tBox_Result.Location = new Point( 20,lb_Result.Height + gHeight * 2);
            this.statusPic.Location = new Point(gWidth / 3, gHeight * 4 + this.lb_Result.Height +  this.tBox_Result.Height);
            this.statusPic.Size = new Size(64, 32);

            //gBox_status中各控件的间距
            //int gWidth = this.gBox_status.Width;
            //int gHeight = (this.gBox_status.Height - this.lb_Pass.Height * 3 - statusPic.Height) / 13 * 2;

            ////pass count:xxx
            //lb_Pass.Location = new Point(5, gHeight * 2);
            //rs_pass.Location = new Point(this.lb_match.Width + 10, gHeight * 2);


            ////fail count : xxx
            //lb_Fail.Location = new Point(5, lb_Pass.Height + gHeight * 3);
            //rs_fail.Location = new Point(this.lb_match.Width + 10, lb_Pass.Height + gHeight * 3);


            ////match count :  xxx
            //lb_match.Location = new Point(5, lb_Pass.Height + lb_Fail.Height + gHeight * 4);
            //rs_match.Location = new Point(this.lb_match.Width + 10, lb_Pass.Height + lb_Fail.Height + gHeight * 4);


            ////设置groupbox中状态图片的位置 pass or fail
            //statusPic.Location = new Point(gWidth / 3, gHeight * 11 / 2 + this.rs_pass.Height * 3);
            //statusPic.Size = new Size(64, 32);

        }
        #region  双击inputPic事件

        //  bool Resizeflag = false;
        //  bool mouseDownflag = false;
        //PictureBox picturebox;
        //Form fm;

        //  Point startLocation;
        //private void pictureboxMouseDown(object sender, MouseEventArgs e)
        //{
        //    if (Resizeflag)
        //    {
        //        mouseDownflag = true;
        //        startLocation = new Point(e.X, e.Y);
        //        //MessageBox.Show(e.X.ToString() + "  " + e.Y.ToString());
        //    }

        //}
        //private void ResizePicture(object sender, MouseEventArgs e)
        //{
        //    Resizeflag = true;
        //}

        //private void InputPic_Click(object sender, EventArgs e)
        //{
        //    if (InputPic.Image == null)
        //        return;

        //    fm = new Form();
        //    fm.Size = new Size(500,500);


        //    //定义
        //    MenuStrip menuStrip = new MenuStrip();
        //    ToolStripMenuItem cutItem = new ToolStripMenuItem();
        //    //
        //    // 
        //    // menuStrip
        //    // 
        //    menuStrip.Items.AddRange(new ToolStripItem[] {
        //   cutItem});
        //    menuStrip.Location = new System.Drawing.Point(0, 0);
        //   menuStrip.Name = "menuStrip";
        //   menuStrip.Size = new System.Drawing.Size(498, 24);
        //    menuStrip.TabIndex = 0;
        //    menuStrip.Text = "menuStrip";
        //    // 
        //    // cutItem
        //    // 
        //   cutItem.Name = "cutItem";
        //    cutItem.Size = new System.Drawing.Size(65, 20);
        //    cutItem.Text = "裁剪图片";
        //    cutItem.MouseDown += new MouseEventHandler(ResizePicture);
        //    //
        //    //picturebox
        //    //          
        //    picturebox = new PictureBox();
        //    picturebox.Parent = fm;
        //    picturebox.Dock = DockStyle.Fill;
        //    picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
        //    picturebox.Image = InputPic.Image;
        //    picturebox.MouseDown += new MouseEventHandler(pictureboxMouseDown);
        //    picturebox.MouseMove += new MouseEventHandler(pictureboxMouseMove);
        //    picturebox.MouseUp += new MouseEventHandler(pictureboxMouseUp);

        //    fm.Controls.Add(menuStrip);
        //    fm.StartPosition = FormStartPosition.CenterScreen;
        //    fm.Show();
        //}
        //private void pictureboxMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (Resizeflag&&mouseDownflag)
        //    {
        //        Graphics g = picturebox.CreateGraphics();
        //        Rectangle rec = new Rectangle(startLocation, new Size(e.X-startLocation.X,e.Y-startLocation.Y));
        //        g.DrawRectangle(Pens.Black, rec);
        //        picturebox.Invalidate();
        //        //MessageBox.Show(e.X.ToString() + "  " + e.Y.ToString());
        //    }

        //}
        //Point endLocation;
        //private void pictureboxMouseUp(object sender, MouseEventArgs e)
        //{
        //    if (Resizeflag && mouseDownflag)
        //    {
        //        Resizeflag = false;
        //        mouseDownflag = false;
        //        endLocation = new Point(e.X, e.Y);
        //        DialogResult result = MessageBox.Show("是否保存?", " 提示", MessageBoxButtons.OKCancel);
        //        if (result.Equals(DialogResult.OK))
        //        {
        //            IntPtr handle = IntPtr.Zero;
        //            try
        //            {
        //                int width = (endLocation.X - startLocation.X) * picturebox.Image.Width / picturebox.Width;
        //                int height = (endLocation.Y - startLocation.Y) * picturebox.Image.Height / picturebox.Height;
        //                picturebox.Image = CutImage((Bitmap)(picturebox.Image), startLocation.X, startLocation.Y, width, height);
        //                MessageBox.Show("1");
        //                InputPic.Image = picturebox.Image;
        //                this.fm.Close();


        //            }
        //            catch
        //            {
        //            }

        //        }
        //        else if (result.Equals(DialogResult.Cancel))
        //        {
        //            MessageBox.Show("2");
        //        }

        //    }
        //   }
        //public static Bitmap CutImage(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        //{
        //    if (b == null)
        //    {
        //        return null;
        //    }

        //    int w = b.Width;
        //    int h = b.Height;

        //    if (StartX >= w || StartY >= h)
        //    {
        //        return null;
        //    }

        //    if (StartX + iWidth > w)
        //    {
        //        iWidth = w - StartX;
        //    }

        //    if (StartY + iHeight > h)
        //    {
        //        iHeight = h - StartY;
        //    }

        //    try
        //    {
        //        Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);

        //        Graphics g = Graphics.FromImage(bmpOut);
        //        g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
        //        g.Dispose();

        //        return bmpOut;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        #endregion

        /// <summary>
        /// 存在匹配结果
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="isLoopDebug"></param>
        public void DoMatchOperation(string savePath, bool isLoopDebug)
        {
            this.resHelper = new ResultHelper(savePath);
            //ResultInfo resultInfo = this.resHelper.ReadFromFile(isLoopDebug);
            //this.SetStatusPicture(resultInfo);
            //this.SetResult(resultInfo.Result);
            this.tBox_Result.Text = "";
            List<ResultInfo> resultList = this.resHelper.ReadFromFile2(isLoopDebug);
            foreach (ResultInfo temp in resultList)
            {
                this.SetStatusPicture(temp);
                this.SetResult(temp.Result);
            }

        }

        public void SetResult(string result)
        {
            this.tBox_Result.Text += result + " ";

        }

        public void HideResultStatus()
        {
            
        }
        private void SetStatusPicture(ResultInfo resultInfo)
        {
            switch (resultInfo.DataType)
            {
                case "BOOL":

                    if (resultInfo.Result == "0")
                        this.statusPic.Image = global::CaVeGen.Properties.Resources.fail;
                    else if (resultInfo.Result == "1")
                        this.statusPic.Image = global::CaVeGen.Properties.Resources.pass;
                    this.statusPic.Visible = true;
                    break;

                case "INT":
                    this.statusPic.Visible = false;
                    break;
                case "FLOAT":
                    this.statusPic.Visible = false;

                    break;
            }
        }
    }

}
