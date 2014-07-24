using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace text3
{
    public partial class Form5 : Form
    {
        public Form5(string Info)
        {
            InitializeComponent();

            //拆解出所需信息
            string[] NameInfo = Info.Split(';');
            if (NameInfo.Length > 1)
            {
                valueList = new string[NameInfo.Length];
                for (int i = 0; i < NameInfo.Length; i++)
                {
                    NameBox.Items.Add(NameInfo[i].Split(',')[0]);
                    valueList[i] = NameInfo[i].Split(',')[1];
                }
            }
            else
            {
                NameBox.Items.AddRange(Info.Split(','));
                valueList = new string[Info.Split(',').Length];
            }
            NameBox.SelectedIndex = 0;
            TypeBox.SelectedIndex = 0;
        }

        private string[] valueList;
        public string Basic = null;



        private void NameBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NameBox.SelectedIndex != -1)
            {
                if (valueList[NameBox.SelectedIndex] != null)
                {
                    ValueBox.Text = valueList[NameBox.SelectedIndex];
                }
                else
                { ValueBox.Text = "0"; }
            }
        }

        private void ValueBox_TextChanged(object sender, EventArgs e)
        {
            if (NameBox.SelectedIndex != -1)
            { valueList[NameBox.SelectedIndex] = ValueBox.Text; }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            List<string> tempInfo = new List<string>();
            for (int i = 0; i < NameBox.Items.Count; i++)
            {
                tempInfo.Add(NameBox.Items[i].ToString() + "," + valueList[i]);
            }
            Basic = String.Join(";", tempInfo.ToArray());
        }
    }
}