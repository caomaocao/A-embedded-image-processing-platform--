namespace CaVeGen.DesignViewFiles.CodeEditor
{
    partial class ErrorViewControl2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorViewControl2));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.lv_wrongList = new System.Windows.Forms.ListView();
            this.column_ImageKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.column_Index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.column_Context = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.column_LineNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.column_PageName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.il_wrongList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ts_wrong = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ts_warning = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ts_info = new System.Windows.Forms.ToolStripButton();
            this.MainPanel.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.lv_wrongList);
            this.MainPanel.Controls.Add(this.toolStrip1);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(588, 305);
            this.MainPanel.TabIndex = 0;
            // 
            // lv_wrongList
            // 
            this.lv_wrongList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.column_ImageKey,
            this.column_Index,
            this.column_Context,
            this.column_LineNumber,
            this.column_PageName});
            this.lv_wrongList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_wrongList.FullRowSelect = true;
            this.lv_wrongList.GridLines = true;
            this.lv_wrongList.LargeImageList = this.il_wrongList;
            this.lv_wrongList.Location = new System.Drawing.Point(0, 25);
            this.lv_wrongList.MultiSelect = false;
            this.lv_wrongList.Name = "lv_wrongList";
            this.lv_wrongList.ShowItemToolTips = true;
            this.lv_wrongList.Size = new System.Drawing.Size(588, 280);
            this.lv_wrongList.SmallImageList = this.il_wrongList;
            this.lv_wrongList.TabIndex = 7;
            this.lv_wrongList.UseCompatibleStateImageBehavior = false;
            this.lv_wrongList.View = System.Windows.Forms.View.Details;
            this.lv_wrongList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lv_wrongList_MouseDoubleClick);
            // 
            // column_ImageKey
            // 
            this.column_ImageKey.Text = "";
            this.column_ImageKey.Width = 28;
            // 
            // column_Index
            // 
            this.column_Index.Text = "索引";
            // 
            // column_Context
            // 
            this.column_Context.Text = "说明";
            this.column_Context.Width = 284;
            // 
            // column_LineNumber
            // 
            this.column_LineNumber.Text = "行";
            this.column_LineNumber.Width = 66;
            // 
            // column_PageName
            // 
            this.column_PageName.Text = "页面名";
            this.column_PageName.Width = 100;
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
            this.ts_wrong.Size = new System.Drawing.Size(71, 22);
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
            this.ts_warning.Image = global::CaVeGen.Properties.Resources.warning;
            this.ts_warning.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ts_warning.Name = "ts_warning";
            this.ts_warning.Size = new System.Drawing.Size(71, 22);
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
            this.ts_info.Size = new System.Drawing.Size(71, 22);
            this.ts_info.Text = "0个消息";
            this.ts_info.CheckedChanged += new System.EventHandler(this.ts_info_CheckedChanged);
            // 
            // ErrorViewControl2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MainPanel);
            this.Name = "ErrorViewControl2";
            this.Size = new System.Drawing.Size(588, 305);
            this.MainPanel.ResumeLayout(false);
            this.MainPanel.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.ColumnHeader column_Index;
        private System.Windows.Forms.ColumnHeader column_Context;
        private System.Windows.Forms.ColumnHeader column_LineNumber;
        private System.Windows.Forms.ColumnHeader column_PageName;
        private System.Windows.Forms.ImageList il_wrongList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ColumnHeader column_ImageKey;
        public System.Windows.Forms.ListView lv_wrongList;
        public System.Windows.Forms.ToolStripButton ts_wrong;
        public System.Windows.Forms.ToolStripButton ts_warning;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton ts_info;
    }
}
