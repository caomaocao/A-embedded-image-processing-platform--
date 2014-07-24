using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ControlTactic;
using System.Drawing.Design;

namespace ControlTactic.GraphicsControl
{
    public partial class featherFileIn : UserControl
    {
        public featherFileIn()
        {
            InitializeComponent();      //初始化控件
            ChangeControlSize();        //根据控件内容改变控件大小
            DrawModuleImage();      //绘制控件背景图片
            this.Resize += new EventHandler(InputValue_Resize);        //为控件加载控件大小变化事件处理函数
        }
        
        private Image image_Module = new Bitmap(1, 1);      //模块绘制背景图
        private Pen pen_Module = new Pen(Color.DimGray, 2);     //模块边框画笔
        private Pen pen_White = new Pen(Color.White, 2);        //模块边框画笔
        private Rectangle rectangle_Module = new Rectangle(0, 0, 1, 1);     //模块矩形框
        private Size originalSize = new Size(100, 30);     //控件初始大小

        //     public static string currentProjectPath = null;
        /// <summary>
        /// 数据类型枚举
        /// </summary>
        //   public enum Style { 整数, 浮点数, 位, 字节, 字, 双字, 字符串, 长字符串 };
        //     private Style dataStyle = Style.整数;
        //[Category("功能属性")]
        //[DisplayName("数据类型")]
        //public Style DataStyle
        //{
        //    get
        //    {
        //        return this.dataStyle;
        //    }
        //    set
        //    {
        //        this.dataStyle = value;
        //    }
        //}
        ///// <summary>
        ///// 模块名称
        ///// </summary>
        //private double constantValue = 0;
        //private Point valuePoint = new Point(0, 7);     //模块名称显示起始坐标
        //private Font valueFont = new Font("Tahoma", 10, FontStyle.Regular);       //模块名称显示字体
        //[Category("功能属性")]
        //[DisplayName("数值")]
        //[Browsable(false)]
        //public double DataValue
        //{
        //    get
        //    {
        //        return this.constantValue;
        //    }
        //    set
        //    {
        //        this.constantValue = value;
        //        ChangeControlSize();
        //        DrawModuleImage();
        //    }
        //}

        /// <summary>
        /// 模块名称 20131119
        /// </summary>
        private string moduleName = "featherIn";
        private Point textPoint = new Point(15, 8);     //模块名称显示起始坐标
        private Font textFont = new Font("Tahoma", 8, FontStyle.Regular);       //模块名称显示字体
        [Category("基本属性")]
        [DisplayName("模块名称")]
        [Browsable(false)]
        public string ModuleName
        {
            get
            {
                return this.moduleName;
                
            }
            set
            {
                this.moduleName = value;
                DrawModuleImage();
            }
        }
        /// <summary>
        /// 工程路径 20131121
        /// </summary>

        private string programpath = null;
        private string controlname = null;

        [Category("基本属性")]
        [DisplayName("输出图片保存地址")]
        [Browsable(false)]
        public string OutputImg
        {
            get
            {
                return programpath + "\\out\\" + serialNumber.ToString() + ".bmp";
            }

        }
        [Category("基本属性")]
        [DisplayName("工程路径")]
        [Browsable(false)]
        public string ProjectPath
        {
            set
            {
                programpath = value;
                featherFileEditor.projectPath = this.programpath;
            }
            get
            {
                return programpath;
            }


        }
        [Category("基本属性")]
        [DisplayName("控件名")]
        [Browsable(false)]
        public string Controlname
        {
            set
            {
                controlname = value;
            }
            get
            {
                return controlname;
            }
        }

        /// <summary>
        /// 输入的文件路径 20131118
        /// </summary>
        private string filePath = " ";
        [Category("功能属性")]
        [DisplayName("选择文件")]
        [Browsable(true)]
        [Editor(typeof(featherFileEditor), typeof(UITypeEditor))]
        public string FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
                //     DrawModuleImage();
            }
        }

        //[Category("基本属性")]
        //[Browsable(false)]
        //public string SaveProjectPath
        //{
        //    get
        //    {
        //        return this.projectPath;
        //    }
        //    set
        //    {
        //        this.projectPath = value;
        //    }
        //}

        [Category("基本属性")]
        [Browsable(false)]
        public string SaveFilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
            }
        }




        [Category("基本属性")]
        [DisplayName("模块序号")]
        public int ShowSerialNumber
        {
            get
            {
                return this.serialNumber;
            }
            set
            {
                ;
            }
        }

        private Point numPoint = new Point(90, 0); //显示序号坐标
        private bool showNumber = false;
        private Font numFont = new Font("Times New Roman", 8, FontStyle.Regular);    //模块序号显示字体
        [Category("基本属性")]
        [DisplayName("显示序号")]
        [Browsable(false)]
        public bool ShowNumber
        {
            get
            {
                numPoint = new Point(this.Width - this.serialNumber.ToString().Length * 7, 0);
                return this.showNumber;
            }
            set
            {
                this.showNumber = value;
                DrawModuleImage();
            }
        }

        [Category("基本属性")]
        [DisplayName("模块类别")]
        public string ShowModuleSort
        {
            get
            {
                return this.moduleSort;
            }
            set
            {
                ;
            }
        }

        private int serialNumber = -1;
        [Category("基本属性")]
        [Browsable(false)]
        public int SerialNumber
        {
            get
            {
                return this.serialNumber;
            }
            set
            {
                this.serialNumber = value;
            }
        }

        private string moduleSort = "输出变量";
        [Category("基本属性")]
        [Browsable(false)]
        public string ModuleSort
        {
            get
            {
                return this.moduleSort;
            }
            set
            {
                this.moduleSort = value;
            }
        }

        /// <summary>
        /// 鼠标点击端点的基本信息
        /// </summary>
        private string pointInformation = "Output,0";
        [Browsable(false)]
        public string PointInformation
        {
            get
            {
                return this.pointInformation;
            }
            set
            {
                this.pointInformation = value;
                if (value == "Output,0")
                {
                    this.outputChoosed--;
                    DrawModuleImage();
                }
            }
        }

        /// <summary>
        /// 控件是否出错
        /// </summary>
        private Color errorColor = Color.Red;
        private bool isError = false;
        [Browsable(false)]
        public bool IsError
        {
            get
            {
                return this.isError;
            }
            set
            {
                this.isError = value;
                if (value == true)
                {//出错则用错误选定颜色
                    this.brush_Module = new SolidBrush(this.errorColor);      //初始化模块颜色画笔

                }
                else
                {//反之则用模块颜色
                    this.brush_Module = new SolidBrush(this.moduleColor);
                }
                DrawModuleImage();      //重新绘制控件图片
            }
        }

        /// <summary>
        /// 控件缩放比例
        /// </summary>
        private float scaling = 1.0F;       //控件缩放比例
        private float originalScaling = 1.0F;       //记录控件上次缩放的比例
        [Browsable(false)]
        public float Scaling
        {
            get
            {
                return this.scaling;
            }
            set
            {
                if (value > 0 && value <= 2.0F)
                {
                    this.scaling = value;
                    this.Size = new Size(Convert.ToInt32(this.originalSize.Width * value),
                        Convert.ToInt32(this.originalSize.Height * value)); ;
                    this.Location = new Point(Convert.ToInt32(this.Location.X * value / this.originalScaling),
                        Convert.ToInt32(this.Location.Y * value / this.originalScaling));
                    this.originalScaling = value;
                    DrawModuleImage();
                }
            }
        }



        //[Category("基本属性")]
        //[Browsable(false)]
        //public double SaveDataValue
        //{
        //    get
        //    {
        //        return this.constantValue;
        //    }
        //    set
        //    {
        //        this.constantValue = value;
        //        DrawModuleImage();
        //    }
        //}

        /// <summary>
        /// 模块填充颜色
        /// </summary>
        private Color moduleColor = Color.White;
        private SolidBrush brush_Module = new SolidBrush(Color.White);      //模块填充画笔
        [Category("基本属性")]
        [DisplayName("模块颜色")]
        [Browsable(false)]
        public Color ModuleColor
        {
            get
            {
                return this.moduleColor;
            }
            set
            {
                this.moduleColor = value;
                this.brush_Module = new SolidBrush(value);      //初始化模块颜色画笔
                DrawModuleImage();      //重新绘制控件图片
            }
        }

        private Color color_Title = Color.Blue;     //模块名称文本颜色
        private SolidBrush brush_Title = new SolidBrush(Color.Blue);        //模块名称文本绘制画笔
        [Category("基本属性")]
        [DisplayName("文本颜色")]
        [Browsable(false)]
        public Color TextColor
        {
            get
            {
                return this.color_Title;
            }
            set
            {
                this.color_Title = value;
                this.brush_Title = new SolidBrush(value);
                DrawModuleImage();      //重新绘制控件图片
            }
        }

        [Category("基本属性")]
        [DisplayName("模块位置")]
        public Point ModuleLocation
        {
            get
            {
                return this.Location;
            }
            set
            {
                this.Location = value;
            }
        }

        /// <summary>
        /// 重载控件BackColor属性
        /// </summary>
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                DrawModuleImage();
            }
        }

        [Category("基本属性")]
        [DisplayName("背景颜色")]
        [Browsable(false)]
        public Color ModuleBackColor
        {
            get
            {
                return this.BackColor;
            }
            set
            {
                this.BackColor = value;
            }
        }

        [Category("基本属性")]
        [DisplayName("模块大小")]
        [Browsable(false)]
        public Size ModuleSize
        {
            get
            {
                return this.Size;
            }
            set
            {
                this.Size = value;
            }
        }

        /// <summary>
        /// 已选择的输出端口
        /// </summary>
        private short outputChoosed = 0;
        [Category("基本属性")]
        [Description("已选择的输出端口")]
        [Browsable(false)]
        public short OutputChoosed
        {
            get
            {
                return this.outputChoosed;
            }
            set
            {
                this.outputChoosed = value;
                DrawModuleImage();      //重新绘制控件图片
            }
        }

        /// <summary>
        /// 判断用户双击的点是否为模块的引脚
        /// </summary>
        private bool checkPoint = false;
        [Browsable(false)]
        public bool CheckPoint
        {
            get
            {
                return this.checkPoint;
            }
            set
            {
                this.checkPoint = value;
            }
        }

        /// <summary>
        /// 修正后的用户双击的点的坐标
        /// </summary>
        private Point modifiedPoint = new Point(0, 0);
        [Browsable(false)]
        public Point ModifiedPoint
        {
            get
            {
                return this.modifiedPoint;
            }
            set
            {
                this.modifiedPoint = value;
            }
        }

        /// <summary>
        /// 鼠标移动点坐标
        /// </summary>
        private Point mouseMovePoint = new Point(0, 0);
        [Browsable(false)]
        public Point MouseMovePoint
        {
            get
            {
                return this.mouseMovePoint;
            }
            set
            {
                this.mouseMovePoint = value;
                this.checkPoint = false;
                Point translatePoint = new Point(Convert.ToInt32((value.X - this.Location.X) / this.scaling),
                    Convert.ToInt32((value.Y - this.Location.Y) / this.scaling));
                if (translatePoint.X > this.originalSize.Width - 10
                    && translatePoint.X < this.originalSize.Width + 3)
                {
                    if (translatePoint.Y > 14 && translatePoint.Y < 26)
                    {
                        this.modifiedPoint = new Point(this.originalSize.Width - 3, 15);
                        this.checkPoint = true;
                        this.pointInformation = "Output,0";
                    }
                }
            }
        }

        /// <summary>
        /// 用户双击的点的坐标，判断该点是否在控件的引脚上
        /// </summary>
        private Point clickPoint = new Point(0, 0);
        [Browsable(false)]
        public Point ClickPoint
        {
            get
            {
                return this.clickPoint;
            }
            set
            {
                this.clickPoint = value;
                this.checkPoint = false;
                Point translatePoint = new Point(Convert.ToInt32((value.X - this.Location.X) / this.scaling),
                    Convert.ToInt32((value.Y - this.Location.Y) / this.scaling));
                if (translatePoint.X > this.originalSize.Width - 10
                    && translatePoint.X < this.originalSize.Width + 3)
                {
                    if (translatePoint.Y > 14 && translatePoint.Y < 26)
                    {
                        this.outputChoosed += 1;
                        this.modifiedPoint = new Point(this.originalSize.Width - 3, 15);
                        this.checkPoint = true;
                        this.pointInformation = "Output,0";
                        DrawModuleImage();
                    }
                }
            }
        }


        /// <summary>
        /// 控件大小变化处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void InputValue_Resize(object sender, EventArgs e)
        {
            this.Size = new Size(Convert.ToInt32(this.originalSize.Width * this.scaling),
                Convert.ToInt32(this.originalSize.Height * this.scaling));
        }

        /// <summary>
        /// 绘制控件背景图片
        /// </summary>
        private void DrawModuleImage()
        {
            Image image = new Bitmap(this.originalSize.Width, this.originalSize.Height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(this.BackColor);
            g.FillRectangle(this.brush_Module, this.rectangle_Module);
            g.DrawRectangle(this.pen_Module, this.rectangle_Module);
            g.DrawLine(this.pen_White, 10, 5, 10, 25);
            g.DrawLine(this.pen_White, 9, 5, this.originalSize.Width - 15, 5);

            SizeF size = g.MeasureString(this.moduleName, this.textFont);
            this.textPoint.X = Convert.ToInt32((90 - size.Width) / 2) + 5;

            g.DrawString(this.moduleName.ToString(), this.textFont, this.brush_Title, this.textPoint);
            if (this.showNumber)
            {
                g.DrawString(this.serialNumber.ToString(), this.numFont, this.brush_Title, this.numPoint);
            }

            g.DrawLine(this.pen_Module, this.originalSize.Width - 6, 15, this.originalSize.Width - 15, 15);

            if (this.outputChoosed > 0)
            {
                g.FillEllipse(Brushes.Black, this.originalSize.Width - 7, 12, 7, 7);
            }
            else
            {
                g.DrawEllipse(this.pen_Module, this.originalSize.Width - 6, 12, 5, 5);
            }
            g.Dispose();

            this.image_Module = new Bitmap(this.Width, this.Height);
            Graphics g_Image = Graphics.FromImage(this.image_Module);
            g_Image.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g_Image.DrawImage(image, this.ClientRectangle,
                new Rectangle(0, 0, this.originalSize.Width, this.originalSize.Height), GraphicsUnit.Pixel);
            g.Dispose();
            image.Dispose();
            this.BackgroundImage = this.image_Module;
        }

        /// <summary>
        /// 根据控件内容改变控件的大小
        /// </summary>
        private void ChangeControlSize()
        {
            this.Width = Convert.ToInt32(this.originalSize.Width * this.scaling);
            this.rectangle_Module = new Rectangle(10, 5, 75, 20);
        }
    }
    
}
