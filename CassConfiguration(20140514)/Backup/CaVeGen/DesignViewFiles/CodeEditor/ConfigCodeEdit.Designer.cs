namespace CaVeGen.DesignViewFiles.CodeEditor
{
    partial class ConfigCodeEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigCodeEdit));
            this.plcCodeEditor1 = new CaVeGen.DesignViewFiles.CodeEditor.PLCCodeEditor();
            ((System.ComponentModel.ISupportInitialize)(this.plcCodeEditor1)).BeginInit();
            this.SuspendLayout();
            // 
            // plcCodeEditor1
            // 
            this.plcCodeEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plcCodeEditor1.IsDirty = false;
            this.plcCodeEditor1.Location = new System.Drawing.Point(0, 0);
            this.plcCodeEditor1.Name = "plcCodeEditor1";
            this.plcCodeEditor1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("plcCodeEditor1.OcxState")));
            this.plcCodeEditor1.Size = new System.Drawing.Size(583, 332);
            this.plcCodeEditor1.TabIndex = 0;
            // 
            // ConfigCodeEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.plcCodeEditor1);
            this.Name = "ConfigCodeEdit";
            this.Size = new System.Drawing.Size(583, 332);
            ((System.ComponentModel.ISupportInitialize)(this.plcCodeEditor1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PLCCodeEditor plcCodeEditor1;
    }
}
