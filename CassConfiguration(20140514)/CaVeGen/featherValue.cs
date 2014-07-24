using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CassGraphicsSystem.Project
{
    public partial class featherValue : Form
    {
        public featherValue()
        {
            InitializeComponent();
        }
        public void setTextbox(string i)
        {
            textBox1.Text = i;
            featherNum = i;
        }
        public int textbox
        {
            get
            {
                return int.Parse( featherNum);
            }
        }
        string featherNum;
        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            featherNum = textBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
