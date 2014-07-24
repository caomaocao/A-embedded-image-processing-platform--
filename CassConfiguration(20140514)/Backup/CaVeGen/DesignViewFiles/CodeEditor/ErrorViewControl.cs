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
        public List<string[]> errorList = new List<string[]>();//��Ŵ���;�����Ϣ
        public List<string[]> ViewErrorInfo = new List<string[]>();//���ӱ�̴�����Ϣ
        public int errorNum = 0;//����ĸ���
        //public int warningNum = 0;//����ĸ���
        //public int infoNum = 0; //��Ϣ�ĸ���

        // ������ʾ���к�
        int highLightLine;
        public int HighLightLine
        {
            get
            {
                return highLightLine;
            }
        }

        // ��Ҫ�ҵ��Ŀؼ�
        // ���鳤��2 ҳ������
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



        // ÿҳ�������ѵĴ�����Ϣ,��ҳ���л�ʱ,��ʾ��ͬ�Ĵ�����Ϣ
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

        // �ļ���
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


        // ���һ����Ϣ���б���
        //private void AddToList(CompileClass.WrongInfoStruct wis, bool state)
        //{
        //    // ���к�
        //    if (wis.lineNumber > 0)
        //    {
        //        // ����ʾ
        //        if (wis.tip == "")
        //            AddWrongSentence(wis.wrongContent, wis.lineNumber, state);
        //        else  // ����ʾ
        //            AddWrongSentence(wis.wrongContent, wis.lineNumber, wis.tip, state);
        //    }
        //    else // ���к�
        //        AddWrongSentence(wis.wrongContent, state);

        //}

        /// <summary>
        /// ��ʾ�����б��еĴ���;���
        /// </summary> 
        public void SetListview()
        {
            ClearList();//���
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

            // ���ñ�ǩ
            this.ts_wrong.Text = errorCount.ToString() + "������";
            this.ts_warning.Text = warningCount.ToString() + "������";
            this.ts_info.Text = infoCount.ToString() + "����Ϣ";
        }

        /// <summary>
        /// ��⵱ǰ�����б��еĴ������
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

        // ��ӳ������,���кŵ�,����ʾ��
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
            lvi.ToolTipText = tip; // ������ʾ    
            this.lv_wrongList.Items.Add(lvi);

        }
        // ��ӳ������,���кŵ�
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
        // ��ӳ������,���кŵ�
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
        // �����ǩ
        private void ts_wrong_CheckedChanged(object sender, EventArgs e)
        {
            SetListview();
        }
        // �����ǩ
        private void ts_warning_CheckedChanged(object sender, EventArgs e)
        {
            SetListview();
        }
        // ��Ϣ��ǩ
        private void ts_info_CheckedChanged(object sender, EventArgs e)
        {
            SetListview();
        }

        /// <summary>
        /// ˫���󷵻�һ���к�,������ʾ���кź��ļ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void lv_wrongList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // �б��е�����Ϊ�к�
            if (this.lv_wrongList.SelectedItems.Count > 0)
            {
                ListViewItem lvi = this.lv_wrongList.SelectedItems[0];
                int nubmer = -1;
                // ��������
                if (!int.TryParse(lvi.SubItems[3].Text, out nubmer))
                {
                    for (int i = 0; i < ViewErrorInfo.Count; i++)
                    {
                        if (ViewErrorInfo[i][2] == lvi.SubItems[2].Text)
                        {
                            this.selectCtrlinfo = ViewErrorInfo[i];
                        }
                    }
                    highLightLine = -1;  // û�кŷ���-1
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
        /// ����б��������ʾ�ĸ���
        /// </summary> 
        public void ClearList()
        {
            this.lv_wrongList.Items.Clear();
            this.ts_warning.Text = "0������";
            this.ts_wrong.Text = "0������";
            this.ts_info.Text = "0����Ϣ";
        }




    }
}
