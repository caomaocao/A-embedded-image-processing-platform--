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
    //解耦控制块存放数据的结构体
    public struct JieOuStruct
    {
        public int JieOuNum;//解耦路数
        public List<List<string>> JieOuAttribute;//各路解耦属性        
        public string[,] JieOuTable;//解耦矩阵
    }

    public partial class JieOu : CanAddIO
    {
        public JieOu()
        {
            InitializeComponent();
            //根据控件本身初始化端口数目
            this.InputNum = 2;
            this.OutputNum = 2;
        }

        //以数组的方式初始化数据列表
        private List<string> configuration
            = new List<string>(new string[] { "2", "1,100000,0,50,自动,100,0,0;1,100000,0,50,自动,100,0,0", "0,0;0,0" });
        [Category("功能属性")]
        [DisplayName("解耦控制组态")]
        [EditorAttribute(typeof(JieOuCassType), typeof(System.Drawing.Design.UITypeEditor))]
        public List<string> Configuration
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
        public List<string> SaveConfigurationDate
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

        /// <summary>
        /// 将解耦控制块的结构体转换成列表
        /// </summary>
        /// <param name="tempStuct">传入的结构体</param>
        /// <returns>返回转换后的列表</returns>
        public List<string> StructToList(JieOuStruct tempStuct)
        {
            List<string> tempList = new List<string>();
            //解耦路数
            tempList.Add(tempStuct.JieOuNum.ToString());
            //解耦属性
            List<string> tempAttribute = new List<string>();
            foreach (List<string> element in tempStuct.JieOuAttribute)
            { tempAttribute.Add(String.Join(",", element.ToArray())); }
            tempList.Add(String.Join(";",tempAttribute.ToArray()));
            //解耦矩阵
            List<string> Jtable = new List<string>();
            for (int i = 0; i < tempStuct.JieOuNum; i++)
            {
                List<string> row = new List<string>();
                for (int j = 0; j < tempStuct.JieOuNum; j++)
                {
                    row.Add(tempStuct.JieOuTable[i, j]);
                }
                Jtable.Add(String.Join(",", row.ToArray()));
            }
            tempList.Add(String.Join(";", Jtable.ToArray()));

            return tempList;
        }

        /// <summary>
        /// 将解耦控制块的数据列表转换成结构体
        /// </summary>
        /// <param name="tempList">传入的列表</param>
        /// <returns>返回转换后的结构体</returns>
        public JieOuStruct ListToStruct(List<string> tempList)
        {
            JieOuStruct tempStruct = new JieOuStruct();
            //解耦路数
            tempStruct.JieOuNum = Convert.ToInt32(tempList[0]);
            //解耦属性
            List<List<string>> tempAttribute=new List<List<string>> ();
            foreach (string elementAttribute in tempList[1].Split(new char[] { ';' }))
            {
                List<string> tempRoad = new List<string>();
                foreach (string element in elementAttribute.Split(new char[] { ',' }))
                { tempRoad.Add(element); }
                tempAttribute.Add(tempRoad);
            }
            tempStruct.JieOuAttribute = tempAttribute;
            //解耦矩阵
            string[,] tempTable = new string[tempStruct.JieOuNum, tempStruct.JieOuNum];
            string[] tempI = tempList[2].Split(new char[] { ';' });
            for (int i = 0; i < tempStruct.JieOuNum; i++)
            {
                string[] tempJ = tempI[i].Split(new char[] { ',' });
                for (int j = 0; j < tempStruct.JieOuNum; j++)
                {
                    tempTable[i, j] = tempJ[j];
                }
            }
            tempStruct.JieOuTable = tempTable;

            return tempStruct;
        }
    }



    internal class JieOuCassType : UITypeEditor
    {
        //为 UITypeEditor 提供一个接口，用于显示 Windows 窗体，或者在设计模式下在属性网格控件的下拉区域中显示控件。 
        IWindowsFormsEditorService editorService;
        //Gallery cassGallery = null;     //初始化Cass图库窗口新实例

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

            JieOu ConvertTool = new JieOu();
            JieOuForm1 Newfrm = new JieOuForm1(ConvertTool.ListToStruct((List<string>)value));
            editorService.ShowDialog(Newfrm);
            return ConvertTool.StructToList(Newfrm.newStruct);
  
        }
    }
        
}
