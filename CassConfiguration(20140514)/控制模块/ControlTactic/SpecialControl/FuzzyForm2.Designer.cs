namespace ControlTactic.SpecialControl
{
    partial class FuzzyForm2
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
            this.ControlTableBox = new System.Windows.Forms.DataGridView();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.LoadDate = new System.Windows.Forms.Button();
            this.SaveDate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ControlTableBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ControlTableBox
            // 
            this.ControlTableBox.AllowUserToAddRows = false;
            this.ControlTableBox.AllowUserToDeleteRows = false;
            this.ControlTableBox.AllowUserToResizeColumns = false;
            this.ControlTableBox.AllowUserToResizeRows = false;
            this.ControlTableBox.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ControlTableBox.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ControlTableBox.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ControlTableBox.Location = new System.Drawing.Point(13, 13);
            this.ControlTableBox.Name = "ControlTableBox";
            this.ControlTableBox.RowHeadersVisible = false;
            this.ControlTableBox.RowTemplate.Height = 23;
            this.ControlTableBox.Size = new System.Drawing.Size(265, 235);
            this.ControlTableBox.TabIndex = 0;
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(290, 102);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(50, 25);
            this.OK.TabIndex = 1;
            this.OK.Text = "确认";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(290, 147);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(50, 25);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "关闭";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // LoadDate
            // 
            this.LoadDate.Location = new System.Drawing.Point(290, 12);
            this.LoadDate.Name = "LoadDate";
            this.LoadDate.Size = new System.Drawing.Size(50, 25);
            this.LoadDate.TabIndex = 3;
            this.LoadDate.Text = "载入";
            this.LoadDate.UseVisualStyleBackColor = true;
            this.LoadDate.Click += new System.EventHandler(this.LoadDate_Click);
            // 
            // SaveDate
            // 
            this.SaveDate.Location = new System.Drawing.Point(290, 57);
            this.SaveDate.Name = "SaveDate";
            this.SaveDate.Size = new System.Drawing.Size(50, 25);
            this.SaveDate.TabIndex = 4;
            this.SaveDate.Text = "保存";
            this.SaveDate.UseVisualStyleBackColor = true;
            this.SaveDate.Click += new System.EventHandler(this.SaveDate_Click);
            // 
            // FuzzyForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 266);
            this.Controls.Add(this.SaveDate);
            this.Controls.Add(this.LoadDate);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.ControlTableBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 300);
            this.Name = "FuzzyForm2";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "模糊控制表";
            this.SizeChanged += new System.EventHandler(this.FuzzyForm2_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.ControlTableBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView ControlTableBox;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button LoadDate;
        private System.Windows.Forms.Button SaveDate;
    }
}