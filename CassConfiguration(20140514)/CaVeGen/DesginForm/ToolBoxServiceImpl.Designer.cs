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
            this.explorerBarTool.Alpha = 0;
            this.explorerBarTool.BackgroundFormatStyle.BackColor = System.Drawing.Color.White;
            this.explorerBarTool.BackgroundFormatStyle.BackColorGradient = System.Drawing.SystemColors.Window;
            this.explorerBarTool.BackgroundFormatStyle.ForeColor = System.Drawing.SystemColors.Window;
            this.explorerBarTool.BackgroundFormatStyle.ForegroundThemeStyle = Janus.Windows.ExplorerBar.ForegroundThemeStyle.None;
            this.explorerBarTool.BackgroundThemeStyle = Janus.Windows.ExplorerBar.BackgroundThemeStyle.None;
            this.explorerBarTool.BlendColor = System.Drawing.SystemColors.Window;
            this.explorerBarTool.BorderStyle = Janus.Windows.ExplorerBar.BorderStyle.Flat;
            this.explorerBarTool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerBarTool.FlatBorderColor = System.Drawing.SystemColors.Window;
            this.explorerBarTool.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.explorerBarTool.ForeColor = System.Drawing.SystemColors.Window;
            this.explorerBarTool.GroupSeparation = 3;
            this.explorerBarTool.GroupsStateStyles.FormatStyle.Font = new System.Drawing.Font("微软雅黑", 9.5F);
            this.explorerBarTool.GroupsStateStyles.FormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.FormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.FormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.FormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.HotFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.True;
            this.explorerBarTool.GroupsStateStyles.HotFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.HotFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.HotFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.PressedFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.True;
            this.explorerBarTool.GroupsStateStyles.PressedFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.PressedFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.PressedFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.SelectedFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.True;
            this.explorerBarTool.GroupsStateStyles.SelectedFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.SelectedFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.GroupsStateStyles.SelectedFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.DisabledFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.DisabledFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.FormatStyle.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.explorerBarTool.ItemsStateStyles.FormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.FormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.FormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.FormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.HotFormatStyle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.explorerBarTool.ItemsStateStyles.HotFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.True;
            this.explorerBarTool.ItemsStateStyles.HotFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.HotFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.HotFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.PressedFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.True;
            this.explorerBarTool.ItemsStateStyles.PressedFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.PressedFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.PressedFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.SelectedFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.True;
            this.explorerBarTool.ItemsStateStyles.SelectedFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.SelectedFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.ItemsStateStyles.SelectedFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.KeepSelection = true;
            this.explorerBarTool.LeftMargin = 3;
            this.explorerBarTool.Location = new System.Drawing.Point(0, 0);
            this.explorerBarTool.Name = "explorerBarTool";
            this.explorerBarTool.RightMargin = 3;
            this.explorerBarTool.Size = new System.Drawing.Size(162, 521);
            this.explorerBarTool.SpecialGroupsStateStyles.DisabledFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.DisabledFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.DisabledFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.DisabledFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.FormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.FormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.FormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.FormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.HotFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.True;
            this.explorerBarTool.SpecialGroupsStateStyles.HotFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.HotFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.HotFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.PressedFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.True;
            this.explorerBarTool.SpecialGroupsStateStyles.PressedFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.PressedFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.PressedFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.SelectedFormatStyle.FontBold = Janus.Windows.ExplorerBar.TriState.True;
            this.explorerBarTool.SpecialGroupsStateStyles.SelectedFormatStyle.FontItalic = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.SelectedFormatStyle.FontStrikeout = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.SpecialGroupsStateStyles.SelectedFormatStyle.FontUnderline = Janus.Windows.ExplorerBar.TriState.False;
            this.explorerBarTool.TabIndex = 0;
            this.explorerBarTool.ThemedAreas = Janus.Windows.ExplorerBar.ThemedArea.None;
            this.explorerBarTool.TopMargin = 3;
            this.explorerBarTool.MouseDown += new System.Windows.Forms.MouseEventHandler(this.explorerBarTool_MouseDown);
            // 
            // ToolBoxServiceImpl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.explorerBarTool);
            this.Name = "ToolBoxServiceImpl";
            this.Size = new System.Drawing.Size(162, 521);
            ((System.ComponentModel.ISupportInitialize)(this.explorerBarTool)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ExplorerBar explorerBarTool;

    }
}
