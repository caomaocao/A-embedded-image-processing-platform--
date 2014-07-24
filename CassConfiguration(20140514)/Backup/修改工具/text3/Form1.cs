using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Data.SqlClient;
using System.IO;
using CaVeGen;
using CaVeGen.DesignViewFiles;

namespace text3
{
    public partial class Form1 : Form
    {
        public Form1()
        {           
            InitializeComponent();
        }

        public static string ToolxmlPath = null;

        public void StartForm(string XMLPath)
        {
            Form1.ToolxmlPath = XMLPath;
            CreateTree();
        }
        
        #region 修改

        private void Modify_Click(object sender, EventArgs e)
        {            
            try
            {
                string selectNode = treeView1.SelectedNode.Text;
                NewXmlNode newnode = new NewXmlNode(selectNode);
               string parentNode = treeView1.SelectedNode.Parent.Text;
               Form2 frm = new Form2(newnode.Type);
                if (newnode.BasicAttribute[0] != "")
                {
                    frm.Basic = newnode.BasicAttribute;
                    frm.function = newnode.FunctionAttribute;
                    frm.code = newnode.CodeAttribute;
                    frm.index2 = newnode.index;
                    frm.modify();
                    frm.ShowDialog();                   
                    
                    if (frm.DialogResult == DialogResult.OK && frm.Savemode == true)
                    {
                        newnode.BasicAttribute = frm.Basic;
                        newnode.FunctionAttribute = frm.function;
                        newnode.CodeAttribute = frm.code;
                        newnode.index = frm.index2;
                        newnode.ModifyNode();
                        CreateTree();

                        foreach (TreeNode node in treeView1.Nodes)
                        {
                            if (node.Text == parentNode)
                            {
                                node.Expand();                               
                                break;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择一个模块种类！");
                }
            }
            catch
            {
                MessageBox.Show("请选择一个模块种类！");
            }          
        }

        #endregion
        
        #region 添加

        private void Add_Click(object sender, EventArgs e)
        {
            
            Form2 frm = new Form2(null);
            frm.AddMode();
            frm.ShowDialog();

            if (frm.DialogResult == DialogResult.OK)
            {
                if (frm.Basic[0] != "")
                {
                    NewXmlNode newnode = new NewXmlNode();

                    newnode.AddNode(frm.Basic, frm.function, frm.index2, frm.code);
            
                    CreateTree();

                    //添加的节电展开？
                }
            }

        }


        #endregion
        
        #region 删除

        private void Delete_Click(object sender, EventArgs e)
        {
            string temp = "";

            try
            {
                temp = treeView1.SelectedNode.Text;

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                DialogResult result;


                result = MessageBox.Show("确定要删除所选的节点吗？", "删除节点！", buttons);

                if (result == DialogResult.Yes)
                {
                    NewXmlNode newnode = new NewXmlNode(temp);

                    newnode.DeleteNode();

                    CreateTree();
                }
                
               
            }
            catch
            {
                MessageBox.Show("没有选择需要删除的目标！");
            }
        }


        #endregion
        
        #region 刷新树
        //在TREEVIEW中从新读取XML文件来创建树

        private  void CreateTree()
        {
            treeView1.Nodes.Clear();//清空TREEVIEW

            XmlDocument CurrentDoc = new XmlDocument();
            CurrentDoc.Load(Form1.ToolxmlPath);

            foreach (XmlNode node in CurrentDoc.FirstChild.ChildNodes)
            {
                if (node.Attributes[0].InnerText != "特殊代码")
                {
                    TreeNode root1 = new TreeNode();
                    root1.Text = node.Attributes[0].Value;
                    treeView1.Nodes.Add(root1);
                    foreach (XmlNode childnode in node.ChildNodes)
                    {

                        TreeNode child = new TreeNode();                        
                        child.Text = childnode.Attributes[0].Value.Split(new char[] { ',' })[2];
                        root1.Nodes.Add(child);
                    }
                }
            }
        }

        #endregion
        
        #region 树节点点击修改
        ///<summary>
        /// 当树中属性被修改后触发的事件
        /// 询问、修改、传值、保存
        ///</summary>     
       
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
          
            string temp = treeView1.SelectedNode.Text;
            try
            {
                if (e.Label.Length > 0)
                {
                    if (e.Label.IndexOfAny(new char[] { '<', '.', ',', '"', '>' }) == -1)
                    {

                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;

                        DialogResult result;

                        //修改和添加的区别语言
                        result = MessageBox.Show("是否保存将节点名改成\n\t"+e.Label+"?", "节点名被修改！", buttons);

                        if (result == DialogResult.Yes)
                        {
                            NewXmlNode method = new NewXmlNode();
                            method.ChangeValue(temp, e.Label);
                            label2.Text = "";
                            label3.Text = "";
                            label4.Text = "";
                            label5.Text = "";
                        }
                        else
                        { e.CancelEdit = true; }

                    }
                    else
                    {
                        e.CancelEdit = true;
                        MessageBox.Show("名称中不能有: '<', '.', ',', ''' ,'>'");
                    }
                }
                else
                {
                    e.CancelEdit = true;
                    MessageBox.Show("名称不能为空！");
                }
         
            }
            catch
            {
                treeView1.SelectedNode.Text = temp;
            }

        }

        #endregion
                
        #region 树节点属性显示
        ///<summary>
        /// 当树中节点点击后显示属性
        ///</summary>  

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //label2.Text = "";
            //label3.Text = "";
            //label4.Text = "";
            NewXmlNode newnode = new NewXmlNode(treeView1.SelectedNode.Text);
            string[] attribute = newnode.BasicAttribute;

            if (attribute[1] != "")
            {
                attribute[1] = "  模块颜色:" + attribute[1];
            }
            if (attribute[2] != "")
            {
                attribute[2] = "  模块类别:" + attribute[2];
            }
            if (attribute[7] != "")
            {
                attribute[7] = "控制模块:" + attribute[7];
            }
            if (attribute[3] != "")
            {
                attribute[3] = "模块名称:" + attribute[3];
            }
            if (attribute[4] != "")
            {
                attribute[4] = " 文本颜色:" + attribute[4];
            }
            if (attribute[5] != "")
            {
                attribute[5] = "输出名称:" + attribute[5];
            }
            if (attribute[6] != "")
            {
                attribute[6] = " 输入名称:" + attribute[6].Split(':')[0];
            }
            label2.Text = "类型:" + attribute[0] + attribute[2];
            label3.Text = attribute[7] + attribute[1];
            label4.Text = attribute[3] + attribute[4];
            label5.Text = attribute[5] + attribute[6];
        }

        private void headcode_Click(object sender, EventArgs e)
        {
            Form4 newform = new Form4();
            NewXmlNode HeadMethod = new NewXmlNode();
            newform.readcode(HeadMethod.ReadHead());


            newform.ShowDialog();
            if (newform.DialogResult == DialogResult.OK)
            {
                HeadMethod.WriteHead(newform.writecode());             
            }
        }



        #endregion

        private void Close_Click(object sender, EventArgs e)
        {
            this.Dispose();        //释放资源     
        }

        private void Update_Click(object sender, EventArgs e)
        {
            try
            {
                ToolBoxServiceImpl.toolXML.Load(Form1.ToolxmlPath);
                CassViewGenerator.UpdateCurrentPro();
            }
            catch
            {
                MessageBox.Show("更新失败！");
            }
        }

    }
}