using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ControlTactic.SpecialControl
{
    public partial class FuzzyForm2 : Form
    {
        public List<string> rows = new List<string>();//偏差模糊论域(列标题)
        public List<string> columns = new List<string>();//偏差变化率模糊论域(行标题)
        public string[,] OutputBox = null;//用来返回的模糊控制表

        public FuzzyForm2()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 外部调用来根据传入的数组创建模糊控制表
        /// </summary>
        /// <param name="InputTable">传入的模糊控制表数组</param>
        public void CreateTable(string[,] InputTable)
        {
            DataGridViewTextBoxColumn newFirstColumn = new DataGridViewTextBoxColumn();
            newFirstColumn.ReadOnly = true;
            ControlTableBox.Columns.Add(newFirstColumn);

            foreach (string column in columns)
            {
                DataGridViewTextBoxColumn newColumn = new DataGridViewTextBoxColumn();
                newColumn.Name = column;
                ControlTableBox.Columns.Add(newColumn);
            }
            for (int RowIndex = 0; RowIndex < rows.Count;RowIndex++ )
            {
                DataGridViewRow newRow = new DataGridViewRow();
                ControlTableBox.Rows.Add(newRow);
                ControlTableBox.Rows[RowIndex].Cells[0].Value = rows[RowIndex];
                for (int ColumnIndex = 1; ColumnIndex < ControlTableBox.Columns.Count; ColumnIndex++)
                {//仅仅全初始化0；实际需要读取数值赋值；以后修改
                    ControlTableBox.Rows[RowIndex].Cells[ColumnIndex].Value = InputTable[RowIndex, ColumnIndex - 1];
                }
            }
        }

        /// <summary>
        /// 确认事件,把当前的模糊控制表值放入返回用的数组
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void OK_Click(object sender, EventArgs e)
        {              
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {              
                for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                {
                    OutputBox[rowIndex, columnIndex] = ControlTableBox.Rows[rowIndex].Cells[columnIndex + 1].Value.ToString();
                }          
            }
        }

        /// <summary>
        /// 窗口大小调整的事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="e">事件数据</param>
        private void FuzzyForm2_SizeChanged(object sender, EventArgs e)
        {
            Size boxSize = new Size();
            boxSize.Height = this.Size.Height - 65;
            boxSize.Width = this.Size.Width - 95;
            ControlTableBox.Size = boxSize;
            Point ButtonLocation = new Point();
            ButtonLocation.Y = 12;
            ButtonLocation.X = this.Size.Width - 70;
            LoadDate.Location = ButtonLocation;
            ButtonLocation.Y += 45;
            SaveDate.Location = ButtonLocation;
            ButtonLocation.Y += 45;
            OK.Location = ButtonLocation;
            ButtonLocation.Y += 45;
            Cancel.Location = ButtonLocation;
        }
        
        /// <summary>
        /// 读取模糊控制表文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadDate_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();  //文件对话框    
                dialog.DefaultExt = "txt";
                dialog.Filter = "FuzzyTable Files|*.txt";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FileStream fStream = new FileStream(dialog.FileName, FileMode.Open);  //读工程文件内容
                    StreamReader sReader = new StreamReader(fStream);
                    this.ControlTableBox.Rows.Clear();
                    this.ControlTableBox.Columns.Clear();

                    string tempLine = null;
                    //while ((tempLine = sReader.ReadLine()) != null)
                    //{
                    for (int i = 0; ; i++)
                    {
                        if ((tempLine = sReader.ReadLine()) == null)
                        { break; }
                        string[] tempRow = tempLine.Split(',');
                        if (i == 0)
                        {//第一行为列
                            for (int x = 0; x < tempRow.Length; x++)
                            {
                                this.ControlTableBox.Columns.Add(tempRow[x], tempRow[x]);
                            }
                        }
                        else
                        {
                            this.ControlTableBox.Rows.Add();
                            for (int y = 0; y < tempRow.Length; y++)
                            {
                                this.ControlTableBox.Rows[ControlTableBox.RowCount - 1].Cells[y].Value = tempRow[y];
                            }
                        }
                    }
                    sReader.Close();
                    fStream.Close();
                }
            }
            catch
            {
                MessageBox.Show("模糊控制表加载失败！");
            }
        }
        /// <summary>
        /// 保存模糊控制表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveDate_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = "txt";
                dialog.Filter = "FuzzyTable Files|*.txt";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FileStream fStream = new FileStream(dialog.FileName, FileMode.Create);
                    StreamWriter sWriter = new StreamWriter(fStream);
                    string[] tempColumn = new string[this.ControlTableBox.ColumnCount];
                    for (int i = 0; i < this.ControlTableBox.ColumnCount; i++)
                    {
                        tempColumn[i] = this.ControlTableBox.Columns[i].Name;
                    }
                    sWriter.WriteLine(String.Join(",", tempColumn));//第一行记录列名
                    for (int i = 0; i < this.ControlTableBox.RowCount; i++)
                    {
                        string[] tempRow = new string[this.ControlTableBox.ColumnCount];
                        for (int j = 0; j < this.ControlTableBox.ColumnCount; j++)
                        {
                            tempRow[j] = this.ControlTableBox.Rows[i].Cells[j].Value.ToString();
                        }
                        sWriter.WriteLine(String.Join(",", tempRow));
                    }
                    sWriter.Close();
                    fStream.Close();
                }
            }
            catch
            {
                MessageBox.Show("模糊控制表保存失败！");
            }
        }
    }
}