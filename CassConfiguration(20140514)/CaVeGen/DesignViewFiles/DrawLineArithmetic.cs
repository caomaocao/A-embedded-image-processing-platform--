/*******************************************************************************
           ** Copyright (C) 2009 CASS ��Ȩ����
           ** �ļ�����CassView.cs 
           ** ����������
           **          ��������ؼ�֮�����ߵ��м�������꣬�Լ�����֮��Ľ������꣬���������ƿ����пؼ�
           ** ���ߣ����罡
           ** ��ʼʱ�䣺2009-3-5
           ** 
********************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using CaVeGen.DesignViewFiles.FilterProperty;

namespace CaVeGen.DesignViewFiles
{
    public struct LineInfo
    {
        public Point StartPoint;
        public Point EndPoint;
        public string StartInfo;//������ʼģ������������������Ϣ
        public string EndInfo;//�������ģ���������յ��������Ϣ
        public List<Point> MiddlePoint;
        public List<Point> CrossPoint;
    } 
   
    public struct BlockInfo
    {
        public int BlockNum;
        public string BlockName;
        public Point StartPoint;
        public int Width;
        public int Height;
    }

    public class DrawLineArithmetic
    {
        private int StartPointOut = 10;//��ʼ����������һ�м���Ĭ�Ͼ���
        private List<BlockInfo> CurrentBlocks = new List<BlockInfo>();
        public List<LineInfo> ModifyLines = new List<LineInfo>();
        private int MaxModifyTime = 8;//ÿһ��·�������������������

        public DrawLineArithmetic(List<LineInfo> Lines, List<BlockInfo> Blocks)
        {            
            CurrentBlocks = Blocks;
            ModifyLines = Lines;

            for (int i = 0; i < ModifyLines.Count; i++)
            {
                BlockInfo Sctrl = new BlockInfo();
                BlockInfo Ectrl = new BlockInfo();
                LineInfo line = ModifyLines[i];
                foreach (BlockInfo block in Blocks)
                {
                    if (block.BlockName == line.StartInfo.Split(';')[0])
                    { Sctrl = block; }
                    else if (block.BlockName == line.EndInfo.Split(';')[0])
                    { Ectrl = block; }
                }

                List<Point> tempMpoint = drawLine(line.StartPoint, line.EndPoint, Sctrl, Ectrl);
                line.MiddlePoint = ClearWay(tempMpoint, line.StartPoint, line.EndPoint, Sctrl, Ectrl);
                ModifyLines[i] = line;
            }
        }

        /// <summary>
        /// ���·���м���������ظ��ĵ㡢�ظ���·���Լ��ƿ�����յ�ģ��
        /// </summary>
        /// <param name="OldWay">�����ظ����·��</param>
        /// <param name="StartP">ֱ�����</param>
        /// <param name="EndP">ֱ���յ�</param>
        /// <param name="Sctrl">��ʼģ��</param>
        /// <param name="Ectrl">�յ�ģ��</param>
        /// <returns>�������ظ����·��</returns>
        private List<Point> ClearWay(List<Point> OldWay, Point StartP, Point EndP, BlockInfo Sctrl, BlockInfo Ectrl)
        {
            List<Point> NewWay = new List<Point>();
            Point FirstMpoint = new Point(StartP.X + StartPointOut, StartP.Y);
            Point LastMpoint = new Point(EndP.X - StartPointOut, EndP.Y);
            //������ڵ���ڵ�
            NewWay.Add(FirstMpoint);
            NewWay.AddRange(OldWay);
            NewWay.Add(LastMpoint);

            ClearPoint(ref NewWay);

            //�ƿ�����յ�ģ��
            int index = NewWay.Count - 1;
            if (index > 2)
            {
                if (NewWay[0].Y == NewWay[1].Y && NewWay[0].Y == StartP.Y
                    && StartP.X > NewWay[1].X && StartP.X < NewWay[0].X)
                {
                    if (NewWay[2].Y > NewWay[1].Y)
                    {
                        NewWay.Insert(2, new Point(NewWay[1].X, Sctrl.StartPoint.Y + Sctrl.Height));
                        NewWay[1] = new Point(NewWay[0].X, NewWay[2].Y);
                    }
                    else
                    {
                        NewWay.Insert(2, new Point(NewWay[1].X, Sctrl.StartPoint.Y));
                        NewWay[1] = new Point(NewWay[0].X, NewWay[2].Y);
                    }
                }
                if (NewWay[index].Y == NewWay[index - 1].Y && NewWay[index].Y == EndP.Y
                    && EndP.X > NewWay[index].X && EndP.X < NewWay[index - 1].X)
                {
                    if (NewWay[index - 2].Y > NewWay[index - 1].Y)
                    {
                        NewWay[index - 1] = new Point(NewWay[index].X, Ectrl.StartPoint.Y + Ectrl.Height);
                        NewWay.Insert(index - 1, new Point(NewWay[index - 2].X, Ectrl.StartPoint.Y + Ectrl.Height));
                    }
                    else
                    {
                        NewWay[index - 1] = new Point(NewWay[index].X, Ectrl.StartPoint.Y);
                        NewWay.Insert(index - 1, new Point(NewWay[index - 2].X, Ectrl.StartPoint.Y));
                    }
                }
            }
            return NewWay;
        }

        /// <summary>
        /// �������ֱ�߼��� ���������
        /// ���յ�����С���������
        /// ��������ͬ���ܳ��ȶ̵�����
        /// </summary>
        /// <param name="UnorderList"></param>
        /// <returns></returns>
        private List<List<Point>> OrderLine(List<List<Point>> UnorderList)
        {
            List<List<Point>> OrderList = new List<List<Point>>();
            foreach (List<Point> line in UnorderList)
            {//����������·�����������������
                List<Point> tempLine = new List<Point>();
                tempLine.AddRange(line);//�����㼯����
                if (tempLine.Count > 2)
                {//ֻ�е㼯����2ʱ�Ż���ֶ����
                    ClearPoint(ref tempLine);
                }

                if (OrderList.Count == 0)
                {
                    OrderList.Add(tempLine);
                }
                else
                {
                    bool isInsert = false;
                    for (int i = 0; i < OrderList.Count; i++)
                    {
                        
                        if (tempLine.Count < OrderList[i].Count 
                            || tempLine.Count == OrderList[i].Count //��������ͬ��Ƚ�·������
                            && CalculateWayLength(tempLine) < CalculateWayLength(OrderList[i]))
                        {
                            OrderList.Insert(i, tempLine);
                            isInsert = true;
                            break;
                        }
                    }
                    if (!isInsert)
                    {
                        OrderList.Add(tempLine);
                    }
                }
            }
            return OrderList;
        }





        /// <summary>
        /// ȥ��·���������ظ��ĵ��ͬһֱ���ϵ��м��
        /// </summary>
        /// <param name="Oldway"></param>
        private void ClearPoint(ref List<Point> NewWay)
        {
            bool isError = true;
            while (isError)
            {
                isError = false;
                for (int j = 1; j < NewWay.Count - 1; j++)
                {
                    if ((NewWay[j - 1].X == NewWay[j].X && NewWay[j].X == NewWay[j + 1].X) 
                        || (NewWay[j - 1].Y == NewWay[j].Y && NewWay[j].Y == NewWay[j + 1].Y))
                    {//��������ͬ��
                        NewWay.Remove(NewWay[j]);
                        isError = true;
                    }
                    if (NewWay[j - 1] == NewWay[j])
                    {//����������ͬ
                        NewWay.RemoveAt(j);
                        isError = true;
                    }
                }
            }
        }



        private List<Point> drawLine(Point start, Point end, BlockInfo Sctrl, BlockInfo Ectrl)
        {
            //��ʼ������ͽ�β��ȥ���Ƿ��谭����//������ 

            //����ģ��ܽ������
            Point FirstMiddlePoint = new Point(start.X + StartPointOut, start.Y);
            Point LastMiddlePoint = new Point(end.X - StartPointOut, end.Y);

            //Ѱ�������м�·��
            if (FirstMiddlePoint.X <= LastMiddlePoint.X)//������� �յ�����
            {
                if (FirstMiddlePoint.Y < LastMiddlePoint.Y)//��������� �յ�������
                { return LeftUpToRightDown(FirstMiddlePoint, LastMiddlePoint, Sctrl, Ectrl); }
                else//��������� �յ�������
                { return LeftDownToRightUp(FirstMiddlePoint, LastMiddlePoint, Sctrl, Ectrl); }

            }
            else//��������յ�����
            {
                if (FirstMiddlePoint.Y < LastMiddlePoint.Y)//��������� �յ�������
                { return RightUpToLeftDown(FirstMiddlePoint, LastMiddlePoint, Sctrl, Ectrl); }
                else//��������� �յ�������
                { return RightDownToLeftUp(FirstMiddlePoint, LastMiddlePoint, Sctrl, Ectrl); }
            }
        }              


        //���µ����ϵ�����
        private List<Point> RightDownToLeftUp(Point start, Point end, BlockInfo Sctrl, BlockInfo Ectrl)
        {   //���ַ���   
            List<Point> WayA =
                new List<Point>(new Point[] { start, new Point(start.X, Ectrl.StartPoint.Y + Ectrl.Height), new Point(end.X, Ectrl.StartPoint.Y + Ectrl.Height), end });
            List<Point> WayB =
                new List<Point>(new Point[] { start, new Point(start.X, Sctrl.StartPoint.Y), new Point(end.X, Sctrl.StartPoint.Y), end });
            List<Point> WayC =
                new List<Point>(new Point[] { start, new Point(start.X, Ectrl.StartPoint.Y), new Point(end.X, Ectrl.StartPoint.Y), end });
            List<Point> WayD =
                new List<Point>(new Point[] { start, new Point(start.X, Sctrl.StartPoint.Y + Sctrl.Height), new Point(end.X, Sctrl.StartPoint.Y + Sctrl.Height), end });

            return ChooseRightWay(WayA, WayB, WayC, WayD);
        }

        private List<Point> RightUpToLeftDown(Point start, Point end, BlockInfo Sctrl, BlockInfo Ectrl)
        {   //���ַ���   
            List<Point> WayA =
                new List<Point>(new Point[] { start, new Point(start.X, Ectrl.StartPoint.Y), new Point(end.X, Ectrl.StartPoint.Y), end });
            List<Point> WayB =
                new List<Point>(new Point[] { start, new Point(start.X, Sctrl.StartPoint.Y + Sctrl.Height), new Point(end.X, Sctrl.StartPoint.Y + Sctrl.Height), end });
            List<Point> WayC =
                new List<Point>(new Point[] { start, new Point(start.X, Ectrl.StartPoint.Y + Ectrl.Height), new Point(end.X, Ectrl.StartPoint.Y + Ectrl.Height), end });
            List<Point> WayD =
                new List<Point>(new Point[] { start, new Point(start.X, Sctrl.StartPoint.Y), new Point(end.X, Sctrl.StartPoint.Y), end });

            return ChooseRightWay(WayA, WayB, WayC, WayD);
        }

        private List<Point> LeftDownToRightUp(Point start, Point end, BlockInfo Sctrl, BlockInfo Ectrl)
        {   //���ַ���  
            List<Point> WayA =
                new List<Point>(new Point[] { start, new Point(start.X, end.Y), end });
            List<Point> WayB =
                new List<Point>(new Point[] { start, new Point(end.X, start.Y), end });
            List<Point> WayC =
                new List<Point>(new Point[] { start, new Point(start.X, Ectrl.StartPoint.Y), new Point(end.X, Ectrl.StartPoint.Y), end });
            //List<Point> WayD =
            //    new List<Point>(new Point[] { start, new Point(Ectrl.StartPoint.X + Ectrl.Width, start.Y), new Point(Ectrl.StartPoint.X + Ectrl.Width, Ectrl.StartPoint.Y),new Point(end.X,Ectrl.StartPoint.Y), end });

            return ChooseRightWay(WayA, WayB, WayC, null);
        }

        private List<Point> LeftUpToRightDown(Point start, Point end, BlockInfo Sctrl, BlockInfo Ectrl)
        {   //���ַ���   
            List<Point> WayA =
                new List<Point>(new Point[] { start, new Point(start.X, end.Y), end });
            List<Point> WayB =
                new List<Point>(new Point[] { start, new Point(end.X, start.Y), end });
            List<Point> WayC =
                new List<Point>(new Point[] { start, new Point(start.X, Ectrl.StartPoint.Y + Ectrl.Height), new Point(end.X, Ectrl.StartPoint.Y + Ectrl.Height), end });
            //List<Point> WayD =
            //    new List<Point>(new Point[] { start, new Point(Ectrl.StartPoint.X + Ectrl.Width, start.Y), new Point(Ectrl.StartPoint.X + Ectrl.Width, Ectrl.StartPoint.Y + Ectrl.Height), new Point(end.X, Ectrl.StartPoint.Y + Ectrl.Height), end });

            return ChooseRightWay(WayA, WayB, WayC, null);
        }



        private List<Point> ChooseRightWay(List<Point> WayA, List<Point> WayB, List<Point> WayC, List<Point> WayD)
        {//����δ�������� �����ǵ�������
            List<List<Point>> tempWays = new List<List<Point>>();//��ŵ�����ķ���
            List<Point> ReturnWay = new List<Point>();
            List<Point> ModifyWayA = WayIsBlock(WayA, MaxModifyTime);
            List<Point> ModifyWayB = WayIsBlock(WayB, MaxModifyTime);
            List<Point> ModifyWayC = WayIsBlock(WayC, MaxModifyTime);

            if (ModifyWayA != null)
            {
                if (ModifyWayA.Count != 0)
                {
                    if (WayIsBlock(ModifyWayA, MaxModifyTime).Count == 0)
                        tempWays.Add(ModifyWayA);
                }
                else
                { return WayA; }
            }
            if (ModifyWayB != null)
            {
                if (ModifyWayB.Count != 0)
                {
                    if (WayIsBlock(ModifyWayB, MaxModifyTime).Count == 0)
                        tempWays.Add(ModifyWayB);
                }
                else
                { return WayB; }
            }
            //if (WayC != null)
            //{
            //    List<Point> ModifyWayC = WayIsBlock(WayC, MaxModifyTime);
            if (ModifyWayC != null)
            {
                if (ModifyWayC.Count != 0)
                {
                    if (WayIsBlock(ModifyWayC, MaxModifyTime).Count == 0)
                        tempWays.Add(ModifyWayC);
                }
                else
                { return WayC; }
            }
            //}
            if (WayD != null)
            {
                List<Point> ModifyWayD = WayIsBlock(WayD, MaxModifyTime);
                if (ModifyWayD != null)
                {
                    if (ModifyWayD.Count != 0)
                    { tempWays.Add(ModifyWayD); }
                    else
                    { return WayD; }
                }
            }
            if (tempWays.Count != 0)
            {
                return OrderLine(tempWays)[0];
            }
            else
            { return WayA; }
        }

        /// <summary>
        /// ����Ӧ�㼯 �ж��Ƿ���� �����򷵻�ԭ·�� �������򷵻�һ��������ķ����㼯 ����ʧ���򷵻ؿ�
        /// </summary>
        /// <param name="Way"></param>
        /// <param name="ModifyTime"></param>
        /// <returns></returns>
        private List<Point> WayIsBlock(List<Point> Way, int ModifyTime)
        {
            List<Point> NewWay = new List<Point>();
            bool Modifyfail = false;
            bool IsModify = false;
            int CurrentTime = ModifyTime;

            //��·���
            Point pointA = Way[0];
            Point pointB = new Point();
            Point pointC = Way[Way.Count - 1];
            NewWay.Add(pointA);//��һ��          

            for (int i = 1; i < Way.Count; i++)
            {
                pointB = Way[i];
                List<List<Point>> ModifyLine = LineIsBlock(pointA, pointB, --CurrentTime);

                if (CurrentTime < 0 || ModifyLine == null)
                { return null; }
                else if (ModifyLine.Count != 0)//��һ����ڶ����ֱ�����ϰ�
                {
                    List<List<Point>> ModifyLineII = new List<List<Point>>();//�����޸ĵķ�������
                    IsModify = true;
                    ModifyLine = OrderLine(ModifyLine);
                    foreach (List<Point> ModifyWay in ModifyLine)//�������ص�·���޸ķ���
                    {
                        for (int j = i + 1; j < Way.Count; j++)
                        { ModifyWay.Add(Way[j]); }
                        List<Point> ModifyWayII = WayIsBlock(ModifyWay, --CurrentTime);
                        if (ModifyWayII == null)
                        { continue; }
                        else
                        {
                            if (ModifyWayII.Count != 0)
                            {
                                List<Point> NewWayII = new List<Point>();
                                NewWayII.AddRange(NewWay);
                                NewWayII.AddRange(ModifyWayII);
                                ModifyLineII.Add(NewWayII);
                            }
                            else if (ModifyWayII.Count == 0)
                            {
                                NewWay.AddRange(ModifyWay);
                                if (NewWay.Count > 2)
                                {//����������2ʱ���ܳ��������
                                    ClearPoint(ref NewWay);
                                }
                                return NewWay;
                            }
                        }
                    }
                    if (ModifyLineII.Count != 0)
                    {
                        return OrderLine(ModifyLineII)[0];
                    }
                }
                else //��һ�ڶ����ֱ�����ϰ���ڶ���ȷ��
                {
                    CurrentTime++;
                    NewWay.Add(pointB);
                    pointA = pointB;
                }
            }
            NewWay.Add(pointC);//·���յ�
            if (IsModify == false)
            {//ԭ����û���޸ķ���һ������
                NewWay.Clear();
                return NewWay;
            }
            else if (Modifyfail == false)
            { return NewWay; }//�˷���·�� ò�ƿ���ɾ�� 
            else//ԭ�����޸ģ���û�к��ʵ��޸ķ����򷵻�null
            { return null; }
        }

        /// <summary>
        /// �ж�һ���߶��Ƿ�����
        /// </summary>
        /// <param name="LineFirstPoint">�߶ε����</param>
        /// <param name="LineSecondPoint">�߶ε��յ�</param>
        /// <param name="ModifyTime">�߶�ʣ����޸ĵĴ���</param>
        /// <returns>����һ����������,���û���޸���Ϊ��,�������null��˵�������˷���</returns>
        private List<List<Point>> LineIsBlock(Point LineFirstPoint, Point LineSecondPoint, int ModifyTime)
        {
            List<List<Point>> ModifyPoints = new List<List<Point>>();
            List<List<Point>> tempModifyLines = new List<List<Point>>();
            List<Point> tempModifyWay = new List<Point>();

            if (ModifyTime <= 0)//����޸Ĵ����þ���Ȼ���裬��������ǰ����
            { return null; }

            foreach (BlockInfo tempBlock in CurrentBlocks)
            {
                Point StartP = LineFirstPoint;
                Point EndP = LineSecondPoint;
                //��ǰ�ؼ����Ľ�����
                Point LeftUp = tempBlock.StartPoint;
                Point RightUp = new Point(LeftUp.X + tempBlock.Width, LeftUp.Y);
                Point LeftDown = new Point(LeftUp.X, LeftUp.Y + tempBlock.Height);
                Point RightDown = new Point(RightUp.X, LeftDown.Y);
                List<Point> Crosses = WaysIsCross(new List<Point>(new Point[] { StartP, EndP }),tempBlock);//ֱ���������ߵĽ��㼯        

                if (Crosses.Count == 0)//����
                { continue; }
                else if (Crosses.Count == 1)
                {
                    #region һ���ڿؼ���
                    //ƽ�������ڹյ�
                    //һ���������������ƽ�ƾ��� ��Ϊ���������ھ���
                    List<Point> Way1 = new List<Point>();
                    List<Point> Way2 = new List<Point>();
                    List<Point> Way3 = new List<Point>();
                    List<Point> Way4 = new List<Point>();
                    Point CrossP = Crosses[0];
                    bool isReverse = false;//�Ƿ��Ƿ���

                    if (EndP.X < LeftUp.X || EndP.X > RightUp.X)//�ڶ����������������ʼ������
                    {
                        isReverse = true;
                        Point tempP = StartP;
                        StartP = EndP;
                        EndP = tempP;
                    }

                    if (StartP.Y == EndP.Y)//ֱ��Ϊˮƽ
                    {
                        //ƽ�Ʒ���
                        //��ƽ��                        
                        Way1.Add(new Point(StartP.X, LeftUp.Y));
                        Way1.Add(new Point(EndP.X, LeftUp.Y));

                        //��ƽ��                    
                        Way2.Add(new Point(StartP.X, LeftDown.Y));
                        Way2.Add(new Point(EndP.X, LeftDown.Y));

                        //�յ㷽��
                        //�Ϲ�
                        Way3.Add(StartP);
                        Way3.Add(CrossP);
                        Way3.Add(new Point(CrossP.X, LeftUp.Y));
                        Way3.Add(new Point(EndP.X, LeftUp.Y));

                        //�¹�
                        Way4.Add(StartP);
                        Way4.Add(CrossP);
                        Way4.Add(new Point(CrossP.X, LeftDown.Y));
                        Way4.Add(new Point(EndP.X, LeftDown.Y));
                    }
                    else if (StartP.X == EndP.X)//ֱ��Ϊ��ֱ
                    {
                        //ƽ�Ʒ���
                        //��ƽ��                   
                        Way1.Add(new Point(LeftUp.X, StartP.Y));
                        Way1.Add(new Point(LeftUp.X, EndP.Y));

                        //��ƽ��                   
                        Way2.Add(new Point(RightUp.X, StartP.Y));
                        Way2.Add(new Point(RightUp.X, EndP.Y));

                        //�յ㷽��
                        //���
                        Way3.Add(StartP);
                        Way3.Add(CrossP);
                        Way3.Add(new Point(LeftUp.X, CrossP.Y));
                        Way3.Add(new Point(LeftUp.X, EndP.Y));

                        //�ҹ�
                        Way4.Add(StartP);
                        Way4.Add(CrossP);
                        Way4.Add(new Point(RightUp.X, CrossP.Y));
                        Way4.Add(new Point(RightUp.X, EndP.Y));
                    }


                    if (isReverse)
                    {
                        tempModifyLines.Add(ReverseLine(Way1));
                        tempModifyLines.Add(ReverseLine(Way2));
                        tempModifyLines.Add(ReverseLine(Way3));
                        tempModifyLines.Add(ReverseLine(Way4));
                    }
                    else
                    {
                        tempModifyLines.Add(Way1);
                        tempModifyLines.Add(Way2);
                        tempModifyLines.Add(Way3);
                        tempModifyLines.Add(Way4);
                    }
                    #endregion
                }
                else if (Crosses.Count == 2)
                {
                    #region ֱ�ߴ����ؼ�
                    Point CrossP1 = Crosses[0];
                    Point CrossP2 = Crosses[1];
                    List<Point> Way1 = new List<Point>();//���ȵ�·��
                    List<Point> Way2 = new List<Point>();//�����ȵ�·��
                    List<Point> Way3 = new List<Point>();
                    List<Point> Way4 = new List<Point>();

                    if (StartP.Y > EndP.Y || EndP.X > StartP.X)//ֱ��������»��������򽻻�������������
                    {
                        Point tempP = CrossP1;
                        CrossP1 = CrossP2;
                        CrossP2 = tempP;
                    }

                    if (CrossP1.X == CrossP2.X)//���½�
                    {
                        //��ƽ��
                        Way1.Add(StartP);
                        Way1.Add(new Point(RightUp.X, StartP.Y));
                        Way1.Add(new Point(RightUp.X, EndP.Y));

                        //�ҹ�ƽ��
                        Way2.Add(StartP);
                        Way2.Add(CrossP1);
                        Way2.Add(new Point(RightUp.X, CrossP1.Y));
                        Way2.Add(new Point(RightUp.X, EndP.Y));

                        //��ƽ��
                        Way3.Add(StartP);
                        Way3.Add(new Point(LeftUp.X, StartP.Y));
                        Way3.Add(new Point(LeftUp.X, EndP.Y));

                        //���ƽ��
                        Way4.Add(StartP);
                        Way4.Add(CrossP1);
                        Way4.Add(new Point(LeftUp.X, CrossP1.Y));
                        Way4.Add(new Point(LeftUp.X, EndP.Y));
                        //int D_left = StartP.X - LeftUp.X;
                        //int D_right = RightUp.X - StartP.X;
                        if (StartP.X * 2 - LeftUp.X - RightUp.X < 0)//�������ƽ������
                        {
                            List<Point> tempWay = Way1;
                            Way1 = Way3;
                            Way3 = tempWay;
                            tempWay = Way2;
                            Way2 = Way4;
                            Way4 = tempWay;
                        }
                    }
                    else if (CrossP1.Y == CrossP2.Y)//���ҽ�
                    {
                        //��ƽ��
                        Way1.Add(StartP);
                        Way1.Add(new Point(StartP.X, RightDown.Y));
                        Way1.Add(new Point(EndP.X, RightDown.Y));

                        //�¹�ƽ��
                        Way2.Add(StartP);
                        Way2.Add(CrossP1);
                        Way2.Add(new Point(CrossP1.X, LeftDown.Y));
                        Way2.Add(new Point(EndP.X, LeftDown.Y));

                        //��ƽ��
                        Way3.Add(StartP);
                        Way3.Add(new Point(StartP.X, LeftUp.Y));
                        Way3.Add(new Point(EndP.X, LeftUp.Y));

                        //�Ϲ�ƽ��
                        Way4.Add(StartP);
                        Way4.Add(CrossP1);
                        Way4.Add(new Point(CrossP1.X, RightUp.Y));
                        Way4.Add(new Point(EndP.X, RightUp.Y));

                        //int D_up = StartP.Y - LeftUp.Y;
                        //int D_down = LeftDown.Y - StartP.Y;
                        if (StartP.Y * 2 - LeftUp.Y - LeftDown.Y < 0)//�Ͻ�����ƽ������
                        {
                            List<Point> tempWay = Way1;
                            Way1 = Way3;
                            Way3 = tempWay;
                            tempWay = Way2;
                            Way2 = Way4;
                            Way4 = tempWay;
                        }
                    }

                    tempModifyLines.Add(Way1);
                    tempModifyLines.Add(Way2);
                    tempModifyLines.Add(Way3);
                    tempModifyLines.Add(Way4);
                    #endregion
                }
            }
            return judgeWay(tempModifyLines, ModifyTime - 1);
        }




        /// <summary>
        /// ��ֱ�ߵĵ�˳����
        /// </summary>
        /// <param name="LinePoints">˳���·��</param>
        /// <returns>�����·��</returns>
        private List<Point> ReverseLine(List<Point> LinePoints)
        {
            List<Point> ReturnLine = new List<Point>();
            for (int i = 1; i <= LinePoints.Count; i++)
            {
                ReturnLine.Add(LinePoints[LinePoints.Count - i]);
            }
            return ReturnLine;
        }

        /// <summary>
        /// �ж������ķ����Ŀ�����
        /// </summary>
        /// <param name="ways">�޸ķ�������</param>
        /// <param name="ModifyTime">ʣ���޸Ĵ���</param>
        /// <returns>���÷�����</returns>
        private List<List<Point>> judgeWay(List<List<Point>> ways, int ModifyTime)
        {
            List<List<Point>> ModifyPoints = new List<List<Point>>();
            List<List<Point>> tempModifyLines = new List<List<Point>>();
            List<Point> tempModifyWay = new List<Point>();

            foreach (List<Point> way in ways)
            {
                tempModifyWay = WayIsBlock(way, ModifyTime);
                if (tempModifyWay != null && tempModifyWay.Count != 0)
                { ModifyPoints.Add(tempModifyWay); }
                else if (tempModifyWay != null && tempModifyWay.Count == 0)
                { ModifyPoints.Add(way); }
            }
            return ModifyPoints;
        }

        /// <summary>
        /// ���������߶μ�Ľ��㣬�粻�ཻ�򷵻�(0,0)
        /// </summary>
        /// <param name="FirstLineStartPoint">��һ���߶��������</param>
        /// <param name="FirstLineEndPoint">��һ���߶��յ�����</param>
        /// <param name="SecondLineStartPoint">�ڶ����߶��������</param>
        /// <param name="SecondLineEndPoint">�ڶ����߶��յ�����</param>
        /// <returns>�����߶ν�������</returns>
        private Point LinesIscross(Point FirstLineStartPoint, Point FirstLineEndPoint, Point SecondLineStartPoint, Point SecondLineEndPoint)
        {
            Point H1 = new Point();//�������
            Point H2 = new Point();//�����ҵ�
            Point S1 = new Point();//�����ϵ�
            Point S2 = new Point();//�����µ�
            Point Cross = new Point();//���߽���
            if (FirstLineStartPoint.X == FirstLineEndPoint.X && SecondLineStartPoint.Y == SecondLineEndPoint.Y)//��1Ϊ���ߣ���2Ϊ����
            {
                if (FirstLineStartPoint.Y < FirstLineEndPoint.Y)//��1������ϣ��յ�����
                {
                    S1 = FirstLineStartPoint;
                    S2 = FirstLineEndPoint;
                }
                else//��1������£��յ�����
                {
                    S1 = FirstLineEndPoint;
                    S2 = FirstLineStartPoint;
                }
                if (SecondLineStartPoint.X < SecondLineEndPoint.X)//��2��������յ�����
                {
                    H1 = SecondLineStartPoint;
                    H2 = SecondLineEndPoint;
                }
                else//��2������ң��յ�����
                {
                    H1 = SecondLineEndPoint;
                    H2 = SecondLineStartPoint;
                }
            }
            else if (FirstLineStartPoint.Y == FirstLineEndPoint.Y && SecondLineStartPoint.X == SecondLineEndPoint.X)//��2Ϊ���ߣ���1Ϊ����
            {
                if (FirstLineStartPoint.X < FirstLineEndPoint.X)//��1��������յ�����
                {
                    H1 = FirstLineStartPoint;
                    H2 = FirstLineEndPoint;
                }
                else//��1������ң��յ�����
                {
                    H1 = FirstLineEndPoint;
                    H2 = FirstLineStartPoint;
                }
                if (SecondLineStartPoint.Y < SecondLineEndPoint.Y)//��2������ϣ��յ�����
                {
                    S1 = SecondLineStartPoint;
                    S2 = SecondLineEndPoint;
                }
                else//��2������£��յ�����
                {
                    S1 = SecondLineEndPoint;
                    S2 = SecondLineStartPoint;
                }
            }
            if (S1.X > H1.X && S1.X < H2.X && H1.Y > S1.Y && H1.Y < S2.Y)
            {
                Cross.X = S1.X;
                Cross.Y = H1.Y;
            }
            return Cross;
        }

        /// <summary>
        /// ����һ����·������ؼ��Ľ���
        /// </summary>
        /// <param name="curWay">·�߷���</param>
        /// <param name="ctrlBlock">�ؼ�����Ϣ</param>
        /// <returns>���ؽ���㼯</returns>
        private List<Point> WaysIsCross(List<Point> curWay, BlockInfo ctrlBlock)
        {
            //��ǰ�ؼ����Ľ�����
            Point LeftUp = ctrlBlock.StartPoint;
            Point RightUp = new Point(LeftUp.X + ctrlBlock.Width, LeftUp.Y);
            Point LeftDown = new Point(LeftUp.X, LeftUp.Y + ctrlBlock.Height);
            Point RightDown = new Point(RightUp.X, LeftDown.Y);
            List<Point> BlockOutline = new List<Point>(new Point[] { LeftUp, RightUp, RightDown, LeftDown, LeftUp });//�ؼ���������
            List<Point> CrossPoints = new List<Point>();//���㼯��

            for (int i = 1; i < curWay.Count; i++)
            {//��·�е�ÿһ�߶��������ߵĽ��㼯
                CrossPoints.AddRange(WaysIsCross(new List<Point>(new Point[] { curWay[i - 1], curWay[i] }), BlockOutline));
            }
            return CrossPoints;
        }


        /// <summary>
        /// ����������·����֮��Ľ���
        /// </summary>
        /// <param name="FirstWay">��һ����·</param>
        /// <param name="SecondWay">�ڶ���·��</param>
        /// <returns>����������·�Ľ���㼯</returns>
        private List<Point> WaysIsCross(List<Point> FirstWay, List<Point> SecondWay)
        {
            List<Point> CrossPoints = new List<Point>();
            Point F1 = FirstWay[0];
            Point F2 = new Point();
            Point S1 = SecondWay[0];
            Point S2 = new Point();
            Point tempCross = new Point();
            for (int i = 1; i < FirstWay.Count; i++)
            {
                F2 = FirstWay[i];
                for (int j = 1; j < SecondWay.Count; j++)
                {
                    S2 = SecondWay[j];
                    tempCross = LinesIscross(F1, F2, S1, S2);
                    if (tempCross != new Point(0, 0))
                    { CrossPoints.Add(tempCross); }
                    S1 = S2;
                }
                F1 = F2;
            }
            return CrossPoints;
        }

        /// <summary>
        /// ����·����;,���س���ֵ
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        private int CalculateWayLength(List<Point> way)
        {
            int length = 0;
            for (int i = 1; i < way.Count; i++)
            {
                if (way[i].X == way[i - 1].X)
                {
                    length += Math.Abs(way[i].Y - way[i - 1].Y);
                }
                else if (way[i].Y == way[i - 1].Y)
                {
                    length += Math.Abs(way[i].X - way[i - 1].X);
                }
            }
            return length;
        }

    }
}
