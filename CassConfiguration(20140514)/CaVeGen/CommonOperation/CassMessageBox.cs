/******************************************************************************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����

           ** �ļ�����CassMessageBox.cs 
           ** ����������
 *                      ������Ҫʵ���˶Ե�������ʾ��Ϣ����з�װ��ʹϵͳ����ʾ��Ϣӵ��һ��ͳһ�Ľ��档
* 
           ** ���ߣ��ⵤ��
 *         ** ��ʼʱ�䣺2007-4-10


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
        /// ��Ϣ
        /// </summary>
        /// <param name="textForMessageBox">Ҫ��ʾ����ʾ��Ϣ</param>
        /// <returns></returns>
        public static DialogResult Information(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "������ʾ",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return messageBoxResult;
        }

        /// <summary>
        /// ѯ����ʾ,Yes,No
        /// </summary>
        /// <param name="textForMessageBox">Ҫ��ʾ����ʾ��Ϣ</param>
        /// <returns></returns>
        public static DialogResult Question(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "ȷ�ϲ�������",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return messageBoxResult;
        }

        /// <summary>
        /// ѯ����ʾ,Yes,No,Cancel
        /// </summary>
        /// <param name="textForMessageBox">Ҫ��ʾ����ʾ��Ϣ</param>
        /// <returns></returns>
        public static DialogResult QuestionT(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "ȷ�ϲ�������",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            return messageBoxResult;
        }


       /// <summary>
        /// ����
       /// </summary>
        /// <param name="textForMessageBox">Ҫ��ʾ����ʾ��Ϣ</param>
       /// <returns></returns>
        public static DialogResult Warning(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "������ʾ",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return messageBoxResult;
        }



        /// <summary>
        /// ����
        /// </summary>
        /// <param name="textForMessageBox">Ҫ��ʾ����ʾ��Ϣ</param>
        /// <returns></returns>
        public static DialogResult Error(string textForMessageBox)
        {
            messageBoxResult = MessageBox.Show(textForMessageBox, "�����쳣",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return messageBoxResult;
        }
    }
}
