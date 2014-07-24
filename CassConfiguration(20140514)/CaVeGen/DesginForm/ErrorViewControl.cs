using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
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
                            else if (element[3] != null)
                            {
                                AddWrongSentence(element[1], true, element[3].ToUpper());
                            }
                            else
                            {
                                AddWrongSentence(element[1], true,"");
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
        #region 代码优化(未完)
        private List<string[]> errorMsg = null;   //错误信息列表
        private List<string[]> warningMsg = null;  //警告信息列表
        private List<string[]> infoMsg = null;    //消息信息列表
        
        public void GetListView()
        {
            //清空
            ClearList();
            int errorCount = 0;
            int warningCount = 0;
            int infoCount = 0;
            //初始化
            errorMsg = new List<string[]>();
            warningMsg = new List<string[]>();
            infoMsg = new List<string[]>();

            // 根据errorList 筛选相关提示信息
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
                            else if (element[3] != null)
                            {
                                AddWrongSentence(element[1], true, element[3].ToUpper());
                            }
                            else
                            {
                                AddWrongSentence(element[1], true, "");
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
        /// 根据类型，显示不同的消息
        /// </summary>
        /// <param name="type"></param>
        private void ShowListView(string type)
        {
            
            switch (type)
            {           
                case "error":
                    HandleErrorMsg();
                    SetInfoList(this.errorMsg,0);
                    break;
                case"warning":
                    HandleWarningMsg();
                    SetInfoList(this.warningMsg,1);
                    break;
                case "info":
                    HandleInfoMsg();
                    SetInfoList(this.infoMsg,2);
                    break;
            }
          
        }

        private  void FormatSubItem()
        {

        }

        //定义一个数据结构
        public struct MsgStruct 
        {
           public string imageIndex ;   //图标编号
           public string index;       //错误索引
           public string context;     //错误内容
           public  string lineNum;    //行号
           public  string pageName;   //页面名
        };

        private void HandleErrorMsg()
        {
           int count = 0;
           string[] subItemsString = null;
           List<string[]> tempList = new List<string[]>();
           MsgStruct msg = new MsgStruct();
           msg.imageIndex = "1";
           foreach (string[] temp in this.errorMsg)
            {
                msg.index = (count++).ToString();
                msg.context = temp[1];
 
                if (temp.Length == 3)
                {

                    msg.lineNum = temp[0];
                    msg.pageName = null;
                    if (temp[0] != null)
                    {
                        
                        AddWrongSentence(temp[1], Convert.ToInt32(temp[0]), true, null);
                       
                        tempList.Add(subItemsString);
                    }
                    else
                    {
                        AddWrongSentence(temp[1], true, null);
                    }
                      tempList.Add(subItemsString);
                }
                else
                {
                    if (temp[0] != null)
                    {
                        AddWrongSentence(temp[1], Convert.ToInt32(temp[0]), true, temp[3].ToUpper());
                    }
                    else if (temp[3] != null)
                    {
                        AddWrongSentence(temp[1], true, temp[3].ToUpper());
                    }
                    else
                    {
                        AddWrongSentence(temp[1], true, "");
                    }
                }
            }
        }

        private void HandleWarningMsg()
        {
            foreach (string[] temp in this.warningMsg)
            {
                if (temp.Length == 3)
                {
                    AddWrongSentence(temp[1], Convert.ToInt32(temp[0]), false, null);
                }
                else
                {
                    if (temp[0] != null)
                    {
                        AddWrongSentence(temp[1], Convert.ToInt32(temp[0]), false, temp[3].ToUpper());
                    }
                    else
                    {
                        AddWrongSentence(temp[1], false, temp[3].ToUpper());
                    }
                }
            }

        }

        private void HandleInfoMsg()
        {
            foreach (string[] temp in this.infoMsg)
            {
                string[] subItemsString = new string[] { "", (this.lv_wrongList.Items.Count + 1).ToString(), temp[1], null, null };
                this.lv_wrongList.Items.Add(new ListViewItem(subItemsString, 2));
            }
        }

        private void SetInfoList(List<string[]> myList,int imageIndex)
        {
            foreach (string[] temp in myList)
            {
                this.lv_wrongList.Items.Add(new ListViewItem(temp, imageIndex));
            }
        }

        #endregion

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

       
        /// <summary>
        /// 添加出错语句,有行号的,有提示的
        /// </summary>
        /// <param name="wrongContext">错误内容</param>
        /// <param name="wrongNumber">错误序号</param>
        /// <param name="tip">点击某一项，给出提示</param>
        /// <param name="isWrong">是否有错</param>
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
      
        /// <summary>
        ///  添加出错语句,有行号的
        /// </summary>
        /// <param name="wrongContext">错误内容</param>
        /// <param name="wrongNumber">错误序号</param>
        /// <param name="isWrong">是否有错</param>
        /// <param name="pageName">页面名</param>
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
       
        /// <summary>
        /// 添加出错语句,无行号的
        /// </summary>
        /// <param name="wrongContext">错误内容</param>
        /// <param name="isWrong">是否有错</param>
        /// <param name="pageName">页面名</param>
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
