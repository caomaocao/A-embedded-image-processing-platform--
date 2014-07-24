/**********************************************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����
 * 
           ** �ļ�����HostDesign.cs 
 * 
           ** ����������
 * 
           **          ��Ҫ�����ṩ������Ľ�����Ʋ������̳�05ϵͳ���������齨���û�������DesignSurface
 *               ��̳�ϵͳ���������齨���û�������DesignSurface��
 * 
 *                      �������������Զ���ؼ�FilteredPropertyGrid,ͨ����������ӷ���͵õ�����ʹ������ҳ���ϵ���ʾ
 *               ���Կؼ�controlfilteredPropertyGrid����ʵʱ�󶨡����ж�CassView�ؼ��������Ŀؼ�����ʾ���Խ�������������á�
 *               CassView�ؼ�������������һ����.
 *                      
 *                      ����˶�ѡ�пؼ��������ʾ�����͵�ǰCassView�ϵ�����ʱ��λ����ʾ,��������ʾ�ؼ�һ��,
 *               ͨ����������ӷ���͵õ�����Ļ�����ʵ��(��CassViewGenerator�н��õ��Ŀؼ���ӵ���������ķ���������).
 * 
 *                      ��������У������UndoEngineImplication���͵ķ��񣬸÷����ܼ�¼��ǰ������Ĳ����������г������ظ��Ĳ�����
 *              �ڸ����У��ڵ�ǰ�ؼ�����LocationChanged ��SizeChangedʱ����������������µĿؼ����Լ��ı��������ʵ����Resizeʱ��������/�ظ�������
             *  �������ݴ����ٶ��������⣬�������Ƚ��ù�����Disabled����
 * 
 *                      ��������б�ѡ��Ŀؼ������仯ʱ��ʾ��ǰ�ؼ������ԣ�����Cassϵͳ����Ҫ�����Խ��й��˲�������ʾ��������ԡ�
 * 
 *                      ����˿ؼ�����Ϲ��ܣ���ÿһ�����Ͽؼ���ΪPublicVariabl.GroupStruct�ṹ������groupList�����С�
 * 
 * 
           ** ���ߣ��ⵤ��
           ** ��ʼʱ�䣺2006-11-8
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
    /// ������Ҫʵ��������е�һЩ��ʼ�������͵�ǰ�ؼ��������ı�ʱ���¼����񣬼̳���DesignSurface�ࡣ
    /// </summary>
    public class HostDesign : DesignSurface
    {
        private object senderSubject = null;                       //��ǰ��������ʾ�Ķ���
        private IDesignerHost host;                                 //��ǰ������Ľӿ�
        public ISelectionService selectionService = null;          //ѡ��ؼ��ķ���
        private ICollection selectedComponents = null;              //��ǰѡ�пؼ��Ķ���
        private object[] selections = null;                         //��ǰѡ�еĶ���
        private FilteredPropertyGrid cassPropertyGrid = null;       //���Կؼ�
        private ContextMenuStrip operateMenu = null;                //��ǰ���Ҽ�����ʱ��ʾ��˵�
        private ToolStripStatusLabel designMousePosition = null;    //��ʾ���λ�õĹ�����
        private UndoEngineImplication undoEngine = null;            //����/�ظ�����
        private ToolStripMenuItem editToolMenuItem = null;          //�༭�˵�

        //private const int undoIndex = 0;       //�༭�˵��г���������Ϊ0
        private const int redoIndex = 1;         //�༭�˵����ظ�������Ϊ1
        private const int cutIndex = 2;          //�༭�˵��м��е�����Ϊ2
        private const int copyIndex = 3;         //�༭�˵��и��Ƶ�����Ϊ3
        //private const int pastIndex = 4;       //�༭�˵���ճ��������Ϊ4
        private const int deleteIndex = 5;       //�༭�˵���ɾ��������Ϊ5
        //private const int selectAllIndex = 6;  //�༭�˵���ȫѡ������Ϊ6
        private const int viewIndex = 7;         //�༭�˵�����ͼ������Ϊ7

        private bool lostFouce = false;          //��־��ǰ������Ƿ�ʧȥ����,false:δ���� 
        private object currentObject = null;
        private ArrayList addedObject = new ArrayList();  //��ӵĿؼ���ʱ���ڶ����в���ʾ
        private bool selectedChangedFlag = false;       //ѡ�ж�����б�־

        /// <summary>
        /// ��Ӳ˵�����
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
        /// ��ʼ������HostDesign��Ķ��󣬼��ص�ǰ��ѡ�ؼ��仯ʱ���¼��������Ϣ����
        /// �õ��ķ������Կؼ��������Ĳ˵���״̬������ǩ�ؼ���TabControl�����˵�����
        /// ��ӵķ��񣺳���/�ظ�
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
                
                //��ӳ���/�ظ���������
                IServiceContainer serviceContainer = host.GetService(typeof(ServiceContainer)) as IServiceContainer;
                undoEngine = new UndoEngineImplication(serviceContainer);
                undoEngine.Enabled = false;    //�رճ������ظ�����
                host.AddService(typeof(UndoEngineImplication), undoEngine);
            }
            catch (Exception ex) { }
        }

        #region  ����������������

        /// <summary>
        /// ��������ĸ�����Ļ�ʵ��(��CassView��ʵ��)�Ĵ�С�����仯ʱ,����ˢ��������ʾ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CassView_Resize(object sender, EventArgs e)
        {
            designMousePosition.Text = "�� " + ((Control)host.RootComponent).Bounds.Width
                + "," + "�� " + ((Control)host.RootComponent).Bounds.Height;            //ʵʱ��ʾ���λ�� 
            cassPropertyGrid.Refresh();
        }

        /// <summary>
        /// �ؼ�λ�øı��¼�������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void senderSubject_LocationChanged(object sender, EventArgs e)
        {
            //((Control)sender).BringToFront();
            cassPropertyGrid.Refresh();
        }

        /// <summary>
        /// �ؼ���С�ı��¼�������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        private void senderSubject_SizeChanged(object sender, EventArgs e)
        {
            cassPropertyGrid.Refresh();
        }
        
        #endregion

        /// <summary>
        /// ���������������͵Ķ�����в����ķ���
        /// </summary>
        /// <param name="type">����</param>
        /// <param name="serviceInstance">�������</param>
        public void AddService(Type type, object serviceInstance)
        {
            this.ServiceContainer.AddService(type, serviceInstance);
        }

        /// <summary>
        /// ��ȡ�Ƿ�ʧȥ����
        /// false:û��ʧȥ����
        /// true:ʧȥ����
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

        #region ��ǰѡ�еĿؼ������仯ʱ

        /// <summary>
        /// �ؼ������仯ʱ���õ��ÿؼ������ԣ�����SelectionControl()������
        /// ����ѡ�еĿؼ����ж����Ƿ������״̬������ǣ���ͬʱ������е������ؼ�����ѡ��״̬
        /// ����ͨ���ؼ���ϵĿؼ����������н���ѡ��ʱ��һ����ʾ
        /// </summary>
        private void selectionService_SelectionChanged(object sender, EventArgs e)
        {
            if (selectedChangedFlag == true)
            {
                return;
            }//end if (selectedChangedFlag == true)

            List<object> selectList = new List<object>();  //��ǰѡ�еĿؼ�
            if (senderSubject != null && !senderSubject.GetType().ToString().Equals(PublicVariable.viewName))
            {
                //��ȥ��̬ί��,��ʵʱ��ʾ�ؼ��ı仯�˵�����
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
                        if (!selectList.Contains(selections[i]))   //�����������
                        {
                            selectList.Add(selections[i]);
                        }//end if (!selectList.Contains(selections[i]))
                    }//end for
                }//end if (selectedComponents.Count > 0)
                selectionService.SetSelectedComponents((ICollection)selectList);  //��ѡ��״̬
            }//end if (selectionService != null)
            SelectionControl();
        }//end function

        /// <summary>
        /// ����ѡ�Ŀؼ�����������ҳ������Կؼ�����ʾ����
        /// ��FilteredPropertyGrid���Կؼ���������,�г���Ҫ���صķ�����Ϣ�Ͳ�����Ҫ��ʾ�ľ���������Ϣ��
        /// ���ѡ�еĿؼ��ĸ�����Ϊ0������С����ơ�ɾ���ɲ�����
        /// ���ѡ�еĿؼ��ĸ�������1���������ʾ������Ӻ���ͼ������
        /// ���ѡ�еĿؼ�Ϊȫ�����򼴿���������ֿ��������
        /// </summary>
        public void SelectionControl()
        {
            if (selectionService != null)
            {
                selectedComponents = selectionService.GetSelectedComponents();

                lostFouce = false;  //��ǰ��������н���
                if (selectedComponents.Count > 0)
                {
                    selections = new Object[selectedComponents.Count];
                    selectedComponents.CopyTo(selections, 0);
                    senderSubject = selections[0]; //���ݶ���; �������Կ���ʾ���ǵ�һ��ѡ��ÿؼ�
                    CassViewGenerator.designSelectedObjects = selections;
                    SetPropertyGrid(senderSubject);
                    cassPropertyGrid.SelectedObjects = selections;
                    cassPropertyGrid.Refresh();
                    editToolMenuItem.Enabled = true;
                    if (selectedComponents.Count > 1)
                    {
                        editToolMenuItem.DropDownItems[viewIndex].Enabled = true;  //��ͼ�ɲ���
                    }
                }//if(selectedComponents.Count > 0)

                //��HostDesign�е����Կؼ���ͨ�������AddService��GetService����
                if (senderSubject != null)
                {
                    if (senderSubject.GetType().ToString().Equals(PublicVariable.viewName)) // cassView
                    {
                        //operateMenu.Items[bringAndSend].Enabled = false;
                        //���С����ơ�ɾ�����ɲ���
                        editToolMenuItem.DropDownItems[cutIndex].Enabled = false;
                        editToolMenuItem.DropDownItems[copyIndex].Enabled = false;
                        editToolMenuItem.DropDownItems[deleteIndex].Enabled = false;
                        editToolMenuItem.DropDownItems[viewIndex].Enabled = false;  //��ͼ���ɲ���
                    }
                    else
                    {
                        //��ӿؼ�λ�ú͸ı��С��ί��
                        ((Control)senderSubject).LocationChanged += new EventHandler(senderSubject_LocationChanged);
                        ((Control)senderSubject).SizeChanged += new EventHandler(senderSubject_SizeChanged);

                        //���С����ơ�ɾ���ɲ���
                        editToolMenuItem.DropDownItems[cutIndex].Enabled = true;
                        editToolMenuItem.DropDownItems[copyIndex].Enabled = true;
                        editToolMenuItem.DropDownItems[deleteIndex].Enabled = true;
                    }//��ΪCassView
                }
                else��//����Ϊ�գ���������Կؼ��е�����
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
        /// ��ȡ�����õ�ǰѡ�еĿؼ��Ķ���
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
        /// ���ظ������ 
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
        /// ��ǰ�����Ķ���
        /// </summary>
        public object CurrentAddedObject
        {
            get
            {
                return currentObject;
            }
        }
        #endregion

        # region �ؼ����Թ���
        /// <summary>
        /// ��������,���ΪCassView����,����˵�"���"��һ����
        /// </summary>
        /// <param name="control">��ǰ��Ҫ����������ʾ�Ķ���</param>
        private void SetPropertyGrid(object control)
        {
            if (control != null && cassPropertyGrid != null)
            {
                cassPropertyGrid.BrowsableAttributes =
                        new AttributeCollection(new Attribute[] { new CategoryAttribute("��������"),
                        new CategoryAttribute("��������")});

                //��������ǿؼ�����ʾ�����ؼ��ġ�Locked������
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


