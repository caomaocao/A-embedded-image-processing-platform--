using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using  System.Diagnostics;

namespace CaVeGen.DesignViewFiles.CreateILXML
{

   
    public struct OrderBlock
    { //排序用结构体
        public Point LUpoint;//左上点坐标
        public List<ControlInfo> ContainCtrls;//包含的控件
        public List<string> InputPorts;//断层接口
    }


    class ConfigOrder
    {

        //存放控件集合所需要申明的临时变量名的集合
        public List<string> TempInfo = new List<string>();
        private CassView cassview = new CassView();

        public ConfigOrder(CassView curCas, string[] pageNameInfo)
        {
            this.cassview = curCas;
        }




        private List<ControlInfo> StartOrder(List<ControlInfo> GroupCtrl, string pageName)
        {
            List<ControlInfo> EndCtrls = new List<ControlInfo>();//尾端控件列表
            foreach (ControlInfo ElementCtrl in GroupCtrl)
            {
                if (CheckTipCtrl(ElementCtrl.OutputInfo))
                {//尾端控件//未处理非正常连接的尾端控件 两个NULL//暂时判断条件为NULL或所有NULL
                    EndCtrls.Add(ElementCtrl);
                }
                else if (ElementCtrl.OutputInfo.Count != 1 || ElementCtrl.OutputInfo[0][0].Split(',').Length != 1)
                {//多输出和单输出多输入控件
                    for (int OPindex = 0; OPindex < ElementCtrl.OutputInfo.Count; OPindex++)
                    {
                        if (ElementCtrl.OutputInfo[OPindex][0] != null)
                        {
                            string[] tempArray = ElementCtrl.OutputInfo[OPindex][0].Split(',');
                            //把原输出信息添加：控件名_出口序号 
                            string tempOutInfo = null;
                            if (ElementCtrl.CodeInfo[2] != null)
                            {
                                tempOutInfo = ElementCtrl.CodeInfo[2];
                            }
                            else
                            {
                                tempOutInfo = ElementCtrl.CodeInfo[1];
                            }
                            //创建临时变量 控件名_端口号
                            tempOutInfo =
                                //"configuration" + CassViewGenerator.ProjectNum + "_" + 
                                tempOutInfo + "_" + OPindex.ToString();
                            ElementCtrl.OutputInfo[OPindex][1] = tempOutInfo;

                            //找到该输出口对应的输入口,并修改其输入口信息
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
                            TempInfo.Add(tempOutInfo);//将所用的临时变量名加入对应队列
                        }
                    }
                }
            }
            if (GroupCtrl.Count != 0 && EndCtrls.Count == 0)
            {
                //CassViewGenerator.SpecialErrors.Add(new string[] { null, "出现无输出回路", "error", pageName });
                //ViewErrorinfo.Add(new string[] { pageName, null, "出现无输出回路" });
            }
            return EndCtrls;
        }






        /// <summary>
        /// 根据所给控件信息集合进行由输入到输出的排序
        /// </summary>
        /// <param name="GroupCtrl">未排序的一个控件组</param>
        /// <returns>返回一个已经排序的控件集合</returns>
        private ControlInfo[] OrderCtrlsNum(List<ControlInfo> GroupCtrl, string pageName)
        {
            Stack<ControlInfo> TempStack = new Stack<ControlInfo>();//排序中所用到的临时堆栈  
            Stack<ControlInfo> OrderStack = new Stack<ControlInfo>();//已进行排序的堆栈            
            List<string> tempCode = new List<string>();//自底向上遍历时按顺序存放临时变量的队列
            List<OrderBlock> TempOB = new List<OrderBlock>();//存放排序快的列表

            #region 控件排序规则
            //用户在连线和指令中只需关心用到的输出输入控件目标
            //当输入或输入接口存在，但无连接对象则程序中默认有输入值输入目标，并不显示在指令和组态操作中
            //在提供给下位机时会完全显示数据流向和数据目标
            //当多输出控件时则必须先将输出值的目标设定为临时变量，（当然如果顺序不错，可取消该操作（待定））
            //多输出控件输出翻译的顺序为逆向20090608
            //当单输出对应多输入时，同样采用临时变量
            //同一控件串中多输出分支后没有再汇聚的情况下依据控件的坐标先上后下的进行翻译20090805
            #endregion

            #region 排序方法
            //如果用户能改动指令则对于空值会出现匹配问题     
            //将输入输出信息中多输出和单输出多输入两情况信息进行修正
            List<ControlInfo> EndCtrls = StartOrder(GroupCtrl, pageName);
            #endregion

            #region 自底向上控件排序算法
            //将一个尾端控件压入排序堆栈,当出现断层且未满足时(即碰到临时变量多输出情况)
            //计算其控件集合中的左上坐标后再压入第二个尾端控件依次计算
            //当断层条件由未满足到满足时先对当前与断层相关的已排序的控件集合依据坐标进行排序
            for (int s = 0; s < EndCtrls.Count; s++)
            {
                TempStack.Push(EndCtrls[s]);
                //OrderBlock newBlock = initializationOB();
                while (TempStack.Count != 0)
                {
                    ControlInfo TopControl = TempStack.Pop();
                    GroupCtrl.Remove(TopControl);//从未排序控件组中删除对应的控件信息
                    //必须删除，用来减少后面计算量和防止多输出控件的重复使用
                    //将栈顶弹出的控件信息压入已排序堆栈
                    OrderStack.Push(TopControl);

                    if (TopControl.ControlName != null && TopControl.InputInfo != null)
                    {//虚拟控件或头控件(无输入信息)直接压入排序堆栈
                        for (int x = 0; x < TopControl.InputInfo.Count; x++)//如果要顺序，则需要逆向遍历
                        {
                            string[] Input = TopControl.InputInfo[x];
                            if (TempInfo.Contains(Input[1]))
                            {//控件输入需要的临时变量
                                tempCode.Add(Input[1]);
                                ControlInfo temp = new ControlInfo();
                                temp.ControlNum = -1;//虚拟控件序号统一为-1
                                temp.CodeInfo = new string[1];
                                temp.CodeInfo[0] = Input[1];
                                TempStack.Push(temp);//虚拟控件1
                            }
                            else if (Input[0] != null)
                            {//将前级输出控件压入堆栈
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
                            {//用户不用的输入口//需要改成LD默认
                                ControlInfo temp = new ControlInfo();
                                if (Input[3] != null && Input[3] != "")
                                {
                                    temp.ControlNum = -1;//虚拟控件序号统一为-1
                                    temp.CodeInfo = new string[2] { Input[2], Input[3] };
                                    TempStack.Push(temp);//虚拟控件2
                                }
                            }
                        }
                    }
                }
                TempOB.Add(CreateOB(OrderStack));
                OrderStack.Clear();
                //if (TempStack.Count == 0 && GroupCtrl.Count != 0)
                //{//还有控件没有排序,但是因为临时变量的原因使堆栈为空
                //    for (int i = 0; i < GroupCtrl.Count; i++)
                //    {//在未排序控件组中找到已经可以被调用的控件压入临时堆栈
                //        if (TempInfo.Contains(GroupCtrl[i].OutputInfo[0][1]) || GroupCtrl[i].OutputInfo.Count != 1)
                //        {
                //            for (int j = 0; j < GroupCtrl[i].OutputInfo.Count; j++)
                //            {
                //                if (GroupCtrl[i].OutputInfo[j][0] != null
                //                    && !tempCode.Contains(GroupCtrl[i].OutputInfo[j][1]))
                //                {//有输出对象且使用过的临时变量中不存在则跳出
                //                    break;
                //                }
                //                if (j == GroupCtrl[i].OutputInfo.Count - 1)
                //                {//遍历完所有输出口且都已经使用过
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
        /// 根据控件输入或输出信息判断是否为终端控件（起点或终点）
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
        /// 初始化排序结构体
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
            //newOB.LUpoint = cassview.FindBlockInfo(orderS[0]).StartPoint;//第一控件的起点坐标
            return newOB;
        }




    }
}
