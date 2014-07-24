using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using CaVeGen.CommonOperation;

namespace CaVeGen
{
    public partial class NewPorject : Form
    {
        public string Pinfo = "ProjectInfo";
        public string Pname = "newProject";
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count">新工程序号</param>
        public NewPorject(string count)
        {
            InitializeComponent();
            this.ProjectName.Text = "NEWPROJECT" + count;
            this.Text = "新建工程";
        }

        /// <summary>
        /// 修改工程时所用
        /// </summary>
        /// <param name="proName"></param>
        /// <param name="proInfo"></param>
        public NewPorject(string proName,string proInfo)
        {
            InitializeComponent();
            this.Text = "修改工程";
            this.ProjectName.Text = proName;
            this.ProjectInfo.Text = proInfo;
            this.Pname = proName;
            this.Pinfo = proInfo;
        }

        //判断输入值是否有问题
        private void TimeValue_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (ProjectInfo.Text != "")
                {
                    Convert.ToInt32(ProjectInfo.Text);
                }
            }
            catch
            {
                MessageBox.Show("有问题!");
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            CloseForm();            
        }



        /// <summary>
        /// 关闭窗口检验填写信息函数
        /// </summary>
        private void CloseForm()
        {
            bool Pass1 = CassViewGenerator.JudgeName(ProjectName.Text, "工程文件名") && judgeProjectName(ProjectName.Text);
            bool Pass2 = Directory.Exists(CassViewGenerator.programPath + "\\" + ProjectName.Text);

            if (Pass1 == false)
            {
                //this.DialogResult = DialogResult.Cancel;
                //this.Dispose();        //释放资源       
            }
            else if (Pass2 == true)
            {
                DialogResult result = new DialogResult();

                if (this.Text == "新建工程")
                {
                    result = CassMessageBox.Question("存在相同的工程，是否删除原有的工程？");
                    if (result == DialogResult.Yes)
                    {
                        Directory.Delete(CassViewGenerator.programPath + "\\" + ProjectName.Text, true);//删除原有工程
                        this.Pname = ProjectName.Text;
                        this.Pinfo = ProjectInfo.Text;
                        this.DialogResult = DialogResult.OK;
                        this.Dispose();        //释放资源   
                    }
                }
                else if (this.Text == "修改工程")
                {                 
                    if (this.Pname == ProjectName.Text)
                    {//工程名相同 为修改信息
                        this.Pinfo = ProjectInfo.Text;
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {//与其他工程冲突
                        CassMessageBox.Warning("存在相同的工程，修改工程名失败！");
                        this.DialogResult = DialogResult.Cancel;
                    }      
                    this.Dispose();        //释放资源                     
                }
            }
            //else if (result == DialogResult.No)
            //{ }

            else
            {
                this.Pname = ProjectName.Text;
                this.Pinfo = ProjectInfo.Text;
                this.DialogResult = DialogResult.OK;
                this.Dispose();        //释放资源        
            }
        }

        private void ProjectName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {//回车
                CloseForm();
            }
        }

        private void TimeValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {//回车
                CloseForm();
            }
        }

        /// <summary>
        /// 判断工程名书写规范
        /// 必须以大写字母开头只能包括字母数字和下划线
        /// </summary>
        /// <param name="projectname"></param>
        /// <returns></returns>
        private bool judgeProjectName(string projectName)
        {
            char[] tempArray = projectName.ToCharArray();
            if (tempArray.Length > 0)
            {
                string newName = null;
                for (int i = 0; i < tempArray.Length; i++)
                {
                   
                    if (tempArray[i] >= 'a' && tempArray[i] <= 'z')
                    {
                        tempArray[i] = (char)(tempArray[i] - 'a' + 'A');//从小写到大些相差32阿斯克马值
                    }
                    else  if (!((tempArray[i] >= 'A' && tempArray[i] <= 'Z')
                        || (tempArray[i] >= '0' && tempArray[i] <= '9')
                        || tempArray[i] == '_'))
                    {
                        CassMessageBox.Information("工程名中不能含有字母数字下划线以外的符号！");
                        return false;
                    }
                    newName += tempArray[i];
                }
                ProjectName.Text = newName;
                return true;
            }
            return false;
        }






    }
}