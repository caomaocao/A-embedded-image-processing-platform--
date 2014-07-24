/*******************************************************************************
           ** Copyright (C) 2009 CASS ��Ȩ����
           ** �ļ�����CassView.cs 
           ** ����������
           **          �������ù��������пؼ��Ŀɼ����Եĵ�ַ
           ** ���ߣ����罡
           ** ��ʼʱ�䣺2009-5-5
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
        private List<string[]> AddressInfo = new List<string[]>();//��ǰ����ʹ�õ��б�       
        static public List<string> Length4 = new List<string>(new string[] { "System.Int32", "System.Single" });//��ַλ4�ֽ�����
        static public List<string> Type4 = new List<string>(new string[] { "uint32", "fp32" });
        static public List<string> Length1 = new List<string>(new string[] { "MyEnum", "System.Boolean", "System.Byte" });//��ַλ1�ֽ�����
        static public List<string> TypeName = new List<string>(new string[] { "32λ����", "�����ȸ�����", "�߼��ͱ���", "�ֽ�" });
        //static public List<string> Type1 = new List<string>(new string[] { "uint8", "uint8" });
        private string SetPath = null;//�趨·��
        private bool isSaved = true;//�޸ı�����
        private string SaveFileName = "AddressSET.txt";

        public int addressLength = 0;//��ַ����
        public List<string[]> ReturnTable = new List<string[]>();//���滹ԭ�ð汾
        public int RW = 0;
        static public int Saddress = 9999;//��ʼ��ַ
        static public int Eaddress = 0;//������ַ

        public AddressTable(List<ArrayList> CassInfos)
        {
            InitializeComponent();
            if (ReturnTable.Count == 0)
            {
                GetAddreesInfo(CassInfos);
            }
            reFreshAddressTable();//��AddressInfoˢ�±��

          //  this.SetPath = Path.Combine(CassViewGenerator.programPath + "\\" + CassViewGenerator.ProjectName, this.SaveFileName);
            this.SetPath = Path.Combine(CassViewGenerator.WorkSpacePath +"\\" + CassViewGenerator.ProjectName,this.SaveFileName );
            if (File.Exists(SetPath))
            {//���ڱ����ַ�ļ����ȡ
                LoadAddress(SetPath);
            }

            SaveListInfo(this.AddressInfo);//��AddressInfo������ReturnInfo 

            isSaved = true;
        }

        public AddressTable(List<ControlInfo> CodeCtrls, Dictionary<string, string[]> CodeArrays)
        {
            InitializeComponent();
            if (ReturnTable.Count == 0)
            {
                GetAddreesInfo(CodeCtrls, CodeArrays);
            }
            reFreshAddressTable();//��AddressInfoˢ�±��

        //    this.SetPath = Path.Combine(CassViewGenerator.programPath + "\\" + CassViewGenerator.ProjectName, this.SaveFileName);
            this.SetPath = Path.Combine(CassViewGenerator.WorkSpacePath + "\\" + CassViewGenerator.ProjectName, this.SaveFileName);

            if (File.Exists(SetPath))
            {//���ڱ����ַ�ļ����ȡ
                LoadAddress(SetPath);
            }

            SaveListInfo(this.AddressInfo);//��AddressInfo������ReturnInfo 

            isSaved = true;
        }


        /// <summary>
        /// �����������л�ȡ�ؼ��ĵ�ַ������Ϣ
        /// </summary>
        /// <param name="CassInfos"></param>
        private void GetAddreesInfo(List<ArrayList> CassInfos)
        {
            int Count = 0;//�������
            foreach (ArrayList node in CassInfos)
            {
                CassView curCass = (CassView)(node[1]);
                foreach (ControlInfo ctrl in curCass.ctrlsInfo)
                {
                    if (ctrl.CodeInfo[2] != null && ctrl.VisibleFunctionProperty != null)
                    {//�е���
                        if (ctrl.VisibleFunctionProperty.Count != 0)
                        {
                            foreach (XProp attribute in ctrl.VisibleFunctionProperty)
                            {
                                if (attribute.VarName != CassViewGenerator.portIndex)
                                {//�ų��ɼ������еĵ��������Ϣ
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
                                    {//bool��char��ͳһΪuint8 20090621
                                        adresInfo[2] = "uint8";
                                        adresInfo[3] = "1";
                                    }
                                    adresInfo[5] = "False";//��ʼʱ��ַΪ�� �������ÿɶ���дΪfalse

                                    this.AddressInfo.Add(adresInfo);
                                }
                            }
                        }
                    }
                }
                //��ȡģ���ͽ�������
                List<string[]> tempList = GenerateCode.CatchArrayInfo(curCass, ref Count);
                foreach (string[] element in tempList)
                {
                    this.AddressInfo.Add(ConvertInfo(element));
                }
            }
        }

        /// <summary>
        /// ��ָ����������Ϣ�л�ȡ��ַ��Ϣ
        /// </summary>
        /// <param name="CassInfos"></param>
        private void GetAddreesInfo(List<ControlInfo> CodeCtrls, Dictionary<string, string[]> CodeArrays)
        {
            foreach (ControlInfo ctrl in CodeCtrls)
            {
                if (ctrl.CodeInfo[2] != null && ctrl.VisibleFunctionProperty != null )//�е���
                {
                    if (ctrl.VisibleFunctionProperty.Count != 0)
                    {
                        foreach (XProp attribute in ctrl.VisibleFunctionProperty)
                        {
                            if (attribute.VarName != CassViewGenerator.portIndex)
                            {//�ų��ɼ������еĵ��������Ϣ
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
                                {//bool��char��ͳһΪuint8 20090621
                                    adresInfo[2] = "uint8";
                                    adresInfo[3] = "1";
                                }
                                adresInfo[5] = "False";//��ʼʱ��ַΪ�� �������ÿɶ���дΪfalse

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
        /// �Ѵ������Ŀؼ���Ϣ�л�ȡ��������Ϣת������ʾ�ڵ�ַ�������õ�������Ϣ
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string[] ConvertInfo(string[] info)
        {
            string[] NeedInfo = new string[8];//��ַ������Ҫ����Ϣ
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
            NeedInfo[5] = "False";//��ʼʱ��ַΪ�� �������ÿɶ���дΪfalse
            NeedInfo[6] = info[6];
            NeedInfo[7] = info[5];
            return NeedInfo;
        }



        /// <summary>
        /// �Ա����ı�������д��Datagridview
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
        /// ���������и��Ƶ��������
        /// </summary>
        /// <param name="Savelist"></param>
        private void SaveListInfo(List<string[]> Savelist)
        {
            this.ReturnTable = new List<string[]>();
            for (int i = 0; i < Savelist.Count; i++)
            {
                if (Savelist[i][5] == "True")
                {//ֻ����ɶ���д��ַ��Ϣ20090626
                    this.ReturnTable.Add(Savelist[i]);
                }
            }
        }

        /// <summary>
        /// ���ݵ�ǰDatagridview���²����б�
        /// </summary>
        private void UpdateCurrentInfo()
        {
            for (int j = 0; j < this.AddressTableView.RowCount; j++)
            {
                for (int i = 0; i < this.AddressInfo.Count; i++)
                {
                    if (this.AddressTableView.Rows[j].Cells[0].Value.ToString() == this.AddressInfo[i][0]//ģ������ͬ
                        && this.AddressTableView.Rows[j].Cells[1].Value.ToString() == this.AddressInfo[i][1])//��������ͬ
                    {
                        if (this.AddressTableView.Rows[j].Cells[4].Value != null)
                        {
                            this.AddressInfo[i][4] = this.AddressTableView.Rows[j].Cells[4].Value.ToString();//��ַ��Ϣ
                        }
                        this.AddressInfo[i][5] = this.AddressTableView.Rows[j].Cells[5].Value.ToString();//����ѡ��   
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// ��ԭ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Revert_Click(object sender, EventArgs e)
        {
            reFreshAddressTable();
        }

        ///// <summary>
        ///// ���
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void MenuItem_Clear_Click(object sender, EventArgs e)
        //{
        //    this.AddressTableView.Rows.Clear();
        //    isSaved = false;
        //}

        ///// <summary>
        ///// ������Ϊ�ɶ���д
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
        ///// ������Ϊ����ʾ
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
        /// ֻ��ʾ����ʾ��
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
        /// ֻ��ʾ�ɶ���д��
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
        /// ��ʾ������
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
        /// ��յ�ַ��
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
        /// ��ַ�Զ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_AutoOrder_Click(object sender, EventArgs e)
        {
            addressLength = 0;
            try
            {
                if (this.AddressTableView.Rows[0].Cells[4].Value != null //��1�еĵ�ַ���ǿ� ��0
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
                        //this.AddressTableView.Rows[i].Cells[4].Value = "";//���
                    }
                    else
                    {
                        this.AddressTableView.Rows[i].Cells[4].Value = addressLength;//�ѵ�ǰ��ַλ������ַ��
                        //���㵱ǰ�н�����ĵ�ַ
                        addressLength = Convert.ToInt32(this.AddressTableView.Rows[i].Cells[3].Value)
                        + Convert.ToInt32(this.AddressTableView.Rows[i].Cells[4].Value);
                    }
                }                
            }
            catch
            {
                MessageBox.Show("�Զ�����ʧ�ܣ�");
                addressLength = 0;
            }
            finally
            { 
                reFlashBool();
                UpdateCurrentInfo();//����               
                SelectErrorRows();
                isSaved = false;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Save_Click(object sender, EventArgs e)
        {
            SaveAddress(this.SetPath);
        }

        /// <summary>
        /// �����ַ
        /// �������е�ַ�ļ��ɶ���д�ĵ�ַ
        /// </summary>
        private void SaveAddress(string SavePath)
        {
            SaveListInfo(this.AddressInfo);

            //AddressTable.Saddress = 9999;//��ʼ��ַ��ʼ��
            //AddressTable.Eaddress = 0;//ĩβ��ַ��ʼ��
            FileStream fStream = new FileStream(SavePath, FileMode.Create);
            StreamWriter sWriter = new StreamWriter(fStream);

            for (int i = 0; i < this.AddressInfo.Count; i++)
            {
                string[] tempRow = new string[this.AddressInfo[i].Length];
                if (this.AddressInfo[i][5] == "True")
                {//ֻ����ɶ���д
                    for (int j = 0; j < this.AddressTableView.ColumnCount; j++)
                    {//ǰ��λ�����ʽ���ݱ��е�˳�� ����Loadʱ��ֵ
                        if (this.AddressTableView.Rows[i].Cells[j].Value != null)
                            tempRow[j] = this.AddressTableView.Rows[i].Cells[j].Value.ToString();
                        else
                            tempRow[j] = "";
                    }
                    tempRow[6] = this.AddressInfo[i][6];//����λ����Ӣ��������
                    //if (tempRow.Length == 8)
                    //{ tempRow[7] = this.AddressInfo[i][7]; }
                    sWriter.WriteLine(String.Join(";", tempRow));//�÷ֺ�������������ֵ�ָ��Ķ���
                }              

            }
            sWriter.Close();
            fStream.Close();
            SelectErrorRows();
            isSaved = true;
        }

        /// <summary>
        /// ����·�������ַ��Ϣ�ļ�
        /// ����Ϣˢ�������Ͳ�������AddressInfo
        /// </summary>
        /// <param name="LoadPath"></param>
        private void LoadAddress(string LoadPath)
        {
 
            FileStream fStream = new FileStream(LoadPath, FileMode.Open);  //�������ļ�����
            StreamReader sReader = new StreamReader(fStream);
            string tempLine = null;

            while ((tempLine = sReader.ReadLine()) != null)
            {
                string[] tempRow = tempLine.Split(';');
                for (int row = 0; row < AddressTableView.RowCount; row++)
                {
                    bool findRow = true;
                    for (int column = 0; column < 4; column++)
                    {//ǰ��λ������ͬ ��ģ���������������������ͺͳ���
                        if (this.AddressTableView.Rows[row].Cells[column].Value.ToString() != tempRow[column])
                        { findRow = false; }
                    }
                    if (findRow)
                    {//��Ӧ����ֵ���ĵ�����ֵ
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
        /// ��XML�ļ�����ԭ���趨���е�ַд��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Load_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();  //���ļ��Ի���
                dialog.DefaultExt = "txt";
                dialog.Filter = "Load Address(*.txt)|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadAddress(dialog.FileName);
                }
            }
            catch
            { CassMessageBox.Error("�����ַ��ʧ�ܣ�"); }
        }

        /// <summary>
        /// ����ֵַ������ֵ�����ı�
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
        /// ���ݵ�ַ�ж��Ƿ�ɶ���д
        /// </summary>
        private void reFlashBool()
        {
            for (int j = 0; j < this.AddressTableView.RowCount; j++)
            {
                if (this.AddressTableView.Rows[j].Cells[4].Value != null
                    && this.AddressTableView.Rows[j].Cells[4].Value.ToString() != "")//���ɼ�
                {
                    this.AddressTableView.Rows[j].Cells[5].Value = "True";
                }
                else//�ɶ���д||ֻ��
                {
                    this.AddressTableView.Rows[j].Cells[5].Value = "False";
                }
            }
        }

        /// <summary>
        /// �رմ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// �ر��¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressTable_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.AddressTableView.EndEdit();
            //�������ı༭״̬
            if (isSaved)
            {
                SaveListInfo(this.AddressInfo);//�ѵ�ǰ��ַ��Ϣ���浽�ⲿ���õ�ReturnTable�б�
                CountSize();
                this.DialogResult = DialogResult.Yes;
                this.Dispose();
            }
            else
            {//�޸ĺ�û�б�����ѯ��
                DialogResult result = CassMessageBox.QuestionT("�Ƿ񱣴����е�ַ��");
                if (result == DialogResult.Yes)
                {
                    UpdateCurrentInfo();
                    //SelectErrorRows();
                    SaveAddress(this.SetPath);
                    CountSize();
                    this.DialogResult = DialogResult.Yes;
                    this.Dispose(); //�ͷ���Դ  
                }
                else if (result == DialogResult.No)
                {
                    this.DialogResult = DialogResult.No;
                    this.Dispose();        //�ͷ���Դ              
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// ����ɿɶ���д�ĵ�ַ��С
        /// ����ȡ��ַ������ĩ��
        /// </summary>
        private void CountSize()
        {
            int RWcount = 0;
            AddressTable.Saddress = 9999;//��ʼ��ַ��ʼ��
            AddressTable.Eaddress = 0;//ĩβ��ַ��ʼ��

            foreach (string[] element in this.ReturnTable)
            {
                if (element[5] == "True")//�ɶ���д
                {
                    RWcount += Convert.ToInt32(element[3]);
                    //��ȡ��ַ��ĩ
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
        /// ���Ϊ�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_SaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();  //���ļ��Ի���
                dialog.DefaultExt = "txt";
                dialog.Filter = "Save Address(*.txt)|(*.txt)";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SaveAddress(dialog.FileName);
                }
            }
            catch
            { CassMessageBox.Error("�����ַ��ʧ�ܣ�"); }
        }

        /// <summary>
        /// ����Ƿ��г�ͻ��ַ ���س�ͻ��ַ�Ĵ�����ʾ
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
                    {//��һ����ַ�����������ַ
                        AddressIndexs.Add(address[i]);
                    }
                    else
                    {
                        bool upLine = true;//�����Ƿ����ظ�
                        bool downLine = true;//�����Ƿ����ظ�
                        for (int j = 0; j < AddressIndexs.Count; j++)
                        {
                            int curStart = Convert.ToInt32(AddressIndexs[j][4]);

                            if (curStart >= startAddress)
                            {//�ҵ���ַ����� ��Ҫ�ж��Ƿ����ظ���ַ����
                                if (startAddress + lengthAddress <= curStart)
                                {//��ַ���޼��
                                    upLine = false;
                                }
                                if (j - 1 < 0 
                                    || (j - 1 >= 0 && Convert.ToInt32(AddressIndexs[j - 1][4]) + Convert.ToInt32(AddressIndexs[j - 1][3]) <= startAddress))
                                {//��ַ���޼��
                                    downLine = false;
                                }
                                if (!upLine && !downLine)
                                {//������������
                                    AddressIndexs.Insert(j, address[i]);
                                }
                                else
                                {
                                    errorInfo.Add(new string[] { null, "�ؼ�����Ϊ" + address[i][0] + "������" 
                                        + address[i][1] + "��ؼ�����Ϊ" + address[j][0] + "������" + address[j][1] + "���ֵ�ַ�ظ���", "error" });
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
        /// ѡ�����г���ĵ�ַ��
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
                CassMessageBox.Error("��ַ�г���" + infos.Count.ToString() + "����ַ�ظ���");
                return true;
            }
        }

        /// <summary>
        /// ���������ʱ����������DGV�ĵ�Ԫ��༭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SLmenuStrip_Click(object sender, EventArgs e)
        {
            this.AddressTableView.EndEdit();
        }

        /// <summary>
        /// ��XML�л�ȡ�Ĳ���������Ϣת������ʾ��Ϣ���û�����������
        /// </summary>
        /// <param name="Oldtype"></param>
        /// <returns></returns>
        static public string ConvertShowType(string Oldtype)
        {
            if (AddressTable.Length4.Contains(Oldtype))
            {//32λ��������
                return AddressTable.Type4[AddressTable.Length4.IndexOf(Oldtype)];
            }
            else if (AddressTable.Length1.Contains(Oldtype))
            {//8λ��������
                return "uint8";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// ��XML�л�ȡ�Ĳ���������Ϣת������ʾ��Ϣ���û���������������
        /// </summary>
        /// <param name="Oldtype"></param>
        /// <returns></returns>
        static public string ConvertShowType2(string Oldtype)
        {
            if (AddressTable.Length4.Contains(Oldtype))
            {//32λ��������
                return AddressTable.TypeName[AddressTable.Length4.IndexOf(Oldtype)];
            }
            else if (AddressTable.Length1.IndexOf(Oldtype)==1)
            {//bool����
                return AddressTable.TypeName[2];
            }
            else
            {
                return AddressTable.TypeName[3];
            }
        }

        //����1 ��ַ��������
    }
}