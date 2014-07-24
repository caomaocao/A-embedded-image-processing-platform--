using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace text3
{
    public partial class Form3 : Form
    {
        public Form3(string FromName)
        {
            InitializeComponent();
            this.Name = FromName;
        }

        public string[] Function = new string[5];

        public string[] Attribute
        {
            get
            {
                Function[0] = textBox1.Text;
                Function[1] = textBox2.Text;
                Function[2] = comboBox1.Text;
                Function[3] = textBox3.Text;
                Function[4] = comboBox2.Text;
                return this.Function;
            }
            set
            {
                textBox1.Text = Function[0];
                textBox2.Text = Function[1];
                comboBox1.Text = Function[2];
                textBox3.Text = Function[3];
                comboBox2.Text = Function[4];
            }
        }


    }
}