namespace CaVeGen.DesignViewFiles
{
    partial class AddressTable
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.AddressTableView = new System.Windows.Forms.DataGridView();
            this.ControlName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AttributeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Able = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SLmenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Load = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.FiletoolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItem_Close = new System.Windows.Forms.ToolStripMenuItem();
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_Revert = new System.Windows.Forms.ToolStripMenuItem();
            this.EdittoolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.MenuItem_ViewSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_ViewUnselect = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_ViewAll = new System.Windows.Forms.ToolStripMenuItem();
            this.AddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_ClearAds = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItem_AutoOrder = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.AddressTableView)).BeginInit();
            this.SLmenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddressTableView
            // 
            this.AddressTableView.AllowUserToAddRows = false;
            this.AddressTableView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.AddressTableView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.AddressTableView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AddressTableView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ControlName,
            this.AttributeName,
            this.Type,
            this.TypeLength,
            this.Address,
            this.Able});
            this.AddressTableView.Location = new System.Drawing.Point(24, 39);
            this.AddressTableView.Name = "AddressTableView";
            this.AddressTableView.RowHeadersWidth = 20;
            this.AddressTableView.RowTemplate.Height = 23;
            this.AddressTableView.Size = new System.Drawing.Size(532, 324);
            this.AddressTableView.TabIndex = 0;
            this.AddressTableView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.AddressTableView_CellEndEdit);
            // 
            // ControlName
            // 
            this.ControlName.FillWeight = 104.7059F;
            this.ControlName.HeaderText = "模块名";
            this.ControlName.Name = "ControlName";
            this.ControlName.ReadOnly = true;
            // 
            // AttributeName
            // 
            this.AttributeName.FillWeight = 117.4924F;
            this.AttributeName.HeaderText = "参数名";
            this.AttributeName.Name = "AttributeName";
            this.AttributeName.ReadOnly = true;
            this.AttributeName.Width = 130;
            // 
            // Type
            // 
            this.Type.FillWeight = 108.3719F;
            this.Type.HeaderText = "类型";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Width = 95;
            // 
            // TypeLength
            // 
            this.TypeLength.FillWeight = 103.306F;
            this.TypeLength.HeaderText = "长度(字节)";
            this.TypeLength.Name = "TypeLength";
            this.TypeLength.ReadOnly = true;
            this.TypeLength.Width = 90;
            // 
            // Address
            // 
            this.Address.FillWeight = 97.89273F;
            this.Address.HeaderText = "地址";
            this.Address.Name = "Address";
            this.Address.Width = 85;
            // 
            // Able
            // 
            this.Able.FillWeight = 68.23108F;
            this.Able.HeaderText = "可读可写";
            this.Able.Name = "Able";
            this.Able.Visible = false;
            this.Able.Width = 60;
            // 
            // SLmenuStrip
            // 
            this.SLmenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.EditToolStripMenuItem,
            this.AddressToolStripMenuItem});
            this.SLmenuStrip.Location = new System.Drawing.Point(0, 0);
            this.SLmenuStrip.Name = "SLmenuStrip";
            this.SLmenuStrip.Size = new System.Drawing.Size(582, 24);
            this.SLmenuStrip.TabIndex = 3;
            this.SLmenuStrip.Text = "SLmenuStrip";
            this.SLmenuStrip.Click += new System.EventHandler(this.SLmenuStrip_Click);
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Load,
            this.MenuItem_Save,
            this.MenuItem_SaveAs,
            this.FiletoolStripSeparator,
            this.MenuItem_Close});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.FileToolStripMenuItem.Text = "文件(F)";
            // 
            // MenuItem_Load
            // 
            this.MenuItem_Load.Name = "MenuItem_Load";
            this.MenuItem_Load.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.MenuItem_Load.Size = new System.Drawing.Size(152, 22);
            this.MenuItem_Load.Text = "导入";
            this.MenuItem_Load.Click += new System.EventHandler(this.MenuItem_Load_Click);
            // 
            // MenuItem_Save
            // 
            this.MenuItem_Save.Name = "MenuItem_Save";
            this.MenuItem_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.MenuItem_Save.Size = new System.Drawing.Size(152, 22);
            this.MenuItem_Save.Text = "保存";
            this.MenuItem_Save.Click += new System.EventHandler(this.MenuItem_Save_Click);
            // 
            // MenuItem_SaveAs
            // 
            this.MenuItem_SaveAs.Name = "MenuItem_SaveAs";
            this.MenuItem_SaveAs.Size = new System.Drawing.Size(152, 22);
            this.MenuItem_SaveAs.Text = "另存为";
            this.MenuItem_SaveAs.Click += new System.EventHandler(this.MenuItem_SaveAs_Click);
            // 
            // FiletoolStripSeparator
            // 
            this.FiletoolStripSeparator.Name = "FiletoolStripSeparator";
            this.FiletoolStripSeparator.Size = new System.Drawing.Size(149, 6);
            // 
            // MenuItem_Close
            // 
            this.MenuItem_Close.Name = "MenuItem_Close";
            this.MenuItem_Close.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.MenuItem_Close.Size = new System.Drawing.Size(152, 22);
            this.MenuItem_Close.Text = "关闭";
            this.MenuItem_Close.Click += new System.EventHandler(this.MenuItem_Close_Click);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_Revert,
            this.EdittoolStripSeparator,
            this.MenuItem_ViewSelect,
            this.MenuItem_ViewUnselect,
            this.MenuItem_ViewAll});
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.EditToolStripMenuItem.Text = "编辑(E)";
            // 
            // MenuItem_Revert
            // 
            this.MenuItem_Revert.Name = "MenuItem_Revert";
            this.MenuItem_Revert.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.MenuItem_Revert.Size = new System.Drawing.Size(196, 22);
            this.MenuItem_Revert.Text = "还原";
            this.MenuItem_Revert.Click += new System.EventHandler(this.MenuItem_Revert_Click);
            // 
            // EdittoolStripSeparator
            // 
            this.EdittoolStripSeparator.Name = "EdittoolStripSeparator";
            this.EdittoolStripSeparator.Size = new System.Drawing.Size(193, 6);
            // 
            // MenuItem_ViewSelect
            // 
            this.MenuItem_ViewSelect.Name = "MenuItem_ViewSelect";
            this.MenuItem_ViewSelect.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
            this.MenuItem_ViewSelect.Size = new System.Drawing.Size(196, 22);
            this.MenuItem_ViewSelect.Text = "显示不显示行";
            this.MenuItem_ViewSelect.Click += new System.EventHandler(this.MenuItem_ViewSelect_Click);
            // 
            // MenuItem_ViewUnselect
            // 
            this.MenuItem_ViewUnselect.Name = "MenuItem_ViewUnselect";
            this.MenuItem_ViewUnselect.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.MenuItem_ViewUnselect.Size = new System.Drawing.Size(196, 22);
            this.MenuItem_ViewUnselect.Text = "显示可读可写行";
            this.MenuItem_ViewUnselect.Click += new System.EventHandler(this.MenuItem_ViewUnselect_Click);
            // 
            // MenuItem_ViewAll
            // 
            this.MenuItem_ViewAll.Name = "MenuItem_ViewAll";
            this.MenuItem_ViewAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D9)));
            this.MenuItem_ViewAll.Size = new System.Drawing.Size(196, 22);
            this.MenuItem_ViewAll.Text = "显示所有行";
            this.MenuItem_ViewAll.Click += new System.EventHandler(this.MenuItem_ViewAll_Click);
            // 
            // AddressToolStripMenuItem
            // 
            this.AddressToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItem_ClearAds,
            this.MenuItem_AutoOrder});
            this.AddressToolStripMenuItem.Name = "AddressToolStripMenuItem";
            this.AddressToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.AddressToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.AddressToolStripMenuItem.Text = "地址(A)";
            // 
            // MenuItem_ClearAds
            // 
            this.MenuItem_ClearAds.Name = "MenuItem_ClearAds";
            this.MenuItem_ClearAds.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.MenuItem_ClearAds.Size = new System.Drawing.Size(162, 22);
            this.MenuItem_ClearAds.Text = "清空";
            this.MenuItem_ClearAds.Click += new System.EventHandler(this.MenuItem_ClearAds_Click);
            // 
            // MenuItem_AutoOrder
            // 
            this.MenuItem_AutoOrder.Name = "MenuItem_AutoOrder";
            this.MenuItem_AutoOrder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuItem_AutoOrder.Size = new System.Drawing.Size(162, 22);
            this.MenuItem_AutoOrder.Text = "自动排序";
            this.MenuItem_AutoOrder.Click += new System.EventHandler(this.MenuItem_AutoOrder_Click);
            // 
            // AddressTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 388);
            this.Controls.Add(this.AddressTableView);
            this.Controls.Add(this.SLmenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.SLmenuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddressTable";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "地址生成表";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddressTable_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.AddressTableView)).EndInit();
            this.SLmenuStrip.ResumeLayout(false);
            this.SLmenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView AddressTableView;
        private System.Windows.Forms.MenuStrip SLmenuStrip;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Load;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Save;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_ViewUnselect;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_ViewAll;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Revert;
        private System.Windows.Forms.ToolStripSeparator EdittoolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_AutoOrder;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_ClearAds;
        private System.Windows.Forms.ToolStripSeparator FiletoolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_ViewSelect;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_Close;
        private System.Windows.Forms.ToolStripMenuItem MenuItem_SaveAs;
        private System.Windows.Forms.DataGridViewTextBoxColumn ControlName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AttributeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Able;
    }
}