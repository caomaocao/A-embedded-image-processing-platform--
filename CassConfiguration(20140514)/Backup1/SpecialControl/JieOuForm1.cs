using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ControlTactic.SpecialControl
{
    public partial class JieOuForm1 : Form
    {
        public JieOuStruct newStruct = new JieOuStruct();//存放传入的结构体,最终用于传出外层读取
        private List<List<string>> JieOuBox = new List<List<string>>();//各路解耦的数据列表
        private string[,] JieOuTable = null;//解耦矩阵
        private int NumOfJieOu;//解耦路数
        private int SaveNum = -1;//所保存的解耦路
        private string LastText = null;//存放上一次正确的解耦路数
        public static List<string> Original
            = new List<string>(new string[] { "1", "100000", "0", "50", "自动", "100", "0", "0" });//存放新建解耦路数时的初值

        /// <summary>
        /// 根绝传入的结构体初始化窗口的值
        /// </summary>
        /// <param name="JStruct">传入的存放解耦控制块各值的结构体</param>
        public JieOuForm1(JieOuStruct JStruct)
        {
            InitializeComponent();
            //结构体副本
            newStruct = JStruct;
            //解耦路数
            NumOfJieOu = JStruct.JieOuNum;
            LastText = JStruct.JieOuNum.ToString();
            JieOuNum.Text = LastText;
            //解耦各路数据列表
            JieOuBox = JStruct.JieOuAttribute;
            for (int i = 1; i <= NumOfJieOu; i++)
            {
                SelectValue.Items.Add("解耦第" + i + "路");
            }
            SelectValue.Text = "解耦第1路";
            //解耦矩阵
            JieOuTable = JStruct.JieOuTable;
            createMatrixBox(NumOfJieOu, true);
        }      

        /// <summary>
        /// 选择解耦路数更变事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void SelectValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SaveNum != -1)
            {
               List< string> tempJieOu = new List<string> ();
                tempJieOu.Add(fKp_Value.Text);
                tempJieOu.Add(fKi_Value.Text);
                tempJieOu.Add(fKd_Value.Text);
                tempJieOu.Add(fSV_Value.Text);
                tempJieOu.Add(cCtrlMod_Value.Text);
                tempJieOu.Add(fMH_Value.Text);
                tempJieOu.Add(fML_Value.Text);
                tempJieOu.Add(fMV_Value.Text);
                JieOuBox[SaveNum] = tempJieOu;
            }

            SaveNum = SelectValue.SelectedIndex;
            fKp_Value.Text = JieOuBox[SaveNum][0];
            fKi_Value.Text = JieOuBox[SaveNum][1];
            fKd_Value.Text = JieOuBox[SaveNum][2];
            fSV_Value.Text = JieOuBox[SaveNum][3];
            cCtrlMod_Value.Text = JieOuBox[SaveNum][4];
            fMH_Value.Text = JieOuBox[SaveNum][5];
            fML_Value.Text = JieOuBox[SaveNum][6];
            fMV_Value.Text = JieOuBox[SaveNum][7];
        }

        /// <summary>
        /// 根据解耦数生成解耦矩阵
        /// </summary>
        /// <param name="Num">解耦路数</param>
        /// <param name="FirstTime">是否时初次生成</param>
        private void createMatrixBox(int Num, bool FirstTime)
        {
            JieOuMatrix.Rows.Clear();
            JieOuMatrix.Columns.Clear();
            //2/2矩阵生成3/3表格 多一行一列用于标示

            DataGridViewTextBoxColumn newFirstColumn = new DataGridViewTextBoxColumn();
            newFirstColumn.ReadOnly = true;
            JieOuMatrix.Columns.Add(newFirstColumn);

            for (int column = 0; column < Num; column++)
            {
                DataGridViewTextBoxColumn newColumn = new DataGridViewTextBoxColumn();
                newColumn.Name = "第" + (column + 1) + "解耦";
                JieOuMatrix.Columns.Add(newColumn);
            }
            for (int row = 0; row < Num; row++)
            {
                JieOuMatrix.Rows.Add();
                JieOuMatrix.Rows[row].Cells[0].Value = "第" + (row + 1) + "解耦";
                for (int column = 1; column <= Num; column++)
                {//只有初次附值才赋值
                    if (FirstTime == true)
                    { JieOuMatrix.Rows[row].Cells[column].Value = JieOuTable[row, column - 1]; }
                    else
                    { JieOuMatrix.Rows[row].Cells[column].Value = "0"; }
                }
            }
        }

        /// <summary>
        /// 修改解耦路数事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ModifyNum_Click(object sender, EventArgs e)
        {
            if (ModifyNum.Text == "设置")
            {
                JieOuNum.ReadOnly = false;
                ModifyNum.Text = "确认";         
            }
            else if (ModifyNum.Text == "确认")
            {
                try
                {
                    int num = Convert.ToInt32(JieOuNum.Text);
                    if (num >= 2 && num <= 10)
                    {
                        NumOfJieOu = Convert.ToInt32(JieOuNum.Text);
                        SelectValue.Items.Clear();
                        JieOuBox.Clear();

                        for (int j = 1; j <= NumOfJieOu; j++)
                        {
                            SelectValue.Items.Add("解耦第" + j + "路");
                            JieOuBox.Add(Original);
                        }
                        SelectValue.Text = "解耦第1路";
                        createMatrixBox(NumOfJieOu, false);
                        LastText = JieOuNum.Text;
                    }
                    else
                    {
                        MessageBox.Show("解耦数必须大于1小于11！");
                        JieOuNum.Text = LastText;
                    }
                }
                catch
                {
                    MessageBox.Show("输入必须为大于1小于11的正整数！");
                    JieOuNum.Text = LastText;
                }
                finally
                {
                    JieOuNum.ReadOnly = true;
                    ModifyNum.Text = "设置";
                }
            }
        }

        /// <summary>
        /// 确认事件,把数据写入结构的副本,用来外部读取
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>   
        private void OK_Click(object sender, EventArgs e)
        {
            newStruct.JieOuNum = NumOfJieOu;

            JieOuBox[SaveNum][0] = fKp_Value.Text;
            JieOuBox[SaveNum][1] = fKi_Value.Text;
            JieOuBox[SaveNum][2] = fKd_Value.Text;
            JieOuBox[SaveNum][3] = fSV_Value.Text;
            JieOuBox[SaveNum][4] = cCtrlMod_Value.Text;
            JieOuBox[SaveNum][5] = fMH_Value.Text;
            JieOuBox[SaveNum][6] = fML_Value.Text;
            JieOuBox[SaveNum][7] = fMV_Value.Text;
            newStruct.JieOuAttribute = JieOuBox;

            string[,] tempTable = new string[NumOfJieOu, NumOfJieOu];
            for (int row = 0; row < NumOfJieOu; row++)
                for (int column = 1; column <= NumOfJieOu; column++)
                    tempTable[row, column - 1] = JieOuMatrix.Rows[row].Cells[column].Value.ToString();

            newStruct.JieOuTable = tempTable;
        }
    }
}