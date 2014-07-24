using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CaVeGen.CommonOperation;

namespace CaVeGen.ProjectForm
{
    public partial class ResetWorkSpaceForm : Form
    {
        //1.声明委托
        public delegate void workSpaceMsg(string msg);
        //2.定义委托
        public workSpaceMsg dele_workSpace;
        
        private string _currentWorkSpace = null;
        public ResetWorkSpaceForm(string currentWorkSpace)
        {
            InitializeComponent();
            this.text_workSpace.Text = currentWorkSpace;
            this._currentWorkSpace = currentWorkSpace;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            
            this.Dispose();
            this.Close();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            //3.调用委托，将参数传给主界面中的programPath
            dele_workSpace(this._currentWorkSpace);           
            this.Close();
            //提示完成切换
            CassMessageBox.Information("工作目录切换成功！"); 

        }

        private void btn_OpenFolder_Click(object sender, EventArgs e)
        {
            this.myFolderBrowserDialog.Description = "工作目录选择";
            if (this._currentWorkSpace == null)
            {
                //默认在桌面
                this.myFolderBrowserDialog.SelectedPath = System.Environment.SpecialFolder.Desktop.ToString();
            }
            else
            {
                this.myFolderBrowserDialog.SelectedPath = this._currentWorkSpace;
            }
            
            DialogResult result = this.myFolderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this._currentWorkSpace = this.myFolderBrowserDialog.SelectedPath;
                this.text_workSpace.Text = this._currentWorkSpace;
            }
            else if (result == DialogResult.Cancel)
            {
                
            }

           
                       
            
        }
    }
}
