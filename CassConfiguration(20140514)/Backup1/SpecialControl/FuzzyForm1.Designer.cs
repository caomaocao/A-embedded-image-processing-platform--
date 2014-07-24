namespace ControlTactic.SpecialControl
{
    partial class FuzzyForm1
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
            this.ModifyRow = new System.Windows.Forms.Button();
            this.RowValue = new System.Windows.Forms.TextBox();
            this.ColumnValue = new System.Windows.Forms.TextBox();
            this.FuzzyRegion = new System.Windows.Forms.GroupBox();
            this.ModifyColumn = new System.Windows.Forms.Button();
            this.ColumnRegion = new System.Windows.Forms.Label();
            this.RowRegion = new System.Windows.Forms.Label();
            this.ControlTable = new System.Windows.Forms.GroupBox();
            this.TableExplain2 = new System.Windows.Forms.Label();
            this.SetTable = new System.Windows.Forms.Button();
            this.TableExplain1 = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.FuzzyRegion.SuspendLayout();
            this.ControlTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // ModifyRow
            // 
            this.ModifyRow.Location = new System.Drawing.Point(186, 42);
            this.ModifyRow.Name = "ModifyRow";
            this.ModifyRow.Size = new System.Drawing.Size(75, 23);
            this.ModifyRow.TabIndex = 0;
            this.ModifyRow.Text = "修改";
            this.ModifyRow.UseVisualStyleBackColor = true;
            this.ModifyRow.Click += new System.EventHandler(this.ModifyRow_Click);
            // 
            // RowValue
            // 
            this.RowValue.Location = new System.Drawing.Point(20, 43);
            this.RowValue.Name = "RowValue";
            this.RowValue.ReadOnly = true;
            this.RowValue.Size = new System.Drawing.Size(151, 21);
            this.RowValue.TabIndex = 1;
            // 
            // ColumnValue
            // 
            this.ColumnValue.Location = new System.Drawing.Point(20, 112);
            this.ColumnValue.Name = "ColumnValue";
            this.ColumnValue.ReadOnly = true;
            this.ColumnValue.Size = new System.Drawing.Size(151, 21);
            this.ColumnValue.TabIndex = 2;
            // 
            // FuzzyRegion
            // 
            this.FuzzyRegion.Controls.Add(this.ModifyColumn);
            this.FuzzyRegion.Controls.Add(this.ColumnRegion);
            this.FuzzyRegion.Controls.Add(this.RowRegion);
            this.FuzzyRegion.Controls.Add(this.ModifyRow);
            this.FuzzyRegion.Controls.Add(this.RowValue);
            this.FuzzyRegion.Controls.Add(this.ColumnValue);
            this.FuzzyRegion.Location = new System.Drawing.Point(12, 12);
            this.FuzzyRegion.Name = "FuzzyRegion";
            this.FuzzyRegion.Size = new System.Drawing.Size(273, 149);
            this.FuzzyRegion.TabIndex = 3;
            this.FuzzyRegion.TabStop = false;
            this.FuzzyRegion.Text = "模糊论域";
            // 
            // ModifyColumn
            // 
            this.ModifyColumn.Location = new System.Drawing.Point(186, 111);
            this.ModifyColumn.Name = "ModifyColumn";
            this.ModifyColumn.Size = new System.Drawing.Size(75, 23);
            this.ModifyColumn.TabIndex = 0;
            this.ModifyColumn.Text = "修改";
            this.ModifyColumn.UseVisualStyleBackColor = true;
            this.ModifyColumn.Click += new System.EventHandler(this.ModifyColumn_Click);
            // 
            // ColumnRegion
            // 
            this.ColumnRegion.AutoSize = true;
            this.ColumnRegion.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ColumnRegion.Location = new System.Drawing.Point(9, 86);
            this.ColumnRegion.Name = "ColumnRegion";
            this.ColumnRegion.Size = new System.Drawing.Size(133, 14);
            this.ColumnRegion.TabIndex = 4;
            this.ColumnRegion.Text = "偏差变化率模糊论域";
            // 
            // RowRegion
            // 
            this.RowRegion.AutoSize = true;
            this.RowRegion.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RowRegion.Location = new System.Drawing.Point(9, 21);
            this.RowRegion.Name = "RowRegion";
            this.RowRegion.Size = new System.Drawing.Size(91, 14);
            this.RowRegion.TabIndex = 3;
            this.RowRegion.Text = "偏差模糊论域";
            // 
            // ControlTable
            // 
            this.ControlTable.Controls.Add(this.TableExplain2);
            this.ControlTable.Controls.Add(this.SetTable);
            this.ControlTable.Controls.Add(this.TableExplain1);
            this.ControlTable.Location = new System.Drawing.Point(12, 178);
            this.ControlTable.Name = "ControlTable";
            this.ControlTable.Size = new System.Drawing.Size(275, 86);
            this.ControlTable.TabIndex = 4;
            this.ControlTable.TabStop = false;
            this.ControlTable.Text = "模糊控制表";
            // 
            // TableExplain2
            // 
            this.TableExplain2.AutoSize = true;
            this.TableExplain2.Location = new System.Drawing.Point(10, 54);
            this.TableExplain2.Name = "TableExplain2";
            this.TableExplain2.Size = new System.Drawing.Size(161, 12);
            this.TableExplain2.TabIndex = 7;
            this.TableExplain2.Text = "率模糊论域为行的模糊控制表";
            // 
            // SetTable
            // 
            this.SetTable.Location = new System.Drawing.Point(189, 37);
            this.SetTable.Name = "SetTable";
            this.SetTable.Size = new System.Drawing.Size(75, 23);
            this.SetTable.TabIndex = 6;
            this.SetTable.Text = "设置";
            this.SetTable.UseVisualStyleBackColor = true;
            this.SetTable.Click += new System.EventHandler(this.SetTable_Click);
            // 
            // TableExplain1
            // 
            this.TableExplain1.AutoSize = true;
            this.TableExplain1.Location = new System.Drawing.Point(10, 29);
            this.TableExplain1.Name = "TableExplain1";
            this.TableExplain1.Size = new System.Drawing.Size(173, 12);
            this.TableExplain1.TabIndex = 5;
            this.TableExplain1.Text = "以偏差模糊论域为列，偏差变化";
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(116, 271);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 5;
            this.OK.Text = "确认";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(210, 271);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 6;
            this.Cancel.Text = "取消";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // FuzzyForm1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 316);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.ControlTable);
            this.Controls.Add(this.FuzzyRegion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FuzzyForm1";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "模糊控制组态";
            this.FuzzyRegion.ResumeLayout(false);
            this.FuzzyRegion.PerformLayout();
            this.ControlTable.ResumeLayout(false);
            this.ControlTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ModifyRow;
        private System.Windows.Forms.TextBox RowValue;
        private System.Windows.Forms.TextBox ColumnValue;
        private System.Windows.Forms.GroupBox FuzzyRegion;
        private System.Windows.Forms.GroupBox ControlTable;
        private System.Windows.Forms.Button ModifyColumn;
        private System.Windows.Forms.Label ColumnRegion;
        private System.Windows.Forms.Label RowRegion;
        private System.Windows.Forms.Button SetTable;
        private System.Windows.Forms.Label TableExplain1;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label TableExplain2;
    }
}