namespace ControlTactic.SpecialControl
{
    partial class CalculatorFrom1
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
            this.Condition = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SETExpression1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ResultBox = new System.Windows.Forms.ComboBox();
            this.SETExpression2 = new System.Windows.Forms.Button();
            this.Result = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ChooseConditon = new System.Windows.Forms.ListBox();
            this.OK = new System.Windows.Forms.Button();
            this.Cencel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Condition
            // 
            this.Condition.Location = new System.Drawing.Point(187, 66);
            this.Condition.Name = "Condition";
            this.Condition.ReadOnly = true;
            this.Condition.Size = new System.Drawing.Size(161, 21);
            this.Condition.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(184, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "计算条件";
            // 
            // SETExpression1
            // 
            this.SETExpression1.Enabled = false;
            this.SETExpression1.Location = new System.Drawing.Point(273, 22);
            this.SETExpression1.Name = "SETExpression1";
            this.SETExpression1.Size = new System.Drawing.Size(75, 23);
            this.SETExpression1.TabIndex = 3;
            this.SETExpression1.Text = "设置";
            this.SETExpression1.UseVisualStyleBackColor = true;
            this.SETExpression1.Click += new System.EventHandler(this.SETExpression1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(184, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "计算结果";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(184, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 14);
            this.label3.TabIndex = 5;
            this.label3.Text = "结果表达式";
            // 
            // ResultBox
            // 
            this.ResultBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ResultBox.Enabled = false;
            this.ResultBox.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ResultBox.FormattingEnabled = true;
            this.ResultBox.Items.AddRange(new object[] {
            "Y0",
            "Y1",
            "Y2",
            "Y3",
            "M0",
            "M1",
            "M2",
            "M3",
            "M4",
            "M5",
            "M6",
            "M7",
            "M8",
            "M9"});
            this.ResultBox.Location = new System.Drawing.Point(258, 118);
            this.ResultBox.Name = "ResultBox";
            this.ResultBox.Size = new System.Drawing.Size(90, 22);
            this.ResultBox.TabIndex = 6;
            this.ResultBox.SelectedIndexChanged += new System.EventHandler(this.ResultBox_SelectedIndexChanged);
            // 
            // SETExpression2
            // 
            this.SETExpression2.Enabled = false;
            this.SETExpression2.Location = new System.Drawing.Point(273, 161);
            this.SETExpression2.Name = "SETExpression2";
            this.SETExpression2.Size = new System.Drawing.Size(75, 23);
            this.SETExpression2.TabIndex = 7;
            this.SETExpression2.Text = "设置";
            this.SETExpression2.UseVisualStyleBackColor = true;
            this.SETExpression2.Click += new System.EventHandler(this.SETExpression2_Click);
            // 
            // Result
            // 
            this.Result.Location = new System.Drawing.Point(187, 202);
            this.Result.Name = "Result";
            this.Result.ReadOnly = true;
            this.Result.Size = new System.Drawing.Size(161, 21);
            this.Result.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(27, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 14);
            this.label4.TabIndex = 9;
            this.label4.Text = "选择条件";
            // 
            // ChooseConditon
            // 
            this.ChooseConditon.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ChooseConditon.FormattingEnabled = true;
            this.ChooseConditon.ItemHeight = 14;
            this.ChooseConditon.Items.AddRange(new object[] {
            "条件1",
            "条件2",
            "条件3",
            "条件4",
            "条件5",
            "条件6",
            "条件7",
            "条件8",
            "条件9",
            "条件10"});
            this.ChooseConditon.Location = new System.Drawing.Point(30, 51);
            this.ChooseConditon.Name = "ChooseConditon";
            this.ChooseConditon.Size = new System.Drawing.Size(109, 172);
            this.ChooseConditon.TabIndex = 10;
            this.ChooseConditon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChooseConditon_MouseClick);
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(172, 252);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 11;
            this.OK.Text = "确认";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cencel
            // 
            this.Cencel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cencel.Location = new System.Drawing.Point(273, 252);
            this.Cencel.Name = "Cencel";
            this.Cencel.Size = new System.Drawing.Size(75, 23);
            this.Cencel.TabIndex = 12;
            this.Cencel.Text = "取消";
            this.Cencel.UseVisualStyleBackColor = true;
            // 
            // CalculatorFrom1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 288);
            this.Controls.Add(this.Cencel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.ChooseConditon);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Result);
            this.Controls.Add(this.SETExpression2);
            this.Controls.Add(this.ResultBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SETExpression1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Condition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalculatorFrom1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "计算器组态";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Condition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SETExpression1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ResultBox;
        private System.Windows.Forms.Button SETExpression2;
        private System.Windows.Forms.TextBox Result;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox ChooseConditon;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cencel;
    }
}