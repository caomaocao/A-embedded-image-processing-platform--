/************************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����
           ** �ļ�����HostControl.cs 
           ** ����������
           **          Ϊ�Զ���ؼ����ÿؼ���Ҫʵ���˽����������HostDesign��ʵ�֣������ǰ�ؼ��У�
           *           ���Զ���ؼ���ΪHostDesign��ĸ������

           ** ���ߣ��ⵤ��
           ** ��ʼʱ�䣺2006-11-11
           ** 
*************************************************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using CaVeGen.CommonOperation;

namespace CaVeGen.DesignViewFiles
{
    /// <summary>
    /// װ��HostDesign�ࣨHostDesign��̳���DesignSurface�ࣩ��
    /// </summary>
    public partial class HostControl : UserControl
    {
        private bool loadFlag = false;����//��ע��ǰ������Ƿ���سɹ�

        /// <summary>
        ///�����������������HostDesign�����ͱ�����
        /// </summary>
        private HostDesign hostDesign;

        public HostControl(HostDesign hostDesignSurface)
        {
            InitializeComponent();
            InitializeHost(hostDesignSurface);
        }

        void HostControl_Scroll(object sender, ScrollEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

         //<summary>
        /// /��ȡ��ǰ���������ͼ��������ǰ����ؼ�����Ϊ�������ͼ�ĸ������ 
        /// </summary>
        /// <param name="hostDesignSurface">HostDesign���ͱ������ñ���Ϊ��������ͱ����������л�ȡ�����������ͼ</param>
        private void InitializeHost(HostDesign hostDesignSurface)
        {
            try
            {
                hostDesign = hostDesignSurface;
                if (hostDesign == null || hostDesign.View == null)
                {
                    return;
                }

                Control control = (Control)hostDesign.View;  //��ȡ�����������ͼ����View�������ࣻ
                control.Parent = this;
                control.Dock = DockStyle.Fill;
                control.Visible = true;
                loadFlag = true;
            }
            catch(NotSupportedException e)
            {
                loadFlag = false;
                CassMessageBox.Error("δ�ṩ������ͼ����ݵ���ͼ");
            }
            catch(ObjectDisposedException e)
            {
                loadFlag = false;
                CassMessageBox.Error("����������ͷţ�");
            }
            catch(InvalidOperationException e)
            {
                loadFlag = false;
                CassMessageBox.Error("��������س�����δ����������������ʧ�ܣ�");
            }
            catch (Exception ex)
            {
                CassMessageBox.Error("���س����������Դ���");
                this.Dispose();//20090603���ڼ���������ͼ�ڲ����رյ�ǰ���򴰿�
                //Application.Exit();
            }
        }

        /// <summary>
        /// ��õ�ǰHostControl�������
        /// </summary>
        public HostDesign HostDesign
        {
            get
            {
                return hostDesign;
            }
        }

        /// <summary>
        /// ��ͼ�����Ƿ�ɹ�
        /// </summary>
        public bool LoadBool
        {
            get
            {
                return loadFlag;
            }
        }

    }
}
