using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using  System.Diagnostics;

namespace CaVeGen.DesignViewFiles.CreateILXML
{

   
    public struct OrderBlock
    { //�����ýṹ��
        public Point LUpoint;//���ϵ�����
        public List<ControlInfo> ContainCtrls;//�����Ŀؼ�
        public List<string> InputPorts;//�ϲ�ӿ�
    }


    class ConfigOrder
    {

        //��ſؼ���������Ҫ��������ʱ�������ļ���
        public List<string> TempInfo = new List<string>();
        private CassView cassview = new CassView();

        public ConfigOrder(CassView curCas, string[] pageNameInfo)
        {
            this.cassview = curCas;
        }




        private List<ControlInfo> StartOrder(List<ControlInfo> GroupCtrl, string pageName)
        {
            List<ControlInfo> EndCtrls = new List<ControlInfo>();//β�˿ؼ��б�
            foreach (ControlInfo ElementCtrl in GroupCtrl)
            {
                if (CheckTipCtrl(ElementCtrl.OutputInfo))
                {//β�˿ؼ�//δ������������ӵ�β�˿ؼ� ����NULL//��ʱ�ж�����ΪNULL������NULL
                    EndCtrls.Add(ElementCtrl);
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
            if (GroupCtrl.Count != 0 && EndCtrls.Count == 0)
            {
                //CassViewGenerator.SpecialErrors.Add(new string[] { null, "�����������·", "error", pageName });
                //ViewErrorinfo.Add(new string[] { pageName, null, "�����������·" });
            }
            return EndCtrls;
        }






        /// <summary>
        /// ���������ؼ���Ϣ���Ͻ��������뵽���������
        /// </summary>
        /// <param name="GroupCtrl">δ�����һ���ؼ���</param>
        /// <returns>����һ���Ѿ�����Ŀؼ�����</returns>
        private ControlInfo[] OrderCtrlsNum(List<ControlInfo> GroupCtrl, string pageName)
        {
            Stack<ControlInfo> TempStack = new Stack<ControlInfo>();//���������õ�����ʱ��ջ  
            Stack<ControlInfo> OrderStack = new Stack<ControlInfo>();//�ѽ�������Ķ�ջ            
            List<string> tempCode = new List<string>();//�Ե����ϱ���ʱ��˳������ʱ�����Ķ���
            List<OrderBlock> TempOB = new List<OrderBlock>();//����������б�

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
            List<ControlInfo> EndCtrls = StartOrder(GroupCtrl, pageName);
            #endregion

            #region �Ե����Ͽؼ������㷨
            //��һ��β�˿ؼ�ѹ�������ջ,�����ֶϲ���δ����ʱ(��������ʱ������������)
            //������ؼ������е������������ѹ��ڶ���β�˿ؼ����μ���
            //���ϲ�������δ���㵽����ʱ�ȶԵ�ǰ��ϲ���ص�������Ŀؼ��������������������
            for (int s = 0; s < EndCtrls.Count; s++)
            {
                TempStack.Push(EndCtrls[s]);
                //OrderBlock newBlock = initializationOB();
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
                }
                TempOB.Add(CreateOB(OrderStack));
                OrderStack.Clear();
                //if (TempStack.Count == 0 && GroupCtrl.Count != 0)
                //{//���пؼ�û������,������Ϊ��ʱ������ԭ��ʹ��ջΪ��
                //    for (int i = 0; i < GroupCtrl.Count; i++)
                //    {//��δ����ؼ������ҵ��Ѿ����Ա����õĿؼ�ѹ����ʱ��ջ
                //        if (TempInfo.Contains(GroupCtrl[i].OutputInfo[0][1]) || GroupCtrl[i].OutputInfo.Count != 1)
                //        {
                //            for (int j = 0; j < GroupCtrl[i].OutputInfo.Count; j++)
                //            {
                //                if (GroupCtrl[i].OutputInfo[j][0] != null
                //                    && !tempCode.Contains(GroupCtrl[i].OutputInfo[j][1]))
                //                {//�����������ʹ�ù�����ʱ�����в�����������
                //                    break;
                //                }
                //                if (j == GroupCtrl[i].OutputInfo.Count - 1)
                //                {//����������������Ҷ��Ѿ�ʹ�ù�
                //                    TempStack.Push(GroupCtrl[i]);
                //                }
                //            }
                //        }
                //    }
                //}

            }
            #endregion

            return OrderStack.ToArray();
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
        /// ��ʼ������ṹ��
        /// </summary>
        /// <returns></returns>
        private OrderBlock initializationOB()
        {
            OrderBlock newOB = new OrderBlock();
            newOB.ContainCtrls = new List<ControlInfo>();
            newOB.InputPorts = new List<string>();
            return newOB;
        }


        private OrderBlock CreateOB(Stack<ControlInfo> orderS)
        {
            OrderBlock newOB = initializationOB();
            newOB.ContainCtrls.AddRange(orderS);
            //newOB.LUpoint = cassview.FindBlockInfo(orderS[0]).StartPoint;//��һ�ؼ����������
            return newOB;
        }




    }
}
