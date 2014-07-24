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
        public string Oname;//������
        public object Item;//��������
        public object[] Change;//�ı����
    }

    public class UndoOperation
    {
        public enum OperateStyle { ֱ�����, ֱ��ɾ��, �����Ŵ�, ������С, �ؼ����, �ؼ�ɾ��, �ؼ��ƶ�, �ؼ���ǰ, �ؼ��ú�, �����޸� };
        private CassView cassview = new CassView();
        private Stack<Operation> UndoStack = new Stack<Operation>();//��������ջ
        private Stack<Operation> RedoStack = new Stack<Operation>();//��������ջ
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
        /// ����
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
        /// ����
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
                case "�����Ŵ�":
                    {
                        changeOpt.Oname = OperateStyle.������С.ToString();
                        changeOpt.Change[0] = doThing.Change[1];
                        changeOpt.Change[1] = doThing.Change[0];
                        break;
                    }
                case "������С":
                    {
                        changeOpt.Oname = OperateStyle.�����Ŵ�.ToString();
                        changeOpt.Change[0] = doThing.Change[1];
                        changeOpt.Change[1] = doThing.Change[0];
                        break;
                    }
                case "�ؼ�ɾ��":
                    {
                        changeOpt.Oname = OperateStyle.�ؼ����.ToString();
                        changeOpt.Change = doThing.Change;
                        break;
                    }
                case "�ؼ����":
                    {
                        changeOpt.Oname = OperateStyle.�ؼ�ɾ��.ToString();
                        changeOpt.Change = doThing.Change;
                        break;
                    }
                case "�ؼ��ƶ�":
                    {
                        changeOpt.Oname = OperateStyle.�ؼ��ƶ�.ToString();
                        changeOpt.Change[0] = doThing.Change[1];
                        changeOpt.Change[1] = doThing.Change[0];
                        break;
                    }
                case "�ؼ���ǰ":
                    {
                        changeOpt.Oname = OperateStyle.�ؼ��ú�.ToString();
                        break;
                    }
                case "�ؼ��ú�":
                    {
                        changeOpt.Oname = OperateStyle.�ؼ���ǰ.ToString();
                        break;
                    }
                case "ֱ��ɾ��":
                    {
                        changeOpt.Oname = OperateStyle.ֱ�����.ToString();
                        changeOpt.Item = doThing.Item;
                        break;
                    }
                case "ֱ�����":
                    {
                        changeOpt.Oname = OperateStyle.ֱ��ɾ��.ToString();
                        changeOpt.Item = doThing.Item;
                        break;
                    }
                case "�����޸�":
                    {
                        changeOpt.Oname = OperateStyle.�����޸�.ToString();
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
                case "�����Ŵ�":
                    {
                        cassview.Scaling = Convert.ToSingle(Opt.Change[1]);
                        break;
                    }
                case "������С":
                    {
                        cassview.Scaling = Convert.ToSingle(Opt.Change[1]);
                        break;
                    }
                case "�ؼ�ɾ��":
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
                case "�ؼ����":
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
                case "�ؼ��ƶ�":
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
                case "�ؼ���ǰ":
                    {
                        ((Control)Opt.Item).BringToFront();
                        break;
                    }
                case "�ؼ��ú�":
                    {
                        ((Control)Opt.Item).SendToBack();
                        break;
                    }
                case "ֱ��ɾ��":
                    {
                        cassview.DeleteLine(GetLineinfoNum((LineInfo)Opt.Item), true);
                        break;
                    }
                case "ֱ�����":
                    {
                      Opt.Item=  AddnewLine((LineInfo)Opt.Item);
                        break;
                    }
                case "�����޸�":
                    {
                        if (Opt.Change[0].ToString() == "ģ�����")
                        {
                            foreach (Control element in cassview.Controls)
                            {
                                if (element.Tag.ToString() == ((Control)Opt.Item).Tag.ToString())
                                {//�ҵ�ɾ�����½��Ŀؼ�
                                    Opt.Item = element;
                                    break;
                                }
                            }
                            string Cmname = TypeDescriptor.GetProperties((Control)Opt.Item)["ModuleName"].GetValue((Control)Opt.Item).ToString();//��ǰ�ؼ�ģ����
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
                                        if (cassview.ctrlsInfo[i].VisibleFunctionProperty[j].Name == "ģ�����")
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
        /// ���ֱ����Ϣ�Լ�����ռ��ֱ�����˵Ŀؼ��ӿ�
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
        /// ��ȡɾ��ֱ����Ϣ�����
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
        /// ������ԭɾ���ؼ���ͬ�Ŀؼ�
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
            {//�¾ɿؼ�����ͬʱ�����
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
                    {//�ҵ��Ķ�Ӧ�Ŀؼ���
                        ((List<string>)(CassViewGenerator.PortInfoList[i][1])).Add(curPnum);
                        break;
                    }
                }
                if (tempCodeinfo[1] + curPnum != tempCodeinfo[2])
                {
                    CassMessageBox.Error("�����ؼ���������");
                }
            }
        }

        /// <summary>
        /// ���ؼ�ɾ������ �ؼ������Ѿ��޸�
        /// ���޸ĳ���������ջ�е�ֱ����Ϣ
        /// </summary>
        /// <param name="Oldname">�ɿؼ���</param>
        /// <param name="Newname">�¿ؼ���</param>
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
                    {//�����Ե�ǰ�ؼ�Ϊ����ֱ����Ϣ
                        tempL.StartInfo = Newname + ";" + start[1];
                    }
                    else if (end[0] == Oldname)
                    {//�����Ե�ǰ�ؼ�Ϊ�յ��ֱ����Ϣ
                        tempL.EndInfo = Newname + ";" + end[1];
                    }
                    info.Change[i] = tempL;
                }
            }
        }


        /// <summary>
        /// �ؼ�ɾ�������Ŀؼ���Ϣɾ������
        /// </summary>
        /// <param name="ctrlInfos">ɾ���Ŀؼ���Ϣ</param>
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
                    {//��һλ���HOSTDesign �ڶ�λ��ſؼ��� ����λΪ�ؼ���Ϣ ����λΪ����Ϣ �Ժ�Ϊ�ؼ����ֱ����Ϣ
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
