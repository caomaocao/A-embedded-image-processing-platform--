using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ControlTactic.SpecialControl
{
    public partial class ProcessForm1 : Form
    {
        public ProcessStruct newStruct = new ProcessStruct();//存放传入的结构体，最终用于传出外层读取
        private int NumCondition;//条件数
        private int NumAction;//动作数
        private bool IsOnlyStart = true;//是否选择＂只在开始时计算一次＂
        private List<List<string>> ControlAtrribute;//存放控件属性列表
        private string[,] OrderBox;//条件为列动作为行的顺序表
        private List<string> Condition = new List<string>();//条件表达式列表
        private List<List<string>> Action = new List<List<string>>();//动作表达式列表

        private int CselectIndex = 0;//当前所选择的条件序号
        private int AselectIndex = 0;//当前所选择的动作序号
        private string ConditionSelect = "";//存放选择过程中的条件名
        private string ActionSelect = "";//存放选择过程中的动作名
        private bool IsCselect = false;//当前的条件名是否被选择
        private bool IsAselect = false;//当前的动作名是否被选择


        /// <summary>
        /// 根据传入的结构体初始化窗口的值
        /// </summary>
        /// <param name="PStruct">传入的存放条件动作块各值的结构体</param>
        public ProcessForm1(ProcessStruct PStruct)
        {
            InitializeComponent();
            //结构体副本
            newStruct = PStruct;
            //条件数和动作数
            NumCondition = PStruct.NumOfConditions;
            NumAction = PStruct.NumOfActions;
            ConditionNum.Text = NumCondition.ToString();
            ActionNum.Text = NumAction.ToString();
            //根据条件数和动作数生成对应的列表
            CreateConditionList(NumCondition);
            CreateActionList(NumAction);
            //选择是否只在开始计算一次
            if (PStruct.IsOnlyStart == true)
            { OnlyStartType.Select(); }
            else if (PStruct.IsOnlyStart == false)
            { EveryTimeType.Select(); }
            //条件表达式和动作表达式的副本
            Action = PStruct.Actions;
            Condition = PStruct.Conditions;
            //顺序表
            if (PStruct.OrderBox.Length != 0)
            {
                OrderBox = PStruct.OrderBox;
            }
            else
            { OrderBox = new string[NumCondition, NumAction]; }
            CreateBox(NumCondition, NumAction);
            for (int i = 0; i < NumCondition; i++)
            {
                for (int j = 0; j < NumAction; j++)
                { OrderDefineBox.Rows[i].Cells[j + 1].Value = OrderBox[i, j]; }
            }
            //当前条件动作模块所在的页面的子策略
            if (PStruct.Tactic.Count != 0)
            { TacticBox.Items.AddRange(PStruct.Tactic.ToArray()); }
            //所在页面的所有控件属性
            ControlAtrribute = PStruct.ControlAttribute;
        }

        /// <summary>
        /// 条件数修改的事件包括数目修改后的列表和顺序表再建
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ModifyConditionNum_Click(object sender, EventArgs e)
        {
            if (ModifyConditionNum.Text == "设置")
            {
                ConditionNum.ReadOnly = false;
                ModifyConditionNum.Text = "确定";
            }
            else if (ModifyConditionNum.Text == "确定")
            {
                try
                {
                    if (ConditionNum.Text != NumCondition.ToString())
                    {
                        int tempCNum = Convert.ToInt32(ConditionNum.Text);
                        if (tempCNum <= 0 || tempCNum > 32)
                        {
                            MessageBox.Show("请输入1-32中的正整数！");
                            ConditionNum.Text = NumCondition.ToString();
                        }
                        else
                        {
                            
                            CreateConditionList(tempCNum);
                            //OrderBox = new string[NumCondition, NumAction];
                            CreateBox(tempCNum, NumAction);
                            NumCondition = tempCNum;//创建完顺序表后再更新条件数
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("请输入正整数！");
                }
                finally
                {
                    ConditionNum.ReadOnly = true;
                    ModifyConditionNum.Text = "设置";
                }
            }
        }

        /// <summary>
        /// 动作数修改的事件包括数目修改后的列表和顺序表再建
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ModifyActionNum_Click(object sender, EventArgs e)
        {
            if (ModifyActionNum.Text == "设置")
            {
                ActionNum.ReadOnly = false;
                ModifyActionNum.Text = "确定";
            }
            else if (ModifyActionNum.Text == "确定")
            {
                try
                {
                    if (ActionNum.Text != NumAction.ToString())
                    {
                        int tempANum = Convert.ToInt32(ActionNum.Text);
                        if (tempANum <= 0 || tempANum > 32)
                        {
                            MessageBox.Show("请输入1-32中的正整数！");
                            ActionNum.Text = NumAction.ToString();
                        }
                        else
                        {                        
                            CreateActionList(tempANum);
                            //OrderBox = new string[NumCondition, NumAction];
                            CreateBox(NumCondition, tempANum); 
                            NumAction = tempANum;//创建完顺序表后再更新动作数
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("请输入正整数！");
                }
                finally
                {
                    ActionNum.ReadOnly = true;
                    ModifyActionNum.Text = "设置";
                }
            }
        }

        /// <summary>
        /// 条件执行方式选择事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void OnlyStartType_CheckedChanged(object sender, EventArgs e)
        {
            this.IsOnlyStart = true;
        }

        /// <summary>
        /// 条件执行方式选择事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void EveryTimeType_CheckedChanged(object sender, EventArgs e)
        {
            this.IsOnlyStart = false;
        }

        /// <summary>
        /// 在条件列表中点击条件时的事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void ConditionList_MouseClick(object sender, MouseEventArgs e)
        {
            if (IsCselect == true)
            {
                ConditionList.Items[CselectIndex] = ConditionSelect;
                IsCselect = false;
            }
            if (ConditionList.SelectedIndex != -1)
            {
                AddCondition.Enabled = true;
                CselectIndex = ConditionList.SelectedIndex;
                ConditionSelect = ConditionList.Items[CselectIndex].ToString();
                ConditionList.Items[CselectIndex] = "*" + ConditionSelect + "*";
                IsCselect = true;
            }
            this.ConditionExp.Text = Condition[CselectIndex];
        }

        /// <summary>
        /// 在动作列表中点击条件时的事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>        
        private void ActionList_MouseClick(object sender, MouseEventArgs e)
        {
            if (IsAselect == true)
            {
                ActionList.Items[AselectIndex] = ActionSelect;
                IsAselect = false;
            }
            if (ActionList.SelectedIndex != -1)
            {
                Evaluate.Enabled = true;
                Transfer.Enabled = true;
                AddActionResult.Enabled = true;
                AddActionExp.Enabled = true;
                TacticBox.Enabled = true;
                AselectIndex = ActionList.SelectedIndex;
                ActionSelect = ActionList.Items[AselectIndex].ToString();
                ActionList.Items[AselectIndex] = "*" + ActionSelect + "*";
                IsAselect = true;
            }

            if (Action[AselectIndex][0] == "Evaluate" || Action[AselectIndex][0] == "")
            {
                this.Evaluate.Select();
                this.EvaluateExp.Text = Action[AselectIndex][1];
                this.ExpBox.Text = Action[AselectIndex][2];
            }
            else if (Action[AselectIndex][0] == "Transfer")
            {
                this.Transfer.Select();
                this.TacticBox.Text = Action[AselectIndex][1];
            }
        }

        /// <summary>
        /// 添加条件表达式事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>      
        private void AddCondition_Click(object sender, EventArgs e)
        {
            ProcessForm2 frm = new ProcessForm2(this.ControlAtrribute, Condition[CselectIndex]);
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                this.ConditionExp.Text = frm.tempExp;
                Condition[CselectIndex] = frm.tempExp;
            }
        }

        /// <summary>
        /// 子策略选择事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>      
        private void TacticBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TacticBox.Text != "")
            { Action[AselectIndex][1] = TacticBox.Text; }
        }

        /// <summary>
        /// 动作赋值结果修改事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>      
        private void AddActionResult_Click(object sender, EventArgs e)
        {
            ProcessForm2 frm = new ProcessForm2(this.ControlAtrribute, Action[AselectIndex][1]);
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                this.EvaluateExp.Text = frm.tempExp;
                Action[AselectIndex][1] = frm.tempExp;
            }
        }

        /// <summary>
        /// 添加动作表达式事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>      
        private void AddActionExp_Click(object sender, EventArgs e)
        {
            ProcessForm2 frm = new ProcessForm2(this.ControlAtrribute, Action[AselectIndex][2]);
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                this.ExpBox.Text = frm.tempExp;
                Action[AselectIndex][2] = frm.tempExp;
            }
        }

        /// <summary>
        /// 动作类型选择事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>      
        private void Evaluate_CheckedChanged(object sender, EventArgs e)
        {
            EvaluateLabel.Visible = true;
            AddActionResult.Visible = true;
            EvaluateExp.Visible = true;
            ExpLabel.Visible = true;
            AddActionExp.Visible = true;
            ExpBox.Visible = true;
            ChildTacticLabel.Visible = false;
            TacticBox.Visible = false;

            Action[AselectIndex][0] = "Evaluate";
        }

        /// <summary>
        /// 动作类型选择事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>      
        private void Transfer_CheckedChanged(object sender, EventArgs e)
        {
            EvaluateLabel.Visible = false;
            AddActionResult.Visible = false;
            EvaluateExp.Visible = false;
            ExpLabel.Visible = false;
            AddActionExp.Visible = false;
            ExpBox.Visible = false;
            ChildTacticLabel.Visible = true;
            TacticBox.Visible = true;

            Action[AselectIndex][0] = "Transfer";
        }

        /// <summary>
        /// 根据传入数创建对应数量的条件列表函数
        /// </summary>
        /// <param name="Num">条件数</param>
        private void CreateConditionList(int Num)
        {
            ConditionList.Items.Clear();
            List<string> newCondition = new List<string>();//新条件列表
            for (int i = 0; i < Num; i++)
            {
                ConditionList.Items.Add("条件" + (i + 1).ToString());
                if (i < Condition.Count)
                {
                    newCondition.Add(Condition[i]);
                }
                else
                { newCondition.Add(null); }
            }
            Condition = newCondition;
        }

        /// <summary>
        /// 根据传入数创建对应数量的动作列表函数
        /// </summary>
        /// <param name="Num">动作数</param>
        private void CreateActionList(int Num)
        {
            ActionList.Items.Clear();
            List<List<string>> newAction = new List<List<string>>();
            for (int i = 0; i < Num; i++)
            {
                ActionList.Items.Add("动作" + (i + 1).ToString());
                if (i < Action.Count)
                {
                    newAction.Add(Action[i]);
                }
                else
                {
                    Action.Add(new List<string>(new string[] { null, null, null }));
                }
            }
        }

        /// <summary>
        /// 由条件数和动作数在DataGridView中生成顺序表函数
        /// </summary>
        /// <param name="ActNum">创建顺序表的动作数</param>
        /// <param name="ConNum">创建顺序表的条件数</param>
        private void CreateBox(int ConNum,int ActNum)
        {//先更新顺序表,并保留为修改部分信息
            string[,] newOrderBox = new string[ConNum, ActNum];
            for (int aNum = 0; aNum < ActNum; aNum++)
            {
                for (int cNum = 0; cNum < ConNum; cNum++)
                {
                    if (aNum < NumAction && cNum < NumCondition)
                    {//当遍历位置在原有顺序表中存在则赋值
                        newOrderBox[cNum, aNum] = OrderBox[cNum, aNum];
                    }
                    else
                    { newOrderBox[cNum, aNum] = ""; }
                }
            }
            OrderBox = newOrderBox;
            //然后创新DATEVIEW中的信息并显示
            OrderDefineBox.Columns.Clear();
            OrderDefineBox.Rows.Clear();
            DataGridViewTextBoxColumn newConditionColumn = new DataGridViewTextBoxColumn();
            newConditionColumn.Name = "动作";
            newConditionColumn.ReadOnly = true;
            OrderDefineBox.Columns.Add(newConditionColumn);

            for (int i = 0; i < ActNum; i++)
            {
                DataGridViewComboBoxColumn newColum = new DataGridViewComboBoxColumn();
                newColum.Name = (i + 1).ToString();
                newColum.Items.AddRange(new string[] { "Y", "N", " " });
                OrderDefineBox.Columns.Add(newColum);
            }
            for (int i = 0; i < ConNum; i++)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.Resizable = DataGridViewTriState.False;
                OrderDefineBox.Rows.Add(newRow);
                OrderDefineBox.Rows[i].Cells[0].Value = "条件" + (i + 1).ToString();
                for (int j = 0; j < ActNum; j++)
                {
                    OrderDefineBox.Rows[i].Cells[j + 1].Value = OrderBox[i, j];
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
            newStruct.NumOfConditions = NumCondition;
            newStruct.NumOfActions = NumAction;
            newStruct.IsOnlyStart = this.IsOnlyStart;

            newStruct.Conditions = Condition;
            newStruct.Actions = Action;

            for (int row = 0; row < OrderDefineBox.Rows.Count; row++)
            {//条件为第一列，值从第二列开始
                for (int column = 1; column < OrderDefineBox.Columns.Count; column++)
                {//调整二位数组与表格之间的一列偏差
                    if (OrderDefineBox.Rows[row].Cells[column].Value != null)
                    { OrderBox[row, column - 1] = OrderDefineBox.Rows[row].Cells[column].Value.ToString(); }
                    else
                    { OrderBox[row, column - 1] = ""; }
                }
            }
            newStruct.OrderBox = OrderBox;
        }


    }
}