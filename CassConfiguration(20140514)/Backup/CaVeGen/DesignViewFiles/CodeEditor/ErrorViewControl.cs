using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CaVeGen.DesignViewFiles.CodeEditor
{
    public partial class ErrorViewControl : UserControl
    {
        public ErrorViewControl()
        {
            InitializeComponent();
        }

        public event EventHandler LineDoubleClick;
        public List<string[]> errorList = new List<string[]>();//存放错误和警告信息
        public List<string[]> ViewErrorInfo = new List<string[]>();//可视编程错误信息
        public int errorNum = 0;//错误的个数
        //public int warningNum = 0;//警告的个数
        //public int infoNum = 0; //消息的个数

        // 高亮显示的行号
        int highLightLine;
        public int HighLightLine
        {
            get
            {
                return highLightLine;
            }
        }

        // 需要找到的控件
        // 数组长度2 页面和序号
        string[] selectCtrlinfo;
        public string[] SelectCtrlinfo
        {
            get
            {
                return selectCtrlinfo;
            }
        }

        public ListView WrongListView
        {
            get
            {
                return this.lv_wrongList;
            }
            set
            {
                this.lv_wrongList = value;
            }
        }



        // 每页都有自已的错误信息,当页面切换时,显示不同的错误信息
        //CompileClass.InfoStruct info = new CompileClass.InfoStruct();
        //public CompileClass.InfoStruct Info
        //{
        //    get
        //    {
        //        return info;
        //    }
        //    set
        //    {
        //        info = value;
        //    }
        //}

        // 文件名
        string fileName = "";
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }


        // 添加一条信息到列表中
        //private void AddToList(CompileClass.WrongInfoStruct wis, bool state)
        //{
        //    // 有行号
        //    if (wis.lineNumber > 0)
        //    {
        //        // 无提示
        //        if (wis.tip == "")
        //            AddWrongSentence(wis.wrongContent, wis.lineNumber, state);
        //        else  // 有提示
        //            AddWrongSentence(wis.wrongContent, wis.lineNumber, wis.tip, state);
        //    }
        //    else // 无行号
        //        AddWrongSentence(wis.wrongContent, state);

        //}

        /// <summary>
        /// 显示错误列表中的错误和警告
        /// </summary> 
        public void SetListview()
        {
            ClearList();//清空
            int errorCount = 0;
            int warningCount = 0;
            int infoCount = 0;

            foreach (string[] element in this.errorList)
            {
                if (element[2] == "error")
                {
                    if (this.ts_wrong.Checked)
                    {
                        if (element.Length == 3)
                        {
                            if (element[0] != null)
                            {
                                AddWrongSentence(element[1], Convert.ToInt32(element[0]), true, null);
                            }
                            else
                            {
                                AddWrongSentence(element[1], true, null);
                            }
                        }
                        else
                        {
                            if (element[0] != null)
                            {
                                AddWrongSentence(element[1], Convert.ToInt32(element[0]), true, element[3].ToUpper());
                            }
                            else
                            {
                                AddWrongSentence(element[1], true, element[3].ToUpper());
                            }
                        }
                    }
                    errorCount++;
                }
                else if (element[2] == "warning")
                {
                    if (this.ts_warning.Checked)
                    {

                        if (element.Length == 3)
                        {
                            AddWrongSentence(element[1], Convert.ToInt32(element[0]), false, null);
                        }
                        else
                        {
                            if (element[0] != null)
                            {
                                AddWrongSentence(element[1], Convert.ToInt32(element[0]), false, element[3].ToUpper());
                            }
                            else
                            {
                                AddWrongSentence(element[1], false, element[3].ToUpper());
                            }
                        }
                    }
                    warningCount++;
                }
                else if (element[2] == "info")
                {
                    if (this.ts_info.Checked)
                    {
                        string[] subItemsString = new string[] { "", (this.lv_wrongList.Items.Count + 1).ToString(), element[1], null, null };
                        this.lv_wrongList.Items.Add(new ListViewItem(subItemsString, 2));
                    }
                    infoCount++;
                }

            }

            // 设置标签
            this.ts_wrong.Text = errorCount.ToString() + "个错误";
            this.ts_warning.Text = warningCount.ToString() + "个警告";
            this.ts_info.Text = infoCount.ToString() + "个消息";
        }

        /// <summary>
        /// 检测当前错误列表中的错误个数
        /// </summary>
        public int checkErrorCount()
        {
            this.errorNum = 0;
            foreach (string[] element in this.errorList)
            {
                if (element[2] == "error")
                { this.errorNum++; }
            }
            return this.errorNum;
        }

        // 添加出错语句,有行号的,有提示的
        private void AddWrongSentence(string wrongContext, int wrongNumber, string tip, bool isWrong)
        {
            int listViewIndex = this.lv_wrongList.Items.Count;
            string[] subItemsString = new string[]{
                "",
                (listViewIndex + 1).ToString(),
                wrongContext,
                wrongNumber.ToString(),
                fileName};

            ListViewItem lvi;
            if (isWrong)
                lvi = new ListViewItem(subItemsString, 1);
            else
                lvi = new ListViewItem(subItemsString, 0);
            lvi.ToolTipText = tip; // 加入提示    
            this.lv_wrongList.Items.Add(lvi);

        }
        // 添加出错语句,有行号的
        public void AddWrongSentence(string wrongContext, int wrongNumber, bool isWrong, string pageName)
        {
            int listViewIndex = this.lv_wrongList.Items.Count;
            string[] subItemsString = new string[]{
                "",
                (listViewIndex + 1).ToString(),
                wrongContext,
                (wrongNumber+1).ToString(),
                pageName};

            ListViewItem lvi;
            if (isWrong)
                lvi = new ListViewItem(subItemsString, 1);
            else
                lvi = new ListViewItem(subItemsString, 0);
            this.lv_wrongList.Items.Add(lvi);
        }
        // 添加出错语句,无行号的
        private void AddWrongSentence(string wrongContext, bool isWrong, string pageName)
        {
            int listViewIndex = this.lv_wrongList.Items.Count;
            string[] subItemsString = new string[]{
                "",
                (listViewIndex + 1).ToString(),
                wrongContext,
                "",
                pageName
                };

            ListViewItem lvi;
            if (isWrong)
                lvi = new ListViewItem(subItemsString, 1);
            else
                lvi = new ListViewItem(subItemsString, 0);
            this.lv_wrongList.Items.Add(lvi);
        }
        // 错误标签
        private void ts_wrong_CheckedChanged(object sender, EventArgs e)
        {
            SetListview();
        }
        // 警告标签
        private void ts_warning_CheckedChanged(object sender, EventArgs e)
        {
            SetListview();
        }
        // 消息标签
        private void ts_info_CheckedChanged(object sender, EventArgs e)
        {
            SetListview();
        }

        /// <summary>
        /// 双击后返回一个行号,高亮显示的行号和文件名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void lv_wrongList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 列表中第三项为行号
            if (this.lv_wrongList.SelectedItems.Count > 0)
            {
                ListViewItem lvi = this.lv_wrongList.SelectedItems[0];
                int nubmer = -1;
                // 不是整数
                if (!int.TryParse(lvi.SubItems[3].Text, out nubmer))
                {
                    for (int i = 0; i < ViewErrorInfo.Count; i++)
                    {
                        if (ViewErrorInfo[i][2] == lvi.SubItems[2].Text)
                        {
                            this.selectCtrlinfo = ViewErrorInfo[i];
                        }
                    }
                    highLightLine = -1;  // 没行号返回-1
                }
                else
                {
                    highLightLine = nubmer - 1;
                    this.selectCtrlinfo = null;
                }
                fileName = lvi.SubItems[4].Text;
            }

            if (this.LineDoubleClick != null)
                this.LineDoubleClick(sender, e);
        }

        /// <summary>
        /// 清空列表和三种提示的个数
        /// </summary> 
        public void ClearList()
        {
            this.lv_wrongList.Items.Clear();
            this.ts_warning.Text = "0个警告";
            this.ts_wrong.Text = "0个错误";
            this.ts_info.Text = "0个消息";
        }




    }
}
