using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using CaVeGen.CommonOperation;

namespace CaVeGen.DesignViewFiles
{
    public struct Operation
    {
        public string Oname;//操作名
        public object Item;//操作对象
        public object[] Change;//改变过程
    }

    public class UndoOperation
    {
        public enum OperateStyle { 直线添加, 直线删除, 比例放大, 比例缩小, 控件添加, 控件删除, 控件移动, 控件置前, 控件置后, 属性修改 };
        private CassView cassview = new CassView();
        private Stack<Operation> UndoStack = new Stack<Operation>();//待撤销堆栈
        private Stack<Operation> RedoStack = new Stack<Operation>();//待重做堆栈
        public bool canUndo = true;
        public bool canRedo = false;

        public UndoOperation(TabPage curPage)
        {
            this.cassview = (CassView)(curPage.Controls[0].GetNextControl(curPage.Controls[0], false));
        }


        public void AddOperate(Operation opt)
        {
            RedoStack.Clear();
            UndoStack.Push(opt);
            canRedo = false;
            canUndo = true;
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            Operation tempOpt = ConvertOpt(UndoStack.Pop());
            OperateDoing(ref tempOpt);
            RedoStack.Push(tempOpt);
            if (UndoStack.Count == 0)
            {
                canUndo = false;
            }
            else
            {
                canUndo = true;
            }
            canRedo = true;
        }

        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            Operation tempOpt = ConvertOpt(RedoStack.Pop());
            OperateDoing( ref tempOpt);
            UndoStack.Push(tempOpt);
            if (RedoStack.Count == 0)
            {
                canRedo = false;
            }
            else
            {
                canRedo = true;
            }
            canUndo = true;
        }


        private Operation ConvertOpt( Operation doThing)
        {
            Operation changeOpt = new Operation();
            changeOpt.Item = doThing.Item;
            if (doThing.Change != null)
            {
                changeOpt.Change = new object[doThing.Change.Length];
            }

            switch (doThing.Oname)
            {
                case "比例放大":
                    {
                        changeOpt.Oname = OperateStyle.比例缩小.ToString();
                        changeOpt.Change[0] = doThing.Change[1];
                        changeOpt.Change[1] = doThing.Change[0];
                        break;
                    }
                case "比例缩小":
                    {
                        changeOpt.Oname = OperateStyle.比例放大.ToString();
                        changeOpt.Change[0] = doThing.Change[1];
                        changeOpt.Change[1] = doThing.Change[0];
                        break;
                    }
                case "控件删除":
                    {
                        changeOpt.Oname = OperateStyle.控件添加.ToString();
                        changeOpt.Change = doThing.Change;
                        break;
                    }
                case "控件添加":
                    {
                        changeOpt.Oname = OperateStyle.控件删除.ToString();
                        changeOpt.Change = doThing.Change;
                        break;
                    }
                case "控件移动":
                    {
                        changeOpt.Oname = OperateStyle.控件移动.ToString();
                        changeOpt.Change[0] = doThing.Change[1];
                        changeOpt.Change[1] = doThing.Change[0];
                        break;
                    }
                case "控件置前":
                    {
                        changeOpt.Oname = OperateStyle.控件置后.ToString();
                        break;
                    }
                case "控件置后":
                    {
                        changeOpt.Oname = OperateStyle.控件置前.ToString();
                        break;
                    }
                case "直线删除":
                    {
                        changeOpt.Oname = OperateStyle.直线添加.ToString();
                        changeOpt.Item = doThing.Item;
                        break;
                    }
                case "直线添加":
                    {
                        changeOpt.Oname = OperateStyle.直线删除.ToString();
                        changeOpt.Item = doThing.Item;
                        break;
                    }
                case "属性修改":
                    {
                        changeOpt.Oname = OperateStyle.属性修改.ToString();
                        changeOpt.Change[0] = doThing.Change[0];
                        changeOpt.Change[1] = doThing.Change[2];
                        changeOpt.Change[2] = doThing.Change[1];
                        break;
                    }
            }
            return changeOpt;
        }


        private void OperateDoing(ref Operation Opt)
        {

            switch (Opt.Oname)
            {
                case "比例放大":
                    {
                        cassview.Scaling = Convert.ToSingle(Opt.Change[1]);
                        break;
                    }
                case "比例缩小":
                    {
                        cassview.Scaling = Convert.ToSingle(Opt.Change[1]);
                        break;
                    }
                case "控件删除":
                    {
                        for (int i = 0; i < cassview.Controls.Count; i++)
                        {
                            if (cassview.Controls[i].Site.Name == ((Control)Opt.Item).Tag.ToString())
                            {
                                cassview.Controls.RemoveAt(i);
                                break;
                            }
                        }
                        break;
                    }
                case "控件添加":
                    {

                        CreateControl(ref Opt);
                        cassview.Controls.Add((Control)Opt.Item);
                        cassview.ctrlsInfo.Add((ControlInfo)Opt.Change[2]);
                        cassview.blocksInfo.Add((BlockInfo)Opt.Change[3]);

                        if (Opt.Change.Length > 4)
                        {
                            for (int i = 4; i < Opt.Change.Length; i++)
                            {
                              Opt.Change[i]=  AddnewLine((LineInfo)Opt.Change[i]);
                            }
                        }
                        cassview.portReflash();
                        cassview.DrawBackgroundImage();
                        break;
                    }
                case "控件移动":
                    {
                        BlockInfo tempBinfo = (BlockInfo)Opt.Change[1];
                        for (int i = 0; i < cassview.blocksInfo.Count; i++)
                        {
                            int x = cassview.Controls.Count;
                            if (cassview.blocksInfo[i].BlockNum == ((BlockInfo)Opt.Change[0]).BlockNum)
                            {
                                foreach (Control element in cassview.Controls)
                                {
                                    PropertyDescriptor Number = TypeDescriptor.GetProperties(element)["SerialNumber"];
                                    if (Number != null && Number.GetValue(element).ToString() == cassview.blocksInfo[i].BlockNum.ToString())
                                    {
                                        element.Location = new Point(tempBinfo.StartPoint.X + 5, tempBinfo.StartPoint.Y + 5);
                                        Opt.Item = element;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case "控件置前":
                    {
                        ((Control)Opt.Item).BringToFront();
                        break;
                    }
                case "控件置后":
                    {
                        ((Control)Opt.Item).SendToBack();
                        break;
                    }
                case "直线删除":
                    {
                        cassview.DeleteLine(GetLineinfoNum((LineInfo)Opt.Item), true);
                        break;
                    }
                case "直线添加":
                    {
                      Opt.Item=  AddnewLine((LineInfo)Opt.Item);
                        break;
                    }
                case "属性修改":
                    {
                        if (Opt.Change[0].ToString() == "模块点名")
                        {
                            foreach (Control element in cassview.Controls)
                            {
                                if (element.Tag.ToString() == ((Control)Opt.Item).Tag.ToString())
                                {//找到删除后新建的控件
                                    Opt.Item = element;
                                    break;
                                }
                            }
                            string Cmname = TypeDescriptor.GetProperties((Control)Opt.Item)["ModuleName"].GetValue((Control)Opt.Item).ToString();//当前控件模块名
                            CassView.ModifyPortName(Cmname, Opt.Change[1].ToString(), Opt.Change[2].ToString());
                            PropertyDescriptor portNameProperty = TypeDescriptor.GetProperties((Control)Opt.Item)["PortName"];
                            cassview.FindControlInfo((Control)Opt.Item).CodeInfo[2] = Cmname + Opt.Change[2].ToString();
                            portNameProperty.SetValue((Control)Opt.Item, Cmname + Opt.Change[2].ToString());

                            for (int i = 0; i < cassview.ctrlsInfo.Count; i++)
                            {
                                if (cassview.ctrlsInfo[i].ControlName == ((Control)Opt.Item).Site.Name && cassview.ctrlsInfo[i].VisibleFunctionProperty != null)
                                {
                                    for (int j = 0; j < cassview.ctrlsInfo[i].VisibleFunctionProperty.Count; j++)
                                    {
                                        if (cassview.ctrlsInfo[i].VisibleFunctionProperty[j].Name == "模块点名")
                                        {
                                            cassview.ctrlsInfo[i].VisibleFunctionProperty[j].TheValue = Opt.Change[2];
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            CassViewGenerator.SpecialValueChange(cassview, (Control)Opt.Item, Opt.Change[0].ToString(), Opt.Change[1], Opt.Change[2]);
                            CassViewGenerator.SetCodeInfo(cassview, (Control)Opt.Item, Opt.Change[0].ToString(), Opt.Change[2].ToString());
                        }
                        break;
                    }
            }
        }



        /// <summary>
        /// 添加直线信息以及重新占用直线两端的控件接口
        /// </summary>
        /// <param name="line"></paranm>
        private LineInfo AddnewLine(LineInfo line)
        {
            string[] start = line.StartInfo.Split(';');
            string[] end = line.EndInfo.Split(';');

            foreach (Control element in cassview.Controls)
            {
                PropertyDescriptor clickPointProperty = TypeDescriptor.GetProperties(element)["ClickPoint"];
                PropertyDescriptor checkPointProperty = TypeDescriptor.GetProperties(element)["CheckPoint"];
                PropertyDescriptor PointInfProperty = TypeDescriptor.GetProperties(element)["PointInformation"];
               
                clickPointProperty.SetValue(element, line.StartPoint);
                if ((bool)checkPointProperty.GetValue(element))
                {
                    line.StartInfo = element.Tag.ToString() + ";" + PointInfProperty.GetValue(element).ToString();
                    continue;
                }
                clickPointProperty.SetValue(element, line.EndPoint);
                if ((bool)checkPointProperty.GetValue(element))
                {
                    line.EndInfo = element.Tag.ToString() + ";" + PointInfProperty.GetValue(element).ToString();
                    continue;
                }

            }
            cassview.linesInfo.Add(line);
            cassview.portReflash();
            cassview.DrawBackgroundImage();
            return line;
        }

        /// <summary>
        /// 获取删除直线信息的序号
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private int GetLineinfoNum(LineInfo line)
        {
            for (int i = 0; i < cassview.linesInfo.Count; i++)
            {
                if (cassview.linesInfo[i].StartInfo == line.StartInfo
                    && cassview.linesInfo[i].EndInfo == line.EndInfo)
                {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// 创建与原删除控件相同的控件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private void CreateControl(ref Operation info)
        {
            HostDesign design = (HostDesign)info.Change[0];
            Control Oldctrl = (Control)info.Item;
            IDesignerHost tems = design.GetService(typeof(IDesignerHost)) as IDesignerHost;
            Control Newctrl = (Control)tems.CreateComponent(Oldctrl.GetType());

            Attribute[] tempAtr = new Attribute[] { new CategoryAttribute(PublicVariable.ControlCategoryName) };

            PropertyDescriptor portNameProperty = TypeDescriptor.GetProperties(Newctrl)["PortName"];
            PropertyDescriptorCollection OldPropertyDescriptors = TypeDescriptor.GetProperties(Oldctrl, tempAtr);
            PropertyDescriptorCollection NewPropertyDescriptors = TypeDescriptor.GetProperties(Newctrl, tempAtr);

            foreach (PropertyDescriptor elementA in OldPropertyDescriptors)
            {
                foreach (PropertyDescriptor elementB in NewPropertyDescriptors)
                {
                    if (elementA.Name == elementB.Name)
                    {
                        elementB.SetValue(Newctrl, elementA.GetValue(Oldctrl));
                    }
                }
            }
            Newctrl.Tag = Newctrl.Site.Name;
            if (info.Change[1].ToString() != Newctrl.Site.Name)
            {//新旧控件名不同时需更新
                UpdateCtrlInfo(ref info, info.Change[1].ToString(), Newctrl.Site.Name);
            }
            info.Item = Newctrl;
            if (portNameProperty != null)
            {
                string[] tempCodeinfo = ((ControlInfo)info.Change[2]).CodeInfo;
                string curPnum = tempCodeinfo[2].Substring(tempCodeinfo[1].Length);

                for (int i = 0; i < CassViewGenerator.PortInfoList.Count; i++)
                {
                    if (((string[])(CassViewGenerator.PortInfoList[i][0]))[0] == tempCodeinfo[0])
                    {//找到的对应的控件类
                        ((List<string>)(CassViewGenerator.PortInfoList[i][1])).Add(curPnum);
                        break;
                    }
                }
                if (tempCodeinfo[1] + curPnum != tempCodeinfo[2])
                {
                    CassMessageBox.Error("撤销控件点名出错！");
                }
            }
        }

        /// <summary>
        /// 当控件删除后撤销 控件本身已经修改
        /// 需修改撤销重做堆栈中的直线信息
        /// </summary>
        /// <param name="Oldname">旧控件名</param>
        /// <param name="Newname">新控件名</param>
        private void UpdateCtrlInfo(ref Operation info, string Oldname, string Newname)
        {
            info.Change[1] = Newname;
            ControlInfo tempCinfo = (ControlInfo)info.Change[2];
            BlockInfo tempBinfo = (BlockInfo)info.Change[3];
            tempCinfo.ControlName = Newname;
            tempBinfo.BlockName = Newname;
            info.Change[2] = tempCinfo;
            info.Change[3] = tempBinfo;

            if (info.Change.Length > 4)
            {
                for (int i = 4; i < info.Change.Length; i++)
                {
                    LineInfo tempL = (LineInfo)info.Change[i];
                    string[] start = tempL.StartInfo.Split(';');
                    string[] end = tempL.EndInfo.Split(';');
                    if (start[0] == Oldname)
                    {//更新以当前控件为起点的直线信息
                        tempL.StartInfo = Newname + ";" + start[1];
                    }
                    else if (end[0] == Oldname)
                    {//更新以当前控件为终点的直线信息
                        tempL.EndInfo = Newname + ";" + end[1];
                    }
                    info.Change[i] = tempL;
                }
            }
        }


        /// <summary>
        /// 控件删除附带的控件信息删除操作
        /// </summary>
        /// <param name="ctrlInfos">删除的控件信息</param>
        public void SpecialAddinfo(List<object> ctrlInfos)
        {
            Operation[] tempList = UndoStack.ToArray();
            for (int i = 0; i < tempList.Length; i++)
            {
                Operation temp = tempList[i];
                if (temp.Change != null && temp.Change[1].ToString() == ((ControlInfo)ctrlInfos[0]).ControlName)
                {
                    List<object> newInfo = new List<object>();
                    newInfo.AddRange(temp.Change);
                    if (newInfo.Count == 2)
                    {//第一位存放HOSTDesign 第二位存放控件名 第三位为控件信息 第四位为块信息 以后为控件相关直线信息
                        newInfo.AddRange(ctrlInfos);
                    }
                    else
                    {
                        newInfo.RemoveRange(2, newInfo.Count - 2);
                        newInfo.AddRange(ctrlInfos);
                    }
                    temp.Change = newInfo.ToArray();
                    tempList[i] = temp;
                }
            }
            UndoStack.Clear();
            for (int y = tempList.Length - 1; y >= 0; y--)
            {
                UndoStack.Push(tempList[y]);
            }
        }
    }
}
