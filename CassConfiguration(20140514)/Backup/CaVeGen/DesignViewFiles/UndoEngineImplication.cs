/**********************************************************************************************
           ** Copyright (C) 2006 CASS 版权所有
 * 
           ** 文件名：UndoEngineImplication.cs 
 * 
           ** 功能描述：
 * 
           **          该类继承了UndoEngine类,完成了撤消和重复的操作,由于在 MenuCommandService中
 *          并不能通过StandardCommands.Undo来实现,而是需要通过一个队列来存放当前发生过的操作.
 * 
 *                      可通过UndoEngine.Enable来实现在设计器中开启还是禁用该功能.由于在设计器加
 *           载晚生后全部处于开启状态时,在控件的添加过程比之前没有该功能时增加三倍左右,而且不能通过鼠标的拖
 *          动来改变控件Resize,故:在设计器类HoseDesign中,通过实时的开启该类(UndoEngine.Enable = true),和添
 *          加完操作记录后,禁用该类来实现Undo 和 Redo的功能.
 * 
 *                      在该类添加完操作记录后,通过获得设计器的typeof(FilteredPropertyGrid)和typeof(ToolStripMenuItem)
 *          服务来动态的改变主页面上的编辑菜单和属性控件的状态.
 * 
 * 
 * 
           ** 作者：吴丹红
           ** 创始时间：2007-4-17
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
    class UndoEngineImplication : UndoEngine  //指定一般的撤消/重复功能。
    {
        List<List<UndoEngine.UndoUnit>> undoUnitList = new List<List<UndoUnit>> ();
        List<UndoEngine.UndoUnit> tempUnList = new List<UndoUnit>();

        // 表示当前执行的撤销和重复操作的链表的索引
        private int currentPos = 0;

        private IServiceContainer service = null;
        private ToolStripMenuItem editToolStripMenuItem = null;
        private FilteredPropertyGrid cassPropertyGrid = null;

        private const int undoIndex = 0;   //撤消菜单在编辑菜单中的索引值
        private const int redoIndex = 1;    //重复菜单在编辑菜单中的索引值

        public UndoEngineImplication(IServiceContainer provider)
            : base(provider)
        {
            service = provider;
            editToolStripMenuItem = (ToolStripMenuItem)service.GetService(typeof(ToolStripMenuItem));
            cassPropertyGrid = (FilteredPropertyGrid)service.GetService(typeof(FilteredPropertyGrid));
        }

        /// <summary>
        /// 撤销动作
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
        /// 重复操作
        /// </summary>
        public void DoRedo()
        {
            if (currentPos < undoUnitList.Count)
            {
                foreach (UndoEngine.UndoUnit undoUnit in undoUnitList[currentPos])
                {
                    undoUnit.Undo();  //执行撤消或重复操作。
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
        /// 将 UndoEngine.UndoUnit 添加到撤消堆栈中。
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
        /// 创建一个新的 UndoEngine.UndoUnit。 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="primary"></param>
        /// <returns></returns>
        protected override UndoEngine.UndoUnit CreateUndoUnit(string name, bool primary)
        {
            return base.CreateUndoUnit(name, primary);
        }

        /// <summary>
        /// 放弃 UndoEngine.UndoUnit。
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
        /// 引发 Undoing 事件。 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUndoing(EventArgs e)
        {
            base.OnUndoing(e);
        }

        /// <summary>
        /// 引发 OnUndone 事件。 
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
