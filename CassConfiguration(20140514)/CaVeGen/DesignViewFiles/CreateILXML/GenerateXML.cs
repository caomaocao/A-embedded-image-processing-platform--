/*******************************************************************************
           ** Copyright (C) 2009 CASS 版权所有
           ** 文件名：CassView.cs 
           ** 功能描述：
           **          用于生成下位机所需要的XML文件和Main.C文件
           ** 作者：宋骁健
           ** 创始时间：2009-5-15
           ** 
********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Windows.Forms;
using CaVeGen;
using CaVeGen.DesignViewFiles;
using CaVeGen.DesignViewFiles.FilterProperty;
using System.Runtime.InteropServices;
using System.IO;
using CaVeGen.CommonOperation;

namespace CaVeGen.DesignViewFiles
{
    class GenerateXML
    {
        private XmlDocument document = new XmlDocument();
        private List<string> StructName = new List<string>();//所用到的控件结构体
        private List<string> SpeicalCtrls = new List<string>(new string[] { "模糊控制器", "解耦补偿控制器", "条件动作表", "计算器组态" });
        private List<List<XmlNode>> SpeicalNodes = new List<List<XmlNode>>();//特殊控件获取数据受生成的节点        
        private List<ArrayList> ModuleCount = new List<ArrayList>();//用于记录模块类型控件的结构体计数
        private List<string> Dsign = new List<string>(new string[] { "==", ">", ">=", "<", "<=" });//优化关系符号
        private List<string> Ssign = new List<string>(new string[] { "", "GT", "GE", "LT", "LE" });//优化关系符号简称
        private List<string> UseOpt =new List<string> ();//生成XML时用到的不重复地记录所有用过的优化控件的优化指令

        private List<string> STlist = new List<string>();//存放所有ST控件.参数的信息，优化所用
        static private List<string[]> UnseeArray = new List<string[]>();//存放不显示给用户的数组信息//暂时用于解耦的数组20090617

        string proName = null;
        string proInfo = null;
        string proIndex = null;
        string savePath = null;
        string Aid = null;//属性标示符 小写c configuration + proNum + _
        string Mid = null;//模块标示符 大写C Configuration + proNum + _
        string Sid = "Sub";//策略标识符 大写Sub
        string Pid = "Page";
        string Lid = "Loop";

        [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]  // 结构体用于将浮点32位数转换成4个字节
        public struct TextUnion
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public float f;
            [System.Runtime.InteropServices.FieldOffset(0)]
            public byte b0;
            [System.Runtime.InteropServices.FieldOffset(1)]
            public byte b1;
            [System.Runtime.InteropServices.FieldOffset(2)]
            public byte b2;
            [System.Runtime.InteropServices.FieldOffset(3)]
            public byte b3;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="savePath">保存路径</param>
        /// <param name="ProInfo">工程信息</param>
        public GenerateXML(string savePath, string[] ProInfo)
        {
            this.savePath = savePath;

            if (ProInfo != null)
            {
                this.proName = ProInfo[0];
                this.proInfo = ProInfo[1];
                this.proIndex = ProInfo[2];
            }
            Aid = "configuration" + proIndex + "_";
            Mid = "Configuration" + proIndex + "_";
        }


        private void CreateFile_ControlFunsC(string desfile)
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                fs = new FileStream(desfile, FileMode.Create);
                sw = new StreamWriter(fs, Encoding.Default);
            }
            catch (Exception ex)
            {
            }

            finally
            {
                sw.Close();
                fs.Close();
            }

        }
        /// <summary> 2013.11.20
        /// 创建工程XML文件,转换成“Main.c",生成"Configuration_ControlFuns.c"
        /// </summary>
        /// <param name="TempInfo">全局临时变量信息</param>
        /// <param name="addressInfo">地址信息</param>
        /// <param name="ctrllist">工程控件集合</param>
        /// <param name="IOList">生成指令表信息</param>
        public void CreateProjectMainC(List<string> TempInfo, List<string[]> addressInfo, List<ControlInfo> ctrllist, List<string[]> IOList, bool flag)
        {
            XmlElement DCS = document.CreateElement("DCS");
            //周期节点
            XmlElement Cycle = document.CreateElement("ProjectInfo");
            Cycle.InnerText = this.proInfo;
            DCS.AppendChild(Cycle);
            //工程序号节点
            XmlElement Pindex = document.CreateElement("ProjectIndex");
            Pindex.InnerText = this.proIndex;
            DCS.AppendChild(Pindex);
            //计算器节点
            XmlElement Cal = document.CreateElement("Calculators");
            if (SpeicalNodes.Count != 0)
            {
                foreach (XmlNode child in SpeicalNodes[0])
                { //特殊控件中第一部分为计算其组态                
                    Cal.AppendChild(child);
                }
            }
            DCS.AppendChild(Cal);


            //条件动作表节点
            XmlElement CA = document.CreateElement("ConditionActions");
            if (SpeicalNodes.Count > 1)
            {
                foreach (XmlNode child in SpeicalNodes[1])
                {//特殊控件中第二部分为计算其组态
                    CA.AppendChild(child);
                }

            }
            DCS.AppendChild(CA);

            //全局临时变量
            XmlElement tempValue = document.CreateElement("EntireVarRegion");
            foreach (string temp in TempInfo)
            {
                XmlElement tempItem = document.CreateElement("item");
                tempItem.SetAttribute("type", "fp32");
                tempItem.InnerText = Aid + temp;
                tempValue.AppendChild(tempItem);
            }
            DCS.AppendChild(tempValue);
            //参数表节点
            List<string> UseControl = new List<string>();//整个工程中用到的控件类型
            XmlElement Parameters = document.CreateElement("Parameters");

            //优先插入数组数据节点
            XmlElement Arrays = document.CreateElement("Arrays");
            bool canAdd = true;//是否需要添加对应的数组节点
            if (SpeicalNodes.Count > 2)
            {
                foreach (XmlNode child in SpeicalNodes[2])
                {//特殊控件中第三部分为数组数据
                    canAdd = true;
                    for (int i = 0; i < addressInfo.Count; i++)
                    {
                        if (addressInfo[i].Length == 8
                            && child.Attributes[1].Value == addressInfo[i][2]
                            && child.InnerText == "{" + addressInfo[i][7] + "}")
                        {//数组属性，且为修改的不可见数组则加入Array节点
                            canAdd = false;
                            break;
                        }
                    }
                    if (canAdd)
                    {
                        Arrays.AppendChild(child);
                    }
                }
            }
            foreach (string[] child in GenerateCode.UnseeArray)
            {//无条件添加不可见数组信息
                Arrays.AppendChild(CreateArrayNode(child));
            }
            Parameters.AppendChild(Arrays);

            #region     ControlFuns
         
            #region  生成 configuration_ControlFuns.c  文件  
                        //2013.11.20  by wtt
            string saveFile_ControlFunsC = this.savePath + "\\configuration_ControlFuns.c";
            if (File.Exists(saveFile_ControlFunsC))
                File.Delete(saveFile_ControlFunsC);

            FileStream  fs_ControlFuns = new FileStream(saveFile_ControlFunsC,FileMode.Create,FileAccess.Write);
            StreamWriter sw_ControlFuns = new StreamWriter(fs_ControlFuns,Encoding.UTF8);
            #endregion

            XmlDocument tempD = new XmlDocument();
            XmlElement ControlFuns = tempD.CreateElement("ControlFuns");
            StructName = new List<string>();//清空控件结构体
            ModuleCount = new List<ArrayList>();//清空模块计数表
            foreach (ControlInfo ctrl in ctrllist)
            {
                if (ctrl.CodeInfo != null && !GenerateCode.SortCtrlName.Contains(ctrl.CodeInfo[0])/*&&!ctrl.CodeInfo[0].Contains("文件输出")*/)//2013.11.26
                {//满足条件1的为算法控件 条件2排除输入输出控件（1，2位放置目标控件及属性名）
                    if (ctrl.CodeInfo[2] == null)
                    {
                        string tempName = GetModuleNum(ctrl.CodeInfo[1], ctrl.ControlNum);
                        StructName.Add(tempName);
                        Parameters.AppendChild(CreateParameterNode(addressInfo, ctrl, tempName));
                    }
                    else
                    {
                        StructName.Add(ctrl.CodeInfo[2]);
                        Parameters.AppendChild(CreateParameterNode(addressInfo, ctrl, ctrl.CodeInfo[2]));
                    }
                }
                if (UseControl.Count == 0 || !UseControl.Contains(ctrl.CodeInfo[0]))
                {//添加指令信息XML节点，节点属性为中文不带Control20090609
                    XmlElement UseType = tempD.CreateElement("item");
                    UseType.SetAttribute("name", ctrl.CodeInfo[0]);
                    if (ctrl.OtherProperty != null)
                    {
                        UseType.InnerText = ctrl.OtherProperty[0];//* 获取模块函数代码段
                        sw_ControlFuns.WriteLine(ctrl.OtherProperty[0]);   //* 2013.11.20  by  wtt

                    }
                    ControlFuns.AppendChild(UseType);
                    UseControl.Add(ctrl.CodeInfo[0]);
                    if (ctrl.OtherProperty != null && ctrl.OtherProperty.Length == 2 && ctrl.OtherProperty[1] != null && ctrl.OtherProperty[1] != "")
                    {//记录控件优化信息
                        UseOpt.Add(ctrl.OtherProperty[1]);
                    }
                }
            }
            //2013.11.20  关闭文件流  by wtt
            sw_ControlFuns.Close();
            fs_ControlFuns.Close();
            #endregion

            //根据指令表中信息额外添加CodeCtrlName列表中的指令信息
            foreach (string[] IOrow in IOList)
            {
                if (IOrow != null)
                {//如果有额外控件信息添加时加入对应的控件指令信息
                    int tempIndex = GenerateCode.CodeCtrlName.IndexOf(IOrow[0]);
                    if (tempIndex >= 0)
                    {
                        string tempName = GenerateCode.SortCtrlName[tempIndex];
                        if (!UseControl.Contains(tempName))
                        {
                            foreach (XmlNode categoryNode in ToolBoxServiceImpl.toolXML.FirstChild.ChildNodes)
                            {
                                bool findNode = false;
                                if (categoryNode != null && categoryNode.Attributes[0].InnerText != CassViewGenerator.SpecialCodeNode
                                    && categoryNode.Attributes[0].InnerText == "变量")
                                {
                                    foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                                    {
                                        if (toolItemNode.Attributes[0].InnerText.Split(',')[2] == tempName
                                            && (toolItemNode.LastChild.Name == "CodeProperty" || toolItemNode.LastChild.Name == "OtherInfo")
                                            && toolItemNode.LastChild.ChildNodes.Count > 0)
                                        {
                                            foreach (XmlNode element in toolItemNode.LastChild.ChildNodes)
                                            {//描述节点不加入控件信息中
                                                if (element.Attributes.Count == 0 || element.Attributes["name"].InnerText == "CodeInfo")
                                                {
                                                    XmlElement UseType = tempD.CreateElement("item");
                                                    UseType.SetAttribute("name", tempName);
                                                    UseType.InnerText = element.InnerText;
                                                    ControlFuns.AppendChild(UseType);
                                                }
                                                else if (element.Attributes["name"].InnerText == "OptimizeInfo")
                                                {
                                                    UseOpt.Add(element.InnerText);
                                                }
                                            }
                                            UseControl.Add(tempName);
                                            findNode = true;
                                            break;
                                        }
                                    }
                                }
                                else if (categoryNode != null && categoryNode.Attributes[0].InnerText == CassViewGenerator.SpecialCodeNode)
                                {//找到特殊指令中所用到的代码
                                    foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                                    {
                                        if (toolItemNode.Attributes["name"].InnerText == tempName
                                            && toolItemNode.Attributes["varname"].InnerText.ToUpper() == IOrow[0])
                                        {
                                            XmlElement UseType = tempD.CreateElement("item");
                                            UseType.SetAttribute("name", tempName);
                                            UseType.InnerText = toolItemNode.InnerText;
                                            ControlFuns.AppendChild(UseType);
                                            UseControl.Add(tempName);
                                            findNode = true;
                                            break;
                                        }
                                    }
                                }
                                if (findNode)
                                    break;
                            }
                        }
                    }
                }
            }
            DCS.AppendChild(Parameters);
            //策略节点
            List<XmlElement> Tactics = CreateTacticNode(IOList, TempInfo, ctrllist);
            foreach (XmlElement tactic in Tactics)
            {
                DCS.AppendChild(tactic);
            }
            document.AppendChild(DCS);
            document.Save(this.savePath + "\\" + this.proName + ".xml");

            //生成Configuration_ControlFuns.xml
            tempD.AppendChild(ControlFuns);
            tempD.Save(this.savePath + "\\" + "Configuration_ControlFuns.xml");
            //针对上位机生成标准C的main函数
            if (flag)
                GenSrcFile(this.savePath + "\\" + this.proName + ".xml", CassViewGenerator.designerPath + "\\XSLTParam_PC.xslt", this.savePath + "\\main.c");
            else
            {
                GenSrcFile(this.savePath + "\\" + this.proName + ".xml", CassViewGenerator.designerPath + "\\XSLTParam.xslt", this.savePath + "\\main.c");
                #region   生成cass_mv_main.c
                //合并文件名：cass_mv_main.c        by wtt 2013.12.27
                CreateCassMVMainCFile(this.savePath+"\\cass_mv_main.c");
                //删除中间文件
                string delFile = null;
                foreach (string file in delFileList)
                {
                    delFile = file.Replace("//", "\\");
                    if (File.Exists(this.savePath + delFile))
                        File.Delete(this.savePath + delFile);
                  }
               
                #endregion
            }
        }

        #region    嵌入式版本：支持Cass的机器视觉文件   2013.12.27
        //cass版本需要合并的中间文件
        private string[] combineFileList = { "//configuration_ControlFuns.c", "//feather.h" };
        private string[] delFileList = { "//configuration_ControlFuns.c", "//main.c" };
     //   private string[] headFileList = { "configuration_address.h", "configuration_system.h", "configuration_control.h" };
        /// <summary>
        /// 生成支持Cass版本的机器视觉main文件
       /// </summary>
      /// <param name="FilePath">文件路径</param>
        public void CreateCassMVMainCFile(string FilePath )
        {
            try
            {
                //新建目标文件流
                FileStream desFileStream = null;
                if (File.Exists(FilePath))
                {
                    desFileStream = new FileStream(FilePath, FileMode.Truncate);
                }
                else
                {
                    desFileStream = new FileStream(FilePath, FileMode.CreateNew);
                }
                StreamWriter desSw = new StreamWriter(desFileStream);

                string insertContent = null;

                //添加3个头文件   2014.1.6
                //获取cass_h的目录
                DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                string cass_hPath = dirInfo + "cass_h";
                //读取头文件
                //desSw.WriteLine("//insert head File ");
                //desSw.WriteLine("/**************Begin****************/");
                //foreach (string headFile in headFileList)
                //{
                //    string headFilePath = cass_hPath + "\\" + headFile;
                //    if (File.Exists(headFilePath))
                //    {
                //        insertContent = FileOperator.ReadFromFile2(headFilePath);
                //        desSw.Write(insertContent);
                //    }
                //    else
                //    {
                //        CassMessageBox.Error("The file ["  +headFile+ "]  is not existed  in directory ["+ cass_hPath+"]!");
                //    }
                //}
                //desSw.WriteLine("/***************End*****************/");
                
                //读取main.c中的内容
                ArrayList maincWriteValue = FileOperator.ReadFromFile(this.savePath+"\\main.c",Encoding.GetEncoding("gb2312"));
                //一行一行遍历，查询特殊字符串
                foreach (string lineData in maincWriteValue)
                {
                    if (lineData.Contains("//"))
                    {
                        //移除所有前导和后导的空白字符
                        string temp = lineData.Trim();
                        //判断是否存在于“合并文件列表”中
                        if (isExists(temp, combineFileList))
                        {
                            temp = temp.Replace("//", "\\");
                            desSw.WriteLine(lineData);
                            desSw.WriteLine("//begin");
                            insertContent = FileOperator.ReadFromFile2(this.savePath + temp);
                            desSw.Write(insertContent);
                            desSw.WriteLine("//end");
                        }
                        continue;
                                 
                    }
                    desSw.WriteLine(lineData);
                }
                desSw.Close();
                desFileStream.Close();
                desSw.Dispose();
                desFileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 判断某字符串是否存在于某个字符串数组中，若存在，返回true，否则false
        /// </summary>
        /// <param name="obj">待查询的字符串</param>
        /// <param name="myList">字符串数组</param>
        /// <returns>布尔值</returns>
        private bool isExists(string obj, string[] myList)
        {
            bool flag = false;
            foreach (string str in myList)
            {
                if (str.Equals(obj))
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        #endregion

        /// <summary>
        /// 创建工程XML文件,转换成Main.c,生成Configuration_ControlFuns.xml
        /// </summary>
        /// <param name="TempInfo">全局临时变量信息</param>
        /// <param name="addressInfo">地址信息</param>
        /// <param name="ctrllist">工程控件集合</param>
        /// <param name="IOList">生成指令表信息</param>
        /// 原来的创建xml接口的函数，视觉平台生成c文件
        public void CreateProjectXML_old(List<string> TempInfo, List<string[]> addressInfo, List<ControlInfo> ctrllist, List<string[]> IOList)
        {
            XmlElement DCS = document.CreateElement("DCS");
            //周期节点
            XmlElement Cycle = document.CreateElement("ProjectInfo");
            Cycle.InnerText = this.proInfo;
            DCS.AppendChild(Cycle);
            //工程序号节点
            XmlElement Pindex = document.CreateElement("ProjectIndex");
            Pindex.InnerText = this.proIndex;
            DCS.AppendChild(Pindex);
            //计算器节点
            XmlElement Cal = document.CreateElement("Calculators");
            if (SpeicalNodes.Count != 0)
            {
                foreach (XmlNode child in SpeicalNodes[0])
                { //特殊控件中第一部分为计算其组态                
                    Cal.AppendChild(child);
                }
            }
                DCS.AppendChild(Cal);
            
        
            //条件动作表节点
            XmlElement CA = document.CreateElement("ConditionActions");
            if (SpeicalNodes.Count > 1)
            {
                foreach (XmlNode child in SpeicalNodes[1])
                {//特殊控件中第二部分为计算其组态
                    CA.AppendChild(child);
                }
               
            }
                DCS.AppendChild(CA);
                       
            //全局临时变量
            XmlElement tempValue = document.CreateElement("EntireVarRegion");
            foreach (string temp in TempInfo)
            {
                XmlElement tempItem = document.CreateElement("item");
                tempItem.SetAttribute("type", "fp32");
                tempItem.InnerText = Aid + temp;
                tempValue.AppendChild(tempItem);
            }
            DCS.AppendChild(tempValue);
            //参数表节点
            List<string> UseControl = new List<string>();//整个工程中用到的控件类型
            XmlElement Parameters = document.CreateElement("Parameters");

            //优先插入数组数据节点
            XmlElement Arrays = document.CreateElement("Arrays");
            bool canAdd = true;//是否需要添加对应的数组节点
            if (SpeicalNodes.Count > 2)
            {
                foreach (XmlNode child in SpeicalNodes[2])
                {//特殊控件中第三部分为数组数据
                    canAdd = true;
                    for (int i = 0; i < addressInfo.Count; i++)
                    {
                        if (addressInfo[i].Length == 8
                            && child.Attributes[1].Value == addressInfo[i][2]
                            && child.InnerText == "{" + addressInfo[i][7] + "}")
                        {//数组属性，且为修改的不可见数组则加入Array节点
                            canAdd = false;
                            break;
                        }
                    }
                    if (canAdd)
                    {
                        Arrays.AppendChild(child);
                    }
                }
            }
            foreach (string[] child in GenerateCode.UnseeArray)
            {//无条件添加不可见数组信息
                Arrays.AppendChild(CreateArrayNode(child));
            }
            Parameters.AppendChild(Arrays);

            XmlDocument tempD = new XmlDocument();
            XmlElement ControlFuns = tempD.CreateElement("ControlFuns");
            StructName = new List<string>();//清空控件结构体
            ModuleCount = new List<ArrayList>();//清空模块计数表
            foreach (ControlInfo ctrl in ctrllist)
            {
                if (ctrl.CodeInfo != null && !GenerateCode.SortCtrlName.Contains(ctrl.CodeInfo[0]) /*&& !ctrl.CodeInfo[0].Contains("文件输出")*/) //2013.11.26
                {//满足条件1的为算法控件 条件2排除输入输出控件（1，2位放置目标控件及属性名）
                    if (ctrl.CodeInfo[2] == null)
                    {
                        string tempName = GetModuleNum(ctrl.CodeInfo[1], ctrl.ControlNum);
                        StructName.Add(tempName);
                        Parameters.AppendChild(CreateParameterNode(addressInfo, ctrl, tempName));
                    }
                    else
                    {
                        StructName.Add(ctrl.CodeInfo[2]);
                        Parameters.AppendChild(CreateParameterNode(addressInfo, ctrl, ctrl.CodeInfo[2]));
                    }
                }
                if (UseControl.Count == 0 || !UseControl.Contains(ctrl.CodeInfo[0]))
                {//添加指令信息XML节点，节点属性为中文不带Control20090609
                    XmlElement UseType = tempD.CreateElement("item");
                    UseType.SetAttribute("name", ctrl.CodeInfo[0]);
                    if (ctrl.OtherProperty != null)
                    {
                        UseType.InnerText = ctrl.OtherProperty[0];
                    }
                    ControlFuns.AppendChild(UseType);
                    UseControl.Add(ctrl.CodeInfo[0]);
                    if (ctrl.OtherProperty != null && ctrl.OtherProperty.Length == 2 && ctrl.OtherProperty[1] != null && ctrl.OtherProperty[1] != "")
                    {//记录控件优化信息
                        UseOpt.Add(ctrl.OtherProperty[1]);
                    }
                }
            }
        
            //根据指令表中信息额外添加CodeCtrlName列表中的指令信息
            foreach (string[] IOrow in IOList)
            {
                if (IOrow != null)
                {//如果有额外控件信息添加时加入对应的控件指令信息
                    int tempIndex = GenerateCode.CodeCtrlName.IndexOf(IOrow[0]);
                    if (tempIndex >= 0)
                    {
                        string tempName = GenerateCode.SortCtrlName[tempIndex];
                        if (!UseControl.Contains(tempName))
                        {
                            foreach (XmlNode categoryNode in ToolBoxServiceImpl.toolXML.FirstChild.ChildNodes)
                            {
                                bool findNode = false;
                                if (categoryNode != null && categoryNode.Attributes[0].InnerText != CassViewGenerator.SpecialCodeNode
                                    && categoryNode.Attributes[0].InnerText == "变量")
                                {
                                    foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                                    {
                                        if (toolItemNode.Attributes[0].InnerText.Split(',')[2] == tempName
                                            && (toolItemNode.LastChild.Name == "CodeProperty" || toolItemNode.LastChild.Name == "OtherInfo") 
                                            && toolItemNode.LastChild.ChildNodes.Count > 0)
                                        {
                                            foreach (XmlNode element in toolItemNode.LastChild.ChildNodes)
                                            {//描述节点不加入控件信息中
                                                if (element.Attributes.Count == 0 || element.Attributes["name"].InnerText == "CodeInfo")
                                                {
                                                    XmlElement UseType = tempD.CreateElement("item");
                                                    UseType.SetAttribute("name", tempName);
                                                    UseType.InnerText = element.InnerText;
                                                    ControlFuns.AppendChild(UseType);
                                                }
                                                else if (element.Attributes["name"].InnerText == "OptimizeInfo")
                                                {
                                                    UseOpt.Add(element.InnerText);

                                                }
                                            }
                                            UseControl.Add(tempName);
                                            findNode = true;
                                            break;
                                        }
                                    }
                                }
                                else if(categoryNode != null && categoryNode.Attributes[0].InnerText == CassViewGenerator.SpecialCodeNode)
                                {//找到特殊指令中所用到的代码
                                    foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                                    {
                                        if (toolItemNode.Attributes["name"].InnerText== tempName
                                            && toolItemNode.Attributes["varname"].InnerText.ToUpper() == IOrow[0])
                                        {
                                            XmlElement UseType = tempD.CreateElement("item");
                                            UseType.SetAttribute("name", tempName);
                                            UseType.InnerText = toolItemNode.InnerText;
                                            ControlFuns.AppendChild(UseType);
                                            UseControl.Add(tempName);

                                            findNode = true;
                                            break;
                                        }
                                    }
                                }
                                if (findNode)
                                    break;
                            }
                        }
                    }
                }
            }
            DCS.AppendChild(Parameters);
            //策略节点
            List<XmlElement> Tactics = CreateTacticNode(IOList, TempInfo, ctrllist);
            foreach (XmlElement tactic in Tactics)
            {
                DCS.AppendChild(tactic);
            }
            document.AppendChild(DCS);
            document.Save(this.savePath + "\\" + this.proName + ".xml");
            //根据xslt模板文件生成 main.c
           GenSrcFile(this.savePath + "\\" + this.proName + ".xml", CassViewGenerator.designerPath + "\\XSLTParam.xslt", this.savePath + "\\configuration" + this.proIndex + "_main.c");
        
            tempD.AppendChild(ControlFuns);
            tempD.Save(this.savePath + "\\" + "Configuration_ControlFuns.xml");

        }

        /// <summary>
        /// 生成Configuration_AddressTable.xml
        /// </summary>
        /// <param name="addressInfo">地址信息</param>
        public void CreateAddressTableXML(List<string[]> addressInfo)
        {
            document = new XmlDocument();
            XmlElement AddressTable = document.CreateElement("AddressTable");
            foreach (string[] element in addressInfo)
            {
                if (element[5] != "不显示" && element[4] != "")
                {//可见属性且地址非空则加节点
                    XmlElement newItem = document.CreateElement("item");
                    newItem.SetAttribute("name", element[0]);
                    newItem.SetAttribute("parameter", element[6]);
                    newItem.InnerText = element[4];
                    AddressTable.AppendChild(newItem);
                }
            }
            document.AppendChild(AddressTable);
            document.Save(this.savePath + "\\" + "Configuration_AddressTable.xml");
        }

        /// <summary>
        /// 生成Configuration_PLC.xml
        /// </summary>
        /// <param name="addressRWsize">可读可写数据长度</param>
        public void CreateBasicInfoXML(int addressRWsize)
        {
            document = new XmlDocument();
            XmlElement BasicInfo = document.CreateElement("BasicInfo");
            XmlElement Pindex = document.CreateElement("ProjectIndex");
            Pindex.InnerText = this.proIndex;
            XmlElement Pname = document.CreateElement("ProjectName");
            Pname.InnerText = this.proName;
            XmlElement RWSize = document.CreateElement("RWSize");
            RWSize.InnerText = addressRWsize.ToString();
            BasicInfo.AppendChild(Pindex);
            BasicInfo.AppendChild(Pname);
            BasicInfo.AppendChild(RWSize);
            document.AppendChild(BasicInfo);
            document.Save(this.savePath + "\\" + "Configuration_PLC.xml");
        }

        /// <summary>
        /// 生成Configuration_Datas.xml
        /// </summary>
        /// <param name="addressInfo">地址信息</param>
        /// <param name="ctrllist">工程控件集合</param>
        public void CreateDatasXML(List<string[]> addressInfo, List<ControlInfo> ctrllist)
        {
            document = new XmlDocument();
            XmlElement Datas = document.CreateElement("Datas");
            XmlElement content = document.CreateElement("content");
            List<string> numberString = new List<string>();
            TextUnion textUnion = new TextUnion();

            if (addressInfo.Count != 0)
            {
                string[] address = new string[AddressTable.Eaddress];

                foreach (string[] element in addressInfo)
                {
                    if (element[5] == "True")//地址为可读可写
                    {//遍历控件集合找到对应的控件
                        XProp attribute = FindCtrlattribute(element, ctrllist);
                        string[] tempValue;
                        if (element.Length == 8)
                        {
                            tempValue = element[7].Split(',');
                        }
                        else
                        {
                            tempValue = new string[1];
                            tempValue[0] = attribute.TheValue.ToString();
                        }
                        if (element[2] == "uint8")//bool和char类统一为uint8
                        {
                            for (int i = 0; i < tempValue.Length; i++)
                            {
                                if (tempValue[i] != "0" && tempValue[i] != "1")
                                {
                                    if (attribute.ValueType == "System.Boolean")//布尔值
                                    {
                                        tempValue[i] = Convert.ToInt16(Convert.ToBoolean(tempValue[i])).ToString();
                                    }
                                    else//自定义attribute.ValueType == "MyEnum"
                                    {
                                        List<string> temp = new List<string>(attribute.EnumValue.Split(','));
                                        tempValue[i] = temp.IndexOf(tempValue[i]).ToString();
                                    }
                                }
                                //numberString.Add("0x" + tempValue[i]);
                                address[Convert.ToInt32(element[4]) + i] = "0x" + tempValue[i];
                            }
                        }
                        else//浮点和无符号32整型
                        {
                            int index = AddressTable.Type4.IndexOf(element[2]);
                            for (int x = 0; x < tempValue.Length; x++)
                            {//浮点与整型不同的处理方法转换成32位
                                if (index == 1)//fp32
                                {
                                    textUnion.f = (float)Convert.ToDouble(tempValue[x]);
                                    address[Convert.ToInt32(element[4]) + x * 4] = "0x" + Convert.ToString(textUnion.b0, 16);
                                    address[Convert.ToInt32(element[4]) + 1 + x * 4] = "0x" + Convert.ToString(textUnion.b1, 16);
                                    address[Convert.ToInt32(element[4]) + 2 + x * 4] = "0x" + Convert.ToString(textUnion.b2, 16);
                                    address[Convert.ToInt32(element[4]) + 3 + x * 4] = "0x" + Convert.ToString(textUnion.b3, 16);
                                }
                                else if (index == 0)//uint32
                                {
                                    UInt32 tempData = Convert.ToUInt32(tempValue[x]);
                                    address[Convert.ToInt32(element[4]) + x * 4] = "0x" + Convert.ToString(tempData & 0xff, 16);
                                    address[Convert.ToInt32(element[4]) + 1 + x * 4] = "0x" + Convert.ToString((tempData >> 8) & 0xff, 16);
                                    address[Convert.ToInt32(element[4]) + 2 + x * 4] = "0x" + Convert.ToString((tempData >> 16) & 0xff, 16);
                                    address[Convert.ToInt32(element[4]) + 3 + x * 4] = "0x" + Convert.ToString(tempData >> 24, 16);
                                }
                            }
                        }
                    }
                }
                address = Addto4(address);//把个位的数补零
                content.InnerText = String.Join(",", address);
            }
            Datas.AppendChild(content);
            document.AppendChild(Datas);
            document.Save(this.savePath + "\\" + "Configuration_Datas.xml");
        }

        /// <summary>
        /// 生成Configuration_Def.xml
        /// </summary>
        /// <param name="addressInfo">地址信息</param>
        /// <param name="ctrllist">工程控件集合</param>
        public void CreateDefXML(List<string[]> addressInfo, List<ControlInfo> ctrllist)
        {
            document = new XmlDocument();
            XmlElement Define = document.CreateElement("Define");

            List<string> deflist = new List<string>();
            foreach (ControlInfo ctrl in ctrllist)
            {
                if (ctrl.CanOptimize)
                {
                    infosss(ref Define, ctrl.VisibleFunctionProperty, addressInfo, ctrl, ref deflist);
                    infosss(ref Define, ctrl.UnvisibleFunctionProperty, addressInfo, ctrl,ref  deflist);
                }
            }
            foreach (string optinfo in this.UseOpt)
            {
                XmlElement content = document.CreateElement("item");
                if (optinfo.Length != 0)
                {
                    content.InnerText = optinfo;
                    Define.AppendChild(content);
                }
                
            }

            XmlElement content1 = document.CreateElement("item");
            content1.InnerText = "OPTIMISE_QUEUE";
            Define.AppendChild(content1);
            XmlElement content2 = document.CreateElement("item");
            content2.InnerText = "OPTIMISE_MATRIX";
            Define.AppendChild(content2);

            document.AppendChild(Define);
            document.Save(this.savePath + "\\" + "Configuration_Def.xml");
        }

        /// <summary>
        /// 对属性集合进行查找是否有优化属性
        /// </summary>
        /// <param name="Define">定义父节电</param>
        /// <param name="properties">属性节电</param>
        /// <param name="addressInfo">地址信息</param>
        /// <param name="ctrl">当前控件信息</param>
        /// <param name="deflist">已经定义队列</param>
        private void infosss(ref XmlElement Define,List<XProp> properties, List<string[]> addressInfo, ControlInfo ctrl,ref List<string> deflist)
        {
            foreach (XProp element in properties)
            {
                try
                {
                    if (element.Optype != null || element.Opvalue != null || element.Relate != null)
                    {//此属性需要优化
                        //List<string> express = new List<string>();
                        List<string> sign = new List<string>();
                        List<string> Opvalue = new List<string>();
                        bool doubleSign = false;
                        if (Findaddressinfo(addressInfo, ctrl, element) || STlist.Contains(ctrl.CodeInfo[2] + "." + element.VarName))
                        {//存在地址列表或被ST
                            doubleSign = true;
                        }
                        if (element.Optype == "特定值优化")
                        {
                            int Sindex = 0;//逻辑符号序号默认为0标示等号
                            float temp1 = Convert.ToSingle(element.TheValue);//用与比较默认转换单精度比较
                            if (element.Opvalue.Contains("/"))
                            {//多个特定值
                                string[] tempV = element.Opvalue.Split('/');
                                foreach (string va in tempV)
                                {
                                    Opvalue.Add(va);
                                    if (doubleSign)
                                    {//存在地址列表需要双份
                                        Opvalue.Add(va);
                                    }
                                }
                            }
                            else
                            {//单个特定值
                                //Opvalue.Add(XProp.GetValue(element.Opvalue, ctrl.VisibleFunctionProperty));
                                //if (Opvalue[0] == null)
                                //{
                                //    Opvalue[0] = element.Opvalue;
                                //}
                                Opvalue.Add(element.Opvalue);
                                if (doubleSign)
                                { //存在地址列表需要双份
                                    Opvalue.Add(element.Opvalue);
                                }
                            }
                            if (element.Relate != "")
                            {//>大于(GT),>=大于等于(GE),<小于(LT),<=小于等于(LE)   
                                Sindex = Dsign.IndexOf(element.Relate);
                            }
                            for (int j = 0; j < Opvalue.Count; j++)
                            {
                                sign.Add(Ssign[Sindex]);
                                if (doubleSign)
                                {//存在地址列表每个值对应 是与非
                                    sign.Add("NOT" + Ssign[Sindex]);
                                }
                                else
                                {
                                    string tempValue = XProp.GetValue(Opvalue[j], ctrl.VisibleFunctionProperty);
                                    if (tempValue == null)
                                    {
                                        tempValue = Opvalue[j];
                                    }
                                    float temp2 = Convert.ToSingle(tempValue);
                                    if (Sindex == 0 && temp1 != temp2 || Sindex == 1 && !(temp1 > temp2) || Sindex == 2 && !(temp1 >= temp2)
                                        || Sindex == 3 && !(temp1 < temp2) || Sindex == 4 && !(temp1 <= temp2))
                                    {//判断是还是非
                                        sign[j] = "NOT" + Ssign[Sindex];
                                    }
                                }
                            }
                        }
                        else if (element.Optype == "值优化")
                        {
                            if (element.EnumValue != "")
                            {
                                if (doubleSign)
                                {
                                    int Maxnum = element.EnumValue.Split(',').Length;
                                    for (int i = 0; i < Maxnum; i++)
                                    {
                                        Opvalue.Add(i.ToString());
                                        sign.Add("");
                                    }
                                }
                                else
                                {
                                    sign.Add("");
                                    string tempValue = new List<string>(element.EnumValue.Split(',')).IndexOf(element.TheValue.ToString()).ToString();
                                    if (tempValue == "-1")
                                        tempValue = element.TheValue.ToString();
                                    Opvalue.Add(tempValue);
                                }
                            }
                            else
                            {
                                sign.Add("");
                                Opvalue.Add(element.TheValue.ToString());
                            }
                        }

                        for (int x = 0; x < Opvalue.Count; x++)
                        {
                            string express = ctrl.CodeInfo[1] + "_" + element.VarName + "_" + sign[x] + Opvalue[x];
                            if (!deflist.Contains(express))
                            {
                                XmlElement content = document.CreateElement("item");
                                deflist.Add(express);
                                content.InnerText = express;
                                Define.AppendChild(content);
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("控件" + ctrl.SortName + "的属性" + element.Name + "优化时出错！");
                    continue;
                }
            }
        }

        /// <summary>
        /// 把0x1的三位变成0x01的四位
        /// 没有值的赋0x00
        /// </summary>
        /// <param name="UnaddList"></param>
        /// <returns></returns>
        private string[] Addto4(string[] UnaddList)
        {
            for (int i = 0; i < UnaddList.Length; i++)
            {
                if (UnaddList[i] == null || UnaddList[i] == "")
                {
                    UnaddList[i] = "0x00";
                }
                else if (UnaddList[i].Length == 3)
                {
                    UnaddList[i] = "0x0" + UnaddList[i][2];
                }

            }
            return UnaddList;
        }

        /// <summary>
        /// 从控件信息集合中找到对应的属性
        /// </summary>
        /// <param name="element">目标地址的属性信息</param>
        /// <param name="ctrllist">控件集合</param>
        /// <returns>返回属性</returns>
        private XProp FindCtrlattribute(string[] element, List<ControlInfo> ctrllist)
        {
            foreach (ControlInfo ctrl in ctrllist)
            {
                if (element[0] == ctrl.CodeInfo[2])
                {//控件名相同
                    foreach (XProp attribute in ctrl.VisibleFunctionProperty)
                    {
                        if (element[6] == attribute.VarName)
                        {//属性名相同
                            return attribute;
                        }
                    }
                    foreach (XProp attribute in ctrl.UnvisibleFunctionProperty)
                    {
                        if (element[6] == attribute.VarName)
                        {//属性名相同
                            return attribute;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 当前控件的某属性是否存在与地址表中
        /// </summary>
        /// <param name="address">地址信息列表</param>
        /// <param name="ctrl">当前控件信息</param>
        /// <param name="atribt">属性信息</param>
        /// <returns>存在地址信息中则为true</returns>
        private bool Findaddressinfo(List<string[]> address, ControlInfo ctrl, XProp atribt)
        {
            foreach (string[] ads in address)
            {
                if (ads[0] == ctrl.CodeInfo[2])
                {//控件名相同
                    if (ads[6] == atribt.VarName)
                    {
                        return true;
                    }                    
                }
            }
            return false;
        }

        /// <summary>
        /// 根据所给的控件信息创建XML中有功能属性的模块结点
        /// </summary>
        /// <param name="ctrlinfo">控件信息</param>
        /// <returns>返回控件的XML节点</returns>
        private XmlNode CreateParameterNode(List<string[]> addressInfo, ControlInfo ctrlinfo, string structName)
        {
            XmlElement newParameter = document.CreateElement("Parameter");
            newParameter.SetAttribute("type", ctrlinfo.CodeInfo[1]);
            newParameter.SetAttribute("name", Aid + structName);
            newParameter.SetAttribute("index",ctrlinfo.ControlNum.ToString());  //2013.11.27
    
            List<string> tempList = new List<string>();//存放已经在E2PROM中用到的英文属性则不显示在other中
            //地址节点
            XmlElement newE2PROM = document.CreateElement("E2PROM");

            foreach (string[] address in addressInfo)
            {
                if (address[0] == structName && address[5] == "True")
                {
                    XmlElement newValue = document.CreateElement("item");
                    if (address.Length == 7)
                    {
                        newValue.SetAttribute("type", address[2]);
                        newValue.SetAttribute("name", address[6]);
                        foreach (XProp element in ctrlinfo.VisibleFunctionProperty)
                        {
                            if (element.VarName == address[6] && element.Opnode != null && element.Opnode != "")
                            {//属性存在            
                                newValue.SetAttribute("optimise", element.Opnode);
                            }
                        }
                        newValue.InnerText = address[4];
                    }
                    else//address.Length == 8
                    {
                        newValue.SetAttribute("type", address[2] + "Array");
                        newValue.SetAttribute("name", address[6]);
                        foreach (XProp element in ctrlinfo.UnvisibleFunctionProperty)
                        {
                            if (element.VarName == address[6] && element.Opnode != null && element.Opnode != "")
                            {//属性存在            
                                newValue.SetAttribute("optimise", element.Opnode);
                            }
                        }
                        newValue.InnerText = address[4] + "," + address[7].Split(',').Length.ToString();
                    }
                    newE2PROM.AppendChild(newValue);
                    tempList.Add(address[6]);
                }
            }
            tempList.Add(CassViewGenerator.portIndex);//添加点名使得其不出现在other节点中

            newParameter.AppendChild(newE2PROM);
            //others节点
            XmlElement newothers = document.CreateElement("others");
            if (ctrlinfo.VisibleFunctionProperty != null)  //20140224
            {
                //可见属性的不显示属性节点
                foreach (XProp element in ctrlinfo.VisibleFunctionProperty)
                {
                    if (!tempList.Contains(element.VarName))
                    {
                        XmlElement newValue = document.CreateElement("item");
                        newValue.SetAttribute("name", element.VarName);
                        if (element.Opnode != null && element.Opnode != "")
                        {//others中的属性 如有优化节点值则加入参数的 优化属性中
                            newValue.SetAttribute("optimise", element.Opnode);
                        }
                        if (CassViewGenerator.ProjectMode == "View" || CassViewGenerator.ProjectMode == "Simu")
                        {
                            if (element.EnumValue != "" && element.ValueType != "COM") //串口 2014.1
                            {
                                newValue.InnerText = new List<string>(element.EnumValue.Split(',')).IndexOf(element.TheValue.ToString()).ToString();
                            }
                            else
                            {
                                if (element.ValueType == "System.Boolean")
                                {//将布尔型字符格式转换成0.1的字符格式写入XML20090609
                                    newValue.InnerText = Convert.ToInt16(Convert.ToBoolean(element.TheValue)).ToString();
                                }
                                else if (element.ValueType == "COM")  //串口 2014.1   将参数变成字符串
                                {
                                    newValue.InnerText = "\"" + element.TheValue.ToString() + "\"";
                                }
                                else
                                {
                                    newValue.InnerText = element.TheValue.ToString();
                                }
                            }
                        }
                        else
                        { newValue.InnerText = element.TheValue.ToString(); }
                        newothers.AppendChild(newValue);
                    }
                }
            }
            if (ctrlinfo.UnvisibleFunctionProperty != null)  //20140224
            {
                //不可见属性的不显示属性节点
                foreach (XProp element in ctrlinfo.UnvisibleFunctionProperty)
                {
                    if (!tempList.Contains(element.VarName))
                    {
                        XmlElement newValue = document.CreateElement("item");
                        newValue.SetAttribute("name", element.VarName);
                        if (element.Opnode != null && element.Opnode != "")
                        {//others中的属性 如有优化节点值则加入参数的 优化属性中
                            newValue.SetAttribute("optimise", element.Opnode);
                        }
                        if (element.EnumValue != "" && !element.TheValue.ToString().Contains("array"))
                        {//排除数组情况
                            newValue.InnerText = new List<string>(element.EnumValue.Split(',')).IndexOf(element.TheValue.ToString()).ToString();
                        }
                        else
                        {
                            newValue.InnerText = element.TheValue.ToString();
                        }
                        newothers.AppendChild(newValue);
                    }
                }
            }
            newParameter.AppendChild(newothers);

            return newParameter;
        }

        /// <summary>
        /// 创建策略节点列表
        /// </summary>
        /// <param name="IOList">生成指令表</param>
        /// <param name="TempInfo">全局临时变量</param>
        /// <returns>返回策略节点列表</returns>
        private List<XmlElement> CreateTacticNode(List<string[]> IOList, List<string> TempInfo, List<ControlInfo> CtrlsInfo)
        {
            List<XmlElement> tactics = new List<XmlElement>();
            List<XmlElement> loops = new List<XmlElement>();
            List<string> MainList = new List<string>();
            List<List<string>> PageList = new List<List<string>>();


            XmlElement newTactic = document.CreateElement("tactic");//赋值无用 防止出错

            //newTactic = document.CreateElement("Main");

            foreach (string[] IOrow in IOList)
            {
                if (IOrow != null && IOrow[0].EndsWith(":"))
                {
                    string MarkName = IOrow[0].TrimEnd(':');
                    string PageName = GenerateCode.getMark(MarkName);
                    int PackNum = GenerateCode.getNum(MarkName);
                    if (PageName == MarkName)
                    {//标示符末尾不带数字则为页面主块
                        OrderMainNode(ref MainList, MarkName);
                        PageList.Add(new List<string>(new string[] { MarkName }));
                    }
                    else
                    {
                        for (int i = 0; i < PageList.Count; i++)
                        {
                            if (PageList[i][0] == PageName)
                            {
                                if (PageList[i].Count == 1 || GenerateCode.getNum(PageList[i][PageList[i].Count - 1]) < PackNum)
                                {
                                    PageList[i].Add(MarkName);
                                }
                                else
                                {
                                    for (int j = 1; j < PageList[i].Count; j++)
                                    {
                                        if (GenerateCode.getNum(PageList[i][j]) > PackNum)
                                        {
                                            PageList[i].Insert(j, MarkName);
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    //if (IOrow[0] == "main:" || IOrow[0] == "Main:" || IOrow[0] == "MAIN:")
                    //{
                    //    newTactic = document.CreateElement("Main");
                    //}
                    //else
                    //{
                    //    if (newTactic.Name == "Main")
                    //    {//main节点末尾加上所有的控件块名
                    //        for (int Pindex = 1; Pindex < GenerateCode.PackInfos.Count; Pindex++)
                    //        {//序号从1开始 去除Main的控件串号
                    //            XmlElement newInstruction = document.CreateElement("instruction");
                    //            newInstruction.SetAttribute("name", Mid + GenerateCode.PackInfos[Pindex][0] + Sid);
                    //            newTactic.AppendChild(newInstruction);
                    //        }
                    //    }
                    if (newTactic.ChildNodes.Count != 0)
                    {
                        loops.Add(newTactic);
                    }
                    newTactic = document.CreateElement("Function");
                    newTactic.SetAttribute("name", Mid + MarkName.TrimStart('_') + Lid);
                    //}
                }
                else if (IOrow != null)
                {
                    XmlElement newInstruction = document.CreateElement("instruction");
                    XmlElement newParam = document.CreateElement("param");
                    string temp = null;
                    if ((IOrow[0] == "LD" || IOrow[0] == "ST"))
                    {//临时变量行或参数都一样
                        newInstruction.SetAttribute("name", IOrow[0] + "Control");
                        newParam.InnerText = "&" + Aid + IOrow[1];
                        newInstruction.AppendChild(newParam);
                        if (IOrow[0] == "ST" && IOrow[1].Contains("."))
                        {//用于优化参数时用的数组
                            STlist.Add(IOrow[1]);
                        }
                    }
                    else//逻辑控件有错！！！！！！！！！！！！ 要加configuration！！！！
                    {
                        newInstruction.SetAttribute("name", IOrow[0] + "Control");
                        if (new List<string>(new string[] { "CALCU", "PROCESS" }).Contains(IOrow[0]))
                        {//条件动作表和计算器组态节点name名不同
                            newInstruction.Attributes["name"].Value = Mid + IOrow[2] + "Control";
                        }
                        if (IOrow.Length == 3)
                        {
                            temp = IOrow[2];
                            if (!StructName.Contains(temp))
                            { temp = GetModuleNum(IOrow[0], Convert.ToInt32(IOrow[2])); }
                            //if (StructName.Contains(temp))
                            { temp = "&" + Aid + temp; }
                        }
                        else if (IOrow[1] != null)
                        {
                            temp = IOrow[1];
                            if (new List<string>(new string[] { "JMP", "CALL" }).Contains(IOrow[0]))
                            {//JMP和CALL控件对应参数加上头尾字符
                                temp = Mid + temp + Lid;
                            }
                            if (IOrow[0] == "CON" && IOrow[1].ToUpper() == "NULL")
                            {//出现常数指令加NULL则跳过不加入XML文件中
                                continue;
                            }
                        }
                        if (temp != null)
                        {
                            newParam.InnerText = temp;
                            newInstruction.AppendChild(newParam);
                        }
                        //if (IOrow[1] != null)
                        //{
                        //    temp = IOrow[1];
                        //    if (StructName.Contains(IOrow[1]))
                        //    { temp = "&" + Aid + temp; }
                        //    else if (new List<string>(new string[] { "JMP", "CALL" }).Contains(IOrow[0]))
                        //    {//JMP和CALL控件对应参数加上头尾字符
                        //        temp = Mid + temp + Sid;
                        //    }
                        //    newParam.InnerText = temp;
                        //    newInstruction.AppendChild(newParam);
                        //}
                        //else if (IOrow.Length == 3)
                        //{//IOrow长度为3则为模块或逻辑控件 第一位为类型，第三位为对应的控件序号
                        //    temp = GetModuleNum(IOrow[0], Convert.ToInt32(IOrow[2]));
                        //    if (StructName.Contains(temp))
                        //    { temp = "&" + Aid + temp; }
                        //    newParam.InnerText = temp;
                        //    newInstruction.AppendChild(newParam);
                        //}
                    }
                    newTactic.AppendChild(newInstruction);
                }
            } 
            loops.Add(newTactic);
            tactics.AddRange(loops); //2013.11.22

            XmlElement MainNode = document.CreateElement("Main");
            foreach (string page in MainList)
            {
                XmlElement newpage = document.CreateElement("instruction");
                newpage.SetAttribute("name", Mid + page.TrimStart('_') + Pid);
                MainNode.AppendChild(newpage);
            }
            tactics.Add(MainNode);
            foreach (List<string> page in PageList)
            {
                XmlElement newpage = document.CreateElement("Function");
                newpage.SetAttribute("name", Mid + page[0].TrimStart('_') + Pid);
                foreach (string loop in page)
                {
                    XmlElement newloop = document.CreateElement("instruction");
                    newloop.SetAttribute("name", Mid + loop.TrimStart('_') + Lid);
                    newpage.AppendChild(newloop);
                }
                tactics.Add(newpage);
            }

            
            return tactics;
        }

        /// <summary>
        /// 按节点父子顺序和字母顺序进行排序
        /// </summary>
        /// <param name="MainList"></param>
        /// <param name="MarkName"></param>
        private void OrderMainNode(ref List<string> MainList, string MarkName)
        {
            string[] level = MarkName.Split('_');
            if (MainList.Count == 0)
            {
                MainList.Add(MarkName);
            }
            else
            {
                if (MarkName == "MAIN")
                {
                    MainList.Insert(0, MarkName);
                }
                else
                {
                    for (int k = 0; k < MainList.Count; k++)
                    {
                        if (MainList[k] != "MAIN")
                        {
                            bool findpoint = false ;
                            string[] tempLevel = MainList[k].Split('_');
                            for (int x = 1; x < tempLevel.Length; x++)
                            {
                                if (compareA(level[x], tempLevel[x]))
                                {
                                    findpoint = true;
                                    break;
                                }
                                else if (compareA(tempLevel[x], level[x]))
                                {
                                    findpoint = false;
                                    break;
                                }
                                if (x + 1 == tempLevel.Length && tempLevel.Length < level.Length)
                                {
                                    findpoint = false;
                                    break;
                                }
                            }
                            if (findpoint)
                            {
                                MainList.Insert(k, MarkName);
                                return;
                            }
                        }
                        if (k + 1 == MainList.Count)
                        {
                            MainList.Add(MarkName);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 比较两个页面字母的大小
        /// a小则返回true，否则返回false
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns> a小则返回true，否则返回false</returns>
        static public bool compareA(string a, string b)
        {
            if (a.Length < b.Length)
            {
                return true;
            }
            else if (a.Length > b.Length)
            {
                return false;
            }
            else
            {
                if (a.Length == 1)
                {
                    if (b.ToUpper()[0] - a.ToUpper()[0] > 0)
                    {
                        return true;
                    }
                    return false;
                }
                else //a.Length==2
                {
                    if (b.ToUpper()[0] - a.ToUpper()[0] > 0 || (b.ToUpper()[0] == a.ToUpper()[0] && b.ToUpper()[1] - a.ToUpper()[1] > 0))
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// XSLT转换XML到Main.c文件
        /// </summary>
        /// <param name="xmlfile">XML文件路径</param>
        /// <param name="xslfile">XSLT文件路径</param>
        /// <param name="desfile">main.C文件路径</param>
        public static void GenSrcFile(string xmlfile, string xslfile, string desfile)
        {
            FileStream fs = null;
            StreamWriter sw = null;

            XPathDocument doc = null;
            XslCompiledTransform transform = null;

            try
            {
                doc = new XPathDocument(xmlfile);
                transform = new XslCompiledTransform();
                transform.Load(xslfile);
            }
            catch (Exception ex)
            {

            }
            try
            {
                fs = new FileStream(desfile, FileMode.Create);
                sw = new StreamWriter(fs, Encoding.Default);
            }
            catch (Exception ex)
            {

            }
            if (transform != null && fs != null)
            {
                XPathNavigator nav = doc.CreateNavigator();
                transform.Transform(nav, null, sw);
            }
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 获取特殊控件的数组属性
        /// </summary>
        /// <param name="CassviewList"></param>
        public List<XmlNode> GetSpecialCtrlInfo(ArrayList CassviewList)
        {
            GenerateCode.UnseeArray = new List<string[]>();//初始化静态队列 用户不可见数组
            document = new XmlDocument();//初始化XML文档 与生成的工程XML共同即与方法CreateProjectXML使用统一文档
            int Count = 0;//初始化数组个数计数
            SpeicalNodes = new List<List<XmlNode>>();//初始化特殊控件节点
            List<XmlNode> CalculNodes = new List<XmlNode>();
            List<XmlNode> ProcesNodes = new List<XmlNode>();
            List<XmlNode> ArrayNodes = new List<XmlNode>();
            foreach (CassViewGenerator.ListObject node in CassviewList)
            {
                CassView cassView = (CassView)(node.tabPage.Controls[0].GetNextControl(node.tabPage.Controls[0], false));
                foreach (Control element in cassView.Controls)
                {
                    PropertyDescriptor MS = TypeDescriptor.GetProperties(element)["ModuleSort"];
                    if (SpeicalCtrls.Contains(MS.GetValue(element).ToString()))
                    {
                        int index = SpeicalCtrls.IndexOf(MS.GetValue(element).ToString());
                        if (index == 0)//模糊
                        {
                            ArrayNodes.Add(CreateArrayNode(GenerateCode.GetFuzzyArray(cassView, element, ref Count)));
                        }
                        else if (index == 1)//解耦
                        {
                            //ArrayNodes.AddRange(CreateJieOu(cassView, element));
                            List<string[]> tempInfos = GenerateCode.GetJieOuArray(cassView, element, ref Count);
                            foreach (string[] info in tempInfos)
                            {
                                ArrayNodes.Add(CreateArrayNode(info));
                            }
                        }
                        else if (index == 2)//条件
                        {
                            ProcesNodes.Add(CreateProcess(cassView, element));
                        }
                        else if (index == 3)//计算器
                        {
                            CalculNodes.Add(CreateCalculator(cassView, element));
                        }
                    }
                }
            }
            SpeicalNodes.Add(CalculNodes);
            SpeicalNodes.Add(ProcesNodes);
            SpeicalNodes.Add(ArrayNodes);
            return ArrayNodes;
        }

        public List<XmlNode> GetSpecialCtrlInfo(Dictionary<string, string[]> arrayInfo)
        {
            SpeicalNodes = new List<List<XmlNode>>();//初始化特殊控件节点
            List<XmlNode> ArrayNodes = new List<XmlNode>();
            foreach (string[] element in arrayInfo.Values)
            {
                ArrayNodes.Add(CreateArrayNode(element));
            }
            SpeicalNodes.Add(new List<XmlNode>());
            SpeicalNodes.Add(new List<XmlNode>());
            SpeicalNodes.Add(ArrayNodes);
            return ArrayNodes;
        }

        /// <summary>
        /// 创建计算器组态节点
        /// </summary>
        /// <param name="curCas">控件所在CASSVIEW容器</param>
        /// <param name="curCtrl">控件</param>
        /// <returns>节点</returns>
        private XmlNode CreateCalculator(CassView curCas, Control curCtrl)
        {
            PropertyDescriptor configurationProperty = TypeDescriptor.GetProperties(curCtrl)["Configuration"];
            ControlTactic.SpecialControl.Calculator ConvertTool = new ControlTactic.SpecialControl.Calculator();
            List<List<string>> tempvalue = (List<List<string>>)configurationProperty.GetValue(curCtrl);
            XmlElement Calculator = document.CreateElement("Calculator");
            ControlInfo currentCtrl = curCas.FindControlInfo(curCtrl);
            Calculator.SetAttribute("name", currentCtrl.CodeInfo[2]);

            if (tempvalue.Count != 0)
            {
               
                Calculator.SetAttribute("name", currentCtrl.CodeInfo[2]);
                foreach (List<string> express in tempvalue)
                {
                    if (express[0] != null || express[1] != null || express[2] != null)
                    {
                        XmlElement newItem = document.CreateElement("item");
                        if (express[0] != null)
                        {
                            XmlElement condition = document.CreateElement("condition");
                            condition.InnerText = ChangeExpress(express[0], currentCtrl.CodeInfo[2]);
                            newItem.AppendChild(condition);
                        }
                        if (express[1] != null && express[2] != null)
                        {
                            XmlElement expression = document.CreateElement("expression");

                            expression.InnerText = ChangeExpress(express[2] + "=" + express[1], currentCtrl.CodeInfo[2]);
                            newItem.AppendChild(expression);
                        }
                        Calculator.AppendChild(newItem);
                    }
                }
            }
            else
            {
 
            }
            return Calculator;
        }

        /// <summary>
        /// 把计算器组态中的中间变量显示时改成portname.fm[0]
        /// </summary>
        /// <param name="changeValue">待改变的表达式</param>
        /// <param name="PortName">计算器portname</param>
        /// <returns></returns>
        private string ChangeExpress(string changeValue, string PortName)
        {
            //List<string> Mvalue = new List<string>(new string[] { "M0", "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8", "M9" });
            string returnString = null;
            for (int i = 0; i < changeValue.Length; i++)
            {
                if (changeValue[i] == 'M' && i + 1 < changeValue.Length && changeValue[i + 1] >= '0' && changeValue[i + 1] <= '9')
                {
                    i++;
                    returnString += Aid + PortName + ".fM[" + changeValue[i] + "]";
                }
                else
                {
                    returnString += changeValue[i];
                }
            }
            return returnString;
        }

        /// <summary>
        /// 由数组信息生成节点
        /// </summary>
        /// <param name="ArrayInfo"></param>
        /// <returns></returns>
        private XmlNode CreateArrayNode(string[] ArrayInfo)
        {//ArrayInfo顺序依次为控件PortName、属性VarName、类型、数组名、长度、数组
            XmlElement newItem = document.CreateElement("item");
            newItem.SetAttribute("name", ArrayInfo[3]);
            newItem.SetAttribute("type", ArrayInfo[2]);
            newItem.SetAttribute("dimension", ArrayInfo[4]);
            newItem.InnerText = "{" + ArrayInfo[5] + "}";
            return newItem;
        }

        /// <summary>
        /// 创建条件动作表节点
        /// </summary>
        /// <param name="curCas">控件所在CASSVIEW容器</param>
        /// <param name="curCtrl">控件</param>
        /// <returns>节点</returns>
        private XmlNode CreateProcess(CassView curCas, Control curCtrl)
        {
            PropertyDescriptor configurationProperty = TypeDescriptor.GetProperties(curCtrl)["Configuration"];
            ControlTactic.SpecialControl.Process ConvertTool = new ControlTactic.SpecialControl.Process();
            ControlTactic.SpecialControl.ProcessStruct tempStruct = ConvertTool.ListToStruct((List<string>)configurationProperty.GetValue(curCtrl));
            tempStruct = AddID(tempStruct);
            XmlElement Process = document.CreateElement("ConditionAction");

            ControlInfo currentCtrl = curCas.FindControlInfo(curCtrl);
            Process.SetAttribute("name", currentCtrl.CodeInfo[2]);

            if (tempStruct.IsOnlyStart)
            {
                for (int i = 0; i < tempStruct.NumOfConditions; i++)
                {//添加所有条件信息到条件数组                   
                    if (tempStruct.Conditions[i] == "")
                    { continue; }
                    XmlElement newItem = document.CreateElement("item");
                    XmlElement newCondition = document.CreateElement("condition");
                    newCondition.InnerText = tempStruct.Conditions[i];
                    newItem.AppendChild(newCondition);

                    XmlElement newExpression = document.CreateElement("expression");
                    newExpression.InnerText = "condition[" + i.ToString() + "] = 1";
                    newItem.AppendChild(newExpression);
                    Process.AppendChild(newItem);
                }
            }
            for (int Aindex = 0; Aindex < tempStruct.NumOfActions; Aindex++)
            {
                XmlElement newItem = document.CreateElement("item");
                if (tempStruct.Actions[Aindex][0] == ""
                    || (tempStruct.Actions[Aindex][1] == "" && tempStruct.Actions[Aindex][2] == ""))
                {//动作为空或则动作的表达式为空
                    continue;
                }
                for (int Cindex = 0; Cindex < tempStruct.NumOfConditions; Cindex++)
                {
                    string Condition = null;
                    if (tempStruct.OrderBox[Cindex, Aindex] == "" || tempStruct.Conditions[Cindex] == "")
                    {//如果顺序表关系为空 或对应的条件为空则跳过
                        continue;
                    }
                    else if (tempStruct.OrderBox[Aindex, Cindex] == "N")
                    {//非关系加上！
                        Condition = "!";
                    }
                    if (tempStruct.IsOnlyStart)
                    {//根据条件动作设定判断显示条件数组还是表达式
                        Condition += "condition[" + Cindex.ToString() + "]";
                    }
                    else
                    {
                        Condition += "(" + tempStruct.Conditions[Cindex] + ")";
                    }
                    XmlElement newCondition = document.CreateElement("condition");
                    newCondition.InnerText = Condition;
                    newItem.AppendChild(newCondition);
                }
                if (newItem.ChildNodes.Count == 0)
                {//如果没有条件则默认为1 添加条件"1"条件子节点
                    XmlElement newCondition = document.CreateElement("condition");
                    newCondition.InnerText = "1";
                    newItem.AppendChild(newCondition);
                }
                XmlElement newExpression = document.CreateElement("expression");
                if (tempStruct.Actions[Aindex][0] == "Evaluate")
                {
                    newExpression = document.CreateElement("expression");
                    newExpression.InnerText = tempStruct.Actions[Aindex][1] + "=" + tempStruct.Actions[Aindex][2];
                }
                else if (tempStruct.Actions[Aindex][0] == "Transfer")
                {
                    newExpression = document.CreateElement("function");
                    newExpression.InnerText = Mid + tempStruct.Actions[Aindex][1] + Sid;
                }
                newItem.AppendChild(newExpression);
                Process.AppendChild(newItem);
            }
            return Process;
        }

        /// <summary>
        /// 在条件和动作表达式中的功能块名前加上标识符
        /// </summary>
        private ControlTactic.SpecialControl.ProcessStruct AddID(ControlTactic.SpecialControl.ProcessStruct ProStruct)
        {
            for (int ctrlcount = 0; ctrlcount < ProStruct.ControlAttribute.Count; ctrlcount++)
            {
                for (int i = 0; i < ProStruct.Conditions.Count; i++)
                {
                    ProStruct.Conditions[i] = AddIDpoint(ProStruct.ControlAttribute[ctrlcount][0], ProStruct.Conditions[i]);
                }
                for (int i = 0; i < ProStruct.Actions.Count; i++)
                {
                    if (ProStruct.Actions[i][0] == "Evaluate")
                    {
                        ProStruct.Actions[i][1] = AddIDpoint(ProStruct.ControlAttribute[ctrlcount][0], ProStruct.Actions[i][1]);
                        ProStruct.Actions[i][2] = AddIDpoint(ProStruct.ControlAttribute[ctrlcount][0], ProStruct.Actions[i][2]);
                    }
                }
            }
            return ProStruct;
        }

        /// <summary>
        /// 在表达式中找到目标名并在该名前加入标示符
        /// </summary>
        /// <param name="portname"></param>
        /// <param name="Exp"></param>
        /// <returns></returns>
        private string AddIDpoint(string portname, string Exp)
        {
            if (Exp != null)
            {
                Stack<int> Addpoints = new Stack<int>();
                for (int i = 0; i < Exp.Length; i++)
                {
                    if (i + portname.Length < Exp.Length && Exp.Substring(i, portname.Length) == portname)
                    {
                        Addpoints.Push(i);
                    }
                }
                while (Addpoints.Count != 0)
                {
                    Exp = Exp.Insert(Addpoints.Pop(), Aid.ToString());
                }
            }
            return Exp;
        }


        /// <summary>
        /// 对管理模块类序号的数组进行查找返回添加操作，仅用于XML文件生成时
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        private string GetModuleNum(string ModuleType, int CtrlNum)
        {
            if (ModuleCount.Count != 0)
            {
                foreach (ArrayList element in ModuleCount)
                {
                    if ((string)(element[0]) == ModuleType)
                    {//第0位放控件类型 如ADD DIV 第二位放该类的控件排序队列
                        List<int> tempList = (List<int>)element[1];
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            if (tempList[i] == CtrlNum)
                            { //找到则返回对应的序号
                                return element[0] + i.ToString();
                            }
                        }
                        //找不到则添加新序号
                        ((List<int>)element[1]).Add(CtrlNum);
                        return element[0] + (((List<int>)element[1]).Count - 1).ToString();
                    }
                }
            }
            //找不到类型则添加新类型
            ArrayList newModule = new ArrayList();
            newModule.Add(ModuleType);
            newModule.Add(new List<int>(new int[] { CtrlNum }));
            ModuleCount.Add(newModule);
            return ModuleType + "0";
        }
    }
}
