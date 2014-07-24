        using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.IO;

namespace ControlTactic.GraphicsControl
{
    public  class featherFileEditor: UITypeEditor
    {




        //  private List<string> filesList = new List<string>();
        // private string filePath = null;
        public static string projectPath = null;



        //为 UITypeEditor 提供一个接口，用于显示 Windows 窗体，或者在设计模式下在属性网格控件的下拉区域中显示控件。 
        // private IWindowsFormsEditorService editorService;
        /// <summary>
        /// 获取由 EditValue 方法使用的编辑器样式
        /// </summary>
        /// <param name="context">可用于获取附加上下文信息的 ITypeDescriptorContext</param>
        /// <returns>UITypeEditorEditStyle 值之一，指示所提供的编辑样式</returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
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
            IWindowsFormsEditorService editorService = null;
            if (context != null && context.Instance != null && provider != null)       //编辑服务的对象不为空
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            }

            if (editorService != null)      //编辑服务类型不为空
            {

                string filePath = this.AddPictureFileDialog();

                string fileName = Path.GetFileName(filePath);
                value = "\"" + fileName + "\"";
                return value;
            }
            return value;

        }


        /// <summary>
        /// 打开“添加图片”对话框
        /// 支持打开多个文件
        /// </summary>
        /// <returns name="filePath">string</returns>
        public string AddPictureFileDialog()
        {

            try
            {
                //   string[] filesPath;
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.DefaultExt = "txt";
                openDialog.Filter = "Image File(*.txt)|*.txt";
                openDialog.Title = "添加文件";
                openDialog.Multiselect = true;   //允许多个文件选中
                //默认指定工程下的路径

                //if ( FileInputEditor.projectPath== null&&FileInput.currentProjectPath==null)
                //{
                //    MessageBox.Show("工程路径赋值为空！");
                //    return null;
                //}
                //if (FileInputEditor.projectPath != null)
                //    openDialog.InitialDirectory = FileInputEditor.projectPath;
                //else if (FileInput.currentProjectPath != null)
                //    openDialog.InitialDirectory = FileInput.currentProjectPath;

                if (featherFileEditor.projectPath == null)
                {
                    MessageBox.Show("工程路径赋值为空！");
                    return null;
                }
                openDialog.InitialDirectory = featherFileEditor.projectPath;
             

                DialogResult result = openDialog.ShowDialog();

                if (result == DialogResult.OK)
                {

                    return openDialog.FileName;
                    // return fileNames;
                }

            }
            catch
            {

            }
            return null;

        }

    }
}

