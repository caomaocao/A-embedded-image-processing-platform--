/*******************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����
           ** �ļ�����CassView.cs 
           ** ����������
           **          ������ʾ������ı�����������Ŀؼ�����ӵ��ÿؼ��������ڡ�
 *                     ��������ڸÿؼ��ϵ�λ����ʾ����ҳ���ϡ������˼�������ҳ�����ԡ�
           ** ���ߣ��ⵤ��
           ** ��ʼʱ�䣺2006-11-30
           ** 
********************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using CaVeGen.DesignViewFiles.FilterProperty;
using CaVeGen.CommonOperation;

namespace CaVeGen.DesignViewFiles
{
    public struct ControlInfo
    {
        public int ControlNum;

        public string SortName;
        public string ControlName;

        /// <summary>
        /// ������ģ��0λΪģ����,1λΪģ�������,2λΪģ����
        /// ���������ģ��1λΪĿ��ģ������2λΪģ������
        /// </summary>
        public string[] CodeInfo;

        public bool HasEnumProperty;
        public bool CanOptimize;
        public List<XProp> VisibleFunctionProperty;
        public List<XProp> UnvisibleFunctionProperty;

        public string[] OtherProperty;

        /// <summary>
        /// �ؼ��Ƿ�����
        /// </summary>
        public bool IsConnect;
        //ÿ�������������������Ϣ:0λΪ��Ӧ��ֱ����һ�˵Ŀؼ�;1λΪʹ�õ�����ʱ����
        public List<string[]> InputInfo;
        public List<string[]> OutputInfo;
    }

    public partial class CassView : UserControl
    {
        #region �������Լ���

        public List<ControlInfo> CopyCtrl = new List<ControlInfo>();//���ƵĿؼ���Ϣ������ճ�������
        public List<ControlInfo> ctrlsInfo = new List<ControlInfo>();
        public List<BlockInfo> blocksInfo = new List<BlockInfo>();
        public List<LineInfo> linesInfo = new List<LineInfo>();
        private int recordControls = 0;
        private Image backgroundImage = null;//ҳ�汳��ͼƬ
        private List<List<ArrayList>> InfosList = new List<List<ArrayList>>();
        private bool isScale = false;//��ʾ�������ſؼ�ʱ����¼�ؼ��ƶ�
        public  bool UndoFlag = false;//�ؼ����� �����ж�


        //ע��
        //����PortInfoList��CtrlsInfo�Ĺ�ϵ�ǵ�����ң��ؼ������е�CodeInfo�����Ϣ���Բ��ҵ���Ӧ��PortInfo�е���Ϣ
        //���Ƿ�����Һܷ��� ����PortInfoList�������� ����������һ��Cassview����

        /// <summary>
        /// ģ����������
        /// </summary>
        [Category("��������")]
        [Browsable(false)]
        public List<ArrayList> PortInfoList
        {
            get
            {
                return CassViewGenerator.PortInfoList;
            }
            set
            {
                CassViewGenerator.PortInfoList = value;
            }
        }

        /// <summary>
        /// �Ѽ�¼��ŵĿؼ���
        /// </summary>
        [Category("��������")]
        [Browsable(false)]
        public int RecordControls
        {
            get
            {
                return this.recordControls;
            }
            set
            {
                this.recordControls = value;
            }
        }

        /// <summary>
        /// ���ؿؼ�BackgroundImage����
        /// </summary>        
        [Browsable(false)]
        [Category("��������")]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Category("��������")]
        [DisplayName("ҳ���С")]
        public Size PageSize
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
        public Color PageBackColor
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

        [Category("��������")]
        [DisplayName("��Ϣ����")]
        public List<List<ArrayList>> infosList
        {
            get
            {
                if (this.ctrlsInfo.Count != 0 && this.blocksInfo.Count != 0
                    && this.ctrlsInfo.Count == this.recordControls)
                {//�������пؼ���Ϣ�ǿղŽ�����Ϣ����,ȫɾ���ؼ�ʱ�����bug20090618������ɾ���ؼ�ʱ
                    this.InfosList = SaveInfo();
                }
                return this.InfosList;
            }
            set
            {
                if (value.Count != 0)
                {
                    ReadInfo(value);
                }
                this.InfosList = value;
            }
        }

        #region ��Ϣת������

        /* ���ڴ洢�õ�List<ArrayList>��ʽ����Ϣ���ݺ�
         * Cassview��ʹ�õĿؼ���ϢctrlsInfo
         * ����ʹ�õĿؼ���������ϢblockInfo��linesInfo
         * ����֮���ת�� */

        /// <summary>
        /// ����Ϣ�����ж�ȡ��Ϣ
        /// </summary>
        /// <param name="infoArray"></param>
        private void ReadInfo(List<List<ArrayList>> infoArray)
        {
            List<ArrayList> ctrol = infoArray[0];
            List<ArrayList> line = infoArray[1];
            for (int i = 0; i < ctrol.Count; i++)
            {
                ArrayList temp = ctrol[i];
                ControlInfo tempCtrl = new ControlInfo();
                BlockInfo tempBlock = new BlockInfo();

                string[] BasicInfo = (string[])(temp[0]);
                Point[] SizeInfo = (Point[])(temp[3]);
                List<string[]> FuncInfo1 = (List<string[]>)(temp[1]);
                List<string[]> FuncInfo2 = (List<string[]>)(temp[2]);
                //��������
                tempCtrl.ControlNum = Convert.ToInt32(BasicInfo[0]);
                tempCtrl.ControlName = BasicInfo[1];
                tempBlock.BlockNum = tempCtrl.ControlNum;
                tempBlock.BlockName = tempCtrl.ControlName;
                tempCtrl.SortName = BasicInfo[2];
                if (BasicInfo[3] != null)
                {
                    tempCtrl.OtherProperty = BasicInfo[3].Split('$');
                }
                tempCtrl.HasEnumProperty = Convert.ToBoolean(BasicInfo[4]);
                tempCtrl.CodeInfo = new string[3];
                tempCtrl.CodeInfo[0] = BasicInfo[5];
                tempCtrl.CodeInfo[1] = BasicInfo[6];
                tempCtrl.CodeInfo[2] = BasicInfo[7];
                string[] LoadInfo = BasicInfo[8].Split(';');
                tempCtrl.CanOptimize = Convert.ToBoolean(BasicInfo[10]);

                if (LoadInfo[0] != "0")
                {//��������˿ڱ��� �˿ڳ�ֵ��Ϣ20090616
                    List<string[]> tempIOinfo = new List<string[]>();
                    foreach (string element in LoadInfo)
                    {
                        string[] info = new string[4];
                        info[2] = element.Split(',')[0];
                        info[3] = element.Split(',')[1];
                        tempIOinfo.Add(info);
                    }
                    tempCtrl.InputInfo = tempIOinfo;
                }
                else
                {
                    tempCtrl.InputInfo = InitializeIOinfo(Convert.ToInt32(BasicInfo[8]));
                }
                tempCtrl.OutputInfo = InitializeIOinfo(Convert.ToInt32(BasicInfo[9]));
                tempCtrl.VisibleFunctionProperty = new List<XProp>();
                tempCtrl.UnvisibleFunctionProperty = new List<XProp>();
                if (FuncInfo1 != null)
                {//�ɼ�����
                    for (int x = 0; x < FuncInfo1.Count; x++)
                    {
                        XProp attribute = new XProp();
                        attribute.EnumValue = FuncInfo1[x][0];
                        attribute.Name = FuncInfo1[x][1];
                        attribute.TheValue = FuncInfo1[x][2];
                        attribute.ValueType = FuncInfo1[x][3];
                        attribute.VarName = FuncInfo1[x][4];
                        attribute.ValueExplain = FuncInfo1[x][5];
                        attribute.Opnode = FuncInfo1[x][6];
                        attribute.Optype = FuncInfo1[x][7];
                        attribute.Opvalue = FuncInfo1[x][8];
                        attribute.Relate = FuncInfo1[x][9];
                        tempCtrl.VisibleFunctionProperty.Add(attribute);
                    }
                }
                if (FuncInfo2 != null)
                {//��������
                    for (int y = 0; y < FuncInfo2.Count; y++)
                    {
                        XProp attribute = new XProp();
                        attribute.EnumValue = FuncInfo2[y][0];
                        attribute.Name = FuncInfo2[y][1];
                        attribute.TheValue = FuncInfo2[y][2];
                        attribute.ValueType = FuncInfo2[y][3];
                        attribute.VarName = FuncInfo2[y][4];
                        attribute.ValueExplain = FuncInfo2[y][5];
                        attribute.Opnode = FuncInfo2[y][6];
                        attribute.Optype = FuncInfo2[y][7];
                        attribute.Opvalue = FuncInfo2[y][8];
                        attribute.Relate = FuncInfo2[y][9];
                        tempCtrl.UnvisibleFunctionProperty.Add(attribute);
                    }
                }
                //λ�á���С��Ϣ
                tempBlock.StartPoint = SizeInfo[0];
                tempBlock.Width = SizeInfo[1].X;
                tempBlock.Height = SizeInfo[1].Y;

                this.ctrlsInfo.Add(tempCtrl);
                this.blocksInfo.Add(tempBlock);
            }
            for (int i = 0; i < line.Count; i++)
            {//������Ϣ
                LineInfo tempLine = new LineInfo();

                Point[] SEinfo = (Point[])(line[i][0]);
                string[] SEctrl = (string[])(line[i][1]);
                tempLine.StartPoint = SEinfo[0];
                tempLine.EndPoint = SEinfo[1];
                tempLine.StartInfo = SEctrl[0];
                tempLine.EndInfo = SEctrl[1];

                this.linesInfo.Add(tempLine);
            }
            portReflash();//ˢ�¿ؼ��˿���Ϣ
            //���¼��������м��
            DrawLineArithmetic newArith = new DrawLineArithmetic(this.linesInfo, this.blocksInfo);
            this.linesInfo = newArith.ModifyLines;

        }

        /// <summary>
        /// �ѿؼ���Ϣ��������Ϣ��ֱ����Ϣ��������Ϣ������
        /// </summary>
        /// <returns></returns>
        private List<List<ArrayList>> SaveInfo()
        {
            List<List<ArrayList>> ReturnInfo = new List<List<ArrayList>>();
            List<ArrayList> FirstInfo = new List<ArrayList>();
            List<ArrayList> SecondInfo = new List<ArrayList>();
            //��һ������Ϣ
            for (int i = 0; i < this.ctrlsInfo.Count; i++)
            {
                ArrayList controlInfo = new ArrayList();
                //������Ϣ
                string[] BasicInfo = new string[11];
                BasicInfo[0] = this.ctrlsInfo[i].ControlNum.ToString();
                BasicInfo[1] = this.ctrlsInfo[i].ControlName;
                BasicInfo[2] = this.ctrlsInfo[i].SortName;
                if (this.ctrlsInfo[i].OtherProperty != null)
                {//����������ͼ�������̬
                    BasicInfo[3] = String.Join("$", this.ctrlsInfo[i].OtherProperty);
                }
                BasicInfo[4] = this.ctrlsInfo[i].HasEnumProperty.ToString();
                BasicInfo[5] = this.ctrlsInfo[i].CodeInfo[0];
                BasicInfo[6] = this.ctrlsInfo[i].CodeInfo[1];
                BasicInfo[7] = this.ctrlsInfo[i].CodeInfo[2];
                BasicInfo[10] = this.ctrlsInfo[i].CanOptimize.ToString();
                if (this.ctrlsInfo[i].InputInfo != null && this.ctrlsInfo[i].InputInfo.Count != 0)
                {
                    List<string> SaveInfo = new List<string>();
                    foreach (string[] element in this.ctrlsInfo[i].InputInfo)
                    {
                        if (element.Length == 4)
                        {
                            SaveInfo.Add(element[2] + "," + element[3]);
                        }
                        else
                        { SaveInfo.Add(",0"); }
                    }
                    BasicInfo[8] = String.Join(";", SaveInfo.ToArray());
                }
                else
                { BasicInfo[8] = "0"; }
                if (this.ctrlsInfo[i].OutputInfo != null)
                {
                    BasicInfo[9] = this.ctrlsInfo[i].OutputInfo.Count.ToString();
                }
                else
                { BasicInfo[9] = "0"; }
                controlInfo.Add(BasicInfo);

                //����������Ϣ
                if (this.ctrlsInfo[i].VisibleFunctionProperty != null)
                {//��������
                    List<string[]> FuncInfo1 = new List<string[]>();
                    foreach (XProp attribute in this.ctrlsInfo[i].VisibleFunctionProperty)
                    {
                        string[] tempA = new string[10];
                        tempA[0] = attribute.EnumValue;
                        tempA[1] = attribute.Name;
                        tempA[2] = attribute.TheValue.ToString();
                        tempA[3] = attribute.ValueType;
                        tempA[4] = attribute.VarName;
                        tempA[5] = attribute.ValueExplain;
                        tempA[6] = attribute.Opnode;
                        tempA[7] = attribute.Optype;
                        tempA[8] = attribute.Opvalue;
                        tempA[9] = attribute.Relate;
                        FuncInfo1.Add(tempA);
                    }
                    controlInfo.Add(FuncInfo1);
                }
                else
                { controlInfo.Add(null); }
                if (this.ctrlsInfo[i].UnvisibleFunctionProperty != null)
                {//��������
                    List<string[]> FuncInfo2 = new List<string[]>();
                    foreach (XProp attribute in this.ctrlsInfo[i].UnvisibleFunctionProperty)
                    {
                        string[] tempA = new string[10];
                        tempA[0] = attribute.EnumValue;
                        tempA[1] = attribute.Name;
                        tempA[2] = attribute.TheValue.ToString();
                        tempA[3] = attribute.ValueType;
                        tempA[4] = attribute.VarName;
                        tempA[5] = attribute.ValueExplain;
                        tempA[6] = attribute.Opnode;
                        tempA[7] = attribute.Optype;
                        tempA[8] = attribute.Opvalue;
                        tempA[9] = attribute.Relate;
                        FuncInfo2.Add(tempA);
                    }
                    controlInfo.Add(FuncInfo2);
                }
                else
                { controlInfo.Add(null); }
                //λ�ô�С��Ϣ
                Point[] SizeInfo = new Point[2];
                for (int j = 0; j < this.blocksInfo.Count; j++)
                {
                    if (this.blocksInfo[j].BlockNum == this.ctrlsInfo[i].ControlNum
                        && this.blocksInfo[j].BlockName == this.ctrlsInfo[i].ControlName)
                    {
                        SizeInfo[0] = this.blocksInfo[j].StartPoint;
                        SizeInfo[1] = new Point(this.blocksInfo[j].Width, this.blocksInfo[j].Height);
                        break;
                    }

                }
                controlInfo.Add(SizeInfo);
                FirstInfo.Add(controlInfo);
            }
            ReturnInfo.Add(FirstInfo);
            //������Ϣ
            for (int i = 0; i < this.linesInfo.Count; i++)
            {
                ArrayList lineinfo = new ArrayList();
                Point[] SEinfo = new Point[2];
                SEinfo[0] = this.linesInfo[i].StartPoint;
                SEinfo[1] = this.linesInfo[i].EndPoint;
                string[] SEctrl = new string[2];
                SEctrl[0] = this.linesInfo[i].StartInfo;
                SEctrl[1] = this.linesInfo[i].EndInfo;
                lineinfo.Add(SEinfo);
                lineinfo.Add(SEctrl);
                SecondInfo.Add(lineinfo);
            }

            ReturnInfo.Add(SecondInfo);
            return ReturnInfo;
        }
        #endregion

        #endregion

        /// <summary>
        /// CassView�๹�캯��
        /// </summary>
        public CassView()
        {
            InitializeComponent();
            this.ControlRemoved += new ControlEventHandler(CassView_ControlRemoved);
        }

        #region ֱ�߲���

        private bool isPortPoint = false;
        /// <summary>
        /// �ж����������Ƿ���ģ��������
        /// </summary>
        [Browsable(false)]
        public bool IsPortPoint
        {
            get
            {
                return this.isPortPoint;
            }
            set
            {
                this.isPortPoint = value;
                this.Invalidate();
            }
        }

        private int choosedLine = -1;
        /// <summary>
        /// ��¼�û���ǰѡ�������
        /// </summary>
        [Browsable(false)]
        public int ChoosedLine
        {
            get
            {
                return this.choosedLine;
            }
            set
            {
                this.choosedLine = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// �������ƹ��ĵ�
        /// </summary>
        [Browsable(false)]
        public Point ClearMouseMovePoint
        {
            get
            {
                return this.mouseMovePoint;
            }
            set
            {
                this.mouseMovePoint = value;
            }
        }

        /// <summary>
        /// ����ƹ��������
        /// </summary>
        private bool showMouseMovePoint = false;        //�Ƿ���ʾ����ƹ���ı�Ǳ���
        private Point mouseMoveShowPoint = new Point(0, 0);     //����ƹ�����ʾ������
        private Point mouseMovePoint = new Point(0, 0);     //����ƹ��������
        [Browsable(false)]
        public Point MouseMovePoint
        {
            get
            {
                return this.mouseMovePoint;
            }
            set
            {
                if (this.getFirstPoint == true)     //���֮ǰ��ѡ��ĳ���Ϸ�ģ������
                {
                    this.mouseMovePoint = this.PointToClient(value);        //�����λ��ת��Ϊ��������������
                    this.showMouseMovePoint = false;        //���ü�¼����Ƿ��ƶ���������һ�Ϸ��˵�ı�־Ϊfalse

                    foreach (Control obj in this.Controls)      //���α���ҳ������пؼ�
                    {
                        //����굱ǰ���괫�ݸ����ؼ�
                        PropertyDescriptor clickPointProperty = TypeDescriptor.GetProperties(obj)["MouseMovePoint"];
                        if (clickPointProperty != null)
                        {
                            clickPointProperty.SetValue(obj, this.mouseMovePoint);
                        }//end if (clickPointProperty != null)
                        else
                        {
                            continue;
                        }//end else if (clickPointProperty == null)

                        //�ж���굱ǰλ���Ƿ���ģ���������
                        PropertyDescriptor checkPointProperty = TypeDescriptor.GetProperties(obj)["CheckPoint"];
                        if (checkPointProperty != null)
                        {
                            if ((bool)checkPointProperty.GetValue(obj) == true)     //��ģ���������
                            {
                                PropertyDescriptor pointInformationProperty = TypeDescriptor.GetProperties(obj)["PointInformation"];
                                if (pointInformationProperty != null)
                                {
                                    string pointInformation = (string)(pointInformationProperty.GetValue(obj));

                                    if (pointInformation != null && pointInformation != ""
                                        && this.preClickInformation[0] != pointInformation[0] //���������
                                        && this.preClickControl != obj) //�ؼ�����ͬ
                                    {
                                        PropertyDescriptor modifiedPointProperty = TypeDescriptor.GetProperties(obj)["ModifiedPoint"];
                                        if (modifiedPointProperty != null)
                                        {
                                            Point modifiedPoint = (Point)(modifiedPointProperty.GetValue(obj));
                                            this.mouseMoveShowPoint = new Point(Convert.ToInt32(modifiedPoint.X
                                                * this.scaling + obj.Location.X),
                                                Convert.ToInt32(modifiedPoint.Y * this.scaling
                                                + obj.Location.Y));
                                            this.showMouseMovePoint = true;
                                        }//end if (modifiedPointProperty != null)
                                        break;
                                    }
                                }
                                //�ѵ�ǰ���λ�����������ŵ���ȷλ�ã����λ�ÿ�������ȷλ��������ƫ��

                            }//end if ((bool)checkPointProperty.GetValue(obj) == true)
                        }//end if (checkPointProperty != null)
                    }//end foreach (Control obj in this.Controls) 
                    this.Refresh();
                }//end if (this.getFirstPoint == true)
            }
        }

        private Point mouseUpPoint = new Point(0, 0);
        /// <summary>
        /// �û����˫��ʱ������
        /// </summary>
        [Browsable(false)]
        public Point MouseUpPoint
        {
            get
            {
                return new Point(-1, -1);
            }
            set
            {
                if (this.getFirstPoint == true)     //���֮ǰ��ѡ��ĳ���Ϸ�ģ������
                {
                    this.isPortPoint = false;
                    this.mouseUpPoint = this.PointToClient(value);      //ת�����λ��

                    foreach (Control obj in this.Controls)      //���α���ҳ�����
                    {
                        //����굯��ʱ������������ݸ���ģ��
                        PropertyDescriptor clickPointProperty = TypeDescriptor.GetProperties(obj)["ClickPoint"];
                        if (clickPointProperty != null)
                        {
                            clickPointProperty.SetValue(obj, this.mouseUpPoint);
                        }//end if(clickPointProperty != null)
                        else
                        {
                            continue;
                        }//end else if (clickPointProperty != null)

                        //�жϸ������Ƿ���ģ��ĺϷ�������
                        PropertyDescriptor checkPointProperty = TypeDescriptor.GetProperties(obj)["CheckPoint"];
                        if (checkPointProperty != null)
                        {
                            this.isPortPoint = (bool)checkPointProperty.GetValue(obj);
                            if (this.isPortPoint == true)       //����������
                            {
                                //��ȡ������Ϣ����������/������ţ�������ŵ�
                                PropertyDescriptor pointInformationProperty = TypeDescriptor.GetProperties(obj)["PointInformation"];
                                if (pointInformationProperty != null)
                                {
                                    string pointInformation = (string)(pointInformationProperty.GetValue(obj));
                                    if (pointInformation == null || pointInformation == "")     //��ȡ���Ƿ���Ϣ
                                    {
                                        if (this.choosedLine != -1)
                                        {
                                            this.choosedLine = -1;
                                        }//end if (this.choosedLine != -1)
                                        this.showMouseMovePoint = false;
                                        this.getFirstPoint = false;
                                        this.isPortPoint = false;
                                        PropertyDescriptor prePointInfProperty = TypeDescriptor.GetProperties(this.preClickControl)["PointInformation"];
                                        if (prePointInfProperty != null)
                                        {
                                            prePointInfProperty.SetValue(this.preClickControl, this.preClickInformation);
                                        }//end if (prePointInfProperty != null)
                                        DrawBackgroundImage();
                                        break;
                                    }//end if (pointInformationProperty != null)

                                    //�����ߵĶ˿�����һ�£�ͬΪ���������˿�,����ͬһ�ؼ���
                                    if (this.preClickInformation[0] == pointInformation[0] || this.preClickControl.Site.Name == obj.Site.Name)
                                    {
                                        //�ͷ�ѡ�е��������ţ�������״̬��ѡ�и�Ϊδѡ��
                                        pointInformationProperty.SetValue(obj, pointInformation);
                                        PropertyDescriptor prePointInfProperty = TypeDescriptor.GetProperties(this.preClickControl)["PointInformation"];
                                        if (prePointInfProperty != null)
                                        {
                                            prePointInfProperty.SetValue(this.preClickControl, this.preClickInformation);
                                        }//end if (prePointInfProperty != null)
                                        if (this.choosedLine != -1)
                                        {
                                            this.choosedLine = -1;
                                        }//end if (this.choosedLine != -1)
                                        this.showMouseMovePoint = false;
                                        this.getFirstPoint = false;
                                        this.isPortPoint = false;
                                        DrawBackgroundImage();
                                        break;
                                    }//end if (this.preClickInformation[0] == pointInformation[0])
                                    else
                                    {//�����ŵĶ˿����Ͳ�һ��
                                        //�������˵��������ͺϷ����������ߣ���¼�˵�
                                        PropertyDescriptor modifiedPointProperty = TypeDescriptor.GetProperties(obj)["ModifiedPoint"];
                                        if (modifiedPointProperty != null)
                                        {
                                            Point modifiedPoint = (Point)(modifiedPointProperty.GetValue(obj));
                                            if (modifiedPoint != this.preClickPoint || this.preClickControl != obj)
                                            {
                                                this.getFirstPoint = false;
                                                //Point middlePoint1 = new Point(0, 0);
                                                //Point middlePoint2 = new Point(0, 0);

                                                bool hasLine = false;
                                                LineInfo newLine = new LineInfo();
                                                string newlineStartInfo = null;
                                                string newlineEndInfo = null;

                                                //if (modifiedPoint.X + obj.Location.X > this.preClickPoint.X + this.preClickControl.Location.X)
                                                if (this.preClickInformation[0] == 'O')      //��һ��ѡ�е�Ϊ������ţ���¼�µ�������Ϣ
                                                {
                                                    newlineStartInfo = this.preClickControl.Site.Name + ";" + this.preClickInformation;
                                                    newlineEndInfo = obj.Site.Name + ";" + pointInformation;

                                                    for (int i = 0; i < this.linesInfo.Count; i++)
                                                    {
                                                        if (this.linesInfo[i].StartInfo + this.linesInfo[i].EndInfo == newlineStartInfo + newlineEndInfo)
                                                        {
                                                            hasLine = true;
                                                            break;
                                                        }//end if (this.lineInformation[i] == newlineInf)
                                                    }//end for

                                                    if (hasLine == true)        //�ж������Ƿ��Ѿ����ڣ����������ͷű�������
                                                    {
                                                        pointInformationProperty.SetValue(obj, pointInformation);
                                                        PropertyDescriptor prePointInfProperty = TypeDescriptor.GetProperties(this.preClickControl)["PointInformation"];
                                                        if (prePointInfProperty != null)
                                                        {
                                                            prePointInfProperty.SetValue(this.preClickControl, this.preClickInformation);
                                                        }//end if (prePointInfProperty != null)
                                                        if (this.choosedLine != -1)
                                                        {
                                                            this.choosedLine = -1;
                                                        }//end if (this.choosedLine != -1)
                                                        this.showMouseMovePoint = false;
                                                        this.getFirstPoint = false;
                                                        this.isPortPoint = false;
                                                        DrawBackgroundImage();
                                                        break;
                                                    }//end if (hasLine == true)
                                                    else        //��¼�µ�������Ϣ�������������;��������Ѿ����ߵ��������
                                                    {
                                                        newLine.StartPoint = new Point(Convert.ToInt32(this.preClickPoint.X * this.scaling + this.preClickControl.Location.X),
                                                            Convert.ToInt32(this.preClickPoint.Y * this.scaling + this.preClickControl.Location.Y));
                                                        newLine.EndPoint = new Point(Convert.ToInt32(modifiedPoint.X * this.scaling + obj.Location.X),
                                                            Convert.ToInt32(modifiedPoint.Y * this.scaling + obj.Location.Y));
                                                        newLine.StartInfo = newlineStartInfo;
                                                        newLine.EndInfo = newlineEndInfo;
                                                        this.linesInfo.Add(newLine);
                                                        portReflash();

                                                        DrawLineArithmetic newArith = new DrawLineArithmetic(this.linesInfo, this.blocksInfo);
                                                        this.linesInfo = newArith.ModifyLines;
                                                        if (CassViewGenerator.currentUndo != null && !CassViewGenerator.isUndo)
                                                        {//��Ӳ�����Ϣ
                                                            Operation lineOpt = new Operation();
                                                            lineOpt.Oname = UndoOperation.OperateStyle.ֱ�����.ToString();
                                                            lineOpt.Item = this.linesInfo[this.linesInfo.Count - 1];//�¼�ֱ�����б�ĩ��
                                                            CassViewGenerator.AddOpt(lineOpt);
                                                        }
                                                        this.mouseUpPoint = newLine.EndPoint;
                                                        this.mouseDownPoint = newLine.StartPoint;
                                                    }//end else if (hasLine == false)
                                                }//end if (modifiedPoint.X + obj.Location.X > this.preClickPoint.X + this.preClickControl.Location.X)
                                                else      //��һ��ѡ�е�Ϊ�������ţ���¼�µ�������Ϣ
                                                {
                                                    newlineStartInfo = obj.Site.Name + ";" + pointInformation;
                                                    newlineEndInfo = this.preClickControl.Site.Name + ";" + this.preClickInformation;

                                                    for (int i = 0; i < this.linesInfo.Count; i++)
                                                    {
                                                        if (this.linesInfo[i].StartInfo + this.linesInfo[i].EndInfo == newlineStartInfo + newlineEndInfo)
                                                        {
                                                            hasLine = true;
                                                            break;
                                                        }//end if (this.lineInformation[i] == newlineInf)
                                                    }//end for

                                                    if (hasLine == true)        //�������Ѿ�����
                                                    {
                                                        pointInformationProperty.SetValue(obj, pointInformation);
                                                        PropertyDescriptor prePointInfProperty = TypeDescriptor.GetProperties(this.preClickControl)["PointInformation"];
                                                        if (prePointInfProperty != null)
                                                        {
                                                            prePointInfProperty.SetValue(this.preClickControl, this.preClickInformation);
                                                        }//end if (prePointInfProperty != null)
                                                        if (this.choosedLine != -1)
                                                        {
                                                            this.choosedLine = -1;
                                                        }//end if (this.choosedLine != -1)
                                                        this.showMouseMovePoint = false;
                                                        this.getFirstPoint = false;
                                                        this.isPortPoint = false;
                                                        DrawBackgroundImage();
                                                        break;
                                                    }//end if (hasLine == true)
                                                    else        //��¼������Ϣ
                                                    {
                                                        newLine.EndPoint = new Point(Convert.ToInt32(this.preClickPoint.X * this.scaling + this.preClickControl.Location.X),
                                                            Convert.ToInt32(this.preClickPoint.Y * this.scaling + this.preClickControl.Location.Y));
                                                        newLine.StartPoint = new Point(Convert.ToInt32(modifiedPoint.X * this.scaling + obj.Location.X),
                                                            Convert.ToInt32(modifiedPoint.Y * this.scaling + obj.Location.Y));
                                                        newLine.EndInfo = newlineEndInfo;
                                                        newLine.StartInfo = newlineStartInfo;
                                                        this.linesInfo.Add(newLine);
                                                        portReflash();

                                                        DrawLineArithmetic newArith = new DrawLineArithmetic(this.linesInfo, this.blocksInfo);
                                                        this.linesInfo = newArith.ModifyLines;
                                                        if (CassViewGenerator.currentUndo != null && !CassViewGenerator.isUndo)
                                                        {//��Ӳ�����Ϣ
                                                            Operation lineOpt = new Operation();
                                                            lineOpt.Oname = UndoOperation.OperateStyle.ֱ�����.ToString();
                                                            lineOpt.Item = this.linesInfo[this.linesInfo.Count - 1];
                                                            CassViewGenerator.AddOpt(lineOpt);
                                                        }
                                                        this.mouseUpPoint = newLine.EndPoint;
                                                        this.mouseDownPoint = newLine.StartPoint;
                                                    }//end else if (hasLine == false)
                                                }//end else if (modifiedPoint.X + obj.Location.X < this.preClickPoint.X + this.preClickControl.Location.X)
                                                this.choosedLine = this.linesInfo.Count - 1;
                                            }//end if (modifiedPoint != this.preClickPoint || this.preClickControl != obj)
                                        }//end if (modifiedPointProperty != null)
                                    }//end else if (this.preClickInformation[0] != pointInformation[0])
                                }//end if (pointInformationProperty != null)
                                break;
                            }// end if (this.isPortPoint == true)
                        }//end if (checkPointProperty != null)
                    }//end foreach
                    if (this.isPortPoint == false)
                    {
                        PropertyDescriptor prePointInfProperty =
                            TypeDescriptor.GetProperties(this.preClickControl)["PointInformation"];
                        if (prePointInfProperty != null)
                        {
                            prePointInfProperty.SetValue(this.preClickControl, this.preClickInformation);
                        }//end if (prePointInfProperty != null)
                        if (this.choosedLine != -1)
                        {
                            this.choosedLine = -1;
                        }

                        this.showMouseMovePoint = false;
                        this.getFirstPoint = false;
                    }//end if (this.isPortPoint == false)

                    DrawBackgroundImage();
                }//end if (this.getFirstPoint == true)
            }
        }

        /// <summary>
        /// �û����˫��ʱ������
        /// </summary>
        private Point mouseDownPoint = new Point(0, 0);
        private Control preClickControl = null;     //��¼�������˶˵����ʼ�˵��Ӧ��ģ��
        private Point preClickPoint = new Point(0, 0);      //��¼�������˶˵����ʼ�˵�
        private string preClickInformation = "";        //��¼�������˶˵����ʼ�˵�Ļ�����Ϣ
        private bool getFirstPoint = false;     //����Ƿ��ѻ����ʼ�˵�����
        [Browsable(false)]
        public Point MouseDownPoint
        {
            get
            {
                return new Point(-1, -1);
            }
            set
            {
                this.mouseDownPoint = this.PointToClient(value);

                #region �����µ�����
                this.isPortPoint = false;
                foreach (Control obj in this.Controls)
                {
                    PropertyDescriptor clickPointProperty = TypeDescriptor.GetProperties(obj)["ClickPoint"];
                    if (clickPointProperty != null)
                    {
                        clickPointProperty.SetValue(obj, this.mouseDownPoint);
                    }//end if(clickPointProperty != null)
                    else
                    {
                        continue;
                    }//end else if (clickPointProperty != null)
                    PropertyDescriptor checkPointProperty = TypeDescriptor.GetProperties(obj)["CheckPoint"];
                    if (checkPointProperty != null)
                    {
                        this.isPortPoint = (bool)checkPointProperty.GetValue(obj);
                        if (this.isPortPoint == true)
                        {
                            PropertyDescriptor modifiedPointProperty = TypeDescriptor.GetProperties(obj)["ModifiedPoint"];
                            if (modifiedPointProperty != null)
                            {
                                Point modifiedPoint = (Point)(modifiedPointProperty.GetValue(obj));
                                if (this.getFirstPoint == false)
                                {
                                    this.getFirstPoint = true;
                                    this.preClickControl = obj;
                                    this.preClickPoint = modifiedPoint;

                                    PropertyDescriptor pointInformationProperty = TypeDescriptor.GetProperties(obj)["PointInformation"];
                                    if (pointInformationProperty != null)
                                    {
                                        this.preClickInformation = (string)(pointInformationProperty.GetValue(obj));
                                    }

                                    this.mouseDownPoint = new Point(Convert.ToInt32(this.preClickPoint.X
                                        * this.scaling + this.preClickControl.Location.X),
                                        Convert.ToInt32(this.preClickPoint.Y * this.scaling
                                        + this.preClickControl.Location.Y));
                                    this.choosedLine = -1;
                                }//end if (this.getFirstPoint == true)
                            }//end if (modifiedPointProperty != null)
                            break;
                        }// end if (this.isPortPoint == true)
                        else
                        {
                            if (this.choosedLine != -1)
                            {
                                this.choosedLine = -1;
                                this.showMouseMovePoint = false;
                                this.Invalidate();
                            }
                        }//end if (this.isPortPoint == false)
                    }//end if (checkPointProperty != null)
                }//end foreach
                if (this.isPortPoint == true)
                {
                    DrawBackgroundImage();
                }
                #endregion

                #region ѡ���ѻ��Ƶ�����
                if (this.isPortPoint == false)
                {
                    this.choosedLine = -1;
                    int x_Max = 0, x_Min = 0, y_Max = 0, y_Min = 0;
                    for (int i = 0; i < this.linesInfo.Count; i++)
                    {
                        if (this.choosedLine != -1)
                        { break; }

                        List<Point> tempLine = new List<Point>();
                        tempLine.Add(this.linesInfo[i].StartPoint);
                        tempLine.AddRange(this.linesInfo[i].MiddlePoint);
                        tempLine.Add(this.linesInfo[i].EndPoint);


                        for (int j = 0; j < tempLine.Count - 1; j++)
                        {
                            x_Max = Math.Max(tempLine[j].X, tempLine[j + 1].X);
                            x_Min = Math.Min(tempLine[j].X, tempLine[j + 1].X);

                            y_Max = Math.Max(tempLine[j].Y, tempLine[j + 1].Y);
                            y_Min = Math.Min(tempLine[j].Y, tempLine[j + 1].Y);

                            if (this.mouseDownPoint.X > x_Min - 2 && this.mouseDownPoint.X < x_Max + 2
                                && this.mouseDownPoint.Y > y_Min - 2 && this.mouseDownPoint.Y < y_Max + 2)
                            {
                                this.choosedLine = i;
                                break;
                            }//end if
                        }
                    }//end for(int i = 0; i < this.absolutePoints.Count; i+=4) 
                    this.Refresh();
                }//end if(this.isPortPoint == false)
                #endregion
            }
        }

        private bool linesEditable = false;
        /// <summary>
        /// ָʾ��ǰ��ģ��˿������Ƿ����ڿ�����״̬
        /// </summary>  
        [Browsable(false)]
        public bool LinesEditable
        {
            get
            {
                return this.linesEditable;
            }
            set
            {
                this.linesEditable = value;
                foreach (Control obj in this.Controls)
                {
                    if (value == true)
                    {
                        obj.Visible = false;
                    }
                    else
                    {
                        obj.Visible = true;
                    }
                }
                DrawBackgroundImage();
            }
        }

        /// <summary>
        /// ��¼ҳ��ģ������ű�������С��ΧΪԭʼ��С��0.2��2��
        /// </summary>
        private float scaling = 1.0F;
        [Browsable(false)]
        public float Scaling
        {
            get
            {
                return this.scaling;
            }
            set
            {
                if (value >= 0.2F && value <= 2.0F)
                {

                    if (CassViewGenerator.currentUndo != null && !CassViewGenerator.isUndo)
                    {
                        Operation scaleOpt = new Operation();
                        if (this.scaling > value)
                        {
                            scaleOpt.Oname = UndoOperation.OperateStyle.������С.ToString();
                        }
                        else
                        {
                            scaleOpt.Oname = UndoOperation.OperateStyle.�����Ŵ�.ToString();
                        }
                        scaleOpt.Change = new object[2];
                        scaleOpt.Change[0] = this.scaling;
                        scaleOpt.Change[1] = value;
                        CassViewGenerator.AddOpt(scaleOpt);
                    }

                    isScale = true;//���ſؼ�����Ŀؼ��ƶ��������������
                    this.scaling = value;
                    foreach (Control obj in this.Controls)
                    {
                        PropertyDescriptor controlScaling = TypeDescriptor.GetProperties(obj)["Scaling"];
                        if (controlScaling != null)
                        {
                            controlScaling.SetValue(obj, value);
                        }
                    }
                    isScale = false;
                    for (int i = 0; i < this.linesInfo.Count; i++)
                    {
                        this.linesInfo[i] = LineInfoScaling(this.linesInfo[i]);
                    }
                    //�޸�ֱ�������յ���������¼���ֱ��·��
                    DrawLineArithmetic newArith = new DrawLineArithmetic(this.linesInfo, this.blocksInfo);
                    this.linesInfo = newArith.ModifyLines;
                    DrawBackgroundImage();
                }
            }

        }

        /// <summary>
        /// ɾ��ѡ�������,���ͷ�ֱ�����˵�
        /// </summary>
        /// <param name="LineNum">��Ҫɾ����ֱ�����</param>
        /// <param name="CanClear">�Ƿ�Ҫ���¶˿���Ϣ</param>
        public void DeleteLine(int LineNum, bool CanClear)
        {
            if (LineNum != -1)
            {
                string[] StartInfo = this.linesInfo[LineNum].StartInfo.Split(';');
                string[] EndInfo = this.linesInfo[LineNum].EndInfo.Split(';');

                int deleteCount = 0;
                foreach (Control obj in this.Controls)
                {
                    if (obj.Site.Name == StartInfo[0])
                    {
                        PropertyDescriptor pointInformationProperty = TypeDescriptor.GetProperties(obj)["PointInformation"];
                        if (pointInformationProperty != null)
                        {
                            pointInformationProperty.SetValue(obj, StartInfo[1]);
                            deleteCount++;
                        }
                    }
                    if (obj.Site.Name == EndInfo[0])
                    {
                        PropertyDescriptor pointInformationProperty = TypeDescriptor.GetProperties(obj)["PointInformation"];
                        if (pointInformationProperty != null)
                        {
                            pointInformationProperty.SetValue(obj, EndInfo[1]);
                            deleteCount++;
                        }
                    }
                    if (deleteCount == 2)
                    {
                        deleteCount = 0;
                        break;
                    }
                }
                if (CassViewGenerator.currentUndo != null && !CassViewGenerator.isUndo)
                {//��Ӳ�����Ϣ
                    Operation lineOpt = new Operation();
                    lineOpt.Oname = UndoOperation.OperateStyle.ֱ��ɾ��.ToString();
                    lineOpt.Item = this.linesInfo[LineNum];
                    CassViewGenerator.AddOpt(lineOpt);
                }
                this.linesInfo.RemoveAt(LineNum);
                if (CanClear)
                {//�������������Ϣ
                    portReflash();
                    DrawBackgroundImage();
                }
                this.choosedLine = -1;
            }
        }

        /// <summary>
        /// ����һ������ʱ �����ߵ���㡢�յ㡢����Ӧ��ģ�����LineInfo�Ľṹ����
        /// </summary>
        /// <param name="ControlName">�ı�λ�õĿؼ���</param>
        /// <param name="ChangeX">X���λ��</param>
        /// <param name="ChangeY">Y���λ��</param>
        private void LineInfoChanged(string ControlName, int ChangeX, int ChangeY)
        {
            for (int j = 0; j < this.linesInfo.Count; j++)
            {
                LineInfo ChangedLine = this.linesInfo[j];
                bool findLine = false;
                if (linesInfo[j].StartInfo.Split(';')[0] == ControlName)
                {
                    ChangedLine.StartPoint = new Point(ChangedLine.StartPoint.X + ChangeX, ChangedLine.StartPoint.Y + ChangeY);
                    findLine = true;
                }
                else if (linesInfo[j].EndInfo.Split(';')[0] == ControlName)
                {
                    ChangedLine.EndPoint = new Point(ChangedLine.EndPoint.X + ChangeX, ChangedLine.EndPoint.Y + ChangeY);
                    findLine = true;
                }
                if (findLine && (ChangeX != 0 || ChangeY != 0))
                {
                    linesInfo.RemoveAt(j);
                    linesInfo.Insert(j, ChangedLine);
                    findLine = false;
                }
            }
        }

        /// <summary>
        /// ���ݵ�ǰ���ű��ʶ��߶ε������յ��������
        /// </summary>
        /// <param name="oldLine">����ǰֱ����Ϣ</param>
        /// <returns>���ź�ֱ����Ϣ</returns>
        private LineInfo LineInfoScaling( LineInfo oldLine)
        {
            LineInfo newLine = oldLine;
            string[] startInfo = oldLine.StartInfo.Split(',', ';');
            string[] endInfo = oldLine.EndInfo.Split(',', ';');

            foreach (Control element in this.Controls)
            {
                if (element.Tag.ToString() == startInfo[0])
                {
                    newLine.StartPoint = new Point(Convert.ToInt32(element.Location.X + element.Width - 3 * this.scaling), 
                        Convert.ToInt32(element.Location.Y + (31 + 15 * Convert.ToInt32(startInfo[2])) * this.scaling));
                }
                else if (element.Tag.ToString() == endInfo[0])
                {
                    newLine.EndPoint = new Point(Convert.ToInt32(element.Location.X + 3 * this.scaling), 
                        Convert.ToInt32(element.Location.Y + (31 + 15 * Convert.ToInt32(startInfo[2])) * this.scaling));
                }
            }
            return newLine;
        }


        #endregion

        #region ��������

        /// <summary>
        /// �ؼ�������ɫ�ı��¼�������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void CassView_BackColorChanged(object sender, EventArgs e)
        {
            foreach (Control obj in this.Controls)
            {
                obj.BackColor = this.BackColor;
            }
            DrawBackgroundImage();
        }

        /// <summary>
        /// �ؼ�ˢ���¼�������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void CassView_Paint(object sender, PaintEventArgs e)
        {
            bool drawLine = false;

            //��ɫ���
            if (this.isPortPoint == true)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.FillEllipse(Brushes.Red, this.mouseDownPoint.X - 3,
                     this.mouseDownPoint.Y - 4, 7, 7);
                if (this.getFirstPoint == false)
                {
                    e.Graphics.FillEllipse(Brushes.Red, this.mouseUpPoint.X - 3,
                        this.mouseUpPoint.Y - 4, 7, 7);
                }
            }

            //ѡ����
            if (this.choosedLine > -1 && this.choosedLine < this.linesInfo.Count)
            {
                LineInfo tempLine = this.linesInfo[this.choosedLine];

                e.Graphics.FillEllipse(Brushes.Red, tempLine.StartPoint.X - 3, tempLine.StartPoint.Y - 4, 7, 7);
                e.Graphics.FillEllipse(Brushes.Red, tempLine.EndPoint.X - 3, tempLine.EndPoint.Y - 4, 7, 7);

                //for (int i = 0; i < tempLine.Count; i++)
                //{
                e.Graphics.DrawLine(Pens.Red, tempLine.StartPoint, tempLine.MiddlePoint[0]);
                for (int j = 0; j + 1 < tempLine.MiddlePoint.Count; j++)
                { e.Graphics.DrawLine(Pens.Red, tempLine.MiddlePoint[j], tempLine.MiddlePoint[j + 1]); }
                e.Graphics.DrawLine(Pens.Red, tempLine.MiddlePoint[tempLine.MiddlePoint.Count - 1], tempLine.EndPoint);
                //}
                drawLine = true;
            }

            //�ƶ���
            if (this.showMouseMovePoint == true && drawLine == false)
            {
                Pen myPen = new Pen(Color.Red, 2);
                e.Graphics.DrawRectangle(myPen, this.mouseMoveShowPoint.X - 5, this.mouseMoveShowPoint.Y - 5, 9, 9);
                this.showMouseMovePoint = false;//20090601
            }

            //����
            if (this.getFirstPoint == true && (this.mouseMovePoint.X != 0 || this.mouseMovePoint.Y != 0))
            {
                e.Graphics.DrawLine(Pens.Yellow, this.mouseDownPoint, this.mouseMovePoint);
            }
        }

        /// <summary>
        /// ҳ���С�ı��¼�������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void CassView_Resize(object sender, EventArgs e)
        {
            DrawBackgroundImage();
        }

        /// <summary>
        /// ���Ʊ���ͼƬ
        /// </summary>
        public void DrawBackgroundImage()//20090601 �ĳɶ���
        {
            this.backgroundImage = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(this.backgroundImage);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(this.BackColor);
            if (this.linesEditable == true || CassViewGenerator.isUndo)
            {//����ʱҲ�ɽ���
                foreach (Control obj in this.Controls)
                {
                    if (obj.BackgroundImage != null)
                    {
                        g.DrawImage(obj.BackgroundImage, obj.Location);
                    }
                }
            }
            for (int i = 0; i < this.linesInfo.Count; i++)
            {
                g.DrawLine(Pens.Black, linesInfo[i].StartPoint, linesInfo[i].MiddlePoint[0]);
                for (int j = 0; j + 1 < linesInfo[i].MiddlePoint.Count; j++)
                { g.DrawLine(Pens.Black, linesInfo[i].MiddlePoint[j], linesInfo[i].MiddlePoint[j + 1]); }
                g.DrawLine(Pens.Black, linesInfo[i].MiddlePoint[linesInfo[i].MiddlePoint.Count - 1], linesInfo[i].EndPoint);
            }

            g.Dispose();
            this.BackgroundImage = this.backgroundImage;
        }

        #endregion

        #region �ؼ�����

        static public List<string> OnlyOneOut = new List<string>(new string[] { "����", "ϵͳ����", "�������", "ADת��", "�ļ�����", "��������","�豸����" });//ֻ��һ�������
        static public List<string> OnlyOneIn = new List<string>(new string[] { "�������", "��ת", "����", "����", "DAת��","�ļ����" });//ֻ��һ�������
        

        /// <summary>
        /// �ӿؼ�λ�ñ仯�¼�������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void Control_LocationChanged(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            if (control.Location.X < 0 || control.Location.Y < 0)
            {
                control.Location = this.PointToClient(MousePosition);
            }
            else
            {
                ControlInfoChanged(control);
                DrawLineArithmetic newArith = new DrawLineArithmetic(this.linesInfo, this.blocksInfo);
                this.linesInfo = newArith.ModifyLines;

                DrawBackgroundImage();
            }
        }

        /// <summary>
        /// ����һ���ؼ�ʱ �ѿؼ������������Ϣ���� ControlInfo�Ľṹ����
        /// </summary>
        /// <param name="ChangeControl"></param>
        private void ControlInfoChanged(Control ChangeControl)
        {
            PropertyDescriptor serialNumber = TypeDescriptor.GetProperties(ChangeControl)["SerialNumber"];
            int ControlNum = Convert.ToInt32(serialNumber.GetValue(ChangeControl));

            for (int i = 0; i < this.blocksInfo.Count; i++)
            {
                if (this.blocksInfo[i].BlockNum == ControlNum)
                {
                    //�ؼ��ƶ�����
                    Operation blockOpt = new Operation();
                    blockOpt.Oname = UndoOperation.OperateStyle.�ؼ��ƶ�.ToString();
                    blockOpt.Item = ChangeControl;
                    blockOpt.Change = new object[2];
                    blockOpt.Change[0] = blocksInfo[i];//�䶯ǰ�Ŀ���Ϣ

                    BlockInfo tempCtrlInfo = this.blocksInfo[i];

                    if (this.blocksInfo[i].StartPoint == new Point(0, 0))//���θ�ֵ
                    {
                        tempCtrlInfo.StartPoint = new Point(ChangeControl.Location.X - 5, ChangeControl.Location.Y - 5);
                        tempCtrlInfo.Width = ChangeControl.Width + 10;
                        tempCtrlInfo.Height = ChangeControl.Height + 10;

                        blockOpt.Change[1] = tempCtrlInfo;//�䶯��Ŀ���Ϣ
                        this.blocksInfo.RemoveAt(i);
                        this.blocksInfo.Insert(i, tempCtrlInfo);
                    }
                    else if (ChangeControl.Location != this.blocksInfo[i].StartPoint)//�ؼ�λ�øı�
                    {
                        int changeX = 0;
                        int changeY = 0;
                        changeX = ChangeControl.Location.X - 5 - tempCtrlInfo.StartPoint.X;
                        changeY = ChangeControl.Location.Y - 5 - tempCtrlInfo.StartPoint.Y;
                        tempCtrlInfo.StartPoint = new Point(ChangeControl.Location.X - 5, ChangeControl.Location.Y - 5);
                        //�ؼ���СҲҪ���£����ֿɱ��С�Ŀؼ�20090610
                        tempCtrlInfo.Width = ChangeControl.Width + 10;
                        tempCtrlInfo.Height = ChangeControl.Height + 10;

                        blockOpt.Change[1] = tempCtrlInfo;//�䶯��Ŀ���Ϣ
                        this.blocksInfo.RemoveAt(i);
                        this.blocksInfo.Insert(i, tempCtrlInfo);
                        LineInfoChanged(tempCtrlInfo.BlockName, changeX, changeY);
                    }
                    if (CassViewGenerator.currentUndo != null && !isScale && !CassViewGenerator.isUndo)
                    {// ����ʱ����¼�ؼ��ƶ���Ϣ
                        UndoFlag = true;
                        CassViewGenerator.AddOpt(blockOpt);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// CassView�ӿؼ��Ƴ��¼�������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void CassView_ControlRemoved(object sender, ControlEventArgs e)
        {
            List<object> tempChange = new List<object>();//������¼ɾ���Ŀؼ���Ϣ�Ϳ���Ϣ

            string removeControlName = (string)(e.Control.Tag);

            for (int i = 0; i < this.ctrlsInfo.Count; i++)
            {
                if (ctrlsInfo[i].ControlName == removeControlName)
                {//ע��ɾ���Ⱥ�
                    if (ctrlsInfo[i].VisibleFunctionProperty != null)
                    {
                        ModifyPortName(ctrlsInfo[i].CodeInfo[1], XProp.GetValue(CassViewGenerator.portIndex, ctrlsInfo[i].VisibleFunctionProperty), null);
                    }
                    tempChange.Add(ctrlsInfo[i]);
                    tempChange.Add(blocksInfo[i]);

                    this.ctrlsInfo.RemoveAt(i);
                    this.blocksInfo.RemoveAt(i);
                    break;
                }
            }

            for (int i = this.linesInfo.Count - 1; i >= 0; i--)//�Ӻ���ǰɾ����Ϣ
            {
                if ((string)(e.Control.Tag) == this.linesInfo[i].StartInfo.Split(';')[0]
                    || (string)(e.Control.Tag) == this.linesInfo[i].EndInfo.Split(';')[0])
                {
                    CassViewGenerator.isUndo = true;//�ؼ�ɾ��������ֱ��ɾ�������ֱ��ɾ������
                    tempChange.Add(linesInfo[i]);
                    DeleteLine(i, false);
                    CassViewGenerator.isUndo = false;
                }
            }

            if (CassViewGenerator.currentUndo != null && !CassViewGenerator.isUndo && CassViewGenerator.currentUndo.canUndo && tempChange.Count != 0)
            {
                CassViewGenerator.currentUndo.SpecialAddinfo(tempChange);
            }

            e.Control.LocationChanged -= new EventHandler(Control_LocationChanged);
            this.recordControls--;
            if (this.ctrlsInfo.Count == 0)
            {//����ؼ�ɾ����ʱ��Infos������Ϣ����20090618
                this.InfosList = SaveInfo();
            }
            portReflash();
            DrawBackgroundImage();
        }

        /// <summary>
        /// CassView�ӿؼ������¼�������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void CassView_ControlAdded(object sender, ControlEventArgs e)
        {
            if (this.Controls.Count > this.recordControls && !CassViewGenerator.isUndo)
            {
                ControlInfo CtrlInfo = new ControlInfo();
                BlockInfo BlckInfo = new BlockInfo();
                CtrlInfo.ControlName = e.Control.Site.Name;
                BlckInfo.BlockName = e.Control.Site.Name;
                bool isCopy = false;//�Ƿ�ʱճ���ؼ�
                PropertyDescriptor serialNumber = TypeDescriptor.GetProperties(e.Control)["SerialNumber"];
                PropertyDescriptor portNameProperty = TypeDescriptor.GetProperties(e.Control)["PortName"];
                PropertyDescriptor showNumberProperty = TypeDescriptor.GetProperties(e.Control)["ShowNumber"];
                PropertyDescriptor controlScaling = TypeDescriptor.GetProperties(e.Control)["Scaling"];
                #region
                //��FileIn  ��ȡ��ǰ���̵�·�� 2013.11.22
                PropertyDescriptor currentProjectPath = TypeDescriptor.GetProperties(e.Control)["ProjectPath"];
                if (currentProjectPath != null)
                {
                    currentProjectPath.SetValue(e.Control, CassViewGenerator.currentProjectPath);
 
                }
                
            
                #endregion

                if (controlScaling != null)
                {//���ݵ�ǰ�����ʵ����ؼ���С
                    controlScaling.SetValue(e.Control, this.scaling);
                }
                //����ģ�����
                if (serialNumber != null)
                {
                    List<int> tempNumlist = new List<int>();//��ʱ������пؼ�����ż���
                    foreach (ControlInfo ctrl in this.ctrlsInfo)
                    {
                        tempNumlist.Add(ctrl.ControlNum);
                    }
                    for (int i = 0; i <= this.ctrlsInfo.Count; i++)
                    {
                        if (!tempNumlist.Contains(i) || i == this.ctrlsInfo.Count)
                        {
                            serialNumber.SetValue(e.Control, i);
                            CtrlInfo.ControlNum = i;
                            BlckInfo.BlockNum = i;
                            break;
                        }
                    }
                }//end if (serialNumber != null)          
                if (showNumberProperty != null)
                {//�����Ƿ���ʾ���
                    showNumberProperty.SetValue(e.Control, CassViewGenerator.ShowNumber);
                }
                //PropertyDescriptor currentControlName = TypeDescriptor.GetProperties(e.Control)["Controlname"];
                //if (currentControlName != null)
                //{
                //    currentControlName.SetValue(e.Control, e.Control.Site.Name);
                //}
                if (ToolBoxServiceImpl.toolXML.FirstChild != null)
                {
                    string moduleSort = null;
                    bool setValue = false;
                    bool StartFind = false;

                    foreach (XmlNode categoryNode in ToolBoxServiceImpl.toolXML.FirstChild.ChildNodes)
                    {
                        if (categoryNode != null && categoryNode.Attributes[0].InnerText != CassViewGenerator.SpecialCodeNode)
                        {
                            //�ϳ��ؼ�
                            if (ToolBoxServiceImpl.typeNameString != null && ToolBoxServiceImpl.typeNameString.Length == 4)
                            {
                                if (categoryNode.Attributes[0].InnerText == ToolBoxServiceImpl.typeNameString[3])
                                {
                                    moduleSort = ToolBoxServiceImpl.typeNameString[2];
                                    StartFind = true;
                                }
                            }//ճ���ؼ�
                            else if (this.FindControlName(e.Control.Site.Name) == -1)
                            {
                                PropertyDescriptor moduleSortProperty = TypeDescriptor.GetProperties(e.Control)["ModuleSort"];
                                if (moduleSortProperty != null)
                                {
                                    moduleSort = moduleSortProperty.GetValue(e.Control).ToString();
                                    StartFind = true;
                                    isCopy = true;
                                }
                                InitializeIOport(e.Control);
                                //������������ͳ���ϵͳ������Ҫ�������
                            }
                            if (StartFind)//��ʼ��ڵ�����
                            {
                                foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                                {
                                    if (moduleSort == toolItemNode.Attributes[0].InnerText.Split(',')[2]
                                        && toolItemNode.FirstChild != null)
                                    {
                                        //��ȡ�������ԣ�����ģ��������ԣ��ı�ģ����ʽ
                                        //����ʼ���ؼ��������Ϣ
                                        ReadBasicProperty(toolItemNode.FirstChild, e.Control, ref CtrlInfo);

                                        //��ȡ�������ԣ�Ϊģ����ӹ������ԣ�����ģ�����
                                        if (toolItemNode.FirstChild != null && toolItemNode.FirstChild.NextSibling != null
                                            && toolItemNode.FirstChild.NextSibling.ChildNodes.Count > 0)
                                        {
                                            ReadFunctionProperty(toolItemNode.FirstChild.NextSibling, ref CtrlInfo);
                                        }//end if (toolItemNode.FirstChild != null)

                                        //��ȡָ�����ԣ�������������Control.Cʱ����
                                        if ((toolItemNode.LastChild.Name == "CodeProperty" || toolItemNode.LastChild.Name == "OtherInfo") && toolItemNode.LastChild != null
                                            && toolItemNode.LastChild.HasChildNodes)
                                        {
                                            ReadOtherProperty(toolItemNode.LastChild, ref CtrlInfo);
                                        }
                                        setValue = true;
                                        break;
                                    }//end if (moduleSort == toolItemNode.Attributes[0].InnerText.Split(',')[2])
                                }// end foreach(XmlNode toolItemNode in categoryNode.ChildNodes)
                            }
                        }//end if (categoryNode != null && categoryNode.Attributes[0].InnerText != CassViewGenerator.SpecialCodeNode)  
                        if (setValue)
                        { break; }
                    }//end foreach (XmlNode categoryNode in ToolBoxServiceImpl.toolXML.FirstChild.ChildNodes)
                    ToolBoxServiceImpl.typeNameString = null;
                }//end if (ToolBoxServiceImpl.toolXML.FirstChild != null)

                if (portNameProperty != null && CtrlInfo.CodeInfo != null)
                {
                    string portName = AddPortName(CtrlInfo, ref CassViewGenerator.PortInfoList);
                    CtrlInfo.CodeInfo[2] = CtrlInfo.CodeInfo[1] + portName;
                    XProp.SetValue(portName, CassViewGenerator.portIndex, CtrlInfo.VisibleFunctionProperty);
                    portNameProperty.SetValue(e.Control, CtrlInfo.CodeInfo[2]);
                }
                if (isCopy && GenerateCode.SortCtrlName.Contains(CtrlInfo.CodeInfo[0]))
                {//����ؼ��Կؼ���Ϣ��Codeinfo���ֽ��и�ֵ
                    foreach (ControlInfo ctrl in this.CopyCtrl)
                    {
                        if (ctrl.CodeInfo[0] == CtrlInfo.CodeInfo[0])
                        {
                            CtrlInfo.CodeInfo = ctrl.CodeInfo;
                            break;
                        }
                    }
                    if ((GenerateCode.SortCtrlName.IndexOf(CtrlInfo.CodeInfo[0]) == 1 || GenerateCode.SortCtrlName.IndexOf(CtrlInfo.CodeInfo[0]) == 2))
                    {
                        if (CtrlInfo.CodeInfo[1] != null)
                        {
                            PropertyDescriptor ChoosePortName = TypeDescriptor.GetProperties(e.Control)["ChoosePortName"];
                            ChoosePortName.SetValue(e.Control, CtrlInfo.CodeInfo[1]);
                        }
                        if (CtrlInfo.CodeInfo[2] != null)
                        {
                            PropertyDescriptor ChooseModuleProperty = TypeDescriptor.GetProperties(e.Control)["ChooseModuleProperty"];
                            ChooseModuleProperty.SetValue(e.Control, CtrlInfo.CodeInfo[2]);
                        }
                    }
                }
                if (!this.ctrlsInfo.Contains(CtrlInfo))
                {
                    this.ctrlsInfo.Add(CtrlInfo);
                    this.blocksInfo.Add(BlckInfo);
                }
                this.recordControls++;

                if (CassViewGenerator.currentUndo != null)
                {//�ؼ��������//ע��ʱ�ؼ�λ�û�ûȷ�� ������blockinfo ���޸�ȷ��ֵ
                    Operation ctrlOpt = new Operation();
                    ctrlOpt.Oname = UndoOperation.OperateStyle.�ؼ����.ToString();
                    ctrlOpt.Item = e.Control;
                    ctrlOpt.Change = new object[4];
                    ctrlOpt.Change[0] = ((HostControl)CassViewGenerator.currentTabPage.Controls[0]).HostDesign;
                    ctrlOpt.Change[1] = e.Control.Site.Name;
                    ctrlOpt.Change[2] = ctrlsInfo[ctrlsInfo.Count - 1];
                    ctrlOpt.Change[3] = blocksInfo[blocksInfo.Count - 1];
                    CassViewGenerator.AddOpt(ctrlOpt);
                    this.UndoFlag = true;
                }
            }//end if (this.Controls.Count > this.recordControls)
            else if(CassViewGenerator .isUndo)
            {//����������Ȼ��Ҫ���ӿؼ�ָ��
                this.recordControls++;
            }
            #region �������ģ����������

            e.Control.BackColor = this.BackColor;
            e.Control.LocationChanged += new EventHandler(Control_LocationChanged);
            e.Control.Tag = e.Control.Site.Name;
            if (this.linesEditable == true)
            {
                e.Control.Visible = false;
            }

            #endregion

            #  region    2013.11.29 �����ļ�
         //   ControlInfo temp = e.Control;
            if (e.Control.Site.Name.Contains("fileOutput"))
            {
                CassViewGenerator.SetCodeInfo(this, (Control)e.Control, "�����ļ�", "\"" + "null" + "\"");
            }

            #endregion

        }

        /// <summary>
        /// ��ʼ���ؼ�����������ڵ�ռ��
        /// ճ���ؼ�ʱ����
        /// </summary>
        /// <param name="ctrl"></param>
        private void InitializeIOport(Control ctrl)
        {
            PropertyDescriptor InputChoosed = TypeDescriptor.GetProperties(ctrl)["InputChoosed"];
            PropertyDescriptor OutputChoosed = TypeDescriptor.GetProperties(ctrl)["OutputChoosed"];
            if (InputChoosed != null)
            {
                InputChoosed.SetValue(ctrl, new bool[10] { false, false, 
            false, false, false, false, false, false, false, false });
            }
            if (OutputChoosed != null)
            {
                OutputChoosed.SetValue(ctrl, new short[10] { 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0 });
            }
        }

        /// <summary>
        /// ��ȡ"BasicProperty"�ӽڵ�,
        /// ���ݿؼ�����������������,�����ݺ���ReadValue������������ֵ,���пؼ����Եĸ�ֵ������
        /// </summary>
        /// <param name="node">�������Խڵ�</param>
        /// <param name="instance">�ؼ�����</param>
        /// <param name="CtrlInfo">�ؼ���Ϣ</param>
        /// <returns>�����Ϣ��ֵ��Ŀؼ���Ϣ</returns>
        private void ReadBasicProperty(XmlNode node, Control instance, ref ControlInfo CtrlInfo)
        {
            try
            {

                CtrlInfo.CodeInfo = new string[3];//��ʼ�����鳤��

                foreach (XmlNode property in node.ChildNodes)
                {
                    //XmlNode.Attributes ��ȡһ�� XmlAttributeCollection���������ýڵ�����ԡ� �����ǻ��"name"������
                    XmlAttribute nameAttr = property.Attributes["name"];

                    //TypeDescriptor.GetProperties ������������͵����� (Property) �ļ��ϡ� 
                    PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(instance)[nameAttr.Value];
                    object value = null;

                    if (propDescriptor != null && ReadValue(property, propDescriptor.Converter, ref value) && value != null)
                    {
                        //PropertyDescriptor.SetValue �����������б���дʱ���������ֵ����Ϊһ����ͬ��ֵ��
                        propDescriptor.SetValue(instance, value);
                    }
                    if (nameAttr.Value == "ModuleSort")
                    {
                        CtrlInfo.SortName = property.InnerText;
                        CtrlInfo.CodeInfo[0] = property.InnerText;//0λ����ModuleSort
                        if (property.InnerText == "����")
                        {//��������ֵ
                            CtrlInfo.CodeInfo[1] = "0";
                        }
                        if (OnlyOneIn.Contains(property.InnerText))
                        {
                            List<string[]> tempInfo = new List<string[]>();
                            tempInfo.Add(new string[4] { null, null, null, "0" });
                            CtrlInfo.InputInfo = tempInfo;
                        }
                        else if (OnlyOneOut.Contains(property.InnerText))
                        { CtrlInfo.OutputInfo = InitializeIOinfo(1); }
                    }
                    else if (nameAttr.Value == "ModuleName")
                    {
                        CtrlInfo.CodeInfo[1] = property.InnerText;//1λ����ModuleName                                   
                    }
                    else if (nameAttr.Value == "OutputName")
                    { //��ʼ���������Ϣ
                        if (property.InnerText != "NULL")
                        {
                            CtrlInfo.OutputInfo = InitializeIOinfo(property.InnerText.Split(',').Length);
                        }
                        else
                        { CtrlInfo.OutputInfo = new List<string[]>(); }
                    }
                    else if (nameAttr.Value == "InputName")
                    {
                        if (property.ChildNodes.Count > 1)
                        {
                            List<string[]> Inputinfo = new List<string[]>();
                            foreach (XmlNode info in property.ChildNodes)
                            {
                                string[] Ininfo = new string[4];
                                Ininfo[2] = info.Attributes["name"].Value;
                                Ininfo[3] = info.InnerText;
                                Inputinfo.Add(Ininfo);
                            }
                            CtrlInfo.InputInfo = Inputinfo;
                        }
                        else if (property.InnerText != "NULL")
                        {
                            CtrlInfo.InputInfo = InitializeIOinfo(property.InnerText.Split(',').Length);
                        }
                        else
                        { CtrlInfo.InputInfo = new List<string[]>(); }
                    }
                }
                //return CtrlInfo;
            }
            catch (NotSupportedException e)
            {
                MessageBox.Show("��ȡ��������ʧ�ܣ�");
                //return CtrlInfo;
            }
        }

        /// <summary>
        /// ��ȡ"Value"�ڵ�
        /// �������ԣ��������ĵ��е�����ֵ�������͵�ת����ת��Ϊ��ǰ���Ե�ֵ����
        /// </summary>
        /// <param name="node">���ڵ�</param>
        /// <param name="converter">���͵�ת����</param>  
        /// <param name="value">����</param>
        /// <returns>��ֵ�Ƿ��ܹ���ת������Ҫ������</returns>
        private bool ReadValue(XmlNode node, TypeConverter converter, ref object value)
        {
            try
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    //XmlNodeTypeö��ָ���ڵ�����͡�.Text:�ڵ���ı����ݡ�Text �ڵ㲻�ܾ����κ��ӽڵ㡣
                    if (child.NodeType == XmlNodeType.Text)
                    {
                        value = converter.ConvertFromInvariantString(node.InnerText);
                        return true;
                    }
                    else if (child.NodeType == XmlNodeType.Element)
                    {//������Ϣ�ڵ����ô˲��ֶ�ȡ����ӿ���Ϣ20090616
                        List<string> tempIO = new List<string>();
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            tempIO.Add(node.ChildNodes[i].Attributes["name"].Value);
                        }
                        value = String.Join(",", tempIO.ToArray());
                        return true;
                    }
                    else if (child.Name.Equals("Binary"))
                    {
                        byte[] data = Convert.FromBase64String(child.InnerText);

                        if (GetConversionSupported(converter, typeof(byte[])))
                        {
                            value = converter.ConvertFrom(data);
                            return true;
                        }
                        else
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            MemoryStream stream = new MemoryStream(data);

                            value = formatter.Deserialize(stream);
                            return true;
                        }
                    }

                    else
                    {
                        value = null;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        ///���ظ�ת�����Ƿ����ʹ��ָ���������Ľ��������͵Ķ���ת��Ϊ��ת���������͡�
        /// �������ת����֧�ָ����͵�ת��,�򷵻�true
        /// </summary>
        private bool GetConversionSupported(TypeConverter converter, Type conversionType)
        {
            return (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType));
        }

        /// <summary>
        /// ��ȡ"FunctionProperty"�ӽڵ�,�Կؼ���Ϣ�Ĺ������Բ��ֽ��г�ʼ��
        /// </summary>
        /// <param name="node">�������Խڵ�</param>
        /// <param name="CtrlInfo">�ؼ���Ϣ</param>
        /// <returns>�����Ϣ��ֵ��Ŀؼ���Ϣ</returns>
        static public void ReadFunctionProperty(XmlNode node, ref ControlInfo CtrlInfo)
        {
            List<XProp> VFunctionProperty = new List<XProp>();
            List<XProp> UFunctionProperty = new List<XProp>();
            foreach (XmlNode property in node.ChildNodes)
            {
                if (property.Name.Equals("Property"))
                {
                    XProp Function = new XProp();
                    Function.Name = property.Attributes["name"].Value;
                    Function.VarName = property.Attributes["varname"].Value;
                    Function.ValueType = property.Attributes["type"].Value;
                    if (property.Attributes.Count >= 5)
                    {//����5������������Ϊ������ӵĲ���������Ϣ
                        Function.ValueExplain = property.Attributes["exp"].Value;
                        if (property.Attributes.Count >= 6)
                        {//�������Կ�ʼΪ�Ż�����
                            Function.Opnode = property.Attributes["opnode"].Value;
                            Function.Optype = property.Attributes["optype"].Value;
                            Function.Opvalue = property.Attributes["opvalue"].Value;
                            Function.Relate = property.Attributes["relate"].Value;
                            if (Function.Opnode != null || Function.Optype != null || Function.Opvalue != null | Function.Relate != null)
                            {
                                CtrlInfo.CanOptimize = true;
                            }
                        }
                    }
                    if (property.Attributes["visible"].Value == "yes")
                    {
                        if (property.Attributes["type"].Value != "MyEnum"&&property.Attributes["type"].Value!="COM")
                        {//20090709��Ӳ��ֿؼ���ģ������Ĺ������� 
                            object obj = property.InnerText;
                            Convert.ChangeType(obj, Type.GetType(property.Attributes["type"].Value));
                            Function.TheValue = obj;
                        }
                        else if (property.Attributes["type"].Value == "MyEnum")
                        {
                            CtrlInfo.HasEnumProperty = true;
                            Function.EnumValue = property.InnerText;
                            Function.TheValue = property.InnerText.Split(',')[0];
                        }
                        else if (property.Attributes["type"].Value == "COM")  //�����豸�� 2014.1 
                        {
                            CtrlInfo.HasEnumProperty = true;
                            string portNames = CassView.GetSystemSerialPortNames();
                            Function.EnumValue = portNames;
                            string[] portsList = portNames.Split(',');
                            if (portsList.Length > 1)
                            {
                                Function.TheValue = portsList[0];
                            }
                            else
                            {
                                Function.TheValue = portNames;
                            }
                           
                            //if (portLlist.Count > 0)
                            //{
                            //    Function.EnumValue = property.InnerText;
                            //    Function.TheValue = portLlist[0];
                            //}
                            //else
                            //{
                            //    Function.EnumValue = property.InnerText;
                            //    Function.TheValue = property.InnerText;
                            //}
                        }
                        VFunctionProperty.Add(Function);
                    }
                    else
                    {
                        if (property.Attributes["type"].Value != "MyEnum")
                        {
                            object obj = property.InnerText;
                            if (Function.ValueType != "queue" && obj.ToString() != "array")
                            {//20090604����ӵĶ���������λ��������,����ֵ������
                                Convert.ChangeType(obj, Type.GetType(property.Attributes["type"].Value));
                            }
                            Function.TheValue = obj;
                        }
                        else
                        {
                            Function.EnumValue = property.InnerText;
                            Function.TheValue = property.InnerText.Split(',')[0];
                        }
                        UFunctionProperty.Add(Function);
                    }
                }//end if (property.Name.Equals("Property"))
            }//end foreach (XmlNode property in node.ChildNodes)  
            CtrlInfo.VisibleFunctionProperty = VFunctionProperty;
            CtrlInfo.UnvisibleFunctionProperty = UFunctionProperty;
        }

        /// <summary>
        /// ��ȡ"OtherInfo"�ӽڵ�,�Կؼ���Ϣ���������Բ��ֽ��г�ʼ��
        /// </summary>
        /// <param name="node">�������Խڵ�</param>
        /// <param name="CtrlInfo">�ؼ���Ϣ</param>
        /// <returns>�����Ϣ��ֵ��Ŀؼ���Ϣ</returns>
        static public void ReadOtherProperty(XmlNode node, ref ControlInfo CtrlInfo)
        {
            string[] CodeAttribute = new string[3];
            foreach (XmlNode element in node.ChildNodes)
            {//�����ڵ㲻����ؼ���Ϣ��
                if (element.Attributes.Count == 0 || element.Attributes["name"].InnerText == "CodeInfo")
                {
                    CodeAttribute[0] = element.InnerText;
                }
                else if (element.Attributes["name"].InnerText == "OptimizeInfo")
                {
                    CodeAttribute[1] = element.InnerText;
                }
                else if (element.Attributes["name"].InnerText == "Description")
                {
                    CodeAttribute[2] = element.InnerText;
                }
            }
            CtrlInfo.OtherProperty = CodeAttribute;
        }

        /// <summary>
        /// ��������XML�е������ؼ���Ϣ
        /// </summary>
        static public void ClearCtrlsInfo(CassView casv)
        {
            for (int i = casv.ctrlsInfo.Count - 1; i >= 0; i--)
            {
                if (casv.ctrlsInfo[i].ControlNum == -1)
                {//����ؼ����ͳһΪ-1
                    casv.ctrlsInfo.RemoveAt(i);
                }
            }
        }

        #endregion

        #region ���ߺ���
        /// <summary>
        /// ��PortNameList������¿ؼ�����Ϣ�������µ�PortName
        /// </summary>
        /// <param name="control">�ؼ���Ϣ</param>
        /// <returns>�����µ�PortName</returns>
        static public string AddPortName(ControlInfo control, ref List<ArrayList> PInfoList)
        {//û�����ӵĿؼ�û�б�����ؼ�������������������20090702
            string FindPortName = null;
            if (PInfoList.Count != 0)
            {
                for (int i = 0; i < PInfoList.Count; i++)
                {
                    if (((string[])(PInfoList[i][0]))[0] == control.CodeInfo[0])
                    {//�ҵ��Ķ�Ӧ�Ŀؼ���
                        List<string> tempList = (List<string>)(PInfoList[i][1]);//��ʱ�������ڼ�������ת������(��ȥ) 

                        for (int j = 0; j <= tempList.Count; j++)
                        {//�����������ռ���б�
                            if (tempList.Contains(j.ToString()))
                            { continue; }
                            else
                            {
                                FindPortName = j.ToString();
                                int nullIndex = tempList.IndexOf(null);
                                if (nullIndex == -1)
                                {//������û�п�λ,����ڶ���ĩ
                                    ((List<string>)(PInfoList[i][1])).Add(FindPortName);
                                }
                                else
                                {//�п�λ�����,�������
                                    ((List<string>)(PInfoList[i][1]))[nullIndex] = FindPortName;
                                }
                                break;
                            }
                            //if (j == tempList.Count)
                            //{
                            //    FindPortName = j.ToString();
                            //    //((List<string>)(PInfoList[i][1])).Add(control.CodeInfo[1] + FindPortName);20090709
                            //    ((List<string>)(PInfoList[i][1])).Add(FindPortName);
                            //    break;
                            //}
                            //else if (tempList[j] == null)
                            //{
                            //    FindPortName = j.ToString();
                            //    //((List<string>)(PInfoList[i][1]))[j] = control.CodeInfo[1] + FindPortName;20090709
                            //    ((List<string>)(PInfoList[i][1]))[j] = FindPortName;
                            //    break;
                            //}
                        }
                        //if (FindPortName != null)
                        break;
                    }
                }
            }
            if (FindPortName == null)
            {//�ջ��߲����б�,�����ģ����                
                ArrayList newPortInfo = new ArrayList();
                FindPortName = "0";
                newPortInfo.Add(new string[] { control.CodeInfo[0], control.CodeInfo[1] });
                //newPortInfo.Add(new List<string>(new string[] { control.CodeInfo[1] + FindPortName }));20090709
                newPortInfo.Add(new List<string>(new string[] { FindPortName }));
                PInfoList.Add(newPortInfo);
            }
            return FindPortName;
        }

        /// <summary>
        /// �޸���PortInfoList�еĶ�Ӧ����Ϣ
        /// </summary>
        /// <param name="MouleName">ģ����</param>
        /// <param name="OldName">ԭPortName</param>
        /// <param name="ModifyName">��Ҫ�ĳɵ�Ŀ����,null��ʾɾ��</param>
        /// <returns>true��ʾ�޸�ɾ���ɹ�,false��ʾʧ��</returns>
        static public bool ModifyPortName(string ModuleName, string OldName, string ModifyName)
        {
            bool FindPortName = false;
            if (CassViewGenerator.PortInfoList.Count != 0)
            {
                int[] tempNum = new int[2];//����ҵ��滻λ�õ��������(������ź�ͬ�����µĿؼ������)
                for (int i = 0; i < CassViewGenerator.PortInfoList.Count; i++)
                {
                    string typeName = ((string[])(CassViewGenerator.PortInfoList[i][0]))[1];

                    if (ModuleName == typeName)
                    {
                        List<string> tempList = (List<string>)(CassViewGenerator.PortInfoList[i][1]);//��ʱ�������ڼ�������ת������(��ȥ) 

                        if (ModifyName != null && tempList.Contains(ModifyName))//�޸��������������޸�ʧ��
                        {
                            return false;
                        }
                        else if (tempList.Contains(OldName))//ȷ�ϴ���ԭPortname
                        {
                            tempNum[0] = i;
                            tempNum[1] = tempList.IndexOf(OldName);
                            FindPortName = true;                            
                        }
                        break;
                    }
                }
                if (FindPortName)
                {
                    ((List<string>)(CassViewGenerator.PortInfoList[tempNum[0]][1]))[tempNum[1]] = ModifyName;
                    for (int j = ((List<string>)(CassViewGenerator.PortInfoList[tempNum[0]][1])).Count - 1; j >= tempNum[1]; j--)
                    {//���β����NULL
                        if (((List<string>)(CassViewGenerator.PortInfoList[tempNum[0]][1]))[j] == null)
                            ((List<string>)(CassViewGenerator.PortInfoList[tempNum[0]][1])).RemoveAt(j);
                        else
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ��ʼ���ؼ��˿���Ϣ
        /// </summary>
        /// <param name="Num">�ؼ�����������������</param>
        /// <returns>��ʼ��������</returns>
        static public List<string[]> InitializeIOinfo(int Num)
        {
            if (Num != 0)
            {
                List<string[]> TempInfo = new List<string[]>();
                for (int i = 0; i < Num; i++)
                {
                    string[] IOinfo = new string[2];
                    TempInfo.Add(IOinfo);
                }
                return TempInfo;
            }
            else
            {
                return new List<string[]>();
            }
        }

        /// <summary>
        /// ����ԭ�пؼ��˿���Ϣ��ʼ��
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="OldInfo"></param>
        /// <returns></returns>
        static public List<string[]> InitializeIOinfo(int Num, List<string[]> OldInfo)
        {
            if (Num != 0)
            {
                List<string[]> TempInfo = new List<string[]>();
                for (int i = 0; i < Num; i++)
                {
                    string[] IOinfo = new string[4];
                    if (i < OldInfo.Count && OldInfo[i].Length == 4)
                    {
                        if (i < OldInfo.Count)
                        {
                            IOinfo[2] = OldInfo[i][2];
                            IOinfo[3] = OldInfo[i][3];
                        }
                        else
                        {
                            IOinfo[2] = OldInfo[OldInfo.Count - 1][2];
                            IOinfo[3] = OldInfo[OldInfo.Count - 1][3];
                        }
                    }
                    else
                    { IOinfo = new string[] { null, null, null, "0" }; }
                    TempInfo.Add(IOinfo);
                }
                return TempInfo;
            }
            else
            {
                return new List<string[]>();
            }
        }

        /// <summary>
        /// �жϸÿؼ��Ƿ��ڵ�ǰCassview�д��ڣ������򷵻��ڿؼ������е����
        /// </summary>
        /// <param name="controlName">��Ҫ���ҵĿؼ���</param>
        /// <returns>���ҿؼ��ڿؼ������е���ţ������ڷ���-1</returns>
        public int FindControlName(string controlName)
        {
            for (int i = 0; i < this.ctrlsInfo.Count; i++)
            {
                if (this.ctrlsInfo[i].ControlName == controlName)
                { return i; }
            }
            return -1;
        }

        /// <summary>
        /// ���ݿؼ����������д��ڵĿؼ���Ϣ���������򷵻ؿ���Ϣ
        /// </summary>
        /// <param name="elementCtrl"></param>
        /// <returns></returns>
        public ControlInfo FindControlInfo(Control elementCtrl)
        {
            PropertyDescriptor PN = TypeDescriptor.GetProperties(elementCtrl)["PortName"];
            string controlname = elementCtrl.Tag.ToString();
            for (int i = 0; i < this.ctrlsInfo.Count; i++)
            {
                //string temp = PN.GetValue(elementCtrl).ToString();
                if ((PN != null && this.ctrlsInfo[i].CodeInfo != null && this.ctrlsInfo[i].CodeInfo[2] == PN.GetValue(elementCtrl).ToString()) 
                    || (controlname != null && controlname == ctrlsInfo[i].ControlName))
                {
                    return this.ctrlsInfo[i];
                }
            }
            return new ControlInfo();
        }

        /// <summary>
        /// ���ݿؼ���Ϣ�ҵ���Ӧ�Ŀ���Ϣ���������򷵻ؿ���Ϣ
        /// </summary>
        /// <param name="ctrlinfo"></param>
        /// <returns></returns>
        public BlockInfo FindBlockInfo(ControlInfo ctrlinfo)
        {
            foreach (BlockInfo element in this.blocksInfo)
            {
                if (element.BlockNum == ctrlinfo.ControlNum)
                {
                    return element;
                }
            }
            return new BlockInfo();
        }

        ///// <summary>
        ///// �жϵ�ǰCassview���Ƿ��������ؼ�
        ///// </summary>
        ///// <param name="portSort">��Ҫ���ҿؼ���ģ������</param>
        ///// <returns>���ؿؼ�����</returns>
        //public List<Control> FindSpecialControl(string portSort)
        //{
        //    foreach (int i = 0; i < this.Controls.Count; i++)
        //    {
        //        PropertyDescriptor moduleSortProperty = TypeDescriptor.GetProperties(control)["ModuleSort"];
        //        if (this.ctrlsInfo[i].CodeInfo[2] == portName)
        //        { return i; }
        //    }
        //    return -1;
        //}

        /// <summary>
        /// ˢ�¿ؼ��ĳ����������Ϣ
        /// </summary>
        public void portReflash()
        {   //������пؼ��������������Ϣ 
            for (int i = 0; i < this.ctrlsInfo.Count; i++)
            {
                ControlInfo tempCtrlInfo = this.ctrlsInfo[i];
                if (tempCtrlInfo.OutputInfo != null)
                {
                    int Onum = tempCtrlInfo.OutputInfo.Count;
                    tempCtrlInfo.OutputInfo = InitializeIOinfo(Onum);
                }
                if (tempCtrlInfo.InputInfo != null)
                {
                    int Inum = tempCtrlInfo.InputInfo.Count;
                    tempCtrlInfo.InputInfo = InitializeIOinfo(Inum, tempCtrlInfo.InputInfo);
                }
                tempCtrlInfo.IsConnect = false;
                this.ctrlsInfo.RemoveAt(i);
                this.ctrlsInfo.Insert(i, tempCtrlInfo);
            }
            //����ֱ��������Ϣ���¶Կؼ��������Ϣ��ֵ
            foreach (LineInfo line in this.linesInfo)
            {
                string StartCtrl = line.StartInfo.Split(';')[0];
                int StartNum = Convert.ToInt32(line.StartInfo.Split(';')[1].Split(',')[1]);
                string EndCtrl = line.EndInfo.Split(';')[0];
                int EndNum = Convert.ToInt32(line.EndInfo.Split(';')[1].Split(',')[1]);
                bool FindStart = false;
                bool FindEnd = false;

                for (int i = 0; i < this.ctrlsInfo.Count; i++)
                {
                    ControlInfo tempCtrlInfo = this.ctrlsInfo[i];
                    if (tempCtrlInfo.ControlName == StartCtrl)
                    {//�������Ϣ�迼�ǵ��������������
                        if (tempCtrlInfo.OutputInfo[StartNum][0] == null)
                        {
                            tempCtrlInfo.OutputInfo[StartNum][0] = EndCtrl + "." + EndNum.ToString();
                        }
                        else
                        {
                            tempCtrlInfo.OutputInfo[StartNum][0] += "," + EndCtrl + "." + EndNum.ToString();
                        }
                        tempCtrlInfo.IsConnect = true;
                        this.ctrlsInfo.RemoveAt(i);
                        this.ctrlsInfo.Insert(i, tempCtrlInfo);
                        FindStart = true;
                    }
                    else if (tempCtrlInfo.ControlName == EndCtrl)
                    {
                        tempCtrlInfo.InputInfo[EndNum][0] = StartCtrl + "." + StartNum.ToString();
                        tempCtrlInfo.IsConnect = true;
                        this.ctrlsInfo.RemoveAt(i);
                        this.ctrlsInfo.Insert(i, tempCtrlInfo);
                        FindEnd = true;
                    }
                    if (FindStart && FindEnd)
                    { break; }
                }
            }
        }

        /// <summary>
        /// ����һ���ؼ���Ϣ
        /// </summary>
        /// <param name="templet"></param>
        /// <returns></returns>
        static public ControlInfo CopyCtrlinfo(ControlInfo templet)
        {
            ControlInfo NewCtrl = new ControlInfo();
            NewCtrl.CodeInfo = new string[3];
            templet.CodeInfo.CopyTo(NewCtrl.CodeInfo, 0);
            NewCtrl.OtherProperty = templet.OtherProperty;
            NewCtrl.ControlName = templet.ControlName;
            NewCtrl.ControlNum = templet.ControlNum;
            NewCtrl.HasEnumProperty = templet.HasEnumProperty;
            NewCtrl.CanOptimize = templet.CanOptimize;
            NewCtrl.InputInfo = templet.InputInfo;
            NewCtrl.OutputInfo = templet.OutputInfo;
            NewCtrl.SortName = templet.SortName;
            NewCtrl.IsConnect = templet.IsConnect;

            NewCtrl.UnvisibleFunctionProperty = CopyProperty(templet.UnvisibleFunctionProperty);

            NewCtrl.VisibleFunctionProperty = CopyProperty(templet.VisibleFunctionProperty);
            return NewCtrl;
        }

        /// <summary>
        /// ���ݹ�������
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        static private List<XProp> CopyProperty(List<XProp> properties)
        {
            List<XProp> copyP = new List<XProp>();
            foreach (XProp element in properties)
            {
                XProp newPerty = new XProp();
                newPerty.EnumValue = element.EnumValue;
                newPerty.Name = element.Name;
                newPerty.Opnode = element.Opnode;
                newPerty.Optype = element.Optype;
                newPerty.Opvalue = element.Opvalue;
                newPerty.Relate = element.Relate;
                newPerty.TheValue = element.TheValue;
                newPerty.ValueExplain = element.ValueExplain;
                newPerty.ValueType = element.ValueType;
                newPerty.VarName = element.VarName;
                copyP.Add(newPerty);
            }
            return copyP;
        }


        /// <summary>
        /// ��ԭ��ǰҳ�����пؼ�����ɫ
        /// �������пؼ���isError���Ա��false
        /// </summary>
        public void colorReflash()
        {
            //�ѵ�ǰ�����Ա��� �ٸ�ֵ �������Խ������ٻ�ԭ
            //����ģʽ�ؼ�VISIBLE����Ϊfalse
            bool curValue = this.LinesEditable;
            this.LinesEditable = false;
            for (int x = 0; x < this.Controls.Count; x++)
            {
                PropertyDescriptor IsError = TypeDescriptor.GetProperties(this.Controls[x])["IsError"];
                IsError.SetValue(this.Controls[x], false);
            }
            this.LinesEditable = curValue;
        }


        #endregion

        #region ������Ϣ

        /// <summary>
        /// ��ȡXML���¿ؼ���Ϣ
        /// ������ʱ����XML������޸Ĺ�������ʱ�򿪹��̺����
        /// ������ڳ�ֵ���и���20090618
        /// </summary>
        public void UpdateCtrlInfo()
        {
            for (int i = 0; i < this.ctrlsInfo.Count; i++)
            {
                bool Update = false;
                foreach (XmlNode categoryNode in ToolBoxServiceImpl.toolXML.FirstChild.ChildNodes)
                {
                    if (categoryNode != null && categoryNode.Attributes[0].InnerText != CassViewGenerator.SpecialCodeNode)
                    {
                        foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                        {
                            if (ctrlsInfo[i].SortName == toolItemNode.Attributes[0].InnerText.Split(',')[2]
                                && toolItemNode.FirstChild != null)
                            {
                                ControlInfo tempCtrl = this.ctrlsInfo[i];
                                //��ȡ�������ԣ�����ģ����ӹ�������
                                if (toolItemNode.FirstChild != null && toolItemNode.FirstChild.NextSibling != null
                                    && toolItemNode.FirstChild.NextSibling.ChildNodes.Count > 0)
                                {
                                    //��������˿���Ϣ
                                    tempCtrl.InputInfo = UpdateInputValue(toolItemNode.FirstChild, tempCtrl.InputInfo);

                                    List<XProp> VFunctionProperty = new List<XProp>();
                                    List<XProp> UFunctionProperty = new List<XProp>();
                                    foreach (XmlNode property in toolItemNode.FirstChild.NextSibling.ChildNodes)
                                    {
                                        if (property.Name.Equals("Property"))
                                        {
                                            if (property.Attributes["visible"].Value == "yes")
                                            {
                                                VFunctionProperty.Add(FindFunctionProperty(property, tempCtrl.VisibleFunctionProperty, true));
                                            }
                                            else
                                            {
                                                UFunctionProperty.Add(FindFunctionProperty(property, tempCtrl.UnvisibleFunctionProperty, false));
                                            }
                                        }//end if (property.Name.Equals("Property"))
                                    }//end foreach (XmlNode property in node.ChildNodes)  
                                    tempCtrl.VisibleFunctionProperty = VFunctionProperty;
                                    tempCtrl.UnvisibleFunctionProperty = UFunctionProperty;
                                }//end if (toolItemNode.FirstChild != null)
                                //��ȡָ�����ԣ�������������Control.Cʱ����
                                if ((toolItemNode.LastChild.Name == "CodeProperty" || toolItemNode.LastChild.Name == "OtherInfo") 
                                    && toolItemNode.LastChild != null && toolItemNode.LastChild.HasChildNodes)
                                {
                                    ReadOtherProperty(toolItemNode.LastChild,ref tempCtrl);
                                }
                                ctrlsInfo[i] = tempCtrl;
                                Update = true;
                                break;
                            }//end if (moduleSort == toolItemNode.Attributes[0].InnerText.Split(',')[2])
                        }// end foreach(XmlNode toolItemNode in categoryNode.ChildNodes)
                    }
                    if (Update)
                        break;
                }
            }
        }

        /// <summary>
        /// ��������˿ڳ�ֵ ���Ҳ��ɾ���˿�
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="InputInfo"></param>
        /// <returns></returns>
        private List<string[]> UpdateInputValue(XmlNode Property, List<string[]> InputInfo)
        {
            foreach (XmlNode node in Property.ChildNodes)
            {
                if (node.Attributes["name"].Value == "InputName")
                {
                    if (node.ChildNodes.Count <= 1)
                    { return InputInfo; }
                    else
                    {
                        foreach (XmlNode element in node.ChildNodes)
                        {//��������˿���Ϣ
                            for (int i = 0; i < InputInfo.Count; i++)
                            {
                                if (InputInfo[i].Length == 4 && element.Attributes["name"].Value == InputInfo[i][2])
                                {//�˿�����ͬ��ֵ�˿ڳ�ֵ
                                    InputInfo[i][3] = element.InnerText;
                                    break;
                                }
                            }
                        }
                        return InputInfo;
                    }
                }
            }
            return InputInfo;
        }

        /// <summary>
        /// ���ݽڵ���Ϣ�ҵ���ǰ�������Լ������Ƿ��ж�Ӧ������
        /// û������ݽڵ���Ϣ����������
        /// ���ɼ�����������Valueֵ��ƥ�� �������޸�
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="FuntionInfos"></param>
        /// <returns>�����ҵ�����</returns>
        private XProp FindFunctionProperty(XmlNode Property, List<XProp> FuntionInfos, bool visible)
        {
            //Ѱ��ƥ����Ϣ
            foreach (XProp element in FuntionInfos)
            {
                if (element.Name == Property.Attributes["name"].Value
                    && element.VarName == Property.Attributes["varname"].Value
                    && element.ValueType == Property.Attributes["type"].Value)
                {                    
                    if (Property.Attributes.Count >= 5)
                    {//����������Ϣ
                        element.ValueExplain = Property.Attributes["exp"].Value;
                        if (Property.Attributes.Count >= 6)
                        {//�����Ż�������Ϣ
                            element.Opnode = Property.Attributes["opnode"].Value;
                            element.Optype = Property.Attributes["optype"].Value;
                            element.Opvalue = Property.Attributes["opvalue"].Value;
                            element.Relate = Property.Attributes["relate"].Value;
                        }
                    }
                    if (Property.Attributes["type"].Value != "MyEnum")
                    {
                        if (!visible)
                        {//���ɼ�����ֱ�Ӹ���ֵ
                            element.TheValue = Property.InnerText;                            
                        }                        
                        return element;
                    }
                    else if (element.EnumValue == Property.InnerText)
                    {
                        if (!visible)
                        {
                            element.TheValue = Property.InnerText.Split(',')[0];
                        }
                        return element;
                    }
                }
            }
            //��������Ϣ
            XProp newFun = new XProp();
            newFun.Name = Property.Attributes["name"].Value;
            newFun.VarName = Property.Attributes["varname"].Value;
            newFun.ValueType = Property.Attributes["type"].Value;
            if (Property.Attributes.Count >= 5)
            {
                newFun.ValueExplain = Property.Attributes["exp"].Value;
                if (Property.Attributes.Count >= 6)
                {
                    newFun.Opnode = Property.Attributes["opnode"].Value;
                    newFun.Optype = Property.Attributes["optype"].Value;
                    newFun.Opvalue = Property.Attributes["opvalue"].Value;
                    newFun.Relate = Property.Attributes["relate"].Value;
                }
            }
            if (newFun.ValueType != "MyEnum")
            {
                object obj = Property.InnerText;
                if (newFun.ValueType != "queue" && obj.ToString() != "array")
                {
                    Convert.ChangeType(obj, Type.GetType(newFun.ValueType));
                }
                newFun.TheValue = obj;
            }
            else if (newFun.ValueType == "COM")
            {
              //����  ���� 2014.1
               string portNames = CassView.GetSystemSerialPortNames();
               newFun.EnumValue = portNames;
               string[] portsList = portNames.Split(',');
               if (portsList.Length > 1)
               {
                   newFun.TheValue = portsList[0];
               }
               else  //����ֻ��һ��������
               {
                   newFun.TheValue = portNames;
               }
            }
            else
            {
                newFun.EnumValue = Property.InnerText;
                newFun.TheValue = Property.InnerText.Split(',')[0];
            }
            return newFun;
        }

        #endregion
        /// <summary>
        /// ��ȡϵͳ������
        /// </summary>
        /// <returns>string</returns>
        public static string GetSystemSerialPortNames()
        {
            //List<string> portLlist = new List<string>();
            //string sysSerialPort = null;
            ////��ȡϵͳ��ǰ���ں�
            //foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
            //{
            //    portLlist.Add(portName);
            //    sysSerialPort = sysSerialPort + portName + ",";
            //}
            //sysSerialPort = sysSerialPort.Substring(0, sysSerialPort.Length - 1);

            string sysSerialPort = String.Join(",", System.IO.Ports.SerialPort.GetPortNames());
           
            return sysSerialPort;
        }
        
    }
}