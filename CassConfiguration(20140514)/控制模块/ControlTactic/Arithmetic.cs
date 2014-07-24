using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ControlTactic
{
    public partial class Arithmetic : UserControl
    {
        private Image image_Module = new Bitmap(1, 1);      //模块绘制背景图
        private Pen pen_Module = new Pen(Color.DimGray, 2);     //模块边框画笔
        private Pen pen_White = new Pen(Color.White, 2);        //模块边框画笔
        private Rectangle rectangle_Module = new Rectangle(0, 0, 1, 1);     //模块矩形框
        private Size originalSize = new Size(110, 58);     //控件初始大小
                
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

        private string moduleSort = "模拟输入";
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

        /// <summary>
        /// 鼠标点击端点的基本信息
        /// </summary>
        private string pointInformation = "Input,0";
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
                string[] information = value.Split(',');
                if (information.Length == 2)
                {
                    int temp = Convert.ToInt32(information[1]);
                    if (information[0] == "Input")
                    {
                        this.inputChoosed[temp] = false;
                    }
                    if (information[0] == "Output")
                    {
                        this.outputChoosed[temp]--;
                    }
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

        /// <summary>
        /// 模块名称
        /// </summary>
        private string moduleTitle = "AI";
        private Font font_Port = new Font("Tahoma", 7, FontStyle.Regular);      //输入输出端口名称显示字体
        private Point titlePoint = new Point(0, 5);     //模块名称显示起始坐标
        private Point titlePoint_1 = new Point(0, 5);
        private Font font_Title = new Font("Tahoma", 10, FontStyle.Regular);       //模块名称显示字体
        [Category("基本属性")]
        [DisplayName("模块名称")]
        [Browsable(false)]
        public string ModuleName
        {
            get
            {
                return this.moduleTitle;
            }
            set
            {
                this.moduleTitle = value;
                DrawModuleImage();
            }
        }

        private string portName = "AI0";
        [Category("基本属性")]
        [DisplayName("模块点名")]
        [Browsable(false)]
        public string PortName
        {
            get
            {
                return this.portName;
            }
            set
            {
                if (value != null && value.Trim() != "")
                {
                    //if (value.ToString().ToUpper().Contains(this.moduleTitle.ToUpper()))
                    //{
                        this.portName = value;
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        this.portName = this.moduleTitle + Convert.ToInt32(value).ToString();
                    //    }
                    //    catch
                    //    { }
                    //}
                    DrawModuleImage();
                }
            }
        }

        /// <summary>
        /// 输入端口名称
        /// </summary>
        private string[] inputportName = new string[1] { "RAW"};
        private string original_InputName = "RAW";
        [Category("基本属性")]
        [Browsable(false)]
        [DisplayName("输入端口")]
        public string InputName
        {
            get
            {
                return this.original_InputName;
            }
            set
            {
                if (value != "NULL")
                {
                    this.original_InputName = value;
                    this.inputportName = value.Split(',');
                }
                else
                {
                    this.original_InputName = "";
                    this.inputportName = new string[0] { };
                }
                ChangeControlSize();        //根据输入内容调整控件大小
                DrawModuleImage();      //重新绘制控件图片
            }
        }

        /// <summary>
        /// 输出端口名称
        /// </summary>
        private string[] outputportName = new string[1] { "PV" };
        private string original_OutputName = "PV";
        [Category("基本属性")]
        [Browsable(false)]
        [DisplayName("输出端口")]
        public string OutputName
        {
            get
            {
                return this.original_OutputName;
            }
            set
            {
                if (value != "NULL")
                {
                    this.original_OutputName = value;
                    this.outputportName = value.Split(',');
                }
                else
                {
                    this.original_OutputName = "";
                    this.outputportName = new string[0] { };
                }
                ChangeControlSize();        //根据输入内容调整控件大小
                DrawModuleImage();      //重新绘制控件图片
            }
        }

        /// <summary>
        /// 模块填充颜色
        /// </summary>
        private Color moduleColor = Color.White;
        private SolidBrush brush_Module = new SolidBrush(Color.White);      //模块填充画笔
        [Category("基本属性")]
        [Browsable(false)]
        [DisplayName("模块颜色")]
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
        [Browsable(false)]
        [DisplayName("文本颜色")]
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

        [Category("基本属性")]
        [Browsable(false)]
        [DisplayName("模块大小")]
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

        /// <summary>
        /// 已选择的输入端口
        /// </summary>
        private bool[] inputChoosed = new bool[10] { false, false, 
            false, false, false, false, false, false, false, false };
        [Category("基本属性")]
        [Description("已选择的输入端口")]
        [Browsable(false)]
        public bool[] InputChoosed
        {
            get
            {
                return this.inputChoosed;
            }
            set
            {
                this.inputChoosed = value;
                DrawModuleImage();      //重新绘制控件图片
            }
        }

        /// <summary>
        /// 已选择的输出端口
        /// </summary>
        private short[] outputChoosed = new short[10] { 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0 };
        [Category("基本属性")]
        [Description("已选择的输出端口")]
        [Browsable(false)]
        public short[] OutputChoosed
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
                if (translatePoint.X > -4 && translatePoint.X < 10)
                {
                    for (int i = 0; i < this.inputportName.Length; i++)
                    {
                        if (translatePoint.Y > 24 + 15 * i && translatePoint.Y < 38 + 15 * i && this.inputChoosed[i] == false)
                        {
                            this.checkPoint = true;
                            this.modifiedPoint = new Point(3, 31 + 15 * i);
                            this.pointInformation = "Input," + i.ToString();
                        }
                    }
                }
                if (translatePoint.X > 99 && translatePoint.X < this.originalSize.Width + 3)
                {
                    for (int i = 0; i < this.outputportName.Length; i++)
                    {
                        if (translatePoint.Y > 24 + 15 * i && translatePoint.Y < 38 + 25 * i)
                        {
                            this.modifiedPoint = new Point(this.originalSize.Width - 3, 31 + 15 * i);
                            this.checkPoint = true;
                            this.pointInformation = "Output," + i.ToString();
                        }
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
                if (translatePoint.X > -4 && translatePoint.X < 10)
                {
                    for (int i = 0; i < this.inputportName.Length; i++)
                    {
                        if (translatePoint.Y > 24 + 15 * i && translatePoint.Y < 38 + 15 * i && this.inputChoosed[i] == false)
                        {
                            this.inputChoosed[i] = true;
                            this.checkPoint = true;
                            this.modifiedPoint = new Point(3, 31 + 15 * i);
                            this.pointInformation = "Input," + i.ToString();
                            DrawModuleImage();
                        }
                    }
                }
                if (translatePoint.X > 99 && translatePoint.X < this.originalSize.Width + 3)
                {
                    for (int i = 0; i < this.outputportName.Length; i++)
                    {
                        if (translatePoint.Y > 24 + 15 * i && translatePoint.Y < 38 + 15 * i)
                        {
                            this.outputChoosed[i] += 1;
                            this.modifiedPoint = new Point(this.originalSize.Width - 3, 31 + 15 * i);
                            this.checkPoint = true;
                            this.pointInformation = "Output," + i.ToString();
                            DrawModuleImage();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Arithmetic()
        {
            InitializeComponent();      //初始化控件
            ChangeControlSize();        //根据控件内容改变控件大小
            DrawModuleImage();      //绘制控件背景图片
            this.Resize += new EventHandler(Arithmetic_Resize);        //为控件加载控件大小变化事件处理函数
        }

        /// <summary>
        /// 控件大小变化处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void Arithmetic_Resize(object sender, EventArgs e)
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
            g.DrawLine(this.pen_White, 15, 5, 15, this.originalSize.Height - 5);
            g.DrawLine(this.pen_White, 14, 5, 95, 5);
            g.DrawLine(Pens.White, 15, 22, 95, 22);
            g.DrawLine(Pens.DimGray, 15, 21, 95, 21);

            SizeF size = g.MeasureString(this.portName, this.font_Title);
            this.titlePoint.X = Convert.ToInt32((this.originalSize.Width - size.Width) / 2);
            g.DrawString(this.portName, this.font_Title, Brushes.Black, this.titlePoint);

            g.DrawLine(Pens.White, 15, this.originalSize.Height - 19, 95, this.originalSize.Height - 19);
            g.DrawLine(Pens.DimGray, 15, this.originalSize.Height - 20, 95, this.originalSize.Height - 20);

            size = g.MeasureString(this.moduleTitle, this.font_Title);
            this.titlePoint_1.X = Convert.ToInt32((this.originalSize.Width - size.Width) / 2);
            this.titlePoint_1.Y = this.originalSize.Height - 20;
            g.DrawString(this.moduleTitle, this.font_Title, this.brush_Title, this.titlePoint_1);
            if (this.showNumber)
            {
                g.DrawString(this.serialNumber.ToString(), this.numFont, this.brush_Title, this.numPoint);
            }

            for (int i = 0; i < this.inputportName.Length; i++)
            {
                g.DrawString(this.inputportName[i], this.font_Port, Brushes.Black, 15, 26 + 15 * i);

                g.DrawLine(this.pen_Module, 5, 31 + 15 * i, 14, 31 + 15 * i);

                if (this.inputChoosed[i] == true)
                {
                    g.FillEllipse(Brushes.Black, -1, 27 + 15 * i, 7, 7);
                }
                else
                {
                    g.DrawEllipse(this.pen_Module, 0, 28 + 15 * i, 5, 5);
                }
            }

            for (int i = 0; i < this.outputportName.Length; i++)
            {
                size = g.MeasureString(this.outputportName[i], this.font_Port);
                g.DrawString(this.outputportName[i], this.font_Port,
                    Brushes.Black, 94 - size.Width, 26 + 15 * i);

                g.DrawLine(this.pen_Module, 104, 31 + 15 * i, 95, 31 + 15 * i);

                if (this.outputChoosed[i] > 0)
                {
                    g.FillEllipse(Brushes.Black, 103, 27 + 15 * i, 7, 7);
                }
                else
                {
                    g.DrawEllipse(this.pen_Module, 104, 28 + 15 * i, 5, 5);
                }
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
            if (this.inputportName.Length > this.outputportName.Length)
            {
                this.originalSize.Height = 43 + 15 * this.inputportName.Length;
                this.Height = Convert.ToInt32(this.originalSize.Height * this.scaling);
            }
            else
            {
                this.originalSize.Height = 43 + 15 * this.outputportName.Length;
                this.Height = Convert.ToInt32(this.originalSize.Height * this.scaling);
            }
            this.rectangle_Module = new Rectangle(15, 5, 80, this.originalSize.Height - 10);
        }
    }
}