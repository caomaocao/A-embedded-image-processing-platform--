/**********************************************************************************************************
           ** Copyright (C) 2006 CASS 版权所有
 * 
           ** 文件名：HostDesign.cs 
 * 
           ** 功能描述：
 * 
           **          主要用于提供设计器的界面设计操作，继承05系统本身的设计组建的用户界面类DesignSurface
 *               类继承系统本身的设计组建的用户界面类DesignSurface。
 * 
 *                      该类中引用了自定义控件FilteredPropertyGrid,通过容器的添加服务和得到服务，使其与主页面上的显示
 *               属性控件controlfilteredPropertyGrid进行实时绑定。类中对CassView控件和其他的控件的显示属性进行了区别的设置。
 *               CassView控件屏蔽了名称这一属性.
 *                      
 *                      添加了对选中控件的鼠标显示操作和当前CassView上点击鼠标时的位置显示,和属性显示控件一样,
 *               通过容器的添加服务和得到服务的机制来实现(在CassViewGenerator中将用到的控件添加到该设计器的服务容器中).
 * 
 *                      在设计器中，添加了UndoEngineImplication类型的服务，该服务能记录当前设计器的操作，并进行撤消和重复的操作。
 *              在该类中，在当前控件发生LocationChanged 和SizeChanged时，在设计器上拖入新的控件，以及改变设计器根实例的Resize时开启撤消/重复操作。
             *  但该内容存在速度慢的问题，故现在先将该功能做Disabled处理。
 * 
 *                      对设计器中被选择的控件发生变化时显示当前控件的属性，并对Cass系统不需要的属性进行过滤操作，显示所需的属性。
 * 
 *                      添加了控件的组合功能，对每一组的组合控件都为PublicVariabl.GroupStruct结构，放在groupList链表中。
 * 
 * 
           ** 作者：吴丹红
           ** 创始时间：2006-11-8
           ** 
*************************************************************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CaVeGen.DesignViewFiles;
using System.Configuration;
using CaVeGen.CommonOperation;
using CaVeGen.DesignViewFiles.FilterProperty;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.IO;
using System.Reflection;


namespace CaVeGen.DesignViewFiles
{
    /// <summary>
    /// 该类主要实现设计器中的一些初始化操作和当前控件对象发生改变时的事件服务，继承了DesignSurface类。
    /// </summary>
    public class HostDesign : DesignSurface
    {
        private object senderSubject = null;                       //当前属性中显示的对象
        private IDesignerHost host;                                 //当前设计器的接口
        public ISelectionService selectionService = null;          //选择控件的服务
        private ICollection selectedComponents = null;              //当前选中控件的对象集
        private object[] selections = null;                         //当前选中的对象
        private FilteredPropertyGrid cassPropertyGrid = null;       //属性控件
        private ContextMenuStrip operateMenu = null;                //当前的右键单击时显示点菜单
        private ToolStripStatusLabel designMousePosition = null;    //显示鼠标位置的工具条
        private UndoEngineImplication undoEngine = null;            //撤消/重复操作
        private ToolStripMenuItem editToolMenuItem = null;          //编辑菜单

        //private const int undoIndex = 0;       //编辑菜单中撤消的索引为0
        private const int redoIndex = 1;         //编辑菜单中重复的索引为1
        private const int cutIndex = 2;          //编辑菜单中剪切的索引为2
        private const int copyIndex = 3;         //编辑菜单中复制的索引为3
        //private const int pastIndex = 4;       //编辑菜单中粘贴的索引为4
        private const int deleteIndex = 5;       //编辑菜单中删除的索引为5
        //private const int selectAllIndex = 6;  //编辑菜单中全选的索引为6
        private const int viewIndex = 7;         //编辑菜单中视图的索引为7

        private bool lostFouce = false;          //标志当前设计器是否失去焦点,false:未市区 
        private object currentObject = null;
        private ArrayList addedObject = new ArrayList();  //添加的控件暂时放在队列中不显示
        private bool selectedChangedFlag = false;       //选中对象进行标志

        /// <summary>
        /// 添加菜单服务
        /// </summary>
        public HostDesign()
        {
            this.AddService(typeof(IMenuCommandService), new MenuCommandService(this));
        }

        public HostDesign(IServiceProvider parentProvider)
            : base(parentProvider)
        {
            this.AddService(typeof(IMenuCommandService), new MenuCommandService(this));
        }

        /// <summary>
        /// 初始化具有HostDesign类的对象，加载当前所选控件变化时的事件服务和消息处理。
        /// 得到的服务：属性控件、上下文菜单、状态栏、标签控件（TabControl）、菜单栏、
        /// 添加的服务：撤消/重复
        /// </summary>
        internal void Initialize()
        {
            host = (IDesignerHost)this.GetService(typeof(IDesignerHost));

            if (host == null)
            {
                return;
            }
            try
            {
                selectionService = (ISelectionService)(this.GetService(typeof(ISelectionService)));
                selectionService.SelectionChanged += new EventHandler(selectionService_SelectionChanged);
                if (host.RootComponent != null)
                {
                    ((Control)host.RootComponent).Resize += new EventHandler(CassView_Resize);
                }
                cassPropertyGrid = (FilteredPropertyGrid)(this.GetService(typeof(FilteredPropertyGrid)));
                operateMenu = (ContextMenuStrip)(this.GetService(typeof(ContextMenuStrip)));
                designMousePosition = (ToolStripStatusLabel)(this.GetService(typeof(ToolStripStatusLabel)));
                editToolMenuItem = (ToolStripMenuItem)this.GetService(typeof(ToolStripMenuItem));
                
                //添加撤消/重复操作服务
                IServiceContainer serviceContainer = host.GetService(typeof(ServiceContainer)) as IServiceContainer;
                undoEngine = new UndoEngineImplication(serviceContainer);
                undoEngine.Enabled = false;    //关闭撤消和重复功能
                host.AddService(typeof(UndoEngineImplication), undoEngine);
            }
            catch (Exception ex) { }
        }

        #region  设计器本身操作函数

        /// <summary>
        /// 当设计器的根组件的基实例(即CassView的实例)的大小发生变化时,重新刷新属性显示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CassView_Resize(object sender, EventArgs e)
        {
            designMousePosition.Text = "行 " + ((Control)host.RootComponent).Bounds.Width
                + "," + "列 " + ((Control)host.RootComponent).Bounds.Height;            //实时显示鼠标位置 
            cassPropertyGrid.Refresh();
        }

        /// <summary>
        /// 控件位置改变事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void senderSubject_LocationChanged(object sender, EventArgs e)
        {
            //((Control)sender).BringToFront();
            cassPropertyGrid.Refresh();
        }

        /// <summary>
        /// 控件大小改变事件处理函数
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void senderSubject_SizeChanged(object sender, EventArgs e)
        {
            cassPropertyGrid.Refresh();
        }
        
        #endregion

        /// <summary>
        /// 在设计器中添加类型的对象进行操作的服务
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="serviceInstance">服务对象</param>
        public void AddService(Type type, object serviceInstance)
        {
            this.ServiceContainer.AddService(type, serviceInstance);
        }

        /// <summary>
        /// 获取是否失去焦点
        /// false:没有失去焦点
        /// true:失去焦点
        /// </summary>
        public bool HostFocuse
        {
            get
            {
                return lostFouce;
            }
            set
            {
                lostFouce = value;
            }
        }

        #region 当前选中的控件发生变化时

        /// <summary>
        /// 控件发生变化时，得到该控件的属性，调用SelectionControl()函数。
        /// 对于选中的控件，判断其是否处于组合状态，如果是，则同时将组合中的其他控件处于选中状态
        /// 对于通过控件组合的控件，则对其进行进行选中时，一起显示
        /// </summary>
        private void selectionService_SelectionChanged(object sender, EventArgs e)
        {
            if (selectedChangedFlag == true)
            {
                return;
            }//end if (selectedChangedFlag == true)

            List<object> selectList = new List<object>();  //当前选中的控件
            if (senderSubject != null && !senderSubject.GetType().ToString().Equals(PublicVariable.viewName))
            {
                //除去动态委托,以实时显示控件的变化了的属性
                ((Control)senderSubject).LocationChanged -=
                    new EventHandler(senderSubject_LocationChanged);
                ((Control)senderSubject).SizeChanged -=
                    new EventHandler(senderSubject_SizeChanged);
            }//end if (senderSubject != null && !senderSubject.GetType().ToString().Equals(PublicVariable.viewName))

            if (selectionService != null)
            {
                selectedComponents = selectionService.GetSelectedComponents();
                if (selectedComponents.Count > 0)
                {
                    selections = new Object[selectedComponents.Count];
                    selectedComponents.CopyTo(selections, 0);
                    CassViewGenerator.designSelectedObjects = selections;
                    for (int i = 0; i < selections.Length; i++)
                    {
                        if (!selectList.Contains(selections[i]))   //不包括才添加
                        {
                            selectList.Add(selections[i]);
                        }//end if (!selectList.Contains(selections[i]))
                    }//end for
                }//end if (selectedComponents.Count > 0)
                selectionService.SetSelectedComponents((ICollection)selectList);  //置选中状态
            }//end if (selectionService != null)
            SelectionControl();
        }//end function

        /// <summary>
        /// 将所选的控件的属性在主页面的属性控件中显示出来
        /// 对FilteredPropertyGrid属性控件进行设置,列出需要隐藏的分类信息和部分需要显示的具体属性信息。
        /// 如果选中的控件的个数不为0，则剪切、复制、删除可操作，
        /// 如果选中的控件的个数大于1个，则可显示区域叠加和视图操作，
        /// 如果选中的控件为全部，则即可整体叠加又可区域叠加
        /// </summary>
        public void SelectionControl()
        {
            if (selectionService != null)
            {
                selectedComponents = selectionService.GetSelectedComponents();

                lostFouce = false;  //当前设计器具有焦点
                if (selectedComponents.Count > 0)
                {
                    selections = new Object[selectedComponents.Count];
                    selectedComponents.CopyTo(selections, 0);
                    senderSubject = selections[0]; //传递对象; 即：属性框显示得是第一个选择得控件
                    CassViewGenerator.designSelectedObjects = selections;
                    SetPropertyGrid(senderSubject);
                    cassPropertyGrid.SelectedObjects = selections;
                    cassPropertyGrid.Refresh();
                    editToolMenuItem.Enabled = true;
                    if (selectedComponents.Count > 1)
                    {
                        editToolMenuItem.DropDownItems[viewIndex].Enabled = true;  //视图可操作
                    }
                }//if(selectedComponents.Count > 0)

                //绑定HostDesign中的属性控件。通过处理的AddService和GetService服务
                if (senderSubject != null)
                {
                    if (senderSubject.GetType().ToString().Equals(PublicVariable.viewName)) // cassView
                    {
                        //operateMenu.Items[bringAndSend].Enabled = false;
                        //剪切、复制、删除不可操作
                        editToolMenuItem.DropDownItems[cutIndex].Enabled = false;
                        editToolMenuItem.DropDownItems[copyIndex].Enabled = false;
                        editToolMenuItem.DropDownItems[deleteIndex].Enabled = false;
                        editToolMenuItem.DropDownItems[viewIndex].Enabled = false;  //视图不可操作
                    }
                    else
                    {
                        //添加控件位置和改变大小的委托
                        ((Control)senderSubject).LocationChanged += new EventHandler(senderSubject_LocationChanged);
                        ((Control)senderSubject).SizeChanged += new EventHandler(senderSubject_SizeChanged);

                        //剪切、复制、删除可操作
                        editToolMenuItem.DropDownItems[cutIndex].Enabled = true;
                        editToolMenuItem.DropDownItems[copyIndex].Enabled = true;
                        editToolMenuItem.DropDownItems[deleteIndex].Enabled = true;
                    }//不为CassView
                }
                else　//对象为空，则清除属性控件中的内容
                {
                    cassPropertyGrid.SelectedObject = null;
                    cassPropertyGrid.Refresh();
                }

            }//if (selectionService != null)
            else
            {
                selections = null;
            }
        }

        /// <summary>
        /// 获取或设置当前选中的控件的对象集
        /// </summary>
        public ICollection SelectedObjects
        {
            get
            {
                return selectedComponents;
            }
            set
            {
                selectedComponents = value;
            }
        }

        /// <summary>
        /// 返回根设计器 
        /// </summary>
        public Control rootControl
        {
            get
            {
                if (host != null && host.RootComponent != null)
                {
                    return (Control)host.RootComponent;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 当前创建的对象
        /// </summary>
        public object CurrentAddedObject
        {
            get
            {
                return currentObject;
            }
        }
        #endregion

        # region 控件属性过滤
        /// <summary>
        /// 设置属性,如果为CassView对象,则过滤掉"设计"这一属性
        /// </summary>
        /// <param name="control">当前需要进行属性显示的对象</param>
        private void SetPropertyGrid(object control)
        {
            if (control != null && cassPropertyGrid != null)
            {
                cassPropertyGrid.BrowsableAttributes =
                        new AttributeCollection(new Attribute[] { new CategoryAttribute("基本属性"),
                        new CategoryAttribute("功能属性")});

                //如果对象是控件则显示锁定控件的“Locked”属性
                if (control.GetType().ToString().Equals(PublicVariable.viewName))
                {
                    cassPropertyGrid.BrowsableProperties = null;
                }
                else
                {
                    cassPropertyGrid.BrowsableProperties = new string[] { "Locked" };
                }
            }
        }
        #endregion



    }//class
}//namespace


