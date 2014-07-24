/**********************************************************************************************
           ** Copyright (C) 2006 CASS 版权所有

           ** 文件名：CassViewGenerator.cs 
           ** 功能描述：

           **          该类为主页面类。在该类中，实现了整个软件的主页面上布局，如设置器、工具栏、菜单栏选项等设置。
 *                 该类是搭建整个软件的核心部分。类中包括对工具栏的加载，对菜单栏中的编辑、文件等触发相应的事件。
 * 
 *                      对于工程文件管理器，对工程的节点进行添加、删除、重命名等操作。对设计器中被选择的控件发生变化时显示当前控件的属性，
 *                 并对Cass系统不需要的属性进行过滤操作，显示所需的属性。

 *                       工具栏是通过添加控件来实现的，而资源管理器是直接进行操作（因为涉及到一些节点的具体操作，如果作为控件可能比较麻烦），
 *                  资源管理器是调用了TreeView控件实现的。
 *                      
 *                       控件属性显示栏通过添加服务与HostDesign.cs类中的FilteredPropertyGrid进行绑定，当前显示的控件的属性的值是否包括动作设置的属性。
 *                 如果有，则在动作设置的ActionObject属性添加相应的值。对控件有绑定的信息，则对输入的绑定和描述进行判断是否唯一，
 *                 即绑定的属性值和描述的属性值是唯一的，但可重复（即不同的控件可绑定到同一个变量上）
 *                      
 *                       对界面正中下方的controllistView控件中若选择图标进行拖动到当前的设计页面，则实现控件在设计页面上的创建过程。
 *                      
 *                      添加了控件叠加的功能，在组织形成新的文件的时候，对需要叠加的控件的背景图片进行重画。
 * 
 *                      添加了消息处理机制，继承了IMessageFilter接口，主要处理选中控件后进行位置的控制等，如按下方向键时的控件的移动和鼠标移动时实时显示鼠标的位置
 *                    
 *  
           ** 作者：吴丹红
 *         ** 创始时间：2006-11-8
*******************************************************************************************/

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using CaVeGen.CommonOperation;
using CaVeGen.DesignViewFiles;
using CaVeGen.DesignViewFiles.CodeEditor;
using CaVeGen.DesignViewFiles.FilterProperty;
using CaVeGen.Help;
using Microsoft.CSharp;
using Crownwood.Magic.Common;
using Crownwood.Magic.Controls;
using Crownwood.Magic.Docking;
using Crownwood.Magic.Menus;
using CodeSense;
using MTabcontrol = Crownwood.Magic.Controls.TabControl;
using MTabPage = Crownwood.Magic.Controls.TabPage;
using CaVeGen.ProjectControl;
using CaVeGen.ProjectForm;
using System.Runtime.CompilerServices;
using System.Threading;
using CaVeGen.MachineVision;
namespace CaVeGen
{
   
    public partial class CassViewGenerator : Form, IMessageFilter
    {
        #region 自定义变量

        private HostDesignManage hostDesignManage = new HostDesignManage();
        static private ArrayList nodeList = new ArrayList();       //记录当前项目中总的设计器个数和所有名称 外部需要更新页面时调用20090617
        private ArrayList tabList = new ArrayList();        //记录当前tabControlView中总共显示的设计器个数和名称
        private string savePath = null;      //记录需要保存的工程项目的路径
        private string saveName = null;     //记录需要保存的工程项目的工程资源文件的名称
        private string originalPath = null;     //重命名时的原路径
        private string originalName = null;     //重命名时的原名
        public static string designerPath = null;       //设计器的路径,即：运行环境下的安装目录(static :其他类需访问 )
        private bool addMenuService = false;        //表示当前未将Undo和Redo的服务,true为已添加
        private bool saveExcepitionFlag = false;        //判断文件保存过程是否发生异常，false为保存时没有异常发生，true为有异常
        private bool editPropertyFlag = false;
        public static bool ShowNumber = false;    //选择显示还是隐藏控件序号的指标
        public static int listViewSelectIndex = -1;     //ListView中按下时的
        public static object[] designSelectedObjects = null;        //当前选中的对象
        public static System.Windows.Forms.TabPage currentTabPage = null;        //当前页面

        public static UndoOperation currentUndo = null;//当前撤销重做对象
        public static bool isUndo = false;//标示符区分当撤销重做操作不记录操作  

        public static string chooseString = "";
        public static Attribute[] functionAttribute = new Attribute[] { new CategoryAttribute(PublicVariable.ControlFuntionName) };

       
        //该结构体作为nodeList的元素，与工程文件管理进行联系，每个tabePage 和一个相对路径（相对于工程文件夹的路径）
        public struct ListObject
        {
            public string pathName;     //路径名称
            public System.Windows.Forms.TabPage tabPage;     //设计器所在的tabPage
            public UndoOperation UndoAndRedo;//撤销重做操作
        }
        //工程的信息
        static public string programPath = AppDomain.CurrentDomain.BaseDirectory + "Configurations";//工程集合目录
        static public string ProjectInfo = "ProjectInfo";//工程描述信息
        static public string ProjectNum = "-1";//序号 
        static public string ProjectName = null;//工程名
        static public string ProjectMode = "View";//工程模式20090701
        static public string CodePageName = "Code.il";//指令页面名称
        static public string MainPageName = "main";//主页面名称
        static public string CodeText = null;//当前工程的指令列表
        static public string SpecialCodeNode = "特殊代码";//XML文件中特殊指令节点名
        private List<string> PnumList = new List<string>();//管理工程序号不重复
        public int addressRWsize = 0;//可读写地址大小
        public List<string[]> addressInfo = new List<string[]>();//地址信息
        public List<string[]> IOlist = new List<string[]>();//指令表
        public List<string> tempValue = new List<string>();//全局临时变量

        //路径 20140217
        public static string ProgramPath = AppDomain.CurrentDomain.BaseDirectory;  //编程目录
        public static string WorkSpacePath = PublicVariable.DefaultWorkSpacePath;    //工作目录(暂时默认在编程目录下的)
      //  public string currentProjectPath = null;     //当前工程目录
        public static string currentProjectPath = null;  //当前工程目录  2013.11.22
        private bool startCountPoint = false;
      
        private DockingManager dockManager;//悬浮框容器管理器

        private Content tabSolution_Controls = null;//控件库容器
        private Content tabSolution_Property = null;//属性设计容器
        //private Content tabSolution_TreeView = null;//资源管理器容器

        //private Content tabSolution_ListBox = null;//工程管理器容器
        //private Content tabSolution_ErrorBox = null; //错误列表容器

        //private Content tabSolution_PicWin = null; //效果图显示容器

        private Content rightTopContent_TabControl = null;   //右上方容器，存放的是TabControl
        private MTabcontrol rightTopTabList = null;   //右上方的TabControl,存放MTabPage
        private MTabPage tab_ProjectManager = null;   //工程管理器
        private MTabPage tab_SolutionManager = null;  //资源管理器

        private Content bottomContent_TabControl = null; //正下方容器，存放的是bottomTabList
        private MTabPage tab_EffectPicManager = null; //效果图
        private MTabPage tab_ErrorFormManager = null;//错误列表
        private MTabcontrol bottomTabList = null;   //底部的TabControl，存放MTabPage

        private CompileMode compileMode = CompileMode.ConfigMode;  //判断是否是仿真模式2013.11.22
     //   private bool isComplySuccess = false;   //判断是否编译成功2014.1.10
        private bool isDebugByStep = false;  //标记是否是单步调试状态2014.1.11
        private bool isRunInLoop = false; //标记是否循环调试 2014.1.14
        private bool isStopDebug = false; //标记是否停止debug  2014.1.15

        private List<string> CurControlsNum = new List<string>();  //2014.4.19    当前选择执行的流程图
     
        #endregion

        /// <summary>
        /// CassViewGenerator类构造函数
        /// </summary>
        public CassViewGenerator()
        {
            InitializeComponent();
            InitWindow();
            AddDockingManagers();
            OpenProjects();
            ModeSelect(CompileMode.ConfigMode);
        }

        #region 定时保存工程、添加设计器服务等初始化操作

        /// <summary>
        /// 为设计器添加可停靠窗口
        /// </summary>
        private void AddDockingManagers()
        {
            dockManager = new DockingManager(this, VisualStyle.IDE);
            //定义对象OuterControl，Docking Manager不会关注该对象以后生成的对象的窗口区域
            //对象InnerControl，Docking Manager不会关注在该对象生成以前的对象的窗口区域
            dockManager.OuterControl = this.toolStripOperation;
            dockManager.InnerControl = this.ViewDesign;

            //添加可隐藏的控件模块库窗口
            this.tabSolution_Controls = new Content(this.dockManager);
            this.tabSolution_Controls.Control = this.toolboxServiceImpl;
            this.tabSolution_Controls.FullTitle = "控制模块库";
            this.tabSolution_Controls.Title = "控制模块库";
            this.tabSolution_Controls.AutoHideSize = this.toolboxServiceImpl.Size;
            this.tabSolution_Controls.DisplaySize = this.toolboxServiceImpl.Size;
            dockManager.Contents.Add(this.tabSolution_Controls);
            dockManager.AddContentWithState(this.tabSolution_Controls, State.DockLeft);


            //添加可隐藏的属性设计窗口
            this.tabSolution_Property = new Content(this.dockManager);
            this.tabSolution_Property.Control = this.controlfilteredPropertyGrid;
            this.tabSolution_Property.FullTitle = "模块属性";
            this.tabSolution_Property.Title = "模块属性";
            this.tabSolution_Property.AutoHideSize = this.controlfilteredPropertyGrid.Size;
            this.tabSolution_Property.DisplaySize = this.controlfilteredPropertyGrid.Size;
            this.tabSolution_Property.DisplayLocation =  new Point(this.Location.X + this.Width - 200, this.Location.Y + 100);
            this.tabSolution_Property.FloatingSize = this.controlfilteredPropertyGrid.Size;
            dockManager.Contents.Add(this.tabSolution_Property);
       //     dockManager.AddContentWithState(this.tabSolution_Property, State.Floating);
         
            
       //     WindowContent wc = dockManager.AddContentWithState(zone, State.DockRight);
      //      dockManager.AddContentToWindowContent(this.tabSolution_Property, wc);
         

            //添加可隐藏的资源管理器窗口
            //this.tabSolution_TreeView = new Content(this.dockManager);
            //this.tabSolution_TreeView.Control = this.solutionTreeView;
            //this.tabSolution_TreeView.FullTitle = "资源管理器";
            //this.tabSolution_TreeView.Title = "资源管理器";
            //this.tabSolution_TreeView.AutoHideSize = this.solutionTreeView.Size;
            //this.tabSolution_TreeView.DisplaySize = this.solutionTreeView.Size;
            
            //添加可隐藏的工程管理器窗口
            //this.tabSolution_ListBox = new Content(dockManager);
            //this.tabSolution_ListBox.Control = this.ProListBox;
            //this.tabSolution_ListBox.FullTitle = "工程管理器";
            //this.tabSolution_ListBox.Title = "工程管理器";
            //this.tabSolution_ListBox.AutoHideSize = this.ProListBox.Size;
            //this.tabSolution_ListBox.DisplaySize = this.ProListBox.Size;

            //Zone zone = dockManager.CreateZoneForContent(State.DockRight);
            //dockManager.ReorderZoneToInnerMost(zone);
            //dockManager.Contents.Add(this.tabSolution_ListBox);
            //dockManager.AddContentToZone(this.tabSolution_ListBox, zone, 0);
            //dockManager.Contents.Add(this.tabSolution_TreeView);
            //dockManager.AddContentToZone(this.tabSolution_TreeView, zone, 1);


            //2013.12.20  添加
            #region   右上方容器配置

            tab_ProjectManager = new MTabPage();
            tab_ProjectManager.Title = "工程管理器";
            tab_ProjectManager.Name = "tab_ProjectManager";
            tab_ProjectManager.Dock = DockStyle.Fill;
            tab_ProjectManager.Control = this.ProListBox;
       //    tab_ProjectManager.Size = this.ProListBox.Size;
            

            tab_SolutionManager = new MTabPage();
            tab_SolutionManager.Title = "资源管理器";
            tab_SolutionManager.Name = "tab_SolutionManager";
            tab_SolutionManager.Dock = DockStyle.Fill;
            tab_SolutionManager.Control = this.solutionTreeView;
        //  tab_SolutionManager.Size = this.solutionTreeView.Size;

            rightTopTabList = new MTabcontrol();
            rightTopTabList.Dock = DockStyle.Fill;
            rightTopTabList.Name = "rightTopTabList";
            rightTopTabList.TabPages.Add(tab_ProjectManager);
            rightTopTabList.TabPages.Add(tab_SolutionManager);
            rightTopTabList.Width = 200;
         
            this.rightTopContent_TabControl = new Content(this.dockManager);
            this.rightTopContent_TabControl.Control = rightTopTabList;
            this.rightTopContent_TabControl.FullTitle = "解决方案资源管理器";
            this.rightTopContent_TabControl.Title = "解决方案资源管理器";
            this.rightTopContent_TabControl.AutoHideSize = rightTopTabList.Size;
            this.rightTopContent_TabControl.DisplaySize = rightTopTabList.Size;

            Zone myZone = dockManager.CreateZoneForContent(State.DockRight);
            dockManager.ReorderZoneToInnerMost(myZone);
            dockManager.Contents.Add(this.rightTopContent_TabControl);
             dockManager.AddContentToZone(this.rightTopContent_TabControl,myZone,0);
            dockManager.Contents.Add(this.tabSolution_Property);
            dockManager.AddContentToZone(this.tabSolution_Property,myZone, 1);

            #endregion

            //添加可隐藏的错误列表窗口
            //this.tabSolution_ErrorBox = new Content(this.dockManager);
            //this.tabSolution_ErrorBox.Control = this.errorForm;
            //this.tabSolution_ErrorBox.FullTitle = "错误列表";
            //this.tabSolution_ErrorBox.Title = "错误列表";
            //this.tabSolution_ErrorBox.AutoHideSize = this.errorForm.Size;
            //this.tabSolution_ErrorBox.DisplaySize = this.errorForm.Size;
            //this.dockManager.Contents.Add(this.tabSolution_ErrorBox);
            //this.dockManager.AddContentWithState(this.tabSolution_ErrorBox, State.DockBottom);

            
            //2014.1.9
            #region 正下方容器配置

            //错误列表
            tab_ErrorFormManager = new MTabPage();
            tab_ErrorFormManager.Title = "错误列表";
            tab_ErrorFormManager.Name = "tab_ErrorFormManager";
            tab_ErrorFormManager.Dock = DockStyle.Fill;
            tab_ErrorFormManager.Control = this.errorForm;
            tab_ErrorFormManager.TabIndex = 1;

            //效果图
            tab_EffectPicManager = new MTabPage();
            tab_EffectPicManager.Title = "效果图";
            tab_EffectPicManager.Name = "tab_EffectPicManager";
            tab_EffectPicManager.Dock = DockStyle.Fill;
            tab_EffectPicManager.Control = this.picWin;
            tab_EffectPicManager.Size = this.picWin.Size;
            tab_EffectPicManager.TabIndex = 2;

            bottomTabList = new MTabcontrol();
            bottomTabList.Dock = DockStyle.Fill;
            bottomTabList.Name = "bottomTabList";
            bottomTabList.TabPages.Add(tab_ErrorFormManager);
            bottomTabList.TabPages.Add(tab_EffectPicManager);
            bottomTabList.Height = 280;

            this.bottomContent_TabControl = new Content(this.dockManager);
            this.bottomContent_TabControl.Control = bottomTabList;
            this.bottomContent_TabControl.FullTitle = "结果输出";
            this.bottomContent_TabControl.Title = "结果输出";
            this.bottomContent_TabControl.AutoHideSize = bottomTabList.Size;
            this.bottomContent_TabControl.DisplaySize = bottomTabList.Size;

            dockManager.Contents.Add(this.bottomContent_TabControl);
            dockManager.AddContentWithState(this.bottomContent_TabControl, State.DockBottom);
           
            #endregion

            //为隐藏窗口添加关闭事件处理函数，当隐藏窗口关闭时设置响应窗口状态栏的状态
            this.dockManager.ContentHidden +=
                new DockingManager.ContentHandler(dockingManager_Controls_ContentHidden);
        }


        /// <summary>
        /// 悬浮拖拉窗口关闭事件处理函数
        /// </summary>
        /// <param name="c">事件发送者</param>
        /// <param name="cea">事件数据</param>
        void dockingManager_Controls_ContentHidden(Content c, EventArgs cea)
        {
            switch (c.FullTitle)
            {
                case "控制模块库":
                    {
                        this.toolStripMenuItem_Controls.Checked = false;
                        break;
                    }
                case "模块属性":
                    {
                        this.toolStripMenuItem_Property.Checked = false;
                        break;
                    }
                case "资源管理器":
                    {
                        this.toolStripMenuItem_TreeView.Checked = false;
                        break;
                    }
                case "工程管理器":
                    {
                        this.toolStripMenuItem_PorList.Checked = false;
                        break;
                    }
                case "错误列表":
                    {
                        this.toolStripMenuItem_ErrorForm.Checked = false;
                        break;
                    }
                case"效果图":
                    {
                        this.toolStripMenuItem_EffectPic.Checked = false;
                        break;

                    }
            }
        }

        /// <summary>
        /// 对主页面进行初始化操作
        /// 比如状态栏的初始化设置、从安装目录下的TimerSetParament.inf文件中读取工程设计页面自动保存的设置，默认的时间设置为10分钟。
        /// </summary>
        private void InitWindow()
        {
            string[] line = new string[2];     //获取读取的定时器配置文件内容
            bool errorFlag = false;            //表示当前程序是否出现异常
            FileStream fStream = null;
            StreamReader sReader = null;

            //添加消息处理
            Application.AddMessageFilter(this);
            Clipboard.Clear(); // 清空剪切板内容

            setEditEnable(false);
            saveForm.Enabled = false;
            saveCurrentFormtoolStripButton.Enabled = false;
            saveProject.Enabled = false;
            saveProjecttoolStripButton.Enabled = false;
            closeProjecttoolStripMenuItem.Enabled = false;
            designViewStripMenu.Enabled = false;
            //2014.1.10
            this.ChangeDebugRunToolStripEnable(false);
            this.StopDebugToolStripButton.Enabled = false;
            solutionTreeView.Nodes.Clear();

            try
            {
                designerPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

                AddPCControls();  //添加图元工具拦

                //从配置文件TimerSetParament中读取自动保存的时间设置
                fStream = new FileStream(Path.Combine(designerPath, PublicVariable.TimeSetFileName), FileMode.Open);
                sReader = new StreamReader(fStream);
                line[0] = sReader.ReadLine();
                line[1] = sReader.ReadLine();

                //对定时器进行设置
                if (line[0].ToLower().Equals("false"))
                {
                    timerClock.Enabled = false;
                }
                else
                {
                    timerClock.Enabled = true;
                }
                //对定时器的时间进行设置
                if (Convert.ToInt16(line[1]) > 0 && Convert.ToInt16(line[1]) < 60)
                {
                    timerClock.Interval = Convert.ToInt16(line[1]) * 1000 * 60;
                }
                else //默认为10分钟
                {
                    timerClock.Interval = 10 * 1000 * 60;
                }
                sReader.Close();
                fStream.Close();
            }
            catch (FileNotFoundException e)
            {
                errorFlag = true;
                //CassMessageBox.Error("安装文件被损坏！");
            }
            catch (DirectoryNotFoundException e)
            {
                errorFlag = true;
                //CassMessageBox.Error("安装文件被损坏！");
            }
            catch (SecurityException e)
            {
                errorFlag = true;
                //CassMessageBox.Error("文件权限被修改！");
            }
            catch (OutOfMemoryException e)      //没有足够的内存继续执行程序时引发的异常。
            {
                errorFlag = true;
                //CassMessageBox.Error("内存溢出！");
            }
            catch (PathTooLongException e)
            {
                //CassMessageBox.Error("文件路径过长！");
            }
            catch (UnauthorizedAccessException e)
            {
                //CassMessageBox.Error("获取参数时发生异常！");
            }
            catch (Exception ex)
            {
                string a = ex.Message + ex.StackTrace;
                //errorFlag = true;
            }
            finally
            {
                if (errorFlag == true)
                {
                    timerClock.Interval = 10 * 1000 * 60;
                    timerClock.Enabled = true;
                }
                if (sReader != null)
                {
                    sReader.Dispose();
                }
                if (fStream != null)
                {
                    fStream.Dispose();
                }
            }
        }

        /// <summary>
        /// 对主页面上的工具栏进行加载操作 ，添加图元工具栏具体内容
        /// </summary>
        private void AddPCControls()
        {
            //ConfigurationSettings: 提供运行时对读取配置节和公共配置设置的支持。
            toolboxServiceImpl.LoadToolBox();
            hostDesignManage.AddService(typeof(IToolboxService), toolboxServiceImpl);
            hostDesignManage.AddService(typeof(FilteredPropertyGrid), controlfilteredPropertyGrid);
            hostDesignManage.AddService(typeof(ContextMenuStrip), controlEditOperation);
            hostDesignManage.AddService(typeof(ToolStripStatusLabel), designMousePosition);
            hostDesignManage.AddService(typeof(ToolStripMenuItem), editToolStripMenuItem);
        }

        /// <summary>
        /// 获得文件的字符流
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>        
        private byte[] loadFile(string filename)
        {
            FileStream fs = null;
            byte[] buffer = null;

            try
            {
                FileOnlyRead(filename);
                fs = new FileStream(filename, FileMode.Open);
                buffer = new byte[(int)fs.Length];
                fs.Read(buffer, 0, buffer.Length);

                fs.Close();
            }
            catch (ArgumentException e)
            {
                CassMessageBox.Error("无效路径！");
            }
            catch (SecurityException e)
            {
                CassMessageBox.Error("没有使用权限，可能文件正在被其他进程使用！");
            }
            catch (FileNotFoundException e)
            {
                CassMessageBox.Error("找不到指定的文件！");
            }
            catch (DirectoryNotFoundException e)
            {
                CassMessageBox.Error("文件路径异常！");
            }
            catch (IOException e)
            {
                CassMessageBox.Error("文件操作异常！");
            }
            catch (Exception ex)
            {
                CassMessageBox.Error("文件操作异常！");
            }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
            }
            return buffer;
        }

        /// <summary>
        /// 如果文件为只读,则修改文件属性
        /// </summary>
        /// <param name="filename"></param>
        private bool FileOnlyRead(string filename)
        {
            if (File.Exists(filename))
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Attributes.ToString().ToLower().IndexOf("readonly") != -1)  //将只读属性设为正常操作
                {
                    fi.Attributes = FileAttributes.Normal;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 撤销、剪贴、拷贝、粘贴等编辑操作

        /// <summary>
        /// 撤销操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undotoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.CurrentDocumentsHostControl.HostDesign == null)   //获取当前的设计页面
            {
                return;
            }
            //IMenuCommandService menuCommandService =
            //    this.CurrentDocumentsHostControl.HostDesign.GetService(typeof(IMenuCommandService)) as IMenuCommandService;

            //if (addMenuService == false)
            //{
            //    System.ComponentModel.Design.MenuCommand undoCommand =
            //        new System.ComponentModel.Design.MenuCommand(new EventHandler(ExecuteUndo), StandardCommands.Undo);
            //    menuCommandService.AddCommand(undoCommand);

            //    System.ComponentModel.Design.MenuCommand redoCommand =
            //        new System.ComponentModel.Design.MenuCommand(new EventHandler(ExecuteRedo), StandardCommands.Redo);
            //    menuCommandService.AddCommand(redoCommand);

            //    addMenuService = true;
            //}
            //menuCommandService.GlobalInvoke(StandardCommands.Undo);
            if (currentUndo != null)
            {
                isUndo = true;
                currentUndo.Undo();
                undotoolStripMenuItem.Enabled = currentUndo.canUndo;
                undotoolStripButton.Enabled = currentUndo.canUndo;
                isUndo = false;
                redotoolStripMenuItem.Enabled = true;
                redotoolStripButton.Enabled = true;
                controlfilteredPropertyGrid.Refresh();//属性修改时刷新
            }
        }

        /// <summary>
        /// 在设计器容器中添加撤消工作的服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteUndo(object sender, EventArgs e)
        {
            //UndoEngineImplication undoEngine =
            //    CurrentDocumentsHostControl.HostDesign.GetService(typeof(UndoEngineImplication)) as UndoEngineImplication;
            //if (undoEngine != null)
            //{
            //    undoEngine.DoUndo();
            //}
        }

        /// <summary>
        /// 重复操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redotoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentUndo != null)
            {
                isUndo = true;
                currentUndo.Redo();
                redotoolStripMenuItem.Enabled = currentUndo.canRedo;
                redotoolStripButton.Enabled = currentUndo.canRedo;
                isUndo = false;
                undotoolStripMenuItem.Enabled = true;
                undotoolStripButton.Enabled = true;
                controlfilteredPropertyGrid.Refresh();//属性修改时刷新
            }
            //if (CurrentDocumentsHostControl != null
            //    && CurrentDocumentsHostControl.HostDesign != null)
            //{
            //    IMenuCommandService menuCommandService =
            //        this.CurrentDocumentsHostControl.HostDesign.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            //    menuCommandService.GlobalInvoke(StandardCommands.Redo);
            //}
        }

        /// <summary>
        /// 重复操作的执行函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteRedo(object sender, EventArgs e)
        {
            //if (CurrentDocumentsHostControl != null
            //    && CurrentDocumentsHostControl.HostDesign != null)
            //{
            //    UndoEngineImplication redoOperation =
            //        CurrentDocumentsHostControl.HostDesign.GetService(typeof(UndoEngineImplication)) as UndoEngineImplication;
            //    if (redoOperation != null)
            //    {
            //        redoOperation.DoRedo();
            //    }
            //}
        }

        /// <summary>
        /// 设计界面鼠标右键菜单鼠标按下事件处理函数，左鼎2008年3月4日修改
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void deleteMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (CurrentDocumentsHostControl != null
                && CurrentDocumentsHostControl.HostDesign != null)
            {
                PerformAction((sender as ToolStripMenuItem).Text);
            }
            this.controlEditOperation.Visible = false;
        }

        /// <summary>
        /// 根据参数的名称，对主页面菜单上的编辑项的名称进行相应的操作。
        /// 如:剪贴、复制、粘贴等操作
        /// </summary>
        /// <param name="text">所触发的菜单的文本名</param>
        private void PerformAction(string text)
        {
            if (this.CurrentDocumentsHostControl == null)
            {
                return;
            }
            IMenuCommandService menuCommandService =
                this.CurrentDocumentsHostControl.HostDesign.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            List<object> newCombineControl = new List<object>();
            try
            {
                StartUndo();//启动撤销
                switch (text.Trim())
                {

                    case "剪切(&T)":
                        //判断当前剪切的控件中是否包括叠加信息

                        if (this.CurrentDocumentsHostControl.HostDesign.SelectedObjects != null
                            && this.CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count > 0)
                        {
                            CopyCtrlinfo();//赋值选定的控件信息
                            object[] deleteSelectedObject = new object[this.CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count];
                            this.CurrentDocumentsHostControl.HostDesign.SelectedObjects.CopyTo(deleteSelectedObject, 0);
                        }
                        if (CassViewGenerator.currentUndo != null && !CassViewGenerator.isUndo)
                        {//控件删除操作
                            GetDelctrl();
                        }
                        menuCommandService.GlobalInvoke(StandardCommands.Cut);
                        //如果剪切以后的控件为空，则禁止编辑操作,tabControlView.SelectedTab.Controls[0]为HostControl
                        if (tabControlView.SelectedTab.Controls[0].GetNextControl(
                            tabControlView.SelectedTab.Controls[0], false).Controls.Count == 0)
                        {
                            cutToolStripMenuItem.Enabled = false;
                            pasteToolStripMenuItem.Enabled = false;
                            copyToolStripMenuItem.Enabled = false;
                            deleteControlToolStripMenuItem.Enabled = false;
                            aliginToolStripMenuItem.Enabled = false;
                            selectAllToolStripMenuItem.Enabled = false;
                        }
                        pasteToolStripMenuItem.Enabled = true;
                        break;

                    case "复制(&C)":
                        CopyCtrlinfo();//赋值选定的控件信息
                        menuCommandService.GlobalInvoke(StandardCommands.Copy);
                        pasteToolStripMenuItem.Enabled = true;
                        break;

                    case "粘贴(&V)":
                        if (this.CurrentDocumentsHostControl.HostDesign != null
                            && this.CurrentDocumentsHostControl.HostDesign.rootControl != null)
                        {
                            this.CurrentDocumentsHostControl.HostDesign.rootControl.Refresh();
                        }
                        menuCommandService.GlobalInvoke(StandardCommands.Paste);
                        break;

                    case "删除(&D)":
                        //判断当前删除的对象的绑定属性是否存在，若存在，删除之前先将队列中的内容删除 
                        //tempControl:表示当前选中控件相对于工程的路径

                        if (this.CurrentDocumentsHostControl.HostDesign.SelectedObjects != null
                            && this.CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count > 0)
                        {
                            object[] deleteSelectedObject =
                                new object[this.CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count];
                            this.CurrentDocumentsHostControl.HostDesign.SelectedObjects.CopyTo(deleteSelectedObject, 0);
                        }

                        if (CassViewGenerator.currentUndo != null && !CassViewGenerator.isUndo)
                        {//控件删除操作
                            GetDelctrl();
                        }

                        menuCommandService.GlobalInvoke(StandardCommands.Delete);
                        //如果删除以后的控件为空，则禁止编辑操作,tabControlView.SelectedTab.Controls[0]为HostControl
                        if (tabControlView.SelectedTab.Controls[0].GetNextControl(
                            tabControlView.SelectedTab.Controls[0], false).Controls.Count == 0)
                        {
                            cutToolStripMenuItem.Enabled = false;
                            pasteToolStripMenuItem.Enabled = false;
                            copyToolStripMenuItem.Enabled = false;
                            deleteControlToolStripMenuItem.Enabled = false;
                            aliginToolStripMenuItem.Enabled = false;
                            selectAllToolStripMenuItem.Enabled = false;
                        }
                        break;

                    case "全选(&A)":
                        menuCommandService.GlobalInvoke(StandardCommands.SelectAll);
                        break;

                    case "左对齐":
                        menuCommandService.GlobalInvoke(StandardCommands.AlignLeft);
                        break;

                    case "中间对齐":
                        menuCommandService.GlobalInvoke(StandardCommands.AlignHorizontalCenters);
                        break;

                    case "右对齐":
                        menuCommandService.GlobalInvoke(StandardCommands.AlignRight);
                        break;

                    case "顶端对齐":
                        menuCommandService.GlobalInvoke(StandardCommands.AlignTop);
                        break;

                    case "居中对齐":
                        menuCommandService.GlobalInvoke(StandardCommands.AlignVerticalCenters);
                        break;

                    case "底端对齐":
                        menuCommandService.GlobalInvoke(StandardCommands.AlignBottom);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CassMessageBox.Warning("操作异常！");
            }
        }

        /// <summary>
        /// 复制剪切时对相应的控件信息进行保存
        /// 至对应页面的CASSVIEW的COPYctrl中
        /// 为粘贴时调用
        /// </summary>
        private void CopyCtrlinfo()
        {
            CassView curCas = (CassView)(currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false));
            curCas.CopyCtrl.Clear();
            foreach (Control element in this.CurrentDocumentsHostControl.HostDesign.SelectedObjects)
            {
                curCas.CopyCtrl.Add(curCas.FindControlInfo(element));
            }
        }

        /// <summary>
        /// 对主页面上菜单上的编辑选项中的单击事件所触发的函数
        /// </summary>
        /// <param name="sender">操作对象 </param>
        /// <param name="e">触发的事件</param>
        private void Action_Click(object sender, EventArgs e)
        {
            if (CurrentDocumentsHostControl != null
                && CurrentDocumentsHostControl.HostDesign != null)
            {
                IDesignerHost host = (IDesignerHost)this.CurrentDocumentsHostControl.HostDesign.GetService(typeof(IDesignerHost));
                if (host != null && host.RootComponent != null
                    && tabControlView.SelectedTab != null
                    && tabControlView.SelectedTab.Controls.Count > 0)
                {
                    Point clickPosition = new Point(-1, -1);
                    clickPosition = tabControlView.SelectedTab.Controls[0].PointToClient(MousePosition);
                    if (clickPosition.X > ((Control)host.RootComponent).Bounds.X
                        && clickPosition.Y > ((Control)host.RootComponent).Bounds.Y
                        && clickPosition.X < ((Control)host.RootComponent).Bounds.X
                        + ((Control)host.RootComponent).Bounds.Width
                        && clickPosition.Y < ((Control)host.RootComponent).Bounds.Y
                        + ((Control)host.RootComponent).Bounds.Height)
                    {
                        //响应鼠标选择的菜单项
                        PerformAction((sender as ToolStripMenuItem).Text);
                    }
                }
            }
        }

        /// <summary>
        /// 菜单栏按钮鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Action_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentDocumentsHostControl != null
                && CurrentDocumentsHostControl.HostDesign != null)
            {
                PerformAction((sender as ToolStripMenuItem).Text);
            }
        }

        /// <summary>
        /// 工具栏按钮鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Action_ToolStripButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentDocumentsHostControl != null
                && CurrentDocumentsHostControl.HostDesign != null)
            {
                PerformAction((sender as ToolStripButton).Text);
            }
        }

        /// <summary>
        /// 将操作添加至撤销重做对象中
        /// </summary>
        /// <param name="Opt"></param>
        public static void AddOpt(Operation Opt)
        {
            CassViewGenerator.currentUndo.AddOperate(Opt); 
        }

        /// <summary>
        /// 操作后启动撤销功能
        /// </summary>
        private void StartUndo()
        {
            //启动撤销
            undotoolStripMenuItem.Enabled = currentUndo.canUndo;
            undotoolStripButton.Enabled = currentUndo.canUndo;

            //重做变灰
            redotoolStripMenuItem.Enabled = currentUndo.canRedo;
            redotoolStripButton.Enabled = currentUndo.canRedo;
        }

        /// <summary>
        /// 控件删除剪切操作时获取该操作控件相关信息
        /// </summary>
        private void GetDelctrl()
        {
            foreach (Control element in (object[])this.CurrentDocumentsHostControl.HostDesign.SelectedObjects.SyncRoot)
            {
                Operation ctrlOpt = new Operation();
                ctrlOpt.Oname = UndoOperation.OperateStyle.控件删除.ToString();
                ctrlOpt.Item = element;
                ctrlOpt.Change = new object[2];
                ctrlOpt.Change[0] = this.CurrentDocumentsHostControl.HostDesign;
                ctrlOpt.Change[1] = element.Site.Name;

                CassViewGenerator.AddOpt(ctrlOpt);
            }
        }

        #endregion

        #region tabViewControl中选中TabPage和双击关闭当前TabPage的操作

        /// <summary>
        /// 当tabControlView控件中的选项卡发生变化时 ，保存单个页面中的页面名称作相应的改变
        /// </summary>
        /// <param name="sender">触发的对象 </param>
        /// <param name="e">触发的事件 </param>
        private void tabControlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlView.SelectedIndex >= 0)
            {
                //bool haveCode = false;//打开页面中是否有指令表
                if (tabControlView.SelectedTab.Text == CodePageName)
                {
                    return;
                }
                CassViewGenerator.currentTabPage = tabControlView.SelectedTab;
                GetCurrentUndo();

                CurrentDocumentsHostControl.HostDesign.SelectionControl();　　　//选中当前页面
                if (tabControlView.SelectedTab.Controls[0].GetNextControl(
                    tabControlView.SelectedTab.Controls[0], false).Controls.Count > 0)      //当前设计器中的控件不为空
                {
                    selectAllToolStripMenuItem.Enabled = true;
                    aliginToolStripMenuItem.Enabled = false;  //双击时视图不可操作
                }
                else
                {
                    selectAllToolStripMenuItem.Enabled = false;
                    aliginToolStripMenuItem.Enabled = false;
                    cutToolStripMenuItem.Enabled = false;
                    copytoolStripButton.Enabled = false;
                    deleteControlToolStripMenuItem.Enabled = false;
                }

                //获取当前设计器的UndoEngine
                UndoEngineImplication undoEngine =
                    (UndoEngineImplication)CurrentDocumentsHostControl.HostDesign.GetService(typeof(UndoEngineImplication));
                if (undoEngine != null)
                {
                    undotoolStripMenuItem.Enabled = undoEngine.CanUndo;
                    redotoolStripMenuItem.Enabled = undoEngine.CanReDo;
                }
                saveForm.Text = "保存 " + tabControlView.SelectedTab.Text;
                menuStrip.Refresh();
            }
        }

        /// <summary>
        /// 由当前页面获取当前页面对应的
        /// 撤销重做对象
        /// </summary>
        private void GetCurrentUndo()
        {
            foreach (ListObject element in this.tabList)
            {
                if (CassViewGenerator.currentTabPage.Text == element.tabPage.Text)
                {
                    CassViewGenerator.currentUndo = element.UndoAndRedo;
                }
                break;
            }
        }

        /// <summary>
        /// 获取tabControlView当前选项卡中的HostControl类型自定义控件。
        /// </summary>　　　
        private HostControl CurrentDocumentsHostControl
        {
            get
            {
                if (CurrentDocumentsDesignIndex >= 0 && this.tabControlView.TabPages[CurrentDocumentsDesignIndex].Text != CodePageName)
                {//指令表页面出错突然20090630？？                   
                    return (HostControl)this.tabControlView.TabPages[CurrentDocumentsDesignIndex].Controls[0];
                }
                else
                {
                    HostControl tempControl = null;
                    return tempControl;
                }
            }
        }

        /// <summary>
        /// 获取tabControlView控件当前选项卡的索引。
        /// </summary>
        private int CurrentDocumentsDesignIndex
        {
            get
            {
                return this.tabControlView.SelectedIndex;
            }
        }

        /// <summary>
        /// 双击时，关闭当前的tabControlView控件中的选项卡，并修改保存单个页面中的页面名称，修改为"保存"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlView_MouseDown(object sender, MouseEventArgs e)
        {
            int index = 0;  //标志当前在tabControlView中所选中的TabPage的索引值
            try
            {
                if (e.Clicks == PublicVariable.DClick && e.Button == MouseButtons.Left)   //左键双击
                {
                    index = tabControlView.SelectedIndex;
                    if (tabControlView.SelectedTab.Text == CodePageName)
                    {
                        if (CassViewGenerator.ProjectMode == "Code")
                        {//指令模式不能关闭该页
                            CassMessageBox.Warning("指令编辑模式不能关闭指令页面！");
                            return;
                        }
                        //双击指令页面关闭时 询问保存当前页面信息
                        ConfigCodeEdit codeEditor = (ConfigCodeEdit)(tabControlView.SelectedTab.Controls[0]);
                        if (CodeText != "" && codeEditor.CodeEditor.IsDirty == true
                            && CassMessageBox.Question("是否保存当前页面信息？") == DialogResult.Yes)
                        {
                            CodeText = codeEditor.CodeEditor.Text;
                            SaveCodetext();
                        }
                    }
                    tabControlView.TabPages.RemoveAt(index);
                    tabList.RemoveAt(index);

                    if (tabList.Count < 1)
                    {
                        setEditEnable(false);
                        saveForm.Enabled = false;
                        saveForm.Text = "保存";
                        menuStrip.Refresh();
                        saveCurrentFormtoolStripButton.Enabled = false;

                        //清除属性栏中的对象。刷新属性栏
                        controlfilteredPropertyGrid.SelectedObject = null;
                        controlfilteredPropertyGrid.Refresh();
                    }
                    //刷新属性控件，显示当前tabControlView所选择的控件,
                    //该条件为在没选中TabPage时直接双击,则先由状态栏刷新函数显示当前双击的设计页面的属性,
                    //再次显示当前TabPage中的第一个设计页面的属性
                    else
                    {
                        CurrentDocumentsHostControl.HostDesign.SelectionControl();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 从当前所有打开的页面中找到指令页面
        /// 返回其序号找不到则返回-1
        /// </summary>
        /// <returns></returns>
        private int findCodePage()
        {
            for (int k = 0; k < this.tabControlView.TabCount; k++)
            {
                if (this.tabControlView.TabPages[k].Text == CodePageName)
                {
                    return k;
                }
            }
            return -1;
        }
        # endregion

        #region 对文件以及文件夹进行操作的函数,添加新页面**

        /// <summary>
        /// 添加新的设计页面
        /// </summary>
        /// <param name="tabText">为string类型变量，该参数作为TabPage的显示内容 </param>
        /// <param name="hstControl">为HostControl变量，该参数作为返回值TabPage的子控件，放在TabPage返回值中</param>
        /// <returns>TabPage变量，返回值直接作为tabControlView控件中的一个子选项</returns>
        private System.Windows.Forms.TabPage AddTabForNewHost(string tabText, HostControl hstControl)
        {
            System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage(tabText);
            hstControl.Parent = tabPage;
            hstControl.Dock = DockStyle.Fill;
            this.tabControlView.TabPages.Add(tabPage);
            this.tabControlView.SelectedIndex = this.tabControlView.TabPages.Count - 1;
            hostDesignManage.ActiveDesignSurface = hstControl.HostDesign;
            CurrentDocumentsHostControl.HostDesign.SelectionControl();
            saveForm.Enabled = true;
            saveForm.Text = "保存 " + tabControlView.SelectedTab.Text;
            saveCurrentFormtoolStripButton.Enabled = true;
            menuStrip.Refresh();
            return tabPage;
        }

        /// <summary>
        /// 新建设计页面时，在工程管理的路径下生成相应的节点名称。在生成设计界面时，
        /// 依次生成DesignView_1，DesignView_2……为名称的文件名
        /// 保证在同一文件夹名称下的所有设计页面文件的名称是唯一。
        /// 该函数中生成一个HostControl对象，并通过函数 AddTabForNewHost
        /// (string tabText, HostControl hstControl)将生成的HostControl对象添加到tabPage对象中，
        /// 将返回的TabPage对象和当前所创建的节点的路径作为 listObject对象的内容，
        /// 添加到nodeList链表中进行管理。
        /// </summary>
        /// <return>false:生成过程中出现错误,true:正常加载</return>
        private bool AddNewDesign()
        {
            //int formCount = 0;         //获取当前添加的设计页面的下标
            string nodeName = null;    //获取当前的设计页面的个数 
            ListObject listObject = new ListObject();  //新增加的链表结点

            try
            {

                //formCount = solutionTreeView.SelectedNode.Nodes.Count + 1;
                //当生成子策略的父策略不是main则取名为父策略名后加序号
                if (solutionTreeView.SelectedNode.Text != MainPageName)
                {
                    nodeName = solutionTreeView.SelectedNode.Text;
                }
                else
                {
                    nodeName = PublicVariable.DesignViewName;
                }
                for (int nodeCount = 1; ; nodeCount++)
                {
                    bool findCount = false;
                    for (int i = 0; i < solutionTreeView.SelectedNode.Nodes.Count; i++)
                    {
                        string tempnodeName = nodeName + nodeCount.ToString();
                        if (tempnodeName == solutionTreeView.SelectedNode.Nodes[i].Name)
                        {
                            findCount = true;
                            break;
                        }
                    }
                    if (!findCount)
                    {
                        nodeName += nodeCount.ToString();
                        break;
                    }
                }

                HostControl hostControl = hostDesignManage.GetNewHost(nodeName);

                if (hostControl != null && hostControl.LoadBool == true)
                {
                    listObject.tabPage = AddTabForNewHost(nodeName, hostControl);

                    TreeNode designNode = CreateNode(nodeName, this.treeMenuPage);

                    this.solutionTreeView.SelectedNode.Nodes.Add(designNode);
                    listObject.pathName = designNode.FullPath.Trim();
                    //hostControl.Tag = listObject.pathName;
                    //将对象添加到链表中
                    this.tabList.Add(listObject);
                    nodeList.Add(listObject);

                    //按键设置，用于对当前控件的选择
                    if (tabControlView.SelectedTab != null)
                    {
                        CassViewGenerator.currentTabPage = this.tabControlView.SelectedTab;
                    }

                    this.solutionTreeView.ExpandAll();

                    if (nodeList.Count > 0)   //如果有设计页面,则显示可以形成项目文件
                    {
                        this.saveProject.Enabled = true;
                    }
                    //设置编辑栏中的具体状态 
                    this.aliginToolStripMenuItem.Enabled = false;
                    this.cutToolStripMenuItem.Enabled = false;
                    this.copyToolStripMenuItem.Enabled = false;
                    this.deleteControlToolStripMenuItem.Enabled = false;
                    this.selectAllMenuItem.Enabled = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除设计界面及节点，每次删除一个节点时，在相应的nodeList和tabList链表中进行删除相应的元素
        /// </summary>
        private void DeleteDesign()
        {
            string nodePath = null;      //节点路径
            string listName = null;      //当前文件夹下的节点的路径，供循环时使用
            int index = -1;     //作为for循环的下标使用,
            TreeNode removeNode = solutionTreeView.SelectedNode;
            DialogResult result;

            try
            {
                result = CassMessageBox.Question("确定删除" + removeNode.Text + "页面？");
                if (result == DialogResult.Yes)
                {
                    //得出该节点的路径。在nodeListt中删除；
                    nodePath = removeNode.FullPath.Trim();
                    for (index = 0; index < nodeList.Count; index++)
                    {
                        listName = ((ListObject)nodeList[index]).pathName;
                        if (listName != null && listName.Contains(nodePath)
                            && listName.Length >= nodePath.Length
                            && listName.Substring(0, nodePath.Length).Equals(nodePath))
                        {
                            if (index >= 0)
                            {
                                nodeList.RemoveAt(index);
                                index--;
                            }
                        }
                    }
                    if (nodeList.Count < 1)
                    {
                        tabList.Clear();
                        tabControlView.TabPages.Clear();
                        setEditEnable(false);
                        saveForm.Enabled = false;
                        saveForm.Text = "保存";
                        saveCurrentFormtoolStripButton.Enabled = false;
                        controlfilteredPropertyGrid.SelectedObject = null;
                        controlfilteredPropertyGrid.Refresh();
                        menuStrip.Refresh();
                    }
                    else        //不为空
                    {
                        //对TabControl链表操作
                        for (index = 0; index < tabList.Count; index++)
                        {
                            listName = ((ListObject)tabList[index]).pathName;
                            //若pathList中存在以当前路径为开头的字符串，则进行删除
                            if (listName != null && listName.Contains(nodePath)
                                && listName.Length >= nodePath.Length
                                && listName.Substring(0, nodePath.Length).Equals(nodePath))
                            {
                                if (index >= 0)
                                {
                                    tabList.RemoveAt(index);
                                    tabControlView.TabPages.RemoveAt(index);
                                    index--;
                                }
                            }
                        }
                        if (tabList.Count < 1)
                        {
                            setEditEnable(false);
                            saveForm.Enabled = false;
                            saveForm.Text = "保存";
                            saveCurrentFormtoolStripButton.Enabled = false;
                            controlfilteredPropertyGrid.SelectedObject = null;
                            controlfilteredPropertyGrid.Refresh();
                            menuStrip.Refresh();
                        }
                    }
                    removeNode.Remove();

                    //try
                    //{
                    //    //在磁盘上做相应的删除操作
                    //    File.Delete(Path.Combine(savePath, nodePath) + ".xml");
                    //    Directory.Delete(Path.Combine(savePath, nodePath), true);
                    //}
                    //catch (Exception ex)
                    //{
                    //    return;
                    //}
                }//if (result == DialogResult.Yes)
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                CassMessageBox.Warning("删除设计页面时发生异常！");
            }
        }

        #endregion

        #region 菜单点击事件处理函数

        private void newViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!AddNewDesign())
            {
                CassMessageBox.Warning("新建设计页面时发生异常！");
                return;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteDesign();
        }

        private void existDesignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExistDesign();
        }

        /// <summary>
        /// 重命名文件夹名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameProjectName();
        }

        /// <summary>
        /// 添加已存在项函数
        ///添加现有项，根据所选择的路径读入现有Xml文件
        /// 根据程杰提供的参数进行资源管理器和设计器界面的显示等。
        /// </summary>
        private void ExistDesign()
        {
            string openName = null;     //打开的文件的名称
            string currentForm = null;      //当前打开的.xml文件的绝对路径
            string fileName = null;     //打开的文件名称,不含后缀
            ListObject listObject = new ListObject();       //表示当前添加的页的结构体，操作成功后添加到nodeList中
            TreeNode tempNode = new TreeNode();     //添加的新的树结点
            int index = 0;      //作循环标志用
            OpenFileDialog dialog = new OpenFileDialog();       //选择文件的对话框
            DialogResult result;        //文件对话框操作后的结果

            try
            {
                // 打开文件对话框
                dialog.DefaultExt = "xml";
                dialog.Filter = "Xml Files|*.xml";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    currentForm = dialog.FileName;

                    //从完整的文件名字中提取文件名字                
                    openName = Path.GetFileName(currentForm);
                    fileName = Path.GetFileNameWithoutExtension(currentForm);
                    currentForm = Path.GetFileNameWithoutExtension(openName);   //去掉路径的后缀

                    if (!JudgeName(fileName, "设计页面名称"))
                    {
                        return;
                    }

                    HostControl hc = hostDesignManage.GetNewDesignHost(openName);
                    if (hc != null && hc.LoadBool == true)
                    {
                        System.Windows.Forms.TabPage tabpage = new System.Windows.Forms.TabPage(currentForm);
                        hc.Parent = tabpage;
                        hc.Dock = DockStyle.Fill;

                        //检查文件名是否已存在该文件夹下。如果否，则继续选择新的现有项

                        for (index = 0; index < solutionTreeView.SelectedNode.Nodes.Count; index++)
                        {
                            tempNode = solutionTreeView.SelectedNode.Nodes[index];
                            if (tempNode.Text.Trim() == currentForm)
                            {
                                result = CassMessageBox.Question("已存在该文件，是否覆盖？");
                                if (result == DialogResult.Yes)
                                {
                                    //覆盖,找出链表中的位置，并修改tabPage;
                                    for (int i = 0; i < nodeList.Count; i++)
                                    {
                                        if (((ListObject)nodeList[i]).pathName == tempNode.FullPath.Trim())
                                        {
                                            nodeList.RemoveAt(i);
                                            listObject.pathName = tempNode.FullPath.Trim();
                                            listObject.tabPage = tabpage;
                                            nodeList.Insert(i, listObject);
                                            break;
                                        }
                                    }

                                    //对tabControlView控件操作
                                    for (int i = 0; i < tabList.Count; i++)
                                    {
                                        if (((ListObject)tabList[i]).pathName == tempNode.FullPath.Trim())
                                        {
                                            tabList.RemoveAt(i);
                                            tabControlView.TabPages.RemoveAt(i);
                                            listObject.pathName = tempNode.FullPath.Trim();
                                            listObject.tabPage = tabpage;

                                            tabList.Insert(i, listObject);
                                            tabControlView.TabPages.Insert(i, tabpage);
                                            tabControlView.SelectedIndex = i;
                                            CassViewGenerator.currentTabPage = tabControlView.SelectedTab;
                                            break;
                                        }
                                    }
                                    return;
                                }//if (result == DialogResult.Yes) 覆盖同名文件
                                else //同名但不覆盖，不做任何操作
                                {
                                    return;
                                }
                            }  //if (tempNode.Text.Trim() == currentForm) 同名
                        }//end for

                        //没有同名称
                        //添加到nodeList链表和资源管理器中
                        if (index == solutionTreeView.SelectedNode.Nodes.Count)
                        {
                            listObject.pathName = solutionTreeView.SelectedNode.FullPath.Trim() + "\\" + currentForm;
                            listObject.tabPage = tabpage;
                            nodeList.Add(listObject);

                            TreeNode treeNode = CreateNode(currentForm, this.treeMenuPage);
                            treeNode.Tag = "Page";

                            solutionTreeView.SelectedNode.Nodes.Add(treeNode);
                            solutionTreeView.ExpandAll();
                        }
                        if (nodeList.Count > 0)   //如果有设计页面,则显示可以形成项目文件
                        {
                            saveForm.Enabled = true;
                            saveProject.Enabled = true;
                        }
                    } //HostControl != null && HostControl.LoadBool
                    else
                    {
                        CassMessageBox.Error("加载页面失败，可能描述文件格式错误！");
                    }
                } //dialog.ShowDialog() == DialogResult.OK

                dialog.Dispose();
            }
            catch (Exception ex) { }
        }

        private void RenameProjectName()
        {
            TreeNode RenameNode = solutionTreeView.SelectedNode;   //重命名节点
            originalPath = RenameNode.FullPath.Trim();    //重命名节点的路径
            originalName = RenameNode.Text.Trim();        //重命名节点的名字
            solutionTreeView.LabelEdit = true;

            if (RenameNode.Text == CodePageName)
            {
                solutionTreeView.LabelEdit = false;
            }
            else if (RenameNode != null)
            {
                if (!RenameNode.IsEditing)
                {
                    RenameNode.BeginEdit();
                }
            }
        }
        #endregion

        #region 工程资源管理器操作

        /// <summary>
        /// 打开页面，菜单栏单击事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void openPageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //先获取当前clickNode的路径名。然后在nodeList中进行比较，
            //获得tabList中的下标，该下标即为TabControl中TabPage的下标。

            int clickIndex = -1;       //作为当前所选中的树节点所在的文件夹的指向的设计页面的索引，
            string clickNodePath = this.solutionTreeView.SelectedNode.FullPath.Trim();    //当前点击的树节点的路径
            for (int i = 0; i < this.tabList.Count; i++)
            {
                if (((ListObject)(this.tabList[i])).pathName.Equals(clickNodePath))
                {
                    this.tabControlView.SelectedIndex = i;
                    clickIndex = i;
                    break;
                }
            }
            if (clickIndex < 0)
            {
                for (clickIndex = 0; clickIndex < nodeList.Count; clickIndex++)
                {
                    if (((ListObject)(nodeList[clickIndex])).pathName.Equals(clickNodePath))
                    {
                        this.tabList.Add((ListObject)nodeList[clickIndex]);
                        this.tabControlView.TabPages.Add(((ListObject)nodeList[clickIndex]).tabPage);
                        this.tabControlView.SelectedIndex = tabList.Count - 1;

                        //第一次双击时，由于TabControl中的TabPage的数量为0，不会触发TabControl的选择变化事件，
                        //故，需在此添加第一次TabPgae加载时，对显示属性处理的过程
                        if (this.tabControlView.Controls.Count == 1)
                        {
                            CassViewGenerator.currentTabPage = this.tabControlView.SelectedTab;
                            //tabControlView.SelectedTab.Controls[0]为HostControl类型变量 
                            CurrentDocumentsHostControl.HostDesign.SelectionControl();
                        }
                        if (this.tabControlView.SelectedTab.Controls[0].GetNextControl(
                            this.tabControlView.SelectedTab.Controls[0], false).Controls.Count > 0)
                        //当前设计器中的控件不为空
                        {
                            setEditEnable(true);//编辑菜单栏操作
                            this.selectAllToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            setEditEnable(false);     //不可编辑
                            selectAllToolStripMenuItem.Enabled = false;   //不可选择
                        }
                        this.undotoolStripMenuItem.Enabled = false;
                        this.redotoolStripMenuItem.Enabled = false;

                        this.saveForm.Enabled = true;
                        this.saveCurrentFormtoolStripButton.Enabled = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 资源管理器子节点添加限制事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeMenuPage_Opening(object sender, CancelEventArgs e)
        {
            if (solutionTreeView.SelectedNode.Parent == null)
            {//判断是否显示删除选项
                deleteToolStripMenuItem.Visible = false;
            }
            else
            {
                deleteToolStripMenuItem.Visible = true;
            }
            if (solutionTreeView.SelectedNode.Level == 4//最多四级子节点 
                || (solutionTreeView.SelectedNode.Level == 3 && solutionTreeView.SelectedNode.Nodes.Count >= 127)//3级子节点最多127个 
                || (solutionTreeView.SelectedNode.Level < 3 && solutionTreeView.SelectedNode.Nodes.Count >= 255))//1、2级子节点最多255个
            {//判断添加选项是否可用
                AddNewItem.Enabled = false;
            }
            else
            {
                AddNewItem.Enabled = true;
            }
        }

        /// <summary>
        /// 双击时改变当前设计器，并修改单个页面保存时显示的页面的名称
        /// 在该函数中需考虑打开的页面是否具有控件叠加的信息，如有， 则需添加到设计器中；
        /// 并对编辑菜单中的状态进行改变，显示相应的可操作项
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void SolutionTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            int clickIndex = -1;       //作为当前所选中的树节点所在的文件夹的指向的设计页面的索引，
            string clickNodePath = null;    //当前点击的树节点的路径

            try
            {
                TreeNode clickNode = this.solutionTreeView.GetNodeAt(e.X, e.Y);
                if (clickNode != null)
                {
                    //双击时先在tabList中查找是否存在，若没有则再在nodeList中查找。显示当前设计器，单击右键只作选择动作
                    if (e.Clicks == PublicVariable.DClick & e.Button == MouseButtons.Left)
                    {
                        //先获取当前clickNode的路径名。然后在nodeList中进行比较，
                        //获得tabList中的下标，该下标即为TabControl中TabPage的下标。
                        clickNodePath = clickNode.FullPath.Trim();

                        for (int i = 0; i < this.tabControlView.TabPages.Count; i++)
                        {
                            if (this.tabControlView.TabPages[i].Text == clickNode.Text)
                            {
                                this.tabControlView.SelectedTab = this.tabControlView.TabPages[i];
                                clickIndex = i;
                                break;
                            }
                        }
                        if (clickIndex < 0)//不存在当前页面
                        {
                            if (clickNode.Text == CodePageName)//20090531
                            {
                                OpenCodetext();
                                //GenerateCode newGenCode = new GenerateCode(CreateCodeList());
                                //this.IOlist = newGenCode.GenerateCodeList();
                                //this.tempValue = newGenCode.TempInfo;
                                //AddCodeListPage(this.IOlist);
                                return;
                            }
                            for (clickIndex = 0; clickIndex < nodeList.Count; clickIndex++)
                            {
                                if (((ListObject)(nodeList[clickIndex])).pathName.Equals(clickNodePath))
                                {
                                    this.tabList.Add((ListObject)nodeList[clickIndex]);
                                    this.tabControlView.TabPages.Add(((ListObject)nodeList[clickIndex]).tabPage);
                                    this.tabControlView.SelectedIndex = tabList.Count - 1;
                                    //设定页面连线编辑设定
                                    ListObject node = (ListObject)nodeList[clickIndex];
                                    CassView tempOBJ = (CassView)node.tabPage.Controls[0].GetNextControl(node.tabPage.Controls[0], false);
                                    tempOBJ.LinesEditable = this.linkToolStripButton.Enabled;
                                    tempOBJ.ChoosedLine = -1;
                                    tempOBJ.IsPortPoint = false;

                                    //第一次双击时，由于TabControl中的TabPage的数量为0，不会触发TabControl的选择变化事件，
                                    //故，需在此添加第一次TabPgae加载时，对显示属性处理的过程
                                    if (this.tabControlView.Controls.Count == 1)
                                    {
                                        CassViewGenerator.currentTabPage = this.tabControlView.SelectedTab;
                                        //tabControlView.SelectedTab.Controls[0]为HostControl类型变量 
                                        CurrentDocumentsHostControl.HostDesign.SelectionControl();
                                    }


                                    if (this.tabControlView.SelectedTab.Controls[0].GetNextControl(
                                        this.tabControlView.SelectedTab.Controls[0], false).Controls.Count > 0)
                                    //当前设计器中的控件不为空
                                    {
                                        setEditEnable(true);//编辑菜单栏操作
                                        this.selectAllToolStripMenuItem.Enabled = true;
                                    }
                                    else
                                    {
                                        setEditEnable(false);     //不可编辑
                                        selectAllToolStripMenuItem.Enabled = false;   //不可选择
                                    }
                                    this.undotoolStripMenuItem.Enabled = false;
                                    this.redotoolStripMenuItem.Enabled = false;

                                    this.saveForm.Enabled = true;
                                    this.saveCurrentFormtoolStripButton.Enabled = true;
                                    break;
                                }
                            }
                        }
                        this.dockManager.ShowContent(this.tabSolution_Property);//显示属性表
                    }  //左键双击
                    else if (e.Button == MouseButtons.Left) //左键单击，什么也不做
                    {
                        TreeNode RenameNode = solutionTreeView.SelectedNode;   //重命名节点
                        this.originalPath = RenameNode.FullPath.Trim();    //重命名节点的路径
                        this.originalName = RenameNode.Text.Trim();        //重命名节点的名称
                        solutionTreeView.LabelEdit = true;

                        if (RenameNode.Text == CodePageName)
                        {
                            solutionTreeView.LabelEdit = false;
                        }
                        else if (RenameNode != null && RenameNode.Text != CodePageName)
                        {
                            if (!RenameNode.IsEditing)
                            {

                                RenameNode.BeginEdit();
                            }
                        }
                        return;
                    }
                    this.solutionTreeView.SelectedNode = clickNode;

                    if (this.tabControlView.SelectedTab != null)
                    {
                        this.saveForm.Text = "保存 " + tabControlView.SelectedTab.Text;
                        this.menuStrip.Refresh();
                    }

                    designViewStripMenu.Enabled = true;
                    this.aliginToolStripMenuItem.Enabled = false;  //双击时视图不可操作
                }//clickNode!= null
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 重命名后进行名称的检查，确保名称的合法性和唯一性
        /// 并且修改后的名称需符合xml节点的输入法要求
        /// 如果修改的设计页面或文件夹名称，则修改磁盘文件上的名称，而对文件的内容不做处理
        /// 名称重新修改成功后，修改nodeList、tabList、起始页面、tabControlView中的名称
        /// 对当前的项目的名称则不可修改
        /// </summary>
        /// <param name="sender">事件主体</param>
        /// <param name="e">所触发事件的类型</param>
        private void SolutionTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            string renamePath = null;     //被修改节点的原路径
            int nodeIndex = 0;           //作为循环体下标 
            bool renameFlag = true;           //标志是否已修改 
            ListObject listObject = new ListObject();

            try
            {
                if (e.Label != null)  //该节点的新文本不为空
                {
                    if (e.Label.Trim().ToString().Length > 0) //不为空
                    {
                        if (!JudgeName(e.Label.ToString(), "策略页面名"))
                        {
                            e.CancelEdit = true;
                            return;
                        }
                        else //字符均合法
                        {
                            //if (solutionTreeView.SelectedNode.Parent != null)
                            //{
                            nodeIndex = solutionTreeView.SelectedNode.Index;
                            //for (int i = 0; i < solutionTreeView.Nodes[0].Nodes.Count; i++) //不应该与自己比较 
                            //{
                            //    if (i == nodeIndex) //不与自己比较 
                            //    {
                            //        continue;
                            //    }
                            //    //if (
                            //    //    //solutionTreeView.SelectedNode.Parent.Nodes[i] != null
                            //    //    //&& 
                            //    //    //solutionTreeView.SelectedNode.Parent.Nodes[i].ContextMenuStrip == solutionTreeView.SelectedNode.ContextMenuStrip
                            //    //    //&& 
                            //    //    e.Label.Trim().ToString().Equals(solutionTreeView.Nodes[i].Text))
                            if (!checkNode(e.Label, solutionTreeView.SelectedNode, solutionTreeView.Nodes))
                            {
                                CassMessageBox.Warning("页面名称" + e.Label + "已存在！");
                                renameFlag = false;
                                e.CancelEdit = true;
                                //e.Node.BeginEdit();
                                return;
                            }
                            else
                            {//成功修改名称后。先修改nodeList中的名称，再修改tabList中修改
                                if (solutionTreeView.SelectedNode.Text == MainPageName)
                                {//修改主页面名
                                    MainPageName = e.Label;
                                    renamePath = e.Label.Trim().ToString();
                                }
                                else
                                {
                                    renamePath = solutionTreeView.SelectedNode.Parent.FullPath.Trim()
                                          + "\\" + e.Label.Trim().ToString();
                                }
                            }
                            if (this.originalPath != renamePath && renameFlag == true)
                            {
                                for (nodeIndex = 0; nodeIndex < nodeList.Count; nodeIndex++)
                                {
                                    if (MainPageName == e.Label || ((ListObject)nodeList[nodeIndex]).pathName.Contains(originalPath))
                                    {
                                        if (((ListObject)nodeList[nodeIndex]).pathName != null
                                            && ((ListObject)nodeList[nodeIndex]).pathName.Length >= originalPath.Length
                                            && ((ListObject)nodeList[nodeIndex]).pathName.Substring(0, originalPath.Length).Equals(originalPath))
                                        {
                                            listObject.tabPage = ((ListObject)nodeList[nodeIndex]).tabPage;
                                            listObject.tabPage.Text = e.Label;
                                            listObject.pathName = renamePath + ((ListObject)nodeList[nodeIndex]).pathName.Substring(originalPath.Length);
                                            nodeList.RemoveAt(nodeIndex);
                                            nodeList.Insert(nodeIndex, listObject);
                                        }
                                    }
                                }
                                for (nodeIndex = 0; nodeIndex < this.tabList.Count; nodeIndex++)
                                {
                                    if (((ListObject)tabList[nodeIndex]).pathName.Contains(originalPath))
                                    {
                                        if (((ListObject)nodeList[nodeIndex]).pathName != null
                                            && ((ListObject)tabList[nodeIndex]).pathName.Length >= originalPath.Length
                                            && ((ListObject)tabList[nodeIndex]).pathName.Substring(0, originalPath.Length).Equals(originalPath))
                                        {
                                            listObject.tabPage = ((ListObject)tabList[nodeIndex]).tabPage;
                                            listObject.tabPage.Text = e.Label;
                                            listObject.pathName = renamePath + ((ListObject)tabList[nodeIndex]).pathName.Substring(originalPath.Length);
                                            this.tabList.RemoveAt(nodeIndex);
                                            this.tabList.Insert(nodeIndex, listObject);
                                        }
                                    }
                                }

                                ////修改文件夹名称
                                //if (Directory.Exists(savePath + "\\" + originalPath))
                                //{
                                //    Directory.Move(savePath + "\\" + originalPath, savePath + "\\"
                                //        + solutionTreeView.SelectedNode.Parent.FullPath.Trim()
                                //        + "\\" + e.Label.Trim().ToString());
                                //}

                                ////修改文件名称
                                //if (File.Exists(savePath + "\\" + originalPath + ".xml"))
                                //{
                                //    File.Move(savePath + "\\" + originalPath + ".xml", savePath + "\\"
                                //        + solutionTreeView.SelectedNode.Parent.FullPath.Trim()
                                //        + "\\" + e.Label.Trim().ToString() + ".xml");
                                //}

                                e.Node.EndEdit(false);
                                saveForm.Text = "保存" + tabControlView.SelectedTab.Text.ToString();        //修改保存当前页面的内容
                                menuStrip.Refresh();

                            }       //为文件节点
                            //}       //不为根节点
                            //else
                            //{
                            //    e.CancelEdit = true;
                            //}
                        }
                    }       //不为空
                    else        //文件名为空
                    {
                        e.CancelEdit = true;
                        CassMessageBox.Warning("文件名不能为空！\n请重新输入文件名");
                        renameFlag = false;
                        //e.Node.BeginEdit();
                    }
                } //该节点的新文本不为空
            }
            catch (XmlException ex)
            {
                CassMessageBox.Warning("输入的文件名不合法！");
                e.CancelEdit = true;
                renameFlag = false;
                //e.Node.BeginEdit();
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 递归遍历节点及其子节点判断是否存在同名节点
        /// </summary>
        /// <param name="name">所要检测的节点名</param>
        /// <param name="curNode">所要修改节点名的节点</param>
        /// <param name="treeNodes">同级节点集合</param>
        /// <returns>是否存在同名</returns>
        private bool checkNode(string name, TreeNode curNode, TreeNodeCollection treeNodes)
        {
            foreach (TreeNode child in treeNodes)
            {
                if (child != curNode && child.Text == name)
                {
                    return false;
                }
                else if (child.Nodes.Count != 0)
                {
                    if (!checkNode(name, curNode, child.Nodes))
                    { return false; }
                }
            }
            return true;
        }

        #endregion

        #region  保存单个页面
        /// <summary>
        /// 单页面保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveForm_Click(object sender, EventArgs e)
        {
            SaveCurrentForm();
        }

        /// <summary>
        /// 保存当前页的函数操作
        /// 
        /// </summary>
        private void SaveCurrentForm()
        {
            XmlDocument document = new XmlDocument(); ;   //定义XML文档
            System.Windows.Forms.TabPage page = tabControlView.SelectedTab;  //当前打开的页面
            string strTab = page.Text;
            try
            {
                if (strTab == CodePageName)
                {
                    ConfigCodeEdit codeEditor = (ConfigCodeEdit)(tabControlView.SelectedTab.Controls[0]);
                    CodeText = codeEditor.CodeEditor.Text;
                    SaveCodetext();
                }
                else
                {
                    Control cassControl = page.Controls[0].GetNextControl(page.Controls[0], false);    //得到CassView控件 
                    CodeDomHostLoader codeDomHostLoader = new CodeDomHostLoader();
                    HostDesign design = ((HostControl)page.Controls[0]).HostDesign;

                    if (cassControl == null)
                    {
                        CassMessageBox.Warning("保存失败！");
                        return;
                    }

                    XmlNode documentNode = document.CreateElement(cassControl.Site.Name.ToString());   //添加总结点
                    document.AppendChild(documentNode);   //添加总节点              
                    //调用CodeDomHostLoader类的SaveToFile函数把当前页属性保存到文件
                    codeDomHostLoader.SaveToFile(document, strTab, cassControl, designerPath,
                        Path.Combine(savePath, ((ListObject)tabList[tabControlView.SelectedIndex]).pathName + ".xml"));
                }
            }
            catch (Exception ex)
            {
                CassMessageBox.Warning("保存过程发生异常！");
            }
        }
        #endregion

        #region 工程管理

        #region 工程管理操作

        /// <summary>
        /// 双击工程 切换工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProListBox_DoubleClick(object sender, EventArgs e)
        {
            if ((ProjectName != ProListBox.SelectedItems[0].Text || tabList.Count == 0))
            {//同一工程双击无效 除非当前所有页面为空
             //   OpenProject(programPath + "\\" + ProListBox.SelectedItems[0].Text + "\\" + ProListBox.SelectedItems[0].Text + ".caproj");
                OpenProject(WorkSpacePath + "\\" + ProListBox.SelectedItems[0].Text + "\\" + ProListBox.SelectedItems[0].Text + ".caproj");
            }
        }

        /// <summary>
        /// 添加新工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        /// <summary>
        /// 删除所选工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CassMessageBox.Question("确定要删除工程" + this.ProListBox.SelectedItems[0].Text + "吗？") == DialogResult.Yes)
            {
                DeleteProject(this.ProListBox.SelectedItems[0].Text);
            }
        }

        /// <summary>
        /// 删除工程
        /// 注：工程名大写
        /// </summary>
        /// <param name="ProName"></param>
        private void DeleteProject(string ProName)
        {
            try
            {
              //  DirectoryInfo info = new DirectoryInfo(programPath);
                DirectoryInfo info = new DirectoryInfo(WorkSpacePath);
                DirectoryInfo[] infos = info.GetDirectories();
                foreach (DirectoryInfo var in infos)
                {
                    if (var.Name == ProName)
                    {
                        if (ProName == ProjectName)
                        {//删除当前工程则不需要读取工程文件
                            DeletePnum(ProjectNum);
                            ClearResource();//并清空当前资源
                        }
                        else
                        {//读取工程序号后删除
                            FileStream fStream = new FileStream(var.FullName + "\\" + var.Name + ".caproj", FileMode.Open);  //读工程文件内容
                            StreamReader sReader = new StreamReader(fStream);              //读取字符
                            string DelIndex = sReader.ReadLine().Split(',')[0];
                            DeletePnum(DelIndex);
                            sReader.Close();
                            fStream.Close();
                        }
                        Directory.Delete(var.FullName, true);
                        for (int i = 0; i < this.ProListBox.Items.Count; i++)
                        {
                            if (this.ProListBox.Items[i].Text == ProName)
                            { this.ProListBox.Items.RemoveAt(i); }
                        }
                        if (ProjectName == var.Name)//删除当前工程清空树
                        {
                            this.solutionTreeView.Nodes.Clear();
                        }
                        break;
                    }
                }
            }
            catch
            { CassMessageBox.Error("删除工程失败！"); }
        }

        /// <summary>
        /// 打开工程管理中现有选定工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenOldStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
              //  OpenProject(programPath + "\\" + ProListBox.SelectedItems[0].Text + "\\" + ProListBox.SelectedItems[0].Text + ".caproj");
                OpenProject(WorkSpacePath + "\\" + ProListBox.SelectedItems[0].Text + "\\" + ProListBox.SelectedItems[0].Text + ".caproj");
            }
            catch
            { CassMessageBox.Information("打开工程失败，请重新选择需要打开的工程名！"); }
        }

        /// <summary>
        /// 打开不存在于工程管理栏中的工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenNewStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();  //文件对话框
            try
            {
                dialog.DefaultExt = "caproj";
                dialog.Filter = "Project Files|*.caproj";
                string SourcePath = null;
                string TargetPath = null;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string UpperProName = Path.GetFileNameWithoutExtension(dialog.FileName).ToUpper();//大写目标文件夹名即工程名
                    SourcePath = Directory.GetCurrentDirectory();//当前打开的目录
                  //  TargetPath = programPath + "\\" + UpperProName;
                    TargetPath = WorkSpacePath + "\\" + UpperProName;

                    if (Directory.Exists(TargetPath))
                    {//存在同名工程则询问
                        DialogResult result = CassMessageBox.Question("存在同名的工程，是否替换原有的工程？");
                        if (result == DialogResult.No)
                        {//不允许替换则释放资源后返回
                            dialog.Dispose();
                            return;
                        }
                        else
                        {//删除原有同名工程//注：考虑原有工程名小写
                            DeleteProject(Path.GetFileNameWithoutExtension(dialog.FileName).ToUpper());
                        }
                    }
                    //不存在同名工程或存在同名时允许替换原有工程文件
                    //则赋值目标工程至工程目录文件夹下
                    DirectoryCopy(SourcePath, TargetPath);
                    OpenProject(TargetPath + "\\" + UpperProName + ".caproj");
                    ListViewItem newProject = new ListViewItem(new string[] { ProjectName, ProjectInfo });
                    this.ProListBox.Items.Add(newProject);
                }//if (dialog.ShowDialog() == DialogResult.OK)
            }
            catch (Exception ex) { }
            finally
            {
                dialog.Dispose();
            }
        }

        /// <summary>
        /// 修改工程名和工程信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyStripMenuItem_Click(object sender, EventArgs e)
        {
            NewPorject newForm = new NewPorject(this.ProListBox.SelectedItems[0].Text, this.ProListBox.SelectedItems[0].SubItems[1].Text);
            newForm.setProjectNameTextUnEnabled(); //2013.11.22
            if (newForm.ShowDialog() == DialogResult.OK)
            {
                if (this.ProListBox.SelectedItems[0].Text == ProjectName)
                {//如果修改的是当前工程
                    //先保存当前工程
                    saveProjectOperation();
                    //修改当前工程名和工程信息
                    ProjectName = newForm.Pname;
                    ProjectInfo = newForm.Pinfo;
                    this.savePath = Path.Combine(WorkSpacePath, ProjectName);//修改保存路径
                 //   this.savePath = Path.Combine(WorkSpacePath,ProjectName);
                    CassViewGenerator.currentProjectPath = this.savePath;  //2013.11.22
                    this.Text = this.Text.Split('_')[0] + "_" + ProjectName;
                }
                string NewProPath = WorkSpacePath + "\\" + this.ProListBox.SelectedItems[0].Text;//修改后工程目录文件夹
                string OldProFile = Path.Combine(NewProPath, this.ProListBox.SelectedItems[0].Text) + ".caproj";//修改后为修改的工程文件路径
                string NewProFild = Path.Combine(NewProPath, newForm.Pname) + ".caproj";//修改后新的工程文件路径

                //读取旧工程文件后删除
                List<string> getInfo = new List<string>();
                string rowInfo = null;
                FileStream fRStream = new FileStream(OldProFile, FileMode.Open);
                StreamReader sReader = new StreamReader(fRStream);
                while ((rowInfo = sReader.ReadLine()) != null)
                {
                    getInfo.Add(rowInfo);
                }
                sReader.Close();
                fRStream.Close();
                File.Delete(OldProFile);

                //将读取的原有工程修改其中的工程信息后写入新的工程文件
                FileStream fWStream = new FileStream(NewProFild, FileMode.Create);
                StreamWriter sWriter = new StreamWriter(fWStream);

                for (int i = 0; i < getInfo.Count; i++)
                {
                    if (i == 0)
                    { sWriter.WriteLine(getInfo[i].Split(',')[0] + "," + newForm.Pinfo); }
                    else
                    { sWriter.WriteLine(getInfo[i]); }
                }
                sWriter.Close();
                fWStream.Close();

                //将修改前工程文件夹移植修改后的新工程文件夹
              //  if (!Directory.Exists(Path.Combine(programPath, newForm.Pname)))
                if(!Directory.Exists(Path.Combine(WorkSpacePath,newForm.Pname)))
                {//存在目标工程则只修改工程信息
                    Directory.Move(NewProPath, Path.Combine(programPath, newForm.Pname));
                }

                //修改工程管理栏中对应的工程文件名和工程信息
                this.ProListBox.SelectedItems[0].Text = newForm.Pname;
                this.ProListBox.SelectedItems[0].SubItems[1].Text = newForm.Pinfo;
            }
        }

        /// <summary>
        /// 将目标文件夹复制到指定目录文件夹
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        static public void DirectoryCopy(string sourcePath, string targetPath)
        {
            //创建目标文件夹
            Directory.CreateDirectory(targetPath);
            DirectoryInfo SourceDirectory = new DirectoryInfo(sourcePath);
            FileInfo[] Finfos = SourceDirectory.GetFiles();
            DirectoryInfo[] Dinfos = SourceDirectory.GetDirectories();

            foreach (FileInfo finfo in Finfos)
            {
                if (finfo.Extension == ".caproj")
                { //工程文件名必须大写
                    File.Copy(finfo.FullName, Path.Combine(targetPath, finfo.Directory.Name.ToUpper() + ".caproj"));
                }
                else
                {
                    File.Copy(finfo.FullName, Path.Combine(targetPath, finfo.Name));
                }
            }

            if (Dinfos.Length != 0)
            {
                foreach (DirectoryInfo dinfo in Dinfos)
                {
                    DirectoryCopy(dinfo.FullName, Path.Combine(targetPath, dinfo.Name));
                }
            }
        }

        /// <summary>
        /// 工程管理菜单弹出事件
        /// 操作可视修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (this.ProListBox.SelectedItems.Count == 0)
            {//工程管理栏没有选定工程禁止打开、删除、修改
                OpenOldStripMenuItem.Enabled = false;
                DeleteStripMenuItem.Enabled = false;
                ModifyStripMenuItem.Enabled = false;
            }
            else
            {
                OpenOldStripMenuItem.Enabled = true;
                DeleteStripMenuItem.Enabled = true;
                ModifyStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// 清空现有程序所占资源,并更新控件XML文件
        /// </summary>
        private void ClearResource()
        {
            this.solutionTreeView.Nodes.Clear();
            this.solutionTreeView.SelectedNode = null;
            this.addMenuService = false;//还原添加撤销重做功能
            this.solutionTreeView.ImageList = solutionImageList;
            this.Text = this.Text.Split('_')[0];//清空窗口名种的工程名

            ToolBoxServiceImpl.typeNameString = null;
            //更新ToolXML文件
            ToolBoxServiceImpl.toolXML.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PublicVariable.ToolConfigFileName));
            this.tabControlView.TabPages.Clear();
            this.tabList.Clear();
            nodeList.Clear();
            this.addressInfo.Clear();//初始化地址信息
            this.IOlist.Clear();//初始化IO指令信息
            StartComply(false);//初始化编译相关数组            
            CodeText = null;//初始化指令列表信息
            this.controlfilteredPropertyGrid.SelectedObjects = null;//清空属性窗口
            CassViewGenerator.PortInfoList = new List<ArrayList>();//切换工程后清空模块点名信息
            CassViewGenerator.currentUndo = null;
            setEditEnable(false);
   
            //重置调试标记  2014.02
            this.ResetDebugMark();
        }

        /// <summary>
        /// 判断所给的名是否符合命名规则
        /// </summary>
        /// <param name="Name">文件或策略名</param>
        /// <param name="RangeName">该名的称呼</param>
        /// <returns>是否符合</returns>
        static public bool JudgeName(string Name, string RangeName)
        {
            try
            {
                if (Name == null)
                {
                    CassMessageBox.Information(RangeName + "为空！");
                }
                else if (Name.ToString().IndexOfAny(new char[]{'#','%', '*', ':', '&', '\\', '/', '!', 
                        '?', '"', '<', '>', '|',',',';','.','-','+','=','@','$','^','(',')','|',}) != -1)//包含非法字符
                {
                    CassMessageBox.Information(RangeName + "中不能包含以下任何字符串：\n/ ? : & \\ * \" < > | # % , ; .! $ % ^ ( ) 等");
                }
                else if (Name.Length > 0
                    && ((Name[0].ToString().CompareTo("0") >= 0
                    && Name[0].ToString().CompareTo("9") <= 0)
                    || Name[0].ToString().CompareTo("_") == 0))
                {
                    CassMessageBox.Information("不能以数字和下划线开头作为" + RangeName + "！");
                }
                else
                { //判断是否符合xml节点的输入法
                    XmlDocument document = new XmlDocument();
                    XmlNode node = document.CreateElement(Name);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                CassMessageBox.Information("读入的文件名不符合Cass工程的命名规则！");
                return false;
            }
        }

        /// <summary>
        /// 由所给的节点名和菜单创建树节点
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <param name="menuList">对该节点右键弹出的菜单</param>
        /// <returns>树节点</returns>
        private TreeNode CreateNode(string nodeName, ContextMenuStrip menuList)
        {
            TreeNode newNode = new TreeNode(nodeName);
            newNode.Name = nodeName;
            newNode.ContextMenuStrip = menuList;
            newNode.ImageIndex = 0;   //显示的图标索引号
            newNode.SelectedImageIndex = 0;
            newNode.Checked = true;

            return newNode;
        }

        #endregion

        #region  新建工程

        /// <summary>
        /// 新建工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        /// <summary>
        /// 新建工程的具体函数操作。新建工程时，如果发现工程的资源管理器中有其他工程处在打开状态，则提示是否保存，
        /// 如“保存”，则调用 saveProjectOperation()函数进行工程的保存操作;
        /// 对新建的工程的名称进行合法性检查，并在工程的资源管理器中新建一个根级文件夹节点作为当前工程项目文件夹。
        /// 
        /// 注:
        /// 在新建工程时,提供的是要保存的工程的名称,
        /// 在这个路径下会产生该以该工程名称命名的文件夹,该文件夹下会生成一个同名称的,以.caproj为后缀的工程资源管理文件,
        /// 并生成一个空的设计页面，名称为：DesignView_1
        /// </summary>
        private void NewProject()
        {
            try
            {
                if (solutionTreeView.Nodes.Count > 0)
                {
                    DialogResult Sresult = CassMessageBox.Question("是否保存原有项目？");
                    if (Sresult == DialogResult.Yes)
                    {
                        saveProjectOperation();  //调用保存工程程序
                        CassViewGenerator.currentTabPage = null;
                    }
                }

                //清空原有资源管理器
                ClearResource();

                string tempNum = GetNewPnum();//临时工程序号
                NewPorject newForm = new NewPorject(tempNum);
                DialogResult Oresult = newForm.ShowDialog();
                if (Oresult == DialogResult.OK)
                {
                    //保存文件路径、工程名、工程周期
                    ProjectName = newForm.Pname;
                    ProjectInfo = newForm.Pinfo;
                    ProjectNum = tempNum;
                    PnumList.Add(ProjectNum);
                    saveName = newForm.Pname + ".caproj";
                  //  savePath = programPath + "\\" + newForm.Pname;
                    savePath = WorkSpacePath + "\\" + newForm.Pname;
                    CassViewGenerator.currentProjectPath = savePath; //2013.11.22
                    ProjectInfo = newForm.Pinfo;


                    //添加到工程资源管理器节点中。
                    HostControl hostControl = hostDesignManage.GetNewHost(MainPageName);
                    if (hostControl != null && hostControl.LoadBool == true)
                    {
                        ListObject listObject = new ListObject();  //新增加的链表结点
                        listObject.tabPage = AddTabForNewHost(MainPageName, hostControl);
                        TreeNode designNode = CreateNode(MainPageName, this.treeMenuPage);

                        this.solutionTreeView.Nodes.Add(designNode);

                        listObject.pathName = designNode.FullPath.Trim();
                        listObject.UndoAndRedo = new UndoOperation(listObject.tabPage);
                        currentUndo = listObject.UndoAndRedo;
                        //将对象添加到链表中
                        this.tabList.Add(listObject);
                        nodeList.Add(listObject);

                        //按键设置，用于对当前控件的选择
                        if (tabControlView.SelectedTab != null)
                        {
                            CassViewGenerator.currentTabPage = this.tabControlView.SelectedTab;
                        }

                        this.solutionTreeView.Nodes.Add(CodePageName);
                        this.solutionTreeView.ExpandAll();
                        this.dockManager.ShowContent(this.tabSolution_Property);//显示属性表

                        if (nodeList.Count > 0)   //如果有设计页面,则显示可以形成项目文件
                        {
                            this.saveProject.Enabled = true;
                        }
                        //设置编辑栏中的具体状态 
                        this.aliginToolStripMenuItem.Enabled = false;
                        this.cutToolStripMenuItem.Enabled = false;
                        this.copyToolStripMenuItem.Enabled = false;
                        this.deleteControlToolStripMenuItem.Enabled = false;
                        this.selectAllMenuItem.Enabled = false;
                    }
                    else
                    {
                        CassMessageBox.Error("新建工程发生异常！");
                    }

                    Directory.CreateDirectory(savePath);
                    FileStream fStream = new FileStream(this.savePath + "//" + this.saveName, FileMode.Create);

                  //  DirectoryInfo[] infos = new DirectoryInfo(programPath).GetDirectories();
                    DirectoryInfo[] infos = new DirectoryInfo(WorkSpacePath).GetDirectories();
                    //if (infos.Length != this.ProListBox.Items.Count)
                    //{//如果工程文件夹数和现有工程数不同则判定为新加了控件
                    //    ListViewItem newProject = new ListViewItem(new string[] { ProjectName, ProjectInfo });
                    //    this.ProListBox.Items.Add(newProject);
                    //}
                    bool exist = false;
                    for (int i = 0; i < this.ProListBox.Items.Count; i++)
                    {
                        if (ProListBox.Items[i].Text == ProjectName)
                        {
                            exist = true;
                        }
                    }
                    if (!exist)
                    {
                        ListViewItem newProject = new ListViewItem(new string[] { ProjectName, ProjectInfo });
                        this.ProListBox.Items.Add(newProject);
                    }
                    this.Text = this.Text.Split('_')[0] + "_" + ProjectName;//显示当前工程名

                    this.saveProject.Enabled = true;        //允许保存工程
                    this.saveProjecttoolStripButton.Enabled = true;
                    this.closeProjecttoolStripMenuItem.Enabled = true;
                    this.addItemtoolStripSplitButton.Enabled = true;
                    this.designViewStripMenu.Enabled = true;
                    fStream.Close();

                    //新建时状态栏显示图形编辑模式
                //    ModeSelect(0);
                }
            }
            catch (FileNotFoundException ex)
            {
                CassMessageBox.Error("安装文件被损坏！");
            }
            catch (DirectoryNotFoundException ex)
            {
                CassMessageBox.Error("安装文件被损坏！");
            }
            catch (SecurityException ex)
            {
                CassMessageBox.Error("文件权限被修改！");
            }
            catch (PathTooLongException e)
            {
                CassMessageBox.Error("磁盘文件物理路径过长！");
            }
            catch (NotSupportedException e)
            {
                CassMessageBox.Error("创建的目录的路径无效！");
            }
            catch (UnauthorizedAccessException e)
            {
                CassMessageBox.Error("磁盘保护,不可创建目录！");
            }
            catch (IOException e)
            {
                CassMessageBox.Error("文件操作无效！");
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 找到没有使用过的工程序号
        /// 注：找到不添加
        /// </summary>
        /// <returns></returns>
        private string GetNewPnum()
        {
            for (int i = 0; ; i++)
            {
                if (!this.PnumList.Contains(i.ToString()))
                {
                    return i.ToString();
                }
            }
        }

        /// <summary>
        /// 删除工程对应的序号
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DeletePnum(string index)
        {
            for (int i = PnumList.Count - 1; i >= 0; i--)
            {
                if (PnumList[i] == index)
                {
                    PnumList.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 查询是否存在对应的工程序号 20140218
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool ExistsPnum(string index)
        {
            for (int i = PnumList.Count - 1; i >= 0; i--)
            {
                if (PnumList[i] == index)
                { 
                    return true;
                }
            }           
            return false;
        }

        /// <summary>
        /// 修改工程序号（应对于移动工程后，出现的工程序号重复现象）20140218
        /// </summary>
        /// <param name="index"></param>
        private void ModifyProjectNum(string path, string index)
        {

            ArrayList oldContent = FileOperator.ReadFromFile(path);
            ArrayList newContent = new ArrayList();
            foreach (string temp in oldContent)
            {
                if (temp.Contains(","))
                {
                    int startIndex = temp.Split(',')[0].Length;
                    string newStr = temp.Substring(startIndex, temp.Length - startIndex);
                    newStr = index + newStr;
                    newContent.Add(newStr);
                }
                else
                {
                    newContent.Add(temp);
                }
            }
            FileOperator.WriteToFile(path, newContent);

        }

        #endregion

        #region 保存工程

        /// <summary>
        /// 保存工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveProject_Click(object sender, EventArgs e)
        {
            saveProjectOperation();

            if (saveExcepitionFlag == true)
            {
                CassMessageBox.Error("保存工程时发生异常！");
            }
        }

        /// <summary>
        /// 保存整个工程的函数操作,由于在新建工程时,提供的是要保存的工程的名称,
        /// 在这个路径下会产生该以该工程名称命名的文件夹,该文件夹下会生成一个同名称的,以.caproj为后缀的工程资源管理文件,
        /// </summary>
        private void saveProjectOperation()
        {
            string projectFullName = null;      //代表工程资源管理文件夹的路径，到.caproj文件为止。是绝对路径
            string SaveToProjectName = null;        //要保存的设计页面的名称
            XmlDocument document = new XmlDocument();       //对每个页面所要生成的xml文档
            ListObject listObject = new ListObject();       //nodeList链表中的每个元素
            System.Windows.Forms.TabPage page = new System.Windows.Forms.TabPage();       //当前设计页面所在的TabPage。
            string formPathName = null;     //要保存的设计页面的路径（相对路径）,在工程资源管理器中的路径,即:ListObject对象中的PathName属性
            Control viewControl = new Control();        //指向当前的设计页面的容器控件，即CassView控件
            FileStream fStream = null;
            StreamWriter sWriter = null;
     
            try
            {
                saveExcepitionFlag = false;     //重置

                //如果该项目名称不存在，则退出
                if (savePath == null || saveName == null)
                {
                    return;
                }

                //将所有控件显示序号属性置为False
                ShowNumber = true;
                SetShowNumber();

                if (Directory.Exists(Path.Combine(savePath, MainPageName)))
                { Directory.Delete(Path.Combine(savePath, MainPageName), true); }

                //创建工程文件,得到工程目录文件夹下的工程资源管理文件，该文件以.caproj为后缀名
                projectFullName = Path.Combine(savePath, saveName);

                //Create 如果文件存在，则不再新建文件；否则，创建新文件。 
                fStream = new FileStream(projectFullName, FileMode.Create);
                sWriter = new StreamWriter(fStream);

                sWriter.WriteLine(ProjectNum + "," + ProjectInfo);//第一行写工程序号,信息20090615

                //对链表中的每个页面进行操作
                for (int i = 0; i < nodeList.Count; i++)
                {
                    listObject = (ListObject)nodeList[i];       //获得链表中的每个节点 
                    formPathName = listObject.pathName;     //相对路径                   
                    page = listObject.tabPage;
                    //CassView cassview = (CassView)(page.Controls[0].GetNextControl(page.Controls[0], false));

                     //listObject.UndoAndRedo.cassview;

                    //创建每一个文件的所需的文件夹，确保存在                
                    string directoryName = Path.Combine(savePath, Path.GetDirectoryName(formPathName));
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    //通过判断当前的.xml文件是否存在来决定是否需要在工程文件中添加该信息
                    sWriter.WriteLine(formPathName + ".xml");//在工程文件中记录该XML文件的路径
                    document.RemoveAll();    //清空前一次所生成的xml文档的内容
                    SaveToProjectName = Path.Combine(savePath, formPathName);   //绝对路径，所需要的创建的.xml文件的路径
                    viewControl = page.Controls[0].GetNextControl(page.Controls[0], false); //获得CassView控件
                    if (viewControl == null)
                    {
                        break;
                    }

                    HostDesign design = ((HostControl)page.Controls[0]).HostDesign;
                    XmlNode documentNode = document.CreateElement(viewControl.Site.Name.ToString());   //添加总结点
                    document.AppendChild(documentNode);   //添加总节点

                    CodeDomHostLoader codeDomHostLoader = new CodeDomHostLoader();
                    //调用CodeDomHostLoader类的SaveProjectToFile函数把该页属性保存到文件
                    codeDomHostLoader.SaveProjectToFile(document, SaveToProjectName, viewControl, designerPath);
                }       //对链表中的每个页面进行操作
                sWriter.Close();        //需要先关闭字符串流，再关闭文件流
                fStream.Close();
                //保存完页面信息后保存指令信息
                int pageIndex = findCodePage();
                if (pageIndex != -1)
                {
                    ConfigCodeEdit codeEditor = (ConfigCodeEdit)(tabControlView.TabPages[pageIndex].Controls[0]);
                    CodeText = codeEditor.CodeEditor.Text;
                }
                SaveCodetext();
            }
            catch (Exception ex)
            {
                saveExcepitionFlag = true;   //发生异常 
            }
            finally
            {
                if (sWriter != null)
                {
                    sWriter.Dispose();
                }
                if (fStream != null)
                {
                    fStream.Dispose();
                }
            }
            controlfilteredPropertyGrid.Refresh();
        }
        #endregion

        #region 打开工程


        /// <summary>
        /// 打开工程的具体操作函数
        /// 打开工程时，先检查工程资源管理器中是否已存在一个工程，如果是，则提示是否需要保存。
        /// 若“保存”，则调用saveProjectOperation()保存工程函数，
        /// 读取当前所选择工程的工程文件,读取文件中对于工程资源管理中的每个节点的相对路径,根据该相对路径读取磁盘上的xml文件，
        /// 再调用hostDesignManage类中的GetNewHost（string,string）函数进行读取xml资源文件的操作，GetNewHost函数返回HostControl类型值，
        /// 将该HostControl对象添加作为一个TabPage实例的子控件。将节点的相对路径和TabPage实例作为listObject类型变量的值，将listObject变量
        /// 作为nodeList和tabList的一个元素。
        /// 根据节点的相对路径，在工程的资源管理器中创建相应的节点
        private void OpenProjects()
        {
            this.ProListBox.Items.Clear();
            this.picWin.Init();//20140218
            //读取sysInfo.txt中的工作目录信息 20140217
            WorkSpacePath=FileOperator.ReadFromFile2(ProgramPath+PublicVariable.sysInfoFileName);
            WorkSpacePath = WorkSpacePath.Trim();
            if (WorkSpacePath == "")
            {
                WorkSpacePath = PublicVariable.DefaultWorkSpacePath;
            }
            if (Directory.Exists(WorkSpacePath))
            {
                // DirectoryInfo info = new DirectoryInfo(programPath);
                DirectoryInfo info = new DirectoryInfo(WorkSpacePath);
                DirectoryInfo[] infos = info.GetDirectories();
                PnumList = new List<string>();//清空序号列表

                this.ProListBox.Items.Clear();
                foreach (DirectoryInfo var in infos)
                {
                    try
                    {

                        if (JudgeName(var.Name, "打开的工程文件名"))
                        {
                            string ProPath = var.FullName + "\\" + var.Name + ".caproj";
                            //加载main页面
                            if (File.Exists(ProPath))
                            {
                                FileStream fStream = new FileStream(ProPath, FileMode.Open);  //读工程文件内容
                                StreamReader sReader = new StreamReader(fStream);             //读取字符                               
                                string rowInfo = sReader.ReadLine();
                                sReader.Close();
                                fStream.Close();
                                string AddIndex = rowInfo.Split(',')[0];

                                //判断是否存在相同的工程序号 20140218
                                if (ExistsPnum(AddIndex))
                                {
                                    //存在相同的工程序号时，
                                    AddIndex = GetNewPnum();
                                    //重写工程文件xx.caproj
                                    ModifyProjectNum(ProPath, AddIndex);

                                }
                                PnumList.Add(AddIndex);

                                ListViewItem proRow = new ListViewItem(new string[] { var.Name, rowInfo.Split(',')[1] });
                                this.ProListBox.Items.Add(proRow);
                            }
                            //else
                            //{
                            //    CassMessageBox.Warning("加载工程时部分页面加载失败！");
                            //}

                        }
                    }
                    catch
                    { continue; }
                }
            }
            else //当默认工作目录不存在时，自动创建一个
            {
                Directory.CreateDirectory(WorkSpacePath);

            }
        }


        private void OpenProject(string pathName)
        {
            bool AddComplete = true;          //成功加载所有页面
            try
            {
                //若已存在项目，则提示是否保存原有项目，否则直接将项目在资源文件中删掉
                if (solutionTreeView.Nodes.Count > 0)
                {
                    DialogResult result = CassMessageBox.Question("是否保存原有项目？");
                    if (result == DialogResult.Yes)
                    {    
                        saveProjectOperation();  //调用保存工程程序
                        CassViewGenerator.currentTabPage = null;
                    }
                }
                //清空原有资源管理器
                ClearResource();

                //检查名称是否合法
                string FileName = Path.GetFileNameWithoutExtension(pathName);
                if (JudgeName(FileName, "打开的工程文件名"))
                {//更新工程名和工程序号
                    ProjectName = FileName;
                    //ProjectNum = this.ProListBox.Items.IndexOf(ProjectName).ToString();
                    saveName = Path.GetFileName(pathName);
                    savePath = Path.GetDirectoryName(pathName);
                    CassViewGenerator.currentProjectPath = savePath;  //2013.11.22
                    this.Text = this.Text.Split('_')[0] + "_" + ProjectName;//显示当前工程名
                }
                else
                { return; }

                AddComplete = AddTreeNode(pathName);

                //加载main页面
                if (AddComplete == false)
                {
                    CassMessageBox.Warning("加载工程时部分页面加载失败！");
                }
                else
                {
                    this.tabList.Add((ListObject)nodeList[0]);
                    this.tabControlView.TabPages.Add(((ListObject)nodeList[0]).tabPage);
                    this.tabControlView.SelectedIndex = tabList.Count - 1;
                    CassViewGenerator.currentTabPage = this.tabControlView.SelectedTab;
                    GetCurrentUndo();

                    CurrentDocumentsHostControl.HostDesign.SelectionControl();//将属性表信息锁定为当前工程页面
                    this.dockManager.ShowContent(this.tabSolution_Property);//显示属性表
                    toolStripMenuItem_Property.Checked = true;//工具栏按钮选择
                }
                if (solutionTreeView.Nodes.Count > 0)
                {
                    solutionTreeView.SelectedNode = solutionTreeView.Nodes[0];
                }
                else  //发生异常情况，如资源管理文件中的行数小于2
                {
                    TreeNode projectNode = CreateNode(Path.GetFileNameWithoutExtension(saveName), this.treeMenuPage);
                    solutionTreeView.Nodes.Add(projectNode);
                }
                UpdateCurrentPro();
            }
            catch (Exception ex) { }
            finally
            {
                if (solutionTreeView.Nodes.Count > 0)
                {
                    saveProject.Enabled = true;
                    saveProjecttoolStripButton.Enabled = true;
                    saveProjecttoolStripButton.Enabled = true;
                    closeProjecttoolStripMenuItem.Enabled = true;
                    addItemtoolStripSplitButton.Enabled = true;


                    solutionTreeView.Nodes.Add(CodePageName);//20090531

                    //对文件菜单中的新建中的新建文件夹和设界面进行始能操作
                    designViewStripMenu.Enabled = true;
                    //状态栏显示图形编辑模式
                 //   ModeSelect(CompileMode.ConfigMode);
                    //编辑模式一致
                    SetEditMode();
                }
            }
        }

        /// <summary>
        /// 读入工程文件，创建对应的页面
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns>是否成功加载所有页面</returns>
        private bool AddTreeNode(string FilePath)
        {
            bool returnBool = true;//返回值，判定是否出现加载页面失败
            string viewFullName = null;       //记录节点的完整路径 ，带.xml
            FileStream fStream = new FileStream(FilePath, FileMode.Open);  //读工程文件内容
            StreamReader sReader = new StreamReader(fStream);              //读取字符

            //空工程处理，则只建立一个以工程名为节点的名称
            if (fStream.Length == 0)
            { //readFileName是工程名字,但还需要去掉.caproj，新建一个工程名节点
                TreeNode projectNode = CreateNode(Path.GetFileNameWithoutExtension(FilePath), this.treeMenuPage);

                solutionTreeView.Nodes.Add(projectNode);
                closeProjecttoolStripMenuItem.Enabled = true;
                addItemtoolStripSplitButton.Enabled = true;
                sReader.Close();
                fStream.Close();
                return false;
            }

            while ((viewFullName = sReader.ReadLine()) != null && viewFullName.Trim() != "")
            {
                string tempPath = Path.Combine(savePath, viewFullName);
                if (File.Exists(tempPath))//目录中存在对应文件
                {
                    if (!viewFullName.Contains("\\"))
                    {//不包含"\\"的判定为主页面
                        MainPageName = Path.GetFileNameWithoutExtension(viewFullName);
                    }

                    HostControl hostControl = hostDesignManage.GetNewDesignHost(tempPath);
                    if (hostControl != null && hostControl.LoadBool == true)
                    { //创建对应目录的页面
                        System.Windows.Forms.TabPage tabpage
                            = new System.Windows.Forms.TabPage(Path.GetFileNameWithoutExtension(viewFullName));

                        hostControl.Parent = tabpage;
                        hostControl.Dock = DockStyle.Fill;

                        string[] nodePathArray = viewFullName.Split('.')[0].Split('\\');//去尾部扩展名，并分级

                        //读取完整的名称，创建节点
                        if (nodePathArray != null && nodePathArray.Length != 0)
                        {
                            for (int i = 0; i < nodePathArray.Length; i++)
                            {
                                TreeNode currentNode = CreateNode(nodePathArray[i], this.treeMenuPage);


                                if (i != nodePathArray.Length - 1)
                                {
                                    currentNode.Tag = "File";
                                }
                                if (solutionTreeView.Nodes.Count == 0 && i == 0)
                                {
                                    //currentNode.ContextMenuStrip = this.treeMenuProject;
                                    solutionTreeView.Nodes.Add(currentNode);
                                    solutionTreeView.SelectedNode = currentNode;
                                }
                                else
                                {
                                    TreeNode[] tempNode = solutionTreeView.Nodes.Find(nodePathArray[i], true);
                                    if (tempNode.Length != 0)
                                    {
                                        solutionTreeView.SelectedNode = tempNode[0];
                                    }
                                    else
                                    {
                                        solutionTreeView.SelectedNode.Nodes.Add(currentNode);
                                    }
                                }
                            }
                            solutionTreeView.ExpandAll();  //将节点展开

                            //添加到链表中
                            ListObject listObject = new ListObject();   //当前新建的设计页面的结构体变量
                            listObject.pathName = viewFullName.Split('.')[0];
                            listObject.tabPage = tabpage;
                            listObject.UndoAndRedo = new UndoOperation(tabpage);
                            nodeList.Add(listObject);
                        }
                    }//if (hostControl != null && hostControl.LoadBool == true)
                    else//页面加载失败
                    {
                        returnBool = false;
                    }
                }//if (File.Exists(savePath + "\\" + viewFullName))
                else if (viewFullName.Contains(","))
                {//第一行放工程序号和描述信息
                    ProjectNum = viewFullName.Split(',')[0];
                    ProjectInfo = viewFullName.Split(',')[1];
                }
            } //while ((viewFullName = sReader.ReadLine()) != null)}
            sReader.Close();
            fStream.Close();
            return returnBool;
        }

        #endregion

        #region 关闭工程

        /// <summary>
        /// 关闭工程菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeProjecttoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (solutionTreeView.Nodes.Count > 0)
            {
                DialogResult result = CassMessageBox.Question("是否保存项目？");//对话框时给出提示信息的操作结果
                if (result == DialogResult.Yes)
                {
                    saveProjectOperation();     //调用保存工程程序
                }
            }
            CassViewGenerator.currentTabPage = null;
            solutionTreeView.Nodes.Clear();     //工程管理目录清空
            nodeList.Clear();       //当前显示的设计页面链表清空
            tabList.Clear();        //当前设计页面链表清空
            tabControlView.TabPages.Clear();        //显示框内容清空

            //刷新属性控件，置空
            controlfilteredPropertyGrid.SelectedObject = null;
            controlfilteredPropertyGrid.Refresh();

            saveForm.Enabled = false;       //不可保存单个页面
            saveCurrentFormtoolStripButton.Enabled = false;     //不可保存当前页面  
            saveForm.Text = "保存";
            saveProject.Enabled = false;
            setEditEnable(false);       //不可编辑
            saveProjecttoolStripButton.Enabled = false;     //不可保存整个工程
            closeProjecttoolStripMenuItem.Enabled = false;      //不可关闭工程
            menuStrip.Refresh();
            addItemtoolStripSplitButton.Enabled = false;
            designViewStripMenu.Enabled = false;
            ToolBoxServiceImpl.typeNameString = null;
        }
        #endregion

        #endregion

        #region 工具栏事件

        //新建工程
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AddNewFile();
        }

        private void designItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewDesign();
        }

        private void existItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExistDesign();
        }

        private void saveCurrentFormtoolStripButton_Click(object sender, EventArgs e)
        {
            SaveCurrentForm();
        }

        private void saveProjecttoolStripButton_Click(object sender, EventArgs e)
        {
            saveProjectOperation();
        }

        //private void openProjecttoolStripButton_Click(object sender, EventArgs e)
        //{
        //    OpenProjects();
        //}

        private void ToolAction_Click(object sender, MouseEventArgs e)
        {
            PerformAction((sender as ToolStripButton).Text);
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AddNewFile();
        }

        private void CompliedtoolStripButton_Click(object sender, EventArgs e)
        {
            this.PreProcess(); //机器视觉预处理
            //Thread  compileThread = new Thread(Comply);
            //compileThread.IsBackground = true;
            //compileThread.Start();
            Comply();
           
        }



        private void ModetoolStripButton_Click(object sender, EventArgs e)
        {
            if (ProjectMode == "View")
            {//当前模式为图则切换至指令
                ModeSelect(CompileMode.ConfigMode);
            }
            else if (ProjectMode == "Code")
            {
                ModeSelect(CompileMode.ILMode);
            }
            else if (ProjectMode == "Simu")
            {
                ModeSelect(CompileMode.SimulationMode);
            }
        }

        #endregion

        #region 退出项目和关闭应用程序的操作

        /// <summary>
        /// 退出项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 关闭窗体前询问是否需要保存，当保存时调用保存工程函数saveProjectOperation()进行保存工程操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CassViewGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (CloseWindow())
            {
                case "yes":
                    {
                        //调用保存工程程序
                        saveProjectOperation();
                        this.Dispose();        //先释放资源
                        this.Close();//20090603便于加载至梯形图内部，关闭当前程序窗口
                        //Application.Exit();
                    }
                    break;
                case "no":
                    {
                        this.Dispose();        //先释放资源
                        this.Close();//20090603便于加载至梯形图内部，关闭当前程序窗口
                        //Application.Exit();
                    }
                    break;
                case "cancel":
                    {
                        e.Cancel = true;
                    }
                    break;
            }
        }

        private string CloseWindow()
        {
            DialogResult result;  //关闭对话框时给出提示信息的操作结果

            if (solutionTreeView.Nodes.Count > 0)
            {
                result = CassMessageBox.QuestionT("是否保存项目并确定退出程序？");
                if (result == DialogResult.Yes)
                {
                    return "yes";  //保存
                }
                else if (result == DialogResult.No)
                {
                    return "no";
                }
                else if (result == DialogResult.Cancel)
                {
                    return "cancel";
                }
            }
            else
            {
                result = CassMessageBox.Question("是否确定退出程序？");
                if (result == DialogResult.Yes)
                {
                    return "no";  //不保存
                }
                else if (result == DialogResult.No)
                {
                    return "cancel";
                }
            }
            return null;

        }

        #endregion

        #region 自动保存工程文件的时钟操作

        /// <summary>
        /// 自动保存所触发的事件，默认自动保存时间10分钟
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerClock_Tick(object sender, EventArgs e)
        {
            saveProjectOperation();
        }

        private void autoSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentDocumentsHostControl != null && CurrentDocumentsHostControl.HostDesign != null)
            {
                CurrentDocumentsHostControl.HostDesign.HostFocuse = true;
            }
            //第二个参数将时间从毫秒转换为分
            TimerParamentSet timerSet = new TimerParamentSet(timerClock.Enabled, timerClock.Interval / 60000);

            timerSet.ShowDialog();

            //显示设置文件后,timerSet提供一个修改标志,如果标志为ture。则重新修改自动保存工程的定时器属性
            if (timerSet.UpdateFlag == true)
            {
                timerClock.Interval = timerSet.TimerInterval * 1000 * 60; //从分转化为毫秒
                timerClock.Enabled = timerSet.TimerEnable;                //获得最新的定时器状态

            }
        }
        #endregion

        #region 属性控件的操作

        public static List<ArrayList> PortInfoList = new List<ArrayList>();        //页面各模块点名集合
        public static List<ArrayList> CodePInfoList = new List<ArrayList>();     //指令列表个模块点名集合
        public static string portIndex = "portnum";//拥有点名的控件才有的功能属性


        /// <summary>
        /// 调用SetActionInformation函数判断当前的对象是否需要进行动作设置
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">时间数据</param>
        private void controlfilteredPropertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            if (tabControlView.SelectedIndex >= 0
                //&& tabList.Count == tabControlView.TabCount//有指令表页面 删除该条件20090619
                && controlfilteredPropertyGrid.SelectedObject != null)
            {
                if (!controlfilteredPropertyGrid.SelectedObject.GetType().ToString().Equals(PublicVariable.viewName))  //不为CassView
                {
                    bringToFrontToolStripButton.Enabled = true;
                    sendToBackStripButton.Enabled = true;
                    CassView cassView = (CassView)(currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false));
                    Control control = (Control)(this.controlfilteredPropertyGrid.SelectedObject);

                    PropertyDescriptor moduleSortProperty = TypeDescriptor.GetProperties(control)["ModuleSort"];
                    if (moduleSortProperty != null)
                    {
                        string sortName = moduleSortProperty.GetValue(control).ToString();
                        if (sortName == "输入变量" || sortName == "输出变量")
                        {
                            SetPortnameList(control);//设置控件点名列表
                            SetPropertyList(control);//设置选定控件的属性列表
                        }
                        else if (sortName == "跳转")
                        {
                            PropertyDescriptor serialNumberProperty = TypeDescriptor.GetProperties(control)["SerialNumberNames"];
                            if (serialNumberProperty != null)
                            {
                                if (currentTabPage != null && currentTabPage.Controls[0] != null
                                    && currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false) != null)
                                { //修改列表 模块序号 如果是已经串联的 则为Bl：1，2，3
                                    List<string> ControlTarget = new List<string>();
                                    new GenerateCode(null).PackOrderCtrl(cassView, new string[] { currentTabPage.Text, null });
                                    //{//是否要排除包含自身的控件序列选项？？？？？？？？？     
                                    foreach (string[] element in GenerateCode.PackInfos)
                                    {
                                        ControlTarget.Add("Bl:" + element[1]);
                                    }
                                    serialNumberProperty.SetValue(control, ControlTarget);
                                }//end if (currentTabPage != null)
                            }//end if(serialNumberProperty != null)
                        }//end else if (sortName == "跳转")
                        else if (sortName == "调用")
                        {
                            PropertyDescriptor serialNumberProperty = TypeDescriptor.GetProperties(control)["SerialNumberNames"];
                            if (serialNumberProperty != null)
                            {
                                if (currentTabPage != null && currentTabPage.Controls[0] != null
                                    && currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false) != null)
                                {
                                    serialNumberProperty.SetValue(control, GetTacticList());
                                }
                            }
                        }
                        else if (sortName == "条件动作表")
                        {
                            PropertyDescriptor configurationProperty = TypeDescriptor.GetProperties(control)["Configuration"];
                            ControlTactic.SpecialControl.Process ConvertTool = new ControlTactic.SpecialControl.Process();
                            ControlTactic.SpecialControl.ProcessStruct tempStruct = ConvertTool.ListToStruct((List<string>)configurationProperty.GetValue(control));
                            tempStruct.Tactic = GetTacticList();
                            tempStruct.ControlAttribute = GetControlValue();
                            configurationProperty.SetValue(control, ConvertTool.StructToList(tempStruct));
                        }
                    }//end if (moduleSortProperty != null)
                }//end if (!controlfilteredPropertyGrid.SelectedObject.GetType().ToString().Equals(PublicVariable.viewName))
                else   //为CassView
                {
                    bringToFrontToolStripButton.Enabled = false;
                    sendToBackStripButton.Enabled = false;
                }//end else if (controlfilteredPropertyGrid.SelectedObject.GetType().ToString().Equals(PublicVariable.viewName))
            }//end if (tabControlView.SelectedIndex >= 0)
            else if (controlfilteredPropertyGrid.SelectedObject == null)
            {
                bringToFrontToolStripButton.Enabled = false;
                sendToBackStripButton.Enabled = false;
            }//end else if (controlfilteredPropertyGrid.SelectedObject == null)
        }

        /// <summary>
        /// 只有当前选中控件为“调用”或“条件控制器”时调用此方法，查找当前页面的所有子策略
        /// </summary>
        private List<string> GetTacticList()
        {
            List<string> ControlTarget = new List<string>();

            Stack<TreeNode> Tnode = new Stack<TreeNode>();//用来存放数节点的堆栈
            Tnode.Push(this.solutionTreeView.Nodes[0]);//放入树根
            while (true)
            {
                if (Tnode.Peek().Text == currentTabPage.Text)//堆栈顶为当前页面
                {
                    if (Tnode.Peek().Nodes.Count != 0)
                    {
                        foreach (TreeNode node in Tnode.Peek().Nodes)
                        {
                            if (node.Text.Split('.').Length != 2)//排除有后缀的树节点(.vw和.lt)
                            { ControlTarget.Add(node.Text); }
                        }
                    }
                    Tnode.Clear();
                    break;
                }
                else if (Tnode.Peek().Nodes.Count != 0)//堆栈顶部树节点有子节点
                {
                    TreeNode curNode = Tnode.Pop();
                    foreach (TreeNode node in curNode.Nodes)
                    {
                        Tnode.Push(node);
                    }
                }
                else//堆栈顶树节点不是当前页面也没有子节点
                    Tnode.Pop();
            }
            return ControlTarget;
        }

        /// <summary>
        /// 只有当前选中控件为“条件控制器”时调用此方法，查找当前页面的所有控件的可见属性
        /// </summary>
        private List<List<string>> GetControlValue()
        {
            CassView cassView = (CassView)(currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false));
            List<List<string>> tempControlValue = new List<List<string>>();
            List<string> Noneed = new List<string>(new string[] { "输入变量", "输出变量", "常数" });//占有PortName位置的控件20090619 需找地方统一定义
            foreach (ControlInfo ctrlinfo in cassView.ctrlsInfo)
            { //遍历当前cassview的所有控件
                if (ctrlinfo.CodeInfo != null && ctrlinfo.CodeInfo[2] != null //有PortName
                    && !Noneed.Contains(ctrlinfo.CodeInfo[0]))//排除个别占有PortName位置的控件
                {
                    List<string> tempValue = new List<string>();
                    tempValue.Add(ctrlinfo.CodeInfo[2]);//tempview第一位放控件的Portname

                    foreach (XProp element in ctrlinfo.VisibleFunctionProperty)
                    {//中文属性名,F英文属性名
                        tempValue.Add(element.Name + "," + element.VarName);
                    }
                    tempControlValue.Add(tempValue);
                }
            }
            return tempControlValue;
        }

        /// <summary>
        /// 判断字符串是否可化为数字
        /// </summary>
        /// <param name="numberString">被判断的字符串</param>
        /// <returns>返回True表示可以被可化为数字，返回False表示不可以被转化为数字</returns>
        public static bool IsNumber(string numberString)
        {
            try
            {
                int temp = System.Convert.ToInt32(numberString);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据当前控件的PortList修改ShowPortName属性
        /// </summary>
        /// <param name="CassControl"></param>
        /// <param name="CurrentCass"></param>
        private void SetPortnameList(Control CassControl)
        {
            PropertyDescriptor portInfoListProperty = TypeDescriptor.GetProperties(CassControl)["PortInfoList"];
            if (portInfoListProperty != null)
            {
                List<string> portnameList = new List<string>();
                foreach (ListObject node in nodeList)
                {
                    CassView cassView = (CassView)(node.tabPage.Controls[0].GetNextControl(node.tabPage.Controls[0], false));
                    foreach (ControlInfo element in cassView.ctrlsInfo)
                    {
                        if (element.CodeInfo != null && element.CodeInfo.Length == 3
                            && element.VisibleFunctionProperty != null && element.VisibleFunctionProperty.Count != 0)
                        {
                            portnameList.Add(element.CodeInfo[2]);
                        }
                    }
                }
                portInfoListProperty.SetValue(CassControl, portnameList);
            }
        }

        /// <summary>
        /// 根据当前控件的PortName修改ShowProtertyName属性
        /// </summary>
        /// <param name="CassControl">修改的目标控件</param>
        /// <param name="CurrentCass">控件所在的Cassview</param>
        static public void SetPropertyList(Control CassControl)
        {
            PropertyDescriptor choosePortNameProperty = TypeDescriptor.GetProperties(CassControl)["ChoosePortName"];
            PropertyDescriptor ShowProtertyName = TypeDescriptor.GetProperties(CassControl)["ShowProtertyName"];
            if (choosePortNameProperty != null)
            {
                string chooseportName = choosePortNameProperty.GetValue(CassControl).ToString();
                List<string[]> ShowPropertyList = new List<string[]>();
                if (chooseportName != "" && ShowProtertyName != null)
                {
                    foreach (ListObject node in nodeList)
                    {
                        CassView cassView = (CassView)(node.tabPage.Controls[0].GetNextControl(node.tabPage.Controls[0], false));
                        foreach (ControlInfo Cctrl in cassView.ctrlsInfo)
                        {
                            if (Cctrl.CodeInfo[2] == chooseportName)
                            {
                                foreach (XProp property in Cctrl.VisibleFunctionProperty)
                                {
                                    if (property.VarName != CassViewGenerator.portIndex)
                                    {//20090616改成中文属性和英文属性数组
                                        ShowPropertyList.Add(new string[] { property.Name, property.VarName });
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                ShowProtertyName.SetValue(CassControl, ShowPropertyList);//当PortName为空时清空ShowPropertyName控件属性
            }
        }

        /// <summary>
        /// 控件属性发生改变事件处理函数
        /// </summary>
        /// <param name="s">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void controlfilteredPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
          //  string controlName = e.OldValue.ToString();//*
            
            //获取当前操作所在的CASSVIEW以及操作对象控件
            CassView cassView = (CassView)(currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false));
            Control control = (Control)(this.controlfilteredPropertyGrid.SelectedObject);
         //   controlName= control.Tag.ToString();//*
            if (e.ChangedItem.PropertyDescriptor.DisplayName == "模块点名")
            {
                string Cmname = TypeDescriptor.GetProperties(control)["ModuleName"].GetValue(control).ToString();//当前控件模块名

                if (!CassView.ModifyPortName(Cmname, e.OldValue.ToString(), e.ChangedItem.Value.ToString()))
                {//有重名替换新名失败
                    e.ChangedItem.PropertyDescriptor.SetValue(this.controlfilteredPropertyGrid.SelectedObject, e.OldValue);
                    CassMessageBox.Information("已经存在相同的模块点名,修改失败！");
                }
                else
                {//更新对应的控件信息中存放的控件点名 并更新控件中隐藏的点名属性
                    if (!CassViewGenerator.isUndo)
                    {//属性修改操作
                        Operation PnOpt = new Operation();
                        PnOpt.Oname = UndoOperation.OperateStyle.属性修改.ToString();
                        PnOpt.Item = control;
                        PnOpt.Change = new object[3];
                        PnOpt.Change[0] = "模块点名";
                        PnOpt.Change[1] = e.OldValue;
                        PnOpt.Change[2] = e.ChangedItem.Value.ToString();
                        CassViewGenerator.AddOpt(PnOpt);
                        StartUndo();//撤销启动
                    }
                    PropertyDescriptor portNameProperty = TypeDescriptor.GetProperties(control)["PortName"];
                    cassView.FindControlInfo(control).CodeInfo[2] = Cmname + e.ChangedItem.Value.ToString();
                    portNameProperty.SetValue(control, Cmname + e.ChangedItem.Value.ToString());
                }
            }
            else if (e.ChangedItem.PropertyDescriptor.Category == PublicVariable.ControlFuntionName)
            {
                if (currentTabPage != null && currentTabPage.Controls[0] != null
                    && currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false) != null
                    && e.ChangedItem.Value != null)//非空20090621
                {
                    SpecialValueChange(cassView, control, e.ChangedItem.PropertyDescriptor.DisplayName.ToString(), e.OldValue, e.ChangedItem.Value);
                    
                    if (CassViewGenerator.currentUndo != null && !CassViewGenerator.isUndo)
                    {
                        Operation propertyOpt = new Operation();
                        propertyOpt.Oname = UndoOperation.OperateStyle.属性修改.ToString();
                        propertyOpt.Item = control;
                        propertyOpt.Change = new object[3];
                        propertyOpt.Change[0] = e.ChangedItem.PropertyDescriptor.DisplayName.ToString();
                        propertyOpt.Change[1] = e.OldValue;
                        propertyOpt.Change[2] = e.ChangedItem.Value;
                        CassViewGenerator.AddOpt(propertyOpt);
                        StartUndo();//撤销启动
                    }
                    SetCodeInfo(cassView, control, e.ChangedItem.PropertyDescriptor.DisplayName.ToString(), e.ChangedItem.Value.ToString());
                }
            }
        }

        /// <summary>
        /// 处理解耦和模糊控件的组态属性修改
        /// </summary>
        /// <param name="cassView"></param>
        /// <param name="control"></param>
        /// <param name="ChangeItem"></param>
        /// <param name="OldValue"></param>
        /// <param name="ChangeValue"></param>
        static public void SpecialValueChange(CassView cassView, Control control, string ChangeItem, object OldValue, object ChangeValue)
        {
            if (ChangeItem == "解耦路数")
            {//当修改解耦路数时修改组态属性对应的所有信息20090610
                ModifyCLinfo(cassView, control, Convert.ToInt32(OldValue), Convert.ToInt32(ChangeValue));
            }
            else if (ChangeItem == "解耦控制组态")
            {//对解耦路数赋值
                ControlInfo JieOu = cassView.FindControlInfo(control);
                for (int i = 0; i < JieOu.VisibleFunctionProperty.Count; i++)
                {
                    if (JieOu.VisibleFunctionProperty[i].Name == "解耦路数")
                    {//解耦中可视属性第一个即序号[0]为解耦路数
                        ModifyCLinfo(cassView, control, Convert.ToInt32(JieOu.VisibleFunctionProperty[i].TheValue), Convert.ToInt32(((List<string>)ChangeValue)[0]));
                        JieOu.VisibleFunctionProperty[i].TheValue = ((List<string>)ChangeValue)[0];
                        break;
                    }
                }
            }
            if (ChangeItem == "偏差模糊论域" || ChangeItem == "偏差变化率模糊论域")
            {//当修改模糊论域时修改组态属性对应的所有信息20090708
                SetFzConfigInfo(control, ChangeItem, Convert.ToInt32(OldValue), Convert.ToInt32(ChangeValue));
            }
            else if (ChangeItem == "模糊控制表")
            {
                ControlInfo Fuzzy = cassView.FindControlInfo(control);
                for (int i = 0; i < Fuzzy.VisibleFunctionProperty.Count; i++)
                {
                    if (Fuzzy.VisibleFunctionProperty[i].Name == "偏差模糊论域")
                    {//更新不可见属性中的模糊论域(值为行数/2,位置为不可见属性第[1]位)
                        Fuzzy.VisibleFunctionProperty[i].TheValue = ((List<string>)ChangeValue)[0];
                    }
                    else if (Fuzzy.VisibleFunctionProperty[i].Name == "偏差变化率模糊论域")
                    {//模糊变化域(值为列数/2,位置为不可见属性第[2]位)20090611
                        Fuzzy.VisibleFunctionProperty[i].TheValue = ((List<string>)ChangeValue)[1];
                    }
                }
            }
        }

        /// <summary>
        /// 更改解耦数目时解耦组态信息进行修改
        /// </summary>
        /// <param name="control">解耦控件</param>
        /// <param name="oldValue">修改前的解耦数</param>
        /// <param name="newValue">修改后的解耦数</param>
        static private void SetJoConfigInfo(Control control, int oldValue, int newValue)
        {
            PropertyDescriptor configurationProperty = TypeDescriptor.GetProperties(control)["Configuration"];
            ControlTactic.SpecialControl.JieOu ConvertTool = new ControlTactic.SpecialControl.JieOu();
            ControlTactic.SpecialControl.JieOuStruct tempStruct = ConvertTool.ListToStruct((List<string>)configurationProperty.GetValue(control));
            tempStruct.JieOuNum = newValue;//结构体中的解耦路数

            //结构体中每路解耦信息
            List<List<string>> tempAttribute = new List<List<string>>();
            for (int count = 0; count < tempStruct.JieOuNum; count++)
            {
                if (count > oldValue - 1)
                {
                    tempAttribute.Add(ControlTactic.SpecialControl.JieOuForm1.Original);//添加新解耦信息
                }
                else
                {
                    tempAttribute.Add(tempStruct.JieOuAttribute[count]);
                }
            }
            tempStruct.JieOuAttribute = tempAttribute;
            //结构体中的解耦表
            string[,] tempTable = new string[tempStruct.JieOuNum, tempStruct.JieOuNum];
            for (int i = 0; i < tempStruct.JieOuNum; i++)
            {
                for (int j = 0; j < tempStruct.JieOuNum; j++)
                {
                    if (i > oldValue - 1 || j > oldValue - 1)
                    { tempTable[i, j] = "0"; }
                    else
                    {
                        tempTable[i, j] = tempStruct.JieOuTable[i, j];
                    }
                }
            }
            tempStruct.JieOuTable = tempTable;
            configurationProperty.SetValue(control, ConvertTool.StructToList(tempStruct));
        }

        /// <summary>
        /// 修改解耦路数时需要对应修改解耦控件相关的信息
        /// </summary>
        /// <param name="cassView">控件所在容器</param>
        /// <param name="control">控件</param>
        /// <param name="oldValue">修改前解耦路数</param>
        /// <param name="newValue">修改后解耦路数</param>
        static private void ModifyCLinfo(CassView cassView, Control control, int oldValue, int newValue)
        {
            ControlInfo JieOu = cassView.FindControlInfo(control);
            if (newValue >= 2 && newValue <= 10)
            {//解耦路数必须大于等于2否则还原
                //修改控件信息中解耦的输出输入端口数目
                PropertyDescriptor configurationPropertyI = TypeDescriptor.GetProperties(control)["InputNum"];
                PropertyDescriptor configurationPropertyO = TypeDescriptor.GetProperties(control)["OutputNum"];
                configurationPropertyI.SetValue(control, newValue);
                configurationPropertyO.SetValue(control, newValue);
                JieOu.InputInfo = CassView.InitializeIOinfo(newValue, JieOu.InputInfo);//解耦控件没有端口初值20090616
                JieOu.OutputInfo = CassView.InitializeIOinfo(newValue);
                for (int i = 0; i < cassView.ctrlsInfo.Count; i++)
                {//将修改后的控件信息覆盖原有的控件信息
                    if (cassView.ctrlsInfo[i].ControlNum == JieOu.ControlNum)
                    {
                        cassView.ctrlsInfo[i] = JieOu;
                    }
                }
                if (oldValue > newValue)
                {//当修改后解耦路数小于原先，则删除缩减端口对应的直线
                    for (int i = cassView.linesInfo.Count - 1; i >= 0; i--)//从后往前删线信息
                    {
                        if (((string)(control.Tag) == cassView.linesInfo[i].StartInfo.Split(';')[0]
                            && (Convert.ToInt32(cassView.linesInfo[i].StartInfo.Split(',')[1]) >= newValue))
                            || (((string)(control.Tag) == cassView.linesInfo[i].EndInfo.Split(';')[0])
                            && Convert.ToInt32(cassView.linesInfo[i].EndInfo.Split(',')[1]) >= newValue))
                        {//直线的输入或输出信息对应当前控件且端口号大于修改后的值则删除
                            cassView.DeleteLine(i, false);
                        }
                    }
                }
                cassView.portReflash();//刷新端口及连线信息
                cassView.DrawBackgroundImage();//刷新背景图
                SetJoConfigInfo(control, Convert.ToInt32(oldValue), newValue);//修改控件端口外观
            }
            else
            {//还原端口旧值
                JieOu.VisibleFunctionProperty[0].TheValue = (oldValue);
                MessageBox.Show("解耦路数必须大于等于2小于等于10！");
            }
        }

        /// <summary>
        /// 更改模糊控制中
        /// 偏差模糊论域和偏差变化率模糊论域
        /// 时模糊控制信息进行修改
        /// </summary>
        /// <param name="control">模糊控件</param>
        /// <param name="oldValue">修改前的论域</param>
        /// <param name="newValue">修改后的论域</param>
        static private void SetFzConfigInfo(Control control, string atriName, int oldValue, int newValue)
        {
            PropertyDescriptor configurationProperty = TypeDescriptor.GetProperties(control)["Configuration"];
            ControlTactic.SpecialControl.Fuzzy ConvertTool = new ControlTactic.SpecialControl.Fuzzy();
            ControlTactic.SpecialControl.FuzzyStruct tempStruct = ConvertTool.ListToStruct((List<string>)configurationProperty.GetValue(control));

            if (atriName == "偏差模糊论域")
            {
                tempStruct.RowNum = newValue;
            }
            else if (atriName == "偏差变化率模糊论域")
            {
                tempStruct.ColumnNum = newValue;
            }


            //结构体中的模糊控制表
            string[,] tempTable = new string[tempStruct.RowNum * 2 + 1, tempStruct.ColumnNum * 2 + 1];
            for (int i = 0; i < tempStruct.RowNum * 2 + 1; i++)
            {
                for (int j = 0; j < tempStruct.ColumnNum * 2 + 1; j++)
                {
                    if ((atriName == "偏差模糊论域" && i > oldValue - 1)
                        || (atriName == "偏差变化率模糊论域" && j > oldValue - 1))
                    { tempTable[i, j] = "0"; }
                    else
                    {
                        tempTable[i, j] = tempStruct.ControlTable[i, j];
                    }
                }
            }
            tempStruct.ControlTable = tempTable;
            configurationProperty.SetValue(control, ConvertTool.StructToList(tempStruct));
        }

        /// <summary>
        /// 设置控件信息中的CodeInfo属性,用于指令生成
        /// </summary>
        /// <param name="CurrentCass"></param>
        /// <param name="control"></param>
        /// <param name="ChangeItem"></param>
        /// <param name="ChangeValue"></param>
        static public void SetCodeInfo(CassView CurrentCass, Control control, string ChangeItem, string ChangeValue)
        {
            ControlInfo ctrlinfo = CurrentCass.ctrlsInfo[CurrentCass.FindControlName(control.Tag.ToString())];
            //ControlInfo ctrlinfo = CurrentCass.FindControlInfo(control);
            if (ctrlinfo.CodeInfo[0] == "输入变量" || ctrlinfo.CodeInfo[0] == "输出变量" || ctrlinfo.CodeInfo[0] == "返回")
            {//特殊一类功能属性
                if (ChangeItem == "选择点名")
                {
                    ctrlinfo.CodeInfo[1] = ChangeValue;
                    ctrlinfo.CodeInfo[2] = null;//更改了点名则清空属性信息
                    SetPropertyList(control);
                }
                else if (ChangeItem == "选择属性")
                {
                    ctrlinfo.CodeInfo[2] = ChangeValue.Split('(')[0];//去掉末尾括号中的中文属性名
                }
            }
                //2013.11.19
            else if (ctrlinfo.CodeInfo[0] == "常数" || ctrlinfo.CodeInfo[0] == "系统变量" || ctrlinfo.CodeInfo[0] == "跳转" || ctrlinfo.CodeInfo[0] == "调用" ||ctrlinfo.CodeInfo[0].Contains( "文件")||ctrlinfo.CodeInfo[0]=="特征输入"/*||ctrlinfo.CodeInfo[0]=="设备输入"*/)
            {//特殊二类功能属性
                if (ChangeItem == "数据类型" || ChangeItem == "变量类型")//常数控件的第二个属性||系统变量的第一个属性
                {
                    ctrlinfo.CodeInfo[2] = ChangeValue;
                }
                else if (ctrlinfo.CodeInfo[0] == "跳转" && ChangeItem == "所跳转目标" && ChangeValue.Contains(":"))
                {//跳转控件分别存放条件序号和序号名
                    string temp = ChangeValue.Split(':')[1];//BL:后的跳转序列
                    foreach (string[] valueArray in GenerateCode.PackInfos)
                    {
                        if (valueArray[1] == temp)
                        {//找到序列对应的串名赋值
                            ctrlinfo.CodeInfo[1] = valueArray[0];
                            break;
                        }
                    }
                }
                else//此类其他控件的唯一属性和常数的第一个属性
                {
                    ctrlinfo.CodeInfo[1] = ChangeValue;
                }
            }
            else//其他功能属性
            {
                for (int i = 0; i < CurrentCass.ctrlsInfo.Count; i++)
                {
                    if (CurrentCass.ctrlsInfo[i].ControlName == control.Site.Name && CurrentCass.ctrlsInfo[i].VisibleFunctionProperty != null)
                    {
                        for (int j = 0; j < CurrentCass.ctrlsInfo[i].VisibleFunctionProperty.Count; j++)
                        {
                            if (CurrentCass.ctrlsInfo[i].VisibleFunctionProperty[j].Name == ChangeItem)
                            {
                                CurrentCass.ctrlsInfo[i].VisibleFunctionProperty[j].TheValue = ChangeValue;
                            }
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 控件属性选项卡发生改变事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void controlfilteredPropertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection != null && e.NewSelection.PropertyDescriptor != null
                && e.NewSelection.PropertyDescriptor.DisplayName != null)
            {
                if (currentTabPage != null && currentTabPage.Controls[0] != null
                    && currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false) != null)
                {
                    CassView cassView = (CassView)(currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false));
                    if (this.controlfilteredPropertyGrid.SelectedObject != null)
                    {
                        Control control = (Control)(this.controlfilteredPropertyGrid.SelectedObject);
                        int ControlIndex = cassView.FindControlName(control.Site.Name);
                        if (ControlIndex != -1 && cassView.ctrlsInfo[ControlIndex].HasEnumProperty)
                        {
                            for (int i = 0; i < cassView.ctrlsInfo[ControlIndex].VisibleFunctionProperty.Count; i++)
                            {
                                if (cassView.ctrlsInfo[ControlIndex].VisibleFunctionProperty[i].Name
                                    == e.NewSelection.PropertyDescriptor.DisplayName)
                                {
                                    //系统串口 2014.1
                                    if (cassView.ctrlsInfo[ControlIndex].VisibleFunctionProperty[i].ValueType == "COM")
                                    {
                                        CassViewGenerator.chooseString = CassView.GetSystemSerialPortNames();
                                    }
                                    else
                                    {
                                        CassViewGenerator.chooseString = cassView.ctrlsInfo[ControlIndex].VisibleFunctionProperty[i].EnumValue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型。
        /// 如果类型转换器支持该类型的转换,则返回true
        /// </summary>
        private bool GetConversionSupported(TypeConverter converter, Type conversionType)
        {
            return (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType));
        }

        #endregion

        #region 菜单栏和工具栏中对应项的操作等

        /// <summary>
        /// 视图的菜单状态发生变化时 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aliginToolStripMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            leftAligntoolStripButton.Enabled = aliginToolStripMenuItem.Enabled;
            rightAligntoolStripButton.Enabled = aliginToolStripMenuItem.Enabled;
            middleAligntoolStripButton.Enabled = aliginToolStripMenuItem.Enabled;
            topAligntoolStripButton.Enabled = aliginToolStripMenuItem.Enabled;
            centerAligntoolStripButton.Enabled = aliginToolStripMenuItem.Enabled;
            buttomsAligntoolStripButton.Enabled = aliginToolStripMenuItem.Enabled;

            leftsToolStripMenuItem.Enabled = aliginToolStripMenuItem.Enabled;
            centersToolStripMenuItem.Enabled = aliginToolStripMenuItem.Enabled;
            rightsToolStripMenuItem.Enabled = aliginToolStripMenuItem.Enabled;
            topsToolStripMenuItem.Enabled = aliginToolStripMenuItem.Enabled;
            middlesToolStripMenuItem.Enabled = aliginToolStripMenuItem.Enabled;
            bottomsToolStripMenuItem.Enabled = aliginToolStripMenuItem.Enabled;

        }

        /// <summary>
        /// 撤消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undotoolStripMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            undotoolStripButton.Enabled = undotoolStripMenuItem.Enabled;
            undoMenuItem.Enabled = undotoolStripMenuItem.Enabled;
        }

        /// <summary>
        /// 重复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redotoolStripMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            redotoolStripButton.Enabled = redotoolStripMenuItem.Enabled;
            redoMenuItem.Enabled = redotoolStripMenuItem.Enabled;
        }

        /// <summary>
        /// 剪切
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cutToolStripMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            cuttoolStripButton.Enabled = cutToolStripMenuItem.Enabled;
            cutMenuItem.Enabled = cutToolStripMenuItem.Enabled;
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            copytoolStripButton.Enabled = copyToolStripMenuItem.Enabled;
            copyMenuItem.Enabled = copyToolStripMenuItem.Enabled;

        }

        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteToolStripMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            pastetoolStripButton.Enabled = pasteToolStripMenuItem.Enabled;
            pasteMenuItem.Enabled = pasteToolStripMenuItem.Enabled;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteControlToolStripMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            deleteMenuItem.Enabled = deleteControlToolStripMenuItem.Enabled;
        }

        /// <summary>
        /// 新建工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            newProjectToolStripMenuItem.Enabled = projectMenuItem.Enabled;
        }


        /// <summary>
        /// 保存当前页 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveForm_EnabledChanged(object sender, EventArgs e)
        {
            saveCurrentFormtoolStripButton.Enabled = saveForm.Enabled;
        }

        /// <summary>
        /// 保存整个工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveProject_EnabledChanged(object sender, EventArgs e)
        {
            saveProjecttoolStripButton.Enabled = saveProject.Enabled;
        }

        /// <summary>
        /// 编辑菜单使能操作 
        /// </summary>
        /// <param name="flag"></param>
        private void setEditEnable(bool flag)
        {
            if (flag == false)
            {
                undotoolStripMenuItem.Enabled = false;
                redotoolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
                copyToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
                selectAllToolStripMenuItem.Enabled = false; //全选
                deleteControlToolStripMenuItem.Enabled = false;
                aliginToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// 控件选择按钮单击事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void linkToolStripButton1_Click(object sender, EventArgs e)
        {
            this.linkToolStripButton.Enabled = false;
            this.editToolStripButton.Enabled = true;
            SetEditMode();
        }

        /// <summary>
        /// 控件选择按钮单击事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void editToolStripButton2_Click(object sender, EventArgs e)
        {
            this.editToolStripButton.Enabled = false;
            this.linkToolStripButton.Enabled = true;
            SetEditMode();
        }

        /// <summary>
        /// 设置编辑模式
        /// 根据linkToolStripButton来判断是直线编辑还是空间编辑
        /// </summary>
        private void SetEditMode()
        {
            this.startCountPoint = this.linkToolStripButton.Enabled;
            Control tempOBJ = null;
            for (int i = 0; i < this.tabList.Count; i++)
            {
                tempOBJ = ((ListObject)this.tabList[i]).tabPage.Controls[0];
                if (tempOBJ != null)
                {
                    tempOBJ = tempOBJ.GetNextControl(tempOBJ, false);
                    if (tempOBJ != null)
                    {
                        ((CassView)tempOBJ).LinesEditable = this.linkToolStripButton.Enabled;
                        ((CassView)tempOBJ).ChoosedLine = -1;
                        ((CassView)tempOBJ).IsPortPoint = false;
                    }
                }
            }
        }

        /// <summary>
        /// 控制模块菜单栏鼠标单击事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void toolStripMenuItem_Controls_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem_Controls.Checked == false)
            {
                this.dockManager.HideContent(this.tabSolution_Controls);
            }
            else
            {
                this.dockManager.ShowContent(this.tabSolution_Controls);
            }
        }

        /// <summary>
        /// 模块属性菜单栏鼠标单击事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void toolStripMenuItem_Property_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem_Property.Checked == true)
            {
                this.dockManager.ShowContent(this.tabSolution_Property);
            }
            else
            {
                this.dockManager.HideContent(this.tabSolution_Property);
            }
        }

        /// <summary>
        /// 资源管理器菜单栏鼠标单击事件处理函数 20140218
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void toolStripMenuItem_TreeView_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem_TreeView.Checked == true)
            {
               // this.dockManager.ShowContent(this.rig);
               
                this.rightTopTabList.TabPages.Add(this.tab_SolutionManager);
                if (this.rightTopTabList.TabPages.Count > 0)
                {
                    this.dockManager.ShowContent(this.rightTopContent_TabControl);
                }
                
            }
            else
            {
             //   this.dockManager.HideContent(this.tabSolution_TreeView);
               
                this.rightTopTabList.TabPages.Remove(this.tab_SolutionManager);
                if (this.rightTopTabList.TabPages.Count < 1)
                {
                    this.dockManager.HideContent(this.rightTopContent_TabControl);
                }
                this.toolStripMenuItem_TreeView.Checked = false;
            }
        }

        /// <summary>
        /// 错误列表菜单栏鼠标单击事件处理函数 20140218
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void toolStripMenuItem_ErrorForm_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem_ErrorForm.Checked == true)
            {
               // this.dockManager.HideContent(this.tabSolution_ErrorBox);
                this.bottomTabList.TabPages.Add(this.tab_ErrorFormManager);
                if (this.bottomTabList.TabPages.Count > 0)
                    this.dockManager.ShowContent(this.bottomContent_TabControl);
            }
            else
            {
              //  this.dockManager.ShowContent(this.tabSolution_ErrorBox);
                this.bottomTabList.TabPages.Remove(this.tab_ErrorFormManager);
                if (this.bottomTabList.TabPages.Count < 1)
                    this.dockManager.HideContent(this.bottomContent_TabControl);
                this.toolStripMenuItem_ErrorForm.Checked = false;
            }
        }

        /// <summary>
        /// 效果图菜单栏鼠标单击事件处理函数 20140218
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_EffectPic_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem_EffectPic.Checked == true)
            {
                this.bottomTabList.TabPages.Add(this.tab_EffectPicManager);
                if (this.bottomTabList.TabPages.Count > 0)
                    this.dockManager.ShowContent(this.bottomContent_TabControl);
            }
            else
            {
                this.bottomTabList.TabPages.Remove(this.tab_EffectPicManager);
                if (this.bottomTabList.TabPages.Count < 1)
                    this.dockManager.HideContent(this.bottomContent_TabControl);
                this.toolStripMenuItem_EffectPic.Checked = false;
               
            }
        }

        /// <summary>
        /// 工程管理器菜单栏 20140218
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_PorList_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem_PorList.Checked == true)
            {
               // this.dockManager.ShowContent(this.tabSolution_ListBox);              
                this.rightTopTabList.TabPages.Add(this.tab_ProjectManager);
                if (this.rightTopTabList.TabPages.Count > 0)
                {
                    this.dockManager.ShowContent(this.rightTopContent_TabControl);
                }
            }
            else
            {
              //  this.dockManager.HideContent(this.tabSolution_ListBox);
                this.rightTopTabList.TabPages.Remove(this.tab_ProjectManager);
                if (this.rightTopTabList.TabPages.Count < 1)
                {
                    this.dockManager.HideContent(this.rightTopContent_TabControl);
                }
                this.toolStripMenuItem_PorList.Checked = false;
            }
        }

        /// <summary>
        /// 放大图标单击事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void zoomtoolStripButton_Click(object sender, EventArgs e)
        {
            if (currentTabPage != null && currentTabPage.Controls[0] != null
                && currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false) != null)
            {
                StartUndo();//启动撤销
                Control obj = currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false);
                ((CassView)obj).Scaling = ((CassView)obj).Scaling + 0.2F;
                if (((CassView)obj).Scaling + 0.2F > 2.0F)
                {
                    zoomtoolStripButton.Enabled = false;
                }
                if (((CassView)obj).Scaling > 0.2F)
                {
                    reducetoolStripButton.Enabled = true;
                }
            }
        }

        /// <summary>
        /// 缩小图标单击事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void reducetoolStripButton_Click(object sender, EventArgs e)
        {
            if (currentTabPage != null && currentTabPage.Controls[0] != null
                && currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false) != null)
            {
                StartUndo();//启动撤销
                Control obj = currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false);
                ((CassView)obj).Scaling = ((CassView)obj).Scaling - 0.2F;

                if (((CassView)obj).Scaling - 0.2F < 0.2F)
                {
                    reducetoolStripButton.Enabled = false;
                }
                if (((CassView)obj).Scaling < 2.0F)
                {
                    zoomtoolStripButton.Enabled = true;
                }
            }
        }

        /// <summary>
        /// 显示隐藏控件序号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectToolStripMenuItem_ShowNum_Click(object sender, EventArgs e)
        {
            SetShowNumber();
        }

        /// <summary>
        /// 编译
        /// 生成地址XML,工程XML,CodeXML,DateXML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectToolStripMenuItem_Complied_Click(object sender, EventArgs e)
        {
            Comply();
        }

        /// <summary>
        /// 选择图形组态模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_Config_Click(object sender, EventArgs e)
        {
            ModeSelect(CompileMode.ConfigMode);
        }

        /// <summary>
        /// 选择指令模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_IL_Click(object sender, EventArgs e)
        {
            ModeSelect(CompileMode.ILMode);
        }
        //private void projectToolStripMenuItem_CircleTime_Click(object sender, EventArgs e)
        //{
        //    //CircleTime newTime = new CircleTime(ProjectTime);
        //    /////////周期是工程属性之一需要保存和读取！！！！！！
        //    //newTime.ShowDialog();
        //    //if (newTime.DialogResult == DialogResult.OK)
        //    //{
        //    //    ProjectTime = newTime.Ctime;
        //    //}
        //}


        #endregion

        #region 置后、置前

        /// <summary>
        /// 置后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendToBackStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentDocumentsHostControl != null && CurrentDocumentsHostControl.HostDesign != null && CurrentDocumentsHostControl.HostDesign.SelectedObjects != null)
            {
                StartUndo();//启动撤销
                object[] selectObject = new object[CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count];  //定义对象数组
                CurrentDocumentsHostControl.HostDesign.SelectedObjects.CopyTo(selectObject, 0);     //通过拷贝函数获取选中的对象集合
                for (int i = 0; i < selectObject.Length; i++)
                {
                    if (!(selectObject[i].GetType().ToString().Equals(PublicVariable.viewName)))
                    {
                        ((Control)selectObject[i]).SendToBack();     //将选中的对象全部置后

                        Operation ctrlOpt = new Operation();
                        ctrlOpt.Oname = UndoOperation.OperateStyle.控件置后.ToString();
                        ctrlOpt.Item = ((Control)selectObject[i]);
                        CassViewGenerator.AddOpt(ctrlOpt);
                    }
                }
            }
        }

        /// <summary>
        /// 置前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bringToFrontToolStripButton_Click(object sender, EventArgs e)
        {
            if (CurrentDocumentsHostControl != null && CurrentDocumentsHostControl.HostDesign != null && CurrentDocumentsHostControl.HostDesign.SelectedObjects != null)
            {
                StartUndo();//启动撤销
                object[] selectObject = new object[CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count];    //定义对象数组
                CurrentDocumentsHostControl.HostDesign.SelectedObjects.CopyTo(selectObject, 0);     //通过拷贝函数获取选中的对象集合
                for (int i = 0; i < selectObject.Length; i++)
                {
                    if (!(selectObject[i].GetType().ToString().Equals(PublicVariable.viewName)))
                    {
                        ((Control)selectObject[i]).BringToFront();       //将选中的对象全部置前

                        Operation ctrlOpt = new Operation();
                        ctrlOpt.Oname = UndoOperation.OperateStyle.控件置前.ToString();
                        ctrlOpt.Item = ((Control)selectObject[i]);
                        CassViewGenerator.AddOpt(ctrlOpt);
                    }
                }
            }
        }

        /// <summary>
        /// 置前、置后使能处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showStyleToolStripMenuItem_EnabledChanged(object sender, EventArgs e)
        {
            //bringToFrontToolStripButton.Enabled = showStyleToolStripMenuItem.Enabled;
            //sendToBackStripButton.Enabled = showStyleToolStripMenuItem.Enabled;

            //bringToFrontMenuItem.Enabled = showStyleToolStripMenuItem.Enabled;
            //sendToBackMenuItem.Enabled = showStyleToolStripMenuItem.Enabled; ;
        }
        #endregion

        #region 帮助

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        /// <summary>
        /// 添加的外部工具用于修改控件信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //text3.Form1 newForm = new text3.Form1();
            //newForm.StartForm(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PublicVariable.ToolConfigFileName));
            //newForm.Show();
        }

        /// <summary>
        /// 当控件库XML文件修改后
        /// 对应修改现有控件结构体
        /// </summary>
        static public void UpdateCurrentPro()
        {
            if (CassViewGenerator.nodeList != null && CassViewGenerator.nodeList.Count != 0)
            {
                foreach (ListObject node in nodeList)
                {//更新所有页面控件结构体信息
                    CassView cassView = (CassView)(node.tabPage.Controls[0].GetNextControl(node.tabPage.Controls[0], false));
                    cassView.UpdateCtrlInfo();
                }
            }
        }

        # endregion

        #region 鼠标,按键等消息的处理

        private bool mouseLeftDown = false;
        private bool isPortPoint = false;

        /// <summary>
        /// 根据Windows的消息号进行相应的处理。主要提供鼠标和方向键的处理
        /// 先处理鼠标左键单击设计器时，显示鼠标位置。
        /// 鼠标右击时显示操作的对话框，并且该对话框根据是否时设计页面而显示不同的操作内容
        /// </summary>
        /// <param name="msg">Windows消息号</param>
        /// <returns>返回false值，表示允许消息可以继续到达下一个筛选器或控件 ，若为true表示筛选消息并禁止消息被调度</returns>
        public bool PreFilterMessage(ref   Message msg)
        {
            object tempObject = null;
            try
            {
                if (CurrentDocumentsHostControl == null || CurrentDocumentsHostControl.HostDesign == null)
                {
                    return false;
                }

                if (editPropertyFlag == true) //处于页面设计属性状态
                {
                    return false;
                }

                IDesignerHost host = (IDesignerHost)CurrentDocumentsHostControl.HostDesign.GetService(typeof(IDesignerHost));
                int MouseX = tabControlView.SelectedTab.PointToClient(Control.MousePosition).X;
                int MouseY = tabControlView.SelectedTab.PointToClient(Control.MousePosition).Y;
                if (MouseX >= 0 && MouseY >= 0 && MouseX < tabControlView.SelectedTab.Size.Width && MouseY < tabControlView.SelectedTab.Size.Height)
                {//必须在当前选定页内触发一下消息处理20090731
                    if (tabControlView.SelectedTab != null && tabControlView.SelectedTab.Controls.Count > 0
                        && CurrentDocumentsHostControl.HostDesign.HostFocuse == false && host.RootComponent != null
                        && tabControlView.SelectedTab.Controls[0].GetNextControl(tabControlView.SelectedTab.Controls[0],
                        false).PointToClient(Control.MousePosition).X
                        >= ((Control)host.RootComponent).Bounds.X    //以当前设计的根组建的基实例的位置和大小作为范围的比较.即当前的CassView类实例
                        && tabControlView.SelectedTab.Controls[0].GetNextControl(tabControlView.SelectedTab.Controls[0],
                        false).PointToClient(Control.MousePosition).X - ((Control)host.RootComponent).Bounds.X
                        <= ((Control)host.RootComponent).Bounds.Width
                        && tabControlView.SelectedTab.Controls[0].GetNextControl(tabControlView.SelectedTab.Controls[0],
                        false).PointToClient(Control.MousePosition).Y
                        >= ((Control)host.RootComponent).Bounds.Y
                        && tabControlView.SelectedTab.Controls[0].GetNextControl(tabControlView.SelectedTab.Controls[0],
                        false).PointToClient(Control.MousePosition).Y - ((Control)host.RootComponent).Bounds.Y
                        <= ((Control)host.RootComponent).Bounds.Height)
                    {
                        switch (msg.Msg)
                        {
                            case PublicVariable.KeyPress: //按键
                                {
                                    if (CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count > 0)
                                    {
                                        object[] selectObject = new object[CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count];
                                        CurrentDocumentsHostControl.HostDesign.SelectedObjects.CopyTo(selectObject, 0);
                                        switch (msg.WParam.ToInt32())
                                        {
                                            case PublicVariable.KeyDown: //方向键: 下
                                                for (int i = 0; i < selectObject.Length; i++)
                                                {
                                                    tempObject = selectObject[i];
                                                    if (((Control)tempObject).Location.Y < ((Control)CurrentDocumentsHostControl.HostDesign.View).Size.Height - ((Control)tempObject).Size.Height)
                                                    {
                                                        ((Control)tempObject).Location = new Point(((Control)tempObject).Location.X, ((Control)tempObject).Location.Y + 1);
                                                    }
                                                }
                                                break;

                                            case PublicVariable.KeyUp:  //上
                                                for (int i = 0; i < selectObject.Length; i++)
                                                {
                                                    tempObject = selectObject[i];
                                                    if (((Control)tempObject).Location.Y > 0)
                                                    {
                                                        ((Control)tempObject).Location = new Point(((Control)tempObject).Location.X, ((Control)tempObject).Location.Y - 1);
                                                    }
                                                }
                                                break;

                                            case PublicVariable.KeyLeft:   //左
                                                for (int i = 0; i < selectObject.Length; i++)
                                                {
                                                    tempObject = selectObject[i];
                                                    if (((Control)tempObject).Location.X > 0)
                                                    {
                                                        ((Control)tempObject).Location = new Point(((Control)tempObject).Location.X - 1, ((Control)tempObject).Location.Y);
                                                    }
                                                }
                                                break;
                                            case PublicVariable.KeyRight:  //右
                                                for (int i = 0; i < selectObject.Length; i++)
                                                {
                                                    tempObject = selectObject[i];
                                                    if (((Control)tempObject).Location.X < ((Control)CurrentDocumentsHostControl.HostDesign.View).Size.Width - ((Control)tempObject).Size.Width)
                                                    {
                                                        ((Control)tempObject).Location = new Point(((Control)tempObject).Location.X + 1, ((Control)tempObject).Location.Y);
                                                    }
                                                }
                                                break;
                                            case PublicVariable.KeyPageUp:      //Page Up
                                                Control obj = currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false);
                                                ((CassView)obj).Scaling = ((CassView)obj).Scaling + 0.2F;
                                                break;
                                            case PublicVariable.KeyPageDown:      //Page Down
                                                Control obj1 = currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false);
                                                ((CassView)obj1).Scaling = ((CassView)obj1).Scaling - 0.2F;
                                                break;
                                            case PublicVariable.KeyDelete:      //Delete
                                                Control obj2 = currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false);
                                                ((CassView)obj2).DeleteLine(((CassView)obj2).ChoosedLine, true);
                                                StartUndo();//启动撤销
                                                break;
                                            case 32://空格键切换连线和箭头功能
                                                if (this.linkToolStripButton.Enabled == false)
                                                {
                                                    this.linkToolStripButton.Enabled = true;
                                                    this.editToolStripButton.Enabled = false;
                                                }
                                                else
                                                {
                                                    this.linkToolStripButton.Enabled = false;
                                                    this.editToolStripButton.Enabled = true;
                                                }
                                                SetEditMode();
                                                break;
                                        }
                                    }
                                }
                                break;

                            case PublicVariable.MoustRight:  //鼠标右键单击
                                {
                                    ToolBoxServiceImpl.typeNameString = null;
                                    if (this.startCountPoint == false)
                                    {
                                        if (controlfilteredPropertyGrid.SelectedObject.GetType().ToString().Equals(PublicVariable.viewName))  //为CassView
                                        {
                                            controlEditOperation.Items[0].Visible = true;     //撤消
                                            controlEditOperation.Items[1].Visible = true;     //重复
                                            controlEditOperation.Items[2].Visible = false;    //剪切
                                            controlEditOperation.Items[3].Visible = false;    //复制
                                            controlEditOperation.Items[5].Visible = false;    //删除
                                            aliginToolStripMenuItem.Enabled = false;          //视图可操作

                                            //如果此时页面上的控件不为空,则可显示全选
                                            if (tabControlView.SelectedTab.Controls[0].GetNextControl(tabControlView.SelectedTab.Controls[0], false).Controls.Count > 0)
                                            {
                                                selectAllToolStripMenuItem.Enabled = true;
                                            }
                                            else
                                            {
                                                selectAllToolStripMenuItem.Enabled = false;
                                            }
                                        }
                                        else
                                        {
                                            controlEditOperation.Items[0].Visible = false; //撤消
                                            controlEditOperation.Items[1].Visible = false; //重复
                                            controlEditOperation.Items[2].Visible = true;  //剪切
                                            controlEditOperation.Items[3].Visible = true;  //复制
                                            controlEditOperation.Items[5].Visible = true;  //删除
                                        }
                                        if (this.ProListBox.ContextMenuStrip.Visible != true)
                                        {//当鼠标不在工程管理窗口时 否则会出现弹出两个菜单20090619
                                            controlEditOperation.Show(Control.MousePosition);
                                        }
                                    }
                                }
                                break;

                            case PublicVariable.MouseLeftDown:
                                {
                                    this.isPortPoint = false;
                                    if (this.startCountPoint == true)
                                    {
                                        Control control = currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false);
                                        if (control != null)
                                        {
                                            ((CassView)control).MouseDownPoint = MousePosition;
                                            this.isPortPoint = ((CassView)control).IsPortPoint;
                                        }
                                        this.mouseLeftDown = true;
                                    }
                                }
                                break;

                            case PublicVariable.MouseLeftUp:
                                {
                                    ToolBoxServiceImpl.typeNameString = null;
                                    if (this.isPortPoint == true)
                                    {
                                        Control control = currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false);
                                        if (control != null)
                                        {
                                            ((CassView)control).MouseUpPoint = MousePosition;
                                            ((CassView)control).ClearMouseMovePoint = new Point(0, 0);
                                            StartUndo();//启动撤销
                                        }
                                    }
                                    this.mouseLeftDown = false;
                                }
                                break;

                            case PublicVariable.MouseMove:  //鼠标移动
                                {
                                    if (this.isPortPoint == true && this.mouseLeftDown == true)
                                    {
                                        Control control = currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false);
                                        if (control != null)
                                        {
                                            ((CassView)control).MouseMovePoint = MousePosition;
                                        }
                                    }
                                    //designMousePosition.Text = "行 " + Convert.ToString(tabControlView.SelectedTab.Controls[0].PointToClient(Control.MousePosition).X - ((Control)host.RootComponent).Bounds.X)
                                    //                    + "," + "列 " + Convert.ToString(tabControlView.SelectedTab.Controls[0].PointToClient(Control.MousePosition).Y - ((Control)host.RootComponent).Bounds.Y);
                                }
                                break;
                            case PublicVariable.MouseDoubleClick:
                                {

                                    object[] selectObject = new object[CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count];
                                    CurrentDocumentsHostControl.HostDesign.SelectedObjects.CopyTo(selectObject, 0);
                                    if (controlfilteredPropertyGrid.SelectedObject.GetType().ToString().Equals(PublicVariable.viewName))
                                    {
                                        //双击cassview
                                    }
                                    else
                                    {
                                        #region  单步调试2014.1
                                        if (isDebugByStep)  //单步调试2014.1
                                        {
                                            this.picWin.ClearInspectionStatus();
                                            string index = TypeDescriptor.GetProperties(selectObject[0])["SerialNumber"].GetValue(selectObject[0]).ToString();
                                            string Cmname = null;
                                            foreach (List<string> ctrlsNum in GenerateCode.CtrlsList)
                                            {
                                                if (ctrlsNum.Contains(index))
                                                {

                                                    //this.picWin.SetDebugStatus("null", false);
                                                    //this.picWin.ClearInspectionStatus();

                                                    this.CurControlsNum = ctrlsNum;
                                                    if (this.picWin.InputPicIndex != null && this.picWin.InputPicIndex != ctrlsNum[0])
                                                    {
                                                        this.ShowInputPic(false, ctrlsNum[0]);
                                                    }
                                                    if (index == ctrlsNum[ctrlsNum.Count - 1].ToString())  //所选的是最后一个输出文件控件
                                                    {

                                                        int lastIndex = ctrlsNum.Count - 2;//获取最后一个控件，读取文件所在路径，设置OutputPic
                                                        if (this.ExistsMatchControl("MATCH"))
                                                        {
                                                            this.DoMatchOper(false);
                                                            //显示match控件前的效果图
                                                            lastIndex--;

                                                        }
                                                        if (lastIndex >= 0)
                                                        {
                                                            Cmname = currentProjectPath + "\\out\\" + ctrlsNum[lastIndex] + ".bmp";
                                                        }
                                                      //  this.ShowOutputPic(Cmname);

                                                    }
                                                    else if (index == ctrlsNum[0].ToString()) //获取输入控件,即第一个控件
                                                    {
                                                        //this.ShowInputPic();

                                                        // this.ShowOutputPic(GetSourceFile(index));
                                                        Cmname = this.GetSourceFile(index);
                                                      //  this.ShowOutputPic(Cmname);
                                                        //this.picWin.SetDebugStatus("null", false);
                                                       // this.picWin.ClearInspectionStatus();
                                                    }
                                                    else
                                                    {
                                                        Cmname = TypeDescriptor.GetProperties(selectObject[0])["OutputImg"].GetValue(selectObject[0]).ToString();
                                                       // this.ShowOutputPic(Cmname);
                                                        //this.picWin.SetDebugStatus("null", false);
                                                        //this.picWin.ClearInspectionStatus();
                                                    }

                                                    this.ShowOutputPic(Cmname);
                                                    break;
                                                }
                                            }
                                                                                  
                                        }
                                        //Form pbxfm = new Form();
                                        //PictureBox newPbx = new PictureBox();
                                        //newPbx.Parent = pbxfm;
                                        //string Cmname = TypeDescriptor.GetProperties(selectObject[0])["OutputImg"].GetValue(selectObject[0]).ToString();
                                         
                                        //newPbx.Image = Image.FromFile(Cmname);
                                        //newPbx.SizeMode = PictureBoxSizeMode.StretchImage;
                                        //newPbx.Dock = DockStyle.Fill;
                                        //pbxfm.Show();

                                        #endregion
                                    }

                                }
                                break;
                        }
                    }
                    if (msg.Msg == PublicVariable.MouseMove)
                    {//鼠标移动显示相对位置20090731
                        int row = tabControlView.SelectedTab.Controls[0].PointToClient(Control.MousePosition).X - ((Control)host.RootComponent).Bounds.X;
                        int column = tabControlView.SelectedTab.Controls[0].PointToClient(Control.MousePosition).Y - ((Control)host.RootComponent).Bounds.Y;
                        if (row >= 0 && row <= ((Control)host.RootComponent).Size.Width && column >= 0 && column <= ((Control)host.RootComponent).Size.Height)
                        {
                            designMousePosition.Text = "X:" + row.ToString() + "," + "Y:" + column.ToString();
                        }
                    }
                }
                Control cass = currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false);
                if (((CassView)cass).UndoFlag == true)
                {//启动撤销
                    StartUndo();
                }
            }
            catch (Exception ex) { }
            return false;
        }
        #endregion

        #region 工程操作
        static public List<string[]> SpecialErrors = new List<string[]>();//每次编译后产生特殊错误

        /// <summary>
        /// 依据指标ShowNumber设置所有控件是否显示序号
        /// </summary>
        private void SetShowNumber()
        {
            if (ShowNumber == false)
            {
                ShowNumber = true;
                projectToolStripMenuItem_ShowNum.Text = "隐藏控件序号";
            }
            else
            {
                ShowNumber = false;
                projectToolStripMenuItem_ShowNum.Text = "显示控件序号";
            }


            //遍历所有页面所有控件对 显示序号属性赋ShowNumber值
            foreach (ListObject node in nodeList)
            {
                CassView currentCas = (CassView)(node.tabPage.Controls[0].GetNextControl(node.tabPage.Controls[0], false));
                //把当前的属性保存 再赋值 设置序号结束后再还原
                //连线模式控件VISIBLE属性为false
                bool curValue = currentCas.LinesEditable;
                currentCas.LinesEditable = false;
                foreach (Control element in currentCas.Controls)
                {
                    PropertyDescriptor showNumberProperty = TypeDescriptor.GetProperties(element)["ShowNumber"];
                    if (showNumberProperty != null)
                    {
                        showNumberProperty.SetValue(element, ShowNumber);
                    }
                }
                currentCas.LinesEditable = curValue;
            }

        }

        /// <summary>
        /// 生成指令表
        /// 根据所有连线的控件来产生一个指令表和一个XML文件
        /// </summary>
        private void codelistToolStripMenuItem_CreateCodelist_Click(object sender, EventArgs e)
        {
            CreateCodeList();//生成指令信息
        }

        /// <summary>
        /// 遍历所有的工程页面获取页面路径和CASSVIEW信息
        /// </summary>
        private List<ArrayList> GetCassinfo()
        {
            try
            {
                List<ArrayList> CassInfos = new List<ArrayList>();
                OrderNodepage(this.solutionTreeView.Nodes[0], ref CassInfos);
                return CassInfos;
            }
            catch
            {
                CassMessageBox.Warning("没有合理的连线控件，指令表生成失败！");
                return new List<ArrayList>();
            }
        }

        /// <summary>
        /// 递归函数根据当前的树先序遍历所有的节点并依此对页面进行排序
        /// </summary>
        /// <param name="pareNode">当前父节点</param>
        /// <param name="tempList">将对应的页面加载至的数组</param>
        /// <returns>用于排序存放的页面列表</returns>
        private void OrderNodepage(TreeNode pareNode, ref List<ArrayList> tempList)
        {
            foreach (ListObject elementPage in nodeList)
            {
                if (elementPage.tabPage.Text == pareNode.Text)
                {
                    ArrayList cassinfo = new ArrayList();
                    CassView cassView = (CassView)(elementPage.tabPage.Controls[0].GetNextControl(elementPage.tabPage.Controls[0], false));
                    cassinfo.Add(new string[] { pareNode.Text, pareNode.FullPath });
                    cassinfo.Add(cassView);
                    tempList.Add(cassinfo);

                    foreach (TreeNode child in pareNode.Nodes)
                    {
                        OrderNodepage(child, ref tempList);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 设置地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void projectToolStripMenuItem_SetAdress_Click(object sender, EventArgs e)
        {
            StartComply(false);
            if (!CheckError(false))
            {//设置地址前提是无错
                this.errorForm.errorList.Add(new string[] { null, "编译有错不能设置地址信息!", "info", null });
            }
            else
            {
                AddressTable newForm;
                if (CassViewGenerator.ProjectMode == "View")
                {
                    newForm = new AddressTable(GetCassinfo());
                }
                else// if (CassViewGenerator.ProjectMode == "Code")
                {
                    ConfigCodeEdit codeEditor = (ConfigCodeEdit)(this.tabControlView.SelectedTab.Controls[0]);
                    newForm = new AddressTable(codeEditor.CodeEditor.Codectrlsinfo, codeEditor.CodeEditor.CodearraysInfo);
                }
                newForm.ReturnTable = this.addressInfo;
                newForm.ShowDialog();
                if (newForm.DialogResult == DialogResult.Yes)
                {//保存设置地址后的可读地址长度、可读可写地址长度和地址表
                    this.addressRWsize = newForm.RW;
                    this.addressInfo = newForm.ReturnTable;
                    this.errorForm.errorList.Add(new string[] { null, "设置地址信息成功!", "info", null });
                }
            }
            this.errorForm.SetListview();
        }

        /// <summary>
        /// 主态图形编辑模式下根据图形信息
        /// 并打开指令页面
        /// </summary>
        private void CreateCodeList()
        {
            GenerateCode newGenCode = new GenerateCode(GetCassinfo());
            this.IOlist = newGenCode.GenerateCodeList();
            this.tempValue = newGenCode.TempInfo;
            this.errorForm.ViewErrorInfo = newGenCode.ViewErrorinfo;//将为连接控件信息赋给错误列表控件
            AddCodeListPage(this.IOlist);
        }

      

       /// <summary>
       /// 编译
       /// 生成指令、查错、提示、生成XML
       /// </summary>
       /// <param name="flag">true:  上位机仿真
       ///                                   false :   嵌入式</param>
        private void Comply()
        {
            try
            {
                this.bgw_processbar.Visible = true;
                this.bgw_processbar.Value = 10;

                saveProjectOperation();//编译开始前保存当前工程

                StartComply(true);//编译开始前执行对应操作               

                this.bgw_processbar.Value = 30;
                //判断当前工程目录下是否存在out文件夹
                if (!Directory.Exists(currentProjectPath+"\\out"))
                {
                    Directory.CreateDirectory(currentProjectPath+"\\out");
                }
              // 判断当前工程目录下是否存在feather.h 文件
                if (!File.Exists(currentProjectPath + "\\feather.h"))
                {
                    File.Create(currentProjectPath + "\\feather.h");
                }

               
                if (CheckError(true))
                { //编译无错
                    this.bgw_processbar.Value = 40;

                    bool flag = compileMode==CompileMode.SimulationMode ? true:false ;
                    CreateXmlFile(flag);
                    this.errorForm.errorList.Add(new string[] { null, "组态文件编译成功!", "info", null });


                    #region 针对一条流程图内多个区域识别操作

                    //读取输入图片
                    string inputPicPath = this.GetSourceFile(GenerateCode.CtrlsList[0][0]);
                    //读取feather.h文件，抽取小区域信息
                    MachineVisionMethod.AnalyseFeatherHeaderHeaderFile(currentProjectPath, inputPicPath);

                    #endregion

                    this.bgw_processbar.Value = 50;

                    #region  2014.1   pc仿真，生成exe
                    // this.isComplySuccess = true;
                    if (flag)  //SIMU 上位机仿真  调用bat，运行main.c
                    {
                        if (!PCMakeBat("CassMake.bat"))  //调用批处理
                        {
                            this.errorForm.errorList.Add(new string[] { null, "批处理过程出现异常！", "error", null });

                        }
                        else if (!File.Exists(currentProjectPath + "\\" + Path.GetFileNameWithoutExtension(currentProjectPath + "\\" + saveName) + ".exe"))
                        {
                            this.errorForm.errorList.Add(new string[] { null, "EXE生成失败！", "error", null });
                            //算法调试时调用 20140224
                            string errorInfo = FileOperator.ReadFromFile2(ProgramPath + "out\\error.txt", Encoding.GetEncoding("gb2312"));
                            CassMessageBox.Warning(errorInfo);

                        }
                        else
                        {
                            this.errorForm.errorList.Add(new string[] { null, "EXE生成成功！", "info", null });
                            this.ChangeDebugRunToolStripEnable(true);//2014.1.9  调试控件使能
                        }
                    }
                    this.bgw_processbar.Value = 70;
                    #endregion

                }
                else
                {//编译出错
                    this.errorForm.errorList.Add(new string[] { null, "组态文件生成失败!", "error", null });
                }

                this.bgw_processbar.Value = 90;
                SaveCodetext();//保存指令信息
                this.bgw_processbar.Value = 100;
                Thread.Sleep(200);
                this.bgw_processbar.Visible = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
               this.errorForm.errorList.Add(new string[] { null, "编译失败!", "info", null });
            }
            finally
            {
                this.errorForm.SetListview();
                
                this.SwitchBottomTabPage(this.tab_ErrorFormManager);
            }
        }

        /// <summary>
        /// 编译开始前期操作
        /// 初始化全局队列 删除相应XML文件
        /// </summary>
        /// <param name="DelXML">删除工程文件中的XML则为true</param>
        private void StartComply(bool DelXML)
        {
            SpecialErrors.Clear();//初始化特殊错误列表
            this.errorForm.ViewErrorInfo.Clear();//初始化可视的错误列表信息
            this.errorForm.errorList.Clear();//错误列表队列清空
            this.errorForm.ClearList();//出错列表信息清空
            PLCCodeEditor.ErrorList.Clear();//出错队列清空
            GenerateCode.UnseeArray.Clear();//不显示数组清空
           
            if (DelXML)
            {//正常编译前删除XML
                DirectoryInfo SourceDirectory = new DirectoryInfo(Path.Combine(WorkSpacePath, ProjectName));
                FileInfo[] Finfos = SourceDirectory.GetFiles();
                for (int i = 0; i < Finfos.Length; i++)
                {
                  
                    if (Finfos[i].Name.ToUpper().Contains("CONFIGURATION") || Finfos[i].Name == ProjectName + ".xml"
                        || Finfos[i].Name.Contains(".exe") || Finfos[i].Name.Contains(".c"))  //2013.11.25
                    {//删除所有编译生成的文件
                           
                               try
                               {

                                   Finfos[i].Delete();
                               }
                               catch
                               {
                                  
                               }     
                    }
                }
            }
        }

        /// <summary>
        /// 编译检测所有错误
        /// 包括指令表错误、特殊错误（包括图形模式控件无连接和指令模式点名冲突）以及地址冲突错误
        /// </summary>
        /// <param name="checkAddress">检测地址冲突错误则为true</param>
        /// <returns>没有错误则返回true</returns>
        private bool CheckError(bool checkAddress)
        {
            if (CassViewGenerator.ProjectMode == "View")
            {//图形编辑模式 生成指令页面             
                CreateCodeList();
            }
            if (CassViewGenerator.ProjectMode == "Simu")
            {
                CreateCodeList();
            }
            OpenCodetext();//切换到指令表页面

            ConfigCodeEdit codeEditor = (ConfigCodeEdit)(this.tabControlView.SelectedTab.Controls[0]);
            if (CassViewGenerator.ProjectMode == "Code")
            {//指令编辑模式 获取指令信息
                codeEditor.CodeEditor.CreateCtrlsinfo();
                this.tempValue = codeEditor.CodeEditor.Codetempvalue;
                this.IOlist = codeEditor.CodeEditor.Codeiolist;
                CodeText = codeEditor.CodeEditor.Text;//将指令信息存放至待保存指令信息中
            }
            //编译查错
            codeEditor.CodeEditor.checkIolist();//******差错用

            this.errorForm.errorList.AddRange(PLCCodeEditor.ErrorList);//加入指令表错误
            this.errorForm.errorList.AddRange(SpecialErrors);//加入特殊错误信息
            if (checkAddress)
            {//检测地址错误信息
                this.errorForm.errorList.AddRange(AddressTable.checkRepeatAdres(this.addressInfo));
            }
            if (this.errorForm.checkErrorCount() == 0)
            {//无错误条目
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 当编译结果中没有错误
        /// 生成6个XML文件和1个Main.c文件
        /// 生成地址XML,工程XML,CodeXML,DateXML,DefXML
        /// </summary>
        /// <param name="flag">true:  上位机仿真
        ///                                   false :   嵌入式
        ///  </param>
        private void CreateXmlFile(bool flag)
        {
            GenerateXML CreateXML = new GenerateXML(this.savePath, new string[] { ProjectName, ProjectInfo, ProjectNum });
            List<ControlInfo> ctrls = new List<ControlInfo>();

            if (CassViewGenerator.ProjectMode == "View"||CassViewGenerator.ProjectMode == "Simu")
            {
                foreach (ListObject node in nodeList)
                {//将对应工程下的所有页面的控件信息放入ctrls列表
                    CassView cassView = (CassView)(node.tabPage.Controls[0].GetNextControl(node.tabPage.Controls[0], false));
                    cassView.colorReflash();//还原当前页面的控件颜色
                    CassView.ClearCtrlsInfo(cassView);
                    ctrls.AddRange(cassView.ctrlsInfo);
                }
                CreateXML.GetSpecialCtrlInfo(nodeList);//获取特殊控件中的数据
            }
            else if (CassViewGenerator.ProjectMode == "Code")
            {//注意：此模式下 会跳过 计算器组态和条件动作表指令
                int pageIndex = findCodePage();
                if (pageIndex != -1)
                {
                    System.Windows.Forms.TabPage tempPage = this.tabControlView.TabPages[pageIndex];
                    ConfigCodeEdit codeEditor = (ConfigCodeEdit)(tempPage.Controls[0]);
                    ctrls = codeEditor.CodeEditor.Codectrlsinfo;
                    CreateXML.GetSpecialCtrlInfo(codeEditor.CodeEditor.CodearraysInfo);
                }
            }
            //生成ControlFuns.xml +  main.c 文件    2013.11.20 注释
       
       //     
            //生成ControlFuns.c +  main.c 文件   2013.11.20 添加
            //if (flag)
            //    CreateXML.CreateProjectMainC(this.tempValue, this.addressInfo, ctrls, this.IOlist, "XSLTParam_PC.xslt");
            //else
             CreateXML.CreateProjectMainC(this.tempValue, this.addressInfo, ctrls, this.IOlist, flag);
                //CreateXML.CreateProjectXML(this.tempValue, this.addressInfo, ctrls, this.IOlist);

            CreateXML.CreateBasicInfoXML(this.addressRWsize);
            CreateXML.CreateAddressTableXML(this.addressInfo);
            CreateXML.CreateDatasXML(this.addressInfo, ctrls);
            CreateXML.CreateDefXML(this.addressInfo, ctrls);
        }

        /// <summary>
        /// 根据指令列表添加指令表页面
        /// 并切换到指令页面
        /// </summary>
        /// <param name="CodeValue"></param>
        /// <returns></returns>
        private void AddCodeListPage(List<string[]> CodeValue)
        {
            ConfigCodeEdit codeEditor = new ConfigCodeEdit();

            System.Windows.Forms.TabPage tempPage;
            int pageIndex = findCodePage();
            if (pageIndex != -1)
            {
                tempPage = this.tabControlView.TabPages[pageIndex];
                codeEditor = (ConfigCodeEdit)(tempPage.Controls[0]);
                codeEditor.CodeEditor.Text = null;
                this.tabControlView.SelectedIndex = pageIndex;
            }
            else
            {
                tempPage = new System.Windows.Forms.TabPage(CodePageName);
                codeEditor.Parent = tempPage;
                codeEditor.Dock = DockStyle.Fill;
                this.tabControlView.TabPages.Add(tempPage);
                this.tabControlView.SelectedIndex = this.tabControlView.TabPages.Count - 1;
            }
            saveForm.Enabled = true;
            saveForm.Text = "保存 " + tabControlView.SelectedTab.Text;

            for (int i = 0, row = 0; i < CodeValue.Count; i++)
            {
                if (CodeValue[i] != null)
                {
                    if (i != 0 && CodeValue[i][0].EndsWith(":"))
                    {//遇到新控件串空行
                        codeEditor.CodeEditor.InsertLine(row, "");
                        row++;
                    }
                    codeEditor.CodeEditor.FormatInsert(CodeValue[i][0], CodeValue[i][1], ref row);
                    row++;
                }
            }
            //将生成的指令信息存放至待保存指令信息中
            CodeText = codeEditor.CodeEditor.Text;
        }

        /// <summary>
        /// 双击出错列表,高亮显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wrongList_LineDoubleClick(object sender, EventArgs e)
        {
            for (int k = 0; k < this.tabControlView.TabCount; k++)
            {
                if (this.errorForm.HighLightLine != -1 && this.tabControlView.TabPages[k].Text == CodePageName)
                {//找到指令页面
                    this.tabControlView.SelectedIndex = k;
                    System.Windows.Forms.TabPage tempPage = this.tabControlView.TabPages[k];
                    ConfigCodeEdit codeEditor = (ConfigCodeEdit)(tempPage.Controls[0]);
                    codeEditor.CodeEditor.ShowHighLightRow(this.errorForm.HighLightLine);
                    return;
                }
                else if (this.errorForm.SelectCtrlinfo != null
                    && this.tabControlView.TabPages[k].Text == this.errorForm.SelectCtrlinfo[0])
                {
                    this.tabControlView.SelectedIndex = k;
                    if (this.errorForm.SelectCtrlinfo[1] != null)
                    {
                        CassView cassView = (CassView)(this.tabControlView.TabPages[k].Controls[0].GetNextControl(this.tabControlView.TabPages[k].Controls[0], false));

                        //把当前的属性保存 再赋值 设置属性结束后再还原
                        //连线模式控件VISIBLE属性为false
                        bool curValue = cassView.LinesEditable;
                        cassView.LinesEditable = false;

                        for (int x = 0; x < cassView.Controls.Count; x++)
                        {
                            PropertyDescriptor serialNumber = TypeDescriptor.GetProperties(cassView.Controls[x])["SerialNumber"];
                            PropertyDescriptor IsError = TypeDescriptor.GetProperties(cassView.Controls[x])["IsError"];
                            if (serialNumber.GetValue(cassView.Controls[x]).ToString() == this.errorForm.SelectCtrlinfo[1])
                            {//如果是当前选择的控件 则保存原有颜色
                                ((HostControl)this.tabControlView.TabPages[k].Controls[0]).ScrollControlIntoView(cassView);
                                //选中查找到的控件
                                List<Component> select = new List<Component>();
                                select.Add(cassView.Controls[x]);
                                ISelectionService selectionService = (ISelectionService)(CurrentDocumentsHostControl.HostDesign.GetService(typeof(ISelectionService)));
                                if (selectionService != null)
                                { selectionService.SetSelectedComponents(select, SelectionTypes.Replace); }

                                ShowControls(0);

                                this.controlfilteredPropertyGrid.SelectedObjects = new object[] { cassView.Controls[x] };
                                IsError.SetValue(cassView.Controls[x], true);
                            }
                            else
                            { IsError.SetValue(cassView.Controls[x], false); }
                        }
                        cassView.LinesEditable = curValue;
                    }
                    return;
                }
            }
            if (this.errorForm.HighLightLine != -1)
            {//未找到指令页面，则重新打开
                ConfigCodeEdit codeEditor = new ConfigCodeEdit();
                System.Windows.Forms.TabPage tempPage = new System.Windows.Forms.TabPage(CodePageName);
                codeEditor.Parent = tempPage;
                codeEditor.Dock = DockStyle.Fill;
                this.tabControlView.TabPages.Add(tempPage);
                this.tabControlView.SelectedIndex = this.tabControlView.TabPages.Count - 1;
                codeEditor.CodeEditor.Text = CodeText;
                codeEditor.CodeEditor.ShowHighLightRow(this.errorForm.HighLightLine);
                return;
            }
        }

        //private void SetSelectControl(int lvIndex, string searchFile, int row, int col)
        //{
        //    int controlX = 0;
        //    int controlY = 0;
        //    Control findControl = null;

        //    bool isOpen = false;
        //    //已打开就将查找页面选中为当前页
        //    for (int i = 0; i < PublicProjectClass.TabList.Count; i++)
        //    {
        //        MainForm.ListObject tabObj = (MainForm.ListObject)PublicProjectClass.TabList[i];
        //        if (tabObj.tabPage.Text == searchFile)
        //        {
        //            tabControlView.SelectedTab = tabObj.tabPage;
        //            isOpen = true;
        //            break;
        //        }
        //    }

        //    if (isOpen == false)
        //    {
        //        for (int j = 0; j < PublicProjectClass.NodeList.Count; j++)
        //        {
        //            MainForm.ListObject nodeObj = (MainForm.ListObject)PublicProjectClass.NodeList[j];
        //            if (nodeObj.tabPage.Text == searchFile)
        //            {
        //                tabControlView.TabPages.Add(nodeObj.tabPage);
        //                tabControlView.SelectedIndex = tabControlView.TabPages.Count - 1;
        //                PublicProjectClass.TabList.Add(nodeObj);


        //                break;
        //            }
        //        }
        //    }
        //    IDesignerHost host = CurrentDocumentsHostControl.HostDesign.GetService(typeof(IDesignerHost)) as IDesignerHost;
        //    int sizeNow = ((CassView)host.RootComponent).ControlAutoSize;
        //    controlX = col * sizeNow + 64;
        //    controlY = row * sizeNow;

        //    ShowControls(controlY);

        //    foreach (Control control in ((Control)host.RootComponent).Controls)
        //    {
        //        if (control.Location.X == controlX && control.Location.Y == controlY)
        //        {
        //            findControl = control;
        //            break;
        //        }
        //    }

        //    if (findControl != null)
        //    {
        //        //选中查找到的控件
        //        Collection<Component> select = new Collection<Component>();
        //        select.Add(findControl);
        //        ISelectionService selectionService = (ISelectionService)(CurrentDocumentsHostControl.HostDesign.GetService(typeof(ISelectionService)));
        //        if (selectionService != null) { selectionService.SetSelectedComponents(select, SelectionTypes.Replace); }
        //    }
        //    //高亮显示该行
        //    ((CassView)host.RootComponent).HighLightRow = row;
        //    ((CassView)host.RootComponent).Refresh();
        //}

        private void ShowControls(int newY)
        {
            try
            {
                IDesignerHost host = CurrentDocumentsHostControl.HostDesign.GetService(typeof(IDesignerHost)) as IDesignerHost;

                CassView cassView = (CassView)host.RootComponent;
                //if (cassView.Height < newY + 10 * cassView.ControlAutoSize)
                //{
                //    cassView.Height = newY + 10 * cassView.ControlAutoSize;
                //}
                ((HostControl)tabControlView.SelectedTab.Controls[0]).VerticalScroll.Visible = false;
                //把滚动条移动到相应位置

                //Size pageSize = this.tabControlView.SelectedTab.Size;
                //Size hostcontrolSize = CurrentDocumentsHostControl.Size;
                //int pageY = pageSize.Height;
                //int hostcontrolY = hostcontrolSize.Height;

                //int max = ((VScrollBar)tabControlView.SelectedTab.Controls[1]).Maximum - ((VScrollBar)tabControlView.SelectedTab.Controls[1]).LargeChange + 1;
                //int newValue = newY * max / (hostcontrolY - pageY);//2008.7.19
                ////int newValue = newY * max / hostcontrolY;//2008.7.19
                //if (newValue < 0)
                //{
                //    newValue = 0;
                //}
                //else if (newValue > max)
                //{
                //    newValue = max;
                //}
                //((VScrollBar)tabControlView.SelectedTab.Controls[1]).Value = newValue;

                ////把Hostcontrol移动到相应位置
                //CurrentDocumentsHostControl.Location = new Point(CurrentDocumentsHostControl.Location.X, -newValue * (hostcontrolY - pageY) / max);
                ////ControlReSize.HostControlMove(CurrentDocumentsHostControl);
            }
            catch
            {
                //CommonOperation.CassMessageBox.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 选择模式 0为可视编译 1为指令编译
        /// 并清空错误列表
        /// </summary>
        /// <param name="mode"></param>
        private void ModeSelect(CompileMode mode)
        {
            //isSimulationMode = false;
            compileMode = mode;
            if (mode == CompileMode.ConfigMode)
            {
                ProjectMode = "View";
                this.ToolStripMenuItem_Config.Checked = true;
                this.ToolStripMenuItem_IL.Checked = false;
                this.ToolStripMenuItem_Simulation.Checked = false;
                this.ModetoolStripButton.Text = "指令编译模式";
                this.ModetoolStripButton.Image = (Image)global::CaVeGen.Properties.Resources.ResourceManager.GetObject("Code");
                this.CurrentMode.Text = "图形编译模式      ";
                //菜单中的显示序号和生成指令表可用
                this.projectToolStripMenuItem_ShowNum.Enabled = true;
                this.projectToolStripMenuItem_CreateCodelist.Enabled = true;
                //打开main页面
                for (int k = 0; k < this.tabControlView.TabCount; k++)
                {
                    if (this.tabControlView.TabPages[k].Text == MainPageName)
                    {//找到指令页面
                        this.tabControlView.SelectedTab = tabControlView.TabPages[k];
                        break;
                    }
                }
            }
            else if (mode == CompileMode.ILMode)
            {
                ProjectMode = "Code";
                this.ToolStripMenuItem_IL.Checked = true;
                this.ToolStripMenuItem_Config.Checked = false;
                this.ToolStripMenuItem_Simulation.Checked = false;
                this.ModetoolStripButton.Text = "图形编译模式";
                this.ModetoolStripButton.Image = (Image)global::CaVeGen.Properties.Resources.ResourceManager.GetObject("View");
                this.CurrentMode.Text = "指令编译模式      ";
                //菜单中的显示序号和生成指令表禁用
                this.projectToolStripMenuItem_ShowNum.Enabled = false;
                this.projectToolStripMenuItem_CreateCodelist.Enabled = false;
                //打开CODE.IL页面
                OpenCodetext();
            }
            else if (mode == CompileMode.SimulationMode)
            {
                ProjectMode = "Simu";
                this.ToolStripMenuItem_Config.Checked = false;
                this.ToolStripMenuItem_IL.Checked = false;
                this.ToolStripMenuItem_Simulation.Checked = true;
                this.ModetoolStripButton.Text = "仿真编译模式";
                //*
                this.ModetoolStripButton.Image = (Image)global::CaVeGen.Properties.Resources.ResourceManager.GetObject("Code");
                this.CurrentMode.Text = "仿真编译模式      ";
                //菜单中的显示序号和生成指令表可用
                this.projectToolStripMenuItem_ShowNum.Enabled = true;
                this.projectToolStripMenuItem_CreateCodelist.Enabled = true;
                //打开main页面
                for (int k = 0; k < this.tabControlView.TabCount; k++)
                {
                    if (this.tabControlView.TabPages[k].Text == MainPageName)
                    {//找到指令页面
                        this.tabControlView.SelectedTab = tabControlView.TabPages[k];
                        break;
                    }
                }
               // isSimulationMode = true;
            }
            StartComply(false);
        }

        /// <summary>
        /// 打开工程目录中的Code.IL
        /// 不存在则新建
        /// </summary>
        private void OpenCodetext()
        {
            ConfigCodeEdit codeEditor = new ConfigCodeEdit();
            System.Windows.Forms.TabPage tempPage;
            //指令页面存在则翻到该页
            int pageIndex = findCodePage();
            if (pageIndex != -1)
            {
                tempPage = this.tabControlView.TabPages[pageIndex];
                codeEditor = (ConfigCodeEdit)(tempPage.Controls[0]);
                this.tabControlView.SelectedIndex = pageIndex;
                return;
            }

            tempPage = new System.Windows.Forms.TabPage(CodePageName);
            codeEditor.Parent = tempPage;
            codeEditor.Dock = DockStyle.Fill;
            this.tabControlView.TabPages.Add(tempPage);
            this.tabControlView.SelectedIndex = this.tabControlView.TabPages.Count - 1;
            if (CodeText != null)
            {//有保存值则直接赋值后返回 否则从工程文件夹中读取
                codeEditor.CodeEditor.Text = CodeText;
                return;
            }
           // string codepath = Path.Combine(programPath + "//" + ProjectName, CodePageName);
            string codepath = Path.Combine(WorkSpacePath + "//" + ProjectName, CodePageName);
            if (!File.Exists(codepath))
            {//工程目录下不存在Code.il
                return;
            }
            //存在则读取
            FileStream fStream = new FileStream(codepath, FileMode.Open);  //读指令文件内容
            StreamReader sReader = new StreamReader(fStream);              //读取字符
            string eachRow = null;//每行指令
            string Codetext = null;//整体文本信息

            while ((eachRow = sReader.ReadLine()) != null)
            {
                if (eachRow.Trim() != "")
                {
                    if (Codetext != null)//除第一行和最后行外 每行添加个回车
                        Codetext = Codetext + "\n";
                    Codetext += eachRow;
                }
            }
            //格式化完成 更新
            codeEditor.CodeEditor.Text = Codetext;
            CodeText = Codetext;//更新当前工程的指令表信息
            sReader.Close();
            fStream.Close();
        }

        /// <summary>
        /// 将当前指令页面信息保存至工程文件夹下
        /// 文件名为Code.il
        /// </summary>
        private void SaveCodetext()
        {
            string codepath = Path.Combine(WorkSpacePath + "//" + ProjectName, CodePageName);
            FileStream fStream = new FileStream(codepath, FileMode.Create);  //写指令文件内容
            StreamWriter sWriter = new StreamWriter(fStream);
            string eachRow = null;
            if (CodeText != null)
            {
                for (int i = 0; i < CodeText.Length; i++)
                {
                    if (CodeText[i] == '\n')
                    {
                        sWriter.WriteLine(eachRow);
                        eachRow = null;
                    }
                    else
                    {
                        eachRow += CodeText[i];
                    }
                }
                if (eachRow != null)
                {//剩下的写入最后行
                    sWriter.WriteLine(eachRow);
                }
            }
            sWriter.Close();
            fStream.Close();
        }

        #endregion

        #region 工程模块
        //Point lefttop;
        //Point righttop;
        //Point leftbottom;
        //Point rightbottom;

        ////private List<Bitmap> picList = new List<Bitmap>();

        //public struct featherRgnstruct
        //{
        //    public Rectangle rec;
        //    public int featherNum;
        //}
        //Bitmap BoundBitmap = null;
        //int pbxWidth = 0;
        //int pbxHeight = 0;
        //Form fm;
        //Point startpoint;
        //Point endpoint;
        //List<featherRgnstruct> featureRegion = new List<featherRgnstruct>();
        //List<featherRgnstruct> backupfeathureRegion = new List<featherRgnstruct>();
        //Graphics g;
        //Size newRecsize = new Size(0, 0);
        //bool isSreenShot = false;
        
        //private void GetBoundtoolStripButton_Click(object sender, EventArgs e)
        //{
        //    OpenFileDialog openFileDialog1 = new OpenFileDialog();
        //    openFileDialog1.InitialDirectory = "c:\\" ;
        //    openFileDialog1.Filter = "bitmap files (*.bmp)|*.bmp";
        //    openFileDialog1.FilterIndex = 2 ;
        //    openFileDialog1.RestoreDirectory = true ;

        //    if (openFileDialog1.ShowDialog() == DialogResult.OK)
        //    {
        //        Bitmap newBitmap = (Bitmap)Image.FromFile(Path.GetFullPath(openFileDialog1.FileName));
        //        lefttop = new Point(0, 0);
        //        righttop = new Point(newBitmap.Width, 0);
        //        leftbottom = new Point(0, newBitmap.Height);
        //        rightbottom = new Point(newBitmap.Width, newBitmap.Height);
        //        Bitmap bitmap = new Bitmap(newBitmap.Width, newBitmap.Height);
        //        bitmap = newBitmap;
        //        int topnum = 0;
        //        int bottomnum = newBitmap.Height;
        //        int BlackPointNum = 0;
        //        int rightnum = newBitmap.Width;
        //        int leftnum = 0;

        //        //for (int i = 3; i < bitmap.Height; i++)
        //        //{
        //        //    BlackPointNum = 0;
        //        //    for (int j = 0; j < bitmap.Width; j++)
        //        //    {
        //        //        if (bitmap.GetPixel(j, i).R == 0)
        //        //        {
        //        //            BlackPointNum++;
        //        //        }
        //        //    }

        //        //    if (BlackPointNum < 10)
        //        //        continue;
        //        //    else
        //        //    {
        //        //        topnum = i;
        //        //        break;
        //        //    }

        //        //}

        //        //for (int i = 3; i < bitmap.Width; i++)
        //        //{
        //        //    BlackPointNum = 0;
        //        //    for (int j = 0; j < bitmap.Height; j++)
        //        //    {
        //        //        if (bitmap.GetPixel(i, j).R == 0)
        //        //        {
        //        //            BlackPointNum++;
        //        //        }
        //        //    }
        //        //    if (BlackPointNum < 10)
        //        //    { }
        //        //    else
        //        //    {
        //        //        leftnum = i;
        //        //        break;
        //        //    }

        //        //}

        //        //for (int i = bitmap.Height - 3; i >= 0; i--)
        //        //{
        //        //    BlackPointNum = 0;
        //        //    for (int j = bitmap.Width - 1; j >= 0; j--)
        //        //    {
        //        //        if (bitmap.GetPixel(j, i).R == 0)
        //        //        {
        //        //            BlackPointNum++;
        //        //        }
        //        //    }
        //        //    if (BlackPointNum < 10)
        //        //    {
        //        //    }
        //        //    else
        //        //    {
        //        //        bottomnum = i;
        //        //        break;
        //        //    }
        //        //}
        //        //for (int i = bitmap.Width - 3; i >= 0; i--)
        //        //{
        //        //    BlackPointNum = 0;
        //        //    for (int j = bitmap.Height - 1; j >= 0; j--)
        //        //    {
        //        //        if (bitmap.GetPixel(i, j).R == 0)
        //        //        {
        //        //            BlackPointNum++;
        //        //        }
        //        //    }
        //        //    if (BlackPointNum < 10)
        //        //    { }
        //        //    else
        //        //    {
        //        //        rightnum = i;
        //        //        break;
        //        //    }

        //        //}
        //        //Rectangle rec = new Rectangle(lefttop, new Size(rightnum - leftnum, bottomnum - topnum));

        //        ////g.DrawRectangle(Pens.Yellow, rec);
        //        //Bitmap bt = new Bitmap(rec.Width, rec.Height);
        //        //int[,] picdata = new int[rec.Width, rec.Height];
        //        ////RichTextBox rbx = new RichTextBox();
        //        //for (int i = 0; i < rec.Height; i++)
        //        //{
        //        //    for (int j = 0; j < rec.Width; j++)
        //        //    {
        //        //        picdata[j, i] = bitmap.GetPixel(leftnum + j, topnum + i).R;
        //        //        Color c = Color.FromArgb(255, picdata[j, i], picdata[j, i], picdata[j, i]);
        //        //        bt.SetPixel(j, i, c);
        //        //        //rbx.Text+=picdata[j,i].ToString()+" ";
        //        //    }
        //        //    // rbx.Text+='\n';
        //        //}
        //        BoundBitmap = newBitmap;
        //        DrawFeathertoolStripButton.Enabled = true;
        //        SaveFeathertoolStripButton.Enabled = true;
        //        CleartoolStripButton.Enabled = true;
        //    }
        //}
        //PictureBox pbx;
        //ToolStripMenuItem backspacing;
        //private void DrawFeathertoolStripButton_Click(object sender, EventArgs e)
        //{
        //    fm = new Form();
        //    pbx = new PictureBox();
        //    pbx.Parent = fm;
        //    if (BoundBitmap.Size.Width > 250)
        //        fm.Size = new Size(BoundBitmap.Size.Width + 10, BoundBitmap.Size.Height + 60);
        //    else
        //        fm.Size = new System.Drawing.Size(250, BoundBitmap.Size.Height + 60);
        //    fm.FormBorderStyle = FormBorderStyle.FixedDialog;
        //    fm.MaximizeBox = false;
        //    backSize = fm.Size;
        //    backbitmap = BoundBitmap;
        //    //pbx.Dock = DockStyle.Fill;
        //    pbx.Size = BoundBitmap.Size;
        //    pbx.Location = new Point(0, 24);
        //    pbx.Image = BoundBitmap;
        //    pbxWidth = pbx.Size.Width;

        //    pbxHeight = pbx.Size.Height;

        //    //定义
        //    MenuStrip menuStrip = new MenuStrip();
        //    ToolStripMenuItem showItem = new ToolStripMenuItem();
        //    ToolStripMenuItem recSize = new ToolStripMenuItem();
        //    ToolStripMenuItem threeRec = new ToolStripMenuItem();
        //    ToolStripMenuItem fiveRec = new ToolStripMenuItem();
        //    ToolStripMenuItem sevenRec = new ToolStripMenuItem();
        //    ToolStripMenuItem dragTodraw = new ToolStripMenuItem();
        //    ToolStripMenuItem screenshot = new ToolStripMenuItem();
        //    backspacing = new ToolStripMenuItem();

        //    //
        //    // 
        //    // menuStrip
        //    // 
        //    menuStrip.Items.AddRange(new ToolStripItem[] {
        //    showItem,recSize,screenshot,backspacing});
        //    menuStrip.Location = new System.Drawing.Point(0, 0);
        //    menuStrip.Name = "menuStrip";
        //    menuStrip.Size = new System.Drawing.Size(498, 24);
        //    menuStrip.TabIndex = 0;
        //    menuStrip.Text = "menuStrip";

        //    screenshot.Name = "screenshot";
        //    screenshot.Size = new Size(65, 20);
        //    screenshot.Text = "截图";
        //    screenshot.Click += new EventHandler(screenshot_click);

        //    backspacing.Name = "backspacing";
        //    backspacing.Size = new System.Drawing.Size(20, 20);
        //    backspacing.Text = "取消截图";
        //    backspacing.Click += new EventHandler(back_click);
        //    backspacing.Enabled = false;

        //    showItem.Name = "showItem";
        //    showItem.Size = new System.Drawing.Size(20, 20);
        //    showItem.Text = "显示";

        //    recSize.Name = "recSize";
        //    recSize.Text = "改变特征区";

        //    dragTodraw.Name = "dragTodraw";
        //    dragTodraw.Text = "自动拖拉矩形";
        //    dragTodraw.Tag = 1;
        //    dragTodraw.Click += new EventHandler(SizeChange);

        //    fiveRec.Name = "fiveRec";
        //    fiveRec.Text = "五乘五矩形";
        //    fiveRec.Tag = 5;
        //    fiveRec.Click += new EventHandler(SizeChange);

        //    threeRec.Name = "threeRec";
        //    threeRec.Text = "三乘三矩阵";
        //    threeRec.Tag = 3;
        //    threeRec.Click += new EventHandler(SizeChange);

        //    sevenRec.Name = "sevenRec";
        //    sevenRec.Text = "七乘七矩阵";
        //    sevenRec.Tag = 7;
        //    sevenRec.Click += new EventHandler(SizeChange);

        //    recSize.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { threeRec, fiveRec, sevenRec });
        //    showItem.MouseDown += new MouseEventHandler(PaintRectangle);

        //    menuStrip.Parent = fm;
        //    menuStrip.Dock = DockStyle.Top;
        //    g = pbx.CreateGraphics();
        //    //pbx.SizeMode = PictureBoxSizeMode.StretchImage;
        //    pbx.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
        //    pbx.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
        //    pbx.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);
        //   // pbx.SizeChanged += new EventHandler(pictureBox1_SizeChange);
        //    //pbx.DoubleClick += new MouseEventArgs(pbx_doubleClick);
        //    //pbx.Paint += new PaintEventHandler(PaintRectangle);
        //    fm.Show();
        //}
        //Bitmap backbitmap=null;
        //Size backSize;
        //private void back_click(object sender, EventArgs e)
        //{
        //    if(backbitmap!=null)
        //    {
        //        pbx.Image = backbitmap;
        //        backspacing.Enabled = false;
        //        fm.Size = backSize;
                
        //    }
 
        //}
        //private void SaveFeathertoolStripButton_Click(object sender, EventArgs e)
        //{
        //    if (File.Exists(savePath + "\\" + "feacher.txt"))
        //    {
        //        File.Delete(savePath + "\\" + "feacher.txt");
        //    }
        //    string alltext = null;
        //    StreamWriter sw = File.CreateText(savePath + "\\" + "feacher.txt");
        //    sw.WriteLine('(' + lefttop.X.ToString() + ',' + lefttop.Y.ToString() + ')');
        //    sw.WriteLine('(' + leftbottom.X.ToString() + ',' + leftbottom.Y.ToString() + ')');
        //    sw.WriteLine('(' + righttop.X.ToString() + ',' + righttop.Y.ToString() + ')');
        //    sw.WriteLine('(' + rightbottom.X.ToString() + ',' + rightbottom.Y.ToString() + ')');

        //    foreach (featherRgnstruct feather in featureRegion)
        //    {
        //        Rectangle rec = feather.rec;
                
        //        for (int i = rec.Y; i < rec.Y + rec.Height; i++)
        //        {
        //            for (int j = rec.X; j < rec.X + rec.Width; j++)
        //            {
        //                alltext+=j.ToString() + "," + i.ToString() + "," + BoundBitmap.GetPixel(j, i).R.ToString() + "," + feather.featherNum+ ",";
        //                //sw.WriteLine(j.ToString() + ":" + i.ToString() + ":" + BoundBitmap.GetPixel(j, i).R.ToString() + ":" + feather.featherNum);
        //            }
        //        }
        //    }
        //    sw.WriteLine(alltext);
        //    //sw.WriteLine("write some text.");
        //    sw.Close();
        //}

        //private void screenshot_click(object sender, EventArgs e)
        //{
        //    isSreenShot = true;
           
        //}

        //private void CleartoolStripButton_Click(object sender, EventArgs e)
        //{
        //    featureRegion.Clear();
        //}
        //private void SizeChange(object sender, EventArgs e)
        //{
        //    switch ((int)((sender as ToolStripItem).Tag))
        //    {
        //        case 1:
        //            //autoSize = true;
        //            break;
        //        case 3:
        //            newRecsize = new Size(3, 3);
        //            //autoSize = false;
        //            break;
        //        case 5:
        //            newRecsize = new Size(5, 5);
        //            //autoSize = false;
        //            break;
        //        case 7:
        //            newRecsize = new Size(7, 7);
        //            //autoSize = false;
        //            break;
        //    }
        //}

        //private void PaintRectangle(object sender, MouseEventArgs e)
        //{
        //    int x, y, width, height;
        //    //(sender as ).Invalidate();
        //    foreach (featherRgnstruct tmpfeather in featureRegion)
        //    {
        //        Rectangle tmprec = tmpfeather.rec;
        //        x = tmprec.X;
        //        y = tmprec.Y;
        //        width = tmprec.Width;
        //        height = tmprec.Height;
        //        Rectangle rec = new Rectangle(x, y, width, height);
        //        g.DrawRectangle(Pens.Blue, rec);

        //    }
        //}

        //private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        //{


        //    Point pt = new Point(e.X, e.Y);
        //    if (e.Button == MouseButtons.Left && isSreenShot)
        //    {
        //        startpoint = pt;
        //        startpoint.Offset(lefttop);
        //        //isDraw = true;
        //    }
        //    else if (e.Button == MouseButtons.Left && !isSreenShot)
        //    {
        //        //MessageBox.Show("1");
        //        Rectangle rec = new Rectangle(pt.X, pt.Y, newRecsize.Width, newRecsize.Height);
        //        featherRgnstruct tmpfeather = new featherRgnstruct();
        //        tmpfeather.featherNum = 1;
        //        tmpfeather.rec = rec;
        //        g.DrawRectangle(Pens.Blue, new Rectangle(rec.X, rec.Y, newRecsize.Width, newRecsize.Height));
        //        featureRegion.Add(tmpfeather);
        //    }
        //    else if (e.Button == MouseButtons.Right && !isSreenShot)
        //    {
        //        backupfeathureRegion.Clear();
        //        //MessageBox.Show("2");
        //        foreach (featherRgnstruct tmp in featureRegion)
        //        {
        //            Rectangle rec = new Rectangle(e.X, e.Y, 1, 1);
        //            if (rec.IntersectsWith(tmp.rec))
        //            {
        //                CassGraphicsSystem.Project.featherValue dialog = new CassGraphicsSystem.Project.featherValue();
        //                dialog.setTextbox(tmp.featherNum.ToString());
        //                //dialog.Show();
        //                //tmp.featherNum= dialog.textbox;
        //                featherRgnstruct tmpfeather = new featherRgnstruct();
        //                if (dialog.ShowDialog() == DialogResult.OK)
        //                {
        //                    if (dialog.textbox != 0)
        //                    {
        //                        tmpfeather.featherNum = dialog.textbox;
        //                        tmpfeather.rec = tmp.rec;
        //                        backupfeathureRegion.Add(tmpfeather);
        //                    }
        //                }
        //                else
        //                {
        //                    backupfeathureRegion.Add(tmp);

        //                }
        //            }
        //            else
        //                backupfeathureRegion.Add(tmp);
        //        }
        //        featureRegion.Clear();
        //        foreach (featherRgnstruct tmp in backupfeathureRegion)
        //        {
        //            featureRegion.Add(tmp);
        //        }
        //    }

        //}
        
        //private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        //{
        //    Graphics gg = (sender as PictureBox).CreateGraphics();
        //    if (isSreenShot)
        //    {
        //        (sender as PictureBox).Invalidate();
        //        gg.DrawRectangle(Pens.Gray, new Rectangle(startpoint, new Size(e.X - startpoint.X, e.Y - startpoint.Y)));

        //    }
        //    //gg.Dispose();
        //}

        //private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (isSreenShot)
        //    {
        //        endpoint = new Point(e.X, e.Y);
        //        endpoint.Offset(lefttop);
        //        DialogResult result = MessageBox.Show("是否确定所取区域", "特征区获取", MessageBoxButtons.OKCancel);
        //        if (result.Equals(DialogResult.OK))
        //        {
                 
        //            lefttop = startpoint;
        //            rightbottom = endpoint;
        //            righttop = new Point(endpoint.X, startpoint.Y);
        //            leftbottom = new Point(startpoint.X, endpoint.Y);
        //            Bitmap tmpbitmap = CutImage(BoundBitmap, lefttop.X, lefttop.Y, righttop.X - lefttop.X, leftbottom.Y - lefttop.Y);
        //            (sender as PictureBox).Image = tmpbitmap;
        //            BoundBitmap = tmpbitmap;
        //            backspacing.Enabled = true;
        //            if (tmpbitmap.Width > 250)
        //            {

        //                fm.Size = new Size(tmpbitmap.Width + 10, tmpbitmap.Height + 60);
        //            }
        //            else
        //            {
        //                fm.Size = new Size(250 + 10, tmpbitmap.Height + 60);
        //            }
        //        }

        //    }
        //    isSreenShot = false;


        //}

        //public static Bitmap CutImage(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        //{
        //    if (b == null)
        //    {
        //        return null;
        //    }

        //    int w = b.Width;
        //    int h = b.Height;

        //    if (StartX >= w || StartY >= h)
        //    {
        //        return null;
        //    }

        //    if (StartX + iWidth > w)
        //    {
        //        iWidth = w - StartX;
        //    }

        //    if (StartY + iHeight > h)
        //    {
        //        iHeight = h - StartY;
        //    }

        //    try
        //    {
        //        Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);

        //        Graphics g = Graphics.FromImage(bmpOut);
        //        g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
        //        g.Dispose();

        //        return bmpOut;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        #endregion

        #region 2013.11.22  图像处理

        //pc机仿真
        private void ToolStripMenuItem_Simulation_Click(object sender, EventArgs e)
        {
            ModeSelect(CompileMode.SimulationMode);
        }
        /// <summary>
        /// 调用批处理，生成.exe
        /// </summary>
        private bool  PCMakeBat(string batName)
        {          
            try
            {

                Process p = new Process();
                p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + batName;             
                p.StartInfo.Arguments = currentProjectPath + "  " + ProjectName + "  " + ProgramPath;//传入参数（工程路径、工程名、系统路径）
                p.StartInfo.UseShellExecute = false;  //以管理员身份打开
                p.StartInfo.CreateNoWindow = true; //设置不显示示dos窗口

            //    p.StartInfo.RedirectStandardInput = true;  // 重定向输入
                p.StartInfo.RedirectStandardOutput = true;  //   重定向输出
                p.StartInfo.RedirectStandardError = true;    //  重定向输出错误

                //启动进程
                p.Start();


             //   StreamReader reader = p.StandardOutput;  //截取输出流

           //     string errorInfo =p.StandardError.ReadToEnd();//截取错误流
           //     if(errorInfo!="")
           //         CassMessageBox.Warning(errorInfo);

         //       this.errorForm.errorList.Add(new string[] { null, errorInfo, "error", null });
                //string line = null;
                //while (!reader.EndOfStream)
                //{
                //    line = reader.ReadLine();
                //    this.errorForm.errorList.Add(new string[] { null, line, "info", null });

                //}

                //while (!readerErr.EndOfStream)
                //{
                //    line = reader.ReadLine();
                //    this.errorForm.errorList.Add(new string[] { null, line, "error", null });
                //}



                //添加重定向输出 事件
             //   p.OutputDataReceived += new DataReceivedEventHandler(PCMakeBat_OutputDataReceived);
             //   p.ErrorDataReceived += new DataReceivedEventHandler(PCMakeBat_ErrorDataReceived);
            //    p.BeginOutputReadLine();
              //  p.BeginErrorReadLine();
                
                
                p.WaitForExit(); //等待结束（很重要！）
                p.Close();
                p.Dispose();
               // Thread.Sleep(200);
                return true;
         
            }
            catch (Exception e)
            {
                MessageBox.Show("批处理错误："+e.ToString());
                return false;
            }

        }

        #region   进程重定向输出  2014.02


        /// <summary>
        /// 进程输出数据——接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PCMakeBat_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                this.errorForm.errorList.Add(new string[] { null, e.Data, "info", null });
            }
        }
        /// <summary>
        /// 进程错误数据——接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PCMakeBat_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                   this.errorForm.errorList.Add(new string[] { null, e.Data, "error", null });
            }
        }
        //定义委托——显示进程输出
    //    private delegate void showTextDelegate( string line);

        //private void showText( string line)
        //{
        //    //if (tb.InvokeRequired)
        //    //{
        //    //    //声明委托
        //        showTextDelegate d = showText;
        //        object[] a = { tb, line };
        //        tb.Invoke(d, a);
        //    }
        //    else
        //    {
        //        this.errorForm.errorList.Add(new string[] { null, line, "error", null });
        //    }
        //}  


        #endregion


        #region 编译前——预处理

        /// <summary>
        /// 预处理
        /// </summary>
        private void PreProcess()
        {
            SetControlPath();  //设置控件地址
            this.RefreshComConfigInfo();//读取设备配置文件中的参数
            this.ResetDebugMark(); //调试前初始化各状态
            this.ClearDebugResult();
        }
       

        /// <summary>
        /// 设置控件地址
        /// </summary>
        private void SetControlPath()
        {
            CassView curCas = (CassView)(currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false));
            foreach (Control tmp in curCas.Controls)
            {
                PropertyDescriptor currentProjectPath = TypeDescriptor.GetProperties(tmp)["ProjectPath"];
                if (currentProjectPath != null)
                {
                    currentProjectPath.SetValue(tmp, CassViewGenerator.currentProjectPath);

                }
            }
        }

        /// <summary>
        /// 刷新设备配置文件
        /// </summary>
        private void RefreshComConfigInfo()
        {
            CassView curCas = (CassView)(currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false));
            for (int i = 0; i < curCas.ctrlsInfo.Count; i++)
            {
                ControlInfo tmp = curCas.ctrlsInfo[i];

                if (tmp.ControlName.ToUpper().StartsWith("CAMERAIN"))
                {

                    //从设备配置文件中读取数据
                    ArrayList contentList = FileOperator.ReadFromFile(this.savePath + "\\" + PublicVariable.CompileFileName);
                    if (contentList != null && contentList.Count > 5)
                    {
                        string portName = contentList[0].ToString();  //设备号
                        string baudRate = contentList[1].ToString();
                        string dataBits = contentList[2].ToString(); //数据位
                        string stopBits = contentList[3].ToString();  //停止位
                        string parity = contentList[4].ToString(); //校验位


                        //对数据进行赋值
                        List<XProp> visibleFunProperty = tmp.VisibleFunctionProperty;
                        for (int j = 0; j < visibleFunProperty.Count; j++)
                        {
                            XProp visibleXProp = visibleFunProperty[j];
                            if (visibleXProp.Name == "设备号")
                            {
                                XProp.SetValue(portName, "portName", curCas.ctrlsInfo[i].VisibleFunctionProperty);
                            }

                        }
                        List<XProp> unVisibleFunProperty = tmp.UnvisibleFunctionProperty;
                        for (int j = 0; j < unVisibleFunProperty.Count; j++)
                        {
                            XProp unVisibleXProp = unVisibleFunProperty[j];
                            switch (unVisibleXProp.Name)
                            {
                                case "波特率":
                                    XProp.SetValue(baudRate, "baudRate", curCas.ctrlsInfo[i].UnvisibleFunctionProperty);
                                    break;
                                case "数据位":
                                    XProp.SetValue(dataBits, "dataBits", curCas.ctrlsInfo[i].UnvisibleFunctionProperty);
                                    break;
                                case "停止位":
                                    XProp.SetValue(stopBits, "stopBits", curCas.ctrlsInfo[i].UnvisibleFunctionProperty);
                                    break;
                                case "校验位":
                                    XProp.SetValue(parity, "parity", curCas.ctrlsInfo[i].UnvisibleFunctionProperty);
                                    break;
                            }
                        }
                    }
                }

            }
        }


        #endregion

        ///// <summary>
       /////串口设备配置信息
       ///// </summary>
       // public struct SerialConfigInfo{
       //     string portName;   //设备号
       //     string baudRate;   //波特率
       //     string dataBist;  //数据位
       //     string stopBist;  //停止位
       //     string parity; //校验位
           
       // };
       


        #region   仿真调试

        #region 调试按钮 点击事件

        //"循环调试运行LoopInRun"点击事件 2014.1.10
        private void RunToolStripButton_Click(object sender, EventArgs e)
        {
            //切换页面
            this.SwitchBottomTabPage(this.tab_EffectPicManager);
            
            //判断当前有很多条并行操作
            if (GenerateCode.CtrlsList.Count > 1)
            {
                //获取当前页面中已选择的对象集合并存于数组selectObject
                object[] selectObject = new object[CurrentDocumentsHostControl.HostDesign.SelectedObjects.Count];
                CurrentDocumentsHostControl.HostDesign.SelectedObjects.CopyTo(selectObject, 0);
                //当前是否有选中的对象为空或者不止一个对象  
                if (selectObject[0].GetType().ToString().EndsWith("CassView") || selectObject.Length != 1)
                {
                    CassMessageBox.Information("当前存在多条并行流程！\n 请选择当前页面中的一个控件，我们将对该控件所在的流程进行循环仿真！");
                    return;
                }
                else
                {
                    //获取所选择对象的序号
                    string index = TypeDescriptor.GetProperties(selectObject[0])["SerialNumber"].GetValue(selectObject[0]).ToString();
                    foreach (List<string> ctrlsNum in GenerateCode.CtrlsList)
                    {
                        if (ctrlsNum.Contains(index))
                        {
                            //设置这条流程图的循环
                            this.CurControlsNum = ctrlsNum;
                            break;
                        }
                    }
                }
            }
            else
            {
                this.CurControlsNum = GenerateCode.CtrlsList[0];
            }
            this.picWin.Init();  //初始化
            this.bgw_processbar.Visible = true;  //进度条可见
            //按钮使能 控制
            this.ChangeDebugRunToolStripEnable(false);
            this.StopDebugToolStripButton.Enabled = true;
            //标记“循环调试”
            this.isRunInLoop = true;
            this.isStopDebug = false;
          
            //启动后台进程
            if (!this.backGroundWorker.IsBusy)
            {
                this.backGroundWorker.RunWorkerAsync();
            }
           
        }

        //“单步调试 Step” 点击事件
        private void DebugByStepToolStripButton_Click(object sender, EventArgs e)
        {
            this.bgw_processbar.Visible = true;

            this.picWin.Init(); //初始化

            this.bgw_processbar.Value = 30;
            
            this.CallProjectExe();//调用一次工程.exe

            this.bgw_processbar.Value = 50;
            if (this.ShowInputPic(false,"-1"))
            {
              //  this.picWin.SetPicBox("output", null);  //输出图片置空
                this.picWin.SetOutputPic(null);
                this.isDebugByStep = true; //标记单步调试状态             
                this.StopDebugToolStripButton.Enabled = true;  //“停止调试”按钮 使能
                //切换页面
                this.SwitchBottomTabPage(this.tab_EffectPicManager);
            }
            else //错误未设置输入图片
            {
                //按钮使能
                this.StopDebugToolStripButton.Enabled = false;
               
            }
            this.bgw_processbar.Value = 90;
            this.ChangeDebugRunToolStripEnable(false);
            this.bgw_processbar.Value = 100;
            Thread.Sleep(100);
            this.bgw_processbar.Visible = false;
            
        }


        //“停止调试 stop”  点击事件
        private void StopDebugToolStripButton_Click(object sender, EventArgs e)
        {
                       
            //按钮使能
            this.StopDebugToolStripButton.Enabled = false;
            this.ChangeDebugRunToolStripEnable(true);
            
            //判断 “是否是循环调试” 且“后台进程是否取消”
            if (this.isRunInLoop/* && this.backGroundWorker.WorkerSupportsCancellation*/)
            {
                ////取消后台进程  异步操作
                this.backGroundWorker.CancelAsync();         
                this.isStopDebug = true;                
                this.isRunInLoop = false; //标记通知“循环调试”
                this.CloseEffectPic();    //关闭效果图            
             //   this.picWin.SetDebugStatus("null",true);
            }
            else
            {

                this.isDebugByStep = false;//关闭单步调试
                this.CloseEffectPic();//关闭效果图
                this.picWin.ClearInspectionStatus(); //关闭描述状态
              //  this.picWin.SetDebugStatus("null",false);
            }
            

        }
        /// <summary>
        /// 调用工程.exe
        /// </summary>
        /// <returns></returns>
        private bool CallProjectExe()
        {
            this.ClearDebugResult();
            return this.PCMakeBat("CallExe.bat");

            #region 此方法“ 调用cmd.exe”将导致异步进行，同时存在很多个cmd.exe，导致资源浪费
            //try
            //{
            //    Process p = new Process();
            //    p.StartInfo.FileName = "cmd.exe";
            //    p.StartInfo.UseShellExecute = false;
            //    p.StartInfo.RedirectStandardInput = true;   // 重定向输入
            //    p.StartInfo.RedirectStandardOutput = true;  //   重定向输出
            //    p.StartInfo.RedirectStandardError = true;   //  重定向输出错误
            //    p.StartInfo.CreateNoWindow = true;          //设置置不显示示窗口

            //    //string cmdStr = null;
            //    //cmdStr = "cd /d "+ currentProjectPath;
            //    p.Start();
            //    p.StandardInput.WriteLine("cd /d " + currentProjectPath);
            //    p.StandardInput.WriteLine(ProjectName + ".exe");
               
  
            //    p.StandardInput.WriteLine("exit");
            //   // p.WaitForExit();
            //   // MessageBox.Show("1");

            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("调用工程.exe失败："  + ex.ToString());
            //    return false;
            //}
            #endregion

        }
        #endregion

        #region 效果图填充、关闭
        /// <summary>
        ///  循环调试：显示流程图的初始图片和最后效果图
       /// </summary>
        /// <param name="isLoop">是否是循环调试</param>  
        private bool ShowEffectPic(bool isLoop)
        {
            //获取输入控件，读取文件所在路径，设置InputPic
            //获取输入控件的模块号，读取文件所在路径，设置InputPic
            string inputIndex = this.CurControlsNum[0];
            this.picWin.InputPicIndex = inputIndex;  //20140419
            string inputPicPath = this.GetSourceFile(inputIndex);
           
            if (!File.Exists(inputPicPath))
            {
                return false;
            }
            else
            {
              //  this.picWin.SetPicBox("input", inputPicPath);
                this.picWin.SetInputPic(inputPicPath);
                
                //获取最后一个控件，读取文件所在路径，设置OutputPic
                int lastIndex = this.CurControlsNum.Count - 2;
                if (this.ExistsMatchControl("MATCH")) //
                {
                    lastIndex--;
                    this.DoMatchOper(true); //执行匹配操作  

                }
              
                string outPicPath = currentProjectPath + "\\out\\" + this.CurControlsNum[lastIndex] + ".bmp";
                this.ShowOutputPic(outPicPath);
                return true;

            }
        }
        /// <summary>
        /// 显示输入图片(默认是原始图片，即FileIn选择文件)
        /// </summary>
        private bool ShowInputPic(bool isLoop,string index)
        {
            string inputIndex = null;
            //获取输入控件的模块号，读取文件所在路径，设置InputPic
            if (index == "-1")
            {
                //默认选择第一条路径的输入

                inputIndex = GenerateCode.CtrlsList[0][0];
            }
            else
            {
                inputIndex = index;
            }
       //     string inputIndex = GenerateCode.ctrlsNum[0];
            this.picWin.InputPicIndex = inputIndex;
            string inputPicPath = this.GetSourceFile(inputIndex);
         //   bool result = false;

            //Control inputControl = FindControlById(inputIndex);
            ////判断当前控件类型是“设备输入”
            //if (inputControl.GetType().ToString().ToUpper().Contains("CAMERAIN"))
            //{

                
            //}
            if (inputPicPath == null||!File.Exists(inputPicPath))  //停止调试
            {             
               // this.picWin.SetDebugStatus("null", false);
                this.isStopDebug = true;
              //  MessageBox.Show("图片不存在,请检查文件是否存在!");
                return false;
            }
           
            // result = this.picWin.SetPicBox("input", inputPicPath);
            this.picWin.SetInputPic(inputPicPath);
            this.picWin.Refresh();         
          //  return result;
            return true;
        }
        /// <summary>
        ///  单步调试：显示输出图片
        /// </summary>
        /// <param name="filePath">文件路径</param>
        private void ShowOutputPic(string filePath)
        {
            //调试错误，内部提示
         //   this.picWin.SetPicBox("output", filePath);
            this.picWin.SetOutputPic(filePath);
            this.picWin.Refresh();

         }

        /// <summary>
        /// 关闭调试图片
        /// </summary>
        private void CloseEffectPic()
        {
            this.picWin.FreePicBox();
        }

        //private void ClearPicInspection()
        //{
        //    this.picWin.ClearInspectionStatus();
        //}


        #endregion

        #region 辅助操作
        /// <summary>
        /// 切换底部bottomTabControl页面
        /// </summary>
        /// <param name="tPage">根据tabPage的Name</param>
        private void SwitchBottomTabPage(MTabPage tPage)
        {
            this.bottomTabList.SelectedTab = tPage;
          //  this.bottomTabList.Refresh();
            if(tPage == this.tab_EffectPicManager) //效果图切换同时，切换到Main页面
            {
                this.SwitchMainPage();
            }
        }

        /// <summary>
        /// 切换Main页面
        /// </summary>
        private void SwitchMainPage()
        {
            for (int k = 0; k < this.tabControlView.TabCount; k++)
            {
                if (this.tabControlView.TabPages[k].Text == MainPageName)
                {//找到指令页面
                    this.tabControlView.SelectedTab = tabControlView.TabPages[k];
                    break;
                }
            }
        }

        /// <summary>
        /// 判断是否符合运行和单步运行的条件(仿真模式且编译成功)
        /// </summary>
        /// <returns>bool</returns>
        //private bool IsMatchDebugRun()
        //{
        //    if (!isSimulationMode)    //仿真模式
        //        return false;
        //    else if(!isComplySuccess)     //编译成功
        //        return false;
        //    else
        //         return true;
        //}


       
        /// <summary>
        /// 执行匹配操作
        /// </summary>
        /// <param name="isRunInLoop">标记是否是循环调试</param>
        private void DoMatchOper(bool isRunInLoop)
        {

            this.picWin.DoMatchOperation(currentProjectPath, isRunInLoop);
            //string content = FileOperator.ReadFromFile2(currentProjectPath + "\\result.txt");
            //if (content != null)
            //{

            //    string[] resMatch = content.Split('\n');
                
            //    int sysPercent = 50;  //系统默认值为50
            //    int curPercent = -1;  //当前匹配率为-1
            //    if (resMatch.Length > 2)
            //    {

            //        sysPercent = Int32.Parse(resMatch[0]);//读取系统匹配率
            //        curPercent = Int32.Parse(resMatch[1]); //读取当前匹配率
            //    }

            //    if (curPercent >= sysPercent)  //当前匹配率>=系统匹配率
            //    {
            //      //  this.picWin.SetDebugStatus("pass", isRunInLoop);
            //    }
            //    else
            //    {
            //      //  this.picWin.SetDebugStatus("fail", isRunInLoop);
            //    }

            //    if (isRunInLoop)
            //    {
            //        File.Delete(currentProjectPath + "\\result.txt");
            //    }
            // //   this.picWin.setMatch(curPercent);
            // //   this.picWin.ShowCounter();
               
            //}
            
        }
     
        
        /// <summary>
        /// 改变调试运行的使能键  
        /// </summary>
        /// <param name="flag">bool</param>
        private void ChangeDebugRunToolStripEnable(bool flag)
        {
            this.RunToolStripButton.Enabled = flag;
            this.DebugByStepToolStripButton.Enabled = flag;
        }

        /// <summary>
        /// 判断是否存在控件（可扩展到查询任何控件是否存在）
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private bool ExistsMatchControl(string control)
        {
            CassView curCas = (CassView)(currentTabPage.Controls[0].GetNextControl(currentTabPage.Controls[0], false));
            foreach (Control tmp in curCas.Controls)
            {
                PropertyDescriptor pConName = TypeDescriptor.GetProperties(tmp)["Controlname"];
                string index = TypeDescriptor.GetProperties(tmp)["SerialNumber"].GetValue(tmp).ToString();
                if (pConName != null&& this.CurControlsNum.Contains(index))
                {
                    //此处字符串都统一为大写后进行比较
                    string cName = tmp.GetType().ToString().ToUpper();
                    if (cName.Contains(control.ToUpper()))
                        return true;
                }
            }
            return false;
        }   


        /// <summary>
        /// 根据控件序号寻找cassView中对应的控件
        /// </summary>
        /// <param name="controlNum">控件序号（唯一）</param>
        /// <returns>Control</returns>
        private Control FindControlById(string controlNum)
        {
            CassView cassView = (CassView)(this.tabControlView.TabPages[0].Controls[0].GetNextControl(this.tabControlView.TabPages[0].Controls[0], false));
            foreach (Control temp in cassView.Controls)
            {
                PropertyDescriptor pSerialNumber = TypeDescriptor.GetProperties(temp)["SerialNumber"];
                if (pSerialNumber.GetValue(temp).ToString() == controlNum)
                    return temp;
            }
            return new Control();
        }

       /// <summary>
       /// 根据“输入控件”序号获取输入图片的完整地址
       /// </summary>
       /// <param name="controlNum"></param>
       /// <returns></returns>
        private string GetSourceFile(string controlNum)
        {

            Control inputControl = FindControlById(controlNum);
            string inputPicPath = null;
                      
            //判断当前控件类型是“设备输入”--2014.2.15
            if (inputControl.GetType().ToString().ToUpper().Contains("CAMERAIN"))
            {
                inputPicPath = currentProjectPath + "\\out\\" + controlNum + ".bmp";

            }
            //判断当前控件类型是“文件输入”
            else if (inputControl.GetType().ToString().ToUpper().Contains("FILEINPUT"))
            {
                PropertyDescriptor pImage = TypeDescriptor.GetProperties(inputControl)["FilePath"];
                if (pImage != null)
                {
                    string fileName = pImage.GetValue(inputControl).ToString();
                    //去双引号          
                    fileName = fileName.Replace("\"", "");
                    inputPicPath = currentProjectPath + "\\" + fileName;
                }
            }
            
            return inputPicPath;
           
        }
        
        #endregion

        #region   循环调试（采用后台进程）

        //在后台线程中运行的，所以在该事件中不能够操作用户界面的内容
        private void backGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
           // int i = 0;
            worker.ReportProgress(10);
            while (!isStopDebug)
            {
                //当进程取消 或者 调用exe出错
                if (worker.CancellationPending )  
                {
                    e.Cancel = true;
                    isStopDebug = true;
                    break;
                }
                if (!this.CallProjectExe())
                {
                    isStopDebug = true;
                    e.Cancel = true;
                    break;
                }
                else
                {

                    worker.ReportProgress(50);
                    Thread.Sleep(100);
                   
                }
            }

        }

        //进程对应的进度条(异步)
        private void backGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
            //显示效果图
            if (!isStopDebug)
            {
                //int count = e.ProgressPercentage;
                //显示进度
                this.bgw_processbar.Value = e.ProgressPercentage;


                //while (!ShowEffectPic(true))
                //{
                //    if (count > 90)
                //        count = 90;
                //    else
                //        count++;
                //   this.bgw_processbar.Value = count;
                //}

                //this.bgw_processbar.Value = 100;
                //Thread.Sleep(200);
                if (ShowEffectPic(true))
                {
                    this.bgw_processbar.Value = 100;
                    Thread.Sleep(300);
                    this.bgw_processbar.Value = 10;
                    Thread.Sleep(200);
                    this.bgw_processbar.Value = 20;
                }
             
            }

        }
        //进程完成后调用
        private void backGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (e.Cancelled)
            //{               
            //    MessageBox.Show("调试停止！");
            //}
           
            if (isStopDebug)
            {
                this.StopDebugToolStripButton.Enabled = false; //按钮使能

                this.ChangeDebugRunToolStripEnable(true);
                this.bgw_processbar.Visible = false;
              //  MessageBox.Show("调试停止！");

            }
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            if (!this.backGroundWorker.IsBusy)
            {
                MessageBox.Show("调试已停止！");
            }
            
        }

        #endregion

        #region 清理标记和按钮复位
        /// <summary>
        /// 调试前初始化
        /// </summary>
        private void ResetDebugMark()
        {
          //  this.isComplySuccess = false;//待定
            if (this.backGroundWorker.IsBusy)
                this.backGroundWorker.CancelAsync();
            this.isRunInLoop = false;  //循环调试标记位
            this.isStopDebug = false;   //停止调试标记位
            this.isDebugByStep = false; //单步调试标记位
            //按钮使能  false
            this.ChangeDebugRunToolStripEnable(false);
            this.StopDebugToolStripButton.Enabled = false;
            //关闭相关描述
            this.CloseEffectPic();  //关闭显示的效果图
            this.picWin.Init();
          //  this.picWin.SetDebugStatus("null", false);
           
        }
        /// <summary>
        /// 删除上一次调试的缓存结果
        /// </summary>
        private void ClearDebugResult()
        {
            File.Delete(currentProjectPath + "\\result.txt");
            FileOperator.DeleteFiles(currentProjectPath + "\\out", ".bmp");
        }

        #endregion



        #endregion



        #region   工程分类_下拉菜单_点击事件
        private void 工件识别ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = this.tabControlView.TabPages.Count;
            if (count == 0)
            {
                 CassMessageBox.Information("请先新建或打开工程！");
                 return;
            }
            
            ClassifyWorkpiece classifyworkpiece = new ClassifyWorkpiece(savePath,ProgramPath);         
            classifyworkpiece.Show();
        }

        private void 数字识别ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = this.tabControlView.TabPages.Count;
            if (count == 0)
            {
                CassMessageBox.Information("请先新建或打开工程！");
                return;
            }
            ClassifyNumber classifynumber = new ClassifyNumber(savePath,ProgramPath);  
            classifynumber.Show();
        }

        private void 缺陷检测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        #endregion

        private void ResetWorkSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetWorkSpaceForm resetWorkSpaceForm = new ResetWorkSpaceForm(WorkSpacePath);
            resetWorkSpaceForm.dele_workSpace += new ResetWorkSpaceForm.workSpaceMsg(ResetWorkSpace);
            resetWorkSpaceForm.Show();
        }

        //重置工作目录 20140217
        private void ResetWorkSpace(string msg)
        {
            //重置全局的工作目录
            WorkSpacePath = msg;

            //清空当前资源管理器中的资源(包括重新加载ToolXML文件)
            ClearResource();

            //将工作目录信息保存在编程目录下的系统文件中
            ArrayList arrWriteValue = new ArrayList();
            arrWriteValue.Add(WorkSpacePath);
            FileOperator.WriteToFile(ProgramPath + PublicVariable.sysInfoFileName, arrWriteValue);

            //打开新工作目录下的所有工程——更新解决方案资源管理器
            OpenProjects();

        }
        #endregion

      
     

    }//class
}