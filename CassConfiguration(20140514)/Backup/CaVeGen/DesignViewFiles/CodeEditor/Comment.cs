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
        /// 指令提示
        /// </summary>
        /// <param name="input"></param>
        /// <param name="currentWord"></param>
        public void SetTipList(string input, string currentWord)
        {
            if (input.Trim() == "" ||
                input == null)
                return;

            this.Items.Clear();
            // 先分析成份,提出指令部份,去掉标号
            // 多行注释的已去除
            ArrayList showList = AnalyzeInput(input, currentWord);//设置描述信息提示
            foreach (string s in showList)
            {
                if (!this.Items.Contains(s))  // 过滤重复成分
                    this.Items.Add(s);
            }
        }
  
        /// <summary>
        /// 先分析成份,提出指令部份,去掉标号
        /// 注: 注释的已去除,光标前的字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="currentWord"></param>
        /// <returns></returns>
        private ArrayList AnalyzeInput(string input, string currentWord)
        {            
            ArrayList tipList = new ArrayList();

            string[] ss = input.Split(':');
            // 带标号
            if (ss.Length == 2)
            {
                input = ss[1];
            }
            // 操作码
            char[] csSplit = new char[] { ' ' };
            ss = input.Split(csSplit, StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length == 1)
            {
                // 完全相等
                string keyword = ss[0].Trim().ToUpper();
                if (PLCCodeEditor.CtrlPropertys.ContainsKey(keyword))
                {
                    ControlInfo dicCS = PLCCodeEditor.CtrlPropertys[keyword];
                    if (currentWord == "" || currentWord == ",")  // 已输入空格
                    {
                        //this.toolTip1.Show(String.Join(",", GetTipmessage(dicCS, -1).ToArray()), this, new Point(Size.Width, 0));
                        GetTipmessage(dicCS, -1);
                    }
                }
                //再添加含有当前字符的指令
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
            else if (ss.Length == 2) // 参数
            {    
                    ShowTipText(input);

            }
            return tipList;
        }

        /// <summary>
        /// 根据当前行字符串显示提示信息
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
                {//参数中间有空格键产生分段时合并这些分段
                    for (int x = 2; x < rowInfo.Length; x++)
                    { rowInfo[1] += rowInfo[x]; }
                }
                string keyWord = rowInfo[0].Trim().ToUpper();
                int tempIndex = getParaIndex(rowInfo[1]);              
                
                // 关键字存在
                if (PLCCodeEditor.CtrlPropertys.ContainsKey(keyWord))
                {
                    ControlInfo dicCS = PLCCodeEditor.CtrlPropertys[keyWord];
                    GetTipmessage(dicCS, tempIndex);
                }
            }
            else if (rowInfo == null || rowInfo.Length == 0||!PLCCodeEditor.CtrlPropertys.ContainsKey(rowInfo[0]))
            {//数组为空 清空提示
                this.HideALL();
            }
        }

        /// <summary>
        /// 根据光标前去掉指令部分的字符串 计算当前的参数序号
        /// </summary>
        /// <param name="currentString"></param>
        /// <returns></returns>
        private int getParaIndex(string currentString)
        {
            int ArrayCount = 0;
            int paraCount = 0;//参数序号
            bool startArray = false;//是否在数组内部

            for (int charIndex = currentString.Length - 1; charIndex >= 0; charIndex--)
            {//逆向遍历字符串
                if (currentString[charIndex] == '}')
                {//开始数组参数
                    startArray = true;
                }
                if (currentString[charIndex] == '{')
                {//结束数组参数
                    if (startArray)
                    {//光标在数组参数之外
                        startArray = false;
                    }
                    else//光标在数组参数内 清空之前记录的参数序号
                    { paraCount = 0; }
                }
                if (currentString[charIndex] == ',')
                {//依据逗号个数计算参数个数
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
        /// 下拉选单选择时提示对应的选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandTipSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedIndex >= 0)
            {
                //this.toolTip1.ToolTipTitle = this.SelectedItem.ToString();

                // 是命令
                if (PLCCodeEditor.SpecialCode.ContainsKey(this.SelectedItem.ToString()))
                {//特殊指令
                    this.toolTip1.Show(PLCCodeEditor.SpecialCode[this.SelectedItem.ToString()], this, new Point(Size.Width, 0));
                }
                else if (PLCCodeEditor.CtrlPropertys.ContainsKey(this.SelectedItem.ToString()))
                {
                    string tip = PLCCodeEditor.SunningCodes[this.SelectedItem.ToString()] + "\n";
                    ControlInfo dicCS = PLCCodeEditor.CtrlPropertys[this.SelectedItem.ToString()];
                   GetTipmessage(dicCS, -1);
                   
                }

                // 是参数
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

                //// 是别名,不会为空
                //if (ParameterListSetting.NickNameList.ContainsValue(this.SelectedItem.ToString()))
                //{
                //    int index = ParameterListSetting.NickNameList.IndexOfValue(this.SelectedItem.ToString());
                //    string realName = ParameterListSetting.NickNameList.Keys[index];
                //    this.toolTip1.Show(ParameterListSetting.ParaList[realName].information, this,
                //        new Point(Size.Width, 0));
                //}
            }
        }
        // 当提示框隐时不用提示
        private void CommandTipSetting_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == false)
                this.toolTip1.Hide(this);
        }

        /// <summary>
        /// 清空下拉信息
        /// 并隐藏下拉框和提示框
        /// </summary>
        /// <param name="isALL"></param>
        public void HideALL()
        {
            this.Items.Clear();
            this.Hide();
            this.toolTip1.Hide(this);
        }

        /// <summary>
        /// 从控件信息中获取提示信息
        /// 如果参数index非-1则返回对应序号的参数提示信息
        /// 否则返回所有参数的提示信息
        /// </summary>
        /// <param name="ctrl">控件信息</param>
        /// <param name="index">参数序号</param>
        /// <returns>提示信息</returns>
        public void GetTipmessage(ControlInfo ctrl, int index)
        {
            List<string> TipMes = new List<string>();//返回用的信息数组
            string tip = null;//提示
            if (ctrl.VisibleFunctionProperty.Count == 0 && ctrl.UnvisibleFunctionProperty.Count == 0)
            {//默认功能属性为空则没有参数提示信息
                this.toolTip1.Hide(this);
                return;
            }
            if (index == -1)
            {//查找全部的参数信息
                int paraCount = 0;

                //foreach (XProp element in ctrl.VisibleFunctionProperty)
                //{
                //    tempMes = AddressTable.ConvertShowType(element.ValueType) + " " + element.VarName;
                //    if (TipMes.Count % 4 == 3)//每行3个参数
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
                { //控件中文名作标题
                    this.toolTip1.ToolTipTitle = ctrl.SortName;
                }

                if (PLCCodeEditor.CanTipCode.Contains(ctrl.CodeInfo[1]))
                {
                    tip += "1个参数";
                }
                else if (paraCount == 0)
                { tip += "无参数"; }
                else
                {
                    tip += paraCount + "个参数";
                }

                tip += "\n" + ctrl.OtherProperty[2];

                this.toolTip1.Show(tip, this, new Point(Size.Width, 0));
                //return TipMes;
            }
            else if(ctrl.VisibleFunctionProperty.Count!=0)
            {//查找制定参数信息
                int ParaCount = 0;//数组参数个数 在不可见功能属性遍历中记录所用
                XProp tempFun = new XProp();

                if (index < ctrl.VisibleFunctionProperty.Count)
                {//查找序号小于可见功能属性
                    tempFun = ctrl.VisibleFunctionProperty[index];
                }
                //else
                //{
                    for (int x = 0; x < ctrl.UnvisibleFunctionProperty.Count; x++)
                    {
                        if (ctrl.UnvisibleFunctionProperty[x].TheValue.ToString().Contains("array"))
                        {//不可见数组参数
                            if (index == ParaCount + ctrl.VisibleFunctionProperty.Count)
                            {//序号符合
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
                //    {//当不为最后一个参数
                //        tip = tip + ",...";
                //    }
                //    if (index != 0)
                //    { //当不为第一个参数
                //        tip = "...," + tip;
                //    }
                //    //类型后换行加入描述信息
                //    tip += "\n" + tempFun.Name + ":\n" + tempFun.ValueExplain;

                //    TipMes.Add(tip);
                //}
                string title = tempFun.Name + " (" + (index + 1) + "/" + (ParaCount + ctrl.VisibleFunctionProperty.Count) + ")";
                tip = AddressTable.ConvertShowType2(tempFun.ValueType) + "\n";
                for (int i = 0; i < tempFun.ValueExplain.Length; i++)
                {
                    if (i % 11 == 0)
                    {//11个字符换行
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
        /// 列表点击事件
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
