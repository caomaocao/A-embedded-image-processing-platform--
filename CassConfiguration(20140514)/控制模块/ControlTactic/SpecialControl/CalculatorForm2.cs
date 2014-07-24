using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ControlTactic.SpecialControl
{
    public partial class CalculatorForm2 : Form
    {
        private string[] Ufunctions ={ "ABS", "ACOS", "ASIN", "ATAN", "SIN", "COS", "TAN", "LN", "LG", "SQRT", "TRUN", "EXP" };//一元函数
        private string[] Bfunctions ={ "MAX", "MIN", "POW" };//二元函数
        private string[] ArithmeticOperation ={ "+", "-", "*", "/", "%" };//算术符号
        private string[] LogicOperation ={ "(", ")", ",", "<", "<=", "==", "<>", ">", ">=", "&&", "||", "!" };//逻辑符号
        private string[] InVariable ={ "X0", "X1", "X2", "X3", "X4", "X5", "X6", "X7" };//输入变量
        private string[] MiVariable ={ "M0", "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8", "M9" };//中间变量
        private string[] OuVariable ={ "Y0", "Y1", "Y2", "Y3" };//输出变量
        public string tempExp = null;//表达式

        public CalculatorForm2()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 表达式元素类型的下拉列表选择事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ElementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ElementType.SelectedItem.ToString() == "函数")
            {
                first.Text = "一元函数";
                second.Text = "二元函数";
                third.Text = "所有函数";
                forth.Visible = false;
                third.Select();
                Element.Items.Clear();
                foreach (string function in Ufunctions)
                { Element.Items.Add(function); }
                foreach (string function in Bfunctions)
                { Element.Items.Add(function); }
            }
            else if (ElementType.SelectedItem.ToString() == "变量")
            {
                first.Text = "输入变量";
                second.Text = "中间变量";
                third.Text = "输出变量";
                forth.Text = "所有变量";
                forth.Visible = true;
                forth.Select();
                Element.Items.Clear();
                foreach (string item in InVariable)
                { Element.Items.Add(item); }
                foreach (string item in MiVariable)
                { Element.Items.Add(item); }
                foreach (string item in OuVariable)
                { Element.Items.Add(item); }
            }
            else if (ElementType.SelectedItem.ToString() == "运算")
            {
                first.Text = "算术运算";
                second.Text = "逻辑运算";
                third.Text = "所有运算";
                forth.Visible = false;
                third.Select();
                Element.Items.Clear();
                foreach (string item in ArithmeticOperation)
                { Element.Items.Add(item); }
                foreach (string item in LogicOperation)
                { Element.Items.Add(item); }
            }
        }

        /// <summary>
        /// 第一个被选事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void first_CheckedChanged(object sender, EventArgs e)
        {
            Element.Items.Clear();
            if (ElementType.SelectedItem.ToString() == "变量")
            {//输入变量
                foreach (string item in InVariable)
                { Element.Items.Add(item); }
            }
            else if (ElementType.SelectedItem.ToString() == "运算")
            {//算术运算
                foreach (string item in ArithmeticOperation)
                { Element.Items.Add(item); }
            }
            else if (ElementType.SelectedItem.ToString() == "函数")
            {//一元函数
                foreach (string function in Ufunctions)
                { Element.Items.Add(function); }
            }
        }

        /// <summary>
        /// 第二个被选事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void second_CheckedChanged(object sender, EventArgs e)
        {
            Element.Items.Clear();
            if (ElementType.SelectedItem.ToString() == "变量")
            {//中间变量
                foreach (string item in MiVariable)
                { Element.Items.Add(item); }
            }
            else if (ElementType.SelectedItem.ToString() == "运算")
            {//逻辑运算
                foreach (string item in LogicOperation)
                { Element.Items.Add(item); }
            }
            else if (ElementType.SelectedItem.ToString() == "函数")
            {//二元函数
                foreach (string function in Bfunctions)
                { Element.Items.Add(function); }
            }
        }

        /// <summary>
        /// 第三个被选事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void third_CheckedChanged(object sender, EventArgs e)
        {
            Element.Items.Clear();
            if (ElementType.SelectedItem.ToString() == "变量")
            {//输出变量
                foreach (string item in OuVariable)
                { Element.Items.Add(item); }
            }
            else if (ElementType.SelectedItem.ToString() == "运算")
            {//所有运算
                foreach (string item in ArithmeticOperation)
                { Element.Items.Add(item); }
                foreach (string item in LogicOperation)
                { Element.Items.Add(item); }
            }
            else if (ElementType.SelectedItem.ToString() == "函数")
            {//所有函数
                foreach (string function in Ufunctions)
                { Element.Items.Add(function); }
                foreach (string function in Bfunctions)
                { Element.Items.Add(function); }
            }
        }

        /// <summary>
        /// 第四个被选事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void forth_CheckedChanged(object sender, EventArgs e)
        {//所有变量
            Element.Items.Clear();
            foreach (string item in InVariable)
            { Element.Items.Add(item); }
            foreach (string item in MiVariable)
            { Element.Items.Add(item); }
            foreach (string item in OuVariable)
            { Element.Items.Add(item); }
        }

        /// <summary>
        /// 点击窗口选择元素事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void Element_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ElementType.Text == "函数")
            {
                bool find = false;
                foreach (string Func in Ufunctions)
                {
                    if (Element.SelectedItem.ToString() == Func)
                    {
                        find = true;
                        ExpressBox.Text = Element.SelectedItem.ToString() + "(" + ExpressBox.Text + ")";
                        break;
                    }
                }
                if (find == false)
                {
                    ExpressBox.Text = Element.SelectedItem.ToString() + "(" + ExpressBox.Text + ", " + ")";
                }
            }
            else
                ExpressBox.Text += Element.SelectedItem.ToString();      
        }
        
        /// <summary>
        /// 确认事件,保存当前表达式
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void OK_Click(object sender, EventArgs e)
        {
            tempExp = ExpressBox.Text;
        }

        /// <summary>
        /// 当前窗体打开赋值
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void CalculatorForm2_Load(object sender, EventArgs e)
        {
            ExpressBox.Text = tempExp;
            ElementType.Text = "变量";
        }
    }
}