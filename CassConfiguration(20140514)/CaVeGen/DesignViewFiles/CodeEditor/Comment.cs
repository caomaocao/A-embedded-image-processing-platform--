using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using CaVeGen.DesignViewFiles.FilterProperty;
using CodeSense;

namespace CaVeGen.DesignViewFiles.CodeEditor
{
    public partial class Comment : ListBox
    {
        public Comment()
        {
            InitializeComponent();
            this.SelectedIndexChanged += new EventHandler(CommandTipSetting_SelectedIndexChanged);
            this.VisibleChanged += new EventHandler(CommandTipSetting_VisibleChanged);
        }

        public Comment(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public event EventHandler ListClick;

        /// <summary>
        /// ָ����ʾ
        /// </summary>
        /// <param name="input"></param>
        /// <param name="currentWord"></param>
        public void SetTipList(string input, string currentWord)
        {
            if (input.Trim() == "" ||
                input == null)
                return;

            this.Items.Clear();
            // �ȷ����ɷ�,���ָ���,ȥ�����
            // ����ע�͵���ȥ��
            ArrayList showList = AnalyzeInput(input, currentWord);//����������Ϣ��ʾ
            foreach (string s in showList)
            {
                if (!this.Items.Contains(s))  // �����ظ��ɷ�
                    this.Items.Add(s);
            }
        }
  
        /// <summary>
        /// �ȷ����ɷ�,���ָ���,ȥ�����
        /// ע: ע�͵���ȥ��,���ǰ���ַ�
        /// </summary>
        /// <param name="input"></param>
        /// <param name="currentWord"></param>
        /// <returns></returns>
        private ArrayList AnalyzeInput(string input, string currentWord)
        {            
            ArrayList tipList = new ArrayList();

            string[] ss = input.Split(':');
            // �����
            if (ss.Length == 2)
            {
                input = ss[1];
            }
            // ������
            char[] csSplit = new char[] { ' ' };
            ss = input.Split(csSplit, StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length == 1)
            {
                // ��ȫ���
                string keyword = ss[0].Trim().ToUpper();
                if (PLCCodeEditor.CtrlPropertys.ContainsKey(keyword))
                {
                    ControlInfo dicCS = PLCCodeEditor.CtrlPropertys[keyword];
                    if (currentWord == "" || currentWord == ",")  // ������ո�
                    {
                        //this.toolTip1.Show(String.Join(",", GetTipmessage(dicCS, -1).ToArray()), this, new Point(Size.Width, 0));
                        GetTipmessage(dicCS, -1);
                    }
                }
                //����Ӻ��е�ǰ�ַ���ָ��
                foreach (string key in PLCCodeEditor.SunningCodes.Keys)
                {
                    if (key.StartsWith(keyword) 
                        //&& keyword != key 
                        && currentWord != "")
                    {
                        tipList.Add(key);
                    }
                }
            }
            else if (ss.Length == 2) // ����
            {    
                    ShowTipText(input);

            }
            return tipList;
        }

        /// <summary>
        /// ���ݵ�ǰ���ַ�����ʾ��ʾ��Ϣ
        /// </summary>
        /// <param name="currentWord"></param>
        public void ShowTipText(string CurrentString)
        {
            string[] rowInfo = CurrentString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (rowInfo.Length == 1 && PLCCodeEditor.CtrlPropertys.ContainsKey(rowInfo[0])
                //this.toolTip1.ToolTipTitle != ""
                )
            {
                ControlInfo dicCS = PLCCodeEditor.CtrlPropertys[rowInfo[0]];

                //this.toolTip1.Show(String.Join(",", GetTipmessage(dicCS, -1).ToArray()), this, new Point(Size.Width, 0));
                GetTipmessage(dicCS, -1);
            }
            else if (rowInfo.Length >= 2)
            {
                if (rowInfo.Length != 2)
                {//�����м��пո�������ֶ�ʱ�ϲ���Щ�ֶ�
                    for (int x = 2; x < rowInfo.Length; x++)
                    { rowInfo[1] += rowInfo[x]; }
                }
                string keyWord = rowInfo[0].Trim().ToUpper();
                int tempIndex = getParaIndex(rowInfo[1]);              
                
                // �ؼ��ִ���
                if (PLCCodeEditor.CtrlPropertys.ContainsKey(keyWord))
                {
                    ControlInfo dicCS = PLCCodeEditor.CtrlPropertys[keyWord];
                    GetTipmessage(dicCS, tempIndex);
                }
            }
            else if (rowInfo == null || rowInfo.Length == 0||!PLCCodeEditor.CtrlPropertys.ContainsKey(rowInfo[0]))
            {//����Ϊ�� �����ʾ
                this.HideALL();
            }
        }

        /// <summary>
        /// ���ݹ��ǰȥ��ָ��ֵ��ַ��� ���㵱ǰ�Ĳ������
        /// </summary>
        /// <param name="currentString"></param>
        /// <returns></returns>
        private int getParaIndex(string currentString)
        {
            int ArrayCount = 0;
            int paraCount = 0;//�������
            bool startArray = false;//�Ƿ��������ڲ�

            for (int charIndex = currentString.Length - 1; charIndex >= 0; charIndex--)
            {//��������ַ���
                if (currentString[charIndex] == '}')
                {//��ʼ�������
                    startArray = true;
                }
                if (currentString[charIndex] == '{')
                {//�����������
                    if (startArray)
                    {//������������֮��
                        startArray = false;
                    }
                    else//�������������� ���֮ǰ��¼�Ĳ������
                    { paraCount = 0; }
                }
                if (currentString[charIndex] == ',')
                {//���ݶ��Ÿ��������������
                    if (startArray)
                    {
                        ArrayCount++;
                    }
                    else
                    {
                        paraCount++;
                    }
                }
            }
            
            return paraCount;
        }

        /// <summary>
        /// ����ѡ��ѡ��ʱ��ʾ��Ӧ��ѡ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandTipSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedIndex >= 0)
            {
                //this.toolTip1.ToolTipTitle = this.SelectedItem.ToString();

                // ������
                if (PLCCodeEditor.SpecialCode.ContainsKey(this.SelectedItem.ToString()))
                {//����ָ��
                    this.toolTip1.Show(PLCCodeEditor.SpecialCode[this.SelectedItem.ToString()], this, new Point(Size.Width, 0));
                }
                else if (PLCCodeEditor.CtrlPropertys.ContainsKey(this.SelectedItem.ToString()))
                {
                    string tip = PLCCodeEditor.SunningCodes[this.SelectedItem.ToString()] + "\n";
                    ControlInfo dicCS = PLCCodeEditor.CtrlPropertys[this.SelectedItem.ToString()];
                   GetTipmessage(dicCS, -1);
                   
                }

                // �ǲ���
                else if (this.Tag != null && PLCCodeEditor.CtrlPropertys.ContainsKey(this.Tag.ToString()))
                {
                    ControlInfo dicCS = PLCCodeEditor.CtrlPropertys[this.Tag.ToString()];
                    foreach (XProp element in dicCS.VisibleFunctionProperty)
                    {
                        if (element.VarName == this.SelectedItem.ToString())
                        {
                            this.toolTip1.Show(element.Name + ":\n" + element.ValueExplain, this, new Point(Size.Width, 0));
                            break;
                        }
                    }
                }

                //// �Ǳ���,����Ϊ��
                //if (ParameterListSetting.NickNameList.ContainsValue(this.SelectedItem.ToString()))
                //{
                //    int index = ParameterListSetting.NickNameList.IndexOfValue(this.SelectedItem.ToString());
                //    string realName = ParameterListSetting.NickNameList.Keys[index];
                //    this.toolTip1.Show(ParameterListSetting.ParaList[realName].information, this,
                //        new Point(Size.Width, 0));
                //}
            }
        }
        // ����ʾ����ʱ������ʾ
        private void CommandTipSetting_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == false)
                this.toolTip1.Hide(this);
        }

        /// <summary>
        /// ���������Ϣ
        /// ���������������ʾ��
        /// </summary>
        /// <param name="isALL"></param>
        public void HideALL()
        {
            this.Items.Clear();
            this.Hide();
            this.toolTip1.Hide(this);
        }

        /// <summary>
        /// �ӿؼ���Ϣ�л�ȡ��ʾ��Ϣ
        /// �������index��-1�򷵻ض�Ӧ��ŵĲ�����ʾ��Ϣ
        /// ���򷵻����в�������ʾ��Ϣ
        /// </summary>
        /// <param name="ctrl">�ؼ���Ϣ</param>
        /// <param name="index">�������</param>
        /// <returns>��ʾ��Ϣ</returns>
        public void GetTipmessage(ControlInfo ctrl, int index)
        {
            List<string> TipMes = new List<string>();//�����õ���Ϣ����
            string tip = null;//��ʾ
            if (ctrl.VisibleFunctionProperty.Count == 0 && ctrl.UnvisibleFunctionProperty.Count == 0)
            {//Ĭ�Ϲ�������Ϊ����û�в�����ʾ��Ϣ
                this.toolTip1.Hide(this);
                return;
            }
            if (index == -1)
            {//����ȫ���Ĳ�����Ϣ
                int paraCount = 0;

                //foreach (XProp element in ctrl.VisibleFunctionProperty)
                //{
                //    tempMes = AddressTable.ConvertShowType(element.ValueType) + " " + element.VarName;
                //    if (TipMes.Count % 4 == 3)//ÿ��3������
                //    { tempMes = "\n" + tempMes; }
                //    TipMes.Add(tempMes);
                //}
                foreach (XProp element in ctrl.UnvisibleFunctionProperty)
                {
                    if (element.TheValue.ToString().Contains("array"))
                    {
                        //tempMes = AddressTable.ConvertShowType(element.ValueType) + " " + element.VarName;
                        //if (TipMes.Count % 4 == 3)
                        //{ tempMes = "\n" + tempMes; }
                        //TipMes.Add(tempMes);
                        paraCount++;
                    }
                }
                paraCount += ctrl.VisibleFunctionProperty.Count;
                if (this.toolTip1.ToolTipTitle != ctrl.SortName)
                { //�ؼ�������������
                    this.toolTip1.ToolTipTitle = ctrl.SortName;
                }

                if (PLCCodeEditor.CanTipCode.Contains(ctrl.CodeInfo[1]))
                {
                    tip += "1������";
                }
                else if (paraCount == 0)
                { tip += "�޲���"; }
                else
                {
                    tip += paraCount + "������";
                }

                tip += "\n" + ctrl.OtherProperty[2];

                this.toolTip1.Show(tip, this, new Point(Size.Width, 0));
                //return TipMes;
            }
            else if(ctrl.VisibleFunctionProperty.Count!=0)
            {//�����ƶ�������Ϣ
                int ParaCount = 0;//����������� �ڲ��ɼ��������Ա����м�¼����
                XProp tempFun = new XProp();

                if (index < ctrl.VisibleFunctionProperty.Count)
                {//�������С�ڿɼ���������
                    tempFun = ctrl.VisibleFunctionProperty[index];
                }
                //else
                //{
                    for (int x = 0; x < ctrl.UnvisibleFunctionProperty.Count; x++)
                    {
                        if (ctrl.UnvisibleFunctionProperty[x].TheValue.ToString().Contains("array"))
                        {//���ɼ��������
                            if (index == ParaCount + ctrl.VisibleFunctionProperty.Count)
                            {//��ŷ���
                                tempFun = ctrl.UnvisibleFunctionProperty[x];
                            }
                            ParaCount++;
                        }
                    }
                //}
                //string tip = AddressTable.ConvertShowType(tempFun.ValueType) + " " + tempFun.VarName;
                //if (index <= ParaCount + ctrl.VisibleFunctionProperty.Count - 1)
                //{
                //    if (index != ParaCount + ctrl.VisibleFunctionProperty.Count - 1)
                //    {//����Ϊ���һ������
                //        tip = tip + ",...";
                //    }
                //    if (index != 0)
                //    { //����Ϊ��һ������
                //        tip = "...," + tip;
                //    }
                //    //���ͺ��м���������Ϣ
                //    tip += "\n" + tempFun.Name + ":\n" + tempFun.ValueExplain;

                //    TipMes.Add(tip);
                //}
                string title = tempFun.Name + " (" + (index + 1) + "/" + (ParaCount + ctrl.VisibleFunctionProperty.Count) + ")";
                tip = AddressTable.ConvertShowType2(tempFun.ValueType) + "\n";
                for (int i = 0; i < tempFun.ValueExplain.Length; i++)
                {
                    if (i % 11 == 0)
                    {//11���ַ�����
                        tip += "\n";
                    }
                    tip += tempFun.ValueExplain[i];
                }
                if (this.toolTip1.ToolTipTitle != title)
                { this.toolTip1.ToolTipTitle = title; }
                this.toolTip1.Show(tip, this, new Point(Size.Width, 0));

            } 
            //return TipMes;
        }

        /// <summary>
        /// �б����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comment_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.SelectedIndex > -1 && this.ListClick != null)
            {
                this.ListClick(sender, e);
            }
        }

        private void Comment_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.SelectedIndex > -1 && this.ListClick != null)
            {
                this.ListClick(sender, e);
            }
        }


    }
}
