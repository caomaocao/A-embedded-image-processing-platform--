using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ControlTactic.SpecialControl
{
    public partial class FuzzyForm1 : Form
    {       
        public FuzzyStruct newStruct = new FuzzyStruct();//存放传入的结构体,最终用于传出外层读取
        private string[,] ControlTableBox = null;//模糊控制表
        private List<string> Rows = new List<string>();//偏差模糊论域列表
        private List<string> Columns = new List<string>();//偏差变化率模糊论域列表
        private string tempRowText = null;//当前偏差模糊论域的值
        private string tempColumnText = null;//当前偏差变化率模糊论域的值

        /// <summary>
        /// 根据传入的结构体初始化窗口的值
        /// </summary>
        /// <param name="FStruct">传入的存放模糊控制块各值的结构体</param>
        public FuzzyForm1(FuzzyStruct FStruct)
        {
            InitializeComponent();
            //结构体副本
            newStruct = FStruct;
            //偏差模糊论域和偏差变化率模糊论域
            RowValue.Text = FStruct.RowNum.ToString();
            ColumnValue.Text = FStruct.ColumnNum.ToString();

            //偏差模糊论域值
            Rows.Clear();
            int countR = Convert.ToInt32(FStruct.RowNum);
            for (int x = -1 * countR; x <= countR; x++)
            { Rows.Add(x.ToString()); }
            //偏差变化率模糊论域值
            Columns.Clear();
            int countC = Convert.ToInt32(FStruct.ColumnNum);
            for (int y = -1 * countC; y <= countC; y++)
            { Columns.Add(y.ToString()); }

            //偏差模糊论域数和偏差变化率模糊论域数
            tempRowText = RowValue.Text;
            tempColumnText = ColumnValue.Text;
            //模糊控制表
            ControlTableBox = FStruct.ControlTable;
        }

        /// <summary>
        /// 偏差模糊论域值修改及确认事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ModifyRow_Click(object sender, EventArgs e)
        {
            if (ModifyRow.Text == "修改")
            {
                RowValue.ReadOnly = false;
                ModifyRow.Text = "确认";
                tempRowText = RowValue.Text;
                SetTable.Enabled = false;
            }
            else if (ModifyRow.Text == "确认")
            {
                Rows = ChangeToArray(RowValue.Text);
                if (Rows.Count == 0)
                { RowValue.Text = tempRowText; }
                else
                { ReSetTable(); }
                RowValue.ReadOnly = true;
                SetTable.Enabled = true;
                ModifyRow.Text = "修改";
            }
        }

        /// <summary>
        /// 偏差变化率模糊论域值修改及确认事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ModifyColumn_Click(object sender, EventArgs e)
        {
            if (ModifyColumn.Text == "修改")
            {
                ColumnValue.ReadOnly = false;
                ModifyColumn.Text = "确认";
                tempColumnText = ColumnValue.Text;
                SetTable.Enabled = false;
            }
            else if (ModifyColumn.Text == "确认")
            {
                Columns = ChangeToArray(ColumnValue.Text);
                if (Columns.Count == 0)
                { ColumnValue.Text = tempColumnText; }
                else
                { ReSetTable(); }
                ColumnValue.ReadOnly = true;
                SetTable.Enabled = true;
                ModifyColumn.Text = "修改";
            }
        }

        /// <summary>
        /// 根据当前的模糊控制表数据生成新窗口的模糊控制表
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void SetTable_Click(object sender, EventArgs e)
        {
            FuzzyForm2 frm = new FuzzyForm2();
            frm.rows = Rows;
            frm.columns = Columns;
            frm.OutputBox = ControlTableBox;
            frm.CreateTable(ControlTableBox);
            frm.ShowDialog();
            ControlTableBox = frm.OutputBox;
        }

        /// <summary>
        /// 当偏差模糊论域或偏差变化率模糊论域改变时，重置表格属性
        /// </summary>
        private void ReSetTable()
        {
            string[,] tempBox = new string[Rows.Count, Columns.Count];
            for (int i = 0; i < Rows.Count; i++)
            {
                for (int j = 0; j < Columns.Count; j++)
                { tempBox[i,j] = "0"; }
            }
            ControlTableBox = tempBox;
        }

        /// <summary>
        /// 当偏差模糊论域或偏差变化率模糊论域改变后确认时,将文本数据化转变并检查是否出错
        /// </summary>
        /// <param name="RegionText">新的模糊论域值</param>
        /// <returns>返回无错的列表</returns>
        private List<string> ChangeToArray(string RegionText)
        {
            List<string> EndValue = new List<string>();
            try
            {
                int count = Convert.ToInt32(RegionText) ;
                for (int i = -1 * count; i <= count; i++)
                {//检验是否可以转换成整形
                    EndValue.Add(i.ToString());
                }
            }
            catch
            {
                EndValue.Clear();
                MessageBox.Show("输入错误，请输入整数！", "错误！",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return EndValue;
        }

        /// <summary>
        /// 确认事件,把数据写入结构的副本,用来外部读取
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>   
        private void OK_Click(object sender, EventArgs e)
        {
            newStruct.ColumnNum = Columns.Count / 2;
            newStruct.RowNum = Rows.Count / 2;
            //newStruct.ColumnValue = Columns;
            //newStruct.RowValue = Rows;
            newStruct.ControlTable = ControlTableBox;
        }
    }
}