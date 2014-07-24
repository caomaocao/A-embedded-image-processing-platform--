using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Xml;

namespace ControlTactic
{
    public class ChooseSerialNumber : UITypeEditor
    {
        public static List<string> SerialNumberName = new List<string>();

        //为 UITypeEditor 提供一个接口，用于显示 Windows 窗体，或者在设计模式下在属性网格控件的下拉区域中显示控件。 
        IWindowsFormsEditorService editorService;
        ListBox listBox = null;

        /// <summary>
        /// 获取由 EditValue 方法使用的编辑器样式
        /// </summary>
        /// <param name="context">可用于获取附加上下文信息的 ITypeDescriptorContext</param>
        /// <returns>UITypeEditorEditStyle 值之一，指示所提供的编辑样式</returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
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
            if (provider != null)       //编辑服务的对象不为空
            {
                editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }

            if (editorService != null)      //编辑服务类型不为空
            {
                if (this.listBox == null)
                {
                    this.listBox = new ListBox();
                    this.listBox.Dock = DockStyle.Fill;
                    this.listBox.MouseClick += new MouseEventHandler(listBox_MouseClick);
                }
                this.listBox.Items.Clear();

                int count = 0;
                foreach (string str in SerialNumberName)
                {
                    if (str != null && str.Trim() != "")
                    {
                        //this.listBox.Items.Add("BL:" + count.ToString());
                        this.listBox.Items.Add(str);
                    }
                    count++;
                }

                this.listBox.SelectedItem = value;
                editorService.DropDownControl(this.listBox);
            }
            if (this.listBox.SelectedItem != null)
            {
                return this.listBox.SelectedItem.ToString();
            }
            else
            {
                return null;
            }
        }

        void listBox_MouseClick(object sender, MouseEventArgs e)
        {
            editorService.CloseDropDown();
        }
    }
}
