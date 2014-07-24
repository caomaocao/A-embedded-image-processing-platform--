using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace text3
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private List<string[]> tempCode = new List<string[]>();
        private string RichBoxTextList = null;

        public void readcode(List<string[]> codelist)
        {
            if (codelist.Count != 0)
            {
                tempCode = codelist;
                foreach (string[] element in tempCode)
                {
                    this.CodeList.Items.Add(element[0]);
                }
            }
            selectCode(0);
        }

        public List<string[]> writecode()
        {
            return tempCode;
        }



        private void selectCode(int index)
        {
            CodeList.SelectedIndex = index;
            VarName.Text = tempCode[index][1];
            Codevalue.Text = tempCode[index][2];
        }

        private void CodeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectCode(this.CodeList.SelectedIndex);
        }

        private void AddCode_Click(object sender, EventArgs e)
        {
            string[] Addcodeinfo = new string[3];
            Addcodeinfo[0] = CodeList.Text;
            Addcodeinfo[1] = VarName.Text;
            Addcodeinfo[2] = Codevalue.Text;
            this.tempCode.Add(Addcodeinfo);
        }

        private void DelCode_Click(object sender, EventArgs e)
        {
            int selectIdex = this.CodeList.SelectedIndex;

            this.tempCode.RemoveAt(selectIdex);
            this.CodeList.Items.RemoveAt(selectIdex);
            if (tempCode.Count != 0)
            {
                selectCode(0);
            }
            else
            {
                CodeList.Text = null;
                VarName.Text = null;
                Codevalue.Text = null;
            }
        }

        private void Codevalue_TextChanged(object sender, EventArgs e)
        {
            if (CodeList.SelectedIndex != -1)
            {
                tempCode[CodeList.SelectedIndex][2] = Codevalue.Text;
            }
        }

    }
}