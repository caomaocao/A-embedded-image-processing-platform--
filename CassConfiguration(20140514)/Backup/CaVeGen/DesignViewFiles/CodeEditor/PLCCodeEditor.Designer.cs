namespace CaVeGen.DesignViewFiles.CodeEditor
{
    partial class PLCCodeEditor
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
            this.Tiptimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // Tiptimer
            // 
            this.Tiptimer.Interval = 400;
            this.Tiptimer.Tick += new System.EventHandler(this.Tiptimer_Tick);
            // 
            // PLCCodeEditor
            // 
            this.HScroll += new AxCodeSense.ICodeSenseEvents_HScrollEventHandler(this.PLCCodeEditor_HScroll);
            this.Change += new AxCodeSense.ICodeSenseEvents_ChangeEventHandler(this.PLCCodeEditor_Change);
            this.KillFocus += new AxCodeSense.ICodeSenseEvents_KillFocusEventHandler(this.PLCCodeEditor_KillFocus);
            this.VScroll += new AxCodeSense.ICodeSenseEvents_VScrollEventHandler(this.PLCCodeEditor_VScroll);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer Tiptimer;
    }
}
