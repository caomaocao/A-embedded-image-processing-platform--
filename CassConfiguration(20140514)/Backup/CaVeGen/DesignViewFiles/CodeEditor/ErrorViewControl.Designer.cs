namespace CaVeGen.DesignViewFiles.CodeEditor
{
    partial class ErrorViewControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorViewControl));
            this.lv_wrongList = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.LineNumber = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.il_wrongList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ts_wrong = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ts_warning = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ts_info = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lv_wrongList
            // 
            this.lv_wrongList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lv_wrongList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader1,
            this.columnHeader2,
            this.LineNumber,
            this.columnHeader4});
            this.lv_wrongList.FullRowSelect = true;
            this.lv_wrongList.GridLines = true;
            this.lv_wrongList.LargeImageList = this.il_wrongList;
            this.lv_wrongList.Location = new System.Drawing.Point(0, 28);
            this.lv_wrongList.MultiSelect = false;
            this.lv_wrongList.Name = "lv_wrongList";
            this.lv_wrongList.ShowItemToolTips = true;
            this.lv_wrongList.Size = new System.Drawing.Size(588, 274);
            this.lv_wrongList.SmallImageList = this.il_wrongList;
            this.lv_wrongList.TabIndex = 7;
            this.lv_wrongList.UseCompatibleStateImageBehavior = false;
            this.lv_wrongList.View = System.Windows.Forms.View.Details;
            this.lv_wrongList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lv_wrongList_MouseDoubleClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "";
            this.columnHeader5.Width = 28;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "索引";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "说明";
            this.columnHeader2.Width = 400;
            // 
            // LineNumber
            // 
            this.LineNumber.Text = "行";
            this.LineNumber.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "页面名";
            this.columnHeader4.Width = 100;
            // 
            // il_wrongList
            // 
            this.il_wrongList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il_wrongList.ImageStream")));
            this.il_wrongList.TransparentColor = System.Drawing.Color.Transparent;
            this.il_wrongList.Images.SetKeyName(0, "warning.png");
            this.il_wrongList.Images.SetKeyName(1, "error.png");
            this.il_wrongList.Images.SetKeyName(2, "info.png");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ts_wrong,
            this.toolStripSeparator1,
            this.ts_warning,
            this.toolStripSeparator2,
            this.ts_info});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(588, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ts_wrong
            // 
            this.ts_wrong.Checked = true;
            this.ts_wrong.CheckOnClick = true;
            this.ts_wrong.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ts_wrong.Image = global::CaVeGen.Properties.Resources.error;
            this.ts_wrong.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ts_wrong.Name = "ts_wrong";
            this.ts_wrong.Size = new System.Drawing.Size(67, 22);
            this.ts_wrong.Text = "0个错误";
            this.ts_wrong.CheckedChanged += new System.EventHandler(this.ts_wrong_CheckedChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ts_warning
            // 
            this.ts_warning.Checked = true;
            this.ts_warning.CheckOnClick = true;
            this.ts_warning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ts_warning.Image = ((System.Drawing.Image)(resources.GetObject("ts_warning.Image")));
            this.ts_warning.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ts_warning.Name = "ts_warning";
            this.ts_warning.Size = new System.Drawing.Size(67, 22);
            this.ts_warning.Text = "0个警告";
            this.ts_warning.CheckedChanged += new System.EventHandler(this.ts_warning_CheckedChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // ts_info
            // 
            this.ts_info.Checked = true;
            this.ts_info.CheckOnClick = true;
            this.ts_info.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ts_info.Image = global::CaVeGen.Properties.Resources.info;
            this.ts_info.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ts_info.Name = "ts_info";
            this.ts_info.Size = new System.Drawing.Size(67, 22);
            this.ts_info.Text = "0个消息";
            this.ts_info.CheckedChanged += new System.EventHandler(this.ts_info_CheckedChanged);
            // 
            // ErrorViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lv_wrongList);
            this.Name = "ErrorViewControl";
            this.Size = new System.Drawing.Size(588, 305);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader LineNumber;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ImageList il_wrongList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        public System.Windows.Forms.ListView lv_wrongList;
        public System.Windows.Forms.ToolStripButton ts_wrong;
        public System.Windows.Forms.ToolStripButton ts_warning;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton ts_info;
    }
}
