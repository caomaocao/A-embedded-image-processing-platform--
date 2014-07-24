using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ControlTactic
{
    public partial class Label : UserControl
    {
        private Image backgroundImage = null;     //控件背景图片
        private Font textFont = new Font("宋体", 10, FontStyle.Regular);     //文本字体
        private Size originalSize = new Size(80, 21);     //控件初始大小


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
                    DrawBackgroundImage();
                }
            }
        }

        /// <summary>
        /// 控件相关文本
        /// </summary>
        private string commentary = "注释内容";     //控件相关文本
        [Category("基本属性")]
        [DisplayName("注释内容")]
        [Description("注释内容")]
        public string Commentary
        {
            get
            {
                return this.commentary;
            }
            set
            {
                this.commentary = value;
                ChangeControlSize();
                DrawBackgroundImage();
            }
        }

        private Color textColor = Color.Black;      //文本颜色
        private Brush textBrush = new SolidBrush(Color.Black);     //文本画刷
        [Category("基本属性")]
        [DisplayName("文本颜色")]
        [Description("文本颜色")]
        public Color TextColor
        {
            get
            {
                return this.textColor;
            }
            set
            {
                this.textColor = value;
                this.textBrush = new SolidBrush(textColor);
                DrawBackgroundImage();
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
        [DisplayName("背景颜色")]
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
                DrawBackgroundImage();
            }
        }

        /// <summary>
        /// Label类构造函数
        /// </summary>
        public Label()
        {
            InitializeComponent();
            this.Resize += new EventHandler(Label_Resize);
            ChangeControlSize();
            DrawBackgroundImage();
        }

        /// <summary>
        /// 绘制文本图片
        /// </summary>
        private void DrawBackgroundImage()
        {
            Image image = new Bitmap(this.originalSize.Width, this.originalSize.Height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            SizeF size = g.MeasureString(this.commentary, this.textFont);
            g.Clear(this.BackColor);
            g.DrawString(this.commentary, this.textFont, this.textBrush,
                0, (this.originalSize.Height - size.Height + size.Height / 4) / 2);
            g.Dispose();

            this.backgroundImage = new Bitmap(this.Width, this.Height);
            Graphics g_backgroundImage = Graphics.FromImage(this.backgroundImage);
            g_backgroundImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g_backgroundImage.DrawImage(image, this.ClientRectangle,
                            new Rectangle(0, 0, this.originalSize.Width, this.originalSize.Height), GraphicsUnit.Pixel);
            g_backgroundImage.Dispose();
            image.Dispose();

            this.BackgroundImage = this.backgroundImage;
        }

        /// <summary>
        /// 根据控件内容改变控件的大小
        /// </summary>
        private void ChangeControlSize()
        {
            this.originalSize.Width = Convert.ToInt32(this.CreateGraphics().MeasureString(this.commentary, this.textFont).Width);
            this.Width = Convert.ToInt32(this.originalSize.Width * this.scaling);
        }

        /// <summary>
        /// 控件大小改变事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void Label_Resize(object sender, EventArgs e)
        {
            this.Size = new Size(Convert.ToInt32(this.originalSize.Width * this.scaling),
                Convert.ToInt32(this.originalSize.Height * this.scaling));
        }
    }
}