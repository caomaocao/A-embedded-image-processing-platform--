/************************************************************************************
           ** Copyright (C) 2006 CASS 版权所有
           ** 文件名：HostControl.cs 
           ** 功能描述：
           **          为自定义控件，该控件主要实现了将设计器（由HostDesign类实现）邦定到当前控件中，
           *           该自定义控件作为HostDesign类的根组件。

           ** 作者：吴丹红
           ** 创始时间：2006-11-11
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
    /// 装载HostDesign类（HostDesign类继承了DesignSurface类）。
    /// </summary>
    public partial class HostControl : UserControl
    {
        private bool loadFlag = false;　　//标注当前设计器是否加载成功

        /// <summary>
        ///定义设计器变量，即HostDesign类类型变量。
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
        /// /获取当前设计器的视图，并将当前的类控件设置为设计器视图的根组件。 
        /// </summary>
        /// <param name="hostDesignSurface">HostDesign类型变量，该变量为设计器类型变量，函数中获取该设计器的视图</param>
        private void InitializeHost(HostDesign hostDesignSurface)
        {
            try
            {
                hostDesign = hostDesignSurface;
                if (hostDesign == null || hostDesign.View == null)
                {
                    return;
                }

                Control control = (Control)hostDesign.View;  //获取根设计器的视图，该View并不是类；
                control.Parent = this;
                control.Dock = DockStyle.Fill;
                control.Visible = true;
                loadFlag = true;
            }
            catch(NotSupportedException e)
            {
                loadFlag = false;
                CassMessageBox.Error("未提供与此设计图面兼容的视图");
            }
            catch(ObjectDisposedException e)
            {
                loadFlag = false;
                CassMessageBox.Error("设计器对象被释放！");
            }
            catch(InvalidOperationException e)
            {
                loadFlag = false;
                CassMessageBox.Error("设计器加载程序尚未创建根设计器或加载失败！");
            }
            catch (Exception ex)
            {
                CassMessageBox.Error("加载程序发生致命性错误！");
                this.Dispose();//20090603便于加载至梯形图内部，关闭当前程序窗口
                //Application.Exit();
            }
        }

        /// <summary>
        /// 获得当前HostControl的设计器
        /// </summary>
        public HostDesign HostDesign
        {
            get
            {
                return hostDesign;
            }
        }

        /// <summary>
        /// 视图加载是否成功
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
