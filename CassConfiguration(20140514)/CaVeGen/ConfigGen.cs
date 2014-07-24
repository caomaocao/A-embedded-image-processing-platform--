using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using CaVeGen.CommonOperation;

namespace CassGraphicsSystem.Project
{
    public partial class ConfigGen : Form
    {
        string SavePath;  //用户的工程目录
        string Programpath;  //系统debug目录
        public ConfigGen(string _savePath,string _programPath)
        {
            SavePath = _savePath;
            Programpath = _programPath;  
            InitializeComponent();
        }
       
        System.Timers.Timer timer;
        public void setTextbox(string i)
        {
            textBox1.Text = i;
        }
      
        public int textbox
        {
            get
            {
                return int.Parse( featherNum);
            }
        }
        string featherNum;

        private void saveDatabutton_Click(object sender, EventArgs e)
        {
          //  string currentSavePath = StringOperateHelp.LeftOfRightmostOf(SavePath, '\\') + "\\Debug";
            if (File.Exists(SavePath + PublicVariable.GenDataConfigFileName))
            {
                File.Delete(SavePath + PublicVariable.GenDataConfigFileName);
            }

            FileStream fs = new FileStream(SavePath + PublicVariable.GenDataConfigFileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string data = "15";//textBox2.Text+","+textBox1.Text+","+textBox3.Text+","+textBox4.Text;
            sw.Write(data);
            //关闭流
            sw.Close();
            fs.Close();
        }
        System.Diagnostics.Process p;
        private void Actbutton_Click(object sender, EventArgs e)
        {
            try
            {
               // string DebugPath=StringOperateHelp.LeftOfRightmostOf(SavePath,'\\');
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(Programpath +PublicVariable.FeatherPointsExePath);
                info.UseShellExecute = false;
                info.WorkingDirectory = SavePath;
                p = System.Diagnostics.Process.Start(info);
                p.Exited += new EventHandler(ProcessExit);
                timer = new System.Timers.Timer(1000);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(ActProcess);
                timer.Start();
               
                
            }
            catch (Exception ex)
            {
                

            }

        }
        public  string[] data;
        private void ProcessExit(object sender, EventArgs e)
        {
            timer.Stop();

            //progressBar.
 
        }
        private void ActProcess(object source,System.Timers.ElapsedEventArgs e)
        {
            //if(progressBar)
           //  progressBar.PerformStep();
        }
        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            try
            {
                p.Close();
                this.DialogResult = DialogResult.OK;
                this.Close();
                
            }
            catch (Exception ex)
            {
 
            }
        }

        private void ConfigGen_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }


       //private void button1_Click(object sender, EventArgs e)
        //{
            //System.Diagnostics.Process p;
            //System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("HeaterControler.exe");
            //p = System.Diagnostics.Process.Start(info);

            //this.DialogResult = DialogResult.OK;
            //featherNum = textBox1.Text;
            //this.Close();
      //  }

      //  private void button2_Click(object sender, EventArgs e)
       // {
            //this.Close();
      //  }


    }
}
