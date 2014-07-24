using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using ControlTactic;



namespace text3
{
    public partial class Form2 : Form
    {
        public string[] Basic = new string[8];
        public string[,] function = new string[100, 5];
        public string [] code ;
        public bool Savemode = true;     //确定属性是否被修改而需要保存
        public int index2;//功能属性的数目

        private bool TColorChange = false;//文本颜色是否被修改
        private bool MColorChange = false;//模块颜色是否被修改
        private int[] TextColorRGB = new int[4];
        private int[] ModuleColorRGB = new int[4];
        private bool FirstTime = false;
        private string InputInfo = null;//输入端口初值信息
        private string[] OtherInfo = new string[3];//其他信息


        public Form2(string ControlType)
        {
            InitializeComponent();
            if (ControlType != null)
            {
                ControlTacticName.Text = ControlType;
            }
            FirstTime = true;//因为构造也触发了DGV的数据变动事件和COMBOX变动事件，所以首次为TRUE
            List<string> tempSource
                = new List<string>(new string[] { "Modules", "Arithmetic", "Constant", "SystemConstant", "OutputValue", "InputValue", "Label", "JMP", "ADControl", "DAControl", "SpecialControl" });
            //ControlTacticName.DataSource = File.ReadAllLines("ControlTacticList.txt");
            ControlTacticName.DataSource = tempSource;
        }



        #region 修改模式
        ///<summary>
        /// 修改节点时需要调用的传值方法
        ///</summary>

        public void modify()
        {
            DisplayName.Text = Basic[0];

            if (Basic[1] != "")
            {
                MColorChange = true;
                ModuleColorRGB = StrToRGB(Basic[1]);
                ModuleOfColor.BackColor = Color.FromArgb(ModuleColorRGB[1], ModuleColorRGB[2], ModuleColorRGB[3]);
            }
            else
                ModuleOfColor.BackColor = tabPage1.BackColor;

            ModuleSort.Text = Basic[2];

            ModuleName.Text = Basic[3];

            if (Basic[4] != "")
            {
                TColorChange = true;
                TextColorRGB = StrToRGB(Basic[4]);
                TextOfColor.BackColor = Color.FromArgb(TextColorRGB[1], TextColorRGB[2], TextColorRGB[3]);
            }
            else
                TextOfColor.BackColor = tabPage1.BackColor;

            OutputName.Text = Basic[5];

            this.InputInfo = Basic[6];
            if (Basic[6].Split(';').Length > 1)
            {
                string[] temp = Basic[6].Split(';');
                List<string> tempInfo = new List<string>();
                foreach (string info in temp)
                {
                    tempInfo.Add(info.Split(',')[0]);
                }
                InputName.Text = String.Join(",", tempInfo.ToArray());
            }
            else
            {
                InputName.Text = Basic[6];//输入初值信息20090616
            }
            ControlTacticName.Text = Basic[7];

            DataGridViewRowCollection rows = this.dataGridView1.Rows;
            for (int i = 0; i < index2; i++)
            {
                rows.Add(function[i, 0], function[i, 1], function[i, 2], function[i, 3], function[i, 4],
                    function[i, 5], function[i, 6], function[i, 7], function[i, 8], function[i, 9]);

            }

            OtherInfo = code;
            infoName.SelectedIndex = 0;
            infoText.Text = code[0];
            
            if (index2 == 0)//没有功能属性
            {
                DeleteFunction.Enabled = false;
                ModifyFunction.Enabled = false;
            }
            if (FirstTime == true)
            {
                Savemode = true;
                FirstTime = false;
            }
        }




        #endregion

        #region 添加模式
        //添加节点时调用，调整控件状态
        public void AddMode()
        {
            ModifyFunction.Enabled = false;
            DeleteFunction.Enabled = false;

            for (int i = 0; i < Basic.Length; i++)
                Basic[i] = "";
        }
        #endregion

        #region 还原键
        private void Return_Click(object sender, EventArgs e)
        {
            modify();

            dataGridView1.Rows.Clear();

            DataGridViewRowCollection rows = this.dataGridView1.Rows;
            for (int i = 0; i < index2; i++)
            {
                rows.Add(function[i, 0], function[i, 1], function[i, 2], function[i, 3], function[i, 4]);

            }

        }
        #endregion

        #region 保存

        #region 保存键
        private void Save_Click(object sender, EventArgs e)
        {

            SaveMethed();
            Savemode = true;
        }
        #endregion

        #region 保存方法
        //保存时调用方法
        private void SaveMethed()
        {
            if (ControlTacticName.Text == "SpecialControl")
            {
                if (ModuleName.Text == "FUZZY")
                { Basic[7] = "SpecialControl.Fuzzy"; }
                else if (ModuleName.Text == "JIEOU")
                { Basic[7] = "SpecialControl.JieOu"; }
                else if (ModuleName.Text == "CALCU")
                { Basic[7] = "SpecialControl.Calculator"; }
                else if (ModuleName.Text == "PROCESS")
                { Basic[7] = "SpecialControl.Process"; }                
            }

            Basic[0] = DisplayName.Text;

            Basic[2] = ModuleSort.Text;

            if (MColorChange == true && ModuleColorChange.Enabled == true)
            {
                Basic[1] = "255," + ModuleColorRGB[1].ToString() + "," + ModuleColorRGB[2].ToString() + "," + ModuleColorRGB[3].ToString();
            }
            else
            { Basic[1] = ""; }

            if (ModuleName.ReadOnly == false)
            { Basic[3] = ModuleName.Text; }
            else
            { Basic[3] = ""; }

            if (TColorChange == true && TextColorChange.Enabled == true)//本无色改成有色出现问题
            {
                Basic[4] = "255," + TextColorRGB[1].ToString() + "," + TextColorRGB[2].ToString() + "," + TextColorRGB[3].ToString();
            }
            else
            { Basic[4] = ""; }

            if (OutputName.ReadOnly == false)
            { Basic[5] = OutputName.Text; }
            else
            { Basic[5] = ""; }

            if (InputName.ReadOnly == false)
            {
                if (InputInfo != null)
                { Basic[6] = InputInfo; }
                else
                {
                    Basic[6] = InputName.Text;
                }
            }
            else
            { Basic[6] = ""; }

            //清空function 然后赋值
            string[,] temp = new string[dataGridView1.RowCount - 1, dataGridView1.ColumnCount];
            function = temp;

            index2 = 0;

            for (int row = 0; row < dataGridView1.RowCount - 1; row++)
            {
                for (int colum = 0; colum < dataGridView1.ColumnCount; colum++)
                {
                    if (dataGridView1.Rows[row].Cells[colum].Value != null)
                    {
                        function[row, colum] = dataGridView1.Rows[row].Cells[colum].Value.ToString();
                    }
                    index2++;
                }
            }
            index2 /= dataGridView1.ColumnCount;//循环中的总单元格数量转换成行数，即功能属性数目

            //保存指令
            code = this.OtherInfo;
            Savemode = true;

        }
        #endregion

        #region 确认保存键
        private void OK_Click(object sender, EventArgs e)
        {
            if (Savemode == false || DisplayName.Text != Basic[0] || ModuleSort.Text != Basic[2] || ModuleName.Text != Basic[3] || OutputName.Text != Basic[5] || InputName.Text != Basic[6])
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                DialogResult result;

                //修改和添加的区别语言
                result = MessageBox.Show("是否保存对属性的修改?", "没有保存!", buttons);

                if (result == DialogResult.Yes)
                {
                    SaveMethed();
                }
            }
            if (DisplayName.Text == "")
            {
                MessageBox.Show("类别栏为空，添加失败！");
            }

        }
        #endregion

        #endregion

        #region 功能属性

        #region 功能属性添加键
        //添加功能属性
        private void AddFunction_Click(object sender, EventArgs e)
        {

            Form3 frm = new Form3( "添加功能属性");

            while (true)
            {
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    DataGridViewRowCollection rows = this.dataGridView1.Rows;

                    rows.Add(frm.Attribute[0], frm.Attribute[1], frm.Attribute[2],frm.Attribute[3],frm.Attribute[4]);
                    index2++;

                    Savemode = false;
                }

                break;
            }
            if (index2 != 0)
            {
                ModifyFunction.Enabled = true;
                DeleteFunction.Enabled = true;
            }

        }
        #endregion

        #region 功能属性删除键
        //删除功能属性
        private void DeleteFunction_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRowCollection rows = this.dataGridView1.Rows;
                rows.RemoveAt(dataGridView1.CurrentRow.Index);
                index2--;
                Savemode = false;
                if (index2 == 0)
                {
                    ModifyFunction.Enabled = false;
                    DeleteFunction.Enabled = false;
                }
            }
            catch
            {
                MessageBox.Show("请选择要删除的属性！");
            }
        }
        #endregion

        #region 功能属性设置键
        //修改功能属性
        private void ModifyFunction_Click(object sender, EventArgs e)
        {
            Savemode = false;

            Form3 frm = new Form3("设置功能属性");
            frm.Function[0] = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            frm.Function[1] = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            frm.Function[2] = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            frm.Function[3] = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            frm.Function[4] = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            frm.Attribute = frm.Function;

            while (true)
            {
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    dataGridView1.CurrentRow.SetValues(frm.Attribute);
                    Savemode = false;
                }
                break;
            }
        }
        #endregion

        #region 功能属性修改事件
        //当DGV中属性修改后触发的事件
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Savemode = false;
        }
        #endregion

        #endregion

        #region 预览

        #region 预览键
        private void Preview_Click(object sender, EventArgs e)
        {
            if (Savemode == false)
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show("是否保存对属性的修改?", "没有保存!", buttons);

                if (result == DialogResult.Yes)
                {
                    SaveMethed();
                    PreviewControler();
                }
            }
            else
            { PreviewControler(); }
        }
        #endregion

        #region 预览方法
        //预览功能
        private void PreviewControler()
        {
            float X = this.panel1.Size.Width ;
            X = X / 373;
            float Y = this.panel1.Size.Height ;
            Y = Y / 245;
            X =Math.Max(1F, Math.Min(2F, Math.Min(X, Y)));
            panel1.Controls.Clear();
            Point XY = new Point(this.panel1.Size.Width / 5 - 10, this.panel1.Size.Height / 6);//定位
            Point YX = new Point(this.panel1.Size.Width / 4 - 40, this.panel1.Size.Height / 3 - 15);

            if (Basic[7] == "Modules")
            {
                Modules controler = new Modules();
                controler.Location = XY;
                SetVelue(controler);
                controler.Scaling = X;
                panel1.Controls.Add(controler);
            }
            else if (Basic[7] == "Arithmetic")
            {
                //ControlTactic.SpecialControl.Fuzzy fuzzy = new ControlTactic.SpecialControl.Fuzzy();
                Arithmetic controler = new Arithmetic();
                controler.Location = XY;
                SetVelue(controler);
                controler.Scaling = X;
                panel1.Controls.Add(controler);
            }
            else if (Basic[7] == "OutputValue")
            {
                OutputValue controler = new OutputValue();
                controler.Location = YX;
                SetVelue(controler);
                controler.Scaling =  X;
                panel1.Controls.Add(controler);
            }
            else if (Basic[7] == "InputValue")
            {
                InputValue controler = new InputValue();
                controler.Location = YX;
                SetVelue(controler);
                controler.Scaling =  X;
                panel1.Controls.Add(controler);
            }
            else if (Basic[7] == "Constant")
            {
                Constant controler = new Constant();
                controler.Location = YX;
                SetVelue(controler);
                controler.Scaling = X;
                panel1.Controls.Add(controler);
            }
            else if (Basic[7] == "SystemConstant")
            {
                SystemConstant controler = new SystemConstant();
                controler.Location = YX;
                SetVelue(controler);
                controler.Scaling =  X;
                panel1.Controls.Add(controler);
            }
            else if (Basic[7] == "Lable")
            {
                ControlTactic.Label controler = new ControlTactic.Label();
                controler.Location = YX;
                SetVelue(controler);
                controler.Scaling = 1.8F * X;
                panel1.Controls.Add(controler);
            }
            else if (Basic[7] == "JMP")
            {
                ControlTactic.JMP controler = new ControlTactic.JMP();
                controler.Location = YX;
                SetVelue(controler);
                controler.Scaling =  X;
                panel1.Controls.Add(controler);
            }
        }
        #endregion

        #region 控键属性输入方法
        //将属性输入控件
        private void SetVelue(object controler)
        {
            object obj = null;
            if (Basic[1] != "")
            {//将文本类型的属性转换成适合输入的类型
                PropertyDescriptor PDor = TypeDescriptor.GetProperties(controler)["ModuleColor"];
                obj = PDor.Converter.ConvertFromInvariantString(Basic[1]);
                PDor.SetValue(controler, obj);
            }
            if (Basic[2] != "")
            {
                PropertyDescriptor PDor = TypeDescriptor.GetProperties(controler)["ModuleSort"];
                obj = PDor.Converter.ConvertFromInvariantString(Basic[2]);
                PDor.SetValue(controler, obj);
            }
            if (Basic[3] != "")
            {
                PropertyDescriptor PDor = TypeDescriptor.GetProperties(controler)["ModuleName"];
                obj = PDor.Converter.ConvertFromInvariantString(Basic[3]);
                PDor.SetValue(controler, obj);
                PropertyDescriptor PDor2 = TypeDescriptor.GetProperties(controler)["PortName"];
                if (PDor2 != null)
                { PDor2.SetValue(controler, PDor2.Converter.ConvertFromInvariantString(Basic[3] + "0")); }
            }
            if (Basic[4] != "")
            {
                PropertyDescriptor PDor = TypeDescriptor.GetProperties(controler)["TextColor"];
                obj = PDor.Converter.ConvertFromInvariantString(Basic[4]);
                PDor.SetValue(controler, obj);
            }
            if (Basic[5] != "")
            {
                PropertyDescriptor PDor = TypeDescriptor.GetProperties(controler)["OutputName"];
                obj = PDor.Converter.ConvertFromInvariantString(Basic[5]);
                PDor.SetValue(controler, obj);
            }
            if (Basic[6] != "")
            {
                PropertyDescriptor PDor = TypeDescriptor.GetProperties(controler)["InputName"];
                //string[] temp1 = Basic[6].Split(';');
                //string temp2 = null;
                obj = PDor.Converter.ConvertFromInvariantString(Basic[6]);
                PDor.SetValue(controler, obj);
            }
        }
        #endregion

        #endregion

        #region 颜色处理

        #region 文本颜色修改键
        //修改文本颜色
        private void TextColorChange_Click(object sender, EventArgs e)
        {
            Savemode = false;
            TColorChange = true;

            colorDialog1.Color = TextOfColor.BackColor;
            colorDialog1.ShowDialog();
            TextOfColor.BackColor = colorDialog1.Color;
            TextColorRGB[1] = colorDialog1.Color.R;
            TextColorRGB[2] = colorDialog1.Color.G;
            TextColorRGB[3] = colorDialog1.Color.B;
        }
        #endregion

        #region 模块颜色修改键
        //修改模块颜色
        private void ModuleColorChange_Click(object sender, EventArgs e)
        {
            Savemode = false;
            MColorChange = true;

            colorDialog1.Color = ModuleOfColor.BackColor;
            colorDialog1.ShowDialog();
            ModuleOfColor.BackColor = colorDialog1.Color;
            ModuleColorRGB[1] = colorDialog1.Color.R;
            ModuleColorRGB[2] = colorDialog1.Color.G;
            ModuleColorRGB[3] = colorDialog1.Color.B;
        }
        #endregion

        #region STR到RGB转化方法
        //将字符串型的RGB转换成颜色属性///可修改优化
        private int[] StrToRGB(string str)
        {
            int numstr = 0;
            int number = 0;
            int[] ColorRGB = new int[4];

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] <= '9' & str[i] >= '0')
                { numstr = numstr * 10 + str[i] - '0'; }
                else if (str[i] == ',')
                {
                    ColorRGB[number++] = numstr;

                    numstr = 0;
                }
            }
            ColorRGB[number] = numstr;
            return ColorRGB;
        }
               #endregion

        #endregion

        #region Combobox属性变动事件
        //combobox变动属性事件，对应controltactic类型进行变动
        private void ControlTactic_SelectedValueChanged(object sender, EventArgs e)
        {
            if (FirstTime != true)
            { Savemode = false;  }

            ModuleName.ReadOnly = false;
            InputName.ReadOnly = false;
            SetValue.Enabled = true;
            OutputName.ReadOnly = false;
            ModuleColorChange.Enabled = true;
            TextColorChange.Enabled = true;
            if (ControlTacticName.Text == "OutputValue" || ControlTacticName.Text == "InputValue")
            {
                InputName.ReadOnly = true;
                SetValue.Enabled = false;
                OutputName.ReadOnly = true;
                TextColorChange.Enabled = false;
            }
            else if (ControlTacticName.Text == "SystemConstant" || ControlTacticName.Text == "Constant")
            {
                ModuleName.ReadOnly = true;
                InputName.ReadOnly = true;
                SetValue.Enabled = false;
                OutputName.ReadOnly = true;
                TextColorChange.Enabled = false;
            }
            else if (ControlTacticName.Text == "Label" || ControlTacticName.Text == "JMP")
            {
                ModuleName.ReadOnly = true;
                InputName.ReadOnly = true;
                SetValue.Enabled = false;
                OutputName.ReadOnly = true;
                ModuleColorChange.Enabled = false;
                TextColorChange.Enabled = false;
            }
        }
        #endregion

        #region 控件代码


        private void CodeList_TextChanged(object sender, EventArgs e)
        {
            this.OtherInfo[this.infoName.SelectedIndex] = this.infoText.Text;
            Savemode = false;
        }
        #endregion

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            Point newpoint = new Point();
            Size newsize = new Size();
            newpoint.X = Math.Max(this.Size.Width - 124 - 94 * 3, 140);
            newpoint.Y = Math.Max(this.Size.Height - 91, 379);
            this.Save.Location = newpoint;
            newpoint.X = Math.Max(this.Size.Width - 124 - 94 * 2, 234);
            newpoint.Y = Math.Max(this.Size.Height - 91, 379);
            this.Return.Location = newpoint;
            newpoint.X = Math.Max(this.Size.Width - 124 - 94, 328);
            newpoint.Y = Math.Max(this.Size.Height - 91, 379);
            this.OK.Location = newpoint;
            newpoint.X = Math.Max(this.Size.Width - 124, 422);
            newpoint.Y = Math.Max(this.Size.Height - 91, 379);
            this.Cencel.Location = newpoint;

            newsize.Width = this.Size.Width;
            newsize.Height = Math.Max(355, this.Size.Height - 115);
            this.tabControl1.Size = newsize;

            newsize.Width = Math.Min(this.Size.Width - 140, 1122);
            newsize.Height = Math.Max(this.tabControl1.Size.Height - 85, 270);
            this.dataGridView1.Size = newsize;

            newpoint.X = Math.Min(this.dataGridView1.Size.Width + 34, 1156);
            newpoint.Y = 26;
            this.AddFunction.Location = newpoint;
            newpoint.X = this.AddFunction.Location.X;
            newpoint.Y = 66;
            this.DeleteFunction.Location = newpoint;
            newpoint.X = this.AddFunction.Location.X;
            newpoint.Y = 106;
            this.ModifyFunction.Location = newpoint;

            newpoint.X = this.tabControl1.Size.Width - 120;
            newpoint.Y = this.tabControl1.Size.Height - 60;
            this.Preview.Location = newpoint;
            newsize.Width = this.tabControl1.Size.Width - 65;
            newsize.Height = this.tabControl1.Size.Height - 110;
            this.panel1.Size = newsize;

            newpoint.X = this.Preview.Location.X;
            newpoint.Y = this.tabControl1.Size.Height - 70;
            newsize.Width = this.tabControl1.Size.Width - 85;
            newsize.Height = this.tabControl1.Size.Height - 120;
            this.infoText.Size = newsize;

            PreviewControler();

        }

        private void SetValue_Click(object sender, EventArgs e)
        {
            Form5 newForm = new Form5(this.InputInfo);
            if (newForm.ShowDialog() == DialogResult.OK)
            {
                this.InputInfo = newForm.Basic;
            }


        }

        private void infoName_SelectedIndexChanged(object sender, EventArgs e)
        {
            infoText.Text = OtherInfo[infoName.SelectedIndex];
        }


    }
}