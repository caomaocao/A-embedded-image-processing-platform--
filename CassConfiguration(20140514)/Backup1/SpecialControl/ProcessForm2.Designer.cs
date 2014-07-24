namespace ControlTactic.SpecialControl
{
    partial class ProcessForm2
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("全局变量");
            this.ExpressBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ControlTree = new System.Windows.Forms.TreeView();
            this.Element = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ElementType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.Cencel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExpressBox
            // 
            this.ExpressBox.Location = new System.Drawing.Point(95, 28);
            this.ExpressBox.Name = "ExpressBox";
            this.ExpressBox.Size = new System.Drawing.Size(234, 21);
            this.ExpressBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(34, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "表达式";
            // 
            // ControlTree
            // 
            this.ControlTree.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ControlTree.Location = new System.Drawing.Point(37, 94);
            this.ControlTree.Name = "ControlTree";
            treeNode1.Name = "root";
            treeNode1.Text = "全局变量";
            this.ControlTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.ControlTree.Size = new System.Drawing.Size(133, 195);
            this.ControlTree.TabIndex = 2;
            this.ControlTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ControlTree_NodeMouseDoubleClick);
            // 
            // Element
            // 
            this.Element.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Element.FormattingEnabled = true;
            this.Element.ItemHeight = 14;
            this.Element.Location = new System.Drawing.Point(196, 131);
            this.Element.Name = "Element";
            this.Element.Size = new System.Drawing.Size(133, 158);
            this.Element.TabIndex = 3;
            this.Element.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Element_MouseDoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(34, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "变量";
            // 
            // ElementType
            // 
            this.ElementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ElementType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ElementType.FormattingEnabled = true;
            this.ElementType.Items.AddRange(new object[] {
            "算术运算",
            "逻辑运算",
            "一元函数",
            "二元函数"});
            this.ElementType.Location = new System.Drawing.Point(196, 94);
            this.ElementType.Name = "ElementType";
            this.ElementType.Size = new System.Drawing.Size(101, 22);
            this.ElementType.TabIndex = 5;
            this.ElementType.SelectedIndexChanged += new System.EventHandler(this.ElementType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(194, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 14);
            this.label3.TabIndex = 6;
            this.label3.Text = "运算＆函数";
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(95, 302);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 7;
            this.OK.Text = "确认";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cencel
            // 
            this.Cencel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cencel.Location = new System.Drawing.Point(196, 302);
            this.Cencel.Name = "Cencel";
            this.Cencel.Size = new System.Drawing.Size(75, 23);
            this.Cencel.TabIndex = 8;
            this.Cencel.Text = "取消";
            this.Cencel.UseVisualStyleBackColor = true;
            // 
            // ProcessForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 336);
            this.Controls.Add(this.Cencel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ElementType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Element);
            this.Controls.Add(this.ControlTree);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExpressBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcessForm2";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "表达式输入";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ExpressBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView ControlTree;
        private System.Windows.Forms.ListBox Element;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ElementType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cencel;
    }
}