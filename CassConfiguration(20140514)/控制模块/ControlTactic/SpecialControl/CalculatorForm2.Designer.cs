namespace ControlTactic.SpecialControl
{
    partial class CalculatorForm2
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
            this.ElementType = new System.Windows.Forms.ComboBox();
            this.ExpressBox = new System.Windows.Forms.TextBox();
            this.Expression = new System.Windows.Forms.Label();
            this.ExpressionType = new System.Windows.Forms.Label();
            this.Element = new System.Windows.Forms.ListBox();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.second = new System.Windows.Forms.RadioButton();
            this.third = new System.Windows.Forms.RadioButton();
            this.forth = new System.Windows.Forms.RadioButton();
            this.first = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // ElementType
            // 
            this.ElementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ElementType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ElementType.FormattingEnabled = true;
            this.ElementType.Items.AddRange(new object[] {
            "变量",
            "运算",
            "函数"});
            this.ElementType.Location = new System.Drawing.Point(23, 104);
            this.ElementType.Name = "ElementType";
            this.ElementType.Size = new System.Drawing.Size(88, 22);
            this.ElementType.TabIndex = 0;
            this.ElementType.SelectedIndexChanged += new System.EventHandler(this.ElementType_SelectedIndexChanged);
            // 
            // ExpressBox
            // 
            this.ExpressBox.Location = new System.Drawing.Point(88, 26);
            this.ExpressBox.Name = "ExpressBox";
            this.ExpressBox.Size = new System.Drawing.Size(198, 21);
            this.ExpressBox.TabIndex = 4;
            // 
            // Expression
            // 
            this.Expression.AutoSize = true;
            this.Expression.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Expression.Location = new System.Drawing.Point(20, 27);
            this.Expression.Name = "Expression";
            this.Expression.Size = new System.Drawing.Size(49, 14);
            this.Expression.TabIndex = 5;
            this.Expression.Text = "表达式";
            // 
            // ExpressionType
            // 
            this.ExpressionType.AutoSize = true;
            this.ExpressionType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ExpressionType.Location = new System.Drawing.Point(20, 68);
            this.ExpressionType.Name = "ExpressionType";
            this.ExpressionType.Size = new System.Drawing.Size(105, 14);
            this.ExpressionType.TabIndex = 6;
            this.ExpressionType.Text = "表达式元素类型";
            // 
            // Element
            // 
            this.Element.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Element.FormattingEnabled = true;
            this.Element.ItemHeight = 14;
            this.Element.Location = new System.Drawing.Point(178, 68);
            this.Element.Name = "Element";
            this.Element.Size = new System.Drawing.Size(108, 144);
            this.Element.TabIndex = 7;
            this.Element.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Element_MouseDoubleClick);
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(57, 252);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 8;
            this.OK.Text = "确定";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(178, 252);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 9;
            this.Cancel.Text = "取消";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // second
            // 
            this.second.AutoSize = true;
            this.second.Location = new System.Drawing.Point(95, 155);
            this.second.Name = "second";
            this.second.Size = new System.Drawing.Size(59, 16);
            this.second.TabIndex = 10;
            this.second.TabStop = true;
            this.second.Text = "second";
            this.second.UseVisualStyleBackColor = true;
            this.second.CheckedChanged += new System.EventHandler(this.second_CheckedChanged);
            // 
            // third
            // 
            this.third.AutoSize = true;
            this.third.Location = new System.Drawing.Point(23, 196);
            this.third.Name = "third";
            this.third.Size = new System.Drawing.Size(53, 16);
            this.third.TabIndex = 11;
            this.third.TabStop = true;
            this.third.Text = "third";
            this.third.UseVisualStyleBackColor = true;
            this.third.CheckedChanged += new System.EventHandler(this.third_CheckedChanged);
            // 
            // forth
            // 
            this.forth.AutoSize = true;
            this.forth.Location = new System.Drawing.Point(95, 196);
            this.forth.Name = "forth";
            this.forth.Size = new System.Drawing.Size(53, 16);
            this.forth.TabIndex = 12;
            this.forth.TabStop = true;
            this.forth.Text = "forth";
            this.forth.UseVisualStyleBackColor = true;
            this.forth.Visible = false;
            this.forth.CheckedChanged += new System.EventHandler(this.forth_CheckedChanged);
            // 
            // first
            // 
            this.first.AutoSize = true;
            this.first.Location = new System.Drawing.Point(23, 155);
            this.first.Name = "first";
            this.first.Size = new System.Drawing.Size(53, 16);
            this.first.TabIndex = 13;
            this.first.TabStop = true;
            this.first.Text = "first";
            this.first.UseVisualStyleBackColor = true;
            this.first.CheckedChanged += new System.EventHandler(this.first_CheckedChanged);
            // 
            // CalculatorForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 293);
            this.Controls.Add(this.first);
            this.Controls.Add(this.forth);
            this.Controls.Add(this.third);
            this.Controls.Add(this.second);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Element);
            this.Controls.Add(this.ExpressionType);
            this.Controls.Add(this.Expression);
            this.Controls.Add(this.ExpressBox);
            this.Controls.Add(this.ElementType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalculatorForm2";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "表达式输入";
            this.Load += new System.EventHandler(this.CalculatorForm2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ElementType;
        private System.Windows.Forms.TextBox ExpressBox;
        private System.Windows.Forms.Label Expression;
        private System.Windows.Forms.Label ExpressionType;
        private System.Windows.Forms.ListBox Element;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.RadioButton second;
        private System.Windows.Forms.RadioButton third;
        private System.Windows.Forms.RadioButton forth;
        private System.Windows.Forms.RadioButton first;
    }
}