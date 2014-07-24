using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ControlTactic
{
    public partial class JMP : UserControl
    {
        private Image image_Module = new Bitmap(1, 1);      //ģ����Ʊ���ͼ
        private Pen pen_Module = new Pen(Color.DimGray, 2);     //ģ��߿򻭱�
        private Pen pen_White = new Pen(Color.White, 2);        //ģ��߿򻭱�
        private Rectangle rectangle_Module = new Rectangle(0, 0, 1, 1);     //ģ����ο�
        private Size originalSize = new Size(100, 30);     //�ؼ���ʼ��С

        /// <summary>
        /// ģ���������
        /// </summary>
        private List<string> serialNumberNames = new List<string>();
        [Category("��������")]
        [Browsable(false)]
        public List<string> SerialNumberNames
        {
            get
            {
                return this.serialNumberNames;
            }
            set
            {
                this.serialNumberNames = value;
                ChooseSerialNumber.SerialNumberName = value;
            }
        }   

        private string chooseSerialNumber = "";
        [Category("��������")]
        [DisplayName("����תĿ��")]
        [EditorAttribute(typeof(ChooseSerialNumber), typeof(System.Drawing.Design.UITypeEditor))]
        public string ChoosePortName
        {
            get
            {
                return this.chooseSerialNumber;
            }
            set
            {
                this.chooseSerialNumber = value;
            }
        }

        [Category("��������")]
        [Browsable(false)]
        public string ShowChooseSerialNumber
        {
            get
            {
                return this.chooseSerialNumber;
            }
            set
            {
                this.chooseSerialNumber = value;
            }
        }

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

        private string moduleSort = "�������";
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
                if (value == "Input,0")
                {
                    this.inputChoosed = false;
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
        private string moduleName = "MEMOUT";
        private Point textPoint = new Point(22, 7);     //ģ��������ʾ��ʼ����
        private Font textFont = new Font("Tahoma", 10, FontStyle.Regular);       //ģ��������ʾ����
        [Category("��������")]
        [DisplayName("ģ������")]
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
        /// ģ�������ɫ
        /// </summary>
        private Color moduleColor = Color.White;
        private SolidBrush brush_Module = new SolidBrush(Color.White);      //ģ����仭��
        [Category("��������")]
        [DisplayName("ģ����ɫ")]
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
                this.brush_Module = new SolidBrush(value);      //��ʼ��ģ����ɫ����
                DrawModuleImage();      //���»��ƿؼ�ͼƬ
            }
        }

        private Color color_Title = Color.Blue;     //ģ�������ı���ɫ
        private SolidBrush brush_Title = new SolidBrush(Color.Blue);        //ģ�������ı����ƻ���
        [Category("��������")]
        [DisplayName("�ı���ɫ")]
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
                DrawModuleImage();      //���»��ƿؼ�ͼƬ
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
        [DisplayName("ģ���С")]
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
        private bool inputChoosed = false;
        [Category("��������")]
        [Description("��ѡ�������˿�")]
        [Browsable(false)]
        public bool InputChoosed
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
                if (translatePoint.X > -3 && translatePoint.X < 10 && this.inputChoosed == false)
                {
                    if (translatePoint.Y > 9 && translatePoint.Y < 21)
                    {
                        this.modifiedPoint = new Point(3, 15);
                        this.checkPoint = true;
                        this.pointInformation = "Input,0";
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
                if (translatePoint.X > -3 && translatePoint.X < 10)
                {
                    if (translatePoint.Y > 9 && translatePoint.Y < 21 && this.inputChoosed == false)
                    {
                        this.inputChoosed = true;
                        this.modifiedPoint = new Point(3, 15);
                        this.checkPoint = true;
                        this.pointInformation = "Input,0";
                        DrawModuleImage();
                    }
                }
            }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        public JMP()
        {
            InitializeComponent();      //��ʼ���ؼ�
            ChangeControlSize();        //���ݿؼ����ݸı�ؼ���С
            DrawModuleImage();      //���ƿؼ�����ͼƬ
            this.Resize += new EventHandler(OutputValue_Resize);        //Ϊ�ؼ����ؿؼ���С�仯�¼���������
        }

        /// <summary>
        /// �ؼ���С�仯��������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void OutputValue_Resize(object sender, EventArgs e)
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
            g.DrawLine(this.pen_White, 15, 5, 15, 25);
            g.DrawLine(this.pen_White, 14, 5, this.originalSize.Width - 10, 5);

            SizeF size = g.MeasureString(this.moduleName, this.textFont);
            this.textPoint.X = Convert.ToInt32((80 - size.Width) / 2) + 10;

            g.DrawString(this.moduleName.ToString(), this.textFont, this.brush_Title, this.textPoint);
            if (this.showNumber)
            {
                g.DrawString(this.serialNumber.ToString(), this.numFont, this.brush_Title, this.numPoint);
            }
            g.DrawLine(this.pen_Module, 14, 15, 6, 15);

            if (this.inputChoosed == true)
            {
                g.FillEllipse(Brushes.Black, -1, 12, 7, 7);
            }
            else
            {
                g.DrawEllipse(this.pen_Module, 0, 12, 5, 5);
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
            this.Width = Convert.ToInt32(this.originalSize.Width * this.scaling);
            this.rectangle_Module = new Rectangle(15, 5, 75, 20);
        }
    }
}