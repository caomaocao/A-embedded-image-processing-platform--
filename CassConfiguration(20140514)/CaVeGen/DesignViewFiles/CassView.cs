/*******************************************************************************
           ** Copyright (C) 2006 CASS 版权所有
           ** 文件名：CassView.cs 
           ** 功能描述：
           **          用于显示设计器的背景，拖拉后的控件将添加到该控件的容器内。
 *                     并把鼠标在该控件上的位置显示在主页面上。增加了键盘设置页面属性。
           ** 作者：吴丹红
           ** 创始时间：2006-11-30
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
        /// 算术类模块0位为模块类,1位为模块类序号,2位为模块名
        /// 输入输出类模块1位为目标模块名，2位为模块属性
        /// </summary>
        public string[] CodeInfo;

        public bool HasEnumProperty;
        public bool CanOptimize;
        public List<XProp> VisibleFunctionProperty;
        public List<XProp> UnvisibleFunctionProperty;

        public string[] OtherProperty;

        /// <summary>
        /// 控件是否被连接
        /// </summary>
        public bool IsConnect;
        //每个输入输出口有两个信息:0位为对应的直线另一端的控件;1位为使用到的临时变量
        public List<string[]> InputInfo;
        public List<string[]> OutputInfo;
    }

    public partial class CassView : UserControl
    {
        #region 容器属性集合

        public List<ControlInfo> CopyCtrl = new List<ControlInfo>();//复制的控件信息副本，粘贴后清空
        public List<ControlInfo> ctrlsInfo = new List<ControlInfo>();
        public List<BlockInfo> blocksInfo = new List<BlockInfo>();
        public List<LineInfo> linesInfo = new List<LineInfo>();
        private int recordControls = 0;
        private Image backgroundImage = null;//页面背景图片
        private List<List<ArrayList>> InfosList = new List<List<ArrayList>>();
        private bool isScale = false;//标示符当缩放控件时不记录控件移动
        public  bool UndoFlag = false;//控件操作 撤销判定


        //注：
        //现在PortInfoList与CtrlsInfo的关系是单向查找，控件属性中的CodeInfo里的信息可以查找到对应的PortInfo中的信息
        //但是反向查找很繁琐 并且PortInfoList是外界变量 并非所属单一的Cassview类中

        /// <summary>
        /// 模块类名集合
        /// </summary>
        [Category("基本属性")]
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
        /// 已记录序号的控件数
        /// </summary>
        [Category("基本属性")]
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
        /// 隐藏控件BackgroundImage属性
        /// </summary>        
        [Browsable(false)]
        [Category("基本属性")]
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

        [Category("基本属性")]
        [DisplayName("页面大小")]
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

        [Category("基本属性")]
        [DisplayName("背景颜色")]
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

        [Category("基本属性")]
        [DisplayName("信息集合")]
        public List<List<ArrayList>> infosList
        {
            get
            {
                if (this.ctrlsInfo.Count != 0 && this.blocksInfo.Count != 0
                    && this.ctrlsInfo.Count == this.recordControls)
                {//当容器中控件信息非空才进行信息保存,全删除控件时会出现bug20090618修正在删除控件时
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

        #region 信息转换函数

        /* 用于存储用的List<ArrayList>格式的信息数据和
         * Cassview中使用的控件信息ctrlsInfo
         * 连线使用的控件和连线信息blockInfo、linesInfo
         * 数据之间的转换 */

        /// <summary>
        /// 从信息数组中读取信息
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
                //基本属性
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
                {//增加输入端口保存 端口初值信息20090616
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
                {//可见属性
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
                {//隐藏属性
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
                //位置、大小信息
                tempBlock.StartPoint = SizeInfo[0];
                tempBlock.Width = SizeInfo[1].X;
                tempBlock.Height = SizeInfo[1].Y;

                this.ctrlsInfo.Add(tempCtrl);
                this.blocksInfo.Add(tempBlock);
            }
            for (int i = 0; i < line.Count; i++)
            {//连线信息
                LineInfo tempLine = new LineInfo();

                Point[] SEinfo = (Point[])(line[i][0]);
                string[] SEctrl = (string[])(line[i][1]);
                tempLine.StartPoint = SEinfo[0];
                tempLine.EndPoint = SEinfo[1];
                tempLine.StartInfo = SEctrl[0];
                tempLine.EndInfo = SEctrl[1];

                this.linesInfo.Add(tempLine);
            }
            portReflash();//刷新控件端口信息
            //重新计算连线中间点
            DrawLineArithmetic newArith = new DrawLineArithmetic(this.linesInfo, this.blocksInfo);
            this.linesInfo = newArith.ModifyLines;

        }

        /// <summary>
        /// 把控件信息、方块信息和直线信息保存在信息集合中
        /// </summary>
        /// <returns></returns>
        private List<List<ArrayList>> SaveInfo()
        {
            List<List<ArrayList>> ReturnInfo = new List<List<ArrayList>>();
            List<ArrayList> FirstInfo = new List<ArrayList>();
            List<ArrayList> SecondInfo = new List<ArrayList>();
            //第一部分信息
            for (int i = 0; i < this.ctrlsInfo.Count; i++)
            {
                ArrayList controlInfo = new ArrayList();
                //基本信息
                string[] BasicInfo = new string[11];
                BasicInfo[0] = this.ctrlsInfo[i].ControlNum.ToString();
                BasicInfo[1] = this.ctrlsInfo[i].ControlName;
                BasicInfo[2] = this.ctrlsInfo[i].SortName;
                if (this.ctrlsInfo[i].OtherProperty != null)
                {//条件动作表和计算器组态
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

                //功能属性信息
                if (this.ctrlsInfo[i].VisibleFunctionProperty != null)
                {//可视属性
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
                {//隐藏属性
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
                //位置大小信息
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
            //连线信息
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
        /// CassView类构造函数
        /// </summary>
        public CassView()
        {
            InitializeComponent();
            this.ControlRemoved += new ControlEventHandler(CassView_ControlRemoved);
        }

        #region 直线操作

        private bool isPortPoint = false;
        /// <summary>
        /// 判断输入坐标是否在模块引脚上
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
        /// 记录用户当前选择的连线
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
        /// 清除鼠标移过的点
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
        /// 鼠标移过点的坐标
        /// </summary>
        private bool showMouseMovePoint = false;        //是否显示鼠标移过点的标记变量
        private Point mouseMoveShowPoint = new Point(0, 0);     //鼠标移过的显示点坐标
        private Point mouseMovePoint = new Point(0, 0);     //鼠标移过点的坐标
        [Browsable(false)]
        public Point MouseMovePoint
        {
            get
            {
                return this.mouseMovePoint;
            }
            set
            {
                if (this.getFirstPoint == true)     //鼠标之前以选中某个合法模块引脚
                {
                    this.mouseMovePoint = this.PointToClient(value);        //把鼠标位置转化为相对设计器的坐标
                    this.showMouseMovePoint = false;        //设置记录鼠标是否移动到连线另一合法端点的标志为false

                    foreach (Control obj in this.Controls)      //依次遍历页面的所有控件
                    {
                        //把鼠标当前坐标传递给各控件
                        PropertyDescriptor clickPointProperty = TypeDescriptor.GetProperties(obj)["MouseMovePoint"];
                        if (clickPointProperty != null)
                        {
                            clickPointProperty.SetValue(obj, this.mouseMovePoint);
                        }//end if (clickPointProperty != null)
                        else
                        {
                            continue;
                        }//end else if (clickPointProperty == null)

                        //判断鼠标当前位置是否在模块的引脚上
                        PropertyDescriptor checkPointProperty = TypeDescriptor.GetProperties(obj)["CheckPoint"];
                        if (checkPointProperty != null)
                        {
                            if ((bool)checkPointProperty.GetValue(obj) == true)     //在模块的引脚上
                            {
                                PropertyDescriptor pointInformationProperty = TypeDescriptor.GetProperties(obj)["PointInformation"];
                                if (pointInformationProperty != null)
                                {
                                    string pointInformation = (string)(pointInformationProperty.GetValue(obj));

                                    if (pointInformation != null && pointInformation != ""
                                        && this.preClickInformation[0] != pointInformation[0] //输出对输入
                                        && this.preClickControl != obj) //控件不相同
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
                                //把当前鼠标位置修正至引脚的正确位置，鼠标位置可能与正确位置有象素偏差

                            }//end if ((bool)checkPointProperty.GetValue(obj) == true)
                        }//end if (checkPointProperty != null)
                    }//end foreach (Control obj in this.Controls) 
                    this.Refresh();
                }//end if (this.getFirstPoint == true)
            }
        }

        private Point mouseUpPoint = new Point(0, 0);
        /// <summary>
        /// 用户鼠标双击时的坐标
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
                if (this.getFirstPoint == true)     //鼠标之前以选中某个合法模块引脚
                {
                    this.isPortPoint = false;
                    this.mouseUpPoint = this.PointToClient(value);      //转化鼠标位置

                    foreach (Control obj in this.Controls)      //依次遍历页面组件
                    {
                        //把鼠标弹起时的坐标参数传递给各模块
                        PropertyDescriptor clickPointProperty = TypeDescriptor.GetProperties(obj)["ClickPoint"];
                        if (clickPointProperty != null)
                        {
                            clickPointProperty.SetValue(obj, this.mouseUpPoint);
                        }//end if(clickPointProperty != null)
                        else
                        {
                            continue;
                        }//end else if (clickPointProperty != null)

                        //判断该坐标是否在模块的合法引脚上
                        PropertyDescriptor checkPointProperty = TypeDescriptor.GetProperties(obj)["CheckPoint"];
                        if (checkPointProperty != null)
                        {
                            this.isPortPoint = (bool)checkPointProperty.GetValue(obj);
                            if (this.isPortPoint == true)       //点在引脚上
                            {
                                //获取引脚信息，输入引脚/输出引脚，引脚序号等
                                PropertyDescriptor pointInformationProperty = TypeDescriptor.GetProperties(obj)["PointInformation"];
                                if (pointInformationProperty != null)
                                {
                                    string pointInformation = (string)(pointInformationProperty.GetValue(obj));
                                    if (pointInformation == null || pointInformation == "")     //获取到非法信息
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

                                    //两连线的端口类型一致，同为输入或输出端口,或在同一控件上
                                    if (this.preClickInformation[0] == pointInformation[0] || this.preClickControl.Site.Name == obj.Site.Name)
                                    {
                                        //释放选中的两个引脚，把引脚状态由选中改为未选中
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
                                    {//两引脚的端口类型不一致
                                        //连线两端的引脚类型合法，绘制连线，记录端点
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
                                                if (this.preClickInformation[0] == 'O')      //第一个选中点为输出引脚，记录新的连线信息
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

                                                    if (hasLine == true)        //判断连线是否已经存在，若存在则释放本次连线
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
                                                    else        //记录新的连线信息，包括相对坐标和绝对坐标已经连线的组件名称
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
                                                        {//添加操作信息
                                                            Operation lineOpt = new Operation();
                                                            lineOpt.Oname = UndoOperation.OperateStyle.直线添加.ToString();
                                                            lineOpt.Item = this.linesInfo[this.linesInfo.Count - 1];//新加直线在列表末端
                                                            CassViewGenerator.AddOpt(lineOpt);
                                                        }
                                                        this.mouseUpPoint = newLine.EndPoint;
                                                        this.mouseDownPoint = newLine.StartPoint;
                                                    }//end else if (hasLine == false)
                                                }//end if (modifiedPoint.X + obj.Location.X > this.preClickPoint.X + this.preClickControl.Location.X)
                                                else      //第一个选中点为输入引脚，记录新的连线信息
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

                                                    if (hasLine == true)        //该连线已经存在
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
                                                    else        //记录连线信息
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
                                                        {//添加操作信息
                                                            Operation lineOpt = new Operation();
                                                            lineOpt.Oname = UndoOperation.OperateStyle.直线添加.ToString();
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
        /// 用户鼠标双击时的坐标
        /// </summary>
        private Point mouseDownPoint = new Point(0, 0);
        private Control preClickControl = null;     //记录连线两端端点的起始端点对应的模块
        private Point preClickPoint = new Point(0, 0);      //记录连线两端端点的起始端点
        private string preClickInformation = "";        //记录连线两端端点的起始端点的基本信息
        private bool getFirstPoint = false;     //标记是否已获得起始端点坐标
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

                #region 绘制新的连线
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

                #region 选择已绘制的连线
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
        /// 指示当前的模块端口连线是否属于可连接状态
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
        /// 记录页面模块的缩放比例，大小范围为原始大小的0.2～2倍
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
                            scaleOpt.Oname = UndoOperation.OperateStyle.比例缩小.ToString();
                        }
                        else
                        {
                            scaleOpt.Oname = UndoOperation.OperateStyle.比例放大.ToString();
                        }
                        scaleOpt.Change = new object[2];
                        scaleOpt.Change[0] = this.scaling;
                        scaleOpt.Change[1] = value;
                        CassViewGenerator.AddOpt(scaleOpt);
                    }

                    isScale = true;//缩放控件引起的控件移动不加入操作队列
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
                    //修改直线起点和终点坐标后重新计算直线路径
                    DrawLineArithmetic newArith = new DrawLineArithmetic(this.linesInfo, this.blocksInfo);
                    this.linesInfo = newArith.ModifyLines;
                    DrawBackgroundImage();
                }
            }

        }

        /// <summary>
        /// 删除选择的线条,并释放直线两端点
        /// </summary>
        /// <param name="LineNum">需要删除的直线序号</param>
        /// <param name="CanClear">是否要更新端口信息</param>
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
                {//添加操作信息
                    Operation lineOpt = new Operation();
                    lineOpt.Oname = UndoOperation.OperateStyle.直线删除.ToString();
                    lineOpt.Item = this.linesInfo[LineNum];
                    CassViewGenerator.AddOpt(lineOpt);
                }
                this.linesInfo.RemoveAt(LineNum);
                if (CanClear)
                {//更新输入输出信息
                    portReflash();
                    DrawBackgroundImage();
                }
                this.choosedLine = -1;
            }
        }

        /// <summary>
        /// 创建一条连线时 把连线的起点、终点、及对应的模块放入LineInfo的结构体里
        /// </summary>
        /// <param name="ControlName">改变位置的控件名</param>
        /// <param name="ChangeX">X轴的位移</param>
        /// <param name="ChangeY">Y轴的位移</param>
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
        /// 根据当前缩放比率对线段的起点和终点进行缩放
        /// </summary>
        /// <param name="oldLine">缩放前直线信息</param>
        /// <returns>缩放后直线信息</returns>
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

        #region 容器操作

        /// <summary>
        /// 控件背景颜色改变事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void CassView_BackColorChanged(object sender, EventArgs e)
        {
            foreach (Control obj in this.Controls)
            {
                obj.BackColor = this.BackColor;
            }
            DrawBackgroundImage();
        }

        /// <summary>
        /// 控件刷新事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void CassView_Paint(object sender, PaintEventArgs e)
        {
            bool drawLine = false;

            //红色起点
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

            //选择线
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

            //移动点
            if (this.showMouseMovePoint == true && drawLine == false)
            {
                Pen myPen = new Pen(Color.Red, 2);
                e.Graphics.DrawRectangle(myPen, this.mouseMoveShowPoint.X - 5, this.mouseMoveShowPoint.Y - 5, 9, 9);
                this.showMouseMovePoint = false;//20090601
            }

            //黄线
            if (this.getFirstPoint == true && (this.mouseMovePoint.X != 0 || this.mouseMovePoint.Y != 0))
            {
                e.Graphics.DrawLine(Pens.Yellow, this.mouseDownPoint, this.mouseMovePoint);
            }
        }

        /// <summary>
        /// 页面大小改变事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void CassView_Resize(object sender, EventArgs e)
        {
            DrawBackgroundImage();
        }

        /// <summary>
        /// 绘制背景图片
        /// </summary>
        public void DrawBackgroundImage()//20090601 改成对外
        {
            this.backgroundImage = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(this.backgroundImage);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(this.BackColor);
            if (this.linesEditable == true || CassViewGenerator.isUndo)
            {//撤销时也可进入
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

        #region 控件操作

        static public List<string> OnlyOneOut = new List<string>(new string[] { "常数", "系统变量", "输入变量", "AD转化", "文件输入", "特征输入","设备输入" });//只有一个输出口
        static public List<string> OnlyOneIn = new List<string>(new string[] { "输出变量", "跳转", "调用", "返回", "DA转化","文件输出" });//只有一个输入口
        

        /// <summary>
        /// 子控件位置变化事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
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
        /// 创建一个控件时 把控件本身的属性信息放入 ControlInfo的结构体里
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
                    //控件移动操作
                    Operation blockOpt = new Operation();
                    blockOpt.Oname = UndoOperation.OperateStyle.控件移动.ToString();
                    blockOpt.Item = ChangeControl;
                    blockOpt.Change = new object[2];
                    blockOpt.Change[0] = blocksInfo[i];//变动前的快信息

                    BlockInfo tempCtrlInfo = this.blocksInfo[i];

                    if (this.blocksInfo[i].StartPoint == new Point(0, 0))//初次赋值
                    {
                        tempCtrlInfo.StartPoint = new Point(ChangeControl.Location.X - 5, ChangeControl.Location.Y - 5);
                        tempCtrlInfo.Width = ChangeControl.Width + 10;
                        tempCtrlInfo.Height = ChangeControl.Height + 10;

                        blockOpt.Change[1] = tempCtrlInfo;//变动后的块信息
                        this.blocksInfo.RemoveAt(i);
                        this.blocksInfo.Insert(i, tempCtrlInfo);
                    }
                    else if (ChangeControl.Location != this.blocksInfo[i].StartPoint)//控件位置改变
                    {
                        int changeX = 0;
                        int changeY = 0;
                        changeX = ChangeControl.Location.X - 5 - tempCtrlInfo.StartPoint.X;
                        changeY = ChangeControl.Location.Y - 5 - tempCtrlInfo.StartPoint.Y;
                        tempCtrlInfo.StartPoint = new Point(ChangeControl.Location.X - 5, ChangeControl.Location.Y - 5);
                        //控件大小也要更新，出现可变大小的控件20090610
                        tempCtrlInfo.Width = ChangeControl.Width + 10;
                        tempCtrlInfo.Height = ChangeControl.Height + 10;

                        blockOpt.Change[1] = tempCtrlInfo;//变动后的块信息
                        this.blocksInfo.RemoveAt(i);
                        this.blocksInfo.Insert(i, tempCtrlInfo);
                        LineInfoChanged(tempCtrlInfo.BlockName, changeX, changeY);
                    }
                    if (CassViewGenerator.currentUndo != null && !isScale && !CassViewGenerator.isUndo)
                    {// 缩放时不记录控件移动信息
                        UndoFlag = true;
                        CassViewGenerator.AddOpt(blockOpt);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// CassView子控件移除事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void CassView_ControlRemoved(object sender, ControlEventArgs e)
        {
            List<object> tempChange = new List<object>();//操作记录删除的控件信息和块信息

            string removeControlName = (string)(e.Control.Tag);

            for (int i = 0; i < this.ctrlsInfo.Count; i++)
            {
                if (ctrlsInfo[i].ControlName == removeControlName)
                {//注意删除先后
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

            for (int i = this.linesInfo.Count - 1; i >= 0; i--)//从后往前删线信息
            {
                if ((string)(e.Control.Tag) == this.linesInfo[i].StartInfo.Split(';')[0]
                    || (string)(e.Control.Tag) == this.linesInfo[i].EndInfo.Split(';')[0])
                {
                    CassViewGenerator.isUndo = true;//控件删除连带的直线删除不添加直线删除操作
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
            {//如果控件删除完时对Infos进行信息保存20090618
                this.InfosList = SaveInfo();
            }
            portReflash();
            DrawBackgroundImage();
        }

        /// <summary>
        /// CassView子控件加载事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void CassView_ControlAdded(object sender, ControlEventArgs e)
        {
            if (this.Controls.Count > this.recordControls && !CassViewGenerator.isUndo)
            {
                ControlInfo CtrlInfo = new ControlInfo();
                BlockInfo BlckInfo = new BlockInfo();
                CtrlInfo.ControlName = e.Control.Site.Name;
                BlckInfo.BlockName = e.Control.Site.Name;
                bool isCopy = false;//是否时粘贴控件
                PropertyDescriptor serialNumber = TypeDescriptor.GetProperties(e.Control)["SerialNumber"];
                PropertyDescriptor portNameProperty = TypeDescriptor.GetProperties(e.Control)["PortName"];
                PropertyDescriptor showNumberProperty = TypeDescriptor.GetProperties(e.Control)["ShowNumber"];
                PropertyDescriptor controlScaling = TypeDescriptor.GetProperties(e.Control)["Scaling"];
                #region
                //对FileIn  获取当前工程的路径 2013.11.22
                PropertyDescriptor currentProjectPath = TypeDescriptor.GetProperties(e.Control)["ProjectPath"];
                if (currentProjectPath != null)
                {
                    currentProjectPath.SetValue(e.Control, CassViewGenerator.currentProjectPath);
 
                }
                
            
                #endregion

                if (controlScaling != null)
                {//根据当前缩放率调整控件大小
                    controlScaling.SetValue(e.Control, this.scaling);
                }
                //给出模块序号
                if (serialNumber != null)
                {
                    List<int> tempNumlist = new List<int>();//临时存放所有控件的序号集合
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
                {//设置是否显示序号
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
                            //拖出控件
                            if (ToolBoxServiceImpl.typeNameString != null && ToolBoxServiceImpl.typeNameString.Length == 4)
                            {
                                if (categoryNode.Attributes[0].InnerText == ToolBoxServiceImpl.typeNameString[3])
                                {
                                    moduleSort = ToolBoxServiceImpl.typeNameString[2];
                                    StartFind = true;
                                }
                            }//粘贴控件
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
                                //输入输出变量和常数系统变量需要特殊操作
                            }
                            if (StartFind)//开始查节点属性
                            {
                                foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                                {
                                    if (moduleSort == toolItemNode.Attributes[0].InnerText.Split(',')[2]
                                        && toolItemNode.FirstChild != null)
                                    {
                                        //读取基本属性，设置模块基本属性，改变模块样式
                                        //并初始化控件出入口信息
                                        ReadBasicProperty(toolItemNode.FirstChild, e.Control, ref CtrlInfo);

                                        //读取功能属性，为模块添加功能属性，设置模块参数
                                        if (toolItemNode.FirstChild != null && toolItemNode.FirstChild.NextSibling != null
                                            && toolItemNode.FirstChild.NextSibling.ChildNodes.Count > 0)
                                        {
                                            ReadFunctionProperty(toolItemNode.FirstChild.NextSibling, ref CtrlInfo);
                                        }//end if (toolItemNode.FirstChild != null)

                                        //读取指令属性，保存用于生成Control.C时调用
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
                {//特殊控件对控件信息的Codeinfo部分进行赋值
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
                {//控件操作添加//注此时控件位置还没确定 即存在blockinfo 但无副确定值
                    Operation ctrlOpt = new Operation();
                    ctrlOpt.Oname = UndoOperation.OperateStyle.控件添加.ToString();
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
            {//撤销操作仍然需要增加控件指标
                this.recordControls++;
            }
            #region 设置组件模块其它属性

            e.Control.BackColor = this.BackColor;
            e.Control.LocationChanged += new EventHandler(Control_LocationChanged);
            e.Control.Tag = e.Control.Site.Name;
            if (this.linesEditable == true)
            {
                e.Control.Visible = false;
            }

            #endregion

            #  region    2013.11.29 保存文件
         //   ControlInfo temp = e.Control;
            if (e.Control.Site.Name.Contains("fileOutput"))
            {
                CassViewGenerator.SetCodeInfo(this, (Control)e.Control, "保存文件", "\"" + "null" + "\"");
            }

            #endregion

        }

        /// <summary>
        /// 初始化控件的输入输出口的占用
        /// 粘贴控件时所用
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
        /// 读取"BasicProperty"子节点,
        /// 根据控件的属性描述符集合,并根据函数ReadValue或许具体的属性值,进行控件属性的赋值操作。
        /// </summary>
        /// <param name="node">基本属性节点</param>
        /// <param name="instance">控件对象</param>
        /// <param name="CtrlInfo">控件信息</param>
        /// <returns>完成信息赋值后的控件信息</returns>
        private void ReadBasicProperty(XmlNode node, Control instance, ref ControlInfo CtrlInfo)
        {
            try
            {

                CtrlInfo.CodeInfo = new string[3];//初始化数组长度

                foreach (XmlNode property in node.ChildNodes)
                {
                    //XmlNode.Attributes 获取一个 XmlAttributeCollection，它包含该节点的属性。 这里是获得"name"的属性
                    XmlAttribute nameAttr = property.Attributes["name"];

                    //TypeDescriptor.GetProperties 返回组件或类型的属性 (Property) 的集合。 
                    PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(instance)[nameAttr.Value];
                    object value = null;

                    if (propDescriptor != null && ReadValue(property, propDescriptor.Converter, ref value) && value != null)
                    {
                        //PropertyDescriptor.SetValue 当在派生类中被重写时，将组件的值设置为一个不同的值。
                        propDescriptor.SetValue(instance, value);
                    }
                    if (nameAttr.Value == "ModuleSort")
                    {
                        CtrlInfo.SortName = property.InnerText;
                        CtrlInfo.CodeInfo[0] = property.InnerText;//0位放置ModuleSort
                        if (property.InnerText == "常数")
                        {//常数赋初值
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
                        CtrlInfo.CodeInfo[1] = property.InnerText;//1位放置ModuleName                                   
                    }
                    else if (nameAttr.Value == "OutputName")
                    { //初始化输出口信息
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
                MessageBox.Show("获取属性描述失败！");
                //return CtrlInfo;
            }
        }

        /// <summary>
        /// 读取"Value"节点
        /// 根据属性，将描述文档中的属性值进行类型的转换，转换为当前属性的值类型
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="converter">类型的转换器</param>  
        /// <param name="value">对象</param>
        /// <returns>该值是否能够被转化成需要的类型</returns>
        private bool ReadValue(XmlNode node, TypeConverter converter, ref object value)
        {
            try
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    //XmlNodeType枚举指定节点的类型。.Text:节点的文本内容。Text 节点不能具有任何子节点。
                    if (child.NodeType == XmlNodeType.Text)
                    {
                        value = converter.ConvertFromInvariantString(node.InnerText);
                        return true;
                    }
                    else if (child.NodeType == XmlNodeType.Element)
                    {//输入信息节点需用此部分读取输入接口信息20090616
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
        ///返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型。
        /// 如果类型转换器支持该类型的转换,则返回true
        /// </summary>
        private bool GetConversionSupported(TypeConverter converter, Type conversionType)
        {
            return (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType));
        }

        /// <summary>
        /// 读取"FunctionProperty"子节点,对控件信息的功能属性部分进行初始化
        /// </summary>
        /// <param name="node">功能属性节点</param>
        /// <param name="CtrlInfo">控件信息</param>
        /// <returns>完成信息赋值后的控件信息</returns>
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
                    {//存在5个属性则第五个为后来添加的参数描述信息
                        Function.ValueExplain = property.Attributes["exp"].Value;
                        if (property.Attributes.Count >= 6)
                        {//第六属性开始为优化属性
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
                        {//20090709添加部分控件的模块点名的功能属性 
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
                        else if (property.Attributes["type"].Value == "COM")  //串口设备号 2014.1 
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
                            {//20090604新添加的队列属性上位机不处理,数组值不处理
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
        /// 读取"OtherInfo"子节点,对控件信息的其他属性部分进行初始化
        /// </summary>
        /// <param name="node">其他属性节点</param>
        /// <param name="CtrlInfo">控件信息</param>
        /// <returns>完成信息赋值后的控件信息</returns>
        static public void ReadOtherProperty(XmlNode node, ref ControlInfo CtrlInfo)
        {
            string[] CodeAttribute = new string[3];
            foreach (XmlNode element in node.ChildNodes)
            {//描述节点不加入控件信息中
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
        /// 清理生成XML中的垃圾控件信息
        /// </summary>
        static public void ClearCtrlsInfo(CassView casv)
        {
            for (int i = casv.ctrlsInfo.Count - 1; i >= 0; i--)
            {
                if (casv.ctrlsInfo[i].ControlNum == -1)
                {//虚拟控件序号统一为-1
                    casv.ctrlsInfo.RemoveAt(i);
                }
            }
        }

        #endregion

        #region 工具函数
        /// <summary>
        /// 在PortNameList中添加新控件的信息，返回新的PortName
        /// </summary>
        /// <param name="control">控件信息</param>
        /// <returns>返回新的PortName</returns>
        static public string AddPortName(ControlInfo control, ref List<ArrayList> PInfoList)
        {//没有连接的控件没有保存其控件点名？！？！？！？20090702
            string FindPortName = null;
            if (PInfoList.Count != 0)
            {
                for (int i = 0; i < PInfoList.Count; i++)
                {
                    if (((string[])(PInfoList[i][0]))[0] == control.CodeInfo[0])
                    {//找到的对应的控件类
                        List<string> tempList = (List<string>)(PInfoList[i][1]);//临时队列用于减少隐性转换次数(可去) 

                        for (int j = 0; j <= tempList.Count; j++)
                        {//遍历点名序号占有列表
                            if (tempList.Contains(j.ToString()))
                            { continue; }
                            else
                            {
                                FindPortName = j.ToString();
                                int nullIndex = tempList.IndexOf(null);
                                if (nullIndex == -1)
                                {//队列中没有空位,则加在队列末
                                    ((List<string>)(PInfoList[i][1])).Add(FindPortName);
                                }
                                else
                                {//有空位则插入,返回序号
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
            {//空或者不在列表,则加新模块类                
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
        /// 修改在PortInfoList中的对应的信息
        /// </summary>
        /// <param name="MouleName">模块名</param>
        /// <param name="OldName">原PortName</param>
        /// <param name="ModifyName">需要改成的目标名,null标示删除</param>
        /// <returns>true表示修改删除成功,false表示失败</returns>
        static public bool ModifyPortName(string ModuleName, string OldName, string ModifyName)
        {
            bool FindPortName = false;
            if (CassViewGenerator.PortInfoList.Count != 0)
            {
                int[] tempNum = new int[2];//存放找到替换位置的两个序号(类型序号和同类型下的控件数序号)
                for (int i = 0; i < CassViewGenerator.PortInfoList.Count; i++)
                {
                    string typeName = ((string[])(CassViewGenerator.PortInfoList[i][0]))[1];

                    if (ModuleName == typeName)
                    {
                        List<string> tempList = (List<string>)(CassViewGenerator.PortInfoList[i][1]);//临时队列用于减少隐性转换次数(可去) 

                        if (ModifyName != null && tempList.Contains(ModifyName))//修改名出现重名则修改失败
                        {
                            return false;
                        }
                        else if (tempList.Contains(OldName))//确认存在原Portname
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
                    {//清空尾部的NULL
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
        /// 初始化控件端口信息
        /// </summary>
        /// <param name="Num">控件的输入或输出口数量</param>
        /// <returns>初始化的数组</returns>
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
        /// 根据原有控件端口信息初始化
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
        /// 判断该控件是否在当前Cassview中存在，存在则返回在控件集合中的序号
        /// </summary>
        /// <param name="controlName">需要查找的控件名</param>
        /// <returns>查找控件在控件集合中的序号，不存在返回-1</returns>
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
        /// 根据控件返回容器中存在的控件信息，不存在则返回空信息
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
        /// 根据控件信息找到对应的块信息，不存在则返回空信息
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
        ///// 判断当前Cassview中是否存在特殊控件
        ///// </summary>
        ///// <param name="portSort">需要查找控件的模块类名</param>
        ///// <returns>返回控件集合</returns>
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
        /// 刷新控件的出入口连线信息
        /// </summary>
        public void portReflash()
        {   //清空所有控件的输入输出口信息 
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
            //根据直线两端信息重新对控件出入口信息赋值
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
                    {//输出口信息需考虑单输出多输入的情况
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
        /// 备份一个控件信息
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
        /// 备份功能属性
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
        /// 还原当前页面所有控件的颜色
        /// 即把所有控件的isError属性编程false
        /// </summary>
        public void colorReflash()
        {
            //把当前的属性保存 再赋值 设置属性结束后再还原
            //连线模式控件VISIBLE属性为false
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

        #region 更新信息

        /// <summary>
        /// 读取XML更新控件信息
        /// 当开发时更改XML后添加修改功能属性时打开工程后调用
        /// 对输入口初值进行更新20090618
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
                                //读取功能属性，更新模块添加功能属性
                                if (toolItemNode.FirstChild != null && toolItemNode.FirstChild.NextSibling != null
                                    && toolItemNode.FirstChild.NextSibling.ChildNodes.Count > 0)
                                {
                                    //更新输入端口信息
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
                                //读取指令属性，保存用于生成Control.C时调用
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
        /// 更新输入端口初值 添加也不删除端口
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
                        {//遍历输入端口信息
                            for (int i = 0; i < InputInfo.Count; i++)
                            {
                                if (InputInfo[i].Length == 4 && element.Attributes["name"].Value == InputInfo[i][2])
                                {//端口名相同则赋值端口初值
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
        /// 根据节点信息找到当前功能属性集合中是否有对应的属性
        /// 没有则根据节点信息创建新属性
        /// 不可见属性则允许Value值不匹配 并进行修改
        /// </summary>
        /// <param name="Property"></param>
        /// <param name="FuntionInfos"></param>
        /// <returns>返回找到属性</returns>
        private XProp FindFunctionProperty(XmlNode Property, List<XProp> FuntionInfos, bool visible)
        {
            //寻找匹配信息
            foreach (XProp element in FuntionInfos)
            {
                if (element.Name == Property.Attributes["name"].Value
                    && element.VarName == Property.Attributes["varname"].Value
                    && element.ValueType == Property.Attributes["type"].Value)
                {                    
                    if (Property.Attributes.Count >= 5)
                    {//更新描述信息
                        element.ValueExplain = Property.Attributes["exp"].Value;
                        if (Property.Attributes.Count >= 6)
                        {//更新优化部分信息
                            element.Opnode = Property.Attributes["opnode"].Value;
                            element.Optype = Property.Attributes["optype"].Value;
                            element.Opvalue = Property.Attributes["opvalue"].Value;
                            element.Relate = Property.Attributes["relate"].Value;
                        }
                    }
                    if (Property.Attributes["type"].Value != "MyEnum")
                    {
                        if (!visible)
                        {//不可见属性直接赋初值
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
            //创建新信息
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
              //串口  调试 2014.1
               string portNames = CassView.GetSystemSerialPortNames();
               newFun.EnumValue = portNames;
               string[] portsList = portNames.Split(',');
               if (portsList.Length > 1)
               {
                   newFun.TheValue = portsList[0];
               }
               else  //表明只有一个串口名
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
        /// 获取系统串口名
        /// </summary>
        /// <returns>string</returns>
        public static string GetSystemSerialPortNames()
        {
            //List<string> portLlist = new List<string>();
            //string sysSerialPort = null;
            ////获取系统当前串口号
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