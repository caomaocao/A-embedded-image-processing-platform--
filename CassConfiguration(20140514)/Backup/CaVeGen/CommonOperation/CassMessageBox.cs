/******************************************************************************************************************************************
           ** Copyright (C) 2006 CASS 版权所有

           ** 文件名：CassMessageBox.cs 
           ** 功能描述：
 *                      该类主要实现了对弹出的提示信息框进行封装，使系统的提示信息拥有一个统一的界面。
* 
           ** 作者：吴丹红
 *         ** 创始时间：2007-4-10


******************************************************************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CaVeGen.CommonOperation
{
    public class CassMessageBox
    {
        private static DialogResult messageBoxResult;

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="textForMessageBox">要显示的提示信息</param>
        /// <returns></returns>
        public static DialogResult Information(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "操作提示",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return messageBoxResult;
        }

        /// <summary>
        /// 询问提示,Yes,No
        /// </summary>
        /// <param name="textForMessageBox">要显示的提示信息</param>
        /// <returns></returns>
        public static DialogResult Question(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "确认操作提醒",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return messageBoxResult;
        }

        /// <summary>
        /// 询问提示,Yes,No,Cancel
        /// </summary>
        /// <param name="textForMessageBox">要显示的提示信息</param>
        /// <returns></returns>
        public static DialogResult QuestionT(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "确认操作提醒",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            return messageBoxResult;
        }


       /// <summary>
        /// 警告
       /// </summary>
        /// <param name="textForMessageBox">要显示的提示信息</param>
       /// <returns></returns>
        public static DialogResult Warning(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "错误提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return messageBoxResult;
        }



        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="textForMessageBox">要显示的提示信息</param>
        /// <returns></returns>
        public static DialogResult Error(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "操作异常",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return messageBoxResult;
        }
    }
}
