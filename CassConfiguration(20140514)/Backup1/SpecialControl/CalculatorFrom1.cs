using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ControlTactic.SpecialControl
{
    public partial class CalculatorFrom1 : Form
    {
        public List<List<string>> newList = new List<List<string>>();//存放传入的列表
        private string[,] Conditions = new string[10, 3];//存放10个条件内容的数组
        private bool IsSelect = false;//当前条件名是否被选择
        private int selectIndex = 0;//当先所选择的条件序号
        private string tmpSelect = "";//存放选择过程中的条件名

        /// <summary>
        /// 根据传入的列表初始化
        /// </summary>
        /// <param name="CList">传入的存放计算器组态各条件属性的列表</param>
        public CalculatorFrom1(List<List<string>> CList)
        {
            InitializeComponent();
            newList = CList;
            if (newList.Count != 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 3; j++)
                    { Conditions[i, j] = newList[i][j]; }
                }
            }
        }

        /// <summary>
        /// 设置计算条件表达式的事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void SETExpression1_Click(object sender, EventArgs e)
        {
            CalculatorForm2 frm = new CalculatorForm2();
            frm.tempExp = Condition.Text;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK && frm.tempExp != "")
            {
                Condition.Text = frm.tempExp;
                Conditions[selectIndex, 0] = Condition.Text;
            }
        }

        /// <summary>
        /// 设置结果表达式的事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void SETExpression2_Click(object sender, EventArgs e)
        {
            CalculatorForm2 frm = new CalculatorForm2();
            frm.tempExp = Result.Text;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK && frm.tempExp != "")
            {
               Result.Text = frm.tempExp;
               Conditions[selectIndex, 1] = Result.Text;
            }
        }

        /// <summary>
        /// 条件窗口中的点击事件,条件切换选定事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ChooseConditon_MouseClick(object sender, MouseEventArgs e)
        {
            ResultBox.Enabled = true;
            SETExpression1.Enabled = true;
            SETExpression2.Enabled = true;
            if (IsSelect == true)
            {
                ChooseConditon.Items[selectIndex] = tmpSelect;                
                IsSelect = false;
            }
            if (ChooseConditon.SelectedIndex != -1)
            {
                tmpSelect = ChooseConditon.Items[ChooseConditon.SelectedIndex].ToString();
                selectIndex = ChooseConditon.SelectedIndex;
                ChooseConditon.Items[selectIndex] = "*" + tmpSelect + "*";
                IsSelect = true;
            }
            ResultBox.Text = Conditions[selectIndex, 2];
            Condition.Text = Conditions[selectIndex, 0];
            Result.Text = Conditions[selectIndex, 1];
        }

        /// <summary>
        /// 确认事件,把数据写入列表的副本,用来外部读取
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param> 
        private void OK_Click(object sender, EventArgs e)
        {
            newList.Clear();
            for (int i = 0; i < 10; i++)
            {
                List<string> tempCondition = new List<string>();
                for (int j = 0; j < 3; j++)
                {
                    tempCondition.Add(Conditions[i, j]);
                }
                newList.Add(tempCondition);
            }
        }

        /// <summary>
        /// 计算结果下拉选择事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ResultBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ResultBox.Text != "")
            { Conditions[selectIndex, 2] = ResultBox.Text; }
        }
    }
}