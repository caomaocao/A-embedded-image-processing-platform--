using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CaVeGen.DesignViewFiles;
using System.Configuration;

namespace CaVeGen
{
    partial class CassViewGenerator
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripMenuItem RopenPageToolStripMenuItem;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CassViewGenerator));
            this.treeMenuPage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddNewItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.existDesignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solutionImageList = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabelCurrentDesign = new System.Windows.Forms.ToolStripStatusLabel();
            this.bgw_processbar = new System.Windows.Forms.ToolStripProgressBar();
            this.CurrentMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.designMousePosition = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.designViewStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveForm = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.ResetWorkSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeProjecttoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undotoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redotoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aliginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.middlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.centersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Windows = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Controls = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Property = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_TreeView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_PorList = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_ErrorForm = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_EffectPic = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem_ShowNum = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem_CreateCodelist = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem_SetAdress = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem_CompliedMode = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Config = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_IL = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Simulation = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem_Complied = new System.Windows.Forms.ToolStripMenuItem();
            this.工程分类ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工件识别ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数字识别ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.缺陷检测ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.角度检测ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.字符检测ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.尺寸测量ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.颜色比对ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.匹配定位ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设备选择ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uSBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ov7670ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.执行ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.下载ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.分辨率配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ControlEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripOperation = new System.Windows.Forms.ToolStrip();
            this.creatProjecttoolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addItemtoolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.designItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.existItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveCurrentFormtoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveProjecttoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.undotoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.redotoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.cuttoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.copytoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pastetoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.leftAligntoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.middleAligntoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.rightAligntoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.topAligntoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.centerAligntoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.buttomsAligntoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.bringToFrontToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sendToBackStripButton = new System.Windows.Forms.ToolStripButton();
            this.linkToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.editToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomtoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.reducetoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.CompliedtoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ModetoolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.RunToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.DebugByStepToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.StopDebugToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.controlEditOperation = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewDesign = new System.Windows.Forms.Panel();
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.tabControlView = new System.Windows.Forms.TabControl();
            this.solutionTreeView = new System.Windows.Forms.TreeView();
            this.ProListBox = new System.Windows.Forms.ListView();
            this.proName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.proInfomation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProjectMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenOldStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddNewStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ModifyStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenNewStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backGroundWorker = new System.ComponentModel.BackgroundWorker();
            this.toolboxServiceImpl = new CaVeGen.DesignViewFiles.ToolBoxServiceImpl();
            this.errorForm = new CaVeGen.DesignViewFiles.CodeEditor.ErrorViewControl2();
            this.picWin = new CaVeGen.DesignViewFiles.PicWin();
            this.controlfilteredPropertyGrid = new CaVeGen.DesignViewFiles.FilterProperty.FilteredPropertyGrid();
            RopenPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeMenuPage.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStripOperation.SuspendLayout();
            this.controlEditOperation.SuspendLayout();
            this.ViewDesign.SuspendLayout();
            this.BottomPanel.SuspendLayout();
            this.ProjectMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // RopenPageToolStripMenuItem
            // 
            RopenPageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("RopenPageToolStripMenuItem.Image")));
            RopenPageToolStripMenuItem.Name = "RopenPageToolStripMenuItem";
            RopenPageToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            RopenPageToolStripMenuItem.Text = "打开";
            RopenPageToolStripMenuItem.Click += new System.EventHandler(this.openPageToolStripMenuItem1_Click);
            // 
            // treeMenuPage
            // 
            this.treeMenuPage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            RopenPageToolStripMenuItem,
            this.AddNewItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.treeMenuPage.Name = "treeMenuDirectory";
            this.treeMenuPage.Size = new System.Drawing.Size(113, 92);
            this.treeMenuPage.Opening += new System.ComponentModel.CancelEventHandler(this.treeMenuPage_Opening);
            // 
            // AddNewItem
            // 
            this.AddNewItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newViewToolStripMenuItem,
            this.existDesignToolStripMenuItem});
            this.AddNewItem.Name = "AddNewItem";
            this.AddNewItem.Size = new System.Drawing.Size(112, 22);
            this.AddNewItem.Text = "添加　";
            // 
            // newViewToolStripMenuItem
            // 
            this.newViewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newViewToolStripMenuItem.Image")));
            this.newViewToolStripMenuItem.Name = "newViewToolStripMenuItem";
            this.newViewToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.newViewToolStripMenuItem.Text = "新建策略";
            this.newViewToolStripMenuItem.Click += new System.EventHandler(this.newViewToolStripMenuItem_Click);
            // 
            // existDesignToolStripMenuItem
            // 
            this.existDesignToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("existDesignToolStripMenuItem.Image")));
            this.existDesignToolStripMenuItem.Name = "existDesignToolStripMenuItem";
            this.existDesignToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.existDesignToolStripMenuItem.Text = "现有策略";
            this.existDesignToolStripMenuItem.Click += new System.EventHandler(this.existDesignToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("renameToolStripMenuItem.Image")));
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.renameToolStripMenuItem.Text = "修改";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteToolStripMenuItem.Image")));
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.deleteToolStripMenuItem.Text = "删除";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // solutionImageList
            // 
            this.solutionImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("solutionImageList.ImageStream")));
            this.solutionImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.solutionImageList.Images.SetKeyName(0, "页面1.png");
            this.solutionImageList.Images.SetKeyName(1, "folder.png");
            this.solutionImageList.Images.SetKeyName(2, "openedFolder.png");
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelCurrentDesign,
            this.bgw_processbar,
            this.CurrentMode,
            this.designMousePosition});
            this.statusStrip.Location = new System.Drawing.Point(0, 434);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1028, 22);
            this.statusStrip.Stretch = false;
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabelCurrentDesign
            // 
            this.statusLabelCurrentDesign.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.statusLabelCurrentDesign.Name = "statusLabelCurrentDesign";
            this.statusLabelCurrentDesign.Size = new System.Drawing.Size(763, 17);
            this.statusLabelCurrentDesign.Spring = true;
            this.statusLabelCurrentDesign.Text = "准备";
            this.statusLabelCurrentDesign.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bgw_processbar
            // 
            this.bgw_processbar.Name = "bgw_processbar";
            this.bgw_processbar.Size = new System.Drawing.Size(100, 16);
            this.bgw_processbar.Visible = false;
            // 
            // CurrentMode
            // 
            this.CurrentMode.AutoSize = false;
            this.CurrentMode.Name = "CurrentMode";
            this.CurrentMode.Size = new System.Drawing.Size(150, 17);
            this.CurrentMode.Text = "当前编辑模式         ";
            // 
            // designMousePosition
            // 
            this.designMousePosition.AutoSize = false;
            this.designMousePosition.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.designMousePosition.Name = "designMousePosition";
            this.designMousePosition.Size = new System.Drawing.Size(100, 17);
            this.designMousePosition.Text = "鼠标位置                  ";
            this.designMousePosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolStripMenuItem_Windows,
            this.projectToolStripMenuItem,
            this.工程分类ToolStripMenuItem,
            this.设备选择ToolStripMenuItem,
            this.执行ToolStripMenuItem,
            this.配置ToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1028, 25);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewFormToolStripMenuItem,
            this.autoSetToolStripMenuItem,
            this.saveForm,
            this.saveProject,
            this.ResetWorkSpaceToolStripMenuItem,
            this.closeProjecttoolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.fileToolStripMenuItem.Text = "文件";
            // 
            // addNewFormToolStripMenuItem
            // 
            this.addNewFormToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectMenuItem,
            this.designViewStripMenu});
            this.addNewFormToolStripMenuItem.Name = "addNewFormToolStripMenuItem";
            this.addNewFormToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.addNewFormToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.addNewFormToolStripMenuItem.Text = "新建(&N)";
            this.addNewFormToolStripMenuItem.ToolTipText = "新建(&N)";
            // 
            // projectMenuItem
            // 
            this.projectMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("projectMenuItem.Image")));
            this.projectMenuItem.Name = "projectMenuItem";
            this.projectMenuItem.Size = new System.Drawing.Size(124, 22);
            this.projectMenuItem.Text = "工程";
            this.projectMenuItem.ToolTipText = "工程";
            this.projectMenuItem.Click += new System.EventHandler(this.projectMenuItem_Click);
            this.projectMenuItem.EnabledChanged += new System.EventHandler(this.projectMenuItem_EnabledChanged);
            // 
            // designViewStripMenu
            // 
            this.designViewStripMenu.Enabled = false;
            this.designViewStripMenu.Image = ((System.Drawing.Image)(resources.GetObject("designViewStripMenu.Image")));
            this.designViewStripMenu.Name = "designViewStripMenu";
            this.designViewStripMenu.Size = new System.Drawing.Size(124, 22);
            this.designViewStripMenu.Text = "设计界面";
            this.designViewStripMenu.ToolTipText = "设计界面";
            this.designViewStripMenu.Click += new System.EventHandler(this.newViewToolStripMenuItem_Click);
            // 
            // autoSetToolStripMenuItem
            // 
            this.autoSetToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("autoSetToolStripMenuItem.Image")));
            this.autoSetToolStripMenuItem.Name = "autoSetToolStripMenuItem";
            this.autoSetToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.autoSetToolStripMenuItem.Text = "自动保存设置";
            this.autoSetToolStripMenuItem.ToolTipText = "自动保存设置";
            this.autoSetToolStripMenuItem.Click += new System.EventHandler(this.autoSetToolStripMenuItem_Click);
            // 
            // saveForm
            // 
            this.saveForm.Enabled = false;
            this.saveForm.Image = ((System.Drawing.Image)(resources.GetObject("saveForm.Image")));
            this.saveForm.Name = "saveForm";
            this.saveForm.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveForm.Size = new System.Drawing.Size(202, 22);
            this.saveForm.Text = "保存当前页(&S)";
            this.saveForm.ToolTipText = "保存当前页(&S)";
            this.saveForm.Click += new System.EventHandler(this.saveForm_Click);
            this.saveForm.EnabledChanged += new System.EventHandler(this.saveForm_EnabledChanged);
            // 
            // saveProject
            // 
            this.saveProject.Enabled = false;
            this.saveProject.Image = ((System.Drawing.Image)(resources.GetObject("saveProject.Image")));
            this.saveProject.Name = "saveProject";
            this.saveProject.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.saveProject.Size = new System.Drawing.Size(202, 22);
            this.saveProject.Text = "保存工程";
            this.saveProject.ToolTipText = "保存工程";
            this.saveProject.Click += new System.EventHandler(this.saveProject_Click);
            this.saveProject.EnabledChanged += new System.EventHandler(this.saveProject_EnabledChanged);
            // 
            // ResetWorkSpaceToolStripMenuItem
            // 
            this.ResetWorkSpaceToolStripMenuItem.Name = "ResetWorkSpaceToolStripMenuItem";
            this.ResetWorkSpaceToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.ResetWorkSpaceToolStripMenuItem.Text = "工作目录切换";
            this.ResetWorkSpaceToolStripMenuItem.Click += new System.EventHandler(this.ResetWorkSpaceToolStripMenuItem_Click);
            // 
            // closeProjecttoolStripMenuItem
            // 
            this.closeProjecttoolStripMenuItem.Enabled = false;
            this.closeProjecttoolStripMenuItem.Name = "closeProjecttoolStripMenuItem";
            this.closeProjecttoolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.C)));
            this.closeProjecttoolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.closeProjecttoolStripMenuItem.Text = "关闭 ";
            this.closeProjecttoolStripMenuItem.ToolTipText = "关闭 ";
            this.closeProjecttoolStripMenuItem.Click += new System.EventHandler(this.closeProjecttoolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.exitToolStripMenuItem.Text = "退出(&X)";
            this.exitToolStripMenuItem.ToolTipText = "退出(&X)";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undotoolStripMenuItem,
            this.redotoolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteControlToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.aliginToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.editToolStripMenuItem.Text = "编辑";
            // 
            // undotoolStripMenuItem
            // 
            this.undotoolStripMenuItem.Enabled = false;
            this.undotoolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("undotoolStripMenuItem.Image")));
            this.undotoolStripMenuItem.Name = "undotoolStripMenuItem";
            this.undotoolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.undotoolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.undotoolStripMenuItem.Text = "撤消(&U)";
            this.undotoolStripMenuItem.ToolTipText = "撤消(&U)";
            this.undotoolStripMenuItem.Click += new System.EventHandler(this.undotoolStripMenuItem_Click);
            this.undotoolStripMenuItem.EnabledChanged += new System.EventHandler(this.undotoolStripMenuItem_EnabledChanged);
            // 
            // redotoolStripMenuItem
            // 
            this.redotoolStripMenuItem.Enabled = false;
            this.redotoolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redotoolStripMenuItem.Image")));
            this.redotoolStripMenuItem.Name = "redotoolStripMenuItem";
            this.redotoolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.redotoolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.redotoolStripMenuItem.Text = "重复(&R)";
            this.redotoolStripMenuItem.ToolTipText = "重复(&R)";
            this.redotoolStripMenuItem.Click += new System.EventHandler(this.redotoolStripMenuItem_Click);
            this.redotoolStripMenuItem.EnabledChanged += new System.EventHandler(this.redotoolStripMenuItem_EnabledChanged);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Enabled = false;
            this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.cutToolStripMenuItem.Text = "剪切(&T)";
            this.cutToolStripMenuItem.ToolTipText = "剪切(&T)";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.Action_Click);
            this.cutToolStripMenuItem.EnabledChanged += new System.EventHandler(this.cutToolStripMenuItem_EnabledChanged);
            this.cutToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Enabled = false;
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.copyToolStripMenuItem.Text = "复制(&C)";
            this.copyToolStripMenuItem.ToolTipText = "复制(&C)";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.Action_Click);
            this.copyToolStripMenuItem.EnabledChanged += new System.EventHandler(this.copyToolStripMenuItem_EnabledChanged);
            this.copyToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.pasteToolStripMenuItem.Text = "粘贴(&V)";
            this.pasteToolStripMenuItem.ToolTipText = "粘贴(&V)";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.Action_Click);
            this.pasteToolStripMenuItem.EnabledChanged += new System.EventHandler(this.pasteToolStripMenuItem_EnabledChanged);
            this.pasteToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // deleteControlToolStripMenuItem
            // 
            this.deleteControlToolStripMenuItem.Enabled = false;
            this.deleteControlToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteControlToolStripMenuItem.Image")));
            this.deleteControlToolStripMenuItem.Name = "deleteControlToolStripMenuItem";
            this.deleteControlToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteControlToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteControlToolStripMenuItem.Text = "删除(&D)";
            this.deleteControlToolStripMenuItem.ToolTipText = "删除(&D)";
            this.deleteControlToolStripMenuItem.Click += new System.EventHandler(this.Action_Click);
            this.deleteControlToolStripMenuItem.EnabledChanged += new System.EventHandler(this.deleteControlToolStripMenuItem_EnabledChanged);
            this.deleteControlToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Enabled = false;
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.selectAllToolStripMenuItem.Text = "全选(&A)";
            this.selectAllToolStripMenuItem.ToolTipText = "全选(&A)";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.Action_Click);
            this.selectAllToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // aliginToolStripMenuItem
            // 
            this.aliginToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leftsToolStripMenuItem,
            this.middlesToolStripMenuItem,
            this.rightsToolStripMenuItem,
            this.topsToolStripMenuItem,
            this.centersToolStripMenuItem,
            this.bottomsToolStripMenuItem});
            this.aliginToolStripMenuItem.Enabled = false;
            this.aliginToolStripMenuItem.Name = "aliginToolStripMenuItem";
            this.aliginToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.aliginToolStripMenuItem.Text = "视图";
            this.aliginToolStripMenuItem.ToolTipText = "视图";
            this.aliginToolStripMenuItem.Visible = false;
            this.aliginToolStripMenuItem.EnabledChanged += new System.EventHandler(this.aliginToolStripMenuItem_EnabledChanged);
            // 
            // leftsToolStripMenuItem
            // 
            this.leftsToolStripMenuItem.Enabled = false;
            this.leftsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("leftsToolStripMenuItem.Image")));
            this.leftsToolStripMenuItem.Name = "leftsToolStripMenuItem";
            this.leftsToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.leftsToolStripMenuItem.Text = "左对齐";
            this.leftsToolStripMenuItem.ToolTipText = "左对齐";
            this.leftsToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // middlesToolStripMenuItem
            // 
            this.middlesToolStripMenuItem.Enabled = false;
            this.middlesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("middlesToolStripMenuItem.Image")));
            this.middlesToolStripMenuItem.Name = "middlesToolStripMenuItem";
            this.middlesToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.middlesToolStripMenuItem.Text = "居中对齐";
            this.middlesToolStripMenuItem.ToolTipText = "居中对齐";
            this.middlesToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // rightsToolStripMenuItem
            // 
            this.rightsToolStripMenuItem.Enabled = false;
            this.rightsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("rightsToolStripMenuItem.Image")));
            this.rightsToolStripMenuItem.Name = "rightsToolStripMenuItem";
            this.rightsToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.rightsToolStripMenuItem.Text = "右对齐";
            this.rightsToolStripMenuItem.ToolTipText = "右对齐";
            this.rightsToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // topsToolStripMenuItem
            // 
            this.topsToolStripMenuItem.Enabled = false;
            this.topsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("topsToolStripMenuItem.Image")));
            this.topsToolStripMenuItem.Name = "topsToolStripMenuItem";
            this.topsToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.topsToolStripMenuItem.Text = "顶端对齐";
            this.topsToolStripMenuItem.ToolTipText = "顶端对齐";
            this.topsToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // centersToolStripMenuItem
            // 
            this.centersToolStripMenuItem.Enabled = false;
            this.centersToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("centersToolStripMenuItem.Image")));
            this.centersToolStripMenuItem.Name = "centersToolStripMenuItem";
            this.centersToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.centersToolStripMenuItem.Text = "中间对齐";
            this.centersToolStripMenuItem.ToolTipText = "中间对齐";
            this.centersToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // bottomsToolStripMenuItem
            // 
            this.bottomsToolStripMenuItem.Enabled = false;
            this.bottomsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("bottomsToolStripMenuItem.Image")));
            this.bottomsToolStripMenuItem.Name = "bottomsToolStripMenuItem";
            this.bottomsToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.bottomsToolStripMenuItem.Text = "底端对齐";
            this.bottomsToolStripMenuItem.ToolTipText = "底端对齐";
            this.bottomsToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_MouseClick);
            // 
            // toolStripMenuItem_Windows
            // 
            this.toolStripMenuItem_Windows.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Controls,
            this.toolStripMenuItem_Property,
            this.toolStripMenuItem_TreeView,
            this.toolStripMenuItem_PorList,
            this.toolStripMenuItem_ErrorForm,
            this.toolStripMenuItem_EffectPic});
            this.toolStripMenuItem_Windows.Name = "toolStripMenuItem_Windows";
            this.toolStripMenuItem_Windows.Size = new System.Drawing.Size(44, 21);
            this.toolStripMenuItem_Windows.Text = "视图";
            // 
            // toolStripMenuItem_Controls
            // 
            this.toolStripMenuItem_Controls.Checked = true;
            this.toolStripMenuItem_Controls.CheckOnClick = true;
            this.toolStripMenuItem_Controls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem_Controls.Name = "toolStripMenuItem_Controls";
            this.toolStripMenuItem_Controls.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem_Controls.Text = "控制模块库";
            this.toolStripMenuItem_Controls.Click += new System.EventHandler(this.toolStripMenuItem_Controls_Click);
            // 
            // toolStripMenuItem_Property
            // 
            this.toolStripMenuItem_Property.Checked = true;
            this.toolStripMenuItem_Property.CheckOnClick = true;
            this.toolStripMenuItem_Property.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem_Property.Name = "toolStripMenuItem_Property";
            this.toolStripMenuItem_Property.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem_Property.Text = "模块属性";
            this.toolStripMenuItem_Property.Click += new System.EventHandler(this.toolStripMenuItem_Property_Click);
            // 
            // toolStripMenuItem_TreeView
            // 
            this.toolStripMenuItem_TreeView.Checked = true;
            this.toolStripMenuItem_TreeView.CheckOnClick = true;
            this.toolStripMenuItem_TreeView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem_TreeView.Name = "toolStripMenuItem_TreeView";
            this.toolStripMenuItem_TreeView.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem_TreeView.Text = "资源管理器";
            this.toolStripMenuItem_TreeView.Click += new System.EventHandler(this.toolStripMenuItem_TreeView_Click);
            // 
            // toolStripMenuItem_PorList
            // 
            this.toolStripMenuItem_PorList.Checked = true;
            this.toolStripMenuItem_PorList.CheckOnClick = true;
            this.toolStripMenuItem_PorList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem_PorList.Name = "toolStripMenuItem_PorList";
            this.toolStripMenuItem_PorList.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem_PorList.Text = "工程管理器";
            this.toolStripMenuItem_PorList.Click += new System.EventHandler(this.toolStripMenuItem_PorList_Click);
            // 
            // toolStripMenuItem_ErrorForm
            // 
            this.toolStripMenuItem_ErrorForm.Checked = true;
            this.toolStripMenuItem_ErrorForm.CheckOnClick = true;
            this.toolStripMenuItem_ErrorForm.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem_ErrorForm.Name = "toolStripMenuItem_ErrorForm";
            this.toolStripMenuItem_ErrorForm.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem_ErrorForm.Text = "错误列表";
            this.toolStripMenuItem_ErrorForm.Click += new System.EventHandler(this.toolStripMenuItem_ErrorForm_Click);
            // 
            // toolStripMenuItem_EffectPic
            // 
            this.toolStripMenuItem_EffectPic.Checked = true;
            this.toolStripMenuItem_EffectPic.CheckOnClick = true;
            this.toolStripMenuItem_EffectPic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem_EffectPic.Name = "toolStripMenuItem_EffectPic";
            this.toolStripMenuItem_EffectPic.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem_EffectPic.Text = "效果图";
            this.toolStripMenuItem_EffectPic.Click += new System.EventHandler(this.toolStripMenuItem_EffectPic_Click);
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem_ShowNum,
            this.projectToolStripMenuItem_CreateCodelist,
            this.projectToolStripMenuItem_SetAdress,
            this.projectToolStripMenuItem_CompliedMode,
            this.projectToolStripMenuItem_Complied});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.projectToolStripMenuItem.Text = "工程操作";
            // 
            // projectToolStripMenuItem_ShowNum
            // 
            this.projectToolStripMenuItem_ShowNum.Name = "projectToolStripMenuItem_ShowNum";
            this.projectToolStripMenuItem_ShowNum.Size = new System.Drawing.Size(148, 22);
            this.projectToolStripMenuItem_ShowNum.Text = "显示控件序号";
            this.projectToolStripMenuItem_ShowNum.Click += new System.EventHandler(this.projectToolStripMenuItem_ShowNum_Click);
            // 
            // projectToolStripMenuItem_CreateCodelist
            // 
            this.projectToolStripMenuItem_CreateCodelist.Name = "projectToolStripMenuItem_CreateCodelist";
            this.projectToolStripMenuItem_CreateCodelist.Size = new System.Drawing.Size(148, 22);
            this.projectToolStripMenuItem_CreateCodelist.Text = "生成指令表";
            this.projectToolStripMenuItem_CreateCodelist.Click += new System.EventHandler(this.codelistToolStripMenuItem_CreateCodelist_Click);
            // 
            // projectToolStripMenuItem_SetAdress
            // 
            this.projectToolStripMenuItem_SetAdress.Name = "projectToolStripMenuItem_SetAdress";
            this.projectToolStripMenuItem_SetAdress.Size = new System.Drawing.Size(148, 22);
            this.projectToolStripMenuItem_SetAdress.Text = "设置地址";
            this.projectToolStripMenuItem_SetAdress.Click += new System.EventHandler(this.projectToolStripMenuItem_SetAdress_Click);
            // 
            // projectToolStripMenuItem_CompliedMode
            // 
            this.projectToolStripMenuItem_CompliedMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Config,
            this.ToolStripMenuItem_IL,
            this.ToolStripMenuItem_Simulation});
            this.projectToolStripMenuItem_CompliedMode.Name = "projectToolStripMenuItem_CompliedMode";
            this.projectToolStripMenuItem_CompliedMode.Size = new System.Drawing.Size(148, 22);
            this.projectToolStripMenuItem_CompliedMode.Text = "编译模式";
            // 
            // ToolStripMenuItem_Config
            // 
            this.ToolStripMenuItem_Config.Checked = true;
            this.ToolStripMenuItem_Config.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ToolStripMenuItem_Config.Name = "ToolStripMenuItem_Config";
            this.ToolStripMenuItem_Config.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItem_Config.Text = "图形组态编译";
            this.ToolStripMenuItem_Config.Click += new System.EventHandler(this.ToolStripMenuItem_Config_Click);
            // 
            // ToolStripMenuItem_IL
            // 
            this.ToolStripMenuItem_IL.Name = "ToolStripMenuItem_IL";
            this.ToolStripMenuItem_IL.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItem_IL.Text = "指令组态编译";
            this.ToolStripMenuItem_IL.Click += new System.EventHandler(this.ToolStripMenuItem_IL_Click);
            // 
            // ToolStripMenuItem_Simulation
            // 
            this.ToolStripMenuItem_Simulation.Name = "ToolStripMenuItem_Simulation";
            this.ToolStripMenuItem_Simulation.Size = new System.Drawing.Size(148, 22);
            this.ToolStripMenuItem_Simulation.Text = "仿真";
            this.ToolStripMenuItem_Simulation.Click += new System.EventHandler(this.ToolStripMenuItem_Simulation_Click);
            // 
            // projectToolStripMenuItem_Complied
            // 
            this.projectToolStripMenuItem_Complied.Name = "projectToolStripMenuItem_Complied";
            this.projectToolStripMenuItem_Complied.Size = new System.Drawing.Size(148, 22);
            this.projectToolStripMenuItem_Complied.Text = "编译";
            this.projectToolStripMenuItem_Complied.Click += new System.EventHandler(this.projectToolStripMenuItem_Complied_Click);
            // 
            // 工程分类ToolStripMenuItem
            // 
            this.工程分类ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.工件识别ToolStripMenuItem,
            this.数字识别ToolStripMenuItem,
            this.缺陷检测ToolStripMenuItem,
            this.角度检测ToolStripMenuItem,
            this.字符检测ToolStripMenuItem,
            this.尺寸测量ToolStripMenuItem,
            this.颜色比对ToolStripMenuItem,
            this.匹配定位ToolStripMenuItem});
            this.工程分类ToolStripMenuItem.Name = "工程分类ToolStripMenuItem";
            this.工程分类ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.工程分类ToolStripMenuItem.Text = "工程分类";
            // 
            // 工件识别ToolStripMenuItem
            // 
            this.工件识别ToolStripMenuItem.Name = "工件识别ToolStripMenuItem";
            this.工件识别ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.工件识别ToolStripMenuItem.Text = "工件识别";
            this.工件识别ToolStripMenuItem.Click += new System.EventHandler(this.工件识别ToolStripMenuItem_Click);
            // 
            // 数字识别ToolStripMenuItem
            // 
            this.数字识别ToolStripMenuItem.Name = "数字识别ToolStripMenuItem";
            this.数字识别ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.数字识别ToolStripMenuItem.Text = "数字识别";
            this.数字识别ToolStripMenuItem.Click += new System.EventHandler(this.数字识别ToolStripMenuItem_Click);
            // 
            // 缺陷检测ToolStripMenuItem
            // 
            this.缺陷检测ToolStripMenuItem.Name = "缺陷检测ToolStripMenuItem";
            this.缺陷检测ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.缺陷检测ToolStripMenuItem.Text = "缺陷检测";
            this.缺陷检测ToolStripMenuItem.Click += new System.EventHandler(this.缺陷检测ToolStripMenuItem_Click);
            // 
            // 角度检测ToolStripMenuItem
            // 
            this.角度检测ToolStripMenuItem.Name = "角度检测ToolStripMenuItem";
            this.角度检测ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.角度检测ToolStripMenuItem.Text = "角度检测";
            // 
            // 字符检测ToolStripMenuItem
            // 
            this.字符检测ToolStripMenuItem.Name = "字符检测ToolStripMenuItem";
            this.字符检测ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.字符检测ToolStripMenuItem.Text = "字符检测";
            // 
            // 尺寸测量ToolStripMenuItem
            // 
            this.尺寸测量ToolStripMenuItem.Name = "尺寸测量ToolStripMenuItem";
            this.尺寸测量ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.尺寸测量ToolStripMenuItem.Text = "尺寸测量";
            // 
            // 颜色比对ToolStripMenuItem
            // 
            this.颜色比对ToolStripMenuItem.Name = "颜色比对ToolStripMenuItem";
            this.颜色比对ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.颜色比对ToolStripMenuItem.Text = "颜色比对";
            // 
            // 匹配定位ToolStripMenuItem
            // 
            this.匹配定位ToolStripMenuItem.Name = "匹配定位ToolStripMenuItem";
            this.匹配定位ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.匹配定位ToolStripMenuItem.Text = "匹配定位";
            // 
            // 设备选择ToolStripMenuItem
            // 
            this.设备选择ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uSBToolStripMenuItem,
            this.ov7670ToolStripMenuItem});
            this.设备选择ToolStripMenuItem.Name = "设备选择ToolStripMenuItem";
            this.设备选择ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.设备选择ToolStripMenuItem.Text = "设备选择";
            // 
            // uSBToolStripMenuItem
            // 
            this.uSBToolStripMenuItem.Name = "uSBToolStripMenuItem";
            this.uSBToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.uSBToolStripMenuItem.Text = "USB摄像头";
            // 
            // ov7670ToolStripMenuItem
            // 
            this.ov7670ToolStripMenuItem.Name = "ov7670ToolStripMenuItem";
            this.ov7670ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.ov7670ToolStripMenuItem.Text = "ov7670";
            // 
            // 执行ToolStripMenuItem
            // 
            this.执行ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.下载ToolStripMenuItem});
            this.执行ToolStripMenuItem.Name = "执行ToolStripMenuItem";
            this.执行ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.执行ToolStripMenuItem.Text = "执行";
            // 
            // 下载ToolStripMenuItem
            // 
            this.下载ToolStripMenuItem.Name = "下载ToolStripMenuItem";
            this.下载ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.下载ToolStripMenuItem.Text = "下载";
            // 
            // 配置ToolStripMenuItem
            // 
            this.配置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.分辨率配置ToolStripMenuItem});
            this.配置ToolStripMenuItem.Name = "配置ToolStripMenuItem";
            this.配置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.配置ToolStripMenuItem.Text = "配置";
            // 
            // 分辨率配置ToolStripMenuItem
            // 
            this.分辨率配置ToolStripMenuItem.Name = "分辨率配置ToolStripMenuItem";
            this.分辨率配置ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.分辨率配置ToolStripMenuItem.Text = "分辨率配置";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.ControlEditToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.helpToolStripMenuItem.Text = "帮助　";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.aboutToolStripMenuItem.Text = "关于";
            this.aboutToolStripMenuItem.ToolTipText = "关于";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ControlEditToolStripMenuItem
            // 
            this.ControlEditToolStripMenuItem.Name = "ControlEditToolStripMenuItem";
            this.ControlEditToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.ControlEditToolStripMenuItem.Text = "控件编辑";
            this.ControlEditToolStripMenuItem.Visible = false;
            this.ControlEditToolStripMenuItem.Click += new System.EventHandler(this.ControlEditToolStripMenuItem_Click);
            // 
            // toolStripOperation
            // 
            this.toolStripOperation.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.toolStripOperation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.creatProjecttoolStripSplitButton,
            this.addItemtoolStripSplitButton,
            this.toolStripSeparator2,
            this.saveCurrentFormtoolStripButton,
            this.saveProjecttoolStripButton,
            this.toolStripSeparator1,
            this.undotoolStripButton,
            this.redotoolStripButton,
            this.cuttoolStripButton,
            this.copytoolStripButton,
            this.pastetoolStripButton,
            this.toolStripSeparator3,
            this.leftAligntoolStripButton,
            this.middleAligntoolStripButton,
            this.rightAligntoolStripButton,
            this.topAligntoolStripButton,
            this.centerAligntoolStripButton,
            this.buttomsAligntoolStripButton,
            this.toolStripSeparator4,
            this.bringToFrontToolStripButton,
            this.sendToBackStripButton,
            this.linkToolStripButton,
            this.editToolStripButton,
            this.zoomtoolStripButton,
            this.reducetoolStripButton,
            this.toolStripSeparator5,
            this.CompliedtoolStripButton,
            this.ModetoolStripButton,
            this.toolStripSeparator6,
            this.RunToolStripButton,
            this.DebugByStepToolStripButton,
            this.StopDebugToolStripButton});
            this.toolStripOperation.Location = new System.Drawing.Point(0, 25);
            this.toolStripOperation.Name = "toolStripOperation";
            this.toolStripOperation.Size = new System.Drawing.Size(1028, 25);
            this.toolStripOperation.TabIndex = 5;
            this.toolStripOperation.Text = "toolStrip1";
            // 
            // creatProjecttoolStripSplitButton
            // 
            this.creatProjecttoolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.creatProjecttoolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem});
            this.creatProjecttoolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("creatProjecttoolStripSplitButton.Image")));
            this.creatProjecttoolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.creatProjecttoolStripSplitButton.Name = "creatProjecttoolStripSplitButton";
            this.creatProjecttoolStripSplitButton.Size = new System.Drawing.Size(34, 22);
            this.creatProjecttoolStripSplitButton.Text = "新建工程";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProjectToolStripMenuItem.Image")));
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.newProjectToolStripMenuItem.Text = "新建工程　";
            this.newProjectToolStripMenuItem.ToolTipText = "新建工程　";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // addItemtoolStripSplitButton
            // 
            this.addItemtoolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addItemtoolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openProjectToolStripMenuItem,
            this.designItemToolStripMenuItem,
            this.existItemToolStripMenuItem});
            this.addItemtoolStripSplitButton.Enabled = false;
            this.addItemtoolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("addItemtoolStripSplitButton.Image")));
            this.addItemtoolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addItemtoolStripSplitButton.Name = "addItemtoolStripSplitButton";
            this.addItemtoolStripSplitButton.Size = new System.Drawing.Size(34, 22);
            this.addItemtoolStripSplitButton.Text = "添加新项";
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openProjectToolStripMenuItem.Image")));
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.openProjectToolStripMenuItem.Text = "添加文件夹";
            this.openProjectToolStripMenuItem.ToolTipText = "添加文件夹";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // designItemToolStripMenuItem
            // 
            this.designItemToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("designItemToolStripMenuItem.Image")));
            this.designItemToolStripMenuItem.Name = "designItemToolStripMenuItem";
            this.designItemToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.designItemToolStripMenuItem.Text = "添加新项";
            this.designItemToolStripMenuItem.ToolTipText = "添加新项";
            this.designItemToolStripMenuItem.Click += new System.EventHandler(this.designItemToolStripMenuItem_Click);
            // 
            // existItemToolStripMenuItem
            // 
            this.existItemToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("existItemToolStripMenuItem.Image")));
            this.existItemToolStripMenuItem.Name = "existItemToolStripMenuItem";
            this.existItemToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.existItemToolStripMenuItem.Text = "添加现有项　";
            this.existItemToolStripMenuItem.ToolTipText = "添加现有项　";
            this.existItemToolStripMenuItem.Click += new System.EventHandler(this.existItemToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // saveCurrentFormtoolStripButton
            // 
            this.saveCurrentFormtoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveCurrentFormtoolStripButton.Enabled = false;
            this.saveCurrentFormtoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveCurrentFormtoolStripButton.Image")));
            this.saveCurrentFormtoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveCurrentFormtoolStripButton.Name = "saveCurrentFormtoolStripButton";
            this.saveCurrentFormtoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveCurrentFormtoolStripButton.Text = "当前页保存";
            this.saveCurrentFormtoolStripButton.Click += new System.EventHandler(this.saveCurrentFormtoolStripButton_Click);
            // 
            // saveProjecttoolStripButton
            // 
            this.saveProjecttoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveProjecttoolStripButton.Enabled = false;
            this.saveProjecttoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveProjecttoolStripButton.Image")));
            this.saveProjecttoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveProjecttoolStripButton.Name = "saveProjecttoolStripButton";
            this.saveProjecttoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveProjecttoolStripButton.Text = "全部保存";
            this.saveProjecttoolStripButton.Click += new System.EventHandler(this.saveProjecttoolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // undotoolStripButton
            // 
            this.undotoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undotoolStripButton.Enabled = false;
            this.undotoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("undotoolStripButton.Image")));
            this.undotoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undotoolStripButton.Name = "undotoolStripButton";
            this.undotoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.undotoolStripButton.Text = "撤消(&U)";
            this.undotoolStripButton.Click += new System.EventHandler(this.undotoolStripMenuItem_Click);
            // 
            // redotoolStripButton
            // 
            this.redotoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.redotoolStripButton.Enabled = false;
            this.redotoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("redotoolStripButton.Image")));
            this.redotoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redotoolStripButton.Name = "redotoolStripButton";
            this.redotoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.redotoolStripButton.Text = "重复(&R)";
            this.redotoolStripButton.Click += new System.EventHandler(this.redotoolStripMenuItem_Click);
            // 
            // cuttoolStripButton
            // 
            this.cuttoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cuttoolStripButton.Enabled = false;
            this.cuttoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("cuttoolStripButton.Image")));
            this.cuttoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cuttoolStripButton.Name = "cuttoolStripButton";
            this.cuttoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.cuttoolStripButton.Text = "剪切(&T)";
            this.cuttoolStripButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ToolAction_Click);
            // 
            // copytoolStripButton
            // 
            this.copytoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copytoolStripButton.Enabled = false;
            this.copytoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copytoolStripButton.Image")));
            this.copytoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copytoolStripButton.Name = "copytoolStripButton";
            this.copytoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.copytoolStripButton.Text = "复制(&C)";
            this.copytoolStripButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ToolAction_Click);
            // 
            // pastetoolStripButton
            // 
            this.pastetoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pastetoolStripButton.Enabled = false;
            this.pastetoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("pastetoolStripButton.Image")));
            this.pastetoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pastetoolStripButton.Name = "pastetoolStripButton";
            this.pastetoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.pastetoolStripButton.Text = "粘贴(&V)";
            this.pastetoolStripButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ToolAction_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // leftAligntoolStripButton
            // 
            this.leftAligntoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.leftAligntoolStripButton.Enabled = false;
            this.leftAligntoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("leftAligntoolStripButton.Image")));
            this.leftAligntoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.leftAligntoolStripButton.Name = "leftAligntoolStripButton";
            this.leftAligntoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.leftAligntoolStripButton.Text = "左对齐 ";
            this.leftAligntoolStripButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_ToolStripButton_MouseClick);
            // 
            // middleAligntoolStripButton
            // 
            this.middleAligntoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.middleAligntoolStripButton.Enabled = false;
            this.middleAligntoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("middleAligntoolStripButton.Image")));
            this.middleAligntoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.middleAligntoolStripButton.Name = "middleAligntoolStripButton";
            this.middleAligntoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.middleAligntoolStripButton.Text = "居中对齐";
            this.middleAligntoolStripButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_ToolStripButton_MouseClick);
            // 
            // rightAligntoolStripButton
            // 
            this.rightAligntoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rightAligntoolStripButton.Enabled = false;
            this.rightAligntoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("rightAligntoolStripButton.Image")));
            this.rightAligntoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rightAligntoolStripButton.Name = "rightAligntoolStripButton";
            this.rightAligntoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.rightAligntoolStripButton.Text = "右对齐";
            this.rightAligntoolStripButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_ToolStripButton_MouseClick);
            // 
            // topAligntoolStripButton
            // 
            this.topAligntoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.topAligntoolStripButton.Enabled = false;
            this.topAligntoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("topAligntoolStripButton.Image")));
            this.topAligntoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.topAligntoolStripButton.Name = "topAligntoolStripButton";
            this.topAligntoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.topAligntoolStripButton.Text = "顶端对齐";
            this.topAligntoolStripButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_ToolStripButton_MouseClick);
            // 
            // centerAligntoolStripButton
            // 
            this.centerAligntoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.centerAligntoolStripButton.Enabled = false;
            this.centerAligntoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("centerAligntoolStripButton.Image")));
            this.centerAligntoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.centerAligntoolStripButton.Name = "centerAligntoolStripButton";
            this.centerAligntoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.centerAligntoolStripButton.Text = "中间对齐";
            this.centerAligntoolStripButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_ToolStripButton_MouseClick);
            // 
            // buttomsAligntoolStripButton
            // 
            this.buttomsAligntoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttomsAligntoolStripButton.Enabled = false;
            this.buttomsAligntoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("buttomsAligntoolStripButton.Image")));
            this.buttomsAligntoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttomsAligntoolStripButton.Name = "buttomsAligntoolStripButton";
            this.buttomsAligntoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.buttomsAligntoolStripButton.Text = "底端对齐";
            this.buttomsAligntoolStripButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Action_ToolStripButton_MouseClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // bringToFrontToolStripButton
            // 
            this.bringToFrontToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bringToFrontToolStripButton.Enabled = false;
            this.bringToFrontToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("bringToFrontToolStripButton.Image")));
            this.bringToFrontToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bringToFrontToolStripButton.Name = "bringToFrontToolStripButton";
            this.bringToFrontToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.bringToFrontToolStripButton.Text = "置前";
            this.bringToFrontToolStripButton.Click += new System.EventHandler(this.bringToFrontToolStripButton_Click);
            // 
            // sendToBackStripButton
            // 
            this.sendToBackStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sendToBackStripButton.Enabled = false;
            this.sendToBackStripButton.Image = ((System.Drawing.Image)(resources.GetObject("sendToBackStripButton.Image")));
            this.sendToBackStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sendToBackStripButton.Name = "sendToBackStripButton";
            this.sendToBackStripButton.Size = new System.Drawing.Size(23, 22);
            this.sendToBackStripButton.Text = "置后";
            this.sendToBackStripButton.Click += new System.EventHandler(this.sendToBackStripButton_Click);
            // 
            // linkToolStripButton
            // 
            this.linkToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.linkToolStripButton.Enabled = false;
            this.linkToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("linkToolStripButton.Image")));
            this.linkToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.linkToolStripButton.Name = "linkToolStripButton";
            this.linkToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.linkToolStripButton.Text = "编辑控件";
            this.linkToolStripButton.Click += new System.EventHandler(this.linkToolStripButton1_Click);
            // 
            // editToolStripButton
            // 
            this.editToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("editToolStripButton.Image")));
            this.editToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editToolStripButton.Name = "editToolStripButton";
            this.editToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.editToolStripButton.Text = "编辑连线";
            this.editToolStripButton.Click += new System.EventHandler(this.editToolStripButton2_Click);
            // 
            // zoomtoolStripButton
            // 
            this.zoomtoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomtoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomtoolStripButton.Image")));
            this.zoomtoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomtoolStripButton.Name = "zoomtoolStripButton";
            this.zoomtoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.zoomtoolStripButton.Text = "放大";
            this.zoomtoolStripButton.Click += new System.EventHandler(this.zoomtoolStripButton_Click);
            // 
            // reducetoolStripButton
            // 
            this.reducetoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reducetoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("reducetoolStripButton.Image")));
            this.reducetoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reducetoolStripButton.Name = "reducetoolStripButton";
            this.reducetoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.reducetoolStripButton.Text = "缩小";
            this.reducetoolStripButton.Click += new System.EventHandler(this.reducetoolStripButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // CompliedtoolStripButton
            // 
            this.CompliedtoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CompliedtoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CompliedtoolStripButton.Image")));
            this.CompliedtoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CompliedtoolStripButton.Name = "CompliedtoolStripButton";
            this.CompliedtoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.CompliedtoolStripButton.Text = "编译";
            this.CompliedtoolStripButton.Click += new System.EventHandler(this.CompliedtoolStripButton_Click);
            // 
            // ModetoolStripButton
            // 
            this.ModetoolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ModetoolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ModetoolStripButton.Image")));
            this.ModetoolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ModetoolStripButton.Name = "ModetoolStripButton";
            this.ModetoolStripButton.Size = new System.Drawing.Size(23, 22);
            this.ModetoolStripButton.Text = "指令编译模式";
            this.ModetoolStripButton.Click += new System.EventHandler(this.ModetoolStripButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // RunToolStripButton
            // 
            this.RunToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RunToolStripButton.Enabled = false;
            this.RunToolStripButton.Image = global::CaVeGen.Properties.Resources.Run;
            this.RunToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RunToolStripButton.Name = "RunToolStripButton";
            this.RunToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.RunToolStripButton.Text = "RunInLoop";
            this.RunToolStripButton.Click += new System.EventHandler(this.RunToolStripButton_Click);
            // 
            // DebugByStepToolStripButton
            // 
            this.DebugByStepToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DebugByStepToolStripButton.Enabled = false;
            this.DebugByStepToolStripButton.Image = global::CaVeGen.Properties.Resources.step;
            this.DebugByStepToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DebugByStepToolStripButton.Name = "DebugByStepToolStripButton";
            this.DebugByStepToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.DebugByStepToolStripButton.Text = "Step";
            this.DebugByStepToolStripButton.Click += new System.EventHandler(this.DebugByStepToolStripButton_Click);
            // 
            // StopDebugToolStripButton
            // 
            this.StopDebugToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.StopDebugToolStripButton.Enabled = false;
            this.StopDebugToolStripButton.Image = global::CaVeGen.Properties.Resources.stop;
            this.StopDebugToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StopDebugToolStripButton.Name = "StopDebugToolStripButton";
            this.StopDebugToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.StopDebugToolStripButton.Text = "Stop";
            this.StopDebugToolStripButton.Click += new System.EventHandler(this.StopDebugToolStripButton_Click);
            // 
            // timerClock
            // 
            this.timerClock.Interval = 100000;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // controlEditOperation
            // 
            this.controlEditOperation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.redoMenuItem,
            this.cutMenuItem,
            this.copyMenuItem,
            this.pasteMenuItem,
            this.deleteMenuItem,
            this.selectAllMenuItem});
            this.controlEditOperation.Name = "controlEditOperation";
            this.controlEditOperation.Size = new System.Drawing.Size(164, 158);
            // 
            // undoMenuItem
            // 
            this.undoMenuItem.Enabled = false;
            this.undoMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("undoMenuItem.Image")));
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.undoMenuItem.Size = new System.Drawing.Size(163, 22);
            this.undoMenuItem.Text = "撤消(&U)";
            this.undoMenuItem.Click += new System.EventHandler(this.undotoolStripMenuItem_Click);
            // 
            // redoMenuItem
            // 
            this.redoMenuItem.Enabled = false;
            this.redoMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("redoMenuItem.Image")));
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.redoMenuItem.Size = new System.Drawing.Size(163, 22);
            this.redoMenuItem.Text = "重复(&R)";
            this.redoMenuItem.Click += new System.EventHandler(this.redotoolStripMenuItem_Click);
            // 
            // cutMenuItem
            // 
            this.cutMenuItem.Enabled = false;
            this.cutMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutMenuItem.Image")));
            this.cutMenuItem.Name = "cutMenuItem";
            this.cutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.cutMenuItem.Size = new System.Drawing.Size(163, 22);
            this.cutMenuItem.Text = "剪切(&T)";
            this.cutMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.deleteMenuItem_MouseDown);
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Enabled = false;
            this.copyMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyMenuItem.Image")));
            this.copyMenuItem.Name = "copyMenuItem";
            this.copyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyMenuItem.Size = new System.Drawing.Size(163, 22);
            this.copyMenuItem.Text = "复制(&C)";
            this.copyMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.deleteMenuItem_MouseDown);
            // 
            // pasteMenuItem
            // 
            this.pasteMenuItem.Enabled = false;
            this.pasteMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteMenuItem.Image")));
            this.pasteMenuItem.Name = "pasteMenuItem";
            this.pasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteMenuItem.Size = new System.Drawing.Size(163, 22);
            this.pasteMenuItem.Text = "粘贴(&V)";
            this.pasteMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.deleteMenuItem_MouseDown);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Enabled = false;
            this.deleteMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteMenuItem.Image")));
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteMenuItem.Size = new System.Drawing.Size(163, 22);
            this.deleteMenuItem.Text = "删除(&D)";
            this.deleteMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.deleteMenuItem_MouseDown);
            // 
            // selectAllMenuItem
            // 
            this.selectAllMenuItem.Enabled = false;
            this.selectAllMenuItem.Name = "selectAllMenuItem";
            this.selectAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllMenuItem.Size = new System.Drawing.Size(163, 22);
            this.selectAllMenuItem.Text = "全选(&A)";
            this.selectAllMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.deleteMenuItem_MouseDown);
            // 
            // ViewDesign
            // 
            this.ViewDesign.Controls.Add(this.BottomPanel);
            this.ViewDesign.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewDesign.Location = new System.Drawing.Point(0, 50);
            this.ViewDesign.Name = "ViewDesign";
            this.ViewDesign.Size = new System.Drawing.Size(1028, 384);
            this.ViewDesign.TabIndex = 0;
            // 
            // BottomPanel
            // 
            this.BottomPanel.Controls.Add(this.tabControlView);
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BottomPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(1028, 384);
            this.BottomPanel.TabIndex = 3;
            // 
            // tabControlView
            // 
            this.tabControlView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlView.Location = new System.Drawing.Point(0, 0);
            this.tabControlView.Name = "tabControlView";
            this.tabControlView.SelectedIndex = 0;
            this.tabControlView.Size = new System.Drawing.Size(1028, 384);
            this.tabControlView.TabIndex = 0;
            // 
            // solutionTreeView
            // 
            this.solutionTreeView.Dock = System.Windows.Forms.DockStyle.Top;
            this.solutionTreeView.LineColor = System.Drawing.Color.Empty;
            this.solutionTreeView.Location = new System.Drawing.Point(0, 0);
            this.solutionTreeView.Name = "solutionTreeView";
            this.solutionTreeView.Size = new System.Drawing.Size(200, 191);
            this.solutionTreeView.TabIndex = 0;
            this.solutionTreeView.TabStop = false;
            this.solutionTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.SolutionTreeView_AfterLabelEdit);
            this.solutionTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SolutionTreeView_MouseDown);
            // 
            // ProListBox
            // 
            this.ProListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.proName,
            this.proInfomation});
            this.ProListBox.ContextMenuStrip = this.ProjectMenuStrip;
            this.ProListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProListBox.FullRowSelect = true;
            this.ProListBox.Location = new System.Drawing.Point(0, 0);
            this.ProListBox.MultiSelect = false;
            this.ProListBox.Name = "ProListBox";
            this.ProListBox.Size = new System.Drawing.Size(200, 184);
            this.ProListBox.TabIndex = 7;
            this.ProListBox.UseCompatibleStateImageBehavior = false;
            this.ProListBox.View = System.Windows.Forms.View.Details;
            this.ProListBox.DoubleClick += new System.EventHandler(this.ProListBox_DoubleClick);
            // 
            // proName
            // 
            this.proName.Text = "工程名";
            this.proName.Width = 78;
            // 
            // proInfomation
            // 
            this.proInfomation.Text = "工程描述信息";
            this.proInfomation.Width = 118;
            // 
            // ProjectMenuStrip
            // 
            this.ProjectMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenOldStripMenuItem,
            this.AddNewStripMenuItem,
            this.DeleteStripMenuItem,
            this.ModifyStripMenuItem,
            this.OpenNewStripMenuItem});
            this.ProjectMenuStrip.Name = "ProjectMenuStrip";
            this.ProjectMenuStrip.Size = new System.Drawing.Size(101, 114);
            this.ProjectMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ProjectMenuStrip_Opening);
            // 
            // OpenOldStripMenuItem
            // 
            this.OpenOldStripMenuItem.Name = "OpenOldStripMenuItem";
            this.OpenOldStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.OpenOldStripMenuItem.Text = "打开";
            this.OpenOldStripMenuItem.Click += new System.EventHandler(this.OpenOldStripMenuItem_Click);
            // 
            // AddNewStripMenuItem
            // 
            this.AddNewStripMenuItem.Name = "AddNewStripMenuItem";
            this.AddNewStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.AddNewStripMenuItem.Text = "新建";
            this.AddNewStripMenuItem.Click += new System.EventHandler(this.AddNewStripMenuItem_Click);
            // 
            // DeleteStripMenuItem
            // 
            this.DeleteStripMenuItem.Name = "DeleteStripMenuItem";
            this.DeleteStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.DeleteStripMenuItem.Text = "删除";
            this.DeleteStripMenuItem.Click += new System.EventHandler(this.DeleteStripMenuItem_Click);
            // 
            // ModifyStripMenuItem
            // 
            this.ModifyStripMenuItem.Name = "ModifyStripMenuItem";
            this.ModifyStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.ModifyStripMenuItem.Text = "修改";
            this.ModifyStripMenuItem.Click += new System.EventHandler(this.ModifyStripMenuItem_Click);
            // 
            // OpenNewStripMenuItem
            // 
            this.OpenNewStripMenuItem.Name = "OpenNewStripMenuItem";
            this.OpenNewStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.OpenNewStripMenuItem.Text = "添加";
            this.OpenNewStripMenuItem.Click += new System.EventHandler(this.OpenNewStripMenuItem_Click);
            // 
            // backGroundWorker
            // 
            this.backGroundWorker.WorkerReportsProgress = true;
            this.backGroundWorker.WorkerSupportsCancellation = true;
            this.backGroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backGroundWorker_DoWork);
            this.backGroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backGroundWorker_ProgressChanged);
            this.backGroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backGroundWorker_RunWorkerCompleted);
            // 
            // toolboxServiceImpl
            // 
            this.toolboxServiceImpl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolboxServiceImpl.Location = new System.Drawing.Point(0, 0);
            this.toolboxServiceImpl.Name = "toolboxServiceImpl";
            this.toolboxServiceImpl.SelectedCategory = null;
            this.toolboxServiceImpl.Size = new System.Drawing.Size(200, 503);
            this.toolboxServiceImpl.TabIndex = 0;
            // 
            // errorForm
            // 
            this.errorForm.FileName = "";
            this.errorForm.Location = new System.Drawing.Point(21, 249);
            this.errorForm.Name = "errorForm";
            this.errorForm.Size = new System.Drawing.Size(588, 226);
            this.errorForm.TabIndex = 8;
            this.errorForm.LineDoubleClick += new System.EventHandler(this.wrongList_LineDoubleClick);
            // 
            // picWin
            // 
            this.picWin.BackColor = System.Drawing.SystemColors.Control;
            this.picWin.InputPicIndex = null;
            this.picWin.Location = new System.Drawing.Point(0, 0);
            this.picWin.Name = "picWin";
            this.picWin.Size = new System.Drawing.Size(676, 268);
            this.picWin.TabIndex = 0;
            // 
            // controlfilteredPropertyGrid
            // 
            this.controlfilteredPropertyGrid.BrowsableProperties = null;
            this.controlfilteredPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlfilteredPropertyGrid.HelpVisible = false;
            this.controlfilteredPropertyGrid.HiddenAttributes = null;
            this.controlfilteredPropertyGrid.HiddenProperties = null;
            this.controlfilteredPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.controlfilteredPropertyGrid.Name = "controlfilteredPropertyGrid";
            this.controlfilteredPropertyGrid.Size = new System.Drawing.Size(250, 300);
            this.controlfilteredPropertyGrid.TabIndex = 2;
            this.controlfilteredPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.controlfilteredPropertyGrid_PropertyValueChanged);
            this.controlfilteredPropertyGrid.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.controlfilteredPropertyGrid_SelectedGridItemChanged);
            this.controlfilteredPropertyGrid.SelectedObjectsChanged += new System.EventHandler(this.controlfilteredPropertyGrid_SelectedObjectsChanged);
            // 
            // CassViewGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 456);
            this.Controls.Add(this.ViewDesign);
            this.Controls.Add(this.toolStripOperation);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "CassViewGenerator";
            this.Text = "控制策略设计平台";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CassViewGenerator_FormClosing);
            this.treeMenuPage.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStripOperation.ResumeLayout(false);
            this.toolStripOperation.PerformLayout();
            this.controlEditOperation.ResumeLayout(false);
            this.ViewDesign.ResumeLayout(false);
            this.BottomPanel.ResumeLayout(false);
            this.ProjectMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ContextMenuStrip treeMenuPage;
        private ToolStripMenuItem AddNewItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ImageList solutionImageList;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabelCurrentDesign;
        private ToolStripStatusLabel designMousePosition;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem addNewFormToolStripMenuItem;
        private ToolStripMenuItem projectMenuItem;
        private ToolStripMenuItem designViewStripMenu;
        private ToolStripMenuItem saveForm;
        private ToolStripMenuItem saveProject;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem aliginToolStripMenuItem;
        private ToolStripMenuItem leftsToolStripMenuItem;
        private ToolStripMenuItem centersToolStripMenuItem;
        private ToolStripMenuItem rightsToolStripMenuItem;
        private ToolStripMenuItem topsToolStripMenuItem;
        private ToolStripMenuItem middlesToolStripMenuItem;
        private ToolStripMenuItem bottomsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStrip toolStripOperation;
        private ToolStripMenuItem closeProjecttoolStripMenuItem;
        private ToolStripSplitButton creatProjecttoolStripSplitButton;
        private ToolStripMenuItem newProjectToolStripMenuItem;
        private ToolStripSplitButton addItemtoolStripSplitButton;
        private ToolStripMenuItem designItemToolStripMenuItem;
        private ToolStripMenuItem existItemToolStripMenuItem;
        private ToolStripMenuItem newViewToolStripMenuItem;
        private ToolStripMenuItem existDesignToolStripMenuItem;
        private ToolStripButton saveCurrentFormtoolStripButton;
        private ToolStripButton saveProjecttoolStripButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton cuttoolStripButton;
        private ToolStripButton copytoolStripButton;
        private ToolStripButton pastetoolStripButton; //
        private ToolBoxServiceImpl toolboxServiceImpl;
        private Timer timerClock;
        private ToolStripMenuItem openProjectToolStripMenuItem;
        private ToolStripButton leftAligntoolStripButton;
        private ToolStripButton centerAligntoolStripButton;
        private ToolStripButton rightAligntoolStripButton;
        private ToolStripButton topAligntoolStripButton;
        private ToolStripButton middleAligntoolStripButton;
        private ToolStripButton buttomsAligntoolStripButton;
        private ContextMenuStrip controlEditOperation;
        private ToolStripMenuItem cutMenuItem;
        private ToolStripMenuItem copyMenuItem;
        private ToolStripMenuItem pasteMenuItem;
        private ToolStripMenuItem deleteMenuItem;
        private ToolStripMenuItem undotoolStripMenuItem;
        private ToolStripMenuItem redotoolStripMenuItem;
        private ToolStripButton undotoolStripButton;
        private  ToolStripButton redotoolStripButton;
        private ToolStripMenuItem undoMenuItem;
        private ToolStripMenuItem redoMenuItem;
        private ToolStripMenuItem selectAllMenuItem;
        private ToolStripButton sendToBackStripButton;
        private ToolStripButton bringToFrontToolStripButton;
        private ToolStripMenuItem autoSetToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem deleteControlToolStripMenuItem;
        private ToolStripButton linkToolStripButton;
        private ToolStripButton editToolStripButton;
        private Panel ViewDesign;
        private TreeView solutionTreeView;

  
        private CaVeGen.DesignViewFiles.CodeEditor.ErrorViewControl2 errorForm;
        private CaVeGen.DesignViewFiles.PicWin picWin;

        private CaVeGen.DesignViewFiles.FilterProperty.FilteredPropertyGrid controlfilteredPropertyGrid;
        private ToolStripMenuItem toolStripMenuItem_Windows;
        private ToolStripMenuItem toolStripMenuItem_Controls;
        private ToolStripMenuItem toolStripMenuItem_Property;
        private ToolStripMenuItem toolStripMenuItem_TreeView;
        private ToolStripButton zoomtoolStripButton;
        private ToolStripButton reducetoolStripButton;
        private ToolStripMenuItem projectToolStripMenuItem;
        private ToolStripMenuItem projectToolStripMenuItem_CreateCodelist;
        private ToolStripMenuItem projectToolStripMenuItem_SetAdress;
        private ToolStripMenuItem projectToolStripMenuItem_Complied;
        private ToolStripMenuItem toolStripMenuItem_PorList;
        private ListView ProListBox;
        private ColumnHeader proName;
        private ColumnHeader proInfomation;
        private ContextMenuStrip ProjectMenuStrip;
        private ToolStripMenuItem AddNewStripMenuItem;
        private ToolStripMenuItem DeleteStripMenuItem;
        private ToolStripMenuItem OpenOldStripMenuItem;
        private ToolStripMenuItem OpenNewStripMenuItem;
        private ToolStripMenuItem ControlEditToolStripMenuItem;
        private ToolStripMenuItem ModifyStripMenuItem;
        private ToolStripMenuItem projectToolStripMenuItem_ShowNum;
        private ToolStripMenuItem projectToolStripMenuItem_CompliedMode;
        private ToolStripMenuItem ToolStripMenuItem_Config;
        private ToolStripMenuItem ToolStripMenuItem_IL;
        private ToolStripButton CompliedtoolStripButton;
        private ToolStripButton ModetoolStripButton;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripStatusLabel CurrentMode;
        private ToolStripMenuItem toolStripMenuItem_ErrorForm;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem 工程分类ToolStripMenuItem;
        private ToolStripMenuItem 工件识别ToolStripMenuItem;
        private ToolStripMenuItem 缺陷检测ToolStripMenuItem;
        private ToolStripMenuItem 角度检测ToolStripMenuItem;
        private ToolStripMenuItem 字符检测ToolStripMenuItem;
        private ToolStripMenuItem 尺寸测量ToolStripMenuItem;
        private ToolStripMenuItem 颜色比对ToolStripMenuItem;
        private ToolStripMenuItem 匹配定位ToolStripMenuItem;
        private ToolStripMenuItem 设备选择ToolStripMenuItem;
        private ToolStripMenuItem uSBToolStripMenuItem;
        private ToolStripMenuItem ov7670ToolStripMenuItem;
        private ToolStripMenuItem 执行ToolStripMenuItem;
        private ToolStripMenuItem 下载ToolStripMenuItem;
        private ToolStripMenuItem 配置ToolStripMenuItem;
        private ToolStripMenuItem 分辨率配置ToolStripMenuItem;
        private ToolStripMenuItem ToolStripMenuItem_Simulation;
        private ToolStripButton RunToolStripButton;
        private ToolStripButton DebugByStepToolStripButton;
        private Panel BottomPanel;
        private TabControl tabControlView;
        private ToolStripButton StopDebugToolStripButton;
        private ToolStripProgressBar bgw_processbar;
        private BackgroundWorker backGroundWorker;
        private ToolStripMenuItem ResetWorkSpaceToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem_EffectPic;
        private ToolStripMenuItem 数字识别ToolStripMenuItem;
    }
}

