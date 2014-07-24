/*******************************************************************************
           ** Copyright (C) 2009 CASS ��Ȩ����
           ** �ļ�����CassView.cs 
           ** ����������
           **          ����������λ������Ҫ��XML�ļ���Main.C�ļ�
           ** ���ߣ����罡
           ** ��ʼʱ�䣺2009-5-15
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
        private List<string> StructName = new List<string>();//���õ��Ŀؼ��ṹ��
        private List<string> SpeicalCtrls = new List<string>(new string[] { "ģ��������", "�����������", "����������", "��������̬" });
        private List<List<XmlNode>> SpeicalNodes = new List<List<XmlNode>>();//����ؼ���ȡ���������ɵĽڵ�        
        private List<ArrayList> ModuleCount = new List<ArrayList>();//���ڼ�¼ģ�����Ϳؼ��Ľṹ�����
        private List<string> Dsign = new List<string>(new string[] { "==", ">", ">=", "<", "<=" });//�Ż���ϵ����
        private List<string> Ssign = new List<string>(new string[] { "", "GT", "GE", "LT", "LE" });//�Ż���ϵ���ż��
        private List<string> UseOpt =new List<string> ();//����XMLʱ�õ��Ĳ��ظ��ؼ�¼�����ù����Ż��ؼ����Ż�ָ��

        private List<string> STlist = new List<string>();//�������ST�ؼ�.��������Ϣ���Ż�����
        static private List<string[]> UnseeArray = new List<string[]>();//��Ų���ʾ���û���������Ϣ//��ʱ���ڽ��������20090617

        string proName = null;
        string proInfo = null;
        string proIndex = null;
        string savePath = null;
        string Aid = null;//���Ա�ʾ�� Сдc configuration + proNum + _
        string Mid = null;//ģ���ʾ�� ��дC Configuration + proNum + _
        string Sid = "Sub";//���Ա�ʶ�� ��дSub
        string Pid = "Page";
        string Lid = "Loop";

        [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]  // �ṹ�����ڽ�����32λ��ת����4���ֽ�
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
        /// <param name="savePath">����·��</param>
        /// <param name="ProInfo">������Ϣ</param>
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
        /// ��������XML�ļ�,ת���ɡ�Main.c",����"Configuration_ControlFuns.c"
        /// </summary>
        /// <param name="TempInfo">ȫ����ʱ������Ϣ</param>
        /// <param name="addressInfo">��ַ��Ϣ</param>
        /// <param name="ctrllist">���̿ؼ�����</param>
        /// <param name="IOList">����ָ�����Ϣ</param>
        public void CreateProjectMainC(List<string> TempInfo, List<string[]> addressInfo, List<ControlInfo> ctrllist, List<string[]> IOList, bool flag)
        {
            XmlElement DCS = document.CreateElement("DCS");
            //���ڽڵ�
            XmlElement Cycle = document.CreateElement("ProjectInfo");
            Cycle.InnerText = this.proInfo;
            DCS.AppendChild(Cycle);
            //������Žڵ�
            XmlElement Pindex = document.CreateElement("ProjectIndex");
            Pindex.InnerText = this.proIndex;
            DCS.AppendChild(Pindex);
            //�������ڵ�
            XmlElement Cal = document.CreateElement("Calculators");
            if (SpeicalNodes.Count != 0)
            {
                foreach (XmlNode child in SpeicalNodes[0])
                { //����ؼ��е�һ����Ϊ��������̬                
                    Cal.AppendChild(child);
                }
            }
            DCS.AppendChild(Cal);


            //����������ڵ�
            XmlElement CA = document.CreateElement("ConditionActions");
            if (SpeicalNodes.Count > 1)
            {
                foreach (XmlNode child in SpeicalNodes[1])
                {//����ؼ��еڶ�����Ϊ��������̬
                    CA.AppendChild(child);
                }

            }
            DCS.AppendChild(CA);

            //ȫ����ʱ����
            XmlElement tempValue = document.CreateElement("EntireVarRegion");
            foreach (string temp in TempInfo)
            {
                XmlElement tempItem = document.CreateElement("item");
                tempItem.SetAttribute("type", "fp32");
                tempItem.InnerText = Aid + temp;
                tempValue.AppendChild(tempItem);
            }
            DCS.AppendChild(tempValue);
            //������ڵ�
            List<string> UseControl = new List<string>();//�����������õ��Ŀؼ�����
            XmlElement Parameters = document.CreateElement("Parameters");

            //���Ȳ����������ݽڵ�
            XmlElement Arrays = document.CreateElement("Arrays");
            bool canAdd = true;//�Ƿ���Ҫ��Ӷ�Ӧ������ڵ�
            if (SpeicalNodes.Count > 2)
            {
                foreach (XmlNode child in SpeicalNodes[2])
                {//����ؼ��е�������Ϊ��������
                    canAdd = true;
                    for (int i = 0; i < addressInfo.Count; i++)
                    {
                        if (addressInfo[i].Length == 8
                            && child.Attributes[1].Value == addressInfo[i][2]
                            && child.InnerText == "{" + addressInfo[i][7] + "}")
                        {//�������ԣ���Ϊ�޸ĵĲ��ɼ����������Array�ڵ�
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
            {//��������Ӳ��ɼ�������Ϣ
                Arrays.AppendChild(CreateArrayNode(child));
            }
            Parameters.AppendChild(Arrays);

            #region     ControlFuns
         
            #region  ���� configuration_ControlFuns.c  �ļ�  
                        //2013.11.20  by wtt
            string saveFile_ControlFunsC = this.savePath + "\\configuration_ControlFuns.c";
            if (File.Exists(saveFile_ControlFunsC))
                File.Delete(saveFile_ControlFunsC);

            FileStream  fs_ControlFuns = new FileStream(saveFile_ControlFunsC,FileMode.Create,FileAccess.Write);
            StreamWriter sw_ControlFuns = new StreamWriter(fs_ControlFuns,Encoding.UTF8);
            #endregion

            XmlDocument tempD = new XmlDocument();
            XmlElement ControlFuns = tempD.CreateElement("ControlFuns");
            StructName = new List<string>();//��տؼ��ṹ��
            ModuleCount = new List<ArrayList>();//���ģ�������
            foreach (ControlInfo ctrl in ctrllist)
            {
                if (ctrl.CodeInfo != null && !GenerateCode.SortCtrlName.Contains(ctrl.CodeInfo[0])/*&&!ctrl.CodeInfo[0].Contains("�ļ����")*/)//2013.11.26
                {//��������1��Ϊ�㷨�ؼ� ����2�ų���������ؼ���1��2λ����Ŀ��ؼ�����������
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
                {//���ָ����ϢXML�ڵ㣬�ڵ�����Ϊ���Ĳ���Control20090609
                    XmlElement UseType = tempD.CreateElement("item");
                    UseType.SetAttribute("name", ctrl.CodeInfo[0]);
                    if (ctrl.OtherProperty != null)
                    {
                        UseType.InnerText = ctrl.OtherProperty[0];//* ��ȡģ�麯�������
                        sw_ControlFuns.WriteLine(ctrl.OtherProperty[0]);   //* 2013.11.20  by  wtt

                    }
                    ControlFuns.AppendChild(UseType);
                    UseControl.Add(ctrl.CodeInfo[0]);
                    if (ctrl.OtherProperty != null && ctrl.OtherProperty.Length == 2 && ctrl.OtherProperty[1] != null && ctrl.OtherProperty[1] != "")
                    {//��¼�ؼ��Ż���Ϣ
                        UseOpt.Add(ctrl.OtherProperty[1]);
                    }
                }
            }
            //2013.11.20  �ر��ļ���  by wtt
            sw_ControlFuns.Close();
            fs_ControlFuns.Close();
            #endregion

            //����ָ�������Ϣ�������CodeCtrlName�б��е�ָ����Ϣ
            foreach (string[] IOrow in IOList)
            {
                if (IOrow != null)
                {//����ж���ؼ���Ϣ���ʱ�����Ӧ�Ŀؼ�ָ����Ϣ
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
                                    && categoryNode.Attributes[0].InnerText == "����")
                                {
                                    foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                                    {
                                        if (toolItemNode.Attributes[0].InnerText.Split(',')[2] == tempName
                                            && (toolItemNode.LastChild.Name == "CodeProperty" || toolItemNode.LastChild.Name == "OtherInfo")
                                            && toolItemNode.LastChild.ChildNodes.Count > 0)
                                        {
                                            foreach (XmlNode element in toolItemNode.LastChild.ChildNodes)
                                            {//�����ڵ㲻����ؼ���Ϣ��
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
                                {//�ҵ�����ָ�������õ��Ĵ���
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
            //���Խڵ�
            List<XmlElement> Tactics = CreateTacticNode(IOList, TempInfo, ctrllist);
            foreach (XmlElement tactic in Tactics)
            {
                DCS.AppendChild(tactic);
            }
            document.AppendChild(DCS);
            document.Save(this.savePath + "\\" + this.proName + ".xml");

            //����Configuration_ControlFuns.xml
            tempD.AppendChild(ControlFuns);
            tempD.Save(this.savePath + "\\" + "Configuration_ControlFuns.xml");
            //�����λ�����ɱ�׼C��main����
            if (flag)
                GenSrcFile(this.savePath + "\\" + this.proName + ".xml", CassViewGenerator.designerPath + "\\XSLTParam_PC.xslt", this.savePath + "\\main.c");
            else
            {
                GenSrcFile(this.savePath + "\\" + this.proName + ".xml", CassViewGenerator.designerPath + "\\XSLTParam.xslt", this.savePath + "\\main.c");
                #region   ����cass_mv_main.c
                //�ϲ��ļ�����cass_mv_main.c        by wtt 2013.12.27
                CreateCassMVMainCFile(this.savePath+"\\cass_mv_main.c");
                //ɾ���м��ļ�
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

        #region    Ƕ��ʽ�汾��֧��Cass�Ļ����Ӿ��ļ�   2013.12.27
        //cass�汾��Ҫ�ϲ����м��ļ�
        private string[] combineFileList = { "//configuration_ControlFuns.c", "//feather.h" };
        private string[] delFileList = { "//configuration_ControlFuns.c", "//main.c" };
     //   private string[] headFileList = { "configuration_address.h", "configuration_system.h", "configuration_control.h" };
        /// <summary>
        /// ����֧��Cass�汾�Ļ����Ӿ�main�ļ�
       /// </summary>
      /// <param name="FilePath">�ļ�·��</param>
        public void CreateCassMVMainCFile(string FilePath )
        {
            try
            {
                //�½�Ŀ���ļ���
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

                //���3��ͷ�ļ�   2014.1.6
                //��ȡcass_h��Ŀ¼
                DirectoryInfo dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                string cass_hPath = dirInfo + "cass_h";
                //��ȡͷ�ļ�
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
                
                //��ȡmain.c�е�����
                ArrayList maincWriteValue = FileOperator.ReadFromFile(this.savePath+"\\main.c",Encoding.GetEncoding("gb2312"));
                //һ��һ�б�������ѯ�����ַ���
                foreach (string lineData in maincWriteValue)
                {
                    if (lineData.Contains("//"))
                    {
                        //�Ƴ�����ǰ���ͺ󵼵Ŀհ��ַ�
                        string temp = lineData.Trim();
                        //�ж��Ƿ�����ڡ��ϲ��ļ��б���
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
        /// �ж�ĳ�ַ����Ƿ������ĳ���ַ��������У������ڣ�����true������false
        /// </summary>
        /// <param name="obj">����ѯ���ַ���</param>
        /// <param name="myList">�ַ�������</param>
        /// <returns>����ֵ</returns>
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
        /// ��������XML�ļ�,ת����Main.c,����Configuration_ControlFuns.xml
        /// </summary>
        /// <param name="TempInfo">ȫ����ʱ������Ϣ</param>
        /// <param name="addressInfo">��ַ��Ϣ</param>
        /// <param name="ctrllist">���̿ؼ�����</param>
        /// <param name="IOList">����ָ�����Ϣ</param>
        /// ԭ���Ĵ���xml�ӿڵĺ������Ӿ�ƽ̨����c�ļ�
        public void CreateProjectXML_old(List<string> TempInfo, List<string[]> addressInfo, List<ControlInfo> ctrllist, List<string[]> IOList)
        {
            XmlElement DCS = document.CreateElement("DCS");
            //���ڽڵ�
            XmlElement Cycle = document.CreateElement("ProjectInfo");
            Cycle.InnerText = this.proInfo;
            DCS.AppendChild(Cycle);
            //������Žڵ�
            XmlElement Pindex = document.CreateElement("ProjectIndex");
            Pindex.InnerText = this.proIndex;
            DCS.AppendChild(Pindex);
            //�������ڵ�
            XmlElement Cal = document.CreateElement("Calculators");
            if (SpeicalNodes.Count != 0)
            {
                foreach (XmlNode child in SpeicalNodes[0])
                { //����ؼ��е�һ����Ϊ��������̬                
                    Cal.AppendChild(child);
                }
            }
                DCS.AppendChild(Cal);
            
        
            //����������ڵ�
            XmlElement CA = document.CreateElement("ConditionActions");
            if (SpeicalNodes.Count > 1)
            {
                foreach (XmlNode child in SpeicalNodes[1])
                {//����ؼ��еڶ�����Ϊ��������̬
                    CA.AppendChild(child);
                }
               
            }
                DCS.AppendChild(CA);
                       
            //ȫ����ʱ����
            XmlElement tempValue = document.CreateElement("EntireVarRegion");
            foreach (string temp in TempInfo)
            {
                XmlElement tempItem = document.CreateElement("item");
                tempItem.SetAttribute("type", "fp32");
                tempItem.InnerText = Aid + temp;
                tempValue.AppendChild(tempItem);
            }
            DCS.AppendChild(tempValue);
            //������ڵ�
            List<string> UseControl = new List<string>();//�����������õ��Ŀؼ�����
            XmlElement Parameters = document.CreateElement("Parameters");

            //���Ȳ����������ݽڵ�
            XmlElement Arrays = document.CreateElement("Arrays");
            bool canAdd = true;//�Ƿ���Ҫ��Ӷ�Ӧ������ڵ�
            if (SpeicalNodes.Count > 2)
            {
                foreach (XmlNode child in SpeicalNodes[2])
                {//����ؼ��е�������Ϊ��������
                    canAdd = true;
                    for (int i = 0; i < addressInfo.Count; i++)
                    {
                        if (addressInfo[i].Length == 8
                            && child.Attributes[1].Value == addressInfo[i][2]
                            && child.InnerText == "{" + addressInfo[i][7] + "}")
                        {//�������ԣ���Ϊ�޸ĵĲ��ɼ����������Array�ڵ�
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
            {//��������Ӳ��ɼ�������Ϣ
                Arrays.AppendChild(CreateArrayNode(child));
            }
            Parameters.AppendChild(Arrays);

            XmlDocument tempD = new XmlDocument();
            XmlElement ControlFuns = tempD.CreateElement("ControlFuns");
            StructName = new List<string>();//��տؼ��ṹ��
            ModuleCount = new List<ArrayList>();//���ģ�������
            foreach (ControlInfo ctrl in ctrllist)
            {
                if (ctrl.CodeInfo != null && !GenerateCode.SortCtrlName.Contains(ctrl.CodeInfo[0]) /*&& !ctrl.CodeInfo[0].Contains("�ļ����")*/) //2013.11.26
                {//��������1��Ϊ�㷨�ؼ� ����2�ų���������ؼ���1��2λ����Ŀ��ؼ�����������
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
                {//���ָ����ϢXML�ڵ㣬�ڵ�����Ϊ���Ĳ���Control20090609
                    XmlElement UseType = tempD.CreateElement("item");
                    UseType.SetAttribute("name", ctrl.CodeInfo[0]);
                    if (ctrl.OtherProperty != null)
                    {
                        UseType.InnerText = ctrl.OtherProperty[0];
                    }
                    ControlFuns.AppendChild(UseType);
                    UseControl.Add(ctrl.CodeInfo[0]);
                    if (ctrl.OtherProperty != null && ctrl.OtherProperty.Length == 2 && ctrl.OtherProperty[1] != null && ctrl.OtherProperty[1] != "")
                    {//��¼�ؼ��Ż���Ϣ
                        UseOpt.Add(ctrl.OtherProperty[1]);
                    }
                }
            }
        
            //����ָ�������Ϣ�������CodeCtrlName�б��е�ָ����Ϣ
            foreach (string[] IOrow in IOList)
            {
                if (IOrow != null)
                {//����ж���ؼ���Ϣ���ʱ�����Ӧ�Ŀؼ�ָ����Ϣ
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
                                    && categoryNode.Attributes[0].InnerText == "����")
                                {
                                    foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                                    {
                                        if (toolItemNode.Attributes[0].InnerText.Split(',')[2] == tempName
                                            && (toolItemNode.LastChild.Name == "CodeProperty" || toolItemNode.LastChild.Name == "OtherInfo") 
                                            && toolItemNode.LastChild.ChildNodes.Count > 0)
                                        {
                                            foreach (XmlNode element in toolItemNode.LastChild.ChildNodes)
                                            {//�����ڵ㲻����ؼ���Ϣ��
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
                                {//�ҵ�����ָ�������õ��Ĵ���
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
            //���Խڵ�
            List<XmlElement> Tactics = CreateTacticNode(IOList, TempInfo, ctrllist);
            foreach (XmlElement tactic in Tactics)
            {
                DCS.AppendChild(tactic);
            }
            document.AppendChild(DCS);
            document.Save(this.savePath + "\\" + this.proName + ".xml");
            //����xsltģ���ļ����� main.c
           GenSrcFile(this.savePath + "\\" + this.proName + ".xml", CassViewGenerator.designerPath + "\\XSLTParam.xslt", this.savePath + "\\configuration" + this.proIndex + "_main.c");
        
            tempD.AppendChild(ControlFuns);
            tempD.Save(this.savePath + "\\" + "Configuration_ControlFuns.xml");

        }

        /// <summary>
        /// ����Configuration_AddressTable.xml
        /// </summary>
        /// <param name="addressInfo">��ַ��Ϣ</param>
        public void CreateAddressTableXML(List<string[]> addressInfo)
        {
            document = new XmlDocument();
            XmlElement AddressTable = document.CreateElement("AddressTable");
            foreach (string[] element in addressInfo)
            {
                if (element[5] != "����ʾ" && element[4] != "")
                {//�ɼ������ҵ�ַ�ǿ���ӽڵ�
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
        /// ����Configuration_PLC.xml
        /// </summary>
        /// <param name="addressRWsize">�ɶ���д���ݳ���</param>
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
        /// ����Configuration_Datas.xml
        /// </summary>
        /// <param name="addressInfo">��ַ��Ϣ</param>
        /// <param name="ctrllist">���̿ؼ�����</param>
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
                    if (element[5] == "True")//��ַΪ�ɶ���д
                    {//�����ؼ������ҵ���Ӧ�Ŀؼ�
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
                        if (element[2] == "uint8")//bool��char��ͳһΪuint8
                        {
                            for (int i = 0; i < tempValue.Length; i++)
                            {
                                if (tempValue[i] != "0" && tempValue[i] != "1")
                                {
                                    if (attribute.ValueType == "System.Boolean")//����ֵ
                                    {
                                        tempValue[i] = Convert.ToInt16(Convert.ToBoolean(tempValue[i])).ToString();
                                    }
                                    else//�Զ���attribute.ValueType == "MyEnum"
                                    {
                                        List<string> temp = new List<string>(attribute.EnumValue.Split(','));
                                        tempValue[i] = temp.IndexOf(tempValue[i]).ToString();
                                    }
                                }
                                //numberString.Add("0x" + tempValue[i]);
                                address[Convert.ToInt32(element[4]) + i] = "0x" + tempValue[i];
                            }
                        }
                        else//������޷���32����
                        {
                            int index = AddressTable.Type4.IndexOf(element[2]);
                            for (int x = 0; x < tempValue.Length; x++)
                            {//���������Ͳ�ͬ�Ĵ�����ת����32λ
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
                address = Addto4(address);//�Ѹ�λ��������
                content.InnerText = String.Join(",", address);
            }
            Datas.AppendChild(content);
            document.AppendChild(Datas);
            document.Save(this.savePath + "\\" + "Configuration_Datas.xml");
        }

        /// <summary>
        /// ����Configuration_Def.xml
        /// </summary>
        /// <param name="addressInfo">��ַ��Ϣ</param>
        /// <param name="ctrllist">���̿ؼ�����</param>
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
        /// �����Լ��Ͻ��в����Ƿ����Ż�����
        /// </summary>
        /// <param name="Define">���常�ڵ�</param>
        /// <param name="properties">���Խڵ�</param>
        /// <param name="addressInfo">��ַ��Ϣ</param>
        /// <param name="ctrl">��ǰ�ؼ���Ϣ</param>
        /// <param name="deflist">�Ѿ��������</param>
        private void infosss(ref XmlElement Define,List<XProp> properties, List<string[]> addressInfo, ControlInfo ctrl,ref List<string> deflist)
        {
            foreach (XProp element in properties)
            {
                try
                {
                    if (element.Optype != null || element.Opvalue != null || element.Relate != null)
                    {//��������Ҫ�Ż�
                        //List<string> express = new List<string>();
                        List<string> sign = new List<string>();
                        List<string> Opvalue = new List<string>();
                        bool doubleSign = false;
                        if (Findaddressinfo(addressInfo, ctrl, element) || STlist.Contains(ctrl.CodeInfo[2] + "." + element.VarName))
                        {//���ڵ�ַ�б��ST
                            doubleSign = true;
                        }
                        if (element.Optype == "�ض�ֵ�Ż�")
                        {
                            int Sindex = 0;//�߼��������Ĭ��Ϊ0��ʾ�Ⱥ�
                            float temp1 = Convert.ToSingle(element.TheValue);//����Ƚ�Ĭ��ת�������ȱȽ�
                            if (element.Opvalue.Contains("/"))
                            {//����ض�ֵ
                                string[] tempV = element.Opvalue.Split('/');
                                foreach (string va in tempV)
                                {
                                    Opvalue.Add(va);
                                    if (doubleSign)
                                    {//���ڵ�ַ�б���Ҫ˫��
                                        Opvalue.Add(va);
                                    }
                                }
                            }
                            else
                            {//�����ض�ֵ
                                //Opvalue.Add(XProp.GetValue(element.Opvalue, ctrl.VisibleFunctionProperty));
                                //if (Opvalue[0] == null)
                                //{
                                //    Opvalue[0] = element.Opvalue;
                                //}
                                Opvalue.Add(element.Opvalue);
                                if (doubleSign)
                                { //���ڵ�ַ�б���Ҫ˫��
                                    Opvalue.Add(element.Opvalue);
                                }
                            }
                            if (element.Relate != "")
                            {//>����(GT),>=���ڵ���(GE),<С��(LT),<=С�ڵ���(LE)   
                                Sindex = Dsign.IndexOf(element.Relate);
                            }
                            for (int j = 0; j < Opvalue.Count; j++)
                            {
                                sign.Add(Ssign[Sindex]);
                                if (doubleSign)
                                {//���ڵ�ַ�б�ÿ��ֵ��Ӧ �����
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
                                    {//�ж��ǻ��Ƿ�
                                        sign[j] = "NOT" + Ssign[Sindex];
                                    }
                                }
                            }
                        }
                        else if (element.Optype == "ֵ�Ż�")
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
                    MessageBox.Show("�ؼ�" + ctrl.SortName + "������" + element.Name + "�Ż�ʱ����");
                    continue;
                }
            }
        }

        /// <summary>
        /// ��0x1����λ���0x01����λ
        /// û��ֵ�ĸ�0x00
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
        /// �ӿؼ���Ϣ�������ҵ���Ӧ������
        /// </summary>
        /// <param name="element">Ŀ���ַ��������Ϣ</param>
        /// <param name="ctrllist">�ؼ�����</param>
        /// <returns>��������</returns>
        private XProp FindCtrlattribute(string[] element, List<ControlInfo> ctrllist)
        {
            foreach (ControlInfo ctrl in ctrllist)
            {
                if (element[0] == ctrl.CodeInfo[2])
                {//�ؼ�����ͬ
                    foreach (XProp attribute in ctrl.VisibleFunctionProperty)
                    {
                        if (element[6] == attribute.VarName)
                        {//��������ͬ
                            return attribute;
                        }
                    }
                    foreach (XProp attribute in ctrl.UnvisibleFunctionProperty)
                    {
                        if (element[6] == attribute.VarName)
                        {//��������ͬ
                            return attribute;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ��ǰ�ؼ���ĳ�����Ƿ�������ַ����
        /// </summary>
        /// <param name="address">��ַ��Ϣ�б�</param>
        /// <param name="ctrl">��ǰ�ؼ���Ϣ</param>
        /// <param name="atribt">������Ϣ</param>
        /// <returns>���ڵ�ַ��Ϣ����Ϊtrue</returns>
        private bool Findaddressinfo(List<string[]> address, ControlInfo ctrl, XProp atribt)
        {
            foreach (string[] ads in address)
            {
                if (ads[0] == ctrl.CodeInfo[2])
                {//�ؼ�����ͬ
                    if (ads[6] == atribt.VarName)
                    {
                        return true;
                    }                    
                }
            }
            return false;
        }

        /// <summary>
        /// ���������Ŀؼ���Ϣ����XML���й������Ե�ģ����
        /// </summary>
        /// <param name="ctrlinfo">�ؼ���Ϣ</param>
        /// <returns>���ؿؼ���XML�ڵ�</returns>
        private XmlNode CreateParameterNode(List<string[]> addressInfo, ControlInfo ctrlinfo, string structName)
        {
            XmlElement newParameter = document.CreateElement("Parameter");
            newParameter.SetAttribute("type", ctrlinfo.CodeInfo[1]);
            newParameter.SetAttribute("name", Aid + structName);
            newParameter.SetAttribute("index",ctrlinfo.ControlNum.ToString());  //2013.11.27
    
            List<string> tempList = new List<string>();//����Ѿ���E2PROM���õ���Ӣ����������ʾ��other��
            //��ַ�ڵ�
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
                            {//���Դ���            
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
                            {//���Դ���            
                                newValue.SetAttribute("optimise", element.Opnode);
                            }
                        }
                        newValue.InnerText = address[4] + "," + address[7].Split(',').Length.ToString();
                    }
                    newE2PROM.AppendChild(newValue);
                    tempList.Add(address[6]);
                }
            }
            tempList.Add(CassViewGenerator.portIndex);//��ӵ���ʹ���䲻������other�ڵ���

            newParameter.AppendChild(newE2PROM);
            //others�ڵ�
            XmlElement newothers = document.CreateElement("others");
            if (ctrlinfo.VisibleFunctionProperty != null)  //20140224
            {
                //�ɼ����ԵĲ���ʾ���Խڵ�
                foreach (XProp element in ctrlinfo.VisibleFunctionProperty)
                {
                    if (!tempList.Contains(element.VarName))
                    {
                        XmlElement newValue = document.CreateElement("item");
                        newValue.SetAttribute("name", element.VarName);
                        if (element.Opnode != null && element.Opnode != "")
                        {//others�е����� �����Ż��ڵ�ֵ���������� �Ż�������
                            newValue.SetAttribute("optimise", element.Opnode);
                        }
                        if (CassViewGenerator.ProjectMode == "View" || CassViewGenerator.ProjectMode == "Simu")
                        {
                            if (element.EnumValue != "" && element.ValueType != "COM") //���� 2014.1
                            {
                                newValue.InnerText = new List<string>(element.EnumValue.Split(',')).IndexOf(element.TheValue.ToString()).ToString();
                            }
                            else
                            {
                                if (element.ValueType == "System.Boolean")
                                {//���������ַ���ʽת����0.1���ַ���ʽд��XML20090609
                                    newValue.InnerText = Convert.ToInt16(Convert.ToBoolean(element.TheValue)).ToString();
                                }
                                else if (element.ValueType == "COM")  //���� 2014.1   ����������ַ���
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
                //���ɼ����ԵĲ���ʾ���Խڵ�
                foreach (XProp element in ctrlinfo.UnvisibleFunctionProperty)
                {
                    if (!tempList.Contains(element.VarName))
                    {
                        XmlElement newValue = document.CreateElement("item");
                        newValue.SetAttribute("name", element.VarName);
                        if (element.Opnode != null && element.Opnode != "")
                        {//others�е����� �����Ż��ڵ�ֵ���������� �Ż�������
                            newValue.SetAttribute("optimise", element.Opnode);
                        }
                        if (element.EnumValue != "" && !element.TheValue.ToString().Contains("array"))
                        {//�ų��������
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
        /// �������Խڵ��б�
        /// </summary>
        /// <param name="IOList">����ָ���</param>
        /// <param name="TempInfo">ȫ����ʱ����</param>
        /// <returns>���ز��Խڵ��б�</returns>
        private List<XmlElement> CreateTacticNode(List<string[]> IOList, List<string> TempInfo, List<ControlInfo> CtrlsInfo)
        {
            List<XmlElement> tactics = new List<XmlElement>();
            List<XmlElement> loops = new List<XmlElement>();
            List<string> MainList = new List<string>();
            List<List<string>> PageList = new List<List<string>>();


            XmlElement newTactic = document.CreateElement("tactic");//��ֵ���� ��ֹ����

            //newTactic = document.CreateElement("Main");

            foreach (string[] IOrow in IOList)
            {
                if (IOrow != null && IOrow[0].EndsWith(":"))
                {
                    string MarkName = IOrow[0].TrimEnd(':');
                    string PageName = GenerateCode.getMark(MarkName);
                    int PackNum = GenerateCode.getNum(MarkName);
                    if (PageName == MarkName)
                    {//��ʾ��ĩβ����������Ϊҳ������
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
                    //    {//main�ڵ�ĩβ�������еĿؼ�����
                    //        for (int Pindex = 1; Pindex < GenerateCode.PackInfos.Count; Pindex++)
                    //        {//��Ŵ�1��ʼ ȥ��Main�Ŀؼ�����
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
                    {//��ʱ�����л������һ��
                        newInstruction.SetAttribute("name", IOrow[0] + "Control");
                        newParam.InnerText = "&" + Aid + IOrow[1];
                        newInstruction.AppendChild(newParam);
                        if (IOrow[0] == "ST" && IOrow[1].Contains("."))
                        {//�����Ż�����ʱ�õ�����
                            STlist.Add(IOrow[1]);
                        }
                    }
                    else//�߼��ؼ��д����������������������� Ҫ��configuration��������
                    {
                        newInstruction.SetAttribute("name", IOrow[0] + "Control");
                        if (new List<string>(new string[] { "CALCU", "PROCESS" }).Contains(IOrow[0]))
                        {//����������ͼ�������̬�ڵ�name����ͬ
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
                            {//JMP��CALL�ؼ���Ӧ��������ͷβ�ַ�
                                temp = Mid + temp + Lid;
                            }
                            if (IOrow[0] == "CON" && IOrow[1].ToUpper() == "NULL")
                            {//���ֳ���ָ���NULL������������XML�ļ���
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
                        //    {//JMP��CALL�ؼ���Ӧ��������ͷβ�ַ�
                        //        temp = Mid + temp + Sid;
                        //    }
                        //    newParam.InnerText = temp;
                        //    newInstruction.AppendChild(newParam);
                        //}
                        //else if (IOrow.Length == 3)
                        //{//IOrow����Ϊ3��Ϊģ����߼��ؼ� ��һλΪ���ͣ�����λΪ��Ӧ�Ŀؼ����
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
        /// ���ڵ㸸��˳�����ĸ˳���������
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
        /// �Ƚ�����ҳ����ĸ�Ĵ�С
        /// aС�򷵻�true�����򷵻�false
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns> aС�򷵻�true�����򷵻�false</returns>
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
        /// XSLTת��XML��Main.c�ļ�
        /// </summary>
        /// <param name="xmlfile">XML�ļ�·��</param>
        /// <param name="xslfile">XSLT�ļ�·��</param>
        /// <param name="desfile">main.C�ļ�·��</param>
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
        /// ��ȡ����ؼ�����������
        /// </summary>
        /// <param name="CassviewList"></param>
        public List<XmlNode> GetSpecialCtrlInfo(ArrayList CassviewList)
        {
            GenerateCode.UnseeArray = new List<string[]>();//��ʼ����̬���� �û����ɼ�����
            document = new XmlDocument();//��ʼ��XML�ĵ� �����ɵĹ���XML��ͬ���뷽��CreateProjectXMLʹ��ͳһ�ĵ�
            int Count = 0;//��ʼ�������������
            SpeicalNodes = new List<List<XmlNode>>();//��ʼ������ؼ��ڵ�
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
                        if (index == 0)//ģ��
                        {
                            ArrayNodes.Add(CreateArrayNode(GenerateCode.GetFuzzyArray(cassView, element, ref Count)));
                        }
                        else if (index == 1)//����
                        {
                            //ArrayNodes.AddRange(CreateJieOu(cassView, element));
                            List<string[]> tempInfos = GenerateCode.GetJieOuArray(cassView, element, ref Count);
                            foreach (string[] info in tempInfos)
                            {
                                ArrayNodes.Add(CreateArrayNode(info));
                            }
                        }
                        else if (index == 2)//����
                        {
                            ProcesNodes.Add(CreateProcess(cassView, element));
                        }
                        else if (index == 3)//������
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
            SpeicalNodes = new List<List<XmlNode>>();//��ʼ������ؼ��ڵ�
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
        /// ������������̬�ڵ�
        /// </summary>
        /// <param name="curCas">�ؼ�����CASSVIEW����</param>
        /// <param name="curCtrl">�ؼ�</param>
        /// <returns>�ڵ�</returns>
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
        /// �Ѽ�������̬�е��м������ʾʱ�ĳ�portname.fm[0]
        /// </summary>
        /// <param name="changeValue">���ı�ı��ʽ</param>
        /// <param name="PortName">������portname</param>
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
        /// ��������Ϣ���ɽڵ�
        /// </summary>
        /// <param name="ArrayInfo"></param>
        /// <returns></returns>
        private XmlNode CreateArrayNode(string[] ArrayInfo)
        {//ArrayInfo˳������Ϊ�ؼ�PortName������VarName�����͡������������ȡ�����
            XmlElement newItem = document.CreateElement("item");
            newItem.SetAttribute("name", ArrayInfo[3]);
            newItem.SetAttribute("type", ArrayInfo[2]);
            newItem.SetAttribute("dimension", ArrayInfo[4]);
            newItem.InnerText = "{" + ArrayInfo[5] + "}";
            return newItem;
        }

        /// <summary>
        /// ��������������ڵ�
        /// </summary>
        /// <param name="curCas">�ؼ�����CASSVIEW����</param>
        /// <param name="curCtrl">�ؼ�</param>
        /// <returns>�ڵ�</returns>
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
                {//�������������Ϣ����������                   
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
                {//����Ϊ�ջ������ı��ʽΪ��
                    continue;
                }
                for (int Cindex = 0; Cindex < tempStruct.NumOfConditions; Cindex++)
                {
                    string Condition = null;
                    if (tempStruct.OrderBox[Cindex, Aindex] == "" || tempStruct.Conditions[Cindex] == "")
                    {//���˳����ϵΪ�� ���Ӧ������Ϊ��������
                        continue;
                    }
                    else if (tempStruct.OrderBox[Aindex, Cindex] == "N")
                    {//�ǹ�ϵ���ϣ�
                        Condition = "!";
                    }
                    if (tempStruct.IsOnlyStart)
                    {//�������������趨�ж���ʾ�������黹�Ǳ��ʽ
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
                {//���û��������Ĭ��Ϊ1 �������"1"�����ӽڵ�
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
        /// �������Ͷ������ʽ�еĹ��ܿ���ǰ���ϱ�ʶ��
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
        /// �ڱ��ʽ���ҵ�Ŀ�������ڸ���ǰ�����ʾ��
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
        /// �Թ���ģ������ŵ�������в��ҷ�����Ӳ�����������XML�ļ�����ʱ
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
                    {//��0λ�ſؼ����� ��ADD DIV �ڶ�λ�Ÿ���Ŀؼ��������
                        List<int> tempList = (List<int>)element[1];
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            if (tempList[i] == CtrlNum)
                            { //�ҵ��򷵻ض�Ӧ�����
                                return element[0] + i.ToString();
                            }
                        }
                        //�Ҳ�������������
                        ((List<int>)element[1]).Add(CtrlNum);
                        return element[0] + (((List<int>)element[1]).Count - 1).ToString();
                    }
                }
            }
            //�Ҳ������������������
            ArrayList newModule = new ArrayList();
            newModule.Add(ModuleType);
            newModule.Add(new List<int>(new int[] { CtrlNum }));
            ModuleCount.Add(newModule);
            return ModuleType + "0";
        }
    }
}
