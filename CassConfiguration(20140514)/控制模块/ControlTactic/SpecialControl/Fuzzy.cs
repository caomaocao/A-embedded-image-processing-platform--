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
    //模糊控制块存放数据的结构体
    public struct FuzzyStruct
    {
        public int RowNum;//偏差模糊论域数
        public int ColumnNum;//偏差变化率模糊论域数
        //public List<string> RowValue;//偏差模糊论域值
        //public List<string> ColumnValue;//偏差变化率模糊论域值
        public string[,] ControlTable;//模糊控制表
    }

    public partial class Fuzzy : Arithmetic
    {
        public Fuzzy()
        {
            InitializeComponent();
        }

        //以数组的方式初始化话数据列表
        private List<string> configuration
            = new List<string>(new string[] { "1", "1", "0,0,0;0,0,0;0,0,0" });
        [Category("功能属性")]
        [DisplayName("模糊控制表")]
        [EditorAttribute(typeof(FuzzyCassType), typeof(System.Drawing.Design.UITypeEditor))]
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
        /// 将模糊控制块的结构体转换成列表
        /// </summary>
        /// <param name="tempStuct">传入的结构体</param>
        /// <returns>返回转换后的列表</returns>
        public List<string> StructToList(FuzzyStruct tempStuct)
        {
            List<string> tempList = new List<string>();
            //模糊论域数
            tempList.Add(tempStuct.RowNum.ToString());
            tempList.Add(tempStuct.ColumnNum.ToString());
            //模糊控制表
            List<string> Ctable = new List<string>();
            for (int i = 0; i < tempStuct.RowNum * 2 + 1; i++)
            {
                List<string> row = new List<string>();
                for (int j = 0; j < tempStuct.ColumnNum * 2 + 1; j++)
                {
                    row.Add(tempStuct.ControlTable[i, j]);
                }
                Ctable.Add(String.Join(",", row.ToArray()));
            }
            tempList.Add(String.Join(";", Ctable.ToArray()));

            return tempList;
        }

        /// <summary>
        /// 将模糊控制表的数据列表转换成结构体
        /// </summary>
        /// <param name="tempList">传入的列表</param>
        /// <returns>返回转换后的结构体</returns>
        public FuzzyStruct ListToStruct(List<string> tempList)
        {
            FuzzyStruct tempStruct = new FuzzyStruct();
            //模糊论域数
            tempStruct.RowNum = Convert.ToInt32(tempList[0]);
            tempStruct.ColumnNum = Convert.ToInt32(tempList[1]);
            //模糊控制表
            string[,] tempTable = new string[tempStruct.RowNum * 2 + 1, tempStruct.ColumnNum * 2 + 1];
            string[] tempI = tempList[tempList.Count - 1].Split(new char[] { ';' });
            for (int i = 0; i < tempStruct.RowNum * 2 + 1; i++)
            {
                string[] tempJ = tempI[i].Split(new char[] { ',' });
                for (int j = 0; j < tempStruct.ColumnNum * 2 + 1; j++)
                {
                    tempTable[i, j] = tempJ[j];
                }
            }
            tempStruct.ControlTable = tempTable;

            return tempStruct;
        }
    }



    internal class FuzzyCassType : UITypeEditor
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

            //object类型转换成自定义结构体方法
            //FuzzyStruct temp = (FuzzyStruct)value;
            //List<List<string>> temp = (List<List<string>>)value;
            Fuzzy ConvertTool = new Fuzzy();
            FuzzyForm1 Newfrm = new FuzzyForm1(ConvertTool.ListToStruct((List<string>)value));
            editorService.ShowDialog(Newfrm);
            return ConvertTool.StructToList(Newfrm.newStruct);
        }
    }


}
