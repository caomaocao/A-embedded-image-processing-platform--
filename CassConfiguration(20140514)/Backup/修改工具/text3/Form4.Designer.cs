namespace text3
{
    partial class Form4
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
            this.Codevalue = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.CodeList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AddCode = new System.Windows.Forms.Button();
            this.VarName = new System.Windows.Forms.TextBox();
            this.DelCode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Codevalue
            // 
            this.Codevalue.Location = new System.Drawing.Point(39, 58);
            this.Codevalue.Name = "Codevalue";
            this.Codevalue.Size = new System.Drawing.Size(344, 241);
            this.Codevalue.TabIndex = 0;
            this.Codevalue.Text = "";
            this.Codevalue.TextChanged += new System.EventHandler(this.Codevalue_TextChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(97, 314);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "确认";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(245, 314);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // CodeList
            // 
            this.CodeList.FormattingEnabled = true;
            this.CodeList.Location = new System.Drawing.Point(39, 32);
            this.CodeList.Name = "CodeList";
            this.CodeList.Size = new System.Drawing.Size(101, 20);
            this.CodeList.TabIndex = 3;
            this.CodeList.SelectedIndexChanged += new System.EventHandler(this.CodeList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "特殊代码名";
            // 
            // AddCode
            // 
            this.AddCode.Location = new System.Drawing.Point(270, 32);
            this.AddCode.Name = "AddCode";
            this.AddCode.Size = new System.Drawing.Size(50, 20);
            this.AddCode.TabIndex = 5;
            this.AddCode.Text = "添加";
            this.AddCode.UseVisualStyleBackColor = true;
            this.AddCode.Click += new System.EventHandler(this.AddCode_Click);
            // 
            // VarName
            // 
            this.VarName.Location = new System.Drawing.Point(158, 31);
            this.VarName.Name = "VarName";
            this.VarName.Size = new System.Drawing.Size(93, 21);
            this.VarName.TabIndex = 6;
            // 
            // DelCode
            // 
            this.DelCode.Location = new System.Drawing.Point(333, 32);
            this.DelCode.Name = "DelCode";
            this.DelCode.Size = new System.Drawing.Size(50, 20);
            this.DelCode.TabIndex = 7;
            this.DelCode.Text = "删除";
            this.DelCode.UseVisualStyleBackColor = true;
            this.DelCode.Click += new System.EventHandler(this.DelCode_Click);
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 363);
            this.Controls.Add(this.DelCode);
            this.Controls.Add(this.VarName);
            this.Controls.Add(this.AddCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CodeList);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Codevalue);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form4";
            this.Text = "修改特殊代码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox Codevalue;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox CodeList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AddCode;
        private System.Windows.Forms.TextBox VarName;
        private System.Windows.Forms.Button DelCode;
    }
}