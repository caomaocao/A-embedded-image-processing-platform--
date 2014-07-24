/**********************************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����
 * 
           ** �ļ�����UndoEngineImplication.cs 
 * 
           ** ����������
 * 
           **          ����̳���UndoEngine��,����˳������ظ��Ĳ���,������ MenuCommandService��
 *          ������ͨ��StandardCommands.Undo��ʵ��,������Ҫͨ��һ����������ŵ�ǰ�������Ĳ���.
 * 
 *                      ��ͨ��UndoEngine.Enable��ʵ����������п������ǽ��øù���.�������������
 *           ��������ȫ�����ڿ���״̬ʱ,�ڿؼ�����ӹ��̱�֮ǰû�иù���ʱ������������,���Ҳ���ͨ��������
 *          �����ı�ؼ�Resize,��:���������HoseDesign��,ͨ��ʵʱ�Ŀ�������(UndoEngine.Enable = true),����
 *          ���������¼��,���ø�����ʵ��Undo �� Redo�Ĺ���.
 * 
 *                      �ڸ�������������¼��,ͨ������������typeof(FilteredPropertyGrid)��typeof(ToolStripMenuItem)
 *          ��������̬�ĸı���ҳ���ϵı༭�˵������Կؼ���״̬.
 * 
 * 
 * 
           ** ���ߣ��ⵤ��
           ** ��ʼʱ�䣺2007-4-17
           ** 
************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using CaVeGen.DesignViewFiles.FilterProperty;
using System.Windows.Forms;

namespace CaVeGen.DesignViewFiles
{
    class UndoEngineImplication : UndoEngine  //ָ��һ��ĳ���/�ظ����ܡ�
    {
        List<List<UndoEngine.UndoUnit>> undoUnitList = new List<List<UndoUnit>> ();
        List<UndoEngine.UndoUnit> tempUnList = new List<UndoUnit>();

        // ��ʾ��ǰִ�еĳ������ظ����������������
        private int currentPos = 0;

        private IServiceContainer service = null;
        private ToolStripMenuItem editToolStripMenuItem = null;
        private FilteredPropertyGrid cassPropertyGrid = null;

        private const int undoIndex = 0;   //�����˵��ڱ༭�˵��е�����ֵ
        private const int redoIndex = 1;    //�ظ��˵��ڱ༭�˵��е�����ֵ

        public UndoEngineImplication(IServiceContainer provider)
            : base(provider)
        {
            service = provider;
            editToolStripMenuItem = (ToolStripMenuItem)service.GetService(typeof(ToolStripMenuItem));
            cassPropertyGrid = (FilteredPropertyGrid)service.GetService(typeof(FilteredPropertyGrid));
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void DoUndo()
        {
            if (currentPos > 0)
            {
                foreach (UndoEngine.UndoUnit undoUnit in undoUnitList[currentPos - 1])
                {
                    undoUnit.Undo();
                }
                currentPos--;
            }
            UpdateUndoRedoMenuCommandsStatus();
        }

        /// <summary>
        /// �ظ�����
        /// </summary>
        public void DoRedo()
        {
            if (currentPos < undoUnitList.Count)
            {
                foreach (UndoEngine.UndoUnit undoUnit in undoUnitList[currentPos])
                {
                    undoUnit.Undo();  //ִ�г������ظ�������
                }
                currentPos++;
            }
            UpdateUndoRedoMenuCommandsStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateUndoRedoMenuCommandsStatus()
        {
            IMenuCommandService menuCommandService = service.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            if (menuCommandService != null)
            {
                MenuCommand undoMenuCommand = menuCommandService.FindCommand(StandardCommands.Undo);
                MenuCommand redoMenuCommand = menuCommandService.FindCommand(StandardCommands.Redo);

                if (undoMenuCommand != null)
                {
                    undoMenuCommand.Enabled = currentPos > 0;

                }
                if (redoMenuCommand != null)
                {
                    redoMenuCommand.Enabled = currentPos < this.undoUnitList.Count;

                }
                editToolStripMenuItem.DropDownItems[redoIndex].Enabled = currentPos < this.undoUnitList.Count;
                editToolStripMenuItem.DropDownItems[undoIndex].Enabled = currentPos > 0;
                if (cassPropertyGrid != null)
                {
                    cassPropertyGrid.Refresh();
                }
            }

        }

        /// <summary>
        /// �� UndoEngine.UndoUnit ��ӵ�������ջ�С�
        /// </summary>
        /// <param name="unit"></param>
        protected override void AddUndoUnit(UndoEngine.UndoUnit unit)
        {            
            tempUnList.Add(unit);

            if (editToolStripMenuItem.DropDownItems.Count > 0)
            {
                editToolStripMenuItem.DropDownItems[undoIndex].Enabled = true;
            }


            if (cassPropertyGrid != null)
            {
                cassPropertyGrid.Refresh();
            }
            //this.Enabled = false;
        }

        /// <summary>
        /// ����һ���µ� UndoEngine.UndoUnit�� 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="primary"></param>
        /// <returns></returns>
        protected override UndoEngine.UndoUnit CreateUndoUnit(string name, bool primary)
        {
            return base.CreateUndoUnit(name, primary);
        }

        /// <summary>
        /// ���� UndoEngine.UndoUnit��
        /// </summary>
        /// <param name="unit"></param>
        protected override void DiscardUndoUnit(UndoEngine.UndoUnit unit)
        {
            //undoUnitList.Remove(unit);
            base.DiscardUndoUnit(unit);


            if (cassPropertyGrid != null)
            {
                cassPropertyGrid.Refresh();
            }
            //this.Enabled = false;
        }

        /// <summary>
        /// ���� Undoing �¼��� 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUndoing(EventArgs e)
        {
            base.OnUndoing(e);
        }

        /// <summary>
        /// ���� OnUndone �¼��� 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUndone(EventArgs e)
        {
            base.OnUndone(e);
        }

        public bool CanUndo
        {
            get
            {
                if (currentPos > 0 && currentPos <= this.undoUnitList.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public bool CanReDo
        {
            get
            {
                if (currentPos >= 0 && currentPos < this.undoUnitList.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public void EndAct()
        {
            undoUnitList.Add(tempUnList);
            currentPos = undoUnitList.Count;
            this.Enabled = false;
        }

        public void StartAct()
        {
            undoUnitList.RemoveRange(currentPos, undoUnitList.Count - currentPos);
            tempUnList = new List<UndoUnit>();
            this.Enabled = true;
        }
    
    }
}
