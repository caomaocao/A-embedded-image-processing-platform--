/*******************************************************************************
           ** Copyright (C) 2009 CASS 版权所有
           ** 文件名：CassView.cs 
           ** 功能描述：
           **          用于设置工程中所有控件的可见属性的地址
           ** 作者：宋骁健
           ** 创始时间：2009-5-5
           ** 
********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CaVeGen.DesignViewFiles.FilterProperty;
using CaVeGen.CommonOperation;
using System.Xml;
using System.Xml.Xsl;

namespace CaVeGen.DesignViewFiles
{
    public partial class AddressTable : Form
    {
        private List<string[]> AddressInfo = new List<string[]>();//当前操作使用的列表       
        static public List<string> Length4 = new List<string>(new string[] { "System.Int32", "System.Single" });//地址位4字节类型
        static public List<string> Type4 = new List<string>(new string[] { "uint32", "fp32" });
        static public List<string> Length1 = new List<string>(new string[] { "MyEnum", "System.Boolean", "System.Byte" });//地址位1字节类型
        static public List<string> TypeName = new List<string>(new string[] { "32位整数", "单精度浮点数", "逻辑型变量", "字节" });
        //static public List<string> Type1 = new List<string>(new string[] { "uint8", "uint8" });
        private string SetPath = null;//设定路径
        private bool isSaved = true;//修改保存标号
        private string SaveFileName = "AddressSET.txt";

        public int addressLength = 0;//地址长度
        public List<string[]> ReturnTable = new List<string[]>();//保存还原用版本
        public int RW = 0;
        static public int Saddress = 9999;//起始地址
        static public int Eaddress = 0;//结束地址

        public AddressTable(List<ArrayList> CassInfos)
        {
            InitializeComponent();
            if (ReturnTable.Count == 0)
            {
                GetAddreesInfo(CassInfos);
            }
            reFreshAddressTable();//由AddressInfo刷新表格

          //  this.SetPath = Path.Combine(CassViewGenerator.programPath + "\\" + CassViewGenerator.ProjectName, this.SaveFileName);
            this.SetPath = Path.Combine(CassViewGenerator.WorkSpacePath +"\\" + CassViewGenerator.ProjectName,this.SaveFileName );
            if (File.Exists(SetPath))
            {//存在保存地址文件则读取
                LoadAddress(SetPath);
            }

            SaveListInfo(this.AddressInfo);//把AddressInfo保存至ReturnInfo 

            isSaved = true;
        }

        public AddressTable(List<ControlInfo> CodeCtrls, Dictionary<string, string[]> CodeArrays)
        {
            InitializeComponent();
            if (ReturnTable.Count == 0)
            {
                GetAddreesInfo(CodeCtrls, CodeArrays);
            }
            reFreshAddressTable();//由AddressInfo刷新表格

        //    this.SetPath = Path.Combine(CassViewGenerator.programPath + "\\" + CassViewGenerator.ProjectName, this.SaveFileName);
            this.SetPath = Path.Combine(CassViewGenerator.WorkSpacePath + "\\" + CassViewGenerator.ProjectName, this.SaveFileName);

            if (File.Exists(SetPath))
            {//存在保存地址文件则读取
                LoadAddress(SetPath);
            }

            SaveListInfo(this.AddressInfo);//把AddressInfo保存至ReturnInfo 

            isSaved = true;
        }


        /// <summary>
        /// 从容器集合中获取控件的地址属性信息
        /// </summary>
        /// <param name="CassInfos"></param>
        private void GetAddreesInfo(List<ArrayList> CassInfos)
        {
            int Count = 0;//数组序号
            foreach (ArrayList node in CassInfos)
            {
                CassView curCass = (CassView)(node[1]);
                foreach (ControlInfo ctrl in curCass.ctrlsInfo)
                {
                    if (ctrl.CodeInfo[2] != null && ctrl.VisibleFunctionProperty != null)
                    {//有点名
                        if (ctrl.VisibleFunctionProperty.Count != 0)
                        {
                            foreach (XProp attribute in ctrl.VisibleFunctionProperty)
                            {
                                if (attribute.VarName != CassViewGenerator.portIndex)
                                {//排除可见属性中的点名序号信息
                                    string[] adresInfo = new string[7];
                                    adresInfo[0] = ctrl.CodeInfo[2];
                                    adresInfo[1] = attribute.Name;
                                    adresInfo[6] = attribute.VarName;

                                    if (Length4.Contains(attribute.ValueType))
                                    {
                                        adresInfo[2] = Type4[Length4.IndexOf(attribute.ValueType)];
                                        adresInfo[3] = "4";
                                    }
                                    else if (Length1.Contains(attribute.ValueType))
                                    {//bool和char类统一为uint8 20090621
                                        adresInfo[2] = "uint8";
                                        adresInfo[3] = "1";
                                    }
                                    adresInfo[5] = "False";//初始时地址为空 所以设置可读可写为false

                                    this.AddressInfo.Add(adresInfo);
                                }
                            }
                        }
                    }
                }
                //获取模糊和解耦数据
                List<string[]> tempList = GenerateCode.CatchArrayInfo(curCass, ref Count);
                foreach (string[] element in tempList)
                {
                    this.AddressInfo.Add(ConvertInfo(element));
                }
            }
        }

        /// <summary>
        /// 从指令表产生的信息中获取地址信息
        /// </summary>
        /// <param name="CassInfos"></param>
        private void GetAddreesInfo(List<ControlInfo> CodeCtrls, Dictionary<string, string[]> CodeArrays)
        {
            foreach (ControlInfo ctrl in CodeCtrls)
            {
                if (ctrl.CodeInfo[2] != null && ctrl.VisibleFunctionProperty != null )//有点名
                {
                    if (ctrl.VisibleFunctionProperty.Count != 0)
                    {
                        foreach (XProp attribute in ctrl.VisibleFunctionProperty)
                        {
                            if (attribute.VarName != CassViewGenerator.portIndex)
                            {//排除可见属性中的点名序号信息
                                string[] adresInfo = new string[7];
                                adresInfo[0] = ctrl.CodeInfo[2];
                                adresInfo[1] = attribute.Name;
                                adresInfo[6] = attribute.VarName;

                                if (Length4.Contains(attribute.ValueType))
                                {
                                    adresInfo[2] = Type4[Length4.IndexOf(attribute.ValueType)];
                                    adresInfo[3] = "4";
                                }
                                else if (Length1.Contains(attribute.ValueType))
                                {//bool和char类统一为uint8 20090621
                                    adresInfo[2] = "uint8";
                                    adresInfo[3] = "1";
                                }
                                adresInfo[5] = "False";//初始时地址为空 所以设置可读可写为false

                                this.AddressInfo.Add(adresInfo);
                            }
                        }
                    }
                    if (ctrl.UnvisibleFunctionProperty.Count != 0)
                    {
                        foreach (XProp attribute in ctrl.UnvisibleFunctionProperty)
                        {
                            if (attribute.TheValue.ToString().Contains("array") && CodeArrays.ContainsKey(attribute.TheValue.ToString()))
                            {
                                this.AddressInfo.Add(ConvertInfo(CodeArrays[attribute.TheValue.ToString()]));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 把从容器的控件信息中获取的数组信息转换成显示在地址表中所用的数组信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string[] ConvertInfo(string[] info)
        {
            string[] NeedInfo = new string[8];//地址表中需要的信息
            NeedInfo[0] = info[0];
            NeedInfo[1] = info[1];
            NeedInfo[2] = info[2];
            if (Type4.Contains(NeedInfo[2]))
            {
                NeedInfo[3] = (4 * Convert.ToInt32(info[4])).ToString();
            }
            else if (NeedInfo[2] == "uint8")
            {
                NeedInfo[3] = Convert.ToInt32(info[4]).ToString();
            }
            NeedInfo[5] = "False";//初始时地址为空 所以设置可读可写为false
            NeedInfo[6] = info[6];
            NeedInfo[7] = info[5];
            return NeedInfo;
        }



        /// <summary>
        /// 对保留的备份重新写入Datagridview
        /// </summary>
        private void reFreshAddressTable()
        {
            this.AddressTableView.Rows.Clear();
            foreach (string[] adres in this.AddressInfo)
            {
                this.AddressTableView.Rows.Add();
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[0].Value = adres[0];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[1].Value = adres[1];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[2].Value = adres[2];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[3].Value = adres[3];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[4].Value = adres[4];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[5].Value = adres[5];
            }
        }

        /// <summary>
        /// 将操作队列复制到保存队列
        /// </summary>
        /// <param name="Savelist"></param>
        private void SaveListInfo(List<string[]> Savelist)
        {
            this.ReturnTable = new List<string[]>();
            for (int i = 0; i < Savelist.Count; i++)
            {
                if (Savelist[i][5] == "True")
                {//只保存可读可写地址信息20090626
                    this.ReturnTable.Add(Savelist[i]);
                }
            }
        }

        /// <summary>
        /// 根据当前Datagridview更新操作列表
        /// </summary>
        private void UpdateCurrentInfo()
        {
            for (int j = 0; j < this.AddressTableView.RowCount; j++)
            {
                for (int i = 0; i < this.AddressInfo.Count; i++)
                {
                    if (this.AddressTableView.Rows[j].Cells[0].Value.ToString() == this.AddressInfo[i][0]//模块名相同
                        && this.AddressTableView.Rows[j].Cells[1].Value.ToString() == this.AddressInfo[i][1])//参数名相同
                    {
                        if (this.AddressTableView.Rows[j].Cells[4].Value != null)
                        {
                            this.AddressInfo[i][4] = this.AddressTableView.Rows[j].Cells[4].Value.ToString();//地址信息
                        }
                        this.AddressInfo[i][5] = this.AddressTableView.Rows[j].Cells[5].Value.ToString();//属性选择   
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Revert_Click(object sender, EventArgs e)
        {
            reFreshAddressTable();
        }

        ///// <summary>
        ///// 清空
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void MenuItem_Clear_Click(object sender, EventArgs e)
        //{
        //    this.AddressTableView.Rows.Clear();
        //    isSaved = false;
        //}

        ///// <summary>
        ///// 所有行为可读可写
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void MenuItem_SelectAll_Click(object sender, EventArgs e)
        //{
        //    for (int i = 0; i < this.AddressTableView.RowCount; i++)
        //    {
        //        this.AddressTableView.Rows[i].Cells[5].Value = "True";
        //    }
        //    isSaved = false;
        //}

        ///// <summary>
        ///// 所有行为不显示
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void MenuItem_NoSelect_Click(object sender, EventArgs e)
        //{
        //    for (int i = 0; i < this.AddressTableView.RowCount; i++)
        //    {
        //        this.AddressTableView.Rows[i].Cells[5].Value = "False";
        //    }
        //    isSaved = false;
        //}

        /// <summary>
        /// 只显示不显示行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ViewSelect_Click(object sender, EventArgs e)
        {
            UpdateCurrentInfo();
            this.AddressTableView.Rows.Clear();
            for (int i = 0; i < AddressInfo.Count; i++)
            {
                if (this.AddressInfo[i][5] == "False")
                {
                    this.AddressTableView.Rows.Add();
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[0].Value = this.AddressInfo[i][0];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[1].Value = this.AddressInfo[i][1];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[2].Value = this.AddressInfo[i][2];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[3].Value = this.AddressInfo[i][3];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[4].Value = this.AddressInfo[i][4];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[5].Value = this.AddressInfo[i][5];
                }
            }
        }

        /// <summary>
        /// 只显示可读可写行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ViewUnselect_Click(object sender, EventArgs e)
        {
            UpdateCurrentInfo();
            this.AddressTableView.Rows.Clear();
            for (int i = 0; i < AddressInfo.Count; i++)
            {
                if (this.AddressInfo[i][5] == "True")
                {
                    this.AddressTableView.Rows.Add();
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[0].Value = this.AddressInfo[i][0];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[1].Value = this.AddressInfo[i][1];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[2].Value = this.AddressInfo[i][2];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[3].Value = this.AddressInfo[i][3];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[4].Value = this.AddressInfo[i][4];
                    this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[5].Value = this.AddressInfo[i][5];
                }
            }
        }

        /// <summary>
        /// 显示所有行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ViewAll_Click(object sender, EventArgs e)
        {
            UpdateCurrentInfo();
            this.AddressTableView.Rows.Clear();
            for (int i = 0; i < AddressInfo.Count; i++)
            {
                this.AddressTableView.Rows.Add();
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[0].Value = this.AddressInfo[i][0];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[1].Value = this.AddressInfo[i][1];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[2].Value = this.AddressInfo[i][2];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[3].Value = this.AddressInfo[i][3];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[4].Value = this.AddressInfo[i][4];
                this.AddressTableView.Rows[this.AddressTableView.RowCount - 1].Cells[5].Value = this.AddressInfo[i][5];
            }
        }

        /// <summary>
        /// 清空地址栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ClearAds_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.AddressTableView.RowCount; i++)
            {
                this.AddressTableView.Rows[i].Cells[4].Value = "";
                this.AddressTableView.Rows[i].Cells[5].Value = "False";
            }
            UpdateCurrentInfo();
            isSaved = false;
        }

        /// <summary>
        /// 地址自动排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_AutoOrder_Click(object sender, EventArgs e)
        {
            addressLength = 0;
            try
            {
                if (this.AddressTableView.Rows[0].Cells[4].Value != null //第1行的地址栏非空 非0
                    && (this.AddressTableView.Rows[0].Cells[4].Value.ToString() != ""
                    && this.AddressTableView.Rows[0].Cells[4].Value.ToString() != "0"))
                {
                    addressLength = Convert.ToInt32(this.AddressTableView.Rows[0].Cells[4].Value);
                }
                else
                {
                    addressLength = 0;
                }
                for (int i = 0; i < this.AddressTableView.RowCount; i++)
                {
                    if (this.AddressTableView.Rows[i].Cells[4].Value != null
                    && this.AddressTableView.Rows[i].Cells[4].Value.ToString() != "")
                    {
                        addressLength = Convert.ToInt32(this.AddressTableView.Rows[i].Cells[3].Value)
                          + Convert.ToInt32(this.AddressTableView.Rows[i].Cells[4].Value);                         
                        //this.AddressTableView.Rows[i].Cells[4].Value = "";//清空
                    }
                    else
                    {
                        this.AddressTableView.Rows[i].Cells[4].Value = addressLength;//把当前地址位赋到地址栏
                        //计算当前行结束后的地址
                        addressLength = Convert.ToInt32(this.AddressTableView.Rows[i].Cells[3].Value)
                        + Convert.ToInt32(this.AddressTableView.Rows[i].Cells[4].Value);
                    }
                }                
            }
            catch
            {
                MessageBox.Show("自动排序失败！");
                addressLength = 0;
            }
            finally
            { 
                reFlashBool();
                UpdateCurrentInfo();//更新               
                SelectErrorRows();
                isSaved = false;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Save_Click(object sender, EventArgs e)
        {
            SaveAddress(this.SetPath);
        }

        /// <summary>
        /// 保存地址
        /// 仅保存有地址的即可读可写的地址
        /// </summary>
        private void SaveAddress(string SavePath)
        {
            SaveListInfo(this.AddressInfo);

            //AddressTable.Saddress = 9999;//起始地址初始化
            //AddressTable.Eaddress = 0;//末尾地址初始化
            FileStream fStream = new FileStream(SavePath, FileMode.Create);
            StreamWriter sWriter = new StreamWriter(fStream);

            for (int i = 0; i < this.AddressInfo.Count; i++)
            {
                string[] tempRow = new string[this.AddressInfo[i].Length];
                if (this.AddressInfo[i][5] == "True")
                {//只保存可读可写
                    for (int j = 0; j < this.AddressTableView.ColumnCount; j++)
                    {//前六位保存格式根据表中的顺序 便于Load时赋值
                        if (this.AddressTableView.Rows[i].Cells[j].Value != null)
                            tempRow[j] = this.AddressTableView.Rows[i].Cells[j].Value.ToString();
                        else
                            tempRow[j] = "";
                    }
                    tempRow[6] = this.AddressInfo[i][6];//第六位放置英文属性名
                    //if (tempRow.Length == 8)
                    //{ tempRow[7] = this.AddressInfo[i][7]; }
                    sWriter.WriteLine(String.Join(";", tempRow));//用分号区别与数组数值分隔的逗号
                }              

            }
            sWriter.Close();
            fStream.Close();
            SelectErrorRows();
            isSaved = true;
        }

        /// <summary>
        /// 根据路径载入地址信息文件
        /// 把信息刷新至表格和操作队列AddressInfo
        /// </summary>
        /// <param name="LoadPath"></param>
        private void LoadAddress(string LoadPath)
        {
 
            FileStream fStream = new FileStream(LoadPath, FileMode.Open);  //读工程文件内容
            StreamReader sReader = new StreamReader(fStream);
            string tempLine = null;

            while ((tempLine = sReader.ReadLine()) != null)
            {
                string[] tempRow = tempLine.Split(';');
                for (int row = 0; row < AddressTableView.RowCount; row++)
                {
                    bool findRow = true;
                    for (int column = 0; column < 4; column++)
                    {//前四位属性相同 即模块名、中文属性名、类型和长度
                        if (this.AddressTableView.Rows[row].Cells[column].Value.ToString() != tempRow[column])
                        { findRow = false; }
                    }
                    if (findRow)
                    {//对应列则赋值第四第五列值
                        this.AddressTableView.Rows[row].Cells[4].Value = tempRow[4];
                        this.AddressTableView.Rows[row].Cells[5].Value = tempRow[5];
                        if (this.AddressInfo[row].Length == 8)
                        {
                            string tempArrayValue = this.AddressInfo[row][7];
                            this.AddressInfo[row] = tempRow;
                            this.AddressInfo[row][7] = tempArrayValue;
                        }
                        else
                        {
                            this.AddressInfo[row] = tempRow;
                        }
                        break;
                    }
                }
            }
            sReader.Close();
            fStream.Close();
            //reFlashBool();
            //SelectErrorRows();
        }

        /// <summary>
        /// 从XML文件导入原有设定进行地址写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Load_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();  //打开文件对话框
                dialog.DefaultExt = "txt";
                dialog.Filter = "Load Address(*.txt)|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadAddress(dialog.FileName);
                }
            }
            catch
            { CassMessageBox.Error("导入地址表失败！"); }
        }

        /// <summary>
        /// 表格地址值或属性值发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressTableView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            reFlashBool();
            UpdateCurrentInfo();
            isSaved = false;
        }

        /// <summary>
        /// 依据地址判断是否可读可写
        /// </summary>
        private void reFlashBool()
        {
            for (int j = 0; j < this.AddressTableView.RowCount; j++)
            {
                if (this.AddressTableView.Rows[j].Cells[4].Value != null
                    && this.AddressTableView.Rows[j].Cells[4].Value.ToString() != "")//不可见
                {
                    this.AddressTableView.Rows[j].Cells[5].Value = "True";
                }
                else//可读可写||只读
                {
                    this.AddressTableView.Rows[j].Cells[5].Value = "False";
                }
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressTable_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.AddressTableView.EndEdit();
            //结束表格的编辑状态
            if (isSaved)
            {
                SaveListInfo(this.AddressInfo);//把当前地址信息保存到外部所用的ReturnTable列表
                CountSize();
                this.DialogResult = DialogResult.Yes;
                this.Dispose();
            }
            else
            {//修改后没有保存则询问
                DialogResult result = CassMessageBox.QuestionT("是否保存现有地址？");
                if (result == DialogResult.Yes)
                {
                    UpdateCurrentInfo();
                    //SelectErrorRows();
                    SaveAddress(this.SetPath);
                    CountSize();
                    this.DialogResult = DialogResult.Yes;
                    this.Dispose(); //释放资源  
                }
                else if (result == DialogResult.No)
                {
                    this.DialogResult = DialogResult.No;
                    this.Dispose();        //释放资源              
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// 计算可可读可写的地址大小
        /// 以摄取地址的起点和末点
        /// </summary>
        private void CountSize()
        {
            int RWcount = 0;
            AddressTable.Saddress = 9999;//起始地址初始化
            AddressTable.Eaddress = 0;//末尾地址初始化

            foreach (string[] element in this.ReturnTable)
            {
                if (element[5] == "True")//可读可写
                {
                    RWcount += Convert.ToInt32(element[3]);
                    //摄取地址起末
                    int s = Convert.ToInt32(element[4]);
                    int e = Convert.ToInt32(element[3]);
                    AddressTable.Saddress = Math.Min(AddressTable.Saddress, s);
                    AddressTable.Eaddress = Math.Max(AddressTable.Eaddress, s + e);
                }
            }
            //this.RW = RWcount;
            this.RW = AddressTable.Eaddress;
        }

        /// <summary>
        /// 另存为事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_SaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();  //打开文件对话框
                dialog.DefaultExt = "txt";
                dialog.Filter = "Save Address(*.txt)|(*.txt)";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SaveAddress(dialog.FileName);
                }
            }
            catch
            { CassMessageBox.Error("保存地址表失败！"); }
        }

        /// <summary>
        /// 检测是否有冲突地址 返回冲突地址的错误提示
        /// </summary>
        /// <returns></returns>
        static public List<string[]> checkRepeatAdres(List<string[]> address)
        {
            List<string[]> AddressIndexs = new List<string[]>();
            List<string[]> errorInfo = new List<string[]>();
            for (int i = 0; i < address.Count; i++)
            {
                if (address[i][4] != null && address[i][4] != "")
                {
                    int startAddress = Convert.ToInt32(address[i][4]);
                    int lengthAddress = Convert.ToInt32(address[i][3]);
                    //int[] newAddress = new int[] { startAddress, lengthAddress };
                    if (AddressIndexs.Count == 0
                        || startAddress >= Convert.ToInt32(AddressIndexs[AddressIndexs.Count - 1][4]) + Convert.ToInt32(AddressIndexs[AddressIndexs.Count - 1][3]))
                    {//第一个地址或则大于最大地址
                        AddressIndexs.Add(address[i]);
                    }
                    else
                    {
                        bool upLine = true;//上限是否有重复
                        bool downLine = true;//下限是否有重复
                        for (int j = 0; j < AddressIndexs.Count; j++)
                        {
                            int curStart = Convert.ToInt32(AddressIndexs[j][4]);

                            if (curStart >= startAddress)
                            {//找到地址插入点 需要判断是否有重复地址部分
                                if (startAddress + lengthAddress <= curStart)
                                {//地址上限检测
                                    upLine = false;
                                }
                                if (j - 1 < 0 
                                    || (j - 1 >= 0 && Convert.ToInt32(AddressIndexs[j - 1][4]) + Convert.ToInt32(AddressIndexs[j - 1][3]) <= startAddress))
                                {//地址下限检测
                                    downLine = false;
                                }
                                if (!upLine && !downLine)
                                {//上下限无问题
                                    AddressIndexs.Insert(j, address[i]);
                                }
                                else
                                {
                                    errorInfo.Add(new string[] { null, "控件点名为" + address[i][0] + "的属性" 
                                        + address[i][1] + "与控件点名为" + address[j][0] + "的属性" + address[j][1] + "出现地址重复！", "error" });
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return errorInfo;
        }

        /// <summary>
        /// 选出所有出错的地址格
        /// </summary>
        /// <returns></returns>
        private bool SelectErrorRows()
        {
            List<string[]> infos = checkRepeatAdres(this.AddressInfo);
            if (infos.Count == 0)
            {
                return false;
            }
            else
            {
                //for (int i = 0; i < infos.Count; i++)
                //{
                //    this.AddressTableView.Rows[indexs[i]].Cells[4].Selected = true;
                //}
                CassMessageBox.Error("地址中出现" + infos.Count.ToString() + "处地址重复！");
                return true;
            }
        }

        /// <summary>
        /// 点击工具栏时触发，结束DGV的单元格编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SLmenuStrip_Click(object sender, EventArgs e)
        {
            this.AddressTableView.EndEdit();
        }

        /// <summary>
        /// 将XML中获取的参数类型信息转换成提示信息中用户所见的类型
        /// </summary>
        /// <param name="Oldtype"></param>
        /// <returns></returns>
        static public string ConvertShowType(string Oldtype)
        {
            if (AddressTable.Length4.Contains(Oldtype))
            {//32位长度类型
                return AddressTable.Type4[AddressTable.Length4.IndexOf(Oldtype)];
            }
            else if (AddressTable.Length1.Contains(Oldtype))
            {//8位长度类型
                return "uint8";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 将XML中获取的参数类型信息转换成提示信息中用户所见的中文类型
        /// </summary>
        /// <param name="Oldtype"></param>
        /// <returns></returns>
        static public string ConvertShowType2(string Oldtype)
        {
            if (AddressTable.Length4.Contains(Oldtype))
            {//32位长度类型
                return AddressTable.TypeName[AddressTable.Length4.IndexOf(Oldtype)];
            }
            else if (AddressTable.Length1.IndexOf(Oldtype)==1)
            {//bool类型
                return AddressTable.TypeName[2];
            }
            else
            {
                return AddressTable.TypeName[3];
            }
        }

        //问题1 地址排序问题
    }
}