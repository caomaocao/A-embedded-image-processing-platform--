namespace ControlTactic.SpecialControl
{
    partial class JieOuForm1
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
            this.SelectNum = new System.Windows.Forms.Label();
            this.JieOuNum = new System.Windows.Forms.TextBox();
            this.MatrixOfJO = new System.Windows.Forms.TabPage();
            this.JieOuMatrix = new System.Windows.Forms.DataGridView();
            this.JieOuAttribute = new System.Windows.Forms.TabPage();
            this.cCtrlMod_Value = new System.Windows.Forms.ComboBox();
            this.fSV_Value = new System.Windows.Forms.TextBox();
            this.fKd_Value = new System.Windows.Forms.TextBox();
            this.fKi_Value = new System.Windows.Forms.TextBox();
            this.SelectValue = new System.Windows.Forms.ComboBox();
            this.fKp_Value = new System.Windows.Forms.TextBox();
            this.cCtrlMod = new System.Windows.Forms.Label();
            this.fSV = new System.Windows.Forms.Label();
            this.fKd = new System.Windows.Forms.Label();
            this.fKi = new System.Windows.Forms.Label();
            this.fKp = new System.Windows.Forms.Label();
            this.JieOuTab = new System.Windows.Forms.TabControl();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.Num_Jieou = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ModifyNum = new System.Windows.Forms.Button();
            this.fMH = new System.Windows.Forms.Label();
            this.fML = new System.Windows.Forms.Label();
            this.fMV = new System.Windows.Forms.Label();
            this.fMV_Value = new System.Windows.Forms.TextBox();
            this.fML_Value = new System.Windows.Forms.TextBox();
            this.fMH_Value = new System.Windows.Forms.TextBox();
            this.MatrixOfJO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.JieOuMatrix)).BeginInit();
            this.JieOuAttribute.SuspendLayout();
            this.JieOuTab.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SelectNum
            // 
            this.SelectNum.AutoSize = true;
            this.SelectNum.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SelectNum.Location = new System.Drawing.Point(15, 24);
            this.SelectNum.Name = "SelectNum";
            this.SelectNum.Size = new System.Drawing.Size(63, 14);
            this.SelectNum.TabIndex = 0;
            this.SelectNum.Text = "路数选择";
            // 
            // JieOuNum
            // 
            this.JieOuNum.Location = new System.Drawing.Point(103, 27);
            this.JieOuNum.Name = "JieOuNum";
            this.JieOuNum.ReadOnly = true;
            this.JieOuNum.Size = new System.Drawing.Size(56, 21);
            this.JieOuNum.TabIndex = 1;
            // 
            // MatrixOfJO
            // 
            this.MatrixOfJO.BackColor = System.Drawing.SystemColors.Control;
            this.MatrixOfJO.Controls.Add(this.JieOuMatrix);
            this.MatrixOfJO.Location = new System.Drawing.Point(4, 21);
            this.MatrixOfJO.Name = "MatrixOfJO";
            this.MatrixOfJO.Padding = new System.Windows.Forms.Padding(3);
            this.MatrixOfJO.Size = new System.Drawing.Size(273, 198);
            this.MatrixOfJO.TabIndex = 2;
            this.MatrixOfJO.Text = "解耦矩阵";
            // 
            // JieOuMatrix
            // 
            this.JieOuMatrix.AllowUserToAddRows = false;
            this.JieOuMatrix.AllowUserToDeleteRows = false;
            this.JieOuMatrix.AllowUserToResizeColumns = false;
            this.JieOuMatrix.AllowUserToResizeRows = false;
            this.JieOuMatrix.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.JieOuMatrix.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.JieOuMatrix.ColumnHeadersHeight = 20;
            this.JieOuMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.JieOuMatrix.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JieOuMatrix.Location = new System.Drawing.Point(3, 3);
            this.JieOuMatrix.Name = "JieOuMatrix";
            this.JieOuMatrix.RowHeadersVisible = false;
            this.JieOuMatrix.RowHeadersWidth = 25;
            this.JieOuMatrix.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.JieOuMatrix.RowTemplate.Height = 23;
            this.JieOuMatrix.Size = new System.Drawing.Size(267, 192);
            this.JieOuMatrix.TabIndex = 0;
            // 
            // JieOuAttribute
            // 
            this.JieOuAttribute.BackColor = System.Drawing.SystemColors.Control;
            this.JieOuAttribute.Controls.Add(this.fMV_Value);
            this.JieOuAttribute.Controls.Add(this.fML_Value);
            this.JieOuAttribute.Controls.Add(this.fMH_Value);
            this.JieOuAttribute.Controls.Add(this.fMV);
            this.JieOuAttribute.Controls.Add(this.fML);
            this.JieOuAttribute.Controls.Add(this.fMH);
            this.JieOuAttribute.Controls.Add(this.cCtrlMod_Value);
            this.JieOuAttribute.Controls.Add(this.fSV_Value);
            this.JieOuAttribute.Controls.Add(this.fKd_Value);
            this.JieOuAttribute.Controls.Add(this.SelectNum);
            this.JieOuAttribute.Controls.Add(this.fKi_Value);
            this.JieOuAttribute.Controls.Add(this.SelectValue);
            this.JieOuAttribute.Controls.Add(this.fKp_Value);
            this.JieOuAttribute.Controls.Add(this.cCtrlMod);
            this.JieOuAttribute.Controls.Add(this.fSV);
            this.JieOuAttribute.Controls.Add(this.fKd);
            this.JieOuAttribute.Controls.Add(this.fKi);
            this.JieOuAttribute.Controls.Add(this.fKp);
            this.JieOuAttribute.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.JieOuAttribute.Location = new System.Drawing.Point(4, 21);
            this.JieOuAttribute.Name = "JieOuAttribute";
            this.JieOuAttribute.Padding = new System.Windows.Forms.Padding(3);
            this.JieOuAttribute.Size = new System.Drawing.Size(273, 198);
            this.JieOuAttribute.TabIndex = 1;
            this.JieOuAttribute.Text = "解耦属性";
            // 
            // cCtrlMod_Value
            // 
            this.cCtrlMod_Value.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cCtrlMod_Value.FormattingEnabled = true;
            this.cCtrlMod_Value.Items.AddRange(new object[] {
            "自动",
            "手动"});
            this.cCtrlMod_Value.Location = new System.Drawing.Point(205, 60);
            this.cCtrlMod_Value.Name = "cCtrlMod_Value";
            this.cCtrlMod_Value.Size = new System.Drawing.Size(52, 20);
            this.cCtrlMod_Value.TabIndex = 36;
            // 
            // fSV_Value
            // 
            this.fSV_Value.Location = new System.Drawing.Point(61, 156);
            this.fSV_Value.Name = "fSV_Value";
            this.fSV_Value.Size = new System.Drawing.Size(52, 21);
            this.fSV_Value.TabIndex = 34;
            // 
            // fKd_Value
            // 
            this.fKd_Value.Location = new System.Drawing.Point(61, 124);
            this.fKd_Value.Name = "fKd_Value";
            this.fKd_Value.Size = new System.Drawing.Size(52, 21);
            this.fKd_Value.TabIndex = 33;
            // 
            // fKi_Value
            // 
            this.fKi_Value.Location = new System.Drawing.Point(61, 92);
            this.fKi_Value.Name = "fKi_Value";
            this.fKi_Value.Size = new System.Drawing.Size(52, 21);
            this.fKi_Value.TabIndex = 32;
            // 
            // SelectValue
            // 
            this.SelectValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectValue.FormattingEnabled = true;
            this.SelectValue.Location = new System.Drawing.Point(84, 21);
            this.SelectValue.Name = "SelectValue";
            this.SelectValue.Size = new System.Drawing.Size(104, 20);
            this.SelectValue.TabIndex = 0;
            this.SelectValue.SelectedIndexChanged += new System.EventHandler(this.SelectValue_SelectedIndexChanged);
            // 
            // fKp_Value
            // 
            this.fKp_Value.Location = new System.Drawing.Point(61, 60);
            this.fKp_Value.Name = "fKp_Value";
            this.fKp_Value.Size = new System.Drawing.Size(52, 21);
            this.fKp_Value.TabIndex = 31;
            // 
            // cCtrlMod
            // 
            this.cCtrlMod.AutoSize = true;
            this.cCtrlMod.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cCtrlMod.Location = new System.Drawing.Point(140, 63);
            this.cCtrlMod.Name = "cCtrlMod";
            this.cCtrlMod.Size = new System.Drawing.Size(59, 12);
            this.cCtrlMod.TabIndex = 29;
            this.cCtrlMod.Text = "自动/手动";
            // 
            // fSV
            // 
            this.fSV.AutoSize = true;
            this.fSV.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fSV.Location = new System.Drawing.Point(14, 159);
            this.fSV.Name = "fSV";
            this.fSV.Size = new System.Drawing.Size(41, 12);
            this.fSV.TabIndex = 28;
            this.fSV.Text = "设定值";
            // 
            // fKd
            // 
            this.fKd.AutoSize = true;
            this.fKd.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fKd.Location = new System.Drawing.Point(21, 127);
            this.fKd.Name = "fKd";
            this.fKd.Size = new System.Drawing.Size(29, 12);
            this.fKd.TabIndex = 27;
            this.fKd.Text = "微分";
            // 
            // fKi
            // 
            this.fKi.AutoSize = true;
            this.fKi.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fKi.Location = new System.Drawing.Point(21, 95);
            this.fKi.Name = "fKi";
            this.fKi.Size = new System.Drawing.Size(29, 12);
            this.fKi.TabIndex = 26;
            this.fKi.Text = "积分";
            // 
            // fKp
            // 
            this.fKp.AutoSize = true;
            this.fKp.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fKp.Location = new System.Drawing.Point(21, 63);
            this.fKp.Name = "fKp";
            this.fKp.Size = new System.Drawing.Size(29, 12);
            this.fKp.TabIndex = 25;
            this.fKp.Text = "比例";
            // 
            // JieOuTab
            // 
            this.JieOuTab.Controls.Add(this.JieOuAttribute);
            this.JieOuTab.Controls.Add(this.MatrixOfJO);
            this.JieOuTab.Location = new System.Drawing.Point(18, 90);
            this.JieOuTab.MaximumSize = new System.Drawing.Size(351, 223);
            this.JieOuTab.Name = "JieOuTab";
            this.JieOuTab.SelectedIndex = 0;
            this.JieOuTab.Size = new System.Drawing.Size(281, 223);
            this.JieOuTab.TabIndex = 0;
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(120, 330);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 2;
            this.OK.Text = "确定";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(220, 330);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 3;
            this.Cancel.Text = "取消";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // Num_Jieou
            // 
            this.Num_Jieou.AutoSize = true;
            this.Num_Jieou.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Num_Jieou.Location = new System.Drawing.Point(32, 30);
            this.Num_Jieou.Name = "Num_Jieou";
            this.Num_Jieou.Size = new System.Drawing.Size(63, 14);
            this.Num_Jieou.TabIndex = 4;
            this.Num_Jieou.Text = "解耦路数";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ModifyNum);
            this.groupBox1.Controls.Add(this.JieOuNum);
            this.groupBox1.Controls.Add(this.Num_Jieou);
            this.groupBox1.Location = new System.Drawing.Point(18, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 62);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "解耦路数设定";
            // 
            // ModifyNum
            // 
            this.ModifyNum.Location = new System.Drawing.Point(189, 27);
            this.ModifyNum.Name = "ModifyNum";
            this.ModifyNum.Size = new System.Drawing.Size(60, 23);
            this.ModifyNum.TabIndex = 5;
            this.ModifyNum.Text = "设置";
            this.ModifyNum.UseVisualStyleBackColor = true;
            this.ModifyNum.Click += new System.EventHandler(this.ModifyNum_Click);
            // 
            // fMH
            // 
            this.fMH.AutoSize = true;
            this.fMH.Location = new System.Drawing.Point(134, 95);
            this.fMH.Name = "fMH";
            this.fMH.Size = new System.Drawing.Size(65, 12);
            this.fMH.TabIndex = 37;
            this.fMH.Text = "控制量上限";
            // 
            // fML
            // 
            this.fML.AutoSize = true;
            this.fML.Location = new System.Drawing.Point(134, 127);
            this.fML.Name = "fML";
            this.fML.Size = new System.Drawing.Size(65, 12);
            this.fML.TabIndex = 38;
            this.fML.Text = "控制量下限";
            // 
            // fMV
            // 
            this.fMV.AutoSize = true;
            this.fMV.Location = new System.Drawing.Point(134, 159);
            this.fMV.Name = "fMV";
            this.fMV.Size = new System.Drawing.Size(65, 12);
            this.fMV.TabIndex = 39;
            this.fMV.Text = "控制量输出";
            // 
            // fMV_Value
            // 
            this.fMV_Value.Location = new System.Drawing.Point(205, 156);
            this.fMV_Value.Name = "fMV_Value";
            this.fMV_Value.Size = new System.Drawing.Size(52, 21);
            this.fMV_Value.TabIndex = 42;
            // 
            // fML_Value
            // 
            this.fML_Value.Location = new System.Drawing.Point(205, 124);
            this.fML_Value.Name = "fML_Value";
            this.fML_Value.Size = new System.Drawing.Size(52, 21);
            this.fML_Value.TabIndex = 41;
            // 
            // fMH_Value
            // 
            this.fMH_Value.Location = new System.Drawing.Point(205, 92);
            this.fMH_Value.Name = "fMH_Value";
            this.fMH_Value.Size = new System.Drawing.Size(52, 21);
            this.fMH_Value.TabIndex = 40;
            // 
            // JieOuForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(317, 363);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.JieOuTab);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JieOuForm1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "解耦控制组态";
            this.MatrixOfJO.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.JieOuMatrix)).EndInit();
            this.JieOuAttribute.ResumeLayout(false);
            this.JieOuAttribute.PerformLayout();
            this.JieOuTab.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label SelectNum;
        private System.Windows.Forms.TextBox JieOuNum;
        private System.Windows.Forms.TabPage MatrixOfJO;
        private System.Windows.Forms.TabPage JieOuAttribute;
        private System.Windows.Forms.TabControl JieOuTab;
        private System.Windows.Forms.ComboBox SelectValue;
        private System.Windows.Forms.DataGridView JieOuMatrix;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.ComboBox cCtrlMod_Value;
        private System.Windows.Forms.TextBox fSV_Value;
        private System.Windows.Forms.TextBox fKd_Value;
        private System.Windows.Forms.TextBox fKi_Value;
        private System.Windows.Forms.TextBox fKp_Value;
        private System.Windows.Forms.Label cCtrlMod;
        private System.Windows.Forms.Label fSV;
        private System.Windows.Forms.Label fKd;
        private System.Windows.Forms.Label fKi;
        private System.Windows.Forms.Label fKp;
        private System.Windows.Forms.Label Num_Jieou;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ModifyNum;
        private System.Windows.Forms.Label fMH;
        private System.Windows.Forms.Label fML;
        private System.Windows.Forms.Label fMV;
        private System.Windows.Forms.TextBox fMV_Value;
        private System.Windows.Forms.TextBox fML_Value;
        private System.Windows.Forms.TextBox fMH_Value;
    }
}