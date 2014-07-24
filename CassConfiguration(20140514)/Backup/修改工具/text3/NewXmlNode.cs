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

namespace text3
{
    class NewXmlNode
    {


        public string[] BasicAttribute = new string[8];
        public string[,] FunctionAttribute = new string[100, 10];
        public string[] CodeAttribute = new string[3];
        public int index = 0;
        public string CurrentName;
        public string Type = null;
       
        private XmlDocument CurrentDoc = new XmlDocument();

        #region 构造函数
        //创建对象时传入节点的TEXT处理，找到其属性
        public NewXmlNode(string NodeName)
        {
            for (int i = 0; i < BasicAttribute.Length; i++)
            {
                BasicAttribute[i] = "";
            }
            CurrentName = NodeName;
            FindAttribute();
        }

        //调用方法用的构造函数
        public NewXmlNode()
        {

        }


        #endregion

        #region 获取属性方法

        ///<summary>
        ///根据节点的TEXT在XML中找到，并获取节点的子属性
        ///</summary>
        private void FindAttribute()
        {
            CurrentDoc.Load(Form1.ToolxmlPath);
            bool FindNode = false;

            foreach (XmlNode node in CurrentDoc.FirstChild.ChildNodes)
            {
                BasicAttribute[0] = node.Attributes[0].Value;

                foreach (XmlNode child in node.ChildNodes)
                {//CurrentName是子节点，附值属性
                    if (child.Attributes[0].Value.Split( ',' )[2] == CurrentName)
                    {
                        this.Type = child.Attributes[0].Value.Split(',', '.')[1];
                        BasicAttribute[7] = child.Attributes[0].Value.Split(new char[] { ',', '.' })[1];

                        foreach (XmlNode attribute in child.ChildNodes)
                        {
                            if (attribute.Name == "BasicProperty")
                            {
                                foreach (XmlNode basic in attribute.ChildNodes)
                                {
                                    if (basic.Attributes[0].Value == "ModuleColor")
                                        BasicAttribute[1] = basic.InnerText;
                                    if (basic.Attributes[0].Value == "ModuleSort")
                                        BasicAttribute[2] = basic.InnerText;
                                    if (basic.Attributes[0].Value == "ModuleName")
                                        BasicAttribute[3] = basic.InnerText;
                                    if (basic.Attributes[0].Value == "TextColor")
                                        BasicAttribute[4] = basic.InnerText;
                                    if (basic.Attributes[0].Value == "OutputName")
                                        BasicAttribute[5] = basic.InnerText;
                                    if (basic.Attributes[0].Value == "InputName")
                                    {
                                        if (basic.ChildNodes.Count > 1)
                                        {//读取输入子节点端口初值信息节点20090616
                                            List<string> Infos = new List<string>();
                                            foreach (XmlNode InputInfo in basic.ChildNodes)
                                            {
                                                string tempInfo = InputInfo.Attributes[0].Value + "," + InputInfo.InnerText;//输入口名加初值
                                                Infos.Add(tempInfo);
                                            }
                                            BasicAttribute[6] += String.Join(";", Infos.ToArray());
                                        }
                                        else
                                        { BasicAttribute[6] = basic.InnerText; }
                                    }
                                }
                            }
                            else if (attribute.Name == "FunctionProperty" && attribute.HasChildNodes)
                            {
                                foreach (XmlNode function in attribute.ChildNodes)
                                {
                                    FunctionAttribute[index, 0] = function.Attributes[0].Value;//name
                                    FunctionAttribute[index, 1] = function.Attributes[1].Value;//varname
                                    FunctionAttribute[index, 2] = function.Attributes[2].Value;//type
                                    FunctionAttribute[index, 4] = function.Attributes[3].Value;//visible
                                    if (function.Attributes.Count >= 5)
                                    {
                                        FunctionAttribute[index, 5] = function.Attributes[4].Value;//exp
                                        if (function.Attributes.Count == 9)
                                        {
                                            FunctionAttribute[index, 6] = function.Attributes[5].Value;//optype
                                            FunctionAttribute[index, 7] = function.Attributes[6].Value;//opvalue
                                            FunctionAttribute[index, 8] = function.Attributes[7].Value;//opnode
                                            FunctionAttribute[index, 9] = function.Attributes[8].Value;//relate
                                        }
                                    
                                    }
                                    FunctionAttribute[index, 3] = function.InnerText;//value
                                    index++;
                                }
                            }
                            else if ((attribute.Name == "CodeProperty" || attribute.Name == "OtherInfo") && attribute.HasChildNodes)
                            {
                                CodeAttribute = new string[3];
                                foreach (XmlNode element in attribute.ChildNodes)
                                {
                                    if (element.Attributes.Count == 0 || element.Attributes["name"].InnerText == "CodeInfo")
                                    {
                                        CodeAttribute[0] = element.InnerText;
                                    }
                                    else if (element.Attributes["name"].InnerText == "Description")
                                    {
                                        CodeAttribute[1] = element.InnerText;
                                    }
                                    else if (element.Attributes["name"].InnerText == "OptimizeInfo")
                                    {
                                        CodeAttribute[2] = element.InnerText;
                                    }
                                }
                            }
                        }
                        FindNode = true;
                        break;
                    }
                    //CurrentName是大类，只将基本属性中[0]附值
                    else if (child.Attributes[0].Value.Split(new char[] { ',' })[3] == CurrentName)
                    {
                        FindNode = true;
                        break;
                    }
                }
                if (FindNode)
                    break;
            }
        }      

        #endregion

        #region 删除节点方法
        ///<summary>
        ///删除所选定的节点，用于外部调用
        ///需要用第一个构造函数
        ///</summary>
        public void DeleteNode()
        {
            CurrentDoc.Load(Form1.ToolxmlPath);

            bool FindNode = false;


            foreach (XmlNode node in CurrentDoc.FirstChild.ChildNodes)
            {
                if (node.Attributes[0].Value == CurrentName)
                {
                    node.ParentNode.RemoveChild(node);
                    break;
                }
                else
                {
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.Attributes[0].Value.Split(new char[] { ',' })[2] == CurrentName)
                        {
                            child.ParentNode.RemoveChild(child);
                            FindNode = true;
                            break;
                        }
                    }
                }
                if (FindNode)
                    break;
            }
            CurrentDoc.Save(Form1.ToolxmlPath);
        }
        #endregion

        #region 创建节点方法
        ///<summary>
        ///由基本属性、功能属性和功能属性个数来创建一个新的节点
        ///用与内部使用
        ///</summary>
        private XmlNode CreateNode(string[] BasicProperty, string[,] FunctionProperty, int ndex, string[] OtherPorperty)
        {
            XmlElement newFDToolBoxItem = CurrentDoc.CreateElement("FDToolBoxItem");

            newFDToolBoxItem.SetAttribute("Type", "ControlTactic." + BasicProperty[7] + ",ControlTactic," + BasicProperty[2] + "," + BasicProperty[0]);


            //添加基本属性
            XmlElement newBasicProperty = CurrentDoc.CreateElement("BasicProperty");
            newFDToolBoxItem.AppendChild(newBasicProperty);

            if (BasicProperty[3] != "") //如果属性空则不创建此子节点
            {
                XmlElement newModuleNameProperty = CurrentDoc.CreateElement("Property");
                newModuleNameProperty.SetAttribute("name", "ModuleName");
                newModuleNameProperty.InnerText = BasicProperty[3];
                newBasicProperty.AppendChild(newModuleNameProperty);
            }

            if (BasicProperty[4] != "")
            {
                XmlElement newTextColorProperty = CurrentDoc.CreateElement("Property");
                newTextColorProperty.SetAttribute("name", "TextColor");
                newTextColorProperty.InnerText = BasicProperty[4];
                newBasicProperty.AppendChild(newTextColorProperty);
            }

            if (BasicProperty[5] != "")
            {
                XmlElement newOutputNameProperty = CurrentDoc.CreateElement("Property");
                newOutputNameProperty.SetAttribute("name", "OutputName");
                newOutputNameProperty.InnerText = BasicProperty[5];
                newBasicProperty.AppendChild(newOutputNameProperty);
            }

            if (BasicProperty[6] != "")
            {
                XmlElement newInputNameProperty = CurrentDoc.CreateElement("Property");
                newInputNameProperty.SetAttribute("name", "InputName");
                if (BasicProperty[6].Split(';').Length > 1)
                {
                    string[] tempArray = BasicProperty[6].Split(';');
                    foreach (string InputInfo in tempArray)
                    {
                        string[] info = InputInfo.Split(',');
                        XmlElement InputNode = CurrentDoc.CreateElement("InputValue");
                        InputNode.SetAttribute("name", info[0]);
                        InputNode.InnerText = info[1];
                        newInputNameProperty.AppendChild(InputNode);
                    }
                }
                else
                {
                    newInputNameProperty.InnerText = BasicProperty[6];
                }
                newBasicProperty.AppendChild(newInputNameProperty);
            }

            //模块颜色和模块类型是必加的节点属性
            XmlElement newModuleColorProperty = CurrentDoc.CreateElement("Property");
            newModuleColorProperty.SetAttribute("name", "ModuleColor");
            newModuleColorProperty.InnerText = BasicProperty[1];
            newBasicProperty.AppendChild(newModuleColorProperty);

            XmlElement newModuleSortProperty = CurrentDoc.CreateElement("Property");
            newModuleSortProperty.SetAttribute("name", "ModuleSort");
            newModuleSortProperty.InnerText = BasicProperty[2];
            newBasicProperty.AppendChild(newModuleSortProperty);

            //添加功能属性

            XmlElement newFunctionProperty = CurrentDoc.CreateElement("FunctionProperty");
            newFDToolBoxItem.AppendChild(newFunctionProperty);

            for (int i = 0; i < ndex; i++)
            {
                XmlElement newProperty = CurrentDoc.CreateElement("Property");
                newProperty.SetAttribute("name", FunctionProperty[i, 0]);
                newProperty.SetAttribute("varname", FunctionProperty[i, 1]);
                newProperty.SetAttribute("type", FunctionProperty[i, 2]);
                newProperty.SetAttribute("visible", FunctionProperty[i, 4]);

                newProperty.SetAttribute("exp", FunctionProperty[i, 5]);
                newProperty.SetAttribute("optype", FunctionProperty[i, 6]);
                newProperty.SetAttribute("opvalue", FunctionProperty[i, 7]);
                newProperty.SetAttribute("opnode", FunctionProperty[i, 8]);
                newProperty.SetAttribute("relate", FunctionProperty[i, 9]);

                newProperty.InnerText = FunctionProperty[i, 3];
                newFunctionProperty.AppendChild(newProperty);
            }

            //添加其他属性

            XmlElement newInfoPorperty = CurrentDoc.CreateElement("OtherInfo");

            XmlElement newCodeinfo = CurrentDoc.CreateElement("Property");
            newCodeinfo.SetAttribute("name", "CodeInfo");
            newCodeinfo.InnerText = OtherPorperty[0];
            XmlElement Description = CurrentDoc.CreateElement("Property");
            Description.SetAttribute("name", "Description");
            Description.InnerText = OtherPorperty[1];
            XmlElement OptimizeInfo = CurrentDoc.CreateElement("Property");
            OptimizeInfo.SetAttribute("name", "OptimizeInfo");
            OptimizeInfo.InnerText = OtherPorperty[2];

            newInfoPorperty.AppendChild(newCodeinfo);
            newInfoPorperty.AppendChild(Description);
            newInfoPorperty.AppendChild(OptimizeInfo);

            newFDToolBoxItem.AppendChild(newInfoPorperty);
            return newFDToolBoxItem;
        }
        #endregion

        #region 添加节点方法
        ///<summary>
        ///由外部传入的节点属性来创建节点，并写到XML文件中
        ///</summary>
        public void AddNode(string[] Basic, string[,] Function, int dex, string [] Code)
        {
            bool writer = false;//是否找到大类别写入节点

            CurrentDoc.Load(Form1.ToolxmlPath);

            foreach (XmlNode node in CurrentDoc.FirstChild.ChildNodes)
            {
                if (node.Attributes[0].Value == Basic[0])
                {
                    node.AppendChild(CreateNode(Basic, Function, dex, Code));
                    CurrentDoc.Save(Form1.ToolxmlPath);
                    writer = true;
                    break;
                }
            }

            //没有找到大类别而新加类别
            if (writer == false)
            {
                XmlElement newCategory = CurrentDoc.CreateElement("Category");
                newCategory.SetAttribute("DisplayName", Basic[0]);
                CurrentDoc.FirstChild.AppendChild(newCategory);
                newCategory.AppendChild(CreateNode(Basic, Function, dex, Code));
                CurrentDoc.Save(Form1.ToolxmlPath);
            }
        }
        #endregion
        
        #region 修改节点方法
        ///<summary>
        ///用于修改节点属性
        ///找到需要修改的节点，用修改后的属性创建一个新的节点来替换原节点********************有问题
        ///</summary>
        public void ModifyNode()
        {
            CurrentDoc.Load(Form1.ToolxmlPath);
            XmlNode MaNode = CurrentDoc.SelectSingleNode("/configuration/Category[@DisplayName='" + BasicAttribute[0] + "']");
            bool FindNode = false;
            if (MaNode != null)
            {
                foreach (XmlNode node in MaNode.ChildNodes)
                {
                    //要修改 如果找的到原节点则替换 找不到才删除添加！！！
                    if (node.Attributes[0].Value.Split(new char[] { ',' })[2] == CurrentName)
                    {
                        MaNode.ReplaceChild(CreateNode(BasicAttribute, FunctionAttribute, index, CodeAttribute), node);
                        FindNode = true;
                        break;
                    }
                    //else
                }
            }
            //没有修改属性后的大类或没有在修改后大类中找到需要修改的节点，则删除修改前的原节点添加新节点
            if (MaNode == null || FindNode == false)
            {
                //修改了大类
                DeleteNode();
                AddNode(BasicAttribute, FunctionAttribute, index, CodeAttribute);
            }
            CurrentDoc.Save(Form1.ToolxmlPath);
        }
        #endregion

        #region 树节点名修改方法
        ///<summary>
        ///外部调用的方法
        ///设置在TREEVIEW中选定修改的属性
        ///</summary>
        public void ChangeValue(string oldValue, string newValue)
        {
            CurrentDoc.Load(Form1.ToolxmlPath);
            XmlNode MaNode = CurrentDoc.SelectSingleNode("/configuration/Category[@DisplayName='" + oldValue + "']");
            string[] temp;

            if (MaNode != null)//找到
            {
                MaNode.Attributes[0].Value = newValue;

                foreach (XmlNode node in MaNode.ChildNodes)
                {
                    temp = node.Attributes[0].Value.Split(new char[] { ',' });
                    node.Attributes[0].Value = temp[0] + "," + temp[1] + "," + temp[2] + "," + newValue;
                }
            }
            else
            {
                CurrentName = oldValue;
                FindAttribute();
                XmlNode NewNode = CurrentDoc.SelectSingleNode("/configuration/Category[@DisplayName='" + BasicAttribute[0] + "']");
                foreach (XmlNode node in NewNode.ChildNodes)
                {
                    temp = node.Attributes[0].Value.Split(new char[] { ',' });
                    if (temp[2] == oldValue)
                    {
                        
                        foreach (XmlNode child in node.FirstChild.ChildNodes)
                        {
                            if (child.Attributes[0].Value == "ModuleSort")
                            {
                                child.InnerText = newValue;
                                break;
                            }
                        }
                        node.Attributes[0].Value = temp[0] + "," + temp[1] + "," + newValue + "," + temp[3];
                        break;
                    }
                }
            }
            CurrentDoc.Save(Form1.ToolxmlPath);
        }
        #endregion

        #region XML中头文件的读写
        public List<string[]> ReadHead()
        {
            CurrentDoc.Load(Form1.ToolxmlPath);
            XmlNode HDNode = CurrentDoc.SelectSingleNode("/configuration/Category[@DisplayName='特殊代码']");
            List<string[]> code = new List<string[]>();

            //if (HDNode != null)
            //{
            //    code = HDNode.FirstChild.InnerText;
            //}


            foreach (XmlNode element in HDNode.ChildNodes)
            {
                string[] nodeInfo = new string[3];
                nodeInfo[0] = element.Attributes["name"].InnerText;
                nodeInfo[1] = element.Attributes["varname"].InnerText;
                nodeInfo[2] = element.InnerText;
                code.Add(nodeInfo);
            }

            return code;
        }

        public void WriteHead(List<string[]> code)
        {
            CurrentDoc.Load(Form1.ToolxmlPath);
            XmlNode OldNode = CurrentDoc.SelectSingleNode("/configuration/Category[@DisplayName='特殊代码']");

            XmlElement newHead = CurrentDoc.CreateElement("Category");
            newHead.SetAttribute("DisplayName", "特殊代码");
            if (code.Count != 0)
            {
                foreach (string[] element in code)
                {
                    XmlElement newLine = CurrentDoc.CreateElement("CodeProperty");
                    newLine.SetAttribute("name", element[0]);
                    newLine.SetAttribute("varname", element[1]);
                    newLine.InnerText = element[2];
                    newHead.AppendChild(newLine);
                }
            }
            if (OldNode != null)
            {
                CurrentDoc.FirstChild.ReplaceChild(newHead, OldNode);
            }
            else
            {
                CurrentDoc.FirstChild.AppendChild(newHead);
            }
            CurrentDoc.Save(Form1.ToolxmlPath);
        }
        #endregion
    }

}