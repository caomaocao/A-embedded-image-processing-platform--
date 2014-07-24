using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ControlTactic.SpecialControl
{
    public partial class ProcessForm2 : Form
    {
        private string[] Ufunctions ={ "ABS", "ACOS", "ASIN", "ATAN","SIN","COS", "TAN", "LN", "LG", "SQRT", "TRUN", "EXP" };//一元函数
        private string[] Bfunctions ={ "MAX", "MIN", "POW" };//二元函数
        private string[] ArithmeticOperation ={ "+", "-", "*", "/", "%" };//算术运算
        private string[] LogicOperation ={ "(", ")", ",", "<", "<=", "==", "<>", ">", ">=", "&&", "||", "!" };//逻辑运算
        private List<List<string>> tempAttribute;//传入的控件属性的副本用来查找英文属性
        public string tempExp = null;//存放表达式
        
        /// <summary>
        /// 初始化函数根据传入的控件属性列表生成一个树
        /// </summary>
        /// <param name="ControlsAttribute">传入的控件属性列表</param>
        public ProcessForm2(List<List<string>> ControlsAttribute,string Exp)
        {
            InitializeComponent();
            
            ExpressBox.Text = Exp;
            ElementType.Text = "一元函数";
            this.tempAttribute = ControlsAttribute;

            //添加树节点！
            if (ControlsAttribute != null)
            {                
                foreach (List<string> Control in ControlsAttribute)
                {
                    TreeNode ControlNode = new TreeNode();
                    ControlNode.Text = Control[0];
                    for (int i = 1; i < Control.Count; i++)
                    {
                        TreeNode ControlAtrribute = new TreeNode();
                        ControlAtrribute.Text = Control[i].Split(',')[0];
                        ControlNode.Nodes.Add(ControlAtrribute);
                    }
                    ControlTree.Nodes[0].Nodes.Add(ControlNode);
                }                
            }
        }

        /// <summary>
        /// 窗口右侧CombBox下拉菜单改变时触发的事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ElementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] temp = null;
            Element.Items.Clear();
            if (ElementType.SelectedItem.ToString() == "算术运算")
            { temp = ArithmeticOperation; }
            else if (ElementType.SelectedItem.ToString() == "逻辑运算")
            { temp = LogicOperation; }
            else if (ElementType.SelectedItem.ToString() == "一元函数")
            { temp = Ufunctions; }
            else if (ElementType.SelectedItem.ToString() == "二元函数")
            { temp = Bfunctions; }
            foreach (string item in temp)
            { Element.Items.Add(item); }
        }

        /// <summary>
        /// 点击对应的函数或则运算符在表达式进行对应的显示
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void Element_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ElementType.Text == "一元函数")
            {
                foreach (string Func in Ufunctions)
                {
                    if (Element.SelectedItem.ToString() == Func)
                    {
                        ExpressBox.Text = Element.SelectedItem.ToString() + "(" + ExpressBox.Text + ")";
                        break;
                    }
                }  
            }
            else if (ElementType.Text == "二元函数")
            {
              foreach (string Func in Bfunctions)
                {
                    if (Element.SelectedItem.ToString() == Func)
                    {
                        ExpressBox.Text = Element.SelectedItem.ToString() + "(" + ExpressBox.Text + ", " + ")";
                        break;
                    }
                }     
            }
            else
                ExpressBox.Text += Element.SelectedItem.ToString();   
        }

        /// <summary>
        /// 左侧点击对应的树节点的控件属性在表达式显示对应的(控件名.属性)
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ControlTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode SelectNode = ControlTree.SelectedNode;
            string ReturnValue = null;
            if (SelectNode != null && SelectNode.Nodes.Count == 0)
            {
                foreach (List<string> TmpControl in tempAttribute)
                {
                    if (SelectNode.Parent.Text == TmpControl[0])
                    {
                        ReturnValue = TmpControl[0] + ".";
                        foreach (string Attribute in TmpControl)
                        {
                            if (Attribute.Split(',')[0] == SelectNode.Text)
                            {
                                ReturnValue += Attribute.Split(',')[1];
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            ExpressBox.Text += ReturnValue;            
        }

        /// <summary>
        /// 确认时将表达式放入tmpExp中被外层读取
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void OK_Click(object sender, EventArgs e)
        {
            tempExp = ExpressBox.Text;
        }        
    }
}