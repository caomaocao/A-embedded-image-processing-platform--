/*******************************************************************************
           ** Copyright (C) 2009 CASS 版权所有
           ** 文件名：CassView.cs 
           ** 功能描述：
           **          用于算出控件之间连线的中间各点坐标，以及连线之间的交点坐标，并尽可能绕开现有控件
           ** 作者：宋骁健
           ** 创始时间：2009-3-5
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
        public string StartInfo;//保存起始模块名，及起点的引脚信息
        public string EndInfo;//保存结束模块名，及终点的引脚信息
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
        private int StartPointOut = 10;//起始点引出到第一中间点的默认距离
        private List<BlockInfo> CurrentBlocks = new List<BlockInfo>();
        public List<LineInfo> ModifyLines = new List<LineInfo>();
        private int MaxModifyTime = 8;//每一条路线所允许的最大调整次数

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
        /// 清除路径中间点中所有重复的点、重复的路径以及绕开起点终点模块
        /// </summary>
        /// <param name="OldWay">含有重复点的路径</param>
        /// <param name="StartP">直线起点</param>
        /// <param name="EndP">直线终点</param>
        /// <param name="Sctrl">起始模块</param>
        /// <param name="Ectrl">终点模块</param>
        /// <returns>不含有重复点的路径</returns>
        private List<Point> ClearWay(List<Point> OldWay, Point StartP, Point EndP, BlockInfo Sctrl, BlockInfo Ectrl)
        {
            List<Point> NewWay = new List<Point>();
            Point FirstMpoint = new Point(StartP.X + StartPointOut, StartP.Y);
            Point LastMpoint = new Point(EndP.X - StartPointOut, EndP.Y);
            //补齐出口点入口点
            NewWay.Add(FirstMpoint);
            NewWay.AddRange(OldWay);
            NewWay.Add(LastMpoint);

            ClearPoint(ref NewWay);

            //绕开起点终点模块
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
        /// 将传入的直线集合 清理多余点后
        /// 按照点数由小到大的排序
        /// 当点数相同以总长度短的优先
        /// </summary>
        /// <param name="UnorderList"></param>
        /// <returns></returns>
        private List<List<Point>> OrderLine(List<List<Point>> UnorderList)
        {
            List<List<Point>> OrderList = new List<List<Point>>();
            foreach (List<Point> line in UnorderList)
            {//将遍历到的路径清除多余点后再排序
                List<Point> tempLine = new List<Point>();
                tempLine.AddRange(line);//创建点集副本
                if (tempLine.Count > 2)
                {//只有点集大于2时才会出现多余点
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
                            || tempLine.Count == OrderList[i].Count //当点数相同则比较路径长度
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
        /// 去除路径中所有重复的点和同一直线上的中间点
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
                    {//相邻三点同线
                        NewWay.Remove(NewWay[j]);
                        isError = true;
                    }
                    if (NewWay[j - 1] == NewWay[j])
                    {//相邻两点相同
                        NewWay.RemoveAt(j);
                        isError = true;
                    }
                }
            }
        }



        private List<Point> drawLine(Point start, Point end, BlockInfo Sctrl, BlockInfo Ectrl)
        {
            //开始引出点和结尾进去点是否被阻碍处理//不考虑 

            //考虑模块很近的情况
            Point FirstMiddlePoint = new Point(start.X + StartPointOut, start.Y);
            Point LastMiddlePoint = new Point(end.X - StartPointOut, end.Y);

            //寻找最优中间路径
            if (FirstMiddlePoint.X <= LastMiddlePoint.X)//起点在左 终点在右
            {
                if (FirstMiddlePoint.Y < LastMiddlePoint.Y)//起点在左上 终点在右下
                { return LeftUpToRightDown(FirstMiddlePoint, LastMiddlePoint, Sctrl, Ectrl); }
                else//起点在左下 终点在右上
                { return LeftDownToRightUp(FirstMiddlePoint, LastMiddlePoint, Sctrl, Ectrl); }

            }
            else//起点在右终点在左
            {
                if (FirstMiddlePoint.Y < LastMiddlePoint.Y)//起点在右上 终点在左下
                { return RightUpToLeftDown(FirstMiddlePoint, LastMiddlePoint, Sctrl, Ectrl); }
                else//起点在右下 终点在左上
                { return RightDownToLeftUp(FirstMiddlePoint, LastMiddlePoint, Sctrl, Ectrl); }
            }
        }              


        //右下到左上的连线
        private List<Point> RightDownToLeftUp(Point start, Point end, BlockInfo Sctrl, BlockInfo Ectrl)
        {   //四种方案   
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
        {   //四种方案   
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
        {   //两种方案  
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
        {   //两种方案   
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
        {//优先未调整方案 而后是调整方案
            List<List<Point>> tempWays = new List<List<Point>>();//存放调整后的方案
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
        /// 给对应点集 判断是否可行 可行则返回原路线 不可行则返回一个调整后的方案点集 调整失败则返回空
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

            //线路起点
            Point pointA = Way[0];
            Point pointB = new Point();
            Point pointC = Way[Way.Count - 1];
            NewWay.Add(pointA);//第一点          

            for (int i = 1; i < Way.Count; i++)
            {
                pointB = Way[i];
                List<List<Point>> ModifyLine = LineIsBlock(pointA, pointB, --CurrentTime);

                if (CurrentTime < 0 || ModifyLine == null)
                { return null; }
                else if (ModifyLine.Count != 0)//第一点与第二点的直线有障碍
                {
                    List<List<Point>> ModifyLineII = new List<List<Point>>();//二次修改的方案集合
                    IsModify = true;
                    ModifyLine = OrderLine(ModifyLine);
                    foreach (List<Point> ModifyWay in ModifyLine)//遍历返回的路线修改方案
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
                                {//当点数大于2时可能出现冗余点
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
                else //第一第二点的直线无障碍则第二点确定
                {
                    CurrentTime++;
                    NewWay.Add(pointB);
                    pointA = pointB;
                }
            }
            NewWay.Add(pointC);//路径终点
            if (IsModify == false)
            {//原方案没有修改返回一个空列
                NewWay.Clear();
                return NewWay;
            }
            else if (Modifyfail == false)
            { return NewWay; }//此返回路径 貌似可以删除 
            else//原方案修改，但没有合适的修改方案则返回null
            { return null; }
        }

        /// <summary>
        /// 判断一条线段是否被阻塞
        /// </summary>
        /// <param name="LineFirstPoint">线段的起点</param>
        /// <param name="LineSecondPoint">线段的终点</param>
        /// <param name="ModifyTime">线段剩余可修改的次数</param>
        /// <returns>返回一个方案集合,如果没有修改则为空,如果返回null则说明抛弃此方案</returns>
        private List<List<Point>> LineIsBlock(Point LineFirstPoint, Point LineSecondPoint, int ModifyTime)
        {
            List<List<Point>> ModifyPoints = new List<List<Point>>();
            List<List<Point>> tempModifyLines = new List<List<Point>>();
            List<Point> tempModifyWay = new List<Point>();

            if (ModifyTime <= 0)//如果修改次数用尽仍然被阻，则抛弃当前方案
            { return null; }

            foreach (BlockInfo tempBlock in CurrentBlocks)
            {
                Point StartP = LineFirstPoint;
                Point EndP = LineSecondPoint;
                //当前控件的四角坐标
                Point LeftUp = tempBlock.StartPoint;
                Point RightUp = new Point(LeftUp.X + tempBlock.Width, LeftUp.Y);
                Point LeftDown = new Point(LeftUp.X, LeftUp.Y + tempBlock.Height);
                Point RightDown = new Point(RightUp.X, LeftDown.Y);
                List<Point> Crosses = WaysIsCross(new List<Point>(new Point[] { StartP, EndP }),tempBlock);//直线与轮廓线的交点集        

                if (Crosses.Count == 0)//不交
                { continue; }
                else if (Crosses.Count == 1)
                {
                    #region 一点在控件内
                    //平移优先于拐点
                    //一点在内情况不考虑平移距离 因为方向优先于距离
                    List<Point> Way1 = new List<Point>();
                    List<Point> Way2 = new List<Point>();
                    List<Point> Way3 = new List<Point>();
                    List<Point> Way4 = new List<Point>();
                    Point CrossP = Crosses[0];
                    bool isReverse = false;//是否是反向

                    if (EndP.X < LeftUp.X || EndP.X > RightUp.X)//第二点在外则调整成起始点在外
                    {
                        isReverse = true;
                        Point tempP = StartP;
                        StartP = EndP;
                        EndP = tempP;
                    }

                    if (StartP.Y == EndP.Y)//直线为水平
                    {
                        //平移方案
                        //上平移                        
                        Way1.Add(new Point(StartP.X, LeftUp.Y));
                        Way1.Add(new Point(EndP.X, LeftUp.Y));

                        //下平移                    
                        Way2.Add(new Point(StartP.X, LeftDown.Y));
                        Way2.Add(new Point(EndP.X, LeftDown.Y));

                        //拐点方案
                        //上拐
                        Way3.Add(StartP);
                        Way3.Add(CrossP);
                        Way3.Add(new Point(CrossP.X, LeftUp.Y));
                        Way3.Add(new Point(EndP.X, LeftUp.Y));

                        //下拐
                        Way4.Add(StartP);
                        Way4.Add(CrossP);
                        Way4.Add(new Point(CrossP.X, LeftDown.Y));
                        Way4.Add(new Point(EndP.X, LeftDown.Y));
                    }
                    else if (StartP.X == EndP.X)//直线为竖直
                    {
                        //平移方案
                        //左平移                   
                        Way1.Add(new Point(LeftUp.X, StartP.Y));
                        Way1.Add(new Point(LeftUp.X, EndP.Y));

                        //右平移                   
                        Way2.Add(new Point(RightUp.X, StartP.Y));
                        Way2.Add(new Point(RightUp.X, EndP.Y));

                        //拐点方案
                        //左拐
                        Way3.Add(StartP);
                        Way3.Add(CrossP);
                        Way3.Add(new Point(LeftUp.X, CrossP.Y));
                        Way3.Add(new Point(LeftUp.X, EndP.Y));

                        //右拐
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
                    #region 直线穿过控件
                    Point CrossP1 = Crosses[0];
                    Point CrossP2 = Crosses[1];
                    List<Point> Way1 = new List<Point>();//优先的路径
                    List<Point> Way2 = new List<Point>();//非优先的路径
                    List<Point> Way3 = new List<Point>();
                    List<Point> Way4 = new List<Point>();

                    if (StartP.Y > EndP.Y || EndP.X > StartP.X)//直线起点在下或则在左则交换两个交点坐标
                    {
                        Point tempP = CrossP1;
                        CrossP1 = CrossP2;
                        CrossP2 = tempP;
                    }

                    if (CrossP1.X == CrossP2.X)//上下交
                    {
                        //右平移
                        Way1.Add(StartP);
                        Way1.Add(new Point(RightUp.X, StartP.Y));
                        Way1.Add(new Point(RightUp.X, EndP.Y));

                        //右拐平移
                        Way2.Add(StartP);
                        Way2.Add(CrossP1);
                        Way2.Add(new Point(RightUp.X, CrossP1.Y));
                        Way2.Add(new Point(RightUp.X, EndP.Y));

                        //左平移
                        Way3.Add(StartP);
                        Way3.Add(new Point(LeftUp.X, StartP.Y));
                        Way3.Add(new Point(LeftUp.X, EndP.Y));

                        //左拐平移
                        Way4.Add(StartP);
                        Way4.Add(CrossP1);
                        Way4.Add(new Point(LeftUp.X, CrossP1.Y));
                        Way4.Add(new Point(LeftUp.X, EndP.Y));
                        //int D_left = StartP.X - LeftUp.X;
                        //int D_right = RightUp.X - StartP.X;
                        if (StartP.X * 2 - LeftUp.X - RightUp.X < 0)//左近则左平移优先
                        {
                            List<Point> tempWay = Way1;
                            Way1 = Way3;
                            Way3 = tempWay;
                            tempWay = Way2;
                            Way2 = Way4;
                            Way4 = tempWay;
                        }
                    }
                    else if (CrossP1.Y == CrossP2.Y)//左右交
                    {
                        //下平移
                        Way1.Add(StartP);
                        Way1.Add(new Point(StartP.X, RightDown.Y));
                        Way1.Add(new Point(EndP.X, RightDown.Y));

                        //下拐平移
                        Way2.Add(StartP);
                        Way2.Add(CrossP1);
                        Way2.Add(new Point(CrossP1.X, LeftDown.Y));
                        Way2.Add(new Point(EndP.X, LeftDown.Y));

                        //上平移
                        Way3.Add(StartP);
                        Way3.Add(new Point(StartP.X, LeftUp.Y));
                        Way3.Add(new Point(EndP.X, LeftUp.Y));

                        //上拐平移
                        Way4.Add(StartP);
                        Way4.Add(CrossP1);
                        Way4.Add(new Point(CrossP1.X, RightUp.Y));
                        Way4.Add(new Point(EndP.X, RightUp.Y));

                        //int D_up = StartP.Y - LeftUp.Y;
                        //int D_down = LeftDown.Y - StartP.Y;
                        if (StartP.Y * 2 - LeftUp.Y - LeftDown.Y < 0)//上近则上平移优先
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
        /// 将直线的点顺序反向
        /// </summary>
        /// <param name="LinePoints">顺序点路线</param>
        /// <returns>反向点路线</returns>
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
        /// 判断所给的方案的可用性
        /// </summary>
        /// <param name="ways">修改方案集合</param>
        /// <param name="ModifyTime">剩余修改次数</param>
        /// <returns>可用方案集</returns>
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
        /// 计算两条线段间的交点，如不相交则返回(0,0)
        /// </summary>
        /// <param name="FirstLineStartPoint">第一条线段起点坐标</param>
        /// <param name="FirstLineEndPoint">第一条线段终点坐标</param>
        /// <param name="SecondLineStartPoint">第二条线段起点坐标</param>
        /// <param name="SecondLineEndPoint">第二条线段终点坐标</param>
        /// <returns>返回线段交点坐标</returns>
        private Point LinesIscross(Point FirstLineStartPoint, Point FirstLineEndPoint, Point SecondLineStartPoint, Point SecondLineEndPoint)
        {
            Point H1 = new Point();//横线左点
            Point H2 = new Point();//横线右点
            Point S1 = new Point();//竖线上点
            Point S2 = new Point();//竖线下点
            Point Cross = new Point();//两线交点
            if (FirstLineStartPoint.X == FirstLineEndPoint.X && SecondLineStartPoint.Y == SecondLineEndPoint.Y)//线1为竖线，线2为横线
            {
                if (FirstLineStartPoint.Y < FirstLineEndPoint.Y)//线1起点在上，终点在下
                {
                    S1 = FirstLineStartPoint;
                    S2 = FirstLineEndPoint;
                }
                else//线1起点在下，终点在上
                {
                    S1 = FirstLineEndPoint;
                    S2 = FirstLineStartPoint;
                }
                if (SecondLineStartPoint.X < SecondLineEndPoint.X)//线2起点在左，终点在右
                {
                    H1 = SecondLineStartPoint;
                    H2 = SecondLineEndPoint;
                }
                else//线2起点在右，终点在左
                {
                    H1 = SecondLineEndPoint;
                    H2 = SecondLineStartPoint;
                }
            }
            else if (FirstLineStartPoint.Y == FirstLineEndPoint.Y && SecondLineStartPoint.X == SecondLineEndPoint.X)//线2为竖线，线1为横线
            {
                if (FirstLineStartPoint.X < FirstLineEndPoint.X)//线1起点在左，终点在右
                {
                    H1 = FirstLineStartPoint;
                    H2 = FirstLineEndPoint;
                }
                else//线1起点在右，终点在左
                {
                    H1 = FirstLineEndPoint;
                    H2 = FirstLineStartPoint;
                }
                if (SecondLineStartPoint.Y < SecondLineEndPoint.Y)//线2起点在上，终点在下
                {
                    S1 = SecondLineStartPoint;
                    S2 = SecondLineEndPoint;
                }
                else//线2起点在下，终点在上
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
        /// 计算一条线路方案与控件的交点
        /// </summary>
        /// <param name="curWay">路线方案</param>
        /// <param name="ctrlBlock">控件块信息</param>
        /// <returns>返回交点点集</returns>
        private List<Point> WaysIsCross(List<Point> curWay, BlockInfo ctrlBlock)
        {
            //当前控件的四角坐标
            Point LeftUp = ctrlBlock.StartPoint;
            Point RightUp = new Point(LeftUp.X + ctrlBlock.Width, LeftUp.Y);
            Point LeftDown = new Point(LeftUp.X, LeftUp.Y + ctrlBlock.Height);
            Point RightDown = new Point(RightUp.X, LeftDown.Y);
            List<Point> BlockOutline = new List<Point>(new Point[] { LeftUp, RightUp, RightDown, LeftDown, LeftUp });//控件的轮廓线
            List<Point> CrossPoints = new List<Point>();//交点集合

            for (int i = 1; i < curWay.Count; i++)
            {//线路中的每一线段与轮廓线的交点集
                CrossPoints.AddRange(WaysIsCross(new List<Point>(new Point[] { curWay[i - 1], curWay[i] }), BlockOutline));
            }
            return CrossPoints;
        }


        /// <summary>
        /// 计算两条线路方案之间的交点
        /// </summary>
        /// <param name="FirstWay">第一条线路</param>
        /// <param name="SecondWay">第二条路线</param>
        /// <returns>返回两条线路的交点点集</returns>
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
        /// 计算路径长途,返回长度值
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
