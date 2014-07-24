namespace CaVeGen.DesginForm
{
    partial class InspectionResultMark
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
            this.gBox_status = new System.Windows.Forms.GroupBox();
            this.rs_match = new System.Windows.Forms.Label();
            this.lb_match = new System.Windows.Forms.Label();
            this.rs_fail = new System.Windows.Forms.Label();
            this.rs_pass = new System.Windows.Forms.Label();
            this.lb_Fail = new System.Windows.Forms.Label();
            this.lb_Pass = new System.Windows.Forms.Label();
            this.statusPic = new System.Windows.Forms.PictureBox();
            this.gBox_status.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPic)).BeginInit();
            this.SuspendLayout();
            // 
            // gBox_status
            // 
            this.gBox_status.BackColor = System.Drawing.SystemColors.Control;
            this.gBox_status.Controls.Add(this.rs_match);
            this.gBox_status.Controls.Add(this.lb_match);
            this.gBox_status.Controls.Add(this.rs_fail);
            this.gBox_status.Controls.Add(this.rs_pass);
            this.gBox_status.Controls.Add(this.lb_Fail);
            this.gBox_status.Controls.Add(this.lb_Pass);
            this.gBox_status.Controls.Add(this.statusPic);
            this.gBox_status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gBox_status.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gBox_status.Location = new System.Drawing.Point(0, 0);
            this.gBox_status.Name = "gBox_status";
            this.gBox_status.Size = new System.Drawing.Size(174, 209);
            this.gBox_status.TabIndex = 6;
            this.gBox_status.TabStop = false;
            this.gBox_status.Text = "Inspection Satus";
            this.gBox_status.Resize += new System.EventHandler(this.gBox_status_Resize);
            // 
            // rs_match
            // 
            this.rs_match.AutoSize = true;
            this.rs_match.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rs_match.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rs_match.Location = new System.Drawing.Point(122, 99);
            this.rs_match.Name = "rs_match";
            this.rs_match.Size = new System.Drawing.Size(0, 17);
            this.rs_match.TabIndex = 6;
            // 
            // lb_match
            // 
            this.lb_match.AutoSize = true;
            this.lb_match.Location = new System.Drawing.Point(7, 99);
            this.lb_match.Name = "lb_match";
            this.lb_match.Size = new System.Drawing.Size(102, 17);
            this.lb_match.TabIndex = 5;
            this.lb_match.Text = "Match Percent : ";
            // 
            // rs_fail
            // 
            this.rs_fail.AutoSize = true;
            this.rs_fail.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rs_fail.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rs_fail.Location = new System.Drawing.Point(122, 69);
            this.rs_fail.Name = "rs_fail";
            this.rs_fail.Size = new System.Drawing.Size(15, 17);
            this.rs_fail.TabIndex = 4;
            this.rs_fail.Text = "0";
            // 
            // rs_pass
            // 
            this.rs_pass.AutoSize = true;
            this.rs_pass.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rs_pass.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rs_pass.Location = new System.Drawing.Point(122, 39);
            this.rs_pass.Name = "rs_pass";
            this.rs_pass.Size = new System.Drawing.Size(15, 17);
            this.rs_pass.TabIndex = 3;
            this.rs_pass.Text = "0";
            // 
            // lb_Fail
            // 
            this.lb_Fail.AutoSize = true;
            this.lb_Fail.Location = new System.Drawing.Point(6, 69);
            this.lb_Fail.Name = "lb_Fail";
            this.lb_Fail.Size = new System.Drawing.Size(80, 17);
            this.lb_Fail.TabIndex = 2;
            this.lb_Fail.Text = "Fail  Count : ";
            // 
            // lb_Pass
            // 
            this.lb_Pass.AutoSize = true;
            this.lb_Pass.Location = new System.Drawing.Point(7, 39);
            this.lb_Pass.Name = "lb_Pass";
            this.lb_Pass.Size = new System.Drawing.Size(79, 17);
            this.lb_Pass.TabIndex = 1;
            this.lb_Pass.Text = "Pass Count :";
            // 
            // statusPic
            // 
            this.statusPic.Image = global::CaVeGen.Properties.Resources.pass;
            this.statusPic.Location = new System.Drawing.Point(51, 154);
            this.statusPic.Name = "statusPic";
            this.statusPic.Size = new System.Drawing.Size(64, 32);
            this.statusPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.statusPic.TabIndex = 0;
            this.statusPic.TabStop = false;
            // 
            // InspectionResultMark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gBox_status);
            this.Name = "InspectionResultMark";
            this.Size = new System.Drawing.Size(174, 209);
            this.gBox_status.ResumeLayout(false);
            this.gBox_status.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gBox_status;
        private System.Windows.Forms.Label rs_match;
        private System.Windows.Forms.Label lb_match;
        private System.Windows.Forms.Label rs_fail;
        private System.Windows.Forms.Label rs_pass;
        private System.Windows.Forms.Label lb_Fail;
        private System.Windows.Forms.Label lb_Pass;
        private System.Windows.Forms.PictureBox statusPic;
    }
}
