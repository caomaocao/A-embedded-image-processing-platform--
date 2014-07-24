namespace ControlTactic.SpecialControl
{
    partial class ProcessForm1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ProcessTab = new System.Windows.Forms.TabControl();
            this.PublicAttribute = new System.Windows.Forms.TabPage();
            this.ConditionAndAction = new System.Windows.Forms.GroupBox();
            this.ModifyActionNum = new System.Windows.Forms.Button();
            this.ModifyConditionNum = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ActionNum = new System.Windows.Forms.TextBox();
            this.ConditionNum = new System.Windows.Forms.TextBox();
            this.ConditionMethod = new System.Windows.Forms.GroupBox();
            this.EveryTimeType = new System.Windows.Forms.RadioButton();
            this.OnlyStartType = new System.Windows.Forms.RadioButton();
            this.ConditionDefine = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.ConditionExp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AddCondition = new System.Windows.Forms.Button();
            this.ConditionList = new System.Windows.Forms.ListBox();
            this.ActionDefine = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.TacticBox = new System.Windows.Forms.ComboBox();
            this.ChildTacticLabel = new System.Windows.Forms.Label();
            this.ExpLabel = new System.Windows.Forms.Label();
            this.EvaluateLabel = new System.Windows.Forms.Label();
            this.ExpBox = new System.Windows.Forms.TextBox();
            this.EvaluateExp = new System.Windows.Forms.TextBox();
            this.AddActionExp = new System.Windows.Forms.Button();
            this.AddActionResult = new System.Windows.Forms.Button();
            this.ActionType = new System.Windows.Forms.GroupBox();
            this.Transfer = new System.Windows.Forms.RadioButton();
            this.Evaluate = new System.Windows.Forms.RadioButton();
            this.ActionList = new System.Windows.Forms.ListBox();
            this.OrderDefine = new System.Windows.Forms.TabPage();
            this.OrderDefineBox = new System.Windows.Forms.DataGridView();
            this.actions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OK = new System.Windows.Forms.Button();
            this.Cencel = new System.Windows.Forms.Button();
            this.ProcessTab.SuspendLayout();
            this.PublicAttribute.SuspendLayout();
            this.ConditionAndAction.SuspendLayout();
            this.ConditionMethod.SuspendLayout();
            this.ConditionDefine.SuspendLayout();
            this.ActionDefine.SuspendLayout();
            this.ActionType.SuspendLayout();
            this.OrderDefine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OrderDefineBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ProcessTab
            // 
            this.ProcessTab.Controls.Add(this.PublicAttribute);
            this.ProcessTab.Controls.Add(this.ConditionDefine);
            this.ProcessTab.Controls.Add(this.ActionDefine);
            this.ProcessTab.Controls.Add(this.OrderDefine);
            this.ProcessTab.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProcessTab.Location = new System.Drawing.Point(0, 0);
            this.ProcessTab.MaximumSize = new System.Drawing.Size(344, 306);
            this.ProcessTab.MinimumSize = new System.Drawing.Size(344, 306);
            this.ProcessTab.Name = "ProcessTab";
            this.ProcessTab.SelectedIndex = 0;
            this.ProcessTab.Size = new System.Drawing.Size(344, 306);
            this.ProcessTab.TabIndex = 0;
            // 
            // PublicAttribute
            // 
            this.PublicAttribute.BackColor = System.Drawing.SystemColors.Control;
            this.PublicAttribute.Controls.Add(this.ConditionAndAction);
            this.PublicAttribute.Controls.Add(this.ConditionMethod);
            this.PublicAttribute.Location = new System.Drawing.Point(4, 21);
            this.PublicAttribute.Name = "PublicAttribute";
            this.PublicAttribute.Padding = new System.Windows.Forms.Padding(3);
            this.PublicAttribute.Size = new System.Drawing.Size(336, 281);
            this.PublicAttribute.TabIndex = 0;
            this.PublicAttribute.Text = "公共属性";
            // 
            // ConditionAndAction
            // 
            this.ConditionAndAction.Controls.Add(this.ModifyActionNum);
            this.ConditionAndAction.Controls.Add(this.ModifyConditionNum);
            this.ConditionAndAction.Controls.Add(this.label2);
            this.ConditionAndAction.Controls.Add(this.label1);
            this.ConditionAndAction.Controls.Add(this.ActionNum);
            this.ConditionAndAction.Controls.Add(this.ConditionNum);
            this.ConditionAndAction.Location = new System.Drawing.Point(32, 29);
            this.ConditionAndAction.Name = "ConditionAndAction";
            this.ConditionAndAction.Size = new System.Drawing.Size(269, 100);
            this.ConditionAndAction.TabIndex = 5;
            this.ConditionAndAction.TabStop = false;
            this.ConditionAndAction.Text = "设定条件动作数";
            // 
            // ModifyActionNum
            // 
            this.ModifyActionNum.Location = new System.Drawing.Point(169, 62);
            this.ModifyActionNum.Name = "ModifyActionNum";
            this.ModifyActionNum.Size = new System.Drawing.Size(61, 23);
            this.ModifyActionNum.TabIndex = 5;
            this.ModifyActionNum.Text = "设置";
            this.ModifyActionNum.UseVisualStyleBackColor = true;
            this.ModifyActionNum.Click += new System.EventHandler(this.ModifyActionNum_Click);
            // 
            // ModifyConditionNum
            // 
            this.ModifyConditionNum.Location = new System.Drawing.Point(169, 24);
            this.ModifyConditionNum.Name = "ModifyConditionNum";
            this.ModifyConditionNum.Size = new System.Drawing.Size(61, 23);
            this.ModifyConditionNum.TabIndex = 4;
            this.ModifyConditionNum.Text = "设置";
            this.ModifyConditionNum.UseVisualStyleBackColor = true;
            this.ModifyConditionNum.Click += new System.EventHandler(this.ModifyConditionNum_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(38, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "动作数";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(38, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "条件数";
            // 
            // ActionNum
            // 
            this.ActionNum.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ActionNum.Location = new System.Drawing.Point(93, 62);
            this.ActionNum.Name = "ActionNum";
            this.ActionNum.ReadOnly = true;
            this.ActionNum.Size = new System.Drawing.Size(53, 21);
            this.ActionNum.TabIndex = 3;
            // 
            // ConditionNum
            // 
            this.ConditionNum.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ConditionNum.Location = new System.Drawing.Point(93, 26);
            this.ConditionNum.Name = "ConditionNum";
            this.ConditionNum.ReadOnly = true;
            this.ConditionNum.Size = new System.Drawing.Size(53, 21);
            this.ConditionNum.TabIndex = 2;
            // 
            // ConditionMethod
            // 
            this.ConditionMethod.Controls.Add(this.EveryTimeType);
            this.ConditionMethod.Controls.Add(this.OnlyStartType);
            this.ConditionMethod.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ConditionMethod.Location = new System.Drawing.Point(32, 155);
            this.ConditionMethod.Name = "ConditionMethod";
            this.ConditionMethod.Size = new System.Drawing.Size(269, 106);
            this.ConditionMethod.TabIndex = 4;
            this.ConditionMethod.TabStop = false;
            this.ConditionMethod.Text = "条件执行方式";
            // 
            // EveryTimeType
            // 
            this.EveryTimeType.AutoSize = true;
            this.EveryTimeType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EveryTimeType.Location = new System.Drawing.Point(42, 70);
            this.EveryTimeType.Name = "EveryTimeType";
            this.EveryTimeType.Size = new System.Drawing.Size(179, 18);
            this.EveryTimeType.TabIndex = 1;
            this.EveryTimeType.TabStop = true;
            this.EveryTimeType.Text = "每次动作结束后重新计算";
            this.EveryTimeType.UseVisualStyleBackColor = true;
            this.EveryTimeType.CheckedChanged += new System.EventHandler(this.EveryTimeType_CheckedChanged);
            // 
            // OnlyStartType
            // 
            this.OnlyStartType.AutoSize = true;
            this.OnlyStartType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.OnlyStartType.Location = new System.Drawing.Point(42, 36);
            this.OnlyStartType.Name = "OnlyStartType";
            this.OnlyStartType.Size = new System.Drawing.Size(151, 18);
            this.OnlyStartType.TabIndex = 0;
            this.OnlyStartType.TabStop = true;
            this.OnlyStartType.Text = "只在开始时计算一次";
            this.OnlyStartType.UseVisualStyleBackColor = true;
            this.OnlyStartType.CheckedChanged += new System.EventHandler(this.OnlyStartType_CheckedChanged);
            // 
            // ConditionDefine
            // 
            this.ConditionDefine.BackColor = System.Drawing.SystemColors.Control;
            this.ConditionDefine.Controls.Add(this.label4);
            this.ConditionDefine.Controls.Add(this.ConditionExp);
            this.ConditionDefine.Controls.Add(this.label3);
            this.ConditionDefine.Controls.Add(this.AddCondition);
            this.ConditionDefine.Controls.Add(this.ConditionList);
            this.ConditionDefine.Location = new System.Drawing.Point(4, 21);
            this.ConditionDefine.Name = "ConditionDefine";
            this.ConditionDefine.Padding = new System.Windows.Forms.Padding(3);
            this.ConditionDefine.Size = new System.Drawing.Size(336, 281);
            this.ConditionDefine.TabIndex = 1;
            this.ConditionDefine.Text = "条件定义";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(28, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 14);
            this.label4.TabIndex = 4;
            this.label4.Text = "选择条件";
            // 
            // ConditionExp
            // 
            this.ConditionExp.Location = new System.Drawing.Point(171, 231);
            this.ConditionExp.Name = "ConditionExp";
            this.ConditionExp.ReadOnly = true;
            this.ConditionExp.Size = new System.Drawing.Size(132, 21);
            this.ConditionExp.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(168, 189);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "表达式";
            // 
            // AddCondition
            // 
            this.AddCondition.Enabled = false;
            this.AddCondition.Location = new System.Drawing.Point(228, 186);
            this.AddCondition.Name = "AddCondition";
            this.AddCondition.Size = new System.Drawing.Size(75, 23);
            this.AddCondition.TabIndex = 1;
            this.AddCondition.Text = "设置";
            this.AddCondition.UseVisualStyleBackColor = true;
            this.AddCondition.Click += new System.EventHandler(this.AddCondition_Click);
            // 
            // ConditionList
            // 
            this.ConditionList.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ConditionList.FormattingEnabled = true;
            this.ConditionList.ItemHeight = 14;
            this.ConditionList.Location = new System.Drawing.Point(31, 66);
            this.ConditionList.Name = "ConditionList";
            this.ConditionList.Size = new System.Drawing.Size(106, 186);
            this.ConditionList.TabIndex = 0;
            this.ConditionList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ConditionList_MouseClick);
            // 
            // ActionDefine
            // 
            this.ActionDefine.BackColor = System.Drawing.SystemColors.Control;
            this.ActionDefine.Controls.Add(this.label5);
            this.ActionDefine.Controls.Add(this.TacticBox);
            this.ActionDefine.Controls.Add(this.ChildTacticLabel);
            this.ActionDefine.Controls.Add(this.ExpLabel);
            this.ActionDefine.Controls.Add(this.EvaluateLabel);
            this.ActionDefine.Controls.Add(this.ExpBox);
            this.ActionDefine.Controls.Add(this.EvaluateExp);
            this.ActionDefine.Controls.Add(this.AddActionExp);
            this.ActionDefine.Controls.Add(this.AddActionResult);
            this.ActionDefine.Controls.Add(this.ActionType);
            this.ActionDefine.Controls.Add(this.ActionList);
            this.ActionDefine.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ActionDefine.Location = new System.Drawing.Point(4, 21);
            this.ActionDefine.Name = "ActionDefine";
            this.ActionDefine.Padding = new System.Windows.Forms.Padding(3);
            this.ActionDefine.Size = new System.Drawing.Size(336, 281);
            this.ActionDefine.TabIndex = 2;
            this.ActionDefine.Text = "动作定义";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 14);
            this.label5.TabIndex = 10;
            this.label5.Text = "选择动作";
            // 
            // TacticBox
            // 
            this.TacticBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TacticBox.Enabled = false;
            this.TacticBox.FormattingEnabled = true;
            this.TacticBox.Location = new System.Drawing.Point(168, 230);
            this.TacticBox.Name = "TacticBox";
            this.TacticBox.Size = new System.Drawing.Size(143, 22);
            this.TacticBox.TabIndex = 9;
            this.TacticBox.Visible = false;
            this.TacticBox.SelectedIndexChanged += new System.EventHandler(this.TacticBox_SelectedIndexChanged);
            // 
            // ChildTacticLabel
            // 
            this.ChildTacticLabel.AutoSize = true;
            this.ChildTacticLabel.Location = new System.Drawing.Point(168, 205);
            this.ChildTacticLabel.Name = "ChildTacticLabel";
            this.ChildTacticLabel.Size = new System.Drawing.Size(49, 14);
            this.ChildTacticLabel.TabIndex = 8;
            this.ChildTacticLabel.Text = "子策略";
            this.ChildTacticLabel.Visible = false;
            // 
            // ExpLabel
            // 
            this.ExpLabel.AutoSize = true;
            this.ExpLabel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExpLabel.Location = new System.Drawing.Point(168, 205);
            this.ExpLabel.Name = "ExpLabel";
            this.ExpLabel.Size = new System.Drawing.Size(49, 14);
            this.ExpLabel.TabIndex = 7;
            this.ExpLabel.Text = "表达式";
            // 
            // EvaluateLabel
            // 
            this.EvaluateLabel.AutoSize = true;
            this.EvaluateLabel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EvaluateLabel.Location = new System.Drawing.Point(168, 144);
            this.EvaluateLabel.Name = "EvaluateLabel";
            this.EvaluateLabel.Size = new System.Drawing.Size(63, 14);
            this.EvaluateLabel.TabIndex = 6;
            this.EvaluateLabel.Text = "赋值结果";
            // 
            // ExpBox
            // 
            this.ExpBox.Location = new System.Drawing.Point(168, 229);
            this.ExpBox.Name = "ExpBox";
            this.ExpBox.ReadOnly = true;
            this.ExpBox.Size = new System.Drawing.Size(143, 23);
            this.ExpBox.TabIndex = 5;
            // 
            // EvaluateExp
            // 
            this.EvaluateExp.Location = new System.Drawing.Point(168, 168);
            this.EvaluateExp.Name = "EvaluateExp";
            this.EvaluateExp.ReadOnly = true;
            this.EvaluateExp.Size = new System.Drawing.Size(143, 23);
            this.EvaluateExp.TabIndex = 4;
            // 
            // AddActionExp
            // 
            this.AddActionExp.Enabled = false;
            this.AddActionExp.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AddActionExp.Location = new System.Drawing.Point(236, 202);
            this.AddActionExp.Name = "AddActionExp";
            this.AddActionExp.Size = new System.Drawing.Size(75, 23);
            this.AddActionExp.TabIndex = 3;
            this.AddActionExp.Text = "设置";
            this.AddActionExp.UseVisualStyleBackColor = true;
            this.AddActionExp.Click += new System.EventHandler(this.AddActionExp_Click);
            // 
            // AddActionResult
            // 
            this.AddActionResult.Enabled = false;
            this.AddActionResult.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AddActionResult.Location = new System.Drawing.Point(236, 141);
            this.AddActionResult.Name = "AddActionResult";
            this.AddActionResult.Size = new System.Drawing.Size(75, 23);
            this.AddActionResult.TabIndex = 2;
            this.AddActionResult.Text = "设置";
            this.AddActionResult.UseVisualStyleBackColor = true;
            this.AddActionResult.Click += new System.EventHandler(this.AddActionResult_Click);
            // 
            // ActionType
            // 
            this.ActionType.Controls.Add(this.Transfer);
            this.ActionType.Controls.Add(this.Evaluate);
            this.ActionType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ActionType.Location = new System.Drawing.Point(168, 31);
            this.ActionType.Name = "ActionType";
            this.ActionType.Size = new System.Drawing.Size(143, 69);
            this.ActionType.TabIndex = 1;
            this.ActionType.TabStop = false;
            this.ActionType.Text = "动作类型";
            // 
            // Transfer
            // 
            this.Transfer.AutoSize = true;
            this.Transfer.Enabled = false;
            this.Transfer.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Transfer.Location = new System.Drawing.Point(15, 43);
            this.Transfer.Name = "Transfer";
            this.Transfer.Size = new System.Drawing.Size(95, 18);
            this.Transfer.TabIndex = 1;
            this.Transfer.TabStop = true;
            this.Transfer.Text = "调用子策略";
            this.Transfer.UseVisualStyleBackColor = true;
            this.Transfer.CheckedChanged += new System.EventHandler(this.Transfer_CheckedChanged);
            // 
            // Evaluate
            // 
            this.Evaluate.AutoSize = true;
            this.Evaluate.Enabled = false;
            this.Evaluate.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Evaluate.Location = new System.Drawing.Point(15, 20);
            this.Evaluate.Name = "Evaluate";
            this.Evaluate.Size = new System.Drawing.Size(81, 18);
            this.Evaluate.TabIndex = 0;
            this.Evaluate.TabStop = true;
            this.Evaluate.Text = "赋值语句";
            this.Evaluate.UseVisualStyleBackColor = true;
            this.Evaluate.CheckedChanged += new System.EventHandler(this.Evaluate_CheckedChanged);
            // 
            // ActionList
            // 
            this.ActionList.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ActionList.FormattingEnabled = true;
            this.ActionList.ItemHeight = 14;
            this.ActionList.Location = new System.Drawing.Point(31, 66);
            this.ActionList.Name = "ActionList";
            this.ActionList.Size = new System.Drawing.Size(106, 186);
            this.ActionList.TabIndex = 0;
            this.ActionList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ActionList_MouseClick);
            // 
            // OrderDefine
            // 
            this.OrderDefine.BackColor = System.Drawing.SystemColors.Control;
            this.OrderDefine.Controls.Add(this.OrderDefineBox);
            this.OrderDefine.Location = new System.Drawing.Point(4, 21);
            this.OrderDefine.Name = "OrderDefine";
            this.OrderDefine.Padding = new System.Windows.Forms.Padding(3);
            this.OrderDefine.Size = new System.Drawing.Size(336, 281);
            this.OrderDefine.TabIndex = 3;
            this.OrderDefine.Text = "顺序表定义";
            // 
            // OrderDefineBox
            // 
            this.OrderDefineBox.AllowUserToAddRows = false;
            this.OrderDefineBox.AllowUserToDeleteRows = false;
            this.OrderDefineBox.AllowUserToResizeColumns = false;
            this.OrderDefineBox.AllowUserToResizeRows = false;
            this.OrderDefineBox.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.OrderDefineBox.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.OrderDefineBox.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.OrderDefineBox.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.actions});
            this.OrderDefineBox.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.OrderDefineBox.Location = new System.Drawing.Point(6, 17);
            this.OrderDefineBox.Name = "OrderDefineBox";
            this.OrderDefineBox.RowHeadersVisible = false;
            this.OrderDefineBox.RowTemplate.Height = 23;
            this.OrderDefineBox.Size = new System.Drawing.Size(324, 258);
            this.OrderDefineBox.TabIndex = 0;
            // 
            // actions
            // 
            this.actions.HeaderText = "动作";
            this.actions.Name = "actions";
            this.actions.ReadOnly = true;
            this.actions.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.actions.Width = 54;
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(148, 314);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 1;
            this.OK.Text = "确认";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cencel
            // 
            this.Cencel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cencel.Location = new System.Drawing.Point(240, 314);
            this.Cencel.Name = "Cencel";
            this.Cencel.Size = new System.Drawing.Size(75, 23);
            this.Cencel.TabIndex = 2;
            this.Cencel.Text = "取消";
            this.Cencel.UseVisualStyleBackColor = true;
            // 
            // ProcessForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 349);
            this.Controls.Add(this.Cencel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.ProcessTab);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcessForm1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "条件动作表组态";
            this.ProcessTab.ResumeLayout(false);
            this.PublicAttribute.ResumeLayout(false);
            this.ConditionAndAction.ResumeLayout(false);
            this.ConditionAndAction.PerformLayout();
            this.ConditionMethod.ResumeLayout(false);
            this.ConditionMethod.PerformLayout();
            this.ConditionDefine.ResumeLayout(false);
            this.ConditionDefine.PerformLayout();
            this.ActionDefine.ResumeLayout(false);
            this.ActionDefine.PerformLayout();
            this.ActionType.ResumeLayout(false);
            this.ActionType.PerformLayout();
            this.OrderDefine.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OrderDefineBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl ProcessTab;
        private System.Windows.Forms.TabPage PublicAttribute;
        private System.Windows.Forms.GroupBox ConditionMethod;
        private System.Windows.Forms.TextBox ActionNum;
        private System.Windows.Forms.TextBox ConditionNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage ConditionDefine;
        private System.Windows.Forms.TabPage ActionDefine;
        private System.Windows.Forms.TabPage OrderDefine;
        private System.Windows.Forms.RadioButton EveryTimeType;
        private System.Windows.Forms.RadioButton OnlyStartType;
        private System.Windows.Forms.TextBox ConditionExp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button AddCondition;
        private System.Windows.Forms.ListBox ConditionList;
        private System.Windows.Forms.ComboBox TacticBox;
        private System.Windows.Forms.Label ChildTacticLabel;
        private System.Windows.Forms.Label ExpLabel;
        private System.Windows.Forms.Label EvaluateLabel;
        private System.Windows.Forms.TextBox ExpBox;
        private System.Windows.Forms.TextBox EvaluateExp;
        private System.Windows.Forms.Button AddActionExp;
        private System.Windows.Forms.Button AddActionResult;
        private System.Windows.Forms.GroupBox ActionType;
        private System.Windows.Forms.RadioButton Transfer;
        private System.Windows.Forms.RadioButton Evaluate;
        private System.Windows.Forms.ListBox ActionList;
        private System.Windows.Forms.DataGridView OrderDefineBox;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cencel;
        private System.Windows.Forms.DataGridViewTextBoxColumn actions;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox ConditionAndAction;
        private System.Windows.Forms.Button ModifyActionNum;
        private System.Windows.Forms.Button ModifyConditionNum;
    }
}