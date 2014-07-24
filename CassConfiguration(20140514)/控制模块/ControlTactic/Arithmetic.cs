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
        private Image image_Module = new Bitmap(1, 1);      //ģ����Ʊ���ͼ
        private Pen pen_Module = new Pen(Color.DimGray, 2);     //ģ��߿򻭱�
        private Pen pen_White = new Pen(Color.White, 2);        //ģ��߿򻭱�
        private Rectangle rectangle_Module = new Rectangle(0, 0, 1, 1);     //ģ����ο�
        private Size originalSize = new Size(110, 58);     //�ؼ���ʼ��С
                
        [Category("��������")]
        [DisplayName("ģ�����")]
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

        [Category("��������")]
        [DisplayName("ģ�����")]
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
        [Category("��������")]
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

        private string moduleSort = "ģ������";
        [Category("��������")]
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

        private Point numPoint = new Point(90, 0); //��ʾ�������
        private bool showNumber = false;
        private Font numFont = new Font("Times New Roman", 8, FontStyle.Regular);    //ģ�������ʾ����
        [Category("��������")]
        [DisplayName("��ʾ���")]
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
        /// ������˵�Ļ�����Ϣ
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
        /// �ؼ��Ƿ����
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
                {//�������ô���ѡ����ɫ
                    this.brush_Module = new SolidBrush(this.errorColor);      //��ʼ��ģ����ɫ����

                }
                else
                {//��֮����ģ����ɫ
                    this.brush_Module = new SolidBrush(this.moduleColor);
                }
                DrawModuleImage();      //���»��ƿؼ�ͼƬ
            }
        }

        /// <summary>
        /// �ؼ����ű���
        /// </summary>
        private float scaling = 1.0F;       //�ؼ����ű���
        private float originalScaling = 1.0F;       //��¼�ؼ��ϴ����ŵı���
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
        /// ģ������
        /// </summary>
        private string moduleTitle = "AI";
        private Font font_Port = new Font("Tahoma", 7, FontStyle.Regular);      //��������˿�������ʾ����
        private Point titlePoint = new Point(0, 5);     //ģ��������ʾ��ʼ����
        private Point titlePoint_1 = new Point(0, 5);
        private Font font_Title = new Font("Tahoma", 10, FontStyle.Regular);       //ģ��������ʾ����
        [Category("��������")]
        [DisplayName("ģ������")]
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
        [Category("��������")]
        [DisplayName("ģ�����")]
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
        /// ����˿�����
        /// </summary>
        private string[] inputportName = new string[1] { "RAW"};
        private string original_InputName = "RAW";
        [Category("��������")]
        [Browsable(false)]
        [DisplayName("����˿�")]
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
                ChangeControlSize();        //�����������ݵ����ؼ���С
                DrawModuleImage();      //���»��ƿؼ�ͼƬ
            }
        }

        /// <summary>
        /// ����˿�����
        /// </summary>
        private string[] outputportName = new string[1] { "PV" };
        private string original_OutputName = "PV";
        [Category("��������")]
        [Browsable(false)]
        [DisplayName("����˿�")]
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
                ChangeControlSize();        //�����������ݵ����ؼ���С
                DrawModuleImage();      //���»��ƿؼ�ͼƬ
            }
        }

        /// <summary>
        /// ģ�������ɫ
        /// </summary>
        private Color moduleColor = Color.White;
        private SolidBrush brush_Module = new SolidBrush(Color.White);      //ģ����仭��
        [Category("��������")]
        [Browsable(false)]
        [DisplayName("ģ����ɫ")]
        public Color ModuleColor
        {
            get
            {
                return this.moduleColor;
            }
            set
            {
                this.moduleColor = value;
                this.brush_Module = new SolidBrush(value);      //��ʼ��ģ����ɫ����
                DrawModuleImage();      //���»��ƿؼ�ͼƬ
            }
        }

        private Color color_Title = Color.Blue;     //ģ�������ı���ɫ
        private SolidBrush brush_Title = new SolidBrush(Color.Blue);        //ģ�������ı����ƻ���
        [Category("��������")]
        [Browsable(false)]
        [DisplayName("�ı���ɫ")]
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
                DrawModuleImage();      //���»��ƿؼ�ͼƬ
            }
        }

        [Category("��������")]
        [DisplayName("ģ��λ��")]
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

        [Category("��������")]
        [Browsable(false)]
        [DisplayName("ģ���С")]
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
        /// ���ؿؼ�BackColor����
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

        [Category("��������")]
        [DisplayName("������ɫ")]
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
        /// ��ѡ�������˿�
        /// </summary>
        private bool[] inputChoosed = new bool[10] { false, false, 
            false, false, false, false, false, false, false, false };
        [Category("��������")]
        [Description("��ѡ�������˿�")]
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
                DrawModuleImage();      //���»��ƿؼ�ͼƬ
            }
        }

        /// <summary>
        /// ��ѡ�������˿�
        /// </summary>
        private short[] outputChoosed = new short[10] { 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0 };
        [Category("��������")]
        [Description("��ѡ�������˿�")]
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
                DrawModuleImage();      //���»��ƿؼ�ͼƬ
            }
        }

        /// <summary>
        /// �ж��û�˫���ĵ��Ƿ�Ϊģ�������
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
        /// ��������û�˫���ĵ������
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
        /// ����ƶ�������
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
        /// �û�˫���ĵ�����꣬�жϸõ��Ƿ��ڿؼ���������
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
        /// ���캯��
        /// </summary>
        public Arithmetic()
        {
            InitializeComponent();      //��ʼ���ؼ�
            ChangeControlSize();        //���ݿؼ����ݸı�ؼ���С
            DrawModuleImage();      //���ƿؼ�����ͼƬ
            this.Resize += new EventHandler(Arithmetic_Resize);        //Ϊ�ؼ����ؿؼ���С�仯�¼�������
        }

        /// <summary>
        /// �ؼ���С�仯������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void Arithmetic_Resize(object sender, EventArgs e)
        {
            this.Size = new Size(Convert.ToInt32(this.originalSize.Width * this.scaling),
                Convert.ToInt32(this.originalSize.Height * this.scaling));
        }

        /// <summary>
        /// ���ƿؼ�����ͼƬ
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
        /// ���ݿؼ����ݸı�ؼ��Ĵ�С
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