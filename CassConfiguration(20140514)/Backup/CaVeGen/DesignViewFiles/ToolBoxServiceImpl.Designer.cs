using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;// 使用 Assembly 类需用此命名空间
using Janus.Windows.ExplorerBar;

namespace CaVeGen.DesignViewFiles
{
    partial class ToolBoxServiceImpl
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
            this.explorerBarTool = new Janus.Windows.ExplorerBar.ExplorerBar();
            ((System.ComponentModel.ISupportInitialize)(this.explorerBarTool)).BeginInit();
            this.SuspendLayout();
            // 
            // explorerBarTool
            // 
            this.explorerBarTool.BlendColor = System.Drawing.SystemColors.Window;
            this.explorerBarTool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerBarTool.FlatBorderColor = System.Drawing.SystemColors.ControlDark;
            this.explorerBarTool.Location = new System.Drawing.Point(0, 0);
            this.explorerBarTool.Name = "explorerBarTool";
            this.explorerBarTool.Size = new System.Drawing.Size(81, 373);
            this.explorerBarTool.TabIndex = 0;
            this.explorerBarTool.MouseDown += new System.Windows.Forms.MouseEventHandler(this.explorerBarTool_MouseDown);
            // 
            // ToolBoxServiceImpl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.explorerBarTool);
            this.Name = "ToolBoxServiceImpl";
            this.Size = new System.Drawing.Size(81, 373);
            ((System.ComponentModel.ISupportInitialize)(this.explorerBarTool)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Janus.Windows.ExplorerBar.ExplorerBar explorerBarTool;
    }
}
