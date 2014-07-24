namespace CaVeGen.DesignViewFiles
{
    partial class PicWin
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
            this.label_Input = new System.Windows.Forms.Label();
            this.label_Output = new System.Windows.Forms.Label();
            this.gBox_status = new System.Windows.Forms.GroupBox();
            this.tBox_Result = new System.Windows.Forms.TextBox();
            this.rs_match = new System.Windows.Forms.Label();
            this.lb_Result = new System.Windows.Forms.Label();
            this.statusPic = new System.Windows.Forms.PictureBox();
            this.OutputPic = new System.Windows.Forms.PictureBox();
            this.InputPic = new System.Windows.Forms.PictureBox();
            this.gBox_status.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputPic)).BeginInit();
            this.SuspendLayout();
            // 
            // label_Input
            // 
            this.label_Input.AutoSize = true;
            this.label_Input.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Input.Location = new System.Drawing.Point(47, 9);
            this.label_Input.Name = "label_Input";
            this.label_Input.Size = new System.Drawing.Size(38, 17);
            this.label_Input.TabIndex = 3;
            this.label_Input.Text = "Input";
            // 
            // label_Output
            // 
            this.label_Output.AutoSize = true;
            this.label_Output.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Output.Location = new System.Drawing.Point(364, 9);
            this.label_Output.Name = "label_Output";
            this.label_Output.Size = new System.Drawing.Size(48, 17);
            this.label_Output.TabIndex = 4;
            this.label_Output.Text = "Output";
            // 
            // gBox_status
            // 
            this.gBox_status.BackColor = System.Drawing.SystemColors.Control;
            this.gBox_status.Controls.Add(this.tBox_Result);
            this.gBox_status.Controls.Add(this.rs_match);
            this.gBox_status.Controls.Add(this.lb_Result);
            this.gBox_status.Controls.Add(this.statusPic);
            this.gBox_status.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gBox_status.Location = new System.Drawing.Point(642, 38);
            this.gBox_status.Name = "gBox_status";
            this.gBox_status.Size = new System.Drawing.Size(174, 209);
            this.gBox_status.TabIndex = 5;
            this.gBox_status.TabStop = false;
            this.gBox_status.Text = "Inspection Satus";
            // 
            // tBox_Result
            // 
            this.tBox_Result.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tBox_Result.Location = new System.Drawing.Point(18, 83);
            this.tBox_Result.Name = "tBox_Result";
            this.tBox_Result.ReadOnly = true;
            this.tBox_Result.Size = new System.Drawing.Size(134, 16);
            this.tBox_Result.TabIndex = 7;
            this.tBox_Result.TabStop = false;
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
            // lb_Result
            // 
            this.lb_Result.AutoSize = true;
            this.lb_Result.Location = new System.Drawing.Point(15, 41);
            this.lb_Result.Name = "lb_Result";
            this.lb_Result.Size = new System.Drawing.Size(46, 17);
            this.lb_Result.TabIndex = 5;
            this.lb_Result.Text = "Result:";
            // 
            // statusPic
            // 
            this.statusPic.Image = global::CaVeGen.Properties.Resources.pass;
            this.statusPic.Location = new System.Drawing.Point(58, 142);
            this.statusPic.Name = "statusPic";
            this.statusPic.Size = new System.Drawing.Size(64, 32);
            this.statusPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.statusPic.TabIndex = 0;
            this.statusPic.TabStop = false;
            // 
            // OutputPic
            // 
            this.OutputPic.BackColor = System.Drawing.Color.White;
            this.OutputPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OutputPic.Location = new System.Drawing.Point(367, 38);
            this.OutputPic.Name = "OutputPic";
            this.OutputPic.Size = new System.Drawing.Size(235, 209);
            this.OutputPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.OutputPic.TabIndex = 2;
            this.OutputPic.TabStop = false;
            // 
            // InputPic
            // 
            this.InputPic.BackColor = System.Drawing.Color.White;
            this.InputPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.InputPic.Location = new System.Drawing.Point(50, 38);
            this.InputPic.Name = "InputPic";
            this.InputPic.Size = new System.Drawing.Size(235, 209);
            this.InputPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.InputPic.TabIndex = 1;
            this.InputPic.TabStop = false;
            // 
            // PicWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.gBox_status);
            this.Controls.Add(this.label_Output);
            this.Controls.Add(this.label_Input);
            this.Controls.Add(this.OutputPic);
            this.Controls.Add(this.InputPic);
            this.Name = "PicWin";
            this.Size = new System.Drawing.Size(819, 269);
            this.Resize += new System.EventHandler(this.PbxWin_Resize);
            this.gBox_status.ResumeLayout(false);
            this.gBox_status.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputPic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox InputPic;
        private System.Windows.Forms.PictureBox OutputPic;
        private System.Windows.Forms.Label label_Input;
        private System.Windows.Forms.Label label_Output;
        private System.Windows.Forms.GroupBox gBox_status;
        private System.Windows.Forms.PictureBox statusPic;
        private System.Windows.Forms.Label rs_match;
        private System.Windows.Forms.Label lb_Result;
        private System.Windows.Forms.TextBox tBox_Result;
    }
}
