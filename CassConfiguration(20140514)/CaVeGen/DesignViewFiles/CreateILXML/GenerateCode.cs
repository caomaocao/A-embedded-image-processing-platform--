/*******************************************************************************
           ** Copyright (C) 2009 CASS 版权所有
           ** 文件名：CassView.cs 
           ** 功能描述：
           **          根据控件的连线关系生成指令表
           ** 作者：宋骁健
           ** 创始时间：2009-5-15
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
        private List<string> Control_C = new List<string>();//用于生成control.c文件
        private List<string> CtrlName_C = new List<string>();//存放已经用过Control.c的控件种类        

        private List<ArrayList> tabList = new List<ArrayList>();
        private List<string> UseTactic = new List<string>();//所使用到策略的集合

        private List<string[]> Rpageinfo = new List<string[]>();//页面信息对应表 父节点名，子节点个数，父节点页面翻译名

        //部分特殊控件由控件名到指令名的对应队列2013
        public static List<string> SortCtrlName
            = new List<string>(new string[] { "返回", "输入变量", "输出变量", "常数", "系统变量", "调用", "跳转", "弹出指令","文件输入","文件输出","特征输入"/*,"拉普拉斯","设备输入"*/});
        public static List<string> CodeCtrlName
            = new List<string>(new string[] { "RET", "LD", "ST", "CON", "SYSVAR", "CALL", "JMP", "POP", "FILEIN", "FILEOUT", "FEATHERIN"/*,"LAPALCE" ,"CAMERAIN"*/ });
        //解耦控件中的不显示数组
        public static List<string> JOUnuseArray 
            = new List<string>(new string[] { "上次偏差数组", "上上次偏差数组", "上次控制器输出" });

        public static List<string[]> PackInfos = new List<string[]> ();//每串控件信息集合

        //需要上位机对部分不可见功能属性进行计算赋值的控件名
        public static List<string> SpicalCtrlName
            = new List<string>(new string[] { "斜坡控制器", "通用线性化器", "三者取中器", "高低选择器" });

        public List<string[]> ViewErrorinfo = new List<string[]>();//未连接的控件信息

        static public List<string[]> UnseeArray = new List<string[]>();//存放不显示给用户的数组信息//暂时用于解耦的数组20090617
        private List<string[]> ArrayInfo = new List<string[]>();//存放数组信息

 
        #region 页面命名规则

        //图形界面可以对页面名进行任意修改
        //但是在生成指令表及编译时会根据各个页面的级别进行聪明命名
        //不算主节点的各级子节点为"_"开始，且个数代表级数
        //"_"后先接A~Z字母由1-2位组成表示该节点的父节点的子节点的序号
        //字母后由1-2位0-9数字组成标示该页面的控件块序号,且第一块无该序号

        #endregion

        //存放控件集合所需要申明的临时变量名的集合
        public List<string> TempInfo = new List<string>();

        static public List< List<string>> CtrlsList = new List<List<string>>(); 

    //    static public List<string> ctrlsNum = new List<string> ();  //2014.1.15
        

        //#region 委托 传递给CassViewGenerator的已排好序的控件序号数组  2014.1.10 
       
        ////声明委托
        //public delegate void SendMeg(List<string> myList);
        ////定义委托 
        //public SendMeg myDeleSendCtrlsNum;

        //#endregion

        public GenerateCode(List<ArrayList> CassInfoList)
        {
            if (CassInfoList != null)
            { this.tabList.AddRange(CassInfoList); }
            //初始化控件串名数组
            PackInfos = new List<string[]>();
        }

        public List<string[]> GenerateCodeList()
        {
            List<string[]> Codelist = new List<string[]>();
            //初始化调用页面策略，并附上主页面名
            //this.UseTactic = new List<string>(new string[] { "main" });20090624全显示不需要掠过
            int Count = 0;//数组序号
    
            foreach (ArrayList node in this.tabList)
            {
                ArrayInfo.AddRange(CatchArrayInfo((CassView)node[1], ref Count));
                //if (UseTactic.Contains(((string[])(node[0]))[0]))
                //{//没有调用则略过该页     
                
                Codelist.AddRange(PackOrderCtrl((CassView)node[1], (string[])node[0]));
                Codelist.Add(null);
                //}
            }
            return Codelist;
        }

        /// <summary>
        /// 获取当前页的显示标号
        /// </summary>
        /// <param name="markArray">当前页面的路径</param>
        /// <returns>返回转换成在指令表中显示的标示符名</returns>
        private string getMark(string[] markArray)
        {
            if (markArray[1] == null || !markArray[1].Contains("\\"))
            {//无父节点的作为Main
                return "main";
            }
            else
            {
                int startIndex = 0;//页面序号
                bool findinfo = false;//是否存在与当前的页面对应表
                string addName = null;
                string[] levelinfo = markArray[1].Split('\\');

                foreach (string[] element in this.Rpageinfo)
                {//遍历页面关联信息表
                    if (element[0] == levelinfo[levelinfo.Length - 2])
                    {//存在父节点的信息
                        startIndex = Convert.ToInt32(element[1]);//父节点现有子节点个数
                        element[1] = (startIndex + 1).ToString();
                        addName = element[2];//父节点标示符名
                        findinfo = true;
                    }
                }
                if (!findinfo)
                {//不存在父节点信息
                    this.Rpageinfo.Add(new string[] { levelinfo[levelinfo.Length - 2], (startIndex + 1).ToString(), null });
                }
                //由序号转换成字母型
                if (startIndex < 26)
                {
                    addName += "_" + Convert.ToChar('A' + startIndex).ToString();
                }
                else
                {
                    addName += "_" + Convert.ToChar('A' + startIndex / 26 - 1).ToString() + Convert.ToChar('A' + startIndex % 26).ToString();
                }
                //将当前节点的关联信息加入关联信息表
                this.Rpageinfo.Add(new string[] { levelinfo[levelinfo.Length - 1], "0", addName });
                return addName;
            }
        }

        /// <summary>
        /// 将参数中尾部的数字块号去掉
        /// 获取其所在的页面名
        /// </summary>
        /// <param name="markName">标示符名</param>
        /// <returns>页面名</returns>
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
        /// 将参数中尾部的数字块号以外去掉
        /// 获取其所在的页面的快号
        /// </summary>
        /// <param name="markName">标示符名</param>
        /// <returns>块号</returns>
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
        /// 对Cassview中的控件进行打包和排序
        /// </summary>
        /// <param name="curCas">需要排序的cassview</param>
        /// <param name="pageName">cassview页面的名称</param>
        /// <returns></returns>
        public List<string[]> PackOrderCtrl(CassView curCas, string[] pageNameInfo)
        {
            List<ControlInfo[]> CassInfo = new List<ControlInfo[]>();
            List<List<ControlInfo>> alreadyPack = PackCtrls(curCas, pageNameInfo[0]);

            string pageName = getMark(pageNameInfo);
            foreach (List<ControlInfo> pack in alreadyPack)
            {   //对控件进行排序
                CassInfo.Add(OrderCtrlsNum(curCas,pack,pageNameInfo[0]));
            }
            return OrderPacksNum(curCas, CassInfo, pageName);
        }

        /// <summary>
        /// 以页面cassview为单位进行控件串间的排序
        /// 并产生控件串名与控件串序号整合列表（控件JMP需要信息）
        /// 以及生成指令表列表和控件串名进行匹配后返回
        /// </summary>
        /// <param name="alreadyOrderInfo">待排序的控件串集合</param>
        /// <param name="pageName">当前CASSVIEW的页面名</param>
        /// <returns></returns>
        private List<string[]> OrderPacksNum(CassView curCas, List<ControlInfo[]> alreadyOrderInfo, string pageName)
        {
            List<Point> OrderPoint = new List<Point>();//存放每串控件中坐标最小值,作为控件串间排序依据
            List<ControlInfo[]> OrderPack = new List<ControlInfo[]>();//控件串间排序后存放列表
            List<string> Indexs = new List<string>();//每串控件的控件序号序列
            List<string> PackName = new List<string>();//控件串名的集合

            GenerateCode.CtrlsList.Clear();

            for (int Pindex = 0; Pindex < alreadyOrderInfo.Count; Pindex++)
            {
                List<string> CnumList = new List<string>();//每串控件序号集合
                Point minP = new Point(1200, 1000);//每串控件最小坐标
                //第一部分确定控件串名
                //所有页面没有0块命名
                if (Pindex == 0)
                {
                    PackName.Add(pageName);
                }
                else
                {
                    PackName.Add(pageName + Pindex.ToString());//所在页面名+数字序号作为块名
                }
                //第二部分找到每串控件中坐标最小值并由此进行控件串间排序
                for (int Cindex = 0; Cindex < alreadyOrderInfo[Pindex].Length; Cindex++)
                {
                    if (alreadyOrderInfo[Pindex][Cindex].ControlNum != -1)
                    {
                        CnumList.Add(alreadyOrderInfo[Pindex][Cindex].ControlNum.ToString());
                        Point CurP = curCas.FindBlockInfo(alreadyOrderInfo[Pindex][Cindex]).StartPoint;
                        if (CurP != new Point() && (CurP.Y < minP.Y || (CurP.Y == minP.Y && CurP.X < minP.X)))
                        {//取Y值小的点，当Y相同则取X相同的点
                            minP = CurP;
                        }
                    }
                }
                //2014.1.15
             //   GenerateCode.ctrlsNum = CnumList;
                GenerateCode.CtrlsList.Add(CnumList);
                ////调用委托
                //if (CnumList != null)
                //{
                //    this.myDeleSendCtrlsNum(CnumList);
                //}
                if (OrderPoint.Count == 0 
                    || minP.Y > OrderPoint[OrderPoint.Count - 1].Y 
                    || (minP.Y == OrderPoint[OrderPoint.Count - 1].Y && minP.X >= OrderPoint[OrderPoint.Count - 1].X))
                {//点排序集合为空 或 Y值大于集合尾元素Y 或 Y相同X值大于等于集合尾元素X
                    OrderPoint.Add(minP);
                    OrderPack.Add(alreadyOrderInfo[Pindex]);
                    Indexs.Add(String.Join(",", CnumList.ToArray()));
                }
                else
                {
                    for (int Gindex = 0; Gindex < OrderPoint.Count; Gindex++)
                    {
                        if (minP.Y < OrderPoint[Gindex].Y || (minP.Y == OrderPoint[Gindex].Y && minP.X < OrderPoint[Gindex].X))
                        {//插入条件 Y值小于当前点Y 或 Y值相同X小于当前点X
                            OrderPoint.Insert(Gindex, minP);
                            OrderPack.Insert(Gindex, alreadyOrderInfo[Pindex]);
                            Indexs.Insert(Gindex, String.Join(",", CnumList.ToArray()));
                            break;
                        }
                    }
                }
            }
            //第三部分将产生的控件串序号依次与串名进行整合
            if (Indexs.Count == PackName.Count)
            {
                for (int i = 0; i < Indexs.Count; i++)
                {
                    PackInfos.Add(new string[] { PackName[i], Indexs[i] });
                }
            }
            //第四部分将控件串名与排序完后生成的指令数组进行整合并返回
            return InsertPackName(CreateCodeList(OrderPack), PackName);
        }


        /// <summary>
        /// 对应生成的指令表和控件串命名进行匹配
        /// </summary>
        /// <param name="CodeInfo"></param>
        private List<string[]> InsertPackName(List<string[]> CodeInfo, List<string> PackInfo)
        {
            for (int i = 0; i < PackInfo.Count; i++)
            {
                if (i == 0)
                {//在第一列插入第一个控件串名
                    CodeInfo.Insert(0, new string[] { PackInfo[0] + ":", null });
                }
                else
                {//其他则用控件串名替换控件串间的NULL
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
        /// 遍历curtcass下所有控件信息根据连接情况将其分块
        /// </summary>
        /// <param name="curtcass">需要处理的Cassview</param>
        /// <param name="pageName">当前Cassview的页名</param>
        /// <returns>已分块好的控件信息组</returns>
        private List<List<ControlInfo>> PackCtrls(CassView curtcass, string pageName)
        {
            List<List<ControlInfo>> GroupCtrls = new List<List<ControlInfo>>();
            List<string> UseCtrl = new List<string>();//保存已经使用过的控件名称
            ////////////////////////////////////////////////////////////////////////////////需优化
            foreach (ControlInfo ctrl in curtcass.ctrlsInfo)
            {
                if (UseCtrl.Contains(ctrl.ControlName))
                { continue; }
                else if (ctrl.IsConnect)
                {//该控件是连接控件且没有被其他控件组容纳
                    Stack<ControlInfo> tempStack = new Stack<ControlInfo>();//用于存放当前控件信息的相连接的控件信息的中转用堆栈
                    List<ControlInfo> Pack = new List<ControlInfo>();

                    //当前控件压入临时堆栈
                    tempStack.Push(ctrl);
                    UseCtrl.Add(ctrl.ControlName);

                    while (tempStack.Count != 0)
                    {
                        //取出栈顶控件
                        ControlInfo TopCtrl = tempStack.Pop();

                        //并将该控件所连接的且非遍历过的控件压入堆栈
                        if (TopCtrl.OutputInfo != null)
                        {
                            if (TopCtrl.OutputInfo.Count == 1 && TopCtrl.OutputInfo[0][0] != null && TopCtrl.OutputInfo[0][0].Split(',').Length == 1)
                            {//单输出对应单输入情况
                                ControlInfo ConnectCtrl = curtcass.ctrlsInfo[curtcass.FindControlName(TopCtrl.OutputInfo[0][0].Split('.')[0])];
                                if (!UseCtrl.Contains(ConnectCtrl.ControlName))
                                {
                                    tempStack.Push(ConnectCtrl);
                                    UseCtrl.Add(ConnectCtrl.ControlName);
                                }
                            }
                            else
                            {//单输出对应多输入或则多输出情况       
                                for (int i = TopCtrl.OutputInfo.Count-1; i >=0; i--)
                                {
                                    if (TopCtrl.OutputInfo[i][0] != null)
                                    {
                                        string[] OputArray = TopCtrl.OutputInfo[i][0].Split(',');//一输出口对应多个控件以逗号分隔
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
                        //将栈顶控件转入新的控件组
                        Pack.Add(TopCtrl);
                    }
                    GroupCtrls.Add(Pack);
                }
                if (!ctrl.IsConnect)
                {
                    string warningInfo = "控件序号为" + ctrl.ControlNum.ToString() + "的" + ctrl.CodeInfo[0] + "控件没有连接";
                    CassViewGenerator.SpecialErrors.Add(new string[] { null, warningInfo, "warning", pageName });
                    ViewErrorinfo.Add(new string[] { pageName, ctrl.ControlNum.ToString(), warningInfo });
                }
                if (ctrl.ControlName.ToUpper().Contains("FILEIN"))
                {
                    if (ctrl.CodeInfo[1].Trim() == "FILEIN"||ctrl.CodeInfo[1].Trim() == "" || ctrl.CodeInfo[1] == null)
                    {
                        string errorInfo = "控件序号为" + ctrl.ControlNum.ToString() + "的'" + ctrl.CodeInfo[0] + " '控件缺少输入参数";
                        CassViewGenerator.SpecialErrors.Add(new string[] { null, errorInfo, "error", pageName });
                        ViewErrorinfo.Add(new string[]{pageName,ctrl.ControlNum.ToString(),errorInfo});


                    }
                }
            }
            return GroupCtrls;
        }

        /// <summary>
        /// 根据所给控件信息集合进行由输入到输出的排序
        /// </summary>
        /// <param name="GroupCtrl">未排序的一个控件组</param>
        /// <returns>返回一个已经排序的控件集合</returns>
        private ControlInfo[] OrderCtrlsNum(CassView curCas,List<ControlInfo> GroupCtrl, string pageName)
        {
            Stack<ControlInfo> TempStack = new Stack<ControlInfo>();//排序中所用到的临时堆栈            
            Stack<ControlInfo> OrderStack = new Stack<ControlInfo>();//已进行排序的堆栈            
            List<string> tempCode = new List<string>();//自底向上遍历时按顺序存放临时变量的队列

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
            foreach (ControlInfo ElementCtrl in GroupCtrl)
            {
                if (CheckTipCtrl(ElementCtrl.OutputInfo))
                {//尾端控件//未处理非正常连接的尾端控件 两个NULL//暂时判断条件为NULL或所有NULL
                    TempStack.Push(ElementCtrl);
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
            if (GroupCtrl.Count != 0 && TempStack.Count == 0)
            {
                CassViewGenerator.SpecialErrors.Add(new string[] { null, "出现无输出回路", "error", pageName });
                ViewErrorinfo.Add(new string[] { pageName, null, "出现无输出回路" });
            }
            #region 自底向上控件排序算法
            if (TempStack.Count != 1)
            {//有多个尾端控件时对尾端控件进行排序
                TempStack = OrderEndCtrls(curCas,TempStack,GroupCtrl);
            }
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
                if (TempStack.Count == 0 && GroupCtrl.Count != 0)
                {//还有控件没有排序,但是因为临时变量的原因使堆栈为空
                    for (int i = 0; i < GroupCtrl.Count; i++)
                    {//在未排序控件组中找到已经可以被调用的控件压入临时堆栈
                        if (TempInfo.Contains(GroupCtrl[i].OutputInfo[0][1]) || GroupCtrl[i].OutputInfo.Count != 1)
                        {
                            for (int j = 0; j < GroupCtrl[i].OutputInfo.Count; j++)
                            {
                                if (GroupCtrl[i].OutputInfo[j][0] != null
                                    && !tempCode.Contains(GroupCtrl[i].OutputInfo[j][1]))
                                {//有输出对象且使用过的临时变量中不存在则跳出
                                    break;
                                }
                                if (j == GroupCtrl[i].OutputInfo.Count - 1)
                                {//遍历完所有输出口且都已经使用过
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
        /// 逆向遍历找到尾端控件所关联的断层处（多输出）的连接口控件坐标
        /// 以此为依据进行排序
        /// </summary>
        /// <param name="endCtrls"></param>
        /// <returns></returns>
        private Stack<ControlInfo> OrderEndCtrls(CassView curCas, Stack<ControlInfo> endCtrls, List<ControlInfo> GroupCtrl)
        {
            List<ControlInfo> orderList = new List<ControlInfo>();//控件排序用队列
            List<Point> orderP = new List<Point>();//点坐标排序队列
            Stack<ControlInfo> returnStack = new Stack<ControlInfo>();//返回用堆栈
            foreach (ControlInfo ctrl in endCtrls)
            {
                Stack<ControlInfo> Findctrls = new Stack<ControlInfo>();
                Findctrls.Push(ctrl);
                bool findPoint = false;
                while (!findPoint)
                {
                    ControlInfo Topctrl = Findctrls.Pop();
                    for (int x = Topctrl.InputInfo.Count - 1; x >= 0; x--)
                    {//逆向遍历上端口优先于下端口
                        string[] Input = Topctrl.InputInfo[x];
                        if (TempInfo.Contains(Input[1]))
                        {//出现临时变量找到断层跳出循环
                            findPoint = true;
                            //依据点的坐标以Y大的优先，Y相同X大的优先排序
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
                        {//将前级输出控件压入堆栈
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
            {//将排序末端控件压入返回堆栈 栈顶控件为Y最小X最小控件
                returnStack.Push(element);
            }
            return returnStack;
        }
 
        /// <summary>
        /// 根据每组控件信息转换成指令列表
        /// </summary>
        /// <param name="PackCtrls">已排序的控件集合</param>
        /// <returns>对应的指令列表</returns>
        private List<string[]> CreateCodeList(List<ControlInfo[]> PackCtrls)
        {
            List<string[]> BlockCode = new List<string[]>();

            foreach (ControlInfo[] groupControls in PackCtrls)
            {
                foreach (ControlInfo CtrlInfo in groupControls)
                {
                    if (CtrlInfo.ControlName != null)
                    {
                        if (SpicalCtrlName.Contains(CtrlInfo.CodeInfo[0]))//特殊控件隐藏属性赋值
                        {
                            SetUnvisibleFunction(CtrlInfo);//C#函数能否改动参数内值
                        }
                        if (CtrlInfo.CodeInfo != null && SortCtrlName.Contains(CtrlInfo.CodeInfo[0]))//特殊类型指令
                        {
                            int tempIndex = SortCtrlName.IndexOf(CtrlInfo.CodeInfo[0]);
                            if (tempIndex == 0)
                            {//返回 无参数
                                BlockCode.Add(new string[] { CodeCtrlName[tempIndex], null });
                            }
                            else if (tempIndex < 3)//输入变量输出变量为双参数
                            {
                                if (CtrlInfo.CodeInfo[1] != null)
                                {//要将其中的中文属性转换成指令表中的英文属性
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
                            else//其余为单参数
                            {
                                BlockCode.Add(new string[] { CodeCtrlName[tempIndex], CtrlInfo.CodeInfo[1] });
                                //if (CodeCtrlName[tempIndex] == "CALL")20090624全显示不需要掠过
                                //{//根据调用指令参数判断所需要使用的策略页面
                                //    this.UseTactic.Add(CtrlInfo.CodeInfo[1]);
                                //}
                            }
                        }
                        else if (CtrlInfo.VisibleFunctionProperty == null || CtrlInfo.VisibleFunctionProperty.Count == 0)//数学运算,不带参数
                        {//第三位存放控件序号用于生成XML时识别非算术控件20090609
                            BlockCode.Add(new string[] { CtrlInfo.CodeInfo[1], null,  CtrlInfo.ControlNum.ToString() });
                        }
                        else
                        {//把原先第二位 即结构体部分替换成参数20090701
                            if (CtrlInfo.CodeInfo[2] == null)//逻辑功能类型,有功能属性
                            {//指令表中和数学运算相同20090609
                                //BlockCode.Add(new string[] { CtrlInfo.ControlName, CtrlInfo.CodeInfo[1] });
                                BlockCode.Add(new string[] { CtrlInfo.CodeInfo[1], CatchPare(CtrlInfo), CtrlInfo.ControlNum.ToString() });
                            }
                            else//算法支撑和控制算法类型
                            {//由两位变成三位 须修正部分代码20090701
                                BlockCode.Add(new string[] { CtrlInfo.CodeInfo[1], CatchPare(CtrlInfo), CtrlInfo.CodeInfo[2] });
                            }
                        }
                    }
                    else if (CtrlInfo.CodeInfo != null)//临时变量输入指令
                    {
                        if (CtrlInfo.CodeInfo.Length == 1)
                        {//利用虚拟控件1                            
                            BlockCode.Add(new string[] { "LD", CtrlInfo.CodeInfo[0] });
                        }
                        else if (CtrlInfo.CodeInfo.Length == 2)
                        {//空输入默认指令//利用虚拟控件2
                            BlockCode.Add(new string[] { "CON", CtrlInfo.CodeInfo[1] });
                        }
                    }
                    //临时变量输出指令
                    if (CtrlInfo.OutputInfo != null && CtrlInfo.OutputInfo.Count != 0 &&
                        (TempInfo.Contains(CtrlInfo.OutputInfo[0][1]) || CtrlInfo.OutputInfo.Count != 1))
                    {
                        //foreach (string[] info in CtrlInfo.OutputInfo)
                        //{
                        for (int x = CtrlInfo.OutputInfo.Count - 1; x >= 0; x--)
                        {//多输出的输出顺序为反向20090608
                            string[] info = CtrlInfo.OutputInfo[x];
                            if (info[1] != null)
                            {
                                BlockCode.Add(new string[] { "ST", info[1] });
                            }
                            else
                            {//多输出且无目标控件时                                
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
        /// 从控件信息中获取显示于指令表的参数信息
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        private string CatchPare(ControlInfo ctrl)
        {
            List<string> tempValueList = new List<string>();//临时属性值列表
            foreach (XProp FuncValue in ctrl.VisibleFunctionProperty)
            {
                if (FuncValue.ValueType == "System.Boolean")
                {
                    tempValueList.Add((FuncValue.TheValue.ToString().ToUpper() == "FALSE" ? 0 : 1).ToString());
                }
                else if (FuncValue.EnumValue != ""&&FuncValue.ValueType!="COM") //串口 2014.1
                {
                    tempValueList.Add(new List<string>(FuncValue.EnumValue.Split(',')).IndexOf(FuncValue.TheValue.ToString()).ToString());
                }
                else
                {
                    tempValueList.Add(FuncValue.TheValue.ToString());
                }
            }
            foreach (XProp FuncValue in ctrl.UnvisibleFunctionProperty)
            {//从不可见属性中找数组参数
                if (FuncValue.TheValue.ToString().Contains("array"))
                {//是数组参数
                    foreach (string[] element in this.ArrayInfo)
                    {
                        if (element[3] == FuncValue.TheValue.ToString())
                        {//3位为数组名 5位为数组数据 作为参数加上大括号
                            tempValueList.Add("{" + element[5] + "}");
                        }
                    }
                }
            }
            return String.Join(",", tempValueList.ToArray());
        }

        /// <summary>
        /// 通过控件的PortName和中文属性寻找英文属性
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
        /// 对于特殊控件的个别用户不可见属性赋值
        /// </summary>
        /// <param name="SpeicalCtrl">控件信息</param>
       static public void SetUnvisibleFunction(ControlInfo SpeicalCtrl)
        {
            if (SpeicalCtrl.CodeInfo[0] == "斜坡控制器")
            {
                int TrueStep = 0;//有效段数              
                int ControlTime = Convert.ToInt32(XProp.GetValue("controlTime", SpeicalCtrl.VisibleFunctionProperty));//控制时间    
                int OutNum = Convert.ToInt32(XProp.GetValue("outNum", SpeicalCtrl.VisibleFunctionProperty));//有效段数    
                int PreValue = 0;//前一时间值
                int NowValue = 0;//当前时间值
                foreach (XProp function in SpeicalCtrl.VisibleFunctionProperty)
                {
                    if (function.VarName.Contains("time"))
                    {
                        if (TrueStep != 0)
                        { PreValue = NowValue; }
                        NowValue = Convert.ToInt32(function.TheValue);
                        if (NowValue < 0 || NowValue < PreValue || NowValue > ControlTime || TrueStep >= OutNum)
                        {//时间值小于0或时间值大于控制时间或当前点时间小于前一点时间 ？？点的序号大于控制段数
                            break;
                        }
                        TrueStep++;
                    }
                }
                XProp.SetValue(TrueStep.ToString(), "sectionNum", SpeicalCtrl.UnvisibleFunctionProperty);                 
            }
            else if (SpeicalCtrl.CodeInfo[0] == "通用线性化器")
            {
                int TruePoint = 0;//有效点个数
                float MinValue = Convert.ToSingle(XProp.GetValue("fInMin", SpeicalCtrl.VisibleFunctionProperty));
                float MaxValue = Convert.ToSingle(XProp.GetValue("fInMax", SpeicalCtrl.VisibleFunctionProperty));
                float PreValue = 0;//前一输入值
                float NowValue = 0;//当前输入值

                foreach (XProp function in SpeicalCtrl.VisibleFunctionProperty)
                {//"第X点输入"取后两字判断
                    if (function.Name.Length >= 5 && function.Name.Substring(3, 2) == "输入")//为点的输入属性
                    {
                        if (TruePoint != 0)
                        { PreValue = NowValue; }
                        NowValue = Convert.ToSingle(function.TheValue);
                        if (NowValue <= MinValue || NowValue <= PreValue || NowValue >= MaxValue)
                        {//输入值小于或等于输入最小值或小于或等于前一点的输入值或大于或等于输入最大值
                            break;
                        }
                        TruePoint++;
                    }
                }
                XProp.SetValue(TruePoint.ToString(), "effNum", SpeicalCtrl.UnvisibleFunctionProperty);           
            }
            else //"三者取中器", "高低选择器"
            {
                int index = 0;
                foreach (string[] Info in SpeicalCtrl.InputInfo)
                {//遍历输入口信息,非NULL表示有效
                    if (Info[0] != null)
                    {
                        index++; 
                    }
                }
                XProp.SetValue(index.ToString(), "ucEffNum", SpeicalCtrl.UnvisibleFunctionProperty);               
            }
        }

        /// <summary>
        /// 更正模糊控件信息中数组属性的数组名并返回数组信息
        /// </summary>
        /// <param name="curCas">控件所在CASSVIEW容器</param>
        /// <param name="curCtrl">控件</param>
        /// <param name="ArrayCount">当前所用到队列编号</param>
        /// <returns>数组信息</returns>
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
                    //tempArray += "\n";//去除每行回车符号 否则导致读取地址表出错BUG20090622
                }
            }
            //创建模糊控制表参数的数组节点
            for (int i = 0; i < Fuzzy.UnvisibleFunctionProperty.Count; i++)
            {
                if (Fuzzy.UnvisibleFunctionProperty[i].Name == "模糊控制表")
                {//ArrayInfo顺序依次为控件PortName、属性VarName、类型、数组名、长度、数组
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
        /// 更正解耦控件信息中数组属性的数组名并返回数组信息
        /// </summary>
        /// <param name="curCas">控件所在CASSVIEW容器</param>
        /// <param name="curCtrl">控件</param>
        /// <param name="ArrayCount">当前所用到队列编号</param>
        /// <returns>数组信息</returns>
        static public List<string[]> GetJieOuArray(CassView curCas, Control curCtrl,ref int ArrayCount)
        {
            List<string[]> ArrayInfos = new List<string[]>();//返回信息数组
            int count = 2;//解耦路数
            //解耦属性中需要生成数组的属性
            List<string> NeedAddArray
                = new List<string>(new string[] { "比例", "积分", "微分", "设定值", "自动/手动", "控制量上限", "控制量下限", "控制量输出", "解耦矩阵的参数" });
           
            PropertyDescriptor configurationProperty = TypeDescriptor.GetProperties(curCtrl)["Configuration"];
            ControlTactic.SpecialControl.JieOu ConvertTool = new ControlTactic.SpecialControl.JieOu();
            ControlTactic.SpecialControl.JieOuStruct tempStruct = ConvertTool.ListToStruct((List<string>)configurationProperty.GetValue(curCtrl));

            ControlInfo JieOu = curCas.FindControlInfo(curCtrl);
            for (int i = 0; i < JieOu.VisibleFunctionProperty.Count; i++)
            {//在可视属性中找到解耦数进行赋值
                if (JieOu.VisibleFunctionProperty[i].Name == "解耦路数")
                {
                    JieOu.VisibleFunctionProperty[i].TheValue = tempStruct.JieOuNum;
                    count = tempStruct.JieOuNum;
                    break;
                }
            }
            //依次为比例、积分、微分、设定值、自动手动、控制量上限、控制量下限、控制量输出数组
            List<string>[] Attributes
                = new List<string>[] { new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() };
            foreach (List<string> attribute in tempStruct.JieOuAttribute)
            {//每个属性 依次由比例、积分、微分、设定值、自动手动、控制量上限、控制量下限、控制量输出组成
                Attributes[0].Add(attribute[0]);
                Attributes[1].Add(attribute[1]);
                Attributes[2].Add(attribute[2]);
                Attributes[3].Add(attribute[3]);
                Attributes[4].Add(new List<string>(new string[] { "自动", "手动" }).IndexOf(attribute[4]).ToString());//自动手动序号
                Attributes[5].Add(attribute[5]);
                Attributes[6].Add(attribute[6]);
                Attributes[7].Add(attribute[7]);
            }
            for (int i = 0; i < JieOu.UnvisibleFunctionProperty.Count; i++)
            {
                int index = NeedAddArray.IndexOf(JieOu.UnvisibleFunctionProperty[i].Name);
                if (index != -1)
                {//ArrayInfo顺序依次为控件PortName、属性VarName、类型、数组名、长度、数组
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
                    {//自动/手动为char型
                        ArrayInfo[2] = "uint8";
                    }
                    ArrayInfo.Add(JieOu.UnvisibleFunctionProperty[i].VarName);
                    ArrayInfos.Add(ArrayInfo.ToArray());
                }
                else if (JOUnuseArray.Contains(JieOu.UnvisibleFunctionProperty[i].Name))
                {//不使用的数组赋值
                    JieOu.UnvisibleFunctionProperty[i].TheValue 
                        = "configuration" + CassViewGenerator.ProjectNum + "_array" + (ArrayCount++).ToString();
                    List<string> tempValue = new List<string>();//生成解耦路数个全0数组
                    for (int k = 0; k < count; k++)
                    { tempValue.Add("0"); }
                    UnseeArray.Add(new string[] { null, null, "fp32", JieOu.UnvisibleFunctionProperty[i].TheValue.ToString(), count.ToString(), String.Join(",", tempValue.ToArray()) });
                }
            }
            return ArrayInfos;
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
        /// 获取对应Cassview中控件的数组信息
        /// </summary>
        /// <param name="curCas"></param>
        /// <param name="count">数组起始序号</param>
       static public List<string[]> CatchArrayInfo(CassView curCass, ref int count)
        {
            List<string[]> Ainfos = new List<string[]>();
            foreach (Control element in curCass.Controls)
            {//获取模糊和解耦的数组信息
                string MS = TypeDescriptor.GetProperties(element)["ModuleSort"].GetValue(element).ToString();
                if (MS == "模糊控制器")
                {
                    string[] info = GenerateCode.GetFuzzyArray(curCass, element,ref count);
                    Ainfos.Add(info);
                }
                else if (MS == "解耦补偿控制器")
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
