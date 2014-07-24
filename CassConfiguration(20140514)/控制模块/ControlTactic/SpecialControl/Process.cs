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
    //条件动作块存放数据的结构体
    public struct ProcessStruct 
    {
        public int NumOfConditions;//条件数
        public int NumOfActions;//动作数
        public bool IsOnlyStart;//是否只在开始计算
        public List<string> Conditions;//条件列表
        public List<List<string>> Actions;//动作列表
        public string[,] OrderBox;//顺序表
        public List<string> Tactic;//条件动作块所在页面的子策略列表
        public List<List<string>> ControlAttribute;//所在页面控件的属性
    }

    public partial class Process : DAControl
    {
        public Process()
        {
            InitializeComponent();
        }

        //以数组的方式初始化数据列表
        List<string> configuration
            = new List<string>(new string[] { "10", "10", "1", ";;;;;;;;;", "||;||;||;||;||;||;||;||;||;||", null, null, null });         
        [Category("功能属性")]
        [DisplayName("条件动作组态")]
        [EditorAttribute(typeof(ProcessCassType), typeof(System.Drawing.Design.UITypeEditor))]
        public List<string> Configuration
        {
            get {
                return configuration; }
            set { configuration = value; 
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
        /// 将条件动作块的结构体转换成列表
        /// </summary>
        /// <param name="tempStuct">传入的结构体</param>
        /// <returns>返回转换后的列表</returns>
        public List<string> StructToList(ProcessStruct tempStuct)
        {
            List<string> tempList = new List<string>();
            //条件数和动作数
            tempList.Add(tempStuct.NumOfConditions.ToString());
            tempList.Add(tempStuct.NumOfActions.ToString());
            //是否只在开始计算
            if (tempStuct.IsOnlyStart == true)
                tempList.Add("1");
            else
                tempList.Add("0");
            //条件列表
            tempList.Add(String.Join(";", tempStuct.Conditions.ToArray()));
            //动作列表
            List<string> tempAction = new List<string>();
            foreach (List<string> action in tempStuct.Actions)
            {
                tempAction.Add(String.Join("|", action.ToArray()));
            }
            tempList.Add(String.Join(";", tempAction.ToArray()));
            //顺序表
            List<string> tempBox = new List<string>();
            for (int i = 0; i < tempStuct.NumOfConditions; i++)
            {
                List<string> tempRow = new List<string>();
                for (int j = 0; j < tempStuct.NumOfActions; j++)
                {
                    tempRow.Add(tempStuct.OrderBox[i, j]);
                }
                tempBox.Add(String.Join(",", tempRow.ToArray()));
            }
            tempList.Add(String.Join(";", tempBox.ToArray()));
            //子策略
            tempList.Add(String.Join(",", tempStuct.Tactic.ToArray()));
            //控件属性
            if (tempStuct.ControlAttribute.Count != 0)
            {
                foreach (List<string> element in tempStuct.ControlAttribute)
                {//每个element为一个控件属性，第一位存放控件名
                    tempList.Add(String.Join(";", element.ToArray()));
                }
            }
            else
            { tempList.Add(null); }

            return tempList;
        }

        /// <summary>
        /// 将条件控制块的数据列表转换成结构体
        /// </summary>
        /// <param name="tempList">传入的列表</param>
        /// <returns>返回转换后的结构体</returns>
        public ProcessStruct ListToStruct(List<string> tempList)
        {
            ProcessStruct tempStruct = new ProcessStruct();
            //条件数和动作数
            tempStruct.NumOfConditions = Convert.ToInt32(tempList[0]);
            tempStruct.NumOfActions = Convert.ToInt32(tempList[1]);
            //是否只在开始计算
            if (tempList[2] == "1")
                tempStruct.IsOnlyStart = true;
            else
                tempStruct.IsOnlyStart = false;
            //条件列表
            List<string> tempConditions = new List<string>();
            foreach (string condition in tempList[3].Split(new char[] { ';' }))
            { tempConditions.Add(condition); }
            tempStruct.Conditions = tempConditions;
            //动作列表
            List<List<string>> tempActions = new List<List<string>>();
            foreach (string action in tempList[4].Split(new char[] { ';' }))
            {
                List<string> tempAction = new List<string>();
                foreach (string element in action.Split(new char[] { '|' }))
                { tempAction.Add(element); }
                tempActions.Add(tempAction);
            }
            tempStruct.Actions = tempActions;
            //顺序表
            string[,] tempBox = new string[tempStruct.NumOfConditions, tempStruct.NumOfActions];
            if (tempList[5] != null)
            {
                string[] tempRow = tempList[5].Split(new char[] { ';' });
                for (int i = 0; i < tempStruct.NumOfConditions; i++)
                {
                    string[] temp = tempRow[i].Split(new char[] { ',' });
                    for (int j = 0; j < tempStruct.NumOfActions; j++)
                    { tempBox[i, j] = temp[j]; }
                }
            }
            tempStruct.OrderBox = tempBox;
            //子策略
            List<string> tempTactic = new List<string>();
            if (tempList[6] != null)
            {
                foreach (string tactic in tempList[6].Split(new char[] { ',' }))
                { tempTactic.Add(tactic); }
            }
            tempStruct.Tactic = tempTactic;
            //控件属性
            List<List<string>> tempCAtrribute = new List<List<string>>();
            if (tempList[7] != null)
            {
                for (int i = 7; i < tempList.Count; i++)
                {
                    string[] element = tempList[i].Split(new char[] { ';' });
                    List<string> CElement = new List<string>();
                    foreach (string attribute in element)
                    { CElement.Add(attribute); }
                    tempCAtrribute.Add(CElement);
                }
            }
            tempStruct.ControlAttribute = tempCAtrribute;

            return tempStruct;
        }
    }



    internal class ProcessCassType : UITypeEditor
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

            Process ConvertTool = new Process();
            ProcessForm1 Newfrm = new ProcessForm1(ConvertTool.ListToStruct((List<string>)value));
            editorService.ShowDialog(Newfrm);
            return ConvertTool.StructToList(Newfrm.newStruct);
        }
    }
}
