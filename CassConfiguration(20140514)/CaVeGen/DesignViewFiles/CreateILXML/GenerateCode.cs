/*******************************************************************************
           ** Copyright (C) 2009 CASS ��Ȩ����
           ** �ļ�����CassView.cs 
           ** ����������
           **          ���ݿؼ������߹�ϵ����ָ���
           ** ���ߣ����罡
           ** ��ʼʱ�䣺2009-5-15
           ** 
********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using CaVeGen.DesignViewFiles;
using CaVeGen.DesignViewFiles.FilterProperty;
using System.Windows.Forms;
using System.ComponentModel;



namespace CaVeGen.DesignViewFiles
{
    class GenerateCode
    {
        private List<string> Control_C = new List<string>();//��������control.c�ļ�
        private List<string> CtrlName_C = new List<string>();//����Ѿ��ù�Control.c�Ŀؼ�����        

        private List<ArrayList> tabList = new List<ArrayList>();
        private List<string> UseTactic = new List<string>();//��ʹ�õ����Եļ���

        private List<string[]> Rpageinfo = new List<string[]>();//ҳ����Ϣ��Ӧ�� ���ڵ������ӽڵ���������ڵ�ҳ�淭����

        //��������ؼ��ɿؼ�����ָ�����Ķ�Ӧ����2013
        public static List<string> SortCtrlName
            = new List<string>(new string[] { "����", "�������", "�������", "����", "ϵͳ����", "����", "��ת", "����ָ��","�ļ�����","�ļ����","��������"/*,"������˹","�豸����"*/});
        public static List<string> CodeCtrlName
            = new List<string>(new string[] { "RET", "LD", "ST", "CON", "SYSVAR", "CALL", "JMP", "POP", "FILEIN", "FILEOUT", "FEATHERIN"/*,"LAPALCE" ,"CAMERAIN"*/ });
        //����ؼ��еĲ���ʾ����
        public static List<string> JOUnuseArray 
            = new List<string>(new string[] { "�ϴ�ƫ������", "���ϴ�ƫ������", "�ϴο��������" });

        public static List<string[]> PackInfos = new List<string[]> ();//ÿ���ؼ���Ϣ����

        //��Ҫ��λ���Բ��ֲ��ɼ��������Խ��м��㸳ֵ�Ŀؼ���
        public static List<string> SpicalCtrlName
            = new List<string>(new string[] { "б�¿�����", "ͨ�����Ի���", "����ȡ����", "�ߵ�ѡ����" });

        public List<string[]> ViewErrorinfo = new List<string[]>();//δ���ӵĿؼ���Ϣ

        static public List<string[]> UnseeArray = new List<string[]>();//��Ų���ʾ���û���������Ϣ//��ʱ���ڽ��������20090617
        private List<string[]> ArrayInfo = new List<string[]>();//���������Ϣ

 
        #region ҳ����������

        //ͼ�ν�����Զ�ҳ�������������޸�
        //����������ָ�������ʱ����ݸ���ҳ��ļ�����д�������
        //�������ڵ�ĸ����ӽڵ�Ϊ"_"��ʼ���Ҹ���������
        //"_"���Ƚ�A~Z��ĸ��1-2λ��ɱ�ʾ�ýڵ�ĸ��ڵ���ӽڵ�����
        //��ĸ����1-2λ0-9������ɱ�ʾ��ҳ��Ŀؼ������,�ҵ�һ���޸����

        #endregion

        //��ſؼ���������Ҫ��������ʱ�������ļ���
        public List<string> TempInfo = new List<string>();

        static public List< List<string>> CtrlsList = new List<List<string>>(); 

    //    static public List<string> ctrlsNum = new List<string> ();  //2014.1.15
        

        //#region ί�� ���ݸ�CassViewGenerator�����ź���Ŀؼ��������  2014.1.10 
       
        ////����ί��
        //public delegate void SendMeg(List<string> myList);
        ////����ί�� 
        //public SendMeg myDeleSendCtrlsNum;

        //#endregion

        public GenerateCode(List<ArrayList> CassInfoList)
        {
            if (CassInfoList != null)
            { this.tabList.AddRange(CassInfoList); }
            //��ʼ���ؼ���������
            PackInfos = new List<string[]>();
        }

        public List<string[]> GenerateCodeList()
        {
            List<string[]> Codelist = new List<string[]>();
            //��ʼ������ҳ����ԣ���������ҳ����
            //this.UseTactic = new List<string>(new string[] { "main" });20090624ȫ��ʾ����Ҫ�ӹ�
            int Count = 0;//�������
    
            foreach (ArrayList node in this.tabList)
            {
                ArrayInfo.AddRange(CatchArrayInfo((CassView)node[1], ref Count));
                //if (UseTactic.Contains(((string[])(node[0]))[0]))
                //{//û�е������Թ���ҳ     
                
                Codelist.AddRange(PackOrderCtrl((CassView)node[1], (string[])node[0]));
                Codelist.Add(null);
                //}
            }
            return Codelist;
        }

        /// <summary>
        /// ��ȡ��ǰҳ����ʾ���
        /// </summary>
        /// <param name="markArray">��ǰҳ���·��</param>
        /// <returns>����ת������ָ�������ʾ�ı�ʾ����</returns>
        private string getMark(string[] markArray)
        {
            if (markArray[1] == null || !markArray[1].Contains("\\"))
            {//�޸��ڵ����ΪMain
                return "main";
            }
            else
            {
                int startIndex = 0;//ҳ�����
                bool findinfo = false;//�Ƿ�����뵱ǰ��ҳ���Ӧ��
                string addName = null;
                string[] levelinfo = markArray[1].Split('\\');

                foreach (string[] element in this.Rpageinfo)
                {//����ҳ�������Ϣ��
                    if (element[0] == levelinfo[levelinfo.Length - 2])
                    {//���ڸ��ڵ����Ϣ
                        startIndex = Convert.ToInt32(element[1]);//���ڵ������ӽڵ����
                        element[1] = (startIndex + 1).ToString();
                        addName = element[2];//���ڵ��ʾ����
                        findinfo = true;
                    }
                }
                if (!findinfo)
                {//�����ڸ��ڵ���Ϣ
                    this.Rpageinfo.Add(new string[] { levelinfo[levelinfo.Length - 2], (startIndex + 1).ToString(), null });
                }
                //�����ת������ĸ��
                if (startIndex < 26)
                {
                    addName += "_" + Convert.ToChar('A' + startIndex).ToString();
                }
                else
                {
                    addName += "_" + Convert.ToChar('A' + startIndex / 26 - 1).ToString() + Convert.ToChar('A' + startIndex % 26).ToString();
                }
                //����ǰ�ڵ�Ĺ�����Ϣ���������Ϣ��
                this.Rpageinfo.Add(new string[] { levelinfo[levelinfo.Length - 1], "0", addName });
                return addName;
            }
        }

        /// <summary>
        /// ��������β�������ֿ��ȥ��
        /// ��ȡ�����ڵ�ҳ����
        /// </summary>
        /// <param name="markName">��ʾ����</param>
        /// <returns>ҳ����</returns>
        static public string getMark(string markName)
        {
            for (int i = markName.Length - 1; i >= 0; i--)
            {
                if (markName[i] >= '0' && markName[i] <= '9')
                {
                    continue;
                }
                else
                {
                    return markName.Substring(0, i + 1);
                }
            }
            return markName;
        }

        /// <summary>
        /// ��������β�������ֿ������ȥ��
        /// ��ȡ�����ڵ�ҳ��Ŀ��
        /// </summary>
        /// <param name="markName">��ʾ����</param>
        /// <returns>���</returns>
        static public int getNum(string markName)
        {
            string markNum = null;
            for (int i = markName.Length - 1; i >= 0; i--)
            {
                if (markName[i] >= '0' && markName[i] <= '9')
                {
                    markNum = markName[i] + markNum;
                }
                else
                {
                    break;
                }
            }
            return Convert.ToInt32(markNum);
        }

        /// <summary>
        /// ��Cassview�еĿؼ����д��������
        /// </summary>
        /// <param name="curCas">��Ҫ�����cassview</param>
        /// <param name="pageName">cassviewҳ�������</param>
        /// <returns></returns>
        public List<string[]> PackOrderCtrl(CassView curCas, string[] pageNameInfo)
        {
            List<ControlInfo[]> CassInfo = new List<ControlInfo[]>();
            List<List<ControlInfo>> alreadyPack = PackCtrls(curCas, pageNameInfo[0]);

            string pageName = getMark(pageNameInfo);
            foreach (List<ControlInfo> pack in alreadyPack)
            {   //�Կؼ���������
                CassInfo.Add(OrderCtrlsNum(curCas,pack,pageNameInfo[0]));
            }
            return OrderPacksNum(curCas, CassInfo, pageName);
        }

        /// <summary>
        /// ��ҳ��cassviewΪ��λ���пؼ����������
        /// �������ؼ�������ؼ�����������б��ؼ�JMP��Ҫ��Ϣ��
        /// �Լ�����ָ����б�Ϳؼ���������ƥ��󷵻�
        /// </summary>
        /// <param name="alreadyOrderInfo">������Ŀؼ�������</param>
        /// <param name="pageName">��ǰCASSVIEW��ҳ����</param>
        /// <returns></returns>
        private List<string[]> OrderPacksNum(CassView curCas, List<ControlInfo[]> alreadyOrderInfo, string pageName)
        {
            List<Point> OrderPoint = new List<Point>();//���ÿ���ؼ���������Сֵ,��Ϊ�ؼ�������������
            List<ControlInfo[]> OrderPack = new List<ControlInfo[]>();//�ؼ�������������б�
            List<string> Indexs = new List<string>();//ÿ���ؼ��Ŀؼ��������
            List<string> PackName = new List<string>();//�ؼ������ļ���

            GenerateCode.CtrlsList.Clear();

            for (int Pindex = 0; Pindex < alreadyOrderInfo.Count; Pindex++)
            {
                List<string> CnumList = new List<string>();//ÿ���ؼ���ż���
                Point minP = new Point(1200, 1000);//ÿ���ؼ���С����
                //��һ����ȷ���ؼ�����
                //����ҳ��û��0������
                if (Pindex == 0)
                {
                    PackName.Add(pageName);
                }
                else
                {
                    PackName.Add(pageName + Pindex.ToString());//����ҳ����+���������Ϊ����
                }
                //�ڶ������ҵ�ÿ���ؼ���������Сֵ���ɴ˽��пؼ���������
                for (int Cindex = 0; Cindex < alreadyOrderInfo[Pindex].Length; Cindex++)
                {
                    if (alreadyOrderInfo[Pindex][Cindex].ControlNum != -1)
                    {
                        CnumList.Add(alreadyOrderInfo[Pindex][Cindex].ControlNum.ToString());
                        Point CurP = curCas.FindBlockInfo(alreadyOrderInfo[Pindex][Cindex]).StartPoint;
                        if (CurP != new Point() && (CurP.Y < minP.Y || (CurP.Y == minP.Y && CurP.X < minP.X)))
                        {//ȡYֵС�ĵ㣬��Y��ͬ��ȡX��ͬ�ĵ�
                            minP = CurP;
                        }
                    }
                }
                //2014.1.15
             //   GenerateCode.ctrlsNum = CnumList;
                GenerateCode.CtrlsList.Add(CnumList);
                ////����ί��
                //if (CnumList != null)
                //{
                //    this.myDeleSendCtrlsNum(CnumList);
                //}
                if (OrderPoint.Count == 0 
                    || minP.Y > OrderPoint[OrderPoint.Count - 1].Y 
                    || (minP.Y == OrderPoint[OrderPoint.Count - 1].Y && minP.X >= OrderPoint[OrderPoint.Count - 1].X))
                {//�����򼯺�Ϊ�� �� Yֵ���ڼ���βԪ��Y �� Y��ͬXֵ���ڵ��ڼ���βԪ��X
                    OrderPoint.Add(minP);
                    OrderPack.Add(alreadyOrderInfo[Pindex]);
                    Indexs.Add(String.Join(",", CnumList.ToArray()));
                }
                else
                {
                    for (int Gindex = 0; Gindex < OrderPoint.Count; Gindex++)
                    {
                        if (minP.Y < OrderPoint[Gindex].Y || (minP.Y == OrderPoint[Gindex].Y && minP.X < OrderPoint[Gindex].X))
                        {//�������� YֵС�ڵ�ǰ��Y �� Yֵ��ͬXС�ڵ�ǰ��X
                            OrderPoint.Insert(Gindex, minP);
                            OrderPack.Insert(Gindex, alreadyOrderInfo[Pindex]);
                            Indexs.Insert(Gindex, String.Join(",", CnumList.ToArray()));
                            break;
                        }
                    }
                }
            }
            //�������ֽ������Ŀؼ�����������봮����������
            if (Indexs.Count == PackName.Count)
            {
                for (int i = 0; i < Indexs.Count; i++)
                {
                    PackInfos.Add(new string[] { PackName[i], Indexs[i] });
                }
            }
            //���Ĳ��ֽ��ؼ�����������������ɵ�ָ������������ϲ�����
            return InsertPackName(CreateCodeList(OrderPack), PackName);
        }


        /// <summary>
        /// ��Ӧ���ɵ�ָ���Ϳؼ�����������ƥ��
        /// </summary>
        /// <param name="CodeInfo"></param>
        private List<string[]> InsertPackName(List<string[]> CodeInfo, List<string> PackInfo)
        {
            for (int i = 0; i < PackInfo.Count; i++)
            {
                if (i == 0)
                {//�ڵ�һ�в����һ���ؼ�����
                    CodeInfo.Insert(0, new string[] { PackInfo[0] + ":", null });
                }
                else
                {//�������ÿؼ������滻�ؼ������NULL
                    for (int j = 0; j < CodeInfo.Count; j++)
                    {
                        if (CodeInfo[j] == null)
                        {
                            CodeInfo[j] = new string[] { PackInfo[i] + ":", null };
                            break;
                        }
                    }
                }
            }
            return CodeInfo;
        }


        /// <summary>
        /// ����curtcass�����пؼ���Ϣ���������������ֿ�
        /// </summary>
        /// <param name="curtcass">��Ҫ�����Cassview</param>
        /// <param name="pageName">��ǰCassview��ҳ��</param>
        /// <returns>�ѷֿ�õĿؼ���Ϣ��</returns>
        private List<List<ControlInfo>> PackCtrls(CassView curtcass, string pageName)
        {
            List<List<ControlInfo>> GroupCtrls = new List<List<ControlInfo>>();
            List<string> UseCtrl = new List<string>();//�����Ѿ�ʹ�ù��Ŀؼ�����
            ////////////////////////////////////////////////////////////////////////////////���Ż�
            foreach (ControlInfo ctrl in curtcass.ctrlsInfo)
            {
                if (UseCtrl.Contains(ctrl.ControlName))
                { continue; }
                else if (ctrl.IsConnect)
                {//�ÿؼ������ӿؼ���û�б������ؼ�������
                    Stack<ControlInfo> tempStack = new Stack<ControlInfo>();//���ڴ�ŵ�ǰ�ؼ���Ϣ�������ӵĿؼ���Ϣ����ת�ö�ջ
                    List<ControlInfo> Pack = new List<ControlInfo>();

                    //��ǰ�ؼ�ѹ����ʱ��ջ
                    tempStack.Push(ctrl);
                    UseCtrl.Add(ctrl.ControlName);

                    while (tempStack.Count != 0)
                    {
                        //ȡ��ջ���ؼ�
                        ControlInfo TopCtrl = tempStack.Pop();

                        //�����ÿؼ������ӵ��ҷǱ������Ŀؼ�ѹ���ջ
                        if (TopCtrl.OutputInfo != null)
                        {
                            if (TopCtrl.OutputInfo.Count == 1 && TopCtrl.OutputInfo[0][0] != null && TopCtrl.OutputInfo[0][0].Split(',').Length == 1)
                            {//�������Ӧ���������
                                ControlInfo ConnectCtrl = curtcass.ctrlsInfo[curtcass.FindControlName(TopCtrl.OutputInfo[0][0].Split('.')[0])];
                                if (!UseCtrl.Contains(ConnectCtrl.ControlName))
                                {
                                    tempStack.Push(ConnectCtrl);
                                    UseCtrl.Add(ConnectCtrl.ControlName);
                                }
                            }
                            else
                            {//�������Ӧ����������������       
                                for (int i = TopCtrl.OutputInfo.Count-1; i >=0; i--)
                                {
                                    if (TopCtrl.OutputInfo[i][0] != null)
                                    {
                                        string[] OputArray = TopCtrl.OutputInfo[i][0].Split(',');//һ����ڶ�Ӧ����ؼ��Զ��ŷָ�
                                        for (int j = 0; j < OputArray.Length; j++)
                                        {
                                            ControlInfo ConnectCtrl = curtcass.ctrlsInfo[curtcass.FindControlName(OputArray[j].Split('.')[0])];
                                            if (!UseCtrl.Contains(ConnectCtrl.ControlName))
                                            {
                                                tempStack.Push(ConnectCtrl);
                                                UseCtrl.Add(ConnectCtrl.ControlName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (TopCtrl.InputInfo != null)
                        {
                            for (int j = TopCtrl.InputInfo.Count - 1; j >= 0; j--)
                            {
                                if (TopCtrl.InputInfo[j][0] != null)
                                {
                                    ControlInfo ConnectCtrl = curtcass.ctrlsInfo[curtcass.FindControlName(TopCtrl.InputInfo[j][0].Split('.')[0])];
                                    if (!UseCtrl.Contains(ConnectCtrl.ControlName))
                                    {
                                        tempStack.Push(ConnectCtrl);
                                        UseCtrl.Add(ConnectCtrl.ControlName);
                                    }
                                }
                            }
                        }
                        //��ջ���ؼ�ת���µĿؼ���
                        Pack.Add(TopCtrl);
                    }
                    GroupCtrls.Add(Pack);
                }
                if (!ctrl.IsConnect)
                {
                    string warningInfo = "�ؼ����Ϊ" + ctrl.ControlNum.ToString() + "��" + ctrl.CodeInfo[0] + "�ؼ�û������";
                    CassViewGenerator.SpecialErrors.Add(new string[] { null, warningInfo, "warning", pageName });
                    ViewErrorinfo.Add(new string[] { pageName, ctrl.ControlNum.ToString(), warningInfo });
                }
                if (ctrl.ControlName.ToUpper().Contains("FILEIN"))
                {
                    if (ctrl.CodeInfo[1].Trim() == "FILEIN"||ctrl.CodeInfo[1].Trim() == "" || ctrl.CodeInfo[1] == null)
                    {
                        string errorInfo = "�ؼ����Ϊ" + ctrl.ControlNum.ToString() + "��'" + ctrl.CodeInfo[0] + " '�ؼ�ȱ���������";
                        CassViewGenerator.SpecialErrors.Add(new string[] { null, errorInfo, "error", pageName });
                        ViewErrorinfo.Add(new string[]{pageName,ctrl.ControlNum.ToString(),errorInfo});


                    }
                }
            }
            return GroupCtrls;
        }

        /// <summary>
        /// ���������ؼ���Ϣ���Ͻ��������뵽���������
        /// </summary>
        /// <param name="GroupCtrl">δ�����һ���ؼ���</param>
        /// <returns>����һ���Ѿ�����Ŀؼ�����</returns>
        private ControlInfo[] OrderCtrlsNum(CassView curCas,List<ControlInfo> GroupCtrl, string pageName)
        {
            Stack<ControlInfo> TempStack = new Stack<ControlInfo>();//���������õ�����ʱ��ջ            
            Stack<ControlInfo> OrderStack = new Stack<ControlInfo>();//�ѽ�������Ķ�ջ            
            List<string> tempCode = new List<string>();//�Ե����ϱ���ʱ��˳������ʱ�����Ķ���

            #region �ؼ��������
            //�û������ߺ�ָ����ֻ������õ����������ؼ�Ŀ��
            //�����������ӿڴ��ڣ��������Ӷ����������Ĭ��������ֵ����Ŀ�꣬������ʾ��ָ�����̬������
            //���ṩ����λ��ʱ����ȫ��ʾ�������������Ŀ��
            //��������ؼ�ʱ������Ƚ����ֵ��Ŀ���趨Ϊ��ʱ����������Ȼ���˳�򲻴���ȡ���ò�������������
            //������ؼ���������˳��Ϊ����20090608
            //���������Ӧ������ʱ��ͬ��������ʱ����
            //ͬһ�ؼ����ж������֧��û���ٻ�۵���������ݿؼ����������Ϻ��µĽ��з���20090805
            #endregion

            #region ���򷽷�
            //����û��ܸĶ�ָ������ڿ�ֵ�����ƥ������     
            //�����������Ϣ�ж�����͵�����������������Ϣ��������
            foreach (ControlInfo ElementCtrl in GroupCtrl)
            {
                if (CheckTipCtrl(ElementCtrl.OutputInfo))
                {//β�˿ؼ�//δ������������ӵ�β�˿ؼ� ����NULL//��ʱ�ж�����ΪNULL������NULL
                    TempStack.Push(ElementCtrl);
                }
                else if (ElementCtrl.OutputInfo.Count != 1 || ElementCtrl.OutputInfo[0][0].Split(',').Length != 1)
                {//������͵����������ؼ�
                    for (int OPindex = 0; OPindex < ElementCtrl.OutputInfo.Count; OPindex++)
                    {
                        if (ElementCtrl.OutputInfo[OPindex][0] != null)
                        {
                            string[] tempArray = ElementCtrl.OutputInfo[OPindex][0].Split(',');
                            //��ԭ�����Ϣ��ӣ��ؼ���_������� 
                            string tempOutInfo = null;
                            if (ElementCtrl.CodeInfo[2] != null)
                            {
                                tempOutInfo = ElementCtrl.CodeInfo[2];
                            }
                            else
                            {
                                tempOutInfo = ElementCtrl.CodeInfo[1];
                            }
                            //������ʱ���� �ؼ���_�˿ں�
                            tempOutInfo =
                                //"configuration" + CassViewGenerator.ProjectNum + "_" + 
                                tempOutInfo + "_" + OPindex.ToString();
                            ElementCtrl.OutputInfo[OPindex][1] = tempOutInfo;

                            //�ҵ�������ڶ�Ӧ�������,���޸����������Ϣ
                            foreach (string IOinfo in tempArray)
                            {
                                for (int j = 0; j < GroupCtrl.Count; j++)
                                {
                                    if (GroupCtrl[j].ControlName == IOinfo.Split('.')[0])
                                    {
                                        GroupCtrl[j].InputInfo[Convert.ToInt32(IOinfo.Split('.')[1])][1] = tempOutInfo;
                                        break;
                                    }
                                }
                            }
                            TempInfo.Add(tempOutInfo);//�����õ���ʱ�����������Ӧ����
                        }
                    }
                }
            }
            if (GroupCtrl.Count != 0 && TempStack.Count == 0)
            {
                CassViewGenerator.SpecialErrors.Add(new string[] { null, "�����������·", "error", pageName });
                ViewErrorinfo.Add(new string[] { pageName, null, "�����������·" });
            }
            #region �Ե����Ͽؼ������㷨
            if (TempStack.Count != 1)
            {//�ж��β�˿ؼ�ʱ��β�˿ؼ���������
                TempStack = OrderEndCtrls(curCas,TempStack,GroupCtrl);
            }
            while (TempStack.Count != 0)
            {
                ControlInfo TopControl = TempStack.Pop();
                GroupCtrl.Remove(TopControl);//��δ����ؼ�����ɾ����Ӧ�Ŀؼ���Ϣ
                //����ɾ�����������ٺ���������ͷ�ֹ������ؼ����ظ�ʹ��
                //��ջ�������Ŀؼ���Ϣѹ���������ջ
                OrderStack.Push(TopControl);

                if (TopControl.ControlName != null && TopControl.InputInfo != null)
                {//����ؼ���ͷ�ؼ�(��������Ϣ)ֱ��ѹ�������ջ
                    for (int x = 0; x < TopControl.InputInfo.Count; x++)//���Ҫ˳������Ҫ�������
                    {
                        string[] Input = TopControl.InputInfo[x];
                        if (TempInfo.Contains(Input[1]))
                        {//�ؼ�������Ҫ����ʱ����
                            tempCode.Add(Input[1]);
                            ControlInfo temp = new ControlInfo();
                            temp.ControlNum = -1;//����ؼ����ͳһΪ-1
                            temp.CodeInfo = new string[1];
                            temp.CodeInfo[0] = Input[1];
                            TempStack.Push(temp);//����ؼ�1
                        }
                        else if (Input[0] != null)
                        {//��ǰ������ؼ�ѹ���ջ
                            for (int i = 0; i < GroupCtrl.Count; i++)
                            {
                                if (GroupCtrl[i].ControlName == Input[0].Split('.')[0])
                                {
                                    TempStack.Push(GroupCtrl[i]);
                                    break;
                                }
                            }
                        }
                        else if (Input[0] == null)
                        {//�û����õ������//��Ҫ�ĳ�LDĬ��
                            ControlInfo temp = new ControlInfo();
                            if (Input[3] != null && Input[3] != "")
                            {
                                temp.ControlNum = -1;//����ؼ����ͳһΪ-1
                                temp.CodeInfo = new string[2] { Input[2], Input[3] };
                                TempStack.Push(temp);//����ؼ�2
                            }
                        }
                    }
                }
                if (TempStack.Count == 0 && GroupCtrl.Count != 0)
                {//���пؼ�û������,������Ϊ��ʱ������ԭ��ʹ��ջΪ��
                    for (int i = 0; i < GroupCtrl.Count; i++)
                    {//��δ����ؼ������ҵ��Ѿ����Ա����õĿؼ�ѹ����ʱ��ջ
                        if (TempInfo.Contains(GroupCtrl[i].OutputInfo[0][1]) || GroupCtrl[i].OutputInfo.Count != 1)
                        {
                            for (int j = 0; j < GroupCtrl[i].OutputInfo.Count; j++)
                            {
                                if (GroupCtrl[i].OutputInfo[j][0] != null
                                    && !tempCode.Contains(GroupCtrl[i].OutputInfo[j][1]))
                                {//�����������ʹ�ù�����ʱ�����в�����������
                                    break;
                                }
                                if (j == GroupCtrl[i].OutputInfo.Count - 1)
                                {//����������������Ҷ��Ѿ�ʹ�ù�
                                    TempStack.Push(GroupCtrl[i]);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #endregion
            return OrderStack.ToArray();
        }

        /// <summary>
        /// ��������ҵ�β�˿ؼ��������Ķϲ㴦��������������ӿڿؼ�����
        /// �Դ�Ϊ���ݽ�������
        /// </summary>
        /// <param name="endCtrls"></param>
        /// <returns></returns>
        private Stack<ControlInfo> OrderEndCtrls(CassView curCas, Stack<ControlInfo> endCtrls, List<ControlInfo> GroupCtrl)
        {
            List<ControlInfo> orderList = new List<ControlInfo>();//�ؼ������ö���
            List<Point> orderP = new List<Point>();//�������������
            Stack<ControlInfo> returnStack = new Stack<ControlInfo>();//�����ö�ջ
            foreach (ControlInfo ctrl in endCtrls)
            {
                Stack<ControlInfo> Findctrls = new Stack<ControlInfo>();
                Findctrls.Push(ctrl);
                bool findPoint = false;
                while (!findPoint)
                {
                    ControlInfo Topctrl = Findctrls.Pop();
                    for (int x = Topctrl.InputInfo.Count - 1; x >= 0; x--)
                    {//��������϶˿��������¶˿�
                        string[] Input = Topctrl.InputInfo[x];
                        if (TempInfo.Contains(Input[1]))
                        {//������ʱ�����ҵ��ϲ�����ѭ��
                            findPoint = true;
                            //���ݵ��������Y������ȣ�Y��ͬX�����������
                            Point sp = curCas.FindBlockInfo(Topctrl).StartPoint;
                            if (orderList.Count == 0)
                            {
                                orderList.Add(ctrl);
                                orderP.Add(sp);
                            }
                            else
                            {
                                for (int y = 0; y < orderP.Count; y++)
                                {
                                    if (orderP[y].Y > sp.Y || (orderP[y].Y == sp.Y && orderP[y].X > sp.X))
                                    {
                                        orderP.Insert(y, sp);
                                        orderList.Insert(y, ctrl);
                                        break;
                                    }
                                    if (y == orderList.Count - 1)
                                    {
                                        orderList.Add(ctrl);
                                        orderP.Add(sp);
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                        else if (Input[0] != null)
                        {//��ǰ������ؼ�ѹ���ջ
                            for (int i = 0; i < GroupCtrl.Count; i++)
                            {
                                if (GroupCtrl[i].ControlName == Input[0].Split('.')[0])
                                {
                                    Findctrls.Push(GroupCtrl[i]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            foreach (ControlInfo element in orderList)
            {//������ĩ�˿ؼ�ѹ�뷵�ض�ջ ջ���ؼ�ΪY��СX��С�ؼ�
                returnStack.Push(element);
            }
            return returnStack;
        }
 
        /// <summary>
        /// ����ÿ��ؼ���Ϣת����ָ���б�
        /// </summary>
        /// <param name="PackCtrls">������Ŀؼ�����</param>
        /// <returns>��Ӧ��ָ���б�</returns>
        private List<string[]> CreateCodeList(List<ControlInfo[]> PackCtrls)
        {
            List<string[]> BlockCode = new List<string[]>();

            foreach (ControlInfo[] groupControls in PackCtrls)
            {
                foreach (ControlInfo CtrlInfo in groupControls)
                {
                    if (CtrlInfo.ControlName != null)
                    {
                        if (SpicalCtrlName.Contains(CtrlInfo.CodeInfo[0]))//����ؼ��������Ը�ֵ
                        {
                            SetUnvisibleFunction(CtrlInfo);//C#�����ܷ�Ķ�������ֵ
                        }
                        if (CtrlInfo.CodeInfo != null && SortCtrlName.Contains(CtrlInfo.CodeInfo[0]))//��������ָ��
                        {
                            int tempIndex = SortCtrlName.IndexOf(CtrlInfo.CodeInfo[0]);
                            if (tempIndex == 0)
                            {//���� �޲���
                                BlockCode.Add(new string[] { CodeCtrlName[tempIndex], null });
                            }
                            else if (tempIndex < 3)//��������������Ϊ˫����
                            {
                                if (CtrlInfo.CodeInfo[1] != null)
                                {//Ҫ�����е���������ת����ָ����е�Ӣ������
                                    BlockCode.Add(new string[] { CodeCtrlName[tempIndex], CtrlInfo.CodeInfo[1] + "." + GetVarName(CtrlInfo.CodeInfo[1], CtrlInfo.CodeInfo[2]) });
                                }
                                else
                                {
                                    //string tempCode =
                                    //    //"configuration" + CassViewGenerator.ProjectNum + "_"+
                                    //    "UnC_UnNum";
                                    BlockCode.Add(new string[] { CodeCtrlName[tempIndex], null });
                                    //if (!TempInfo.Contains(tempCode))
                                    //{
                                    //    TempInfo.Add(tempCode);
                                    //}
                                }
                            }
                            else//����Ϊ������
                            {
                                BlockCode.Add(new string[] { CodeCtrlName[tempIndex], CtrlInfo.CodeInfo[1] });
                                //if (CodeCtrlName[tempIndex] == "CALL")20090624ȫ��ʾ����Ҫ�ӹ�
                                //{//���ݵ���ָ������ж�����Ҫʹ�õĲ���ҳ��
                                //    this.UseTactic.Add(CtrlInfo.CodeInfo[1]);
                                //}
                            }
                        }
                        else if (CtrlInfo.VisibleFunctionProperty == null || CtrlInfo.VisibleFunctionProperty.Count == 0)//��ѧ����,��������
                        {//����λ��ſؼ������������XMLʱʶ��������ؼ�20090609
                            BlockCode.Add(new string[] { CtrlInfo.CodeInfo[1], null,  CtrlInfo.ControlNum.ToString() });
                        }
                        else
                        {//��ԭ�ȵڶ�λ ���ṹ�岿���滻�ɲ���20090701
                            if (CtrlInfo.CodeInfo[2] == null)//�߼���������,�й�������
                            {//ָ����к���ѧ������ͬ20090609
                                //BlockCode.Add(new string[] { CtrlInfo.ControlName, CtrlInfo.CodeInfo[1] });
                                BlockCode.Add(new string[] { CtrlInfo.CodeInfo[1], CatchPare(CtrlInfo), CtrlInfo.ControlNum.ToString() });
                            }
                            else//�㷨֧�źͿ����㷨����
                            {//����λ�����λ ���������ִ���20090701
                                BlockCode.Add(new string[] { CtrlInfo.CodeInfo[1], CatchPare(CtrlInfo), CtrlInfo.CodeInfo[2] });
                            }
                        }
                    }
                    else if (CtrlInfo.CodeInfo != null)//��ʱ��������ָ��
                    {
                        if (CtrlInfo.CodeInfo.Length == 1)
                        {//��������ؼ�1                            
                            BlockCode.Add(new string[] { "LD", CtrlInfo.CodeInfo[0] });
                        }
                        else if (CtrlInfo.CodeInfo.Length == 2)
                        {//������Ĭ��ָ��//��������ؼ�2
                            BlockCode.Add(new string[] { "CON", CtrlInfo.CodeInfo[1] });
                        }
                    }
                    //��ʱ�������ָ��
                    if (CtrlInfo.OutputInfo != null && CtrlInfo.OutputInfo.Count != 0 &&
                        (TempInfo.Contains(CtrlInfo.OutputInfo[0][1]) || CtrlInfo.OutputInfo.Count != 1))
                    {
                        //foreach (string[] info in CtrlInfo.OutputInfo)
                        //{
                        for (int x = CtrlInfo.OutputInfo.Count - 1; x >= 0; x--)
                        {//����������˳��Ϊ����20090608
                            string[] info = CtrlInfo.OutputInfo[x];
                            if (info[1] != null)
                            {
                                BlockCode.Add(new string[] { "ST", info[1] });
                            }
                            else
                            {//���������Ŀ��ؼ�ʱ                                
                                //string tempCode = 
                                //    //"configuration" + CassViewGenerator.ProjectNum +   "_"+
                                //    "UnC_UnNum";
                                BlockCode.Add(new string[] { "POP", null });
                                //if (!TempInfo.Contains(tempCode))
                                //{
                                //    TempInfo.Add(tempCode);
                                //}
                            }
                        }
                    }
                }
                BlockCode.Add(null);
            }
            return BlockCode;
        }

        /// <summary>
        /// �ӿؼ���Ϣ�л�ȡ��ʾ��ָ���Ĳ�����Ϣ
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        private string CatchPare(ControlInfo ctrl)
        {
            List<string> tempValueList = new List<string>();//��ʱ����ֵ�б�
            foreach (XProp FuncValue in ctrl.VisibleFunctionProperty)
            {
                if (FuncValue.ValueType == "System.Boolean")
                {
                    tempValueList.Add((FuncValue.TheValue.ToString().ToUpper() == "FALSE" ? 0 : 1).ToString());
                }
                else if (FuncValue.EnumValue != ""&&FuncValue.ValueType!="COM") //���� 2014.1
                {
                    tempValueList.Add(new List<string>(FuncValue.EnumValue.Split(',')).IndexOf(FuncValue.TheValue.ToString()).ToString());
                }
                else
                {
                    tempValueList.Add(FuncValue.TheValue.ToString());
                }
            }
            foreach (XProp FuncValue in ctrl.UnvisibleFunctionProperty)
            {//�Ӳ��ɼ����������������
                if (FuncValue.TheValue.ToString().Contains("array"))
                {//���������
                    foreach (string[] element in this.ArrayInfo)
                    {
                        if (element[3] == FuncValue.TheValue.ToString())
                        {//3λΪ������ 5λΪ�������� ��Ϊ�������ϴ�����
                            tempValueList.Add("{" + element[5] + "}");
                        }
                    }
                }
            }
            return String.Join(",", tempValueList.ToArray());
        }

        /// <summary>
        /// ͨ���ؼ���PortName����������Ѱ��Ӣ������
        /// </summary>
        /// <returns></returns>
        private string GetVarName(string PortName, string Name)
        {
            foreach (ArrayList node in this.tabList)
            {
                CassView curCas = (CassView)node[1];

                foreach (ControlInfo CtrlInfo in curCas.ctrlsInfo)
                {
                    if (CtrlInfo.CodeInfo != null && CtrlInfo.CodeInfo.Length > 2 && CtrlInfo.CodeInfo[2] == PortName)
                    {
                        foreach (XProp element in CtrlInfo.VisibleFunctionProperty)
                        {
                            if (element.Name == Name)
                            { return element.VarName; }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// ��������ؼ��ĸ����û����ɼ����Ը�ֵ
        /// </summary>
        /// <param name="SpeicalCtrl">�ؼ���Ϣ</param>
       static public void SetUnvisibleFunction(ControlInfo SpeicalCtrl)
        {
            if (SpeicalCtrl.CodeInfo[0] == "б�¿�����")
            {
                int TrueStep = 0;//��Ч����              
                int ControlTime = Convert.ToInt32(XProp.GetValue("controlTime", SpeicalCtrl.VisibleFunctionProperty));//����ʱ��    
                int OutNum = Convert.ToInt32(XProp.GetValue("outNum", SpeicalCtrl.VisibleFunctionProperty));//��Ч����    
                int PreValue = 0;//ǰһʱ��ֵ
                int NowValue = 0;//��ǰʱ��ֵ
                foreach (XProp function in SpeicalCtrl.VisibleFunctionProperty)
                {
                    if (function.VarName.Contains("time"))
                    {
                        if (TrueStep != 0)
                        { PreValue = NowValue; }
                        NowValue = Convert.ToInt32(function.TheValue);
                        if (NowValue < 0 || NowValue < PreValue || NowValue > ControlTime || TrueStep >= OutNum)
                        {//ʱ��ֵС��0��ʱ��ֵ���ڿ���ʱ���ǰ��ʱ��С��ǰһ��ʱ�� ���������Ŵ��ڿ��ƶ���
                            break;
                        }
                        TrueStep++;
                    }
                }
                XProp.SetValue(TrueStep.ToString(), "sectionNum", SpeicalCtrl.UnvisibleFunctionProperty);                 
            }
            else if (SpeicalCtrl.CodeInfo[0] == "ͨ�����Ի���")
            {
                int TruePoint = 0;//��Ч�����
                float MinValue = Convert.ToSingle(XProp.GetValue("fInMin", SpeicalCtrl.VisibleFunctionProperty));
                float MaxValue = Convert.ToSingle(XProp.GetValue("fInMax", SpeicalCtrl.VisibleFunctionProperty));
                float PreValue = 0;//ǰһ����ֵ
                float NowValue = 0;//��ǰ����ֵ

                foreach (XProp function in SpeicalCtrl.VisibleFunctionProperty)
                {//"��X������"ȡ�������ж�
                    if (function.Name.Length >= 5 && function.Name.Substring(3, 2) == "����")//Ϊ�����������
                    {
                        if (TruePoint != 0)
                        { PreValue = NowValue; }
                        NowValue = Convert.ToSingle(function.TheValue);
                        if (NowValue <= MinValue || NowValue <= PreValue || NowValue >= MaxValue)
                        {//����ֵС�ڻ����������Сֵ��С�ڻ����ǰһ�������ֵ����ڻ�����������ֵ
                            break;
                        }
                        TruePoint++;
                    }
                }
                XProp.SetValue(TruePoint.ToString(), "effNum", SpeicalCtrl.UnvisibleFunctionProperty);           
            }
            else //"����ȡ����", "�ߵ�ѡ����"
            {
                int index = 0;
                foreach (string[] Info in SpeicalCtrl.InputInfo)
                {//�����������Ϣ,��NULL��ʾ��Ч
                    if (Info[0] != null)
                    {
                        index++; 
                    }
                }
                XProp.SetValue(index.ToString(), "ucEffNum", SpeicalCtrl.UnvisibleFunctionProperty);               
            }
        }

        /// <summary>
        /// ����ģ���ؼ���Ϣ���������Ե�������������������Ϣ
        /// </summary>
        /// <param name="curCas">�ؼ�����CASSVIEW����</param>
        /// <param name="curCtrl">�ؼ�</param>
        /// <param name="ArrayCount">��ǰ���õ����б��</param>
        /// <returns>������Ϣ</returns>
        static public string[] GetFuzzyArray(CassView curCas, Control curCtrl, ref int ArrayCount)
        {
            List<string> ArrayInfo = new List<string>();
            PropertyDescriptor configurationProperty = TypeDescriptor.GetProperties(curCtrl)["Configuration"];
            ControlTactic.SpecialControl.Fuzzy ConvertTool = new ControlTactic.SpecialControl.Fuzzy();
            ControlTactic.SpecialControl.FuzzyStruct tempStruct = ConvertTool.ListToStruct((List<string>)configurationProperty.GetValue(curCtrl));

            ControlInfo Fuzzy = curCas.FindControlInfo(curCtrl);
            string tempArray = null;
            for (int row = 0; row < tempStruct.RowNum * 2 + 1; row++)
            {
                for (int column = 0; column < tempStruct.ColumnNum * 2 + 1; column++)
                {
                    tempArray += tempStruct.ControlTable[row, column];
                    if (row != tempStruct.RowNum * 2 || column != tempStruct.ColumnNum * 2)
                    { tempArray += ","; }
                }
                if (row != tempStruct.RowNum - 1)
                {
                    //tempArray += "\n";//ȥ��ÿ�лس����� �����¶�ȡ��ַ�����BUG20090622
                }
            }
            //����ģ�����Ʊ����������ڵ�
            for (int i = 0; i < Fuzzy.UnvisibleFunctionProperty.Count; i++)
            {
                if (Fuzzy.UnvisibleFunctionProperty[i].Name == "ģ�����Ʊ�")
                {//ArrayInfo˳������Ϊ�ؼ�PortName������VarName�����͡������������ȡ�����
                    Fuzzy.UnvisibleFunctionProperty[i].TheValue 
                        = "configuration" + CassViewGenerator.ProjectNum + "_array" + (ArrayCount++).ToString();
                    ArrayInfo.Add(Fuzzy.CodeInfo[2]);
                    ArrayInfo.Add(Fuzzy.UnvisibleFunctionProperty[i].Name);
                    ArrayInfo.Add("fp32");
                    ArrayInfo.Add(Fuzzy.UnvisibleFunctionProperty[i].TheValue.ToString());
                    ArrayInfo.Add(tempStruct.ControlTable.Length.ToString());
                    ArrayInfo.Add(tempArray);
                    ArrayInfo.Add(Fuzzy.UnvisibleFunctionProperty[i].VarName);
                }
            }
            return ArrayInfo.ToArray();
        }

        /// <summary>
        /// ��������ؼ���Ϣ���������Ե�������������������Ϣ
        /// </summary>
        /// <param name="curCas">�ؼ�����CASSVIEW����</param>
        /// <param name="curCtrl">�ؼ�</param>
        /// <param name="ArrayCount">��ǰ���õ����б��</param>
        /// <returns>������Ϣ</returns>
        static public List<string[]> GetJieOuArray(CassView curCas, Control curCtrl,ref int ArrayCount)
        {
            List<string[]> ArrayInfos = new List<string[]>();//������Ϣ����
            int count = 2;//����·��
            //������������Ҫ�������������
            List<string> NeedAddArray
                = new List<string>(new string[] { "����", "����", "΢��", "�趨ֵ", "�Զ�/�ֶ�", "����������", "����������", "���������", "�������Ĳ���" });
           
            PropertyDescriptor configurationProperty = TypeDescriptor.GetProperties(curCtrl)["Configuration"];
            ControlTactic.SpecialControl.JieOu ConvertTool = new ControlTactic.SpecialControl.JieOu();
            ControlTactic.SpecialControl.JieOuStruct tempStruct = ConvertTool.ListToStruct((List<string>)configurationProperty.GetValue(curCtrl));

            ControlInfo JieOu = curCas.FindControlInfo(curCtrl);
            for (int i = 0; i < JieOu.VisibleFunctionProperty.Count; i++)
            {//�ڿ����������ҵ����������и�ֵ
                if (JieOu.VisibleFunctionProperty[i].Name == "����·��")
                {
                    JieOu.VisibleFunctionProperty[i].TheValue = tempStruct.JieOuNum;
                    count = tempStruct.JieOuNum;
                    break;
                }
            }
            //����Ϊ���������֡�΢�֡��趨ֵ���Զ��ֶ������������ޡ����������ޡ��������������
            List<string>[] Attributes
                = new List<string>[] { new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() };
            foreach (List<string> attribute in tempStruct.JieOuAttribute)
            {//ÿ������ �����ɱ��������֡�΢�֡��趨ֵ���Զ��ֶ������������ޡ����������ޡ�������������
                Attributes[0].Add(attribute[0]);
                Attributes[1].Add(attribute[1]);
                Attributes[2].Add(attribute[2]);
                Attributes[3].Add(attribute[3]);
                Attributes[4].Add(new List<string>(new string[] { "�Զ�", "�ֶ�" }).IndexOf(attribute[4]).ToString());//�Զ��ֶ����
                Attributes[5].Add(attribute[5]);
                Attributes[6].Add(attribute[6]);
                Attributes[7].Add(attribute[7]);
            }
            for (int i = 0; i < JieOu.UnvisibleFunctionProperty.Count; i++)
            {
                int index = NeedAddArray.IndexOf(JieOu.UnvisibleFunctionProperty[i].Name);
                if (index != -1)
                {//ArrayInfo˳������Ϊ�ؼ�PortName������VarName�����͡������������ȡ�����
                    List<string> ArrayInfo = new List<string>();
                    JieOu.UnvisibleFunctionProperty[i].TheValue 
                        = "configuration" + CassViewGenerator.ProjectNum + "_array" + (ArrayCount++).ToString();
                    ArrayInfo.Add(JieOu.CodeInfo[2]);
                    ArrayInfo.Add(JieOu.UnvisibleFunctionProperty[i].Name);
                    ArrayInfo.Add("fp32");
                    ArrayInfo.Add(JieOu.UnvisibleFunctionProperty[i].TheValue.ToString());
                    if (index == 8)
                    {
                        List<string> tempArray = new List<string>();
                        foreach (string element in tempStruct.JieOuTable)
                        {
                            tempArray.Add(element);
                        }
                        ArrayInfo.Add(tempArray.Count.ToString());
                        ArrayInfo.Add(String.Join(",", tempArray.ToArray()));
                    }
                    else
                    {
                        ArrayInfo.Add(Attributes[index].Count.ToString());
                        ArrayInfo.Add(String.Join(",", Attributes[index].ToArray()));
                    }
                    if (index == 4)
                    {//�Զ�/�ֶ�Ϊchar��
                        ArrayInfo[2] = "uint8";
                    }
                    ArrayInfo.Add(JieOu.UnvisibleFunctionProperty[i].VarName);
                    ArrayInfos.Add(ArrayInfo.ToArray());
                }
                else if (JOUnuseArray.Contains(JieOu.UnvisibleFunctionProperty[i].Name))
                {//��ʹ�õ����鸳ֵ
                    JieOu.UnvisibleFunctionProperty[i].TheValue 
                        = "configuration" + CassViewGenerator.ProjectNum + "_array" + (ArrayCount++).ToString();
                    List<string> tempValue = new List<string>();//���ɽ���·����ȫ0����
                    for (int k = 0; k < count; k++)
                    { tempValue.Add("0"); }
                    UnseeArray.Add(new string[] { null, null, "fp32", JieOu.UnvisibleFunctionProperty[i].TheValue.ToString(), count.ToString(), String.Join(",", tempValue.ToArray()) });
                }
            }
            return ArrayInfos;
        }


        /// <summary>
        /// ���ݿؼ�����������Ϣ�ж��Ƿ�Ϊ�ն˿ؼ��������յ㣩
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        private bool CheckTipCtrl(List<string[]> Info)
        {
            if (Info == null || Info.Count == 0)
            { return true; }
            else
            {
                foreach (string[] element in Info)
                {
                    if (element.Length != 0)
                    {
                        foreach (string elementInfo in element)
                        {
                            if (elementInfo != null)
                            { return false; }
                        }
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// ��ȡ��ӦCassview�пؼ���������Ϣ
        /// </summary>
        /// <param name="curCas"></param>
        /// <param name="count">������ʼ���</param>
       static public List<string[]> CatchArrayInfo(CassView curCass, ref int count)
        {
            List<string[]> Ainfos = new List<string[]>();
            foreach (Control element in curCass.Controls)
            {//��ȡģ���ͽ����������Ϣ
                string MS = TypeDescriptor.GetProperties(element)["ModuleSort"].GetValue(element).ToString();
                if (MS == "ģ��������")
                {
                    string[] info = GenerateCode.GetFuzzyArray(curCass, element,ref count);
                    Ainfos.Add(info);
                }
                else if (MS == "�����������")
                {
                    List<string[]> tempInfos = GenerateCode.GetJieOuArray(curCass, element,ref count);
                    foreach (string[] info in tempInfos)
                    {
                        Ainfos.Add(info);

                    }
                }
            }
            return Ainfos;
        }


    }
}
