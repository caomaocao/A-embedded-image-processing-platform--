using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace CaVeGen.DesignViewFiles.FilterProperty
{
    public class EditControl : UITypeEditor
    {
        //Ϊ UITypeEditor �ṩһ���ӿڣ�������ʾ Windows ���壬���������ģʽ������������ؼ���������������ʾ�ؼ��� 
        IWindowsFormsEditorService editorService;
        ListBox listBox = null;

        /// <summary>
        /// ��ȡ�� EditValue ����ʹ�õı༭����ʽ
        /// </summary>
        /// <param name="context">�����ڻ�ȡ������������Ϣ�� ITypeDescriptorContext</param>
        /// <returns>UITypeEditorEditStyle ֵ֮һ��ָʾ���ṩ�ı༭��ʽ</returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        } 

        /// <summary>
        /// ʹ��GetEditStyle������ָʾ�ı༭����ʽ�༭ָ�������ֵ
        /// </summary>
        /// <param name="context">�����ڻ�ȡ������������Ϣ�� ITypeDescriptorContext</param>
        /// <param name="provider">IServiceProvider��ͨ�������ܻ�ñ༭����</param>
        /// <param name="value">���ڱ༭��ֵ��ʵ��</param>
        /// <returns>�µĶ���ֵ������ö����ֵ��δ���ģ�����Ӧ�����봫�ݸ����Ķ�����ͬ�Ķ���</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)       //�༭����Ķ���Ϊ��
            {
                editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }

            if (editorService != null)      //�༭�������Ͳ�Ϊ��
            {
                if (this.listBox == null)
                {
                    this.listBox = new ListBox();
                    this.listBox.Dock = DockStyle.Fill;
                    this.listBox.MouseClick += new MouseEventHandler(listBox_MouseClick);
                }
                this.listBox.Items.Clear();
                foreach (string str in CassViewGenerator.chooseString.Split(','))
                {
                    this.listBox.Items.Add(str);
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
