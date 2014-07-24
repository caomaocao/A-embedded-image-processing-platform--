using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace ControlTactic.SpecialControl
{
    public partial class Calculator : Arithmetic
    {
        public Calculator()
        {
            InitializeComponent();
        }

        List<List<string>> configuration = new List<List<string>>();

        [Category("功能属性")]
        [DisplayName("计算器组态")]
        [EditorAttribute(typeof(CalculatorCassType), typeof(System.Drawing.Design.UITypeEditor))]
        public List<List<string>> Configuration
        {
            get
            {
                return configuration;
            }
            set
            {
                configuration = value;
            }
        }

        [Category("基本属性")]
        [Browsable(false)]
        public List<List<string>> SaveConfigurationDate
        {
            get
            {
                return this.configuration;
            }
            set
            {
                this.configuration = value;
            }
        }
    }



    internal class CalculatorCassType : UITypeEditor
    {
        //为 UITypeEditor 提供一个接口，用于显示 Windows 窗体，或者在设计模式下在属性网格控件的下拉区域中显示控件。 
        IWindowsFormsEditorService editorService;       

        /// <summary>
        /// 获取由 EditValue 方法使用的编辑器样式
        /// </summary>
        /// <param name="context">可用于获取附加上下文信息的 ITypeDescriptorContext</param>
        /// <returns>UITypeEditorEditStyle 值之一，指示所提供的编辑样式</returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// 使用GetEditStyle方法所指示的编辑器样式编辑指定对象的值
        /// </summary>
        /// <param name="context">可用于获取附加上下文信息的 ITypeDescriptorContext</param>
        /// <param name="provider">IServiceProvider，通过它可能获得编辑服务</param>
        /// <param name="value">正在编辑的值的实例</param>
        /// <returns>新的对象值，如果该对象的值尚未更改，则这应返回与传递给它的对象相同的对象</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //if (provider != null)       //编辑服务的对象不为空
            //{
            //    //显示一个省略号 (...) 按钮，该按钮可启动模式对话框，对于这种对话框，用户必须输入数据才能继续程序；
            //    //该按钮也可以启动非模式对话框，这种对话框停留在屏幕上，可供用户随时使用，但它允许用户执行其他活动。 
            editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            //}

            CalculatorFrom1 Newfrm = new CalculatorFrom1((List<List<string>>)value);
            editorService.ShowDialog(Newfrm);
            return Newfrm.newList;
        }
    }
}
