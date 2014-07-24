using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Xml;
using AxCodeSense;
using CodeSense;
using CaVeGen.DesignViewFiles.FilterProperty;


namespace CaVeGen.DesignViewFiles.CodeEditor
{
    [ToolboxItem(true)]
    public partial class PLCCodeEditor : AxCodeSense.AxCodeSense
    {
        int lastLineNumber;          // 上次行号        
        bool isHeadMark = false;        // 是否多行注释
        bool isEndMark = false;     // 是否是行尾注释
        int rowHeight = 15;         //行高15像素
        int charWeight = 9;         //字宽9像素
        cmTokenType lastTokenType;
        Comment commandTipSet = new Comment();
        

        public List<ControlInfo> Codectrlsinfo = new List<ControlInfo>();//指令控件信息
        public Dictionary<string, string[]> CodearraysInfo = new Dictionary<string, string[]>();//指令数组信息
        public List<string> Codetempvalue = new List<string>();//指令临时变量信息
        public List<string[]> Codeiolist = new List<string[]>();//指令信息
        private Dictionary<ArrayList, int> CodeInfo = new Dictionary<ArrayList, int>();//<行号,分类过的信息>

        //输出参数信息 <参数，<行号,ST or LD>>
        private Dictionary<string, Dictionary<string, string>> SLinfo = new Dictionary<string, Dictionary<string, string>>();

        private int MaxLine = 0;//最大有效行 
        private Dictionary<string, int> Codeportlist = new Dictionary<string, int>();//记录功能块点名对应的行号信息
        static public List<string> CanTipCode = new List<string>(new string[] { "LD", "ST", "JMP", "CALL" });//属性有提示的指令集合

        static public List<string[]> ErrorList = new List<string[]>();//错误列表 数组四位分别为行号；错误信息；错误、警告还是消息；所在页面名
        static public Dictionary<string, string> SunningCodes = new Dictionary<string, string>();//高亮指令集合
        static public Dictionary<string, ControlInfo> CtrlPropertys = new Dictionary<string, ControlInfo>();//指令与控件信息集合
        static public Dictionary<string, string> SpecialCode = new Dictionary<string, string>();//特殊指令对应代码集合



        public ArrayList alBreakPoints = new ArrayList();  // 断点列表,存放的是行号        
        public int iRunLineNo = -1;   // 运行的行号

        const int WM_KEYDOWN = 0x0100;
        const int WM_CHAR = 0x0102;
        const int VK_LEFT = 0x25;
        const int VK_UP = 0x26;
        const int VK_RIGHT = 0x27;
        const int VK_DOWN = 0x28;
        const int VK_ENTER = 0x0D;
        const int VK_ESC = 0x1B;
        const int VK_SPACE = 0x20;
        const int WM_LBUTTONDOWN = 0x201;
        const int WM_LBUTTONMID = 0x20A;//鼠标滚轮
        const int WM_LBUTTONDBLCLK = 0x0203;
        const int VK_DELETE = 0x2E;
        const int VK_BACK = 0x8;
        const int WM_PAINT = 0xf;
        const int WM_NCPAINT = 0x85;
        const int MS_MOVE = 0x200;


        // 标号长度
        int markLength = 20;
        [DefaultValue(20)]
        public int MarkLength
        {
            get { return markLength; }
            set { markLength = value; }
        }
        // 操作码长度
        int operatorLength = 20;
        [DefaultValue(20)]
        public int OperatorLength
        {
            get { return operatorLength; }
            set { operatorLength = value; }
        }
        // 操作数长度
        int paraLength = 40;
        [DefaultValue(40)]
        public int ParaLength
        {
            get { return paraLength; }
            set { paraLength = value; }
        }


        #region 初始化
        public PLCCodeEditor()
            : base()
        {
            InitializeComponent();

            this.SelChange += new AxCodeSense.ICodeSenseEvents_SelChangeEventHandler(PLCCodeEditor_SelChange);
            this.Controls.Add(commandTipSet);
            this.commandTipSet.Hide();
            commandTipSet.ListClick += new System.EventHandler(this.commandTipSet_MouseClick);
        }




        // 映射替换的结构体
        public struct ReflectionStruct
        {
            public ArrayList valueKList;
            public ArrayList xReflectionList;
            public ArrayList yReflectionList;
        }

        /// <summary>
        /// 设置PLC语言,并确定高亮关键字
        /// </summary>
        static public void SetPLCLanguage()
        {
            Language dcs_lan = new Language();
            foreach (string SunningKey in SunningCodes.Keys)
            {
                dcs_lan.Keywords += SunningKey + "\n";
            }
            foreach (string Code in SpecialCode.Keys)
            {
                dcs_lan.Keywords += Code + "\n";
            }
            dcs_lan.Operators = "&" + "\n" + "*";
            dcs_lan.SingleLineComments = "//";
            dcs_lan.MultiLineComments1 = "(*";
            dcs_lan.MultiLineComments2 = "*)";
            dcs_lan.ScopeKeywords1 = "{";
            dcs_lan.ScopeKeywords2 = "}";
            dcs_lan.EscapeChar = "\\";

            Globals glo = new Globals();

            glo.RegisterLanguage("DCSLan", dcs_lan);
            HotKey hotkey = new HotKey();   // 全选
            hotkey.Modifiers1 = 0x02;
            hotkey.VirtKey1 = "A";
            glo.RegisterHotKey(hotkey, cmCommand.cmCmdSelectAll);
            hotkey.Modifiers1 = 0x02;       // 查找与替换
            hotkey.VirtKey1 = "F";
            glo.RegisterHotKey(hotkey, cmCommand.cmCmdFindReplace);

        }

        #endregion

        #region 格式化

        //获得行的范围
        private Range GetLineRange(int lineNo, int colLength)
        {
            Range range = new Range();
            range.StartLineNo = lineNo;
            range.EndLineNo = lineNo;
            range.StartColNo = 0;
            range.EndColNo = colLength;         //光标的range
            return range;
        }

        /// <summary>
        /// 获得当前行号
        /// </summary>
        /// <returns></returns>
        private int GetCaretLineNumber()
        {
            return this.GetSel(true).StartLineNo;
        }

        /// <summary>
        /// 刷新指令信息
        /// 获得当前行所在指令信息CodeInfo中的序号
        /// </summary>
        /// <returns></returns>
        private int GetCaretLineIndex(int CurrentRindex)
        {
            GetCode();
            List<int> tempNum = new List<int>();
            tempNum.AddRange(CodeInfo.Values);//对应序号列表
           //int cout = GetCaretLineNumber();
           return tempNum.IndexOf(CurrentRindex);//当前需要提示行所在序号
        }
        // 获得当前位置
        private Position GetCaretPosition()
        {
            Position p = new Position();
            p.LineNo = this.GetSel(false).StartLineNo;
            p.ColNo = this.GetSel(false).StartColNo;
            return p;
        }
        // 获得当前字的起始位置
        private Position GetWordPosition()
        {
            Position ps = GetCaretPosition();
            ps.ColNo = ps.ColNo - CurrentWordLength;
            return ps;
        }
        // 格式化
        private void Format()
        {
            //选择和行号不变:不作格式化            
            //if (lastLineNumber == GetCaretLineNumber())
            //    return;
            if (SelLength != 0)
            {
                lastLineNumber = GetCaretLineNumber();
                lastTokenType = CurrentToken;
                return;
            }

            string formatedText = FormatRow(lastLineNumber, true, false);
            string noFormatedText = GetLine(lastLineNumber);
            if (formatedText == noFormatedText)        //出错没进行格式化
            {
                lastLineNumber = GetCaretLineNumber();
                lastTokenType = CurrentToken;
                return;
            }

            Position position = GetCaretPosition();                            //先保存实际的Caret,一删一加Caret变了,Token也会变  

            int length = (formatedText.Length > noFormatedText.Length) ? formatedText.Length : noFormatedText.Length;
            ReplaceText(formatedText, GetLineRange(lastLineNumber,
                GetLineLength(lastLineNumber)));       //获取当前行的字符数,不能超过,否则会有溢出

            SetCaretPos(position.LineNo, position.ColNo);  //还原Caret

            lastLineNumber = GetCaretLineNumber();                     //当前行变为上一行  
            lastTokenType = CurrentToken;
        }

        /// <summary>
        ///  改变内容和光标移动,并进行提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PLCCodeEditor_SelChange(object sender, ICodeSenseEvents_SelChangeEvent e)
        {
            //this.CreateGraphics().FillEllipse(Brushes.Red, new Rectangle(0, 0, 10, 10));


            //选择和行号不变:不作格式化            

            if (lastLineNumber == GetCaretLineNumber())
            {
                lastTokenType = CurrentToken; // After edit Token will be changed
                return;
            }
            if (SelLength != 0)
            {
                lastLineNumber = GetCaretLineNumber();
                lastTokenType = CurrentToken;
                return;
            }

            string formatedText = FormatRow(lastLineNumber, true, false);
            string noFormatedText = GetLine(lastLineNumber);
            if (formatedText == noFormatedText)        //出错没进行格式化
            {
                lastLineNumber = GetCaretLineNumber();
                lastTokenType = CurrentToken;
                return;
            }

            Position position = GetCaretPosition();                            //先保存实际的Caret,一删一加Caret变了,Token也会变  

            int length = (formatedText.Length > noFormatedText.Length) ? formatedText.Length : noFormatedText.Length;
            ReplaceText(formatedText, GetLineRange(lastLineNumber,
                GetLineLength(lastLineNumber)));       //获取当前行的字符数,不能超过,否则会有溢出

            SetCaretPos(position.LineNo, position.ColNo);  //还原Caret

            lastLineNumber = GetCaretLineNumber();                     //当前行变为上一行  
            lastTokenType = CurrentToken;
        }

        /// <summary>
        /// 获得光标前的字符串
        /// 如果是左右键调用则参数为-1或1
        /// 并网上查找连接符返回完整光标前的字符串
        /// </summary>
        /// <param name="LeftOrRight"></param>
        /// <param name="UpOrDown"></param>
        /// <returns></returns>
        private string GetCaretString(int LeftOrRight, int UpOrDown)
        {//需要根据左右键情况 对位置进行相对偏移
            int lineNum = GetCaretLineNumber() + UpOrDown;

            string input = GetLine(lineNum);
            input = RemoveMarkPart(input, IsPureMark(input));
            Range range = GetSel(false);
            if (input.Length >= range.EndColNo && range.EndColNo + LeftOrRight >= 0 && range.EndColNo + LeftOrRight < input.Length)
                input = input.Substring(0, range.EndColNo + LeftOrRight);

            while (lineNum >= 1)
            {//存在上一行
                string line = GetLine(--lineNum);//上一行信息
                line = RemoveMarkPart(line.Trim(), IsPureMark(line));//去除该行的注释和空格
                if (line.EndsWith(@"\"))
                {//当上一行以连接符结尾则去掉连接符后接到当前行字符串前
                    input = line.TrimEnd('\\') + input.Trim(); ;
                }
                else//不存在连接符则中断
                { break; }
            }
            return input;
        }

        /// <summary>
        /// 根据坐标找到对应的行
        /// </summary>
        /// <param name="LeftOrRight"></param>
        /// <param name="UpOrDown"></param>
        /// <returns></returns>
        private string GetCaretString(Point MousePoint)
        {//需要根据左右键情况 对位置进行相对偏移
           Point m = PointToClient(MousePoint);
            int lineNum = m.Y / this.rowHeight;
            int pIndex = m.X / this.charWeight;
            string input = GetLine(lineNum);
            this.commandTipSet.Location = new Point((pIndex - 5) * this.charWeight, (lineNum + 1) * this.rowHeight);

            input = RemoveMarkPart(input, IsPureMark(input));
            //Range range = GetSel(false);
            if (input.Length >= pIndex && pIndex >= 0 && pIndex < input.Length)
                input = input.Substring(0, pIndex);

            while (lineNum >= 1)
            {//存在上一行
                string line = GetLine(--lineNum);//上一行信息
                line = RemoveMarkPart(line.Trim(), IsPureMark(line));//去除该行的注释和空格
                if (line.EndsWith(@"\"))
                {//当上一行以连接符结尾则去掉连接符后接到当前行字符串前
                    input = line.TrimEnd('\\') + input.Trim(); ;
                }
                else//不存在连接符则中断
                { break; }
            }
            return input;
        }

        /// <summary>
        ///  提示悬浮框
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="UpOrDown">行偏移</param>
        private void SetTipFloating(Position ps, int UpOrDown)
        {
            RectClass rc = (RectClass)this.PosFromChar(ps);
           if (this.commandTipSet.Items.Count > 0)
            {
                // 获得左下角的坐标
                Point pt = new Point(rc.left, rc.bottom + rowHeight * UpOrDown);
                this.commandTipSet.Location = pt;
                this.commandTipSet.SelectedIndex = 0;// 弹出列表框先选择第一项
                this.commandTipSet.Show();;
            }
            else
            {
                Point pt = new Point(rc.left - this.commandTipSet.Size.Width, rc.bottom + rowHeight * UpOrDown);
                this.commandTipSet.Location = pt;
                this.commandTipSet.Hide();
            }
        }

        // 去掉逗号前后的空格
        private string RemoveEmptyCom(string text)
        {
            // 逗号后有空格

            if (text.Contains(", "))
            {
                text = text.Replace(", ", ",");
                text = RemoveEmptyCom(text);
            }
            // 逗号前有空格
            if (text.Contains(" ,"))
            {
                text = text.Replace(" ,", ",");
                text = RemoveEmptyCom(text);
            }
            return text;
        }
        // 获得行的类型
        private cmTokenType GetLineType(int lineNo, Position caretPos)
        {
            cmTokenType cm;
            SetCaretPos(lineNo, 2);        //设置光标,获得类型
            cm = CurrentToken;
            SetCaretPos(caretPos.LineNo, caretPos.ColNo);//还原光标
            return cm;

        }

        /// <summary>
        /// 获得行的类型,包含纯单行|纯起始行|多行注释(不包含第一行),也就是无任何代码,返回为Multi,否则为unknow
        /// </summary>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        private cmTokenType GetLineType(int lineNo)
        {
            string text = GetLine(lineNo).Trim();
            if (text.StartsWith(@"(*") || text.StartsWith(@"//"))   // 含纯单行|纯起始行
                return cmTokenType.cmTokenTypeMultiLineComment;

            if (text.Contains("(*") || text.Contains(@"//"))    // 不是纯注释
                return cmTokenType.cmTokenTypeUnknown;

            if (FindRealRemark(lineNo))          // 多行注释(不包含第一行)
                return cmTokenType.cmTokenTypeMultiLineComment;
            else
                return cmTokenType.cmTokenTypeUnknown;

        }

        /// <summary>
        /// 多行注释标志
        /// </summary>
        /// <param name="currentLineText"></param>
        private void SetLineType(string currentLineText)
        {
            if (currentLineText.Contains("(*"))
            {
                isHeadMark = true;
                if (currentLineText.Contains("*)"))
                {
                    isEndMark = true;
                }
                else
                {
                    isEndMark = false;
                }
            }
            else if (currentLineText.Contains("*)"))
            {
                if (isHeadMark == true)
                    isEndMark = true;
                else
                    isEndMark = false;
                isHeadMark = false;
            }
            else
            {
                if (isEndMark == true && isHeadMark == true) // 上一行是单行注释
                {
                    isHeadMark = false;
                    isEndMark = false;
                }
                if (isEndMark == true)
                    isEndMark = false;
            }
        }

        /// <summary>
        /// 找注释起始符,找到返回True,否则返回False
        /// </summary>
        /// <param name="lineNo"></param>
        /// <returns></returns> 
        private bool FindRealRemark(int lineNo)
        {
            if (lineNo == 0)        // 第一行
                return false;

            string rangeText = string.Empty;    // 查找的范围           
            for (int i = 0; i < lineNo; i++)
            {
                rangeText += GetLine(i).Trim();
            }
            // 从后往前查
            for (int i = rangeText.Length - 1; i > 0; i--)
            {
                if (rangeText[i] == '*')        // 遇到第一个起始匹配:*
                {
                    if (rangeText[i - 1] == '(')  // 遇到第二个起始匹配:(,返回True
                        return true;
                }

                if (rangeText[i] == ')')        // 遇到第一个结束匹配:)
                {
                    if (rangeText[i - 1] == '*')  // 遇到第二个结束匹配:*,返回false
                        return false;
                }
            }
            // 找不到返回False
            return false;
        }
        // 找到代码行
        private int GetCodeLine(int lineNo)
        {
            string text = GetLine(lineNo).Trim();
            // 多行注释递归到有代码行 只有标号 空行
            while (GetLineType(lineNo) == cmTokenType.cmTokenTypeMultiLineComment || text == "")
            {
                lineNo--;
                text = GetLine(lineNo).Trim();
            }
            return lineNo;
        }
        // 上一行是否有连接符
        public bool isContinueLine(int preLineNo)
        {
            preLineNo = GetCodeLine(preLineNo);
            bool isContinue = false;
            string text = GetLine(preLineNo).Trim();
            // 存在单行注释
            if (text.Contains("(*") && text.Contains("*)"))      // 单行注释
            {
                text = text.Substring(0, text.IndexOf("(*"));
            }

            if (text.Contains("//"))                            // 单行注释
            {
                text = text.Substring(0, text.IndexOf("//"));
            }
            if (text.Contains("(*") && !text.StartsWith("(*")) // 注释的第一行但不能只有注释
            {
                text = text.Substring(0, text.IndexOf("(*"));
            }

            text = text.Trim();
            if (text.EndsWith("\\"))
                isContinue = true;
            return isContinue;

        }

        /// <summary>
        /// 将插入行进行格式化后显示
        /// </summary>
        /// <param name="oper">操作码</param>
        /// <param name="para">参数码</param>
        /// <param name="index">添加行</param>
        public void FormatInsert(string oper, string para, ref int index)
        {
            if (para != null && para.Length > paraLength)
            {//参数长度大于限制长度时
                FormatInsert(oper, para.Substring(0, paraLength - 2) + "\\", ref index);
                index++;
                FormatInsert(null, para.Substring(paraLength - 2), ref index);
            }
            else
            {
                InsertLine(index, oper + " " + para);
                ReplaceText(FormatRow(index, true, false), GetLineRange(index, GetLineLength(index)));
            }
        }

        /// <summary>
        ///  格式化行,isContinue是否是连接行
        /// </summary>
        /// <param name="lineNo">行号</param>
        /// <param name="isEdit"></param>
        /// <param name="getError">是否获取格式错误的信息</param>
        /// <returns></returns>
        public string FormatRow(int lineNo, bool isEdit, bool getError)
        {
            string tempRow = GetLine(lineNo);  // 不对返回临时变量，不进行格式化         
            string text = tempRow.Trim();
            if (text.Length == 0)
                return tempRow;
            //cmTokenType lastTokenType = GetLineType(lineNo);    // 获得行的类型
            SetLineType(text);                                  // 设置注释标志
            // 注释的最后还有文字
            if (text.Contains("*)"))
            {
                int index = text.IndexOf("*)") + 2;
                if (text.Substring(index).Trim().Length != 0)
                {
                    if (getError)
                    {
                        ErrorList.Add(new string[] { lineNo.ToString(), "格式错误:注释后还有文字", "error" });
                    }
                    return tempRow;
                }
            }

            int totalLength = markLength + operatorLength + paraLength;

            // 格式不合的不进行格式化
            char[] cs = new char[] { ' ' };

            text = text.Replace("\t", " ");    // 删除缩进符           

            string[] texts;
            string remarkText = ""; // 注释内容
            string markText = "";   // 标号内容 

            if (isEdit == true)
            {
                if ((lastTokenType == cmTokenType.cmTokenTypeMultiLineComment && !text.Contains("(*")) ||
                    text.StartsWith("(*") || text.StartsWith(@"//"))        // 纯注释
                {
                    text = text.TrimStart(' ');
                    text = text.TrimEnd(' ');
                    text = text.PadLeft(totalLength + text.Length, ' ');
                    return text;
                }
                if (text.Contains("(*") && !text.StartsWith("(*")) // 注释的第一行但不能只有注释
                {
                    remarkText = text.Substring(text.IndexOf("(*"));
                    text = text.Substring(0, text.IndexOf("(*"));
                }

                if (text.Contains("(*") && text.Contains("*)"))      // 单行注释
                {
                    remarkText = text.Substring(text.IndexOf("(*"));
                    text = text.Substring(0, text.IndexOf("(*"));

                }

                if (text.Contains("//"))                            // 单行注释
                {
                    remarkText = text.Substring(text.IndexOf("//"));
                    text = text.Substring(0, text.IndexOf("//"));
                }
            }// Edit状态
            else
            {
                if (isHeadMark == true)
                {
                    if ((text.Contains("(*") && !text.StartsWith("(*")))
                    {
                        remarkText = text.Substring(text.IndexOf("(*"));
                        text = text.Substring(0, text.IndexOf("(*"));
                    }// 注释的第一行但不能只有注释
                    else
                    {
                        text = text.TrimStart(' ');
                        text = text.TrimEnd(' ');
                        text = text.PadLeft(totalLength + text.Length, ' ');
                        return text;
                    }// 纯注释
                }
                else
                {
                    if (isEndMark == true || text.StartsWith(@"//"))
                    {
                        text = text.TrimStart(' ');
                        text = text.TrimEnd(' ');
                        text = text.PadLeft(totalLength + text.Length, ' ');
                        return text;
                    }// False中的结尾

                    if (text.Contains(@"//") && !text.StartsWith(@"//"))                           // 单行注释
                    {
                        remarkText = text.Substring(text.IndexOf("//"));
                        text = text.Substring(0, text.IndexOf("//"));
                    }
                }
            }// Scan状态

            bool isContinue = false;
            if (lineNo > 0)
                isContinue = isContinueLine(lineNo - 1);
            if (!isContinue)
            {// 不是连接行
                // 第二才有标号提出标号，标号中无空格
                if ((!text.Contains("File")) && (!text.Contains("FILE")) && text.Contains(":"))          // 提出第一个带":"
                {
                    markText = text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    text = text.Remove(0, markText.Length + 1); // 加上标号
                    // 删除标号中的空格
                    markText = markText.Trim();
                    string markSplit = null;
                    foreach (string s in markText.Split(cs, StringSplitOptions.RemoveEmptyEntries))
                    {
                        markSplit += s;
                    }
                    markText = markSplit;
                    markText += ":";
                }
                if (markText.Length > markLength)
                {
                    if (getError)
                    {
                        ErrorList.Add(new string[] { lineNo.ToString(), "格式错误:标号太长", "error" });
                    }
                    text = tempRow;
                    return text;
                }
                markText = markText.PadRight(markLength, ' ');  // 格式化标号 
                markText = markText.ToUpper();      // 标号进行大写
                // 去逗号边的空格
                text = RemoveEmptyCom(text);
                // 提出标号，注释，逗号边的空格后判断行是否格式正确            
                text = text.ToUpper();  // 操作码，参数大写
                texts = text.Split(cs, StringSplitOptions.RemoveEmptyEntries);
                // 有多余成份
                if (texts.Length > 2)
                {
                    if (getError)
                    {
                        ErrorList.Add(new string[] { lineNo.ToString(), "格式错误:多余代码出现", "error" });
                    }
                    text = tempRow;
                    return text;
                }

                if (texts.Length != 0)   // 有指令
                {
                    if (texts.Length == 1)        // 只有操作码
                    {
                        if (texts[0].Length > operatorLength)
                        {
                            if (getError)
                            {
                                ErrorList.Add(new string[] { lineNo.ToString(), "格式错误:操作码太长", "error" });
                            }
                            text = tempRow;
                            return text;
                        }
                        texts[0] = texts[0].PadRight(operatorLength + paraLength, ' ');
                        text = markText + texts[0];
                    }
                    else            // 超过部份并且不是注释则删除
                    {
                        if (texts[0].Length > operatorLength)
                        {
                            if (getError)
                            {
                                ErrorList.Add(new string[] { lineNo.ToString(), "格式错误:操作码太长", "error" });
                            }
                            text = tempRow;
                            return text;
                        }
                        if (texts[1].Length > paraLength)
                        {
                            //AddWrongSentence("格式错误:操作数太长", lineNo + 1, true);
                            //string temp = texts[1].Substring(0, 36) + "\\";
                            //textInsert(texts[1].Substring(36), lineNo + 1);
                            //texts[1] = temp;
                            text = tempRow;
                            return text;
                        }
                        if (texts[1].EndsWith("\\"))
                            isContinue = true;
                        else
                            isContinue = false;
                        texts[1] = texts[1].PadRight(paraLength, ' ');    // 参数
                        texts[0] = texts[0].PadRight(operatorLength, ' ');    // 操作码
                        text = markText + texts[0] + texts[1];
                    }

                    if (remarkText != "")    // 表示头行注释            
                        text += remarkText;
                }
                else   // 无指令
                {
                    markText = markText.PadRight(totalLength, ' ');
                    text = markText + remarkText;
                }
            }
            else
            {// 连接行
                // 去逗号边的空格
                text = RemoveEmptyCom(text);
                // 提出标号，注释，逗号边的空格后判断行是否格式正确            
                text = text.ToUpper();  // 操作码，参数大写
                texts = text.Split(cs, StringSplitOptions.RemoveEmptyEntries);
                if (texts.Length > 1)
                {// 不止是参数
                    //AddWrongSentence("格式错误:多余代码出现", lineNo + 1, true);
                    text = tempRow;
                    return text;
                }
                if (texts[0].Length > paraLength)
                {
                    //AddWrongSentence("格式错误:操作数太长", lineNo + 1, true);
                    text = tempRow;
                    return text;
                }
                if (texts.Length == 1)
                {
                    markText = markText.PadRight(markLength + operatorLength, ' ');
                    texts[0] = texts[0].PadRight(paraLength, ' ');// 参数
                    text = markText + texts[0] + remarkText;
                    if (texts[0].EndsWith("\\"))
                        isContinue = true;
                    else
                        isContinue = false;
                }

            }

            return text;

        }
        // 是否改变过
        bool isDirty = false;

        /// <summary>
        /// 是否修改过
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                isDirty = value;
            }
        }
        private void PLCCodeEditor_Change(object sender, ICodeSenseEvents_ChangeEvent e)
        {
            isDirty = true;
        }

        #endregion

        // 将哪一行进行高亮显示
        public void ShowHighLightRow(int lineNo)
        {
            if (lineNo < this.LineCount && lineNo != -1)
            {
                HighlightedLine = lineNo;
                SetCaretPos(lineNo, 0);
            }
        }

        // 加入断点
        public void AddBreakPoint(int lineNo)
        {
            if (!alBreakPoints.Contains(lineNo))
                alBreakPoints.Add(lineNo);
        }

        // 移除断点
        public void RemoveBreakPoint(int lineNo)
        {
            if (alBreakPoints.Contains(lineNo))
                alBreakPoints.Remove(lineNo);
        }

        // 设置运行的行号
        public void SetRunLine(int lineNo)
        {
            iRunLineNo = lineNo;
        }

        #region 参数替换,提取操作码、操作数、

        /// <summary>
        /// 获得所要的代码格式
        /// </summary>
        public void GetCode()
        {
            HighlightedLine = -1;    // 高亮取消

            //List<string> marks = new List<string>();      // 标号
            //List<string> editingOperators = new List<string>();        // 当前编辑的操作码
            //List<List<string>> parameters = new List<List<string>>();     // 参数集合，关键字为操作码   
            //ArrayList usingLineNum = new ArrayList(); // 使用的行号
            //ArrayList replaceLineNum = new ArrayList();   // 替换后的行号
            string allFormatedText = null;
            CodeInfo.Clear();
            // 提出操作码,并提出输出型集令集合
            isHeadMark = false;
            bool isContinue = false;
            //string allFormatedText = "";
            for (int lineNo = 0; lineNo < LineCount; lineNo++)
            {
                string formatedText = FormatRow(lineNo, false, false);     // 先格式化每一行,后面的查错判断必须在格式化的情况下进行
                if (lineNo != LineCount - 1)
                    allFormatedText += formatedText + "\n";
                else
                    allFormatedText += formatedText;

             

                ArrayList oppamk = TakeOperatorAndPara(formatedText, lineNo, ref isContinue);  // 提出操作码 
                if (oppamk.Count != 0)
                {
                    CodeInfo.Add(oppamk, lineNo);
                    MaxLine = lineNo;//用于记录最后一行行号
                }
            }

            //格式化完成 更新
            //Text = allFormatedText;     

            // 最后组合替换
            //CompositionReplace(ref parameters, ref editingOperator, ref marks, ref usingLineNum, ref replaceLineNum);

            //code.editingOperator = editingOperators;
            //code.parameters = parameters;
            //code.marks = marks;
            //code.lineCount = LineCount;
            //code.replaceLineNum = replaceLineNum;

        }

        /// <summary>
        /// 分离参数为类型与数字,返回类型部份
        /// </summary>
        /// <param name="para"></param>
        /// <param name="num"></param>
        /// <returns></returns> 
        private string DepartPara(string para, ref string num)
        {
            string type = "";

            if (para[0] == 'H') // 十六进制
            {
                return "H";
            }
            else
            {
                foreach (char c in para)
                {
                    // 不是数字
                    if (c < '0' || c > '9')
                    {
                        type += c;
                    }
                    else
                    {
                        num += c;
                    }

                }
            }
            return type;
        }

        /// <summary>
        /// 除去注释部份
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isPureMark"></param>
        /// <returns></returns> 
        private string RemoveMarkPart(string text, bool isPureMark)
        {
            if (isPureMark == false)
            {
                if (text.Contains("(*") && !text.Contains(@"//"))  // 只有多行注释标记
                {
                    int markIndex = text.IndexOf("(*");
                    text = text.Substring(0, markIndex);
                }
                else if (!text.Contains("(*") && text.Contains(@"//"))  // 只有单行注释标记
                {
                    int markIndex = text.IndexOf(@"//");
                    text = text.Substring(0, markIndex);
                }
                else if (text.Contains("(*") && text.Contains(@"//"))  // 两者都有
                {
                    int markIndex = text.IndexOf("(*") < text.IndexOf(@"//") ? text.IndexOf("(*") : text.IndexOf(@"//");
                    text = text.Substring(0, markIndex);
                }
            }
            else
            {
                return "";      // 纯注释返回空
            }
            return text;
        }

        /// <summary>
        /// 判断当前行是否是纯注释
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns> 
        private bool IsPureMark(string text)
        {
            if ((isHeadMark == true &&              //头注释
                 (text.StartsWith("(*") ||
                 text.StartsWith(@"//") ||
                 (!text.Contains("(*") &&
                 !text.Contains(@"//")))) ||

                 isEndMark == true &&                //尾注释
                 (!text.Contains("(*") ||
                 text.StartsWith("(*") ||
                 text.IndexOf("*)") < text.IndexOf("(*"))    //尾注释后还跟着一个头注释
                )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 参数合并,连接符删去,返回是否具有连接符 text为移除注释的内容
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineNo"></param> 
        private void MergeParameters(ref string text, int lineNo)
        {
            string connectText = text;
            bool isContinue = connectText.Trim().EndsWith(@"\");
            bool isPureMark = false;
            bool endTemp = false;
            bool headTemp = false;
            // 保存注释两个变量
            endTemp = isEndMark;
            headTemp = isHeadMark;
            while (isContinue)
            {// 循环所有连接符                
                text = text.Trim().TrimEnd('\\');

                connectText = GetLine(++lineNo);
                connectText = connectText.Trim();
                SetLineType(connectText); // 设置下类型
                isPureMark = IsPureMark(connectText); // 判断是否是纯注释
                connectText = RemoveMarkPart(connectText, isPureMark);        // 移除注释部份
                while (connectText == "")
                {// 找到非空行
                    if (lineNo == LineCount)
                        break;
                    connectText = GetLine(++lineNo);
                    connectText = connectText.Trim();
                    SetLineType(connectText); // 设置下类型
                    isPureMark = IsPureMark(connectText); // 判断是否是纯注释
                    connectText = RemoveMarkPart(connectText, isPureMark);        // 移除注释部份
                }

                text += connectText.Trim().ToUpper().TrimEnd('\\');
                isContinue = connectText.Trim().EndsWith(@"\");

            }
            // 还原
            isEndMark = endTemp;
            isHeadMark = headTemp;
        }


        /// <summary>
        /// 提出标号与操作码与参数,在考虑注释和标号的情况下 isContinue是判断是否是连接符的行
        /// </summary>
        /// <param name="Linetext">当前指令行</param>
        /// <param name="LineNo">行号</param>
        /// <param name="isContinue">当前行是否为连接行</param>
        /// <returns>返回当前行得三部分信息组合后数组</returns>
        private ArrayList TakeOperatorAndPara(string Linetext, int LineNo, ref bool isContinue)
        {
            ArrayList OpPaMk = new ArrayList();//当前行操作符、参数、标号
            Linetext = RemoveMarkPart(Linetext.Trim(), IsPureMark(Linetext));    // 移除空格和注释部份
            bool isNextContinue = Linetext.Trim().EndsWith(@"\"); // 下行是否要连接

            string curMark = "";
            string curOperator = "";
            List<string> curParas = new List<string>(); // 当前操作符对应的参数集合

            if (!isContinue && Linetext.Trim().Length > 0)
            {//跳过连接行和空行
                MergeParameters(ref Linetext, LineNo);  // 连接符处理
                string[] rowInfo = Linetext.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);    // 不包含注释  

                if (!rowInfo[0].Contains(":"))    // 不包含标号
                {// 一定是操作码                    
                    curOperator = rowInfo[0];
                    if (rowInfo.Length >= 2)   // 两个部分数据
                    {
                        if (rowInfo[1].Contains(",")) // 第二个位置不是注释, 两个以上的参数时
                        {
                            SplitPara(rowInfo[1], ref curParas);
                        }
                        else   // 第二个位置不是注释,并只有一个参数
                        {
                            curParas.Add(rowInfo[1]);
                        }
                    }
                }   // if 不包含标号
                else    // 包含标号
                {
                    //// 标号不能重复
                    //if (marks.Contains(rowInfo[0].TrimEnd(':')))
                    //{
                    //    ErrorList.Add(new string[] { (LineNo + 1).ToString(), "标号出现重复", "error" });
                    //}
                    //if (rowInfo[0].Length > markLength)
                    //{
                    //    ErrorList.Add(new string[] { (LineNo + 1).ToString(), "标号过长", "error" });
                    //}

                    curMark = rowInfo[0].TrimEnd(':');

                    if (rowInfo.Length >= 2)
                    {// 不只包含标号 
                        curOperator = rowInfo[1];
                        if (rowInfo.Length >= 3)   // 三部分数据
                        {
                            if (rowInfo[2].Contains(",")) // 两个以上的参数时
                            {
                                SplitPara(rowInfo[2], ref curParas);
                            }
                            else    // 一个参数   
                            {
                                curParas.Add(rowInfo[2]);
                            }
                        }
                    }
                }
            }// 不为空

            if (curMark != "")
            {
                OpPaMk.Add(curOperator);
                OpPaMk.Add(curParas);
                OpPaMk.Add(curMark);
            }
            else if (curOperator != "")
            {
                OpPaMk.Add(curOperator);
                OpPaMk.Add(curParas);
            }
            if (Linetext.Length > 0)// 只有当前行不是注释,才能赋值
                isContinue = isNextContinue;
            return OpPaMk;
        }

        /// <summary>
        /// 从参数字符串中提取单个参数，数组参数为一个单位
        /// </summary>
        /// <param name="paraInfo">参数字符串</param>
        /// <param name="curParas">参数数组</param>
        private void SplitPara(string paraInfo, ref List<string> curParas)
        {
            string[] tempSa = paraInfo.Split('{');
            foreach (string Sa in tempSa)
            {
                string t = Sa.TrimEnd(',');
                if (t.Contains("}"))
                {
                    string[] Ea = t.Split('}');
                    curParas.Add(Ea[0]);
                    if (Ea.Length > 1 && Ea[1] != "")
                    { curParas.AddRange(Ea[1].Split(',')); }
                }
                else
                {
                    curParas.AddRange(t.Split(','));
                }
            }
        }


        // 添加出错语句,有行号的,并且有提示的
        //public void AddWrongSentence(string wrongContext, int wrongNumber, string tip, bool isWrong)
        //{
        //    CompileClass.WrongInfoStruct wis = new CompileClass.WrongInfoStruct();
        //    wis.wrongContent = wrongContext;
        //    wis.lineNumber = wrongNumber;
        //    wis.tip = tip;
        //    if (isWrong)
        //        info.wrongInfo.Add(wis);
        //    else
        //        info.warningInfo.Add(wis);
        //}
        // 添加出错语句,有行号的
        //public void AddWrongSentence(string wrongContext, int wrongNumber, bool isWrong)
        //{
        //    CompileClass.WrongInfoStruct wis = new CompileClass.WrongInfoStruct();
        //    wis.wrongContent = wrongContext;
        //    wis.lineNumber = wrongNumber;
        //    wis.tip = "";
        //    if (isWrong)
        //        this.info.wrongInfo.Add(wis);
        //    else
        //        this.info.warningInfo.Add(wis);
        //}
        // 添加出错语句,无行号的为-1
        //public void AddWrongSentence(string wrongContext, bool isWrong)
        //{
        //    CompileClass.WrongInfoStruct wis = new CompileClass.WrongInfoStruct();
        //    wis.wrongContent = wrongContext;
        //    wis.lineNumber = -1;
        //    wis.tip = "";
        //    if (isWrong)
        //        this.info.wrongInfo.Add(wis);
        //    else
        //        this.info.warningInfo.Add(wis);
        //}

        // 将组合进行替换,最后换的话效率太低了
        //private void CompositionReplace(ref ArrayList parameters, ref ArrayList editingOperator, ref ArrayList marks,
        //    ref ArrayList usingLineNum, ref ArrayList replaceLineNum)
        //{
        //    ArrayList alOperator = (ArrayList)editingOperator.Clone();
        //    int insertIndex = 0;
        //    foreach (string eachOperator in alOperator)
        //    {
        //        foreach (FunctionComposition.CompostionStruct cs in compositions)
        //        {
        //            // 遇到组合关键字
        //            if (cs.commandKey == eachOperator)
        //            {
        //                // 参数替换
        //                ArrayList repParaList = ParameterReplace((ArrayList)parameters[insertIndex], cs.parametersModule, cs.parameterSite);
        //                // 插入操作码
        //                editingOperator.RemoveAt(insertIndex);
        //                editingOperator.InsertRange(insertIndex, cs.operators);
        //                // 插入操作数
        //                parameters.RemoveAt(insertIndex);
        //                parameters.InsertRange(insertIndex, repParaList);
        //                // 插入标号
        //                marks.RemoveAt(insertIndex);
        //                marks.InsertRange(insertIndex, cs.marks);

        //                // 加入行号对应关系
        //                usingLineNum.Add(insertIndex + 1);
        //                ArrayList al = new ArrayList();
        //                for (int i = 0; i < editingOperator.Count; i++)
        //                {
        //                    al.Add(insertIndex + i + 1);
        //                }
        //                replaceLineNum.Add(al);
        //            }
        //        }
        //        insertIndex++;
        //    }
        //}
        /// <summary>
        /// 组合中参数进行替换
        /// </summary>
        /// <param name="paraList">参数列表</param>
        /// <param name="repParaList">要被替换的参数</param>
        /// <param name="siteList">位置信息</param>
        /// <returns></returns>
        private ArrayList ParameterReplace(ArrayList paraList, ArrayList repParaList, ArrayList siteList)
        {
            int paraIndex = 0;
            // 每个参数
            for (int sentence = 0; sentence < repParaList.Count; sentence++)
            {
                ArrayList al = (ArrayList)repParaList[sentence];
                for (int site = 0; site < al.Count; site++)
                {
                    // 空参数跳过,不进行替换
                    if (al[site].ToString().Trim() != "")
                    {
                        al[site] = paraList[(int)siteList[paraIndex]].ToString();
                        paraIndex++;
                    }
                }
            }
            return repParaList;
        }
        #endregion

        #region 消息处理
        /// <summary>
        /// 预处理键盘回车等事件，提示列表处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool PreProcessMessage(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_KEYDOWN:
                    //Tiptimer.Enabled = false;//显示用时间片停转
                    switch ((int)msg.WParam)
                    {
                        case VK_UP://上键
                            if (this.commandTipSet.SelectedIndex > 0)
                            {
                                this.commandTipSet.SelectedIndex--;
                                return true;

                            }
                            else if (this.commandTipSet.SelectedIndex ==
                                0)
                            {
                                return true;  // 第一项，不动
                            }

                            SetTipFloating(GetWordPosition(), -1);
                            this.commandTipSet.ShowTipText(GetCaretString(0, -1));
                            break;
                        case VK_DOWN://下键
                            if (this.commandTipSet.SelectedIndex <
                                this.commandTipSet.Items.Count - 1)
                            {
                                this.commandTipSet.SelectedIndex++;
                                return true;  // 前面几项下移
                            }
                            else if (this.commandTipSet.SelectedIndex ==
                                this.commandTipSet.Items.Count - 1 && this.commandTipSet.SelectedIndex != -1)
                            {
                                return true;  // 最后一项，不动
                            }
                            SetTipFloating(GetWordPosition(), 1);
                            this.commandTipSet.ShowTipText(GetCaretString(0, 1));
                            break;
                        case VK_ENTER://回车
                            if (this.commandTipSet.SelectedIndex > -1)
                            {
                                AutoCode();
                                return true;
                            }
                            this.currentTipList.Clear();//回车时清空提示列表
                            commandTipSet.HideALL();
                            break;
                        case VK_SPACE://空格
                            if (this.commandTipSet.SelectedIndex > -1)
                            {
                                AutoCode();
                                return true;
                            }
                            //AutoAttribute();
                            //this.commandTipSet.HideALL();
                            break;
                        case VK_LEFT://左键
                            //this.commandTipSet.SetTipList(GetCaretString(-1,0), this.CurrentWord);
                            SetTipFloating(GetWordPosition(), 0);
                            this.commandTipSet.ShowTipText(GetCaretString(-1, 0));
                            break;
                        case VK_RIGHT://右键
                            //this.commandTipSet.SetTipList(GetCaretString(1,0), this.CurrentWord);
                            SetTipFloating(GetWordPosition(), 0);
                            this.commandTipSet.ShowTipText(GetCaretString(1, 0));
                            break;
                        case VK_ESC://取消
                            this.commandTipSet.HideALL();
                            return true;
                        // 删除键不进行预处理
                        case VK_BACK:
                            if (GetCaretString(-1, 0).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == 0)
                            {
                                this.commandTipSet.HideALL();
                            }
                            return false;
                        case VK_DELETE:
                            return false;
                        default:
                            break;

                    }
                    break;
                case WM_LBUTTONDOWN:
                    //MessageBox.Show("aa");
                    break;
                default:
                    break;
            }
            return base.PreProcessMessage(ref msg);
        }

        /// <summary>
        /// 基本消息,但不包含键盘的预处理消息(Del enter 上下左右)
        /// </summary>
        /// <param name="m"></param> 
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CHAR:
                    base.WndProc(ref m); // 输入      
                    //Tiptimer.Enabled = false;//时间片停转
                    string curRow = GetCaretString(0, 0);

                    if (curRow.Contains(":"))
                    { //取标号
                        curRow = curRow.Split(':')[1];
                    }
                    string[] rowInfo = curRow.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (rowInfo.Length != 0)
                    {//非空 且并不是操作指令刚结束 也不是按清空键
                        AutoAttribute(rowInfo);
                    }
                    SetTipFloating(GetWordPosition(), 0);  // 提示
                    return;
                case WM_LBUTTONDOWN://鼠标左键  // 打断点
                    base.WndProc(ref m);//点两次光标才有效
                    this.commandTipSet.HideALL();
                    //SetTipFloating(GetWordPosition());
                    //this.commandTipSetting1.ShowTipText(GetCaretString(0));
                    ///20090709点击不显示提示

                    //if (this.commandTipSetting1 != null)
                    //{
                    //    this.commandTipSetting1.Items.Clear();
                    //    this.commandTipSetting1.Hide();
                    //}
                    //int x = (int)m.LParam & (0xFFFF);
                    //int y = ((int)m.LParam >> 16);
                    //PositionClass maxPc = new PositionClass();
                    //maxPc.LineNo = this.LineCount - 1; // 最大行号
                    //maxPc.ColNo = 0;
                    //RectClass maxRc = (RectClass)PosFromChar(maxPc); // 最大行号的位置
                    //if (x < 25 && y <           Rc.bottom)
                    //{// 单击有效边距处
                    //    //if(slBreakPoints.ContainsKey(
                    //    PositionClass ps = (PositionClass)GetSelFromPoint(x + 25, y);  // 点击边距是获得不到位置,必须在内容里
                    //    if (alBreakPoints.Contains(ps.LineNo))
                    //    {
                    //        RemoveBreakPoint(ps.LineNo);
                    //    }
                    //    else
                    //    {
                    //        if (ps.LineNo < this.LineCount)
                    //        {
                    //            if (this.Code.editingOperator != null)
                    //            {
                    //                if (!this.Code.editingOperator[ps.LineNo].Equals("MPS") && !this.Code.editingOperator[ps.LineNo].Equals("MPP")
                    //                    && !this.Code.editingOperator[ps.LineNo].Equals("MRD")
                    //                    && !this.Code.editingOperator[ps.LineNo].Equals("ANB") && !this.Code.editingOperator[ps.LineNo].Equals("ORB"))
                    //                {
                    //                    AddBreakPoint(ps.LineNo);
                    //                }
                    //                else
                    //                {
                    //                    //CassMessageBox.CassMessageBox.Information("该位置未能插入断点！");
                    //                }
                    //            }
                    //            else
                    //            {
                    //                //CassMessageBox.CassMessageBox.Information("请先编译再新建断点！");
                    //            }
                    //            //AddBreakPoint(ps.LineNo);
                    //            //SetRunLine(ps.LineNo);
                    //        }
                    //    }
                    //}
                    break;
                case WM_NCPAINT:
                    base.WndProc(ref m);
                    //this.CreateGraphics().FillEllipse(Brushes.Red, new Rectangle(0, 0, 10, 10));
                    //SetTipFloating(GetWordPosition());
                    //GetWordPosition();
                    //this.commandTipSetting1.ShowTipText(GetCaretString(0));
                    return;
                case WM_PAINT:
                    base.WndProc(ref m);
                    foreach (int lineNo in alBreakPoints)
                    {
                        PositionClass pc = new PositionClass();
                        pc.ColNo = 0;
                        pc.LineNo = lineNo;

                        //RectClass rc = (RectClass)PosFromChar(pc);
                        //Bitmap imageBreakPoint = global::SystemConfiguration.Properties.Resources.BreakPoint;
                        //this.CreateGraphics().FillEllipse(Brushes.Red, new Rectangle(8, rc.top, 15, 15));
                        //this.CreateGraphics().DrawImage(imageBreakPoint, new Point(8, rc.top));
                        //imageBreakPoint.Dispose();
                    }
                    if (iRunLineNo > -1 && iRunLineNo < LineCount)
                    {
                        PositionClass pc = new PositionClass();
                        pc.ColNo = 0;
                        pc.LineNo = iRunLineNo;

                        //RectClass rc = (RectClass)PosFromChar(pc);
                        //Bitmap imageBreakPoint = global::SystemConfiguration.Properties.Resources.RUN;
                        //if (alBreakPoints.Contains(iRunLineNo))
                        //imageBreakPoint = global::SystemConfiguration.Properties.Resources.Break_Run;
                        //this.CreateGraphics().DrawImage(imageBreakPoint, new Point(8, rc.top));
                        //imageBreakPoint.Dispose();
                    }
                    return;
                case MS_MOVE:
                    //GetChildAtPoint(Control.MousePosition);
                    //this.commandTipSet.Location = Control.MousePosition;
                    //if (Tiptimer.Enabled == false)
                    //{
                    //    Tiptimer.Start();
                    //}
                    //else
                    //{//时间片已经启动，则重启
                    //    Tiptimer.Stop();
                    //    Tiptimer.Start();
                    //}                  
                    break;
                case WM_LBUTTONMID:
                    //if (commandTipSet.Items.Count != 0)
                    //{
                        commandTipSet.HideALL();
                    //} 
                    break;
                default:
                    break;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 时间片到达显示鼠标下的属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tiptimer_Tick(object sender, EventArgs e)
        {
            this.commandTipSet.ShowTipText(GetCaretString(Control.MousePosition));
            Tiptimer.Stop();
        }

        #endregion

        /// <summary>
        /// 点击提示列表信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commandTipSet_MouseClick(object sender, EventArgs e)
        { 
            AutoCode();
        }

        public List<string> currentTipList = new List<string>();//当前相关的所有提示信息

        /// <summary>
        /// 从当前相关的所有提示信息中选择
        /// 以关键字开头的提示信息加入提示列表中
        /// </summary>
        /// <param name="Keyword"></param>
        public void selectTip(string Keyword)
        {
            this.commandTipSet.Items.Clear();
            foreach (string tip in currentTipList)
            {
                if (Keyword == null || tip.ToUpper().StartsWith(Keyword.ToUpper()))
                {//都转换成大写才进行匹配
                    this.commandTipSet.Items.Add(tip);
                }
            }
        }

        /// <summary>
        /// 自动完成代码
        /// </summary> 
        private void AutoCode()
        {
            Range range = GetSel(true);

            // 逗号和点不能替换
            if (CurrentWord == "," || CurrentWord == ".")
                range.StartColNo = range.EndColNo - CurrentWordLength + 1;
            else
                range.StartColNo = range.EndColNo - CurrentWordLength;

            range.EndColNo = range.StartColNo + this.commandTipSet.SelectedItem.ToString().Length;
            ReplaceText(this.commandTipSet.SelectedItem.ToString(), range);
            SetCaretPos(range.StartLineNo, range.EndColNo);

            // 回车时提示框消失
            this.commandTipSet.Items.Clear();
            this.commandTipSet.Hide();
        }

        /// <summary>
        /// 给予相应的属性提示
        /// </summary>
        /// <param name="rowInfo">指令信息数组</param>
        private void AutoAttribute(string[] rowInfo)
        {
            List<string> ShowtipList = new List<string>();//加入到提示列表中的列表
            List<ArrayList> tempCode = new List<ArrayList>();//指令信息
            List<string> PntipList = new List<string>();//点名列表
            List<string> Marklist = new List<string>();//标示符列表              
            List<string> LDtiplist = new List<string>();//LD提示列表
            string Keyword = null;//用户键入的关键字
            int curIndex = GetCaretLineIndex(GetCaretLineNumber()); //函数中包括刷新指令列表            
            AutoGetInfo(ref tempCode, ref PntipList, ref Marklist, ref LDtiplist);

            if (rowInfo.Length == 1)
            {
                Keyword = rowInfo[0].Trim().ToUpper();
                if (this.CurrentWord == ""
                    //||this.CurrentWord.Length!=1
                    )
                {//输入空格 //另外一个判断条件是在 “JMP  MAIN1”的MAIN1前按空格时出现的情况 原因不明
                    if (PLCCodeEditor.CtrlPropertys.ContainsKey(Keyword))
                    {//且操作符部分指令正确
                        int Codeindex = CanTipCode.IndexOf(rowInfo[0].ToUpper());//提示的指令
                        if (Codeindex >= 0)
                        {//ST LD JMP CALL 找到相关的提示信息
                            if (Codeindex == 0)
                            {//提示列表为点名和可LD临时变量
                                foreach (string tip in PntipList)
                                {
                                    ShowtipList.Add(tip);
                                }
                                foreach (string tip in LDtiplist)
                                {
                                    ShowtipList.Add(tip);
                                }
                            }
                            else if (Codeindex == 1)
                            { //提示列表为点名和可ST临时变量                               
                                foreach (string tip in PntipList)
                                {
                                    ShowtipList.Add(tip);
                                }
                                for (int j = curIndex - 1; j >= 0; j--)
                                {//从当前序号的上一行开始向上遍历
                                    if (CtrlPropertys.ContainsKey((string)tempCode[j][0]))
                                    {
                                        ControlInfo codeCtrl = CtrlPropertys[((string)tempCode[j][0])];

                                        if (codeCtrl.OutputInfo.Count != 0 || tempCode[j].Count == 3)
                                        {//有输出控件或遍历到当前块头(有标示长度为3)
                                            if (codeCtrl.OutputInfo.Count != 1 && codeCtrl.OutputInfo.Count >= curIndex - j && ((List<string>)tempCode[j][1]).Count != 0)
                                            {//多输出控件 且在输出临时变量的行数范围之内
                                                //多输出控件临时变量必须在使用多输出控件指令后立即ST对应端口的临时变量，提示也如此
                                                //且该输出控件指令的参数必须不为0
                                                for (int x = 0; x < codeCtrl.OutputInfo.Count; x++)
                                                {//提示列表中加入控件点名_端口号
                                                    string tempValue = codeCtrl.CodeInfo[1] + ((List<string>)tempCode[j][1])[0] + "_" + x.ToString();
                                                    if (!LDtiplist.Contains(tempValue))
                                                    {//该临时变量没有ST过
                                                        ShowtipList.Add(tempValue);
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (Codeindex < 4) //JMP CALL
                            {
                                string curMark = null;//控件所在的标示符号
                                for (int j = curIndex; j >= 0; j--)
                                {
                                    if (tempCode[j].Count == 3)
                                    {
                                        curMark = tempCode[j][2].ToString();
                                        break;
                                    }
                                }
                                foreach (string tip in Marklist)
                                {
                                    string cutmark = GenerateCode.getMark(curMark);
                                    int IsError = tip.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });//提示中是否含有数字
                                    int curLevelNum = curMark.Split('_').Length;
                                    int tipLevelNum = tip.Split('_').Length;
                                    if ((Codeindex == 2 && curLevelNum == tipLevelNum && tip != curMark  && tip.StartsWith(cutmark)) //跳转排除自身
                                        || (Codeindex == 3 && IsError == -1 && tipLevelNum == curLevelNum + 1 && (tip.StartsWith(cutmark) || curLevelNum == 1)))//调用目标不含数字部分 且为下一级子页面
                                    {//不同指令提示范围不同
                                        ShowtipList.Add(tip);//跳跃提示加入提示列表
                                    }
                                }
                            }
                            currentTipList = ShowtipList;
                            selectTip(null);
                        }
                        else
                        {//普通指令 显示参数提示信息
                            //string ShowTip = String.Join(",", commandTipSet.GetTipmessage(PLCCodeEditor.CtrlPropertys[Keyword], 0).ToArray());
                            //commandTipSet.toolTip1.Show(ShowTip, commandTipSet, new Point(commandTipSet.Size.Width, 0));
                            commandTipSet.GetTipmessage(PLCCodeEditor.CtrlPropertys[Keyword], 0);
                        }
                    }
                }
                else
                {//正在输入操作符
                    currentTipList.Clear();
                    currentTipList.AddRange(PLCCodeEditor.SunningCodes.Keys);
                    currentTipList.AddRange(PLCCodeEditor.SpecialCode.Keys);
                    //从中选出满足关键字的指令
                    selectTip(Keyword);
                }
            }
            else if (rowInfo.Length == 2 && CurrentWord != "")
            {
                string[] tempValue = rowInfo[1].Split('.');
                if (tempValue.Length < 3)
                {
                    if (rowInfo[1].EndsWith("."))
                    {//输入完点名后的点
                        Keyword = null;
                        string PN = tempValue[0];
                        string CtrlName = null;
                        if (PntipList.Contains(PN))
                        {//存在对应的点名则寻找其可见属性
                            int hitMax = 0;//取最长匹配个数的控件
                            foreach (string key in PLCCodeEditor.SunningCodes.Keys)
                            {
                                if (PN.StartsWith(key) && key.Length > hitMax)
                                {
                                    CtrlName = key;
                                }
                            }
                            ControlInfo codeCtrl = PLCCodeEditor.CtrlPropertys[CtrlName];
                            foreach (XProp element in codeCtrl.VisibleFunctionProperty)
                            {
                                if (element.VarName != CassViewGenerator.portIndex)
                                {
                                    ShowtipList.Add(element.VarName);
                                }
                            }
                            this.commandTipSet.Tag = codeCtrl.CodeInfo[1];
                            currentTipList = ShowtipList;
                        }
                    }
                    else if (rowInfo[1].Contains("."))
                    {//输入为点名参数时
                        Keyword = tempValue[1].Trim();
                    }
                    else
                    {//没有"."存在 且键入的值非空格
                        Keyword = rowInfo[1].Trim(); 
                        //提示
                        commandTipSet.ShowTipText(rowInfo[0] + " " + rowInfo[1]);
                    }
                    selectTip(Keyword);
                }
                else
                { commandTipSet.HideALL(); }
            }     
        }

        /// <summary>
        /// 从当前指令表获取所需的信息数组
        /// </summary>
        /// <param name="CodeinfList">指令信息数组</param>
        /// <param name="PntipList">模块点名</param>
        /// <param name="Marklist">标示符</param>
        /// <param name="LDtiplist">ST申明的临时变量</param>
        private void AutoGetInfo(ref List<ArrayList> CodeinfList, ref List<string> PntipList, ref List<string> Marklist, ref List<string> LDtiplist)
        {
            CodeinfList.AddRange(this.CodeInfo.Keys);//指令行信息列表

            for (int i = 0; i < CodeinfList.Count; i++)
            {
                string Operator = (string)CodeinfList[i][0];
                List<string> Paras = (List<string>)CodeinfList[i][1];
                if (CodeinfList[i].Count == 3)
                {//记录标示符
                    Marklist.Add(CodeinfList[i][2].ToString());
                }
                if (Paras.Count > 1)
                {//寻找存在点名的控件指令并记录其点名和可见属性
                    ControlInfo codeCtrl = CtrlPropertys[Operator];
                    if (XProp.GetValue(CassViewGenerator.portIndex, codeCtrl.VisibleFunctionProperty) != null)
                    {
                        PntipList.Add(codeCtrl.CodeInfo[1] + Paras[0]);
                    }
                }
                else if (Operator == "ST" && Paras.Count != 0 && !LDtiplist.Contains(Paras[0]))
                {//记录所有ST申明的临时变量
                    LDtiplist.Add(Paras[0]);
                }
            }
        }
        
        /// <summary>
        /// 从XML文件中获取指令及其参数的描述信息
        /// 注：与CASSVIEW的添加控件类似 需要优化或统一
        /// </summary>
        static public void GetInfomation()
        {
            SunningCodes.Clear();
            CtrlPropertys.Clear();

            foreach (XmlNode categoryNode in ToolBoxServiceImpl.toolXML.FirstChild.ChildNodes)
            {
                if (categoryNode != null && categoryNode.Attributes[0].InnerText != CassViewGenerator.SpecialCodeNode)
                {
                    foreach (XmlNode toolItemNode in categoryNode.ChildNodes)
                    {
                        ControlInfo ctrlStruct = new ControlInfo();
                        ctrlStruct.CodeInfo = new string[3];
                        if (toolItemNode.FirstChild.Name == "BasicProperty")
                        {
                            foreach (XmlNode property in toolItemNode.FirstChild.ChildNodes)
                            {//基本信息中加入ModuleName和ModuleSort
                                if (property.Attributes["name"].Value == "ModuleName")
                                {
                                    //if (property.InnerText == "CALCU" || property.InnerText == "PROCESS")
                                    //{//跳过计算器组态和条件动作表控件
                                    //    break;
                                    //}
                                    ctrlStruct.CodeInfo[1] = property.InnerText;
                                }
                                else if (property.Attributes["name"].Value == "ModuleSort")
                                {
                                    ctrlStruct.SortName = property.InnerText;
                                    ctrlStruct.CodeInfo[0] = property.InnerText;
                                    if (CassView.OnlyOneIn.Contains(property.InnerText))
                                    {
                                        List<string[]> tempInfo = new List<string[]>();
                                        tempInfo.Add(new string[4] { null, null, null, "0" });
                                        ctrlStruct.InputInfo = tempInfo;
                                        ctrlStruct.OutputInfo = CassView.InitializeIOinfo(0);
                                    }
                                    else if (CassView.OnlyOneOut.Contains(property.InnerText))
                                    {
                                        ctrlStruct.OutputInfo = CassView.InitializeIOinfo(1);
                                        ctrlStruct.InputInfo = CassView.InitializeIOinfo(0);
                                    }
                                }
                                else if (property.Attributes["name"].Value == "OutputName")
                                { //初始化输出口信息
                                    if (property.InnerText != "NULL")
                                    {
                                        ctrlStruct.OutputInfo = CassView.InitializeIOinfo(property.InnerText.Split(',').Length);
                                    }
                                    else
                                    { ctrlStruct.OutputInfo = new List<string[]>(); }
                                }
                                else if (property.Attributes["name"].Value == "InputName")
                                {
                                    if (property.ChildNodes.Count > 1)
                                    {
                                        List<string[]> Inputinfo = new List<string[]>();
                                        foreach (XmlNode info in property.ChildNodes)
                                        {
                                            string[] Ininfo = new string[4];
                                            Ininfo[2] = info.Attributes["name"].Value;
                                            Ininfo[3] = info.InnerText;
                                            Inputinfo.Add(Ininfo);
                                        }
                                        ctrlStruct.InputInfo = Inputinfo;
                                    }
                                    else if (property.InnerText != "NULL")
                                    {
                                        ctrlStruct.InputInfo = CassView.InitializeIOinfo(property.InnerText.Split(',').Length);
                                    }
                                    else
                                    { ctrlStruct.InputInfo = new List<string[]>(); }
                                }
                            }
                        }
                        if (ctrlStruct.CodeInfo[1] == null && GenerateCode.SortCtrlName.Contains(ctrlStruct.SortName))
                        {//跳过注释控件
                            ctrlStruct.CodeInfo[1] = GenerateCode.CodeCtrlName[GenerateCode.SortCtrlName.IndexOf(ctrlStruct.SortName)];
                        }
                        //读入功能属性
                        if (toolItemNode.FirstChild.NextSibling.Name == "FunctionProperty")
                        {
                            CassView.ReadFunctionProperty(toolItemNode.FirstChild.NextSibling, ref ctrlStruct);
                        }
                        //读入代码信息
                        if ((toolItemNode.LastChild.Name == "CodeProperty" || toolItemNode.LastChild.Name == "OtherInfo") && toolItemNode.LastChild.ChildNodes.Count > 0)
                        {
                            CassView.ReadOtherProperty(toolItemNode.LastChild, ref ctrlStruct);
                        }

                        if (ctrlStruct.CodeInfo[1] != null && ctrlStruct.SortName != null)
                        {
                            SunningCodes.Add(ctrlStruct.CodeInfo[1], ctrlStruct.SortName);
                            CtrlPropertys.Add(ctrlStruct.CodeInfo[1], ctrlStruct);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据当前指令表生成对应的控件信息
        /// 并更新控件点名数组
        /// </summary>
        public void CreateCtrlsinfo()
        {
            GetCode();
            CassViewGenerator.CodePInfoList.Clear();
            CodearraysInfo.Clear();
            Codectrlsinfo.Clear();
            Codetempvalue.Clear();
            Codeiolist.Clear();
            Codeportlist.Clear();

            int arrayCount = 0;
            int moduleCount = 0;
            string curMark = null;
            foreach (ArrayList rowInfo in CodeInfo.Keys)
            {
                string Operator = (string)rowInfo[0];
                List<string> Paras = (List<string>)rowInfo[1];
                try
                {
                    if (rowInfo.Count == 3)
                    {
                        Codeiolist.Add(new string[] { rowInfo[2] + ":", null });
                        curMark = rowInfo[2].ToString();
                    }
                    if (Operator != "")
                    {
                        ControlInfo ctrlinfo = CassView.CopyCtrlinfo(CtrlPropertys[Operator]);

                        //指令表中的参数设置控件信息中对应的属性      
                        if (ctrlinfo.VisibleFunctionProperty != null && ctrlinfo.VisibleFunctionProperty.Count != 0)
                        {//控件可视属性存在则判定为算法控件
                            //20090709改成参数的第一位为点名序号

                            int paracout = 0;//参数序号
                            foreach (XProp element in ctrlinfo.VisibleFunctionProperty)
                            {
                                element.TheValue = Paras[paracout++];
                                if (element.VarName == CassViewGenerator.portIndex)
                                {//如果有点名序号属性
                                    ctrlinfo.CodeInfo[2] = ctrlinfo.CodeInfo[1] + element.TheValue;
                                    if (!Codeportlist.ContainsKey(ctrlinfo.CodeInfo[2]))
                                    {
                                        Codeportlist.Add(ctrlinfo.CodeInfo[2], CodeInfo[rowInfo]);//加载模块点名和对应行号信息
                                    }
                                    else
                                    {
                                        CassViewGenerator.SpecialErrors.Add(new string[] { CodeInfo[rowInfo].ToString(), "指令控件点名" + ctrlinfo.CodeInfo[2] + "出现重复！", "error", curMark });
                                    }
                                }
                            }
                            foreach (XProp element in ctrlinfo.UnvisibleFunctionProperty)
                            {
                                if (GenerateCode.JOUnuseArray.Contains(element.Name))
                                {//不使用的数组赋值
                                    element.TheValue
                                        = "configuration" + CassViewGenerator.ProjectNum + "_array" + (arrayCount++).ToString();
                                    List<string> tempValue = new List<string>();//生成解耦路数个全0数组
                                    int count = Convert.ToInt32(XProp.GetValue("iN", ctrlinfo.VisibleFunctionProperty));//解耦路数                                    
                                    for (int k = 0; k < count; k++)
                                    { tempValue.Add("0"); }
                                    GenerateCode.UnseeArray.Add(new string[] { null, null, "fp32", element.TheValue.ToString(), count.ToString(), String.Join(",", tempValue.ToArray()) });
                                }
                                else if (element.TheValue.ToString().Contains("array"))
                                {
                                    string[] array = new string[7];
                                    array[0] = ctrlinfo.CodeInfo[2];
                                    array[1] = element.Name;
                                    array[2] = AddressTable.ConvertShowType(element.ValueType);
                                    element.TheValue = "configuration" + CassViewGenerator.ProjectNum + "_array" + arrayCount++;
                                    array[3] = element.TheValue.ToString();
                                    array[5] = Paras[paracout++];
                                    array[4] = array[5].Split(',').Length.ToString();
                                    array[6] = element.VarName;
                                    CodearraysInfo.Add(array[3], array);
                                }
                            }
                            Codeiolist.Add(new string[] { Operator, null, ctrlinfo.CodeInfo[2] });
                        }
                        else if (ctrlinfo.UnvisibleFunctionProperty.Count != 0)
                        {//sin 等控件 只含有不可见属性
                            ctrlinfo.ControlNum = moduleCount++;
                            Codeiolist.Add(new string[] { Operator, null, ctrlinfo.ControlNum.ToString() });
                        }
                        else //if(ctrl.VisibleFunctionProperty.Count==0)
                        {
                            if (Paras.Count == 1)
                            {
                                string express = null;
                                if (Operator == "ST" || Operator == "LD")
                                {
                                    string[] temp = Paras[0].Split('.');
                                    ctrlinfo.CodeInfo[1] = temp[0];
                                    if (temp.Length == 2)
                                    {//控件
                                        //找到与当前控件点名最匹配的控件信息
                                        string CtrlName = null;
                                        int hitMax = 0;//取最长匹配个数的控件
                                        foreach (string key in PLCCodeEditor.SunningCodes.Keys)
                                        {
                                            if (temp[0].StartsWith(key) && key.Length > hitMax)
                                            {
                                                CtrlName = key;
                                            }
                                        }
                                        ControlInfo tempCtrl = CtrlPropertys[CtrlName];
                                        //遍历该控件信息中的课件属性找到小写版本的属性名进行赋值
                                        foreach (XProp element in tempCtrl.VisibleFunctionProperty)
                                        {
                                            if (element.VarName.ToUpper() == temp[1])
                                            {
                                                ctrlinfo.CodeInfo[2] = element.VarName;
                                                break;
                                            }
                                        }
                                        express = ctrlinfo.CodeInfo[1] + "." + ctrlinfo.CodeInfo[2];
                                    }
                                    else
                                    {//临时变量
                                        if (!Codetempvalue.Contains(temp[0]))
                                        {
                                            Codetempvalue.Add(temp[0]);
                                        }
                                        express = temp[0];
                                    }
                                }
                                else
                                { 
                                    ctrlinfo.CodeInfo[1] = Paras[0];
                                    express = Paras[0];
                                }
                                Codeiolist.Add(new string[] { Operator, express });
                            }
                            else
                            {//ST 后为空
                                //MessageBox.Show("!有问题！");
                            }
                        }
                        if (GenerateCode.SpicalCtrlName.Contains(ctrlinfo.SortName))
                        {
                            if (GenerateCode.SpicalCtrlName.IndexOf(ctrlinfo.SortName) > 1)
                            {//三者取中 高低选需先设置其IO口信息                                
                                foreach (string[] Info in ctrlinfo.InputInfo)
                                {//遍历输入口信息,将所有入口信息变成已连接
                                    Info[0] = "";
                                }
                                //根据当前块信息中CON NULL的个数对输入口信息进行更改
                                CodeSetspecialctrl(ctrlinfo, rowInfo);
                            }
                            GenerateCode.SetUnvisibleFunction(ctrlinfo);
                        }
                        this.Codectrlsinfo.Add(ctrlinfo);
                    }
                }
                catch
                { 
                    if(SpecialCode.ContainsKey(Operator))
                    {//特殊指令
                        Codeiolist.Add(new string[] { Operator, null });
                    }
                    continue;                
                }
            }
        }

        /// <summary>
        /// 对三者取中和高低选控件根据指令表中
        /// 出现CON null的个数进行修正
        /// </summary>
        /// <param name="specialCtrl"></param>
        /// <param name="special"></param>
        private void CodeSetspecialctrl(ControlInfo specialCtrl, ArrayList special)
        {
            //需要逆向遍历 且字典不支持序号遍历
            List<ArrayList> OrderInfo = new List<ArrayList>();
            OrderInfo.AddRange(CodeInfo.Keys);

            int unConnectNum = specialCtrl.InputInfo.Count;
            int inputIndex = 0;//所设定到的输入信息序号
            int currentNum = 0;//计数遍历到当前何端口

            for (int i = OrderInfo.IndexOf(special) - 1; i >= 0; i--)
            {//逆向遍历 起始行为特殊控件的上一行
                string Operator = (string)OrderInfo[i][0];
                List<string> Paras = (List<string>)OrderInfo[i][1];

           
                if (Operator == "")
                {//当前块结束
                    return;
                }
                else
                {
                    ControlInfo NextCtrl = CtrlPropertys[Operator];                 

                    unConnectNum = unConnectNum + NextCtrl.InputInfo.Count - NextCtrl.OutputInfo.Count;
                    if (currentNum > unConnectNum)
                    {
                        currentNum = unConnectNum;
                        if (Paras.Count != 0  && Paras[0].ToUpper() == "NULL")
                        {
                            specialCtrl.InputInfo[inputIndex++][0] = null;
                        }
                    }     
                    if (unConnectNum <= 0)
                    {//当前控件所需输入数量满足则返回
                        return;
                    }
                }
            }
        }

        #region 查错

        /// <summary>
        /// 判定当前的参数与类型是否匹配，并返回
        /// </summary>
        /// <param name="proType">参数类型</param>
        /// <param name="proValue">参数值</param>
        /// <param name="proEnum">自定义类型参数选择项</param>
        /// <returns>是否匹配</returns>
        private bool checkProperty(string proType, object proValue, string proEnum)
        {
            try
            {
                if (proType == "System.Single")
                { Convert.ToSingle(proValue); }
                else if (proType == "System.Int32")
                { Convert.ToUInt32(proValue); }
                else if (proType == "MyEnum")
                {
                    int MaxCount = proEnum.Split(',').Length;
                    int CurSelect = Convert.ToUInt16(proValue);
                    if (CurSelect < 0 || CurSelect >= MaxCount)
                    {
                        return false;
                    }
                }
                else if (proType == "System.Boolean")
                {
                    int CurSelect = Convert.ToUInt16(proValue);
                    if (CurSelect != 0 && CurSelect != 1)
                    {
                        return false;
                    }
                }
                else if (proType == "COM")
                {
                    //串口 差错预留
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检验跳转指令是否出现错误
        /// </summary>
        /// <param name="JMPlist">跳转指令信息列表</param>
        /// <param name="marks">控件块名列表</param>
        private void checkJmp(List<string[]> JMPlist, List<string> marks)
        {
            foreach (string[] info in JMPlist)
            {
                if (info.Length != 4)
                {
                    //MessageBox.Show("不可能的错误！");
                }
                else
                {

                    if (!marks.Contains(info[3]))
                    {
                        ErrorList.Add(new string[] { info[0], info[1] + "跳转目标不存在", "error", info[2] });
                    }
                    //注：
                    //JMP只能在页面相同（即块名中"_"分隔第一部分相同）间跳转
                    //CALL只能在页面不相同间跳转  不能含有数字 且自能调用下一级子页面 
                    string curBlock = GenerateCode.getMark(info[2]);//所在的当前页面
                    string tagBlock = GenerateCode.getMark(info[3]);//跳转目标所在的页面
                    if (info[1] == "JMP" && curBlock != tagBlock
                        || info[1] == "CALL" && (curBlock == tagBlock || tagBlock != info[3] 
                        || (tagBlock.Substring(curBlock.Length).Contains("_") && tagBlock.Substring(curBlock.Length).Substring(1).Contains("_"))))
                    {
                        ErrorList.Add(new string[] { info[0], info[1] + "调用范围越权", "error", info[2] });
                    }
                    else if (info[2] == info[3])
                    {
                        ErrorList.Add(new string[] { info[0], info[1] + "跳跃自身", "error", info[2] });
                    }
                    else
                    {//只有在非越权跳转和跳跃自身都无错下才进行循环跳转判定
                        Stack<List<string>> JmpPath = new Stack<List<string>>();//记录当前跳转条目的跳转路径                       
                        Stack<string> Target = new Stack<string>();//跳转过程中所用的跳转目标块列表
                        Stack<string> Rowindex = new Stack<string>();//与跳转目标绑定的对应条目行号
                        bool isRoll = false;//出现循环
                        string curNum = null;
                        string curTarget = null;
                        JmpPath.Push(new List<string>(new string[] { info[2] }));//当前所在的块名作为起点
                        Target.Push(info[3]);
                        Rowindex.Push(info[0]);

                        while (Target.Count != 0)
                        {
                            curTarget = Target.Pop();
                            curNum = Rowindex.Pop();
                            int waycount = 0;//计算当前块内的跳转数目
                            foreach (string[] Secinfo in JMPlist)
                            {
                                if (Secinfo[0] != curNum && curTarget == Secinfo[2])
                                {//排除自身 遍历到的跳转条目存在与当前的目标块
                                    if (Secinfo[3] == curTarget || JmpPath.Peek().Contains(Secinfo[3]))
                                    {//遍历到的跳转条目为跳转自身 或 跳转的目标存在与跳转路径
                                        isRoll = true;
                                        break;
                                    }
                                    else
                                    {//否则将其跳转目标压入目标堆栈
                                        Target.Push(Secinfo[3]);
                                        Rowindex.Push(Secinfo[0]);
                                        waycount++;
                                    }
                                }
                            }
                            if (waycount == 0)
                            {//顶部路径到达终点,弹出
                                JmpPath.Pop();
                            }
                            else if (waycount == 1)
                            {//正常路径,加载遍历到的块名
                                JmpPath.Peek().Add(curTarget);
                            }
                            else
                            {//出现分支,将当前顶部路径复制分叉路数压入路径堆栈
                                for (int o = waycount - 1; o > 0; o--)
                                {
                                    JmpPath.Push(JmpPath.Peek());
                                }
                            }
                            if (isRoll)
                            {
                                ErrorList.Add(new string[] { info[0], info[1] + "跳跃循环", "error", info[2] });
                                break;
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检验ST输出信息是否有误
        /// </summary>
        /// <param name="SLs">输入输出信息表</param>
        private void checkSL(Dictionary<string, Dictionary<string, string>> SLs)
        {
            foreach (string info in SLs.Keys)
            {
                if (info.Contains("."))
                {
                    string[] tempPN = info.Split('.');
                    if (Codeportlist.Count != 0 && !Codeportlist.ContainsKey(tempPN[0]))
                    {//点名集合中不存在该点名
                        foreach (string num in SLs[info].Keys)
                        {
                            ErrorList.Add(new string[] { num, info + SLs[info][num] + "点名不存在", "error" });
                        }
                    }
                    else
                    {//存在对应的点名则寻找其可见属性
                        bool findElement = false;
                        if (tempPN.Length == 2)
                        {
                            int hitMax = 0;//取最长匹配个数的控件
                            foreach (string key in PLCCodeEditor.SunningCodes.Keys)
                            {
                                if (tempPN[0].StartsWith(key) && key.Length > hitMax)
                                {
                                    ControlInfo codeCtrl = PLCCodeEditor.CtrlPropertys[key];
                                    foreach (XProp element in codeCtrl.VisibleFunctionProperty)
                                    {
                                        if (element.VarName.ToUpper() == tempPN[1])
                                        { findElement = true; }
                                    }
                                }
                            }
                        }
                        if (tempPN.Length == 1 || !findElement)
                        {
                            foreach (string num in SLs[info].Keys)
                            {
                                ErrorList.Add(new string[] { num, info + "源信息有误", "error" });
                            }
                        }
                    }
                }
                else
                {
                    if (SLs[info].ContainsValue("输出"))
                    {
                        foreach (string pn in Codeportlist.Keys)
                        {
                            if (info == pn)
                            {//与任意的点名相同
                                foreach (string num in SLs[info].Keys)
                                {
                                    ErrorList.Add(new string[] { num, SLs[info][num] + "源信息" + info + "与模块点名信息重复", "error" });
                                }
                                break;
                            }
                        }
                        if (SLs[info].ContainsValue("输出"))
                        {//当前的参数存在输出ST使用   
                            if (SLs[info].Count == 1)
                            {//但是没有LD调用
                                foreach (string num in SLs[info].Keys)
                                {
                                    ErrorList.Add(new string[] { num, SLs[info][num] + "临时变量" + info + "没有使用", "warning" });
                                }
                            }
                        }
                        else//!SLs[info].ContainsValue("输出")
                        {//存在参数使用,但是没有ST申明
                            foreach (string num in SLs[info].Keys)
                            {
                                ErrorList.Add(new string[] { num, SLs[info][num] + "临时变量" + info + "没有输出申明", "error" });
                            }
                        }
                    }

                    int errorIndex = checkName(info);
                    if (errorIndex == 0)
                    {
                        foreach (string num in SLs[info].Keys)
                        {
                            ErrorList.Add(new string[] { num, "临时变量名" + info + "不能以数字开头", "error" });
                        }
                    }
                    else if (errorIndex == -1)
                    {
                        foreach (string num in SLs[info].Keys)
                        {
                            ErrorList.Add(new string[] { num, "临时变量名" + info + "命名有误", "error" });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检验传入的变量名是否符合命名规则
        /// 不能以数字开头
        /// 只能包含数字字母或下划线
        /// </summary>
        /// <param name="tempName"></param>
        /// <returns>返回1为无误,0为数字开头错误,-1为其余错误</returns>
        private int checkName(string tempName)
        {
            if (tempName[0] >= '0' && tempName[0] <= '9')
            {
                return 0;
            }
            for (int i = 0; i < tempName.Length; i++)
            {//只能由英文、数字、下划线组成
                char p = tempName[i];
                if (p >= '0' && p <= '9' || p >= 'a' & p <= 'z' || p >= 'A' && p <= 'Z' || p == '_')
                {                }
                else
                { return -1; }
            }
            return 1;
        }

        /// <summary>
        /// 校验指令表 生成错误列表
        /// </summary>
        /// <param name="iolist"></param>
        /// <returns>返回是否有错误或警告</returns>
        public bool checkIolist()
        {
            ErrorList.Clear();//错误列表清空
            SLinfo.Clear();//输出信息清空
            List<string> marks = new List<string>();      // 标号
            List<string> editingOperators = new List<string>();        // 当前编辑的操作码
            List<List<string>> parameters = new List<List<string>>();     // 参数集合，关键字为操作码 
            List<string> numList = new List<string>();//指令行号列表
            List<string[]> JMPlist = new List<string[]>();

            FormateAllRow();//格式化所有行
            GetCode();

            foreach (ArrayList rowInfo in CodeInfo.Keys)
            {
                string Operator = (string)rowInfo[0];
                List<string> Paras = (List<string>)rowInfo[1];

                if (Operator != "")
                {
                    editingOperators.Add(Operator);
                    parameters.Add(Paras);
                    numList.Add(CodeInfo[rowInfo].ToString());
                }
                if ((Operator == "" && editingOperators.Count != 0 //当前没有操作符且操作符列表不为空 
                    || MaxLine == CodeInfo[rowInfo]) //最后行
                    && marks.Count != 0)//且标示符列表不为空
                {//块的断层
                    checkBlock(numList, editingOperators, parameters, marks[marks.Count - 1], ref JMPlist);
                    numList.Clear();
                    parameters.Clear();
                    editingOperators.Clear();
                }
                if (rowInfo.Count == 3)
                {
                    if (marks.Contains((string)rowInfo[2]))
                    {
                        ErrorList.Add(new string[] { CodeInfo[rowInfo].ToString(), "存在相同的标示符" + (string)rowInfo[2], "error" });
                    }
                    marks.Add((string)rowInfo[2]);
                }
            }
            //判断是否有跳跃错误
            checkJmp(JMPlist, marks);
            checkSL(SLinfo);

            if (!marks.Contains("main") && !marks.Contains("Main") && !marks.Contains("MAIN"))
            {
                ErrorList.Add(new string[] { null, "没有main函数", "error" });
            }
            if (ErrorList.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 以控件回路为单位进行判断错误
        /// </summary>
        /// <param name="BlockLineNum">控件行号列表</param>
        /// <param name="editOpers">控件操作符列表</param>
        /// <param name="paras">控件参数列表</param>
        /// <param name="curmark">当前控件回路块名</param>
        /// <param name="JMPlist">跳转列表</param>
        private void checkBlock(List<string> BlockLineNum, List<string> editOpers, List<List<string>> paras, string curmark, ref List<string[]> JMPlist)
        {
            int errorIndex = checkName(curmark);
            if (errorIndex == 0)
            {//此处标示符号的行号为该块第一行的行号-1
                ErrorList.Add(new string[] { (Convert.ToInt32(BlockLineNum[0]) - 1).ToString(), "标示符名" + curmark + "不能以数字开头", "error", curmark });
            }
            else if (errorIndex == -1)
            {
                ErrorList.Add(new string[] { (Convert.ToInt32(BlockLineNum[0]) - 1).ToString(), "标示符名" + curmark + "命名有误", "error", curmark });
            }

            for (int i = 0; i < editOpers.Count; i++)
            {
                if (editOpers[i] != "")
                {

                    if (editOpers[i] == "JMP" || editOpers[i] == "CALL")
                    {//记录跳跃信息行号，跳转符号，当前块名，目标块名
                        if (paras[i].Count != 0)
                        {
                            JMPlist.Add(new string[] { BlockLineNum[i], editOpers[i], curmark, paras[i][0] });
                        }
                        else
                        {
                            ErrorList.Add(new string[] { BlockLineNum[i], "跳转控件" + editOpers[i] + "没有跳转目标", "error", curmark });
                        }
                        continue;
                    }
                    if (SpecialCode.ContainsKey(editOpers[i]))
                    {//跳过特殊指令
                        continue;
                    }
                    if (!CtrlPropertys.ContainsKey(editOpers[i]))
                    {//现有指令
                        ErrorList.Add(new string[] { BlockLineNum[i], "不存在关键字" + editOpers[i], "error", curmark });
                        continue;
                    }
                    //判断是否为该块的输入模块
                    ControlInfo curCtrl = CtrlPropertys[editOpers[i]];
                    if (curCtrl.CodeInfo[1] == "ST" || curCtrl.CodeInfo[1] == "LD")
                    {
                        if (paras[i].Count == 0)
                        {
                            ErrorList.Add(new string[] { BlockLineNum[i], editOpers[i] + "对象为空", "error", curmark });
                        }
                        else if (curCtrl.CodeInfo[1] == "ST")
                        { //需记录输出信息
                            if (SLinfo.ContainsKey(paras[i][0]))
                            {//存在对应的输出参数
                                if (SLinfo[paras[i][0]].ContainsValue("输出"))
                                {//并且已经作为过输出对象
                                    ErrorList.Add(new string[] { BlockLineNum[i], "输出源" + paras[i][0] + "多次作为输出对象", "warning", curmark });
                                }
                                else
                                {//没有作为输出对象 则添加
                                    SLinfo[paras[i][0]].Add(BlockLineNum[i], "输出");
                                }
                            }
                            else if (editOpers[i] != "POP")
                            {
                                Dictionary<string, string> SLtemp = new Dictionary<string, string>();
                                SLtemp.Add(BlockLineNum[i], "输出");
                                SLinfo.Add(paras[i][0], SLtemp);
                            }
                        }
                        else if (curCtrl.CodeInfo[1] == "LD")
                        { //需记录输入信息
                            if (SLinfo.ContainsKey(paras[i][0]))
                            {//存在对应的输入参数,则对应的加入当前的信息
                                SLinfo[paras[i][0]].Add(BlockLineNum[i], "输入");
                            }
                            else
                            {//不存在,则新建
                                Dictionary<string, string> SLtemp = new Dictionary<string, string>();
                                SLtemp.Add(BlockLineNum[i], "输入");
                                SLinfo.Add(paras[i][0], SLtemp);
                            }
                        }
                    }
                    //判断参数个数和参数值范围
                    if (curCtrl.VisibleFunctionProperty != null && curCtrl.VisibleFunctionProperty.Count != 0)
                    {//控件可视属性存在则判定为算法控件
                          int paracout = 0;//正确的参数个数

                        foreach (XProp element in curCtrl.VisibleFunctionProperty)
                        {
                            if (paras[i].Count <= paracout)
                            { //实际遍历到参数 大于现有参数 则跳过操作
                                paracout++;
                                continue;
                            }
                            if (!checkProperty(element.ValueType, paras[i][paracout++], element.EnumValue))
                            {
                                ErrorList.Add(new string[] { BlockLineNum[i], "控件指令" + curCtrl.CodeInfo[1] + "参数" + element.VarName + ",值" + element.TheValue.ToString() + "范围出错", "error", curmark });
                            }
                        }

                        foreach (XProp element in curCtrl.UnvisibleFunctionProperty)
                        {
                            if (element.TheValue.ToString().Contains("array")//必须为数组参数 
                                && !GenerateCode.JOUnuseArray.Contains(element.Name))//且不为用户不可见数组
                            {
                                if (paras[i].Count <= paracout)
                                {//实际遍历到参数 大于现有参数 则跳过操作
                                    paracout++;
                                    continue;
                                }
                                string[] tempValue = paras[i][paracout++].Split(',');
                                if (tempValue.Length > 1)
                                {
                                    foreach (string temp in tempValue)
                                    {
                                        checkProperty(element.ValueType, temp, element.EnumValue);
                                    }
                                }
                                else
                                {
                                    ErrorList.Add(new string[] { BlockLineNum[i], "控件指令" + curCtrl.CodeInfo[1] + "数组参数" + element.VarName + "有误", "error", curmark });
                                    //break;
                                }
                            }
                        }
                        if (paracout > paras[i].Count)
                        {
                            ErrorList.Add(new string[] { BlockLineNum[i], curCtrl.CodeInfo[1] + "的参数个数不匹配(不足)", "error", curmark });
                        }
                        if (paracout < paras[i].Count)
                        {
                            ErrorList.Add(new string[] { BlockLineNum[i], curCtrl.CodeInfo[1] + "的参数个数不匹配(过多)", "error", curmark });
                        }
                    }
                    //判断输出引脚连接问题
                    if (curCtrl.OutputInfo != null && curCtrl.OutputInfo.Count != 0)
                    {//有输出
                        int unConnectNum = 0;
                        if (curCtrl.OutputInfo.Count != 1)
                        {
                            for (int j = 0; j < curCtrl.OutputInfo.Count; j++)
                            {
                                if (i + j + 1 >= editOpers.Count || (editOpers[i + j + 1] != "ST" && editOpers[i + j + 1] != "POP"))
                                {
                                    ErrorList.Add(new string[] { BlockLineNum[i], curCtrl.CodeInfo[1] + "的多输出的输出输出个数与ST个数不一致", "error", curmark });
                                    break;
                                }
                                else if (paras[i + j + 1].Count == 0 || editOpers[i + j + 1] == "POP")
                                {//当用户指令表模式下没有连接怎么显示？？20090706
                                    unConnectNum++;
                                }
                            }
                            if (unConnectNum == curCtrl.OutputInfo.Count)
                            {
                                ErrorList.Add(new string[] { BlockLineNum[i], "控件指令" + curCtrl.CodeInfo[1] + "的输出没连接", "warning", curmark });
                            }
                        }
                        else
                        {//会出现连锁错误！！！！！！！！考虑是否要解决20090707
                            unConnectNum = 1;
                            for (int j = unConnectNum; j + i < editOpers.Count; j++)
                            {//往下行遍历序号
                                if (CtrlPropertys.ContainsKey(editOpers[i + j]))
                                {//下行操作指令无误
                                    ControlInfo NextCtrl = CtrlPropertys[editOpers[i + j]];
                                    unConnectNum = unConnectNum - NextCtrl.InputInfo.Count + NextCtrl.OutputInfo.Count;
                                }
                                else if (editOpers[i + j] == "POP")
                                {
                                    unConnectNum--;
                                }
                                if (unConnectNum <= 0)
                                {//当前控件所需输入数量满足则跳出循环
                                    break;
                                }
                            }
                            if (unConnectNum > 0)
                            {//当当前回路遍历完还有未满足的端口存在则出错
                                ErrorList.Add(new string[] { BlockLineNum[i], "控制回路" + curmark + "中的控件" + curCtrl.CodeInfo[1] + "缺少输出模块", "error", curmark });
                            }
                        }
                    }
                    //判断输入口问题
                    if (curCtrl.InputInfo != null && curCtrl.InputInfo.Count != 0)
                    {
                        int unConnectNum = curCtrl.InputInfo.Count;
                        for (int j = 1; i - j >= 0; j++)
                        {//往上行遍历序号
                            if (CtrlPropertys.ContainsKey(editOpers[i - j]))
                            {//上行操作指令无误
                                ControlInfo NextCtrl = CtrlPropertys[editOpers[i - j]];
                                unConnectNum = unConnectNum + NextCtrl.InputInfo.Count - NextCtrl.OutputInfo.Count;
                            }
                            else if (editOpers[i - j] == "POP")
                            {
                                unConnectNum++;
                            }
                            if (unConnectNum <= 0)
                            {//当前控件所需输入数量满足则跳出循环
                                break;
                            }
                        }
                        if (unConnectNum > 0)
                        {//当当前回路遍历完还有未满足的端口存在则出错
                            ErrorList.Add(new string[] { BlockLineNum[i], "控制回路" + curmark + "中的控件" + curCtrl.CodeInfo[1] + "缺少输入模块", "error", curmark });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 格式化当前指令页面的所有行
        /// 并检测格式错误
        /// </summary>
        public void FormateAllRow()
        {
            string allFormatedText = null;
            for (int lineNo = 0; lineNo < LineCount; lineNo++)
            {
                string formatedText = FormatRow(lineNo, false, true);    // 先格式化每一行,后面的查错判断必须在格式化的情况下进行

                if (lineNo != LineCount - 1)
                    allFormatedText += formatedText + "\n";
                else
                    allFormatedText += formatedText;
            }
            //格式化完成 更新
            Text = allFormatedText;
        }

        #endregion

        /// <summary>
        /// 失去焦点,提示消息，但点击菜单时不会，vs也是这样的        
        /// </summary>
        /// <param name="e"></param> 
        protected override void OnLostFocus(EventArgs e)
        {
            this.commandTipSet.Items.Clear();
            this.commandTipSet.Hide();
            base.OnLostFocus(e);
        }

        /// <summary>
        /// 失去焦点事件
        /// 隐藏提示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PLCCodeEditor_KillFocus(object sender, ICodeSenseEvents_KillFocusEvent e)
        {
            if (commandTipSet.SelectedIndex == -1)
            {
                this.commandTipSet.HideALL();
            }
        }

        /// <summary>
        /// 竖滚动条事件
        /// 隐藏提示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PLCCodeEditor_VScroll(object sender, ICodeSenseEvents_VScrollEvent e)
        {
            this.commandTipSet.HideALL();
        }

        /// <summary>
        /// 横滚动条事件
        /// 隐藏提示框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PLCCodeEditor_HScroll(object sender, ICodeSenseEvents_HScrollEvent e)
        {
            this.commandTipSet.HideALL();
        }

    }
}
