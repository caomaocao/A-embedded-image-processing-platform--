/**********************************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����
           ** �ļ�����ToolBoxServiceImpl.cs 
           ** ����������
           **          ��Ҫ������乤�����ľ�����Ϣ����App.config�ļ��е�ÿ���ؼ���ӵ��������С�����д�� IToolboxService�ӿ��еĺ���,
 * �������������ⲿ�������ؼ�ExplorerBarTool��
 * 
 * ����ͨ����ȡ�����ļ��е�Xml�ڵ���Ϣ������Щ�ڵ���Ϣ���������ļ����з��࣬
 * ����ȡ��Ӧ�����еľ���ؼ����ͣ���øÿؼ���ͼ������Ʋ���ʾ�ڷ���������С�
 * 
           ** ���ߣ��ⵤ��
           ** ��ʼʱ�䣺2006-11-8
           ** 
************************************************************************************************/

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
using System.Reflection;
using Janus.Windows.ExplorerBar;
using CaVeGen.CommonOperation;
using System.IO;
using System.Xml;

namespace CaVeGen.DesignViewFiles
{
    public partial class ToolBoxServiceImpl : UserControl, IToolboxService
    {
        private string designPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
        public static string[] typeNameString = null;
        public static XmlDocument toolXML = new XmlDocument();

        /// <summary>
        /// ToolBoxServiceImpl�๹�캯��
        /// </summary>
        public ToolBoxServiceImpl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// װ�ع�����,��ȡ�����ļ��ķ�����Ϣ������ExplorerBarGroup����ʾ���������
        /// </summary>
        public void LoadToolBox()
        {
            toolXML.Load(Path.Combine(this.designPath, PublicVariable.ToolConfigFileName));

            this.explorerBarTool.Groups.Clear();
            if (toolXML.FirstChild != null)
            {
                for (int i = 0; i < toolXML.FirstChild.ChildNodes.Count; i++)
                {
                    if (toolXML.FirstChild.ChildNodes[i] != null
                        && toolXML.FirstChild.ChildNodes[i].Attributes[0].InnerText != null
                        && toolXML.FirstChild.ChildNodes[i].Attributes[0].InnerText != CassViewGenerator.SpecialCodeNode)
                    {
                        ExplorerBarGroup catGroup =
                            new ExplorerBarGroup(toolXML.FirstChild.ChildNodes[i].Attributes[0].InnerText);
                        this.explorerBarTool.Groups.Add(catGroup);
                        for (int j = 0; j < toolXML.FirstChild.ChildNodes[i].ChildNodes.Count; j++)
                        {
                            if (toolXML.FirstChild.ChildNodes[i].ChildNodes[j] != null)
                            {
                                string[] assemblyClass =
                                    toolXML.FirstChild.ChildNodes[i].ChildNodes[j].Attributes[0].InnerText.Split(new char[] { ',' });
                                Type toolboxItemType = GetTypeFromLoadedAssembly(assemblyClass[0], assemblyClass[1]);
                                ToolboxItem toolItem = new ToolboxItem(toolboxItemType);        //��ʾ�������䡱�е���
                                ExplorerBarItem barItem = new ExplorerBarItem(toolItem.DisplayName);
                                barItem.Image = (Image)global::CaVeGen.Properties.Resources.ResourceManager.GetObject(assemblyClass[2]);
                                barItem.Tag = assemblyClass;
                                barItem.Text = assemblyClass[2];
                                if (toolXML.FirstChild.ChildNodes[i].ChildNodes[j].LastChild.Name == "OtherInfo")
                                {
                                    if (toolXML.FirstChild.ChildNodes[i].ChildNodes[j].LastChild.ChildNodes.Count == 3)
                                    {//�ؼ�������Ϣ
                                        barItem.ToolTipText = toolXML.FirstChild.ChildNodes[i].ChildNodes[j].LastChild.ChildNodes[1].InnerText;
                                    }
                                }
                                catGroup.Items.Add(barItem);
                            }//end if (toolXML.FirstChild.ChildNodes[i].ChildNodes[j] != null)
                        }//end for (int j = 0; j < toolXML.FirstChild.ChildNodes[i].ChildNodes.Count; j++)
                    }//end if (toolXML.FirstChild.ChildNodes[i] != null)
                    else if (toolXML.FirstChild.ChildNodes[i].Attributes[0].InnerText == CassViewGenerator.SpecialCodeNode)
                    {
                        foreach (XmlNode node in toolXML.FirstChild.ChildNodes[i].ChildNodes)
                        {
                            if (node.Attributes["name"].InnerText != "ͷ�ļ�")
                            {
                                CaVeGen.DesignViewFiles.CodeEditor.PLCCodeEditor.SpecialCode.Add(node.Attributes["varname"].InnerText.ToUpper(), node.Attributes["name"].InnerText);
                            }
                        }
                    }
                }//end for (int i = 0; i < toolXML.FirstChild.ChildNodes.Count; i++)
            }//end if (toolXML.FirstChild != null)
        }//end function void LoadToolBox()

        /// <summary>
        /// ���������ͳ��򼯣��õ������ؼ������ƺ�����
        /// </summary>
        /// <param name="classname">����</param>
        /// <param name="assembly">����</param>
        /// <returns></returns>
        public Type GetTypeFromLoadedAssembly(string classname, string assembly)
        {
            Assembly[] loadedAsms = AppDomain.CurrentDomain.GetAssemblies();
            Type loadedType = null;
            Assembly foundAssembly = null;

            foreach (Assembly asm in loadedAsms)
            {
                if (string.Compare(assembly, asm.GetName().Name, true) == 0)
                {
                    foundAssembly = asm;
                    break;
                }
            }

            if (foundAssembly != null)
            {
                loadedType = foundAssembly.GetType(classname);
            }
            else
            {
                // try to load it with a partial name
                Assembly asm = Assembly.LoadWithPartialName(assembly);
                if (asm != null)
                {
                    loadedType = asm.GetType(classname);
                }
            }
            return loadedType;
        }

        /// <summary>
        /// �ú�����ʵ���˶Թ������е�ĳ���������ʱ�������¸����µĶ��󣬲�������õ����λ�����ڵ������������
        /// </summary>
        /// <param name="sender">�¼�����</param>
        /// <param name="e">�������¼�����</param>
        private void explorerBarTool_MouseDown(object sender, MouseEventArgs e)
        {
            Type toolboxItemType = null;
            ToolboxItem toolItem;
            ExplorerBarItem node = new ExplorerBarItem();

            try
            {
                CassViewGenerator.listViewSelectIndex = -1;      //���������ʶ���Ǵ�Listview���ϳ��Ķ���
                if (sender is ExplorerBar && e.Clicks < PublicVariable.DClick)
                {
                    ExplorerBarItem clickedNode = explorerBarTool.GetItemAt(e.X, e.Y);
                    if (clickedNode != null)
                    {
                        typeNameString = (string[])clickedNode.Tag;     //��������ToolboxItem���ͱ���
                        toolboxItemType = GetTypeFromLoadedAssembly(typeNameString[0], typeNameString[1]);
                        toolItem = new ToolboxItem(toolboxItemType);
                        DataObject dataObj = SerializeToolboxItem(toolItem) as DataObject;      //��������
                        DoDragDrop(dataObj, DragDropEffects.Copy);
                    }
                }
            }
            catch (Exception ex) { }
        }

        #region IToolBoxService Members

        public void AddLinkedToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem, string category, IDesignerHost host)
        {
            // TODO:  Add ToolboxServiceImpl.AddLinkedToolboxItem implementation
        }

        void System.Drawing.Design.IToolboxService.AddLinkedToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem, IDesignerHost host)
        {
            // TODO:  Add ToolboxServiceImpl.System.Drawing.Design.IToolboxService.AddLinkedToolboxItem implementation
        }

        public System.Drawing.Design.ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host)
        {
            // we know that we serialized the item in a dataobject, so 
            // get it out 
            return (ToolboxItem)((DataObject)serializedObject).GetData(typeof(ToolboxItem));
        }

        System.Drawing.Design.ToolboxItem System.Drawing.Design.IToolboxService.DeserializeToolboxItem(object serializedObject)
        {
            return (ToolboxItem)((DataObject)serializedObject).GetData(typeof(ToolboxItem));
        }

        public object SerializeToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem)  //����toolboxItem��������
        {
            DataObject dataObj = null;
            if (toolboxItem != null)
            {
                // package the item in a dataobject
                dataObj = new DataObject(toolboxItem);
            }
            return dataObj;
        }

        public void RemoveToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem, string category)
        {
            // we handle adding and removing of toolbox items ourself
        }

        void System.Drawing.Design.IToolboxService.RemoveToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem)
        {
            // we handle adding and removing of toolbox items ourself
        }

        public bool SetCursor()
        {
            // not critical for sample app
            return false;
        }

        public void AddToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem, string category)
        {
            // we handle adding and removing of toolbox items ourself
        }

        void System.Drawing.Design.IToolboxService.AddToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem)
        {
            // we handle adding and removing of toolbox items ourself
        }

        public void AddCreator(System.Drawing.Design.ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
        {
            // we only deal with the standard win forms tools that we know of
        }

        void System.Drawing.Design.IToolboxService.AddCreator(System.Drawing.Design.ToolboxItemCreatorCallback creator, string format)
        {
            // we only deal with the standard win forms tools that we know of
        }

        public void RemoveCreator(string format, IDesignerHost host)
        {
            // we only deal with the standard win forms tools that we know of
        }

        void System.Drawing.Design.IToolboxService.RemoveCreator(string format)
        {
            // we only deal with the standard win forms tools that we know of
        }

        public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
        {
            return (this as IToolboxService).GetToolboxItems(category);
        }

        /// <summary>
        /// ����ָ�������ƥ��Ĺ������ȡ��������ļ���
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        ToolboxItemCollection System.Drawing.Design.IToolboxService.GetToolboxItems(string category)
        {
            IList items = new ArrayList();
            // return all toolbox items
            if (explorerBarTool != null && explorerBarTool.Groups != null && explorerBarTool.Groups.Count > 0)
            {
                // iterate over categories
                foreach (ExplorerBarGroup nd in explorerBarTool.Groups)
                {
                    if (string.Compare(nd.Text, category, true) == 0)
                    {
                        foreach (ExplorerBarItem itemNode in nd.Items)
                        {
                            items.Add(itemNode.Tag);
                        }
                    }
                }
            }
            ToolboxItem[] obj = new ToolboxItem[items.Count];
            items.CopyTo(obj, 0);
            return new ToolboxItemCollection(obj);
        }

        /// <summary>
        /// �ӹ������ȡ��ָ������������������Ĺ�������ļ��ϡ�
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        ToolboxItemCollection System.Drawing.Design.IToolboxService.GetToolboxItems(IDesignerHost host)
        {
            IList items = new ArrayList();

            // return all toolbox items
            if (explorerBarTool != null && explorerBarTool.Groups != null && explorerBarTool.Groups.Count > 0)
            {
                // iterate over categories
                foreach (ExplorerBarGroup nd in explorerBarTool.Groups)
                {
                    foreach (ExplorerBarItem itemNode in nd.Items)
                    {
                        items.Add(itemNode.Tag);
                    }
                }
            }
            ToolboxItem[] obj = new ToolboxItem[items.Count];
            items.CopyTo(obj, 0);
            return new ToolboxItemCollection(obj);
        }

        ToolboxItemCollection System.Drawing.Design.IToolboxService.GetToolboxItems()//�ӹ������ȡ����������������ϡ�
        {
            return (this as IToolboxService).GetToolboxItems((IDesignerHost)null);
        }

        public CategoryNameCollection CategoryNames
        {
            get
            {
                string[] names = null;
                if (explorerBarTool != null && explorerBarTool.Groups != null && explorerBarTool.Groups.Count > 0)
                {
                    names = new string[explorerBarTool.Groups.Count];
                    int categoryCount = 0;
                    foreach (ExplorerBarGroup nd in explorerBarTool.Groups)
                    {
                        names[categoryCount++] = nd.Text;
                    }
                }
                return new CategoryNameCollection(names);
            }
        }

        public System.Drawing.Design.ToolboxItem GetSelectedToolboxItem(IDesignerHost host)//�����ǰѡ���Ĺ����������������������ã�������֧��ָ��������������ȡ����
        {
            ToolboxItem item = null;
            return item;
        }

        System.Drawing.Design.ToolboxItem System.Drawing.Design.IToolboxService.GetSelectedToolboxItem()//��ȡ��ǰѡ���Ĺ������
        {
            return (this as IToolboxService).GetSelectedToolboxItem((IDesignerHost)null);
        }

        public void SetSelectedToolboxItem(System.Drawing.Design.ToolboxItem toolboxItem)
        {
            // we handle this manually
        }

        public bool IsSupported(object serializedObject, ICollection filterAttributes)
        {
            return true;
        }

        bool System.Drawing.Design.IToolboxService.IsSupported(object serializedObject, IDesignerHost host)
        {
            return true;
        }

        public void SelectedToolboxItemUsed()
        {
        }

        public bool IsToolboxItem(object serializedObject, IDesignerHost host)
        {
            // TODO:  Add ToolboxServiceImpl.IsToolboxItem implementation
            return false;
        }

        bool System.Drawing.Design.IToolboxService.IsToolboxItem(object serializedObject)
        {
            // TODO:  Add ToolboxServiceImpl.System.Drawing.Design.IToolBoxService.IsToolboxItem implementation
            return false;
        }

        public string SelectedCategory
        {
            get
            {
                string category = null;
                // get the selected node and find the category
                if (explorerBarTool != null && explorerBarTool.Groups != null &&
                    explorerBarTool.Groups.Count > 0 && explorerBarTool.SelectedItem != null &&
                   explorerBarTool.SelectedItem.Tag is ToolboxItem)
                {
                    category = explorerBarTool.SelectedItem.Group.Text;
                }
                return category;
            }
            set
            {
                // TODO:  Add ToolboxServiceImpl.SelectedCategory setter implementation
            }
        }

        #endregion


    }//class
}//namespace

