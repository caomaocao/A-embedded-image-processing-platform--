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
        int lastLineNumber;          // �ϴ��к�        
        bool isHeadMark = false;        // �Ƿ����ע��
        bool isEndMark = false;     // �Ƿ�����βע��
        int rowHeight = 15;         //�и�15����
        int charWeight = 9;         //�ֿ�9����
        cmTokenType lastTokenType;
        Comment commandTipSet = new Comment();
        

        public List<ControlInfo> Codectrlsinfo = new List<ControlInfo>();//ָ��ؼ���Ϣ
        public Dictionary<string, string[]> CodearraysInfo = new Dictionary<string, string[]>();//ָ��������Ϣ
        public List<string> Codetempvalue = new List<string>();//ָ����ʱ������Ϣ
        public List<string[]> Codeiolist = new List<string[]>();//ָ����Ϣ
        private Dictionary<ArrayList, int> CodeInfo = new Dictionary<ArrayList, int>();//<�к�,���������Ϣ>

        //���������Ϣ <������<�к�,ST or LD>>
        private Dictionary<string, Dictionary<string, string>> SLinfo = new Dictionary<string, Dictionary<string, string>>();

        private int MaxLine = 0;//�����Ч�� 
        private Dictionary<string, int> Codeportlist = new Dictionary<string, int>();//��¼���ܿ������Ӧ���к���Ϣ
        static public List<string> CanTipCode = new List<string>(new string[] { "LD", "ST", "JMP", "CALL" });//��������ʾ��ָ���

        static public List<string[]> ErrorList = new List<string[]>();//�����б� ������λ�ֱ�Ϊ�кţ�������Ϣ�����󡢾��滹����Ϣ������ҳ����
        static public Dictionary<string, string> SunningCodes = new Dictionary<string, string>();//����ָ���
        static public Dictionary<string, ControlInfo> CtrlPropertys = new Dictionary<string, ControlInfo>();//ָ����ؼ���Ϣ����
        static public Dictionary<string, string> SpecialCode = new Dictionary<string, string>();//����ָ���Ӧ���뼯��



        public ArrayList alBreakPoints = new ArrayList();  // �ϵ��б�,��ŵ����к�        
        public int iRunLineNo = -1;   // ���е��к�

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
        const int WM_LBUTTONMID = 0x20A;//������
        const int WM_LBUTTONDBLCLK = 0x0203;
        const int VK_DELETE = 0x2E;
        const int VK_BACK = 0x8;
        const int WM_PAINT = 0xf;
        const int WM_NCPAINT = 0x85;
        const int MS_MOVE = 0x200;


        // ��ų���
        int markLength = 20;
        [DefaultValue(20)]
        public int MarkLength
        {
            get { return markLength; }
            set { markLength = value; }
        }
        // �����볤��
        int operatorLength = 20;
        [DefaultValue(20)]
        public int OperatorLength
        {
            get { return operatorLength; }
            set { operatorLength = value; }
        }
        // ����������
        int paraLength = 40;
        [DefaultValue(40)]
        public int ParaLength
        {
            get { return paraLength; }
            set { paraLength = value; }
        }


        #region ��ʼ��
        public PLCCodeEditor()
            : base()
        {
            InitializeComponent();

            this.SelChange += new AxCodeSense.ICodeSenseEvents_SelChangeEventHandler(PLCCodeEditor_SelChange);
            this.Controls.Add(commandTipSet);
            this.commandTipSet.Hide();
            commandTipSet.ListClick += new System.EventHandler(this.commandTipSet_MouseClick);
        }




        // ӳ���滻�Ľṹ��
        public struct ReflectionStruct
        {
            public ArrayList valueKList;
            public ArrayList xReflectionList;
            public ArrayList yReflectionList;
        }

        /// <summary>
        /// ����PLC����,��ȷ�������ؼ���
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
            HotKey hotkey = new HotKey();   // ȫѡ
            hotkey.Modifiers1 = 0x02;
            hotkey.VirtKey1 = "A";
            glo.RegisterHotKey(hotkey, cmCommand.cmCmdSelectAll);
            hotkey.Modifiers1 = 0x02;       // �������滻
            hotkey.VirtKey1 = "F";
            glo.RegisterHotKey(hotkey, cmCommand.cmCmdFindReplace);

        }

        #endregion

        #region ��ʽ��

        //����еķ�Χ
        private Range GetLineRange(int lineNo, int colLength)
        {
            Range range = new Range();
            range.StartLineNo = lineNo;
            range.EndLineNo = lineNo;
            range.StartColNo = 0;
            range.EndColNo = colLength;         //����range
            return range;
        }

        /// <summary>
        /// ��õ�ǰ�к�
        /// </summary>
        /// <returns></returns>
        private int GetCaretLineNumber()
        {
            return this.GetSel(true).StartLineNo;
        }

        /// <summary>
        /// ˢ��ָ����Ϣ
        /// ��õ�ǰ������ָ����ϢCodeInfo�е����
        /// </summary>
        /// <returns></returns>
        private int GetCaretLineIndex(int CurrentRindex)
        {
            GetCode();
            List<int> tempNum = new List<int>();
            tempNum.AddRange(CodeInfo.Values);//��Ӧ����б�
           //int cout = GetCaretLineNumber();
           return tempNum.IndexOf(CurrentRindex);//��ǰ��Ҫ��ʾ���������
        }
        // ��õ�ǰλ��
        private Position GetCaretPosition()
        {
            Position p = new Position();
            p.LineNo = this.GetSel(false).StartLineNo;
            p.ColNo = this.GetSel(false).StartColNo;
            return p;
        }
        // ��õ�ǰ�ֵ���ʼλ��
        private Position GetWordPosition()
        {
            Position ps = GetCaretPosition();
            ps.ColNo = ps.ColNo - CurrentWordLength;
            return ps;
        }
        // ��ʽ��
        private void Format()
        {
            //ѡ����кŲ���:������ʽ��            
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
            if (formatedText == noFormatedText)        //����û���и�ʽ��
            {
                lastLineNumber = GetCaretLineNumber();
                lastTokenType = CurrentToken;
                return;
            }

            Position position = GetCaretPosition();                            //�ȱ���ʵ�ʵ�Caret,һɾһ��Caret����,TokenҲ���  

            int length = (formatedText.Length > noFormatedText.Length) ? formatedText.Length : noFormatedText.Length;
            ReplaceText(formatedText, GetLineRange(lastLineNumber,
                GetLineLength(lastLineNumber)));       //��ȡ��ǰ�е��ַ���,���ܳ���,����������

            SetCaretPos(position.LineNo, position.ColNo);  //��ԭCaret

            lastLineNumber = GetCaretLineNumber();                     //��ǰ�б�Ϊ��һ��  
            lastTokenType = CurrentToken;
        }

        /// <summary>
        ///  �ı����ݺ͹���ƶ�,��������ʾ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PLCCodeEditor_SelChange(object sender, ICodeSenseEvents_SelChangeEvent e)
        {
            //this.CreateGraphics().FillEllipse(Brushes.Red, new Rectangle(0, 0, 10, 10));


            //ѡ����кŲ���:������ʽ��            

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
            if (formatedText == noFormatedText)        //����û���и�ʽ��
            {
                lastLineNumber = GetCaretLineNumber();
                lastTokenType = CurrentToken;
                return;
            }

            Position position = GetCaretPosition();                            //�ȱ���ʵ�ʵ�Caret,һɾһ��Caret����,TokenҲ���  

            int length = (formatedText.Length > noFormatedText.Length) ? formatedText.Length : noFormatedText.Length;
            ReplaceText(formatedText, GetLineRange(lastLineNumber,
                GetLineLength(lastLineNumber)));       //��ȡ��ǰ�е��ַ���,���ܳ���,����������

            SetCaretPos(position.LineNo, position.ColNo);  //��ԭCaret

            lastLineNumber = GetCaretLineNumber();                     //��ǰ�б�Ϊ��һ��  
            lastTokenType = CurrentToken;
        }

        /// <summary>
        /// ��ù��ǰ���ַ���
        /// ��������Ҽ����������Ϊ-1��1
        /// �����ϲ������ӷ������������ǰ���ַ���
        /// </summary>
        /// <param name="LeftOrRight"></param>
        /// <param name="UpOrDown"></param>
        /// <returns></returns>
        private string GetCaretString(int LeftOrRight, int UpOrDown)
        {//��Ҫ�������Ҽ���� ��λ�ý������ƫ��
            int lineNum = GetCaretLineNumber() + UpOrDown;

            string input = GetLine(lineNum);
            input = RemoveMarkPart(input, IsPureMark(input));
            Range range = GetSel(false);
            if (input.Length >= range.EndColNo && range.EndColNo + LeftOrRight >= 0 && range.EndColNo + LeftOrRight < input.Length)
                input = input.Substring(0, range.EndColNo + LeftOrRight);

            while (lineNum >= 1)
            {//������һ��
                string line = GetLine(--lineNum);//��һ����Ϣ
                line = RemoveMarkPart(line.Trim(), IsPureMark(line));//ȥ�����е�ע�ͺͿո�
                if (line.EndsWith(@"\"))
                {//����һ�������ӷ���β��ȥ�����ӷ���ӵ���ǰ���ַ���ǰ
                    input = line.TrimEnd('\\') + input.Trim(); ;
                }
                else//���������ӷ����ж�
                { break; }
            }
            return input;
        }

        /// <summary>
        /// ���������ҵ���Ӧ����
        /// </summary>
        /// <param name="LeftOrRight"></param>
        /// <param name="UpOrDown"></param>
        /// <returns></returns>
        private string GetCaretString(Point MousePoint)
        {//��Ҫ�������Ҽ���� ��λ�ý������ƫ��
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
            {//������һ��
                string line = GetLine(--lineNum);//��һ����Ϣ
                line = RemoveMarkPart(line.Trim(), IsPureMark(line));//ȥ�����е�ע�ͺͿո�
                if (line.EndsWith(@"\"))
                {//����һ�������ӷ���β��ȥ�����ӷ���ӵ���ǰ���ַ���ǰ
                    input = line.TrimEnd('\\') + input.Trim(); ;
                }
                else//���������ӷ����ж�
                { break; }
            }
            return input;
        }

        /// <summary>
        ///  ��ʾ������
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="UpOrDown">��ƫ��</param>
        private void SetTipFloating(Position ps, int UpOrDown)
        {
            RectClass rc = (RectClass)this.PosFromChar(ps);
           if (this.commandTipSet.Items.Count > 0)
            {
                // ������½ǵ�����
                Point pt = new Point(rc.left, rc.bottom + rowHeight * UpOrDown);
                this.commandTipSet.Location = pt;
                this.commandTipSet.SelectedIndex = 0;// �����б����ѡ���һ��
                this.commandTipSet.Show();;
            }
            else
            {
                Point pt = new Point(rc.left - this.commandTipSet.Size.Width, rc.bottom + rowHeight * UpOrDown);
                this.commandTipSet.Location = pt;
                this.commandTipSet.Hide();
            }
        }

        // ȥ������ǰ��Ŀո�
        private string RemoveEmptyCom(string text)
        {
            // ���ź��пո�

            if (text.Contains(", "))
            {
                text = text.Replace(", ", ",");
                text = RemoveEmptyCom(text);
            }
            // ����ǰ�пո�
            if (text.Contains(" ,"))
            {
                text = text.Replace(" ,", ",");
                text = RemoveEmptyCom(text);
            }
            return text;
        }
        // ����е�����
        private cmTokenType GetLineType(int lineNo, Position caretPos)
        {
            cmTokenType cm;
            SetCaretPos(lineNo, 2);        //���ù��,�������
            cm = CurrentToken;
            SetCaretPos(caretPos.LineNo, caretPos.ColNo);//��ԭ���
            return cm;

        }

        /// <summary>
        /// ����е�����,����������|����ʼ��|����ע��(��������һ��),Ҳ�������κδ���,����ΪMulti,����Ϊunknow
        /// </summary>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        private cmTokenType GetLineType(int lineNo)
        {
            string text = GetLine(lineNo).Trim();
            if (text.StartsWith(@"(*") || text.StartsWith(@"//"))   // ��������|����ʼ��
                return cmTokenType.cmTokenTypeMultiLineComment;

            if (text.Contains("(*") || text.Contains(@"//"))    // ���Ǵ�ע��
                return cmTokenType.cmTokenTypeUnknown;

            if (FindRealRemark(lineNo))          // ����ע��(��������һ��)
                return cmTokenType.cmTokenTypeMultiLineComment;
            else
                return cmTokenType.cmTokenTypeUnknown;

        }

        /// <summary>
        /// ����ע�ͱ�־
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
                if (isEndMark == true && isHeadMark == true) // ��һ���ǵ���ע��
                {
                    isHeadMark = false;
                    isEndMark = false;
                }
                if (isEndMark == true)
                    isEndMark = false;
            }
        }

        /// <summary>
        /// ��ע����ʼ��,�ҵ�����True,���򷵻�False
        /// </summary>
        /// <param name="lineNo"></param>
        /// <returns></returns> 
        private bool FindRealRemark(int lineNo)
        {
            if (lineNo == 0)        // ��һ��
                return false;

            string rangeText = string.Empty;    // ���ҵķ�Χ           
            for (int i = 0; i < lineNo; i++)
            {
                rangeText += GetLine(i).Trim();
            }
            // �Ӻ���ǰ��
            for (int i = rangeText.Length - 1; i > 0; i--)
            {
                if (rangeText[i] == '*')        // ������һ����ʼƥ��:*
                {
                    if (rangeText[i - 1] == '(')  // �����ڶ�����ʼƥ��:(,����True
                        return true;
                }

                if (rangeText[i] == ')')        // ������һ������ƥ��:)
                {
                    if (rangeText[i - 1] == '*')  // �����ڶ�������ƥ��:*,����false
                        return false;
                }
            }
            // �Ҳ�������False
            return false;
        }
        // �ҵ�������
        private int GetCodeLine(int lineNo)
        {
            string text = GetLine(lineNo).Trim();
            // ����ע�͵ݹ鵽�д����� ֻ�б�� ����
            while (GetLineType(lineNo) == cmTokenType.cmTokenTypeMultiLineComment || text == "")
            {
                lineNo--;
                text = GetLine(lineNo).Trim();
            }
            return lineNo;
        }
        // ��һ���Ƿ������ӷ�
        public bool isContinueLine(int preLineNo)
        {
            preLineNo = GetCodeLine(preLineNo);
            bool isContinue = false;
            string text = GetLine(preLineNo).Trim();
            // ���ڵ���ע��
            if (text.Contains("(*") && text.Contains("*)"))      // ����ע��
            {
                text = text.Substring(0, text.IndexOf("(*"));
            }

            if (text.Contains("//"))                            // ����ע��
            {
                text = text.Substring(0, text.IndexOf("//"));
            }
            if (text.Contains("(*") && !text.StartsWith("(*")) // ע�͵ĵ�һ�е�����ֻ��ע��
            {
                text = text.Substring(0, text.IndexOf("(*"));
            }

            text = text.Trim();
            if (text.EndsWith("\\"))
                isContinue = true;
            return isContinue;

        }

        /// <summary>
        /// �������н��и�ʽ������ʾ
        /// </summary>
        /// <param name="oper">������</param>
        /// <param name="para">������</param>
        /// <param name="index">�����</param>
        public void FormatInsert(string oper, string para, ref int index)
        {
            if (para != null && para.Length > paraLength)
            {//�������ȴ������Ƴ���ʱ
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
        ///  ��ʽ����,isContinue�Ƿ���������
        /// </summary>
        /// <param name="lineNo">�к�</param>
        /// <param name="isEdit"></param>
        /// <param name="getError">�Ƿ��ȡ��ʽ�������Ϣ</param>
        /// <returns></returns>
        public string FormatRow(int lineNo, bool isEdit, bool getError)
        {
            string tempRow = GetLine(lineNo);  // ���Է�����ʱ�����������и�ʽ��         
            string text = tempRow.Trim();
            if (text.Length == 0)
                return tempRow;
            //cmTokenType lastTokenType = GetLineType(lineNo);    // ����е�����
            SetLineType(text);                                  // ����ע�ͱ�־
            // ע�͵����������
            if (text.Contains("*)"))
            {
                int index = text.IndexOf("*)") + 2;
                if (text.Substring(index).Trim().Length != 0)
                {
                    if (getError)
                    {
                        ErrorList.Add(new string[] { lineNo.ToString(), "��ʽ����:ע�ͺ�������", "error" });
                    }
                    return tempRow;
                }
            }

            int totalLength = markLength + operatorLength + paraLength;

            // ��ʽ���ϵĲ����и�ʽ��
            char[] cs = new char[] { ' ' };

            text = text.Replace("\t", " ");    // ɾ��������           

            string[] texts;
            string remarkText = ""; // ע������
            string markText = "";   // ������� 

            if (isEdit == true)
            {
                if ((lastTokenType == cmTokenType.cmTokenTypeMultiLineComment && !text.Contains("(*")) ||
                    text.StartsWith("(*") || text.StartsWith(@"//"))        // ��ע��
                {
                    text = text.TrimStart(' ');
                    text = text.TrimEnd(' ');
                    text = text.PadLeft(totalLength + text.Length, ' ');
                    return text;
                }
                if (text.Contains("(*") && !text.StartsWith("(*")) // ע�͵ĵ�һ�е�����ֻ��ע��
                {
                    remarkText = text.Substring(text.IndexOf("(*"));
                    text = text.Substring(0, text.IndexOf("(*"));
                }

                if (text.Contains("(*") && text.Contains("*)"))      // ����ע��
                {
                    remarkText = text.Substring(text.IndexOf("(*"));
                    text = text.Substring(0, text.IndexOf("(*"));

                }

                if (text.Contains("//"))                            // ����ע��
                {
                    remarkText = text.Substring(text.IndexOf("//"));
                    text = text.Substring(0, text.IndexOf("//"));
                }
            }// Edit״̬
            else
            {
                if (isHeadMark == true)
                {
                    if ((text.Contains("(*") && !text.StartsWith("(*")))
                    {
                        remarkText = text.Substring(text.IndexOf("(*"));
                        text = text.Substring(0, text.IndexOf("(*"));
                    }// ע�͵ĵ�һ�е�����ֻ��ע��
                    else
                    {
                        text = text.TrimStart(' ');
                        text = text.TrimEnd(' ');
                        text = text.PadLeft(totalLength + text.Length, ' ');
                        return text;
                    }// ��ע��
                }
                else
                {
                    if (isEndMark == true || text.StartsWith(@"//"))
                    {
                        text = text.TrimStart(' ');
                        text = text.TrimEnd(' ');
                        text = text.PadLeft(totalLength + text.Length, ' ');
                        return text;
                    }// False�еĽ�β

                    if (text.Contains(@"//") && !text.StartsWith(@"//"))                           // ����ע��
                    {
                        remarkText = text.Substring(text.IndexOf("//"));
                        text = text.Substring(0, text.IndexOf("//"));
                    }
                }
            }// Scan״̬

            bool isContinue = false;
            if (lineNo > 0)
                isContinue = isContinueLine(lineNo - 1);
            if (!isContinue)
            {// ����������
                // �ڶ����б�������ţ�������޿ո�
                if ((!text.Contains("File")) && (!text.Contains("FILE")) && text.Contains(":"))          // �����һ����":"
                {
                    markText = text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    text = text.Remove(0, markText.Length + 1); // ���ϱ��
                    // ɾ������еĿո�
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
                        ErrorList.Add(new string[] { lineNo.ToString(), "��ʽ����:���̫��", "error" });
                    }
                    text = tempRow;
                    return text;
                }
                markText = markText.PadRight(markLength, ' ');  // ��ʽ����� 
                markText = markText.ToUpper();      // ��Ž��д�д
                // ȥ���űߵĿո�
                text = RemoveEmptyCom(text);
                // �����ţ�ע�ͣ����űߵĿո���ж����Ƿ��ʽ��ȷ            
                text = text.ToUpper();  // �����룬������д
                texts = text.Split(cs, StringSplitOptions.RemoveEmptyEntries);
                // �ж���ɷ�
                if (texts.Length > 2)
                {
                    if (getError)
                    {
                        ErrorList.Add(new string[] { lineNo.ToString(), "��ʽ����:����������", "error" });
                    }
                    text = tempRow;
                    return text;
                }

                if (texts.Length != 0)   // ��ָ��
                {
                    if (texts.Length == 1)        // ֻ�в�����
                    {
                        if (texts[0].Length > operatorLength)
                        {
                            if (getError)
                            {
                                ErrorList.Add(new string[] { lineNo.ToString(), "��ʽ����:������̫��", "error" });
                            }
                            text = tempRow;
                            return text;
                        }
                        texts[0] = texts[0].PadRight(operatorLength + paraLength, ' ');
                        text = markText + texts[0];
                    }
                    else            // �������ݲ��Ҳ���ע����ɾ��
                    {
                        if (texts[0].Length > operatorLength)
                        {
                            if (getError)
                            {
                                ErrorList.Add(new string[] { lineNo.ToString(), "��ʽ����:������̫��", "error" });
                            }
                            text = tempRow;
                            return text;
                        }
                        if (texts[1].Length > paraLength)
                        {
                            //AddWrongSentence("��ʽ����:������̫��", lineNo + 1, true);
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
                        texts[1] = texts[1].PadRight(paraLength, ' ');    // ����
                        texts[0] = texts[0].PadRight(operatorLength, ' ');    // ������
                        text = markText + texts[0] + texts[1];
                    }

                    if (remarkText != "")    // ��ʾͷ��ע��            
                        text += remarkText;
                }
                else   // ��ָ��
                {
                    markText = markText.PadRight(totalLength, ' ');
                    text = markText + remarkText;
                }
            }
            else
            {// ������
                // ȥ���űߵĿո�
                text = RemoveEmptyCom(text);
                // �����ţ�ע�ͣ����űߵĿո���ж����Ƿ��ʽ��ȷ            
                text = text.ToUpper();  // �����룬������д
                texts = text.Split(cs, StringSplitOptions.RemoveEmptyEntries);
                if (texts.Length > 1)
                {// ��ֹ�ǲ���
                    //AddWrongSentence("��ʽ����:����������", lineNo + 1, true);
                    text = tempRow;
                    return text;
                }
                if (texts[0].Length > paraLength)
                {
                    //AddWrongSentence("��ʽ����:������̫��", lineNo + 1, true);
                    text = tempRow;
                    return text;
                }
                if (texts.Length == 1)
                {
                    markText = markText.PadRight(markLength + operatorLength, ' ');
                    texts[0] = texts[0].PadRight(paraLength, ' ');// ����
                    text = markText + texts[0] + remarkText;
                    if (texts[0].EndsWith("\\"))
                        isContinue = true;
                    else
                        isContinue = false;
                }

            }

            return text;

        }
        // �Ƿ�ı��
        bool isDirty = false;

        /// <summary>
        /// �Ƿ��޸Ĺ�
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

        // ����һ�н��и�����ʾ
        public void ShowHighLightRow(int lineNo)
        {
            if (lineNo < this.LineCount && lineNo != -1)
            {
                HighlightedLine = lineNo;
                SetCaretPos(lineNo, 0);
            }
        }

        // ����ϵ�
        public void AddBreakPoint(int lineNo)
        {
            if (!alBreakPoints.Contains(lineNo))
                alBreakPoints.Add(lineNo);
        }

        // �Ƴ��ϵ�
        public void RemoveBreakPoint(int lineNo)
        {
            if (alBreakPoints.Contains(lineNo))
                alBreakPoints.Remove(lineNo);
        }

        // �������е��к�
        public void SetRunLine(int lineNo)
        {
            iRunLineNo = lineNo;
        }

        #region �����滻,��ȡ�����롢��������

        /// <summary>
        /// �����Ҫ�Ĵ����ʽ
        /// </summary>
        public void GetCode()
        {
            HighlightedLine = -1;    // ����ȡ��

            //List<string> marks = new List<string>();      // ���
            //List<string> editingOperators = new List<string>();        // ��ǰ�༭�Ĳ�����
            //List<List<string>> parameters = new List<List<string>>();     // �������ϣ��ؼ���Ϊ������   
            //ArrayList usingLineNum = new ArrayList(); // ʹ�õ��к�
            //ArrayList replaceLineNum = new ArrayList();   // �滻����к�
            string allFormatedText = null;
            CodeInfo.Clear();
            // ���������,���������ͼ����
            isHeadMark = false;
            bool isContinue = false;
            //string allFormatedText = "";
            for (int lineNo = 0; lineNo < LineCount; lineNo++)
            {
                string formatedText = FormatRow(lineNo, false, false);     // �ȸ�ʽ��ÿһ��,����Ĳ���жϱ����ڸ�ʽ��������½���
                if (lineNo != LineCount - 1)
                    allFormatedText += formatedText + "\n";
                else
                    allFormatedText += formatedText;

             

                ArrayList oppamk = TakeOperatorAndPara(formatedText, lineNo, ref isContinue);  // ��������� 
                if (oppamk.Count != 0)
                {
                    CodeInfo.Add(oppamk, lineNo);
                    MaxLine = lineNo;//���ڼ�¼���һ���к�
                }
            }

            //��ʽ����� ����
            //Text = allFormatedText;     

            // �������滻
            //CompositionReplace(ref parameters, ref editingOperator, ref marks, ref usingLineNum, ref replaceLineNum);

            //code.editingOperator = editingOperators;
            //code.parameters = parameters;
            //code.marks = marks;
            //code.lineCount = LineCount;
            //code.replaceLineNum = replaceLineNum;

        }

        /// <summary>
        /// �������Ϊ����������,�������Ͳ���
        /// </summary>
        /// <param name="para"></param>
        /// <param name="num"></param>
        /// <returns></returns> 
        private string DepartPara(string para, ref string num)
        {
            string type = "";

            if (para[0] == 'H') // ʮ������
            {
                return "H";
            }
            else
            {
                foreach (char c in para)
                {
                    // ��������
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
        /// ��ȥע�Ͳ���
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isPureMark"></param>
        /// <returns></returns> 
        private string RemoveMarkPart(string text, bool isPureMark)
        {
            if (isPureMark == false)
            {
                if (text.Contains("(*") && !text.Contains(@"//"))  // ֻ�ж���ע�ͱ��
                {
                    int markIndex = text.IndexOf("(*");
                    text = text.Substring(0, markIndex);
                }
                else if (!text.Contains("(*") && text.Contains(@"//"))  // ֻ�е���ע�ͱ��
                {
                    int markIndex = text.IndexOf(@"//");
                    text = text.Substring(0, markIndex);
                }
                else if (text.Contains("(*") && text.Contains(@"//"))  // ���߶���
                {
                    int markIndex = text.IndexOf("(*") < text.IndexOf(@"//") ? text.IndexOf("(*") : text.IndexOf(@"//");
                    text = text.Substring(0, markIndex);
                }
            }
            else
            {
                return "";      // ��ע�ͷ��ؿ�
            }
            return text;
        }

        /// <summary>
        /// �жϵ�ǰ���Ƿ��Ǵ�ע��
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns> 
        private bool IsPureMark(string text)
        {
            if ((isHeadMark == true &&              //ͷע��
                 (text.StartsWith("(*") ||
                 text.StartsWith(@"//") ||
                 (!text.Contains("(*") &&
                 !text.Contains(@"//")))) ||

                 isEndMark == true &&                //βע��
                 (!text.Contains("(*") ||
                 text.StartsWith("(*") ||
                 text.IndexOf("*)") < text.IndexOf("(*"))    //βע�ͺ󻹸���һ��ͷע��
                )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �����ϲ�,���ӷ�ɾȥ,�����Ƿ�������ӷ� textΪ�Ƴ�ע�͵�����
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
            // ����ע����������
            endTemp = isEndMark;
            headTemp = isHeadMark;
            while (isContinue)
            {// ѭ���������ӷ�                
                text = text.Trim().TrimEnd('\\');

                connectText = GetLine(++lineNo);
                connectText = connectText.Trim();
                SetLineType(connectText); // ����������
                isPureMark = IsPureMark(connectText); // �ж��Ƿ��Ǵ�ע��
                connectText = RemoveMarkPart(connectText, isPureMark);        // �Ƴ�ע�Ͳ���
                while (connectText == "")
                {// �ҵ��ǿ���
                    if (lineNo == LineCount)
                        break;
                    connectText = GetLine(++lineNo);
                    connectText = connectText.Trim();
                    SetLineType(connectText); // ����������
                    isPureMark = IsPureMark(connectText); // �ж��Ƿ��Ǵ�ע��
                    connectText = RemoveMarkPart(connectText, isPureMark);        // �Ƴ�ע�Ͳ���
                }

                text += connectText.Trim().ToUpper().TrimEnd('\\');
                isContinue = connectText.Trim().EndsWith(@"\");

            }
            // ��ԭ
            isEndMark = endTemp;
            isHeadMark = headTemp;
        }


        /// <summary>
        /// ������������������,�ڿ���ע�ͺͱ�ŵ������ isContinue���ж��Ƿ������ӷ�����
        /// </summary>
        /// <param name="Linetext">��ǰָ����</param>
        /// <param name="LineNo">�к�</param>
        /// <param name="isContinue">��ǰ���Ƿ�Ϊ������</param>
        /// <returns>���ص�ǰ�е���������Ϣ��Ϻ�����</returns>
        private ArrayList TakeOperatorAndPara(string Linetext, int LineNo, ref bool isContinue)
        {
            ArrayList OpPaMk = new ArrayList();//��ǰ�в����������������
            Linetext = RemoveMarkPart(Linetext.Trim(), IsPureMark(Linetext));    // �Ƴ��ո��ע�Ͳ���
            bool isNextContinue = Linetext.Trim().EndsWith(@"\"); // �����Ƿ�Ҫ����

            string curMark = "";
            string curOperator = "";
            List<string> curParas = new List<string>(); // ��ǰ��������Ӧ�Ĳ�������

            if (!isContinue && Linetext.Trim().Length > 0)
            {//���������кͿ���
                MergeParameters(ref Linetext, LineNo);  // ���ӷ�����
                string[] rowInfo = Linetext.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);    // ������ע��  

                if (!rowInfo[0].Contains(":"))    // ���������
                {// һ���ǲ�����                    
                    curOperator = rowInfo[0];
                    if (rowInfo.Length >= 2)   // ������������
                    {
                        if (rowInfo[1].Contains(",")) // �ڶ���λ�ò���ע��, �������ϵĲ���ʱ
                        {
                            SplitPara(rowInfo[1], ref curParas);
                        }
                        else   // �ڶ���λ�ò���ע��,��ֻ��һ������
                        {
                            curParas.Add(rowInfo[1]);
                        }
                    }
                }   // if ���������
                else    // �������
                {
                    //// ��Ų����ظ�
                    //if (marks.Contains(rowInfo[0].TrimEnd(':')))
                    //{
                    //    ErrorList.Add(new string[] { (LineNo + 1).ToString(), "��ų����ظ�", "error" });
                    //}
                    //if (rowInfo[0].Length > markLength)
                    //{
                    //    ErrorList.Add(new string[] { (LineNo + 1).ToString(), "��Ź���", "error" });
                    //}

                    curMark = rowInfo[0].TrimEnd(':');

                    if (rowInfo.Length >= 2)
                    {// ��ֻ������� 
                        curOperator = rowInfo[1];
                        if (rowInfo.Length >= 3)   // ����������
                        {
                            if (rowInfo[2].Contains(",")) // �������ϵĲ���ʱ
                            {
                                SplitPara(rowInfo[2], ref curParas);
                            }
                            else    // һ������   
                            {
                                curParas.Add(rowInfo[2]);
                            }
                        }
                    }
                }
            }// ��Ϊ��

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
            if (Linetext.Length > 0)// ֻ�е�ǰ�в���ע��,���ܸ�ֵ
                isContinue = isNextContinue;
            return OpPaMk;
        }

        /// <summary>
        /// �Ӳ����ַ�������ȡ�����������������Ϊһ����λ
        /// </summary>
        /// <param name="paraInfo">�����ַ���</param>
        /// <param name="curParas">��������</param>
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


        // ��ӳ������,���кŵ�,��������ʾ��
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
        // ��ӳ������,���кŵ�
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
        // ��ӳ������,���кŵ�Ϊ-1
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

        // ����Ͻ����滻,��󻻵Ļ�Ч��̫����
        //private void CompositionReplace(ref ArrayList parameters, ref ArrayList editingOperator, ref ArrayList marks,
        //    ref ArrayList usingLineNum, ref ArrayList replaceLineNum)
        //{
        //    ArrayList alOperator = (ArrayList)editingOperator.Clone();
        //    int insertIndex = 0;
        //    foreach (string eachOperator in alOperator)
        //    {
        //        foreach (FunctionComposition.CompostionStruct cs in compositions)
        //        {
        //            // ������Ϲؼ���
        //            if (cs.commandKey == eachOperator)
        //            {
        //                // �����滻
        //                ArrayList repParaList = ParameterReplace((ArrayList)parameters[insertIndex], cs.parametersModule, cs.parameterSite);
        //                // ���������
        //                editingOperator.RemoveAt(insertIndex);
        //                editingOperator.InsertRange(insertIndex, cs.operators);
        //                // ���������
        //                parameters.RemoveAt(insertIndex);
        //                parameters.InsertRange(insertIndex, repParaList);
        //                // ������
        //                marks.RemoveAt(insertIndex);
        //                marks.InsertRange(insertIndex, cs.marks);

        //                // �����кŶ�Ӧ��ϵ
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
        /// ����в��������滻
        /// </summary>
        /// <param name="paraList">�����б�</param>
        /// <param name="repParaList">Ҫ���滻�Ĳ���</param>
        /// <param name="siteList">λ����Ϣ</param>
        /// <returns></returns>
        private ArrayList ParameterReplace(ArrayList paraList, ArrayList repParaList, ArrayList siteList)
        {
            int paraIndex = 0;
            // ÿ������
            for (int sentence = 0; sentence < repParaList.Count; sentence++)
            {
                ArrayList al = (ArrayList)repParaList[sentence];
                for (int site = 0; site < al.Count; site++)
                {
                    // �ղ�������,�������滻
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

        #region ��Ϣ����
        /// <summary>
        /// Ԥ������̻س����¼�����ʾ�б���
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool PreProcessMessage(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_KEYDOWN:
                    //Tiptimer.Enabled = false;//��ʾ��ʱ��Ƭͣת
                    switch ((int)msg.WParam)
                    {
                        case VK_UP://�ϼ�
                            if (this.commandTipSet.SelectedIndex > 0)
                            {
                                this.commandTipSet.SelectedIndex--;
                                return true;

                            }
                            else if (this.commandTipSet.SelectedIndex ==
                                0)
                            {
                                return true;  // ��һ�����
                            }

                            SetTipFloating(GetWordPosition(), -1);
                            this.commandTipSet.ShowTipText(GetCaretString(0, -1));
                            break;
                        case VK_DOWN://�¼�
                            if (this.commandTipSet.SelectedIndex <
                                this.commandTipSet.Items.Count - 1)
                            {
                                this.commandTipSet.SelectedIndex++;
                                return true;  // ǰ�漸������
                            }
                            else if (this.commandTipSet.SelectedIndex ==
                                this.commandTipSet.Items.Count - 1 && this.commandTipSet.SelectedIndex != -1)
                            {
                                return true;  // ���һ�����
                            }
                            SetTipFloating(GetWordPosition(), 1);
                            this.commandTipSet.ShowTipText(GetCaretString(0, 1));
                            break;
                        case VK_ENTER://�س�
                            if (this.commandTipSet.SelectedIndex > -1)
                            {
                                AutoCode();
                                return true;
                            }
                            this.currentTipList.Clear();//�س�ʱ�����ʾ�б�
                            commandTipSet.HideALL();
                            break;
                        case VK_SPACE://�ո�
                            if (this.commandTipSet.SelectedIndex > -1)
                            {
                                AutoCode();
                                return true;
                            }
                            //AutoAttribute();
                            //this.commandTipSet.HideALL();
                            break;
                        case VK_LEFT://���
                            //this.commandTipSet.SetTipList(GetCaretString(-1,0), this.CurrentWord);
                            SetTipFloating(GetWordPosition(), 0);
                            this.commandTipSet.ShowTipText(GetCaretString(-1, 0));
                            break;
                        case VK_RIGHT://�Ҽ�
                            //this.commandTipSet.SetTipList(GetCaretString(1,0), this.CurrentWord);
                            SetTipFloating(GetWordPosition(), 0);
                            this.commandTipSet.ShowTipText(GetCaretString(1, 0));
                            break;
                        case VK_ESC://ȡ��
                            this.commandTipSet.HideALL();
                            return true;
                        // ɾ����������Ԥ����
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
        /// ������Ϣ,�����������̵�Ԥ������Ϣ(Del enter ��������)
        /// </summary>
        /// <param name="m"></param> 
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CHAR:
                    base.WndProc(ref m); // ����      
                    //Tiptimer.Enabled = false;//ʱ��Ƭͣת
                    string curRow = GetCaretString(0, 0);

                    if (curRow.Contains(":"))
                    { //ȡ���
                        curRow = curRow.Split(':')[1];
                    }
                    string[] rowInfo = curRow.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (rowInfo.Length != 0)
                    {//�ǿ� �Ҳ����ǲ���ָ��ս��� Ҳ���ǰ���ռ�
                        AutoAttribute(rowInfo);
                    }
                    SetTipFloating(GetWordPosition(), 0);  // ��ʾ
                    return;
                case WM_LBUTTONDOWN://������  // ��ϵ�
                    base.WndProc(ref m);//�����ι�����Ч
                    this.commandTipSet.HideALL();
                    //SetTipFloating(GetWordPosition());
                    //this.commandTipSetting1.ShowTipText(GetCaretString(0));
                    ///20090709�������ʾ��ʾ

                    //if (this.commandTipSetting1 != null)
                    //{
                    //    this.commandTipSetting1.Items.Clear();
                    //    this.commandTipSetting1.Hide();
                    //}
                    //int x = (int)m.LParam & (0xFFFF);
                    //int y = ((int)m.LParam >> 16);
                    //PositionClass maxPc = new PositionClass();
                    //maxPc.LineNo = this.LineCount - 1; // ����к�
                    //maxPc.ColNo = 0;
                    //RectClass maxRc = (RectClass)PosFromChar(maxPc); // ����кŵ�λ��
                    //if (x < 25 && y <           Rc.bottom)
                    //{// ������Ч�߾ദ
                    //    //if(slBreakPoints.ContainsKey(
                    //    PositionClass ps = (PositionClass)GetSelFromPoint(x + 25, y);  // ����߾��ǻ�ò���λ��,������������
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
                    //                    //CassMessageBox.CassMessageBox.Information("��λ��δ�ܲ���ϵ㣡");
                    //                }
                    //            }
                    //            else
                    //            {
                    //                //CassMessageBox.CassMessageBox.Information("���ȱ������½��ϵ㣡");
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
                    //{//ʱ��Ƭ�Ѿ�������������
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
        /// ʱ��Ƭ������ʾ����µ�����
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
        /// �����ʾ�б���Ϣ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void commandTipSet_MouseClick(object sender, EventArgs e)
        { 
            AutoCode();
        }

        public List<string> currentTipList = new List<string>();//��ǰ��ص�������ʾ��Ϣ

        /// <summary>
        /// �ӵ�ǰ��ص�������ʾ��Ϣ��ѡ��
        /// �Թؼ��ֿ�ͷ����ʾ��Ϣ������ʾ�б���
        /// </summary>
        /// <param name="Keyword"></param>
        public void selectTip(string Keyword)
        {
            this.commandTipSet.Items.Clear();
            foreach (string tip in currentTipList)
            {
                if (Keyword == null || tip.ToUpper().StartsWith(Keyword.ToUpper()))
                {//��ת���ɴ�д�Ž���ƥ��
                    this.commandTipSet.Items.Add(tip);
                }
            }
        }

        /// <summary>
        /// �Զ���ɴ���
        /// </summary> 
        private void AutoCode()
        {
            Range range = GetSel(true);

            // ���ź͵㲻���滻
            if (CurrentWord == "," || CurrentWord == ".")
                range.StartColNo = range.EndColNo - CurrentWordLength + 1;
            else
                range.StartColNo = range.EndColNo - CurrentWordLength;

            range.EndColNo = range.StartColNo + this.commandTipSet.SelectedItem.ToString().Length;
            ReplaceText(this.commandTipSet.SelectedItem.ToString(), range);
            SetCaretPos(range.StartLineNo, range.EndColNo);

            // �س�ʱ��ʾ����ʧ
            this.commandTipSet.Items.Clear();
            this.commandTipSet.Hide();
        }

        /// <summary>
        /// ������Ӧ��������ʾ
        /// </summary>
        /// <param name="rowInfo">ָ����Ϣ����</param>
        private void AutoAttribute(string[] rowInfo)
        {
            List<string> ShowtipList = new List<string>();//���뵽��ʾ�б��е��б�
            List<ArrayList> tempCode = new List<ArrayList>();//ָ����Ϣ
            List<string> PntipList = new List<string>();//�����б�
            List<string> Marklist = new List<string>();//��ʾ���б�              
            List<string> LDtiplist = new List<string>();//LD��ʾ�б�
            string Keyword = null;//�û�����Ĺؼ���
            int curIndex = GetCaretLineIndex(GetCaretLineNumber()); //�����а���ˢ��ָ���б�            
            AutoGetInfo(ref tempCode, ref PntipList, ref Marklist, ref LDtiplist);

            if (rowInfo.Length == 1)
            {
                Keyword = rowInfo[0].Trim().ToUpper();
                if (this.CurrentWord == ""
                    //||this.CurrentWord.Length!=1
                    )
                {//����ո� //����һ���ж��������� ��JMP  MAIN1����MAIN1ǰ���ո�ʱ���ֵ���� ԭ����
                    if (PLCCodeEditor.CtrlPropertys.ContainsKey(Keyword))
                    {//�Ҳ���������ָ����ȷ
                        int Codeindex = CanTipCode.IndexOf(rowInfo[0].ToUpper());//��ʾ��ָ��
                        if (Codeindex >= 0)
                        {//ST LD JMP CALL �ҵ���ص���ʾ��Ϣ
                            if (Codeindex == 0)
                            {//��ʾ�б�Ϊ�����Ϳ�LD��ʱ����
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
                            { //��ʾ�б�Ϊ�����Ϳ�ST��ʱ����                               
                                foreach (string tip in PntipList)
                                {
                                    ShowtipList.Add(tip);
                                }
                                for (int j = curIndex - 1; j >= 0; j--)
                                {//�ӵ�ǰ��ŵ���һ�п�ʼ���ϱ���
                                    if (CtrlPropertys.ContainsKey((string)tempCode[j][0]))
                                    {
                                        ControlInfo codeCtrl = CtrlPropertys[((string)tempCode[j][0])];

                                        if (codeCtrl.OutputInfo.Count != 0 || tempCode[j].Count == 3)
                                        {//������ؼ����������ǰ��ͷ(�б�ʾ����Ϊ3)
                                            if (codeCtrl.OutputInfo.Count != 1 && codeCtrl.OutputInfo.Count >= curIndex - j && ((List<string>)tempCode[j][1]).Count != 0)
                                            {//������ؼ� ���������ʱ������������Χ֮��
                                                //������ؼ���ʱ����������ʹ�ö�����ؼ�ָ�������ST��Ӧ�˿ڵ���ʱ��������ʾҲ���
                                                //�Ҹ�����ؼ�ָ��Ĳ������벻Ϊ0
                                                for (int x = 0; x < codeCtrl.OutputInfo.Count; x++)
                                                {//��ʾ�б��м���ؼ�����_�˿ں�
                                                    string tempValue = codeCtrl.CodeInfo[1] + ((List<string>)tempCode[j][1])[0] + "_" + x.ToString();
                                                    if (!LDtiplist.Contains(tempValue))
                                                    {//����ʱ����û��ST��
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
                                string curMark = null;//�ؼ����ڵı�ʾ����
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
                                    int IsError = tip.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });//��ʾ���Ƿ�������
                                    int curLevelNum = curMark.Split('_').Length;
                                    int tipLevelNum = tip.Split('_').Length;
                                    if ((Codeindex == 2 && curLevelNum == tipLevelNum && tip != curMark  && tip.StartsWith(cutmark)) //��ת�ų�����
                                        || (Codeindex == 3 && IsError == -1 && tipLevelNum == curLevelNum + 1 && (tip.StartsWith(cutmark) || curLevelNum == 1)))//����Ŀ�겻�����ֲ��� ��Ϊ��һ����ҳ��
                                    {//��ָͬ����ʾ��Χ��ͬ
                                        ShowtipList.Add(tip);//��Ծ��ʾ������ʾ�б�
                                    }
                                }
                            }
                            currentTipList = ShowtipList;
                            selectTip(null);
                        }
                        else
                        {//��ָͨ�� ��ʾ������ʾ��Ϣ
                            //string ShowTip = String.Join(",", commandTipSet.GetTipmessage(PLCCodeEditor.CtrlPropertys[Keyword], 0).ToArray());
                            //commandTipSet.toolTip1.Show(ShowTip, commandTipSet, new Point(commandTipSet.Size.Width, 0));
                            commandTipSet.GetTipmessage(PLCCodeEditor.CtrlPropertys[Keyword], 0);
                        }
                    }
                }
                else
                {//�������������
                    currentTipList.Clear();
                    currentTipList.AddRange(PLCCodeEditor.SunningCodes.Keys);
                    currentTipList.AddRange(PLCCodeEditor.SpecialCode.Keys);
                    //����ѡ������ؼ��ֵ�ָ��
                    selectTip(Keyword);
                }
            }
            else if (rowInfo.Length == 2 && CurrentWord != "")
            {
                string[] tempValue = rowInfo[1].Split('.');
                if (tempValue.Length < 3)
                {
                    if (rowInfo[1].EndsWith("."))
                    {//�����������ĵ�
                        Keyword = null;
                        string PN = tempValue[0];
                        string CtrlName = null;
                        if (PntipList.Contains(PN))
                        {//���ڶ�Ӧ�ĵ�����Ѱ����ɼ�����
                            int hitMax = 0;//ȡ�ƥ������Ŀؼ�
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
                    {//����Ϊ��������ʱ
                        Keyword = tempValue[1].Trim();
                    }
                    else
                    {//û��"."���� �Ҽ����ֵ�ǿո�
                        Keyword = rowInfo[1].Trim(); 
                        //��ʾ
                        commandTipSet.ShowTipText(rowInfo[0] + " " + rowInfo[1]);
                    }
                    selectTip(Keyword);
                }
                else
                { commandTipSet.HideALL(); }
            }     
        }

        /// <summary>
        /// �ӵ�ǰָ����ȡ�������Ϣ����
        /// </summary>
        /// <param name="CodeinfList">ָ����Ϣ����</param>
        /// <param name="PntipList">ģ�����</param>
        /// <param name="Marklist">��ʾ��</param>
        /// <param name="LDtiplist">ST��������ʱ����</param>
        private void AutoGetInfo(ref List<ArrayList> CodeinfList, ref List<string> PntipList, ref List<string> Marklist, ref List<string> LDtiplist)
        {
            CodeinfList.AddRange(this.CodeInfo.Keys);//ָ������Ϣ�б�

            for (int i = 0; i < CodeinfList.Count; i++)
            {
                string Operator = (string)CodeinfList[i][0];
                List<string> Paras = (List<string>)CodeinfList[i][1];
                if (CodeinfList[i].Count == 3)
                {//��¼��ʾ��
                    Marklist.Add(CodeinfList[i][2].ToString());
                }
                if (Paras.Count > 1)
                {//Ѱ�Ҵ��ڵ����Ŀؼ�ָ���¼������Ϳɼ�����
                    ControlInfo codeCtrl = CtrlPropertys[Operator];
                    if (XProp.GetValue(CassViewGenerator.portIndex, codeCtrl.VisibleFunctionProperty) != null)
                    {
                        PntipList.Add(codeCtrl.CodeInfo[1] + Paras[0]);
                    }
                }
                else if (Operator == "ST" && Paras.Count != 0 && !LDtiplist.Contains(Paras[0]))
                {//��¼����ST��������ʱ����
                    LDtiplist.Add(Paras[0]);
                }
            }
        }
        
        /// <summary>
        /// ��XML�ļ��л�ȡָ��������������Ϣ
        /// ע����CASSVIEW����ӿؼ����� ��Ҫ�Ż���ͳһ
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
                            {//������Ϣ�м���ModuleName��ModuleSort
                                if (property.Attributes["name"].Value == "ModuleName")
                                {
                                    //if (property.InnerText == "CALCU" || property.InnerText == "PROCESS")
                                    //{//������������̬������������ؼ�
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
                                { //��ʼ���������Ϣ
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
                        {//����ע�Ϳؼ�
                            ctrlStruct.CodeInfo[1] = GenerateCode.CodeCtrlName[GenerateCode.SortCtrlName.IndexOf(ctrlStruct.SortName)];
                        }
                        //���빦������
                        if (toolItemNode.FirstChild.NextSibling.Name == "FunctionProperty")
                        {
                            CassView.ReadFunctionProperty(toolItemNode.FirstChild.NextSibling, ref ctrlStruct);
                        }
                        //���������Ϣ
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
        /// ���ݵ�ǰָ������ɶ�Ӧ�Ŀؼ���Ϣ
        /// �����¿ؼ���������
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

                        //ָ����еĲ������ÿؼ���Ϣ�ж�Ӧ������      
                        if (ctrlinfo.VisibleFunctionProperty != null && ctrlinfo.VisibleFunctionProperty.Count != 0)
                        {//�ؼ��������Դ������ж�Ϊ�㷨�ؼ�
                            //20090709�ĳɲ����ĵ�һλΪ�������

                            int paracout = 0;//�������
                            foreach (XProp element in ctrlinfo.VisibleFunctionProperty)
                            {
                                element.TheValue = Paras[paracout++];
                                if (element.VarName == CassViewGenerator.portIndex)
                                {//����е����������
                                    ctrlinfo.CodeInfo[2] = ctrlinfo.CodeInfo[1] + element.TheValue;
                                    if (!Codeportlist.ContainsKey(ctrlinfo.CodeInfo[2]))
                                    {
                                        Codeportlist.Add(ctrlinfo.CodeInfo[2], CodeInfo[rowInfo]);//����ģ������Ͷ�Ӧ�к���Ϣ
                                    }
                                    else
                                    {
                                        CassViewGenerator.SpecialErrors.Add(new string[] { CodeInfo[rowInfo].ToString(), "ָ��ؼ�����" + ctrlinfo.CodeInfo[2] + "�����ظ���", "error", curMark });
                                    }
                                }
                            }
                            foreach (XProp element in ctrlinfo.UnvisibleFunctionProperty)
                            {
                                if (GenerateCode.JOUnuseArray.Contains(element.Name))
                                {//��ʹ�õ����鸳ֵ
                                    element.TheValue
                                        = "configuration" + CassViewGenerator.ProjectNum + "_array" + (arrayCount++).ToString();
                                    List<string> tempValue = new List<string>();//���ɽ���·����ȫ0����
                                    int count = Convert.ToInt32(XProp.GetValue("iN", ctrlinfo.VisibleFunctionProperty));//����·��                                    
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
                        {//sin �ȿؼ� ֻ���в��ɼ�����
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
                                    {//�ؼ�
                                        //�ҵ��뵱ǰ�ؼ�������ƥ��Ŀؼ���Ϣ
                                        string CtrlName = null;
                                        int hitMax = 0;//ȡ�ƥ������Ŀؼ�
                                        foreach (string key in PLCCodeEditor.SunningCodes.Keys)
                                        {
                                            if (temp[0].StartsWith(key) && key.Length > hitMax)
                                            {
                                                CtrlName = key;
                                            }
                                        }
                                        ControlInfo tempCtrl = CtrlPropertys[CtrlName];
                                        //�����ÿؼ���Ϣ�еĿμ������ҵ�Сд�汾�����������и�ֵ
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
                                    {//��ʱ����
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
                            {//ST ��Ϊ��
                                //MessageBox.Show("!�����⣡");
                            }
                        }
                        if (GenerateCode.SpicalCtrlName.Contains(ctrlinfo.SortName))
                        {
                            if (GenerateCode.SpicalCtrlName.IndexOf(ctrlinfo.SortName) > 1)
                            {//����ȡ�� �ߵ�ѡ����������IO����Ϣ                                
                                foreach (string[] Info in ctrlinfo.InputInfo)
                                {//�����������Ϣ,�����������Ϣ���������
                                    Info[0] = "";
                                }
                                //���ݵ�ǰ����Ϣ��CON NULL�ĸ������������Ϣ���и���
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
                    {//����ָ��
                        Codeiolist.Add(new string[] { Operator, null });
                    }
                    continue;                
                }
            }
        }

        /// <summary>
        /// ������ȡ�к͸ߵ�ѡ�ؼ�����ָ�����
        /// ����CON null�ĸ�����������
        /// </summary>
        /// <param name="specialCtrl"></param>
        /// <param name="special"></param>
        private void CodeSetspecialctrl(ControlInfo specialCtrl, ArrayList special)
        {
            //��Ҫ������� ���ֵ䲻֧����ű���
            List<ArrayList> OrderInfo = new List<ArrayList>();
            OrderInfo.AddRange(CodeInfo.Keys);

            int unConnectNum = specialCtrl.InputInfo.Count;
            int inputIndex = 0;//���趨����������Ϣ���
            int currentNum = 0;//������������ǰ�ζ˿�

            for (int i = OrderInfo.IndexOf(special) - 1; i >= 0; i--)
            {//������� ��ʼ��Ϊ����ؼ�����һ��
                string Operator = (string)OrderInfo[i][0];
                List<string> Paras = (List<string>)OrderInfo[i][1];

           
                if (Operator == "")
                {//��ǰ�����
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
                    {//��ǰ�ؼ������������������򷵻�
                        return;
                    }
                }
            }
        }

        #region ���

        /// <summary>
        /// �ж���ǰ�Ĳ����������Ƿ�ƥ�䣬������
        /// </summary>
        /// <param name="proType">��������</param>
        /// <param name="proValue">����ֵ</param>
        /// <param name="proEnum">�Զ������Ͳ���ѡ����</param>
        /// <returns>�Ƿ�ƥ��</returns>
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
                    //���� ���Ԥ��
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ������תָ���Ƿ���ִ���
        /// </summary>
        /// <param name="JMPlist">��תָ����Ϣ�б�</param>
        /// <param name="marks">�ؼ������б�</param>
        private void checkJmp(List<string[]> JMPlist, List<string> marks)
        {
            foreach (string[] info in JMPlist)
            {
                if (info.Length != 4)
                {
                    //MessageBox.Show("�����ܵĴ���");
                }
                else
                {

                    if (!marks.Contains(info[3]))
                    {
                        ErrorList.Add(new string[] { info[0], info[1] + "��תĿ�겻����", "error", info[2] });
                    }
                    //ע��
                    //JMPֻ����ҳ����ͬ����������"_"�ָ���һ������ͬ������ת
                    //CALLֻ����ҳ�治��ͬ����ת  ���ܺ������� �����ܵ�����һ����ҳ�� 
                    string curBlock = GenerateCode.getMark(info[2]);//���ڵĵ�ǰҳ��
                    string tagBlock = GenerateCode.getMark(info[3]);//��תĿ�����ڵ�ҳ��
                    if (info[1] == "JMP" && curBlock != tagBlock
                        || info[1] == "CALL" && (curBlock == tagBlock || tagBlock != info[3] 
                        || (tagBlock.Substring(curBlock.Length).Contains("_") && tagBlock.Substring(curBlock.Length).Substring(1).Contains("_"))))
                    {
                        ErrorList.Add(new string[] { info[0], info[1] + "���÷�ΧԽȨ", "error", info[2] });
                    }
                    else if (info[2] == info[3])
                    {
                        ErrorList.Add(new string[] { info[0], info[1] + "��Ծ����", "error", info[2] });
                    }
                    else
                    {//ֻ���ڷ�ԽȨ��ת����Ծ�����޴��²Ž���ѭ����ת�ж�
                        Stack<List<string>> JmpPath = new Stack<List<string>>();//��¼��ǰ��ת��Ŀ����ת·��                       
                        Stack<string> Target = new Stack<string>();//��ת���������õ���תĿ����б�
                        Stack<string> Rowindex = new Stack<string>();//����תĿ��󶨵Ķ�Ӧ��Ŀ�к�
                        bool isRoll = false;//����ѭ��
                        string curNum = null;
                        string curTarget = null;
                        JmpPath.Push(new List<string>(new string[] { info[2] }));//��ǰ���ڵĿ�����Ϊ���
                        Target.Push(info[3]);
                        Rowindex.Push(info[0]);

                        while (Target.Count != 0)
                        {
                            curTarget = Target.Pop();
                            curNum = Rowindex.Pop();
                            int waycount = 0;//���㵱ǰ���ڵ���ת��Ŀ
                            foreach (string[] Secinfo in JMPlist)
                            {
                                if (Secinfo[0] != curNum && curTarget == Secinfo[2])
                                {//�ų����� ����������ת��Ŀ�����뵱ǰ��Ŀ���
                                    if (Secinfo[3] == curTarget || JmpPath.Peek().Contains(Secinfo[3]))
                                    {//����������ת��ĿΪ��ת���� �� ��ת��Ŀ���������ת·��
                                        isRoll = true;
                                        break;
                                    }
                                    else
                                    {//��������תĿ��ѹ��Ŀ���ջ
                                        Target.Push(Secinfo[3]);
                                        Rowindex.Push(Secinfo[0]);
                                        waycount++;
                                    }
                                }
                            }
                            if (waycount == 0)
                            {//����·�������յ�,����
                                JmpPath.Pop();
                            }
                            else if (waycount == 1)
                            {//����·��,���ر������Ŀ���
                                JmpPath.Peek().Add(curTarget);
                            }
                            else
                            {//���ַ�֧,����ǰ����·�����Ʒֲ�·��ѹ��·����ջ
                                for (int o = waycount - 1; o > 0; o--)
                                {
                                    JmpPath.Push(JmpPath.Peek());
                                }
                            }
                            if (isRoll)
                            {
                                ErrorList.Add(new string[] { info[0], info[1] + "��Ծѭ��", "error", info[2] });
                                break;
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// ����ST�����Ϣ�Ƿ�����
        /// </summary>
        /// <param name="SLs">���������Ϣ��</param>
        private void checkSL(Dictionary<string, Dictionary<string, string>> SLs)
        {
            foreach (string info in SLs.Keys)
            {
                if (info.Contains("."))
                {
                    string[] tempPN = info.Split('.');
                    if (Codeportlist.Count != 0 && !Codeportlist.ContainsKey(tempPN[0]))
                    {//���������в����ڸõ���
                        foreach (string num in SLs[info].Keys)
                        {
                            ErrorList.Add(new string[] { num, info + SLs[info][num] + "����������", "error" });
                        }
                    }
                    else
                    {//���ڶ�Ӧ�ĵ�����Ѱ����ɼ�����
                        bool findElement = false;
                        if (tempPN.Length == 2)
                        {
                            int hitMax = 0;//ȡ�ƥ������Ŀؼ�
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
                                ErrorList.Add(new string[] { num, info + "Դ��Ϣ����", "error" });
                            }
                        }
                    }
                }
                else
                {
                    if (SLs[info].ContainsValue("���"))
                    {
                        foreach (string pn in Codeportlist.Keys)
                        {
                            if (info == pn)
                            {//������ĵ�����ͬ
                                foreach (string num in SLs[info].Keys)
                                {
                                    ErrorList.Add(new string[] { num, SLs[info][num] + "Դ��Ϣ" + info + "��ģ�������Ϣ�ظ�", "error" });
                                }
                                break;
                            }
                        }
                        if (SLs[info].ContainsValue("���"))
                        {//��ǰ�Ĳ����������STʹ��   
                            if (SLs[info].Count == 1)
                            {//����û��LD����
                                foreach (string num in SLs[info].Keys)
                                {
                                    ErrorList.Add(new string[] { num, SLs[info][num] + "��ʱ����" + info + "û��ʹ��", "warning" });
                                }
                            }
                        }
                        else//!SLs[info].ContainsValue("���")
                        {//���ڲ���ʹ��,����û��ST����
                            foreach (string num in SLs[info].Keys)
                            {
                                ErrorList.Add(new string[] { num, SLs[info][num] + "��ʱ����" + info + "û���������", "error" });
                            }
                        }
                    }

                    int errorIndex = checkName(info);
                    if (errorIndex == 0)
                    {
                        foreach (string num in SLs[info].Keys)
                        {
                            ErrorList.Add(new string[] { num, "��ʱ������" + info + "���������ֿ�ͷ", "error" });
                        }
                    }
                    else if (errorIndex == -1)
                    {
                        foreach (string num in SLs[info].Keys)
                        {
                            ErrorList.Add(new string[] { num, "��ʱ������" + info + "��������", "error" });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ���鴫��ı������Ƿ������������
        /// ���������ֿ�ͷ
        /// ֻ�ܰ���������ĸ���»���
        /// </summary>
        /// <param name="tempName"></param>
        /// <returns>����1Ϊ����,0Ϊ���ֿ�ͷ����,-1Ϊ�������</returns>
        private int checkName(string tempName)
        {
            if (tempName[0] >= '0' && tempName[0] <= '9')
            {
                return 0;
            }
            for (int i = 0; i < tempName.Length; i++)
            {//ֻ����Ӣ�ġ����֡��»������
                char p = tempName[i];
                if (p >= '0' && p <= '9' || p >= 'a' & p <= 'z' || p >= 'A' && p <= 'Z' || p == '_')
                {                }
                else
                { return -1; }
            }
            return 1;
        }

        /// <summary>
        /// У��ָ��� ���ɴ����б�
        /// </summary>
        /// <param name="iolist"></param>
        /// <returns>�����Ƿ��д���򾯸�</returns>
        public bool checkIolist()
        {
            ErrorList.Clear();//�����б����
            SLinfo.Clear();//�����Ϣ���
            List<string> marks = new List<string>();      // ���
            List<string> editingOperators = new List<string>();        // ��ǰ�༭�Ĳ�����
            List<List<string>> parameters = new List<List<string>>();     // �������ϣ��ؼ���Ϊ������ 
            List<string> numList = new List<string>();//ָ���к��б�
            List<string[]> JMPlist = new List<string[]>();

            FormateAllRow();//��ʽ��������
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
                if ((Operator == "" && editingOperators.Count != 0 //��ǰû�в������Ҳ������б�Ϊ�� 
                    || MaxLine == CodeInfo[rowInfo]) //�����
                    && marks.Count != 0)//�ұ�ʾ���б�Ϊ��
                {//��Ķϲ�
                    checkBlock(numList, editingOperators, parameters, marks[marks.Count - 1], ref JMPlist);
                    numList.Clear();
                    parameters.Clear();
                    editingOperators.Clear();
                }
                if (rowInfo.Count == 3)
                {
                    if (marks.Contains((string)rowInfo[2]))
                    {
                        ErrorList.Add(new string[] { CodeInfo[rowInfo].ToString(), "������ͬ�ı�ʾ��" + (string)rowInfo[2], "error" });
                    }
                    marks.Add((string)rowInfo[2]);
                }
            }
            //�ж��Ƿ�����Ծ����
            checkJmp(JMPlist, marks);
            checkSL(SLinfo);

            if (!marks.Contains("main") && !marks.Contains("Main") && !marks.Contains("MAIN"))
            {
                ErrorList.Add(new string[] { null, "û��main����", "error" });
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
        /// �Կؼ���·Ϊ��λ�����жϴ���
        /// </summary>
        /// <param name="BlockLineNum">�ؼ��к��б�</param>
        /// <param name="editOpers">�ؼ��������б�</param>
        /// <param name="paras">�ؼ������б�</param>
        /// <param name="curmark">��ǰ�ؼ���·����</param>
        /// <param name="JMPlist">��ת�б�</param>
        private void checkBlock(List<string> BlockLineNum, List<string> editOpers, List<List<string>> paras, string curmark, ref List<string[]> JMPlist)
        {
            int errorIndex = checkName(curmark);
            if (errorIndex == 0)
            {//�˴���ʾ���ŵ��к�Ϊ�ÿ��һ�е��к�-1
                ErrorList.Add(new string[] { (Convert.ToInt32(BlockLineNum[0]) - 1).ToString(), "��ʾ����" + curmark + "���������ֿ�ͷ", "error", curmark });
            }
            else if (errorIndex == -1)
            {
                ErrorList.Add(new string[] { (Convert.ToInt32(BlockLineNum[0]) - 1).ToString(), "��ʾ����" + curmark + "��������", "error", curmark });
            }

            for (int i = 0; i < editOpers.Count; i++)
            {
                if (editOpers[i] != "")
                {

                    if (editOpers[i] == "JMP" || editOpers[i] == "CALL")
                    {//��¼��Ծ��Ϣ�кţ���ת���ţ���ǰ������Ŀ�����
                        if (paras[i].Count != 0)
                        {
                            JMPlist.Add(new string[] { BlockLineNum[i], editOpers[i], curmark, paras[i][0] });
                        }
                        else
                        {
                            ErrorList.Add(new string[] { BlockLineNum[i], "��ת�ؼ�" + editOpers[i] + "û����תĿ��", "error", curmark });
                        }
                        continue;
                    }
                    if (SpecialCode.ContainsKey(editOpers[i]))
                    {//��������ָ��
                        continue;
                    }
                    if (!CtrlPropertys.ContainsKey(editOpers[i]))
                    {//����ָ��
                        ErrorList.Add(new string[] { BlockLineNum[i], "�����ڹؼ���" + editOpers[i], "error", curmark });
                        continue;
                    }
                    //�ж��Ƿ�Ϊ�ÿ������ģ��
                    ControlInfo curCtrl = CtrlPropertys[editOpers[i]];
                    if (curCtrl.CodeInfo[1] == "ST" || curCtrl.CodeInfo[1] == "LD")
                    {
                        if (paras[i].Count == 0)
                        {
                            ErrorList.Add(new string[] { BlockLineNum[i], editOpers[i] + "����Ϊ��", "error", curmark });
                        }
                        else if (curCtrl.CodeInfo[1] == "ST")
                        { //���¼�����Ϣ
                            if (SLinfo.ContainsKey(paras[i][0]))
                            {//���ڶ�Ӧ���������
                                if (SLinfo[paras[i][0]].ContainsValue("���"))
                                {//�����Ѿ���Ϊ���������
                                    ErrorList.Add(new string[] { BlockLineNum[i], "���Դ" + paras[i][0] + "�����Ϊ�������", "warning", curmark });
                                }
                                else
                                {//û����Ϊ������� �����
                                    SLinfo[paras[i][0]].Add(BlockLineNum[i], "���");
                                }
                            }
                            else if (editOpers[i] != "POP")
                            {
                                Dictionary<string, string> SLtemp = new Dictionary<string, string>();
                                SLtemp.Add(BlockLineNum[i], "���");
                                SLinfo.Add(paras[i][0], SLtemp);
                            }
                        }
                        else if (curCtrl.CodeInfo[1] == "LD")
                        { //���¼������Ϣ
                            if (SLinfo.ContainsKey(paras[i][0]))
                            {//���ڶ�Ӧ���������,���Ӧ�ļ��뵱ǰ����Ϣ
                                SLinfo[paras[i][0]].Add(BlockLineNum[i], "����");
                            }
                            else
                            {//������,���½�
                                Dictionary<string, string> SLtemp = new Dictionary<string, string>();
                                SLtemp.Add(BlockLineNum[i], "����");
                                SLinfo.Add(paras[i][0], SLtemp);
                            }
                        }
                    }
                    //�жϲ��������Ͳ���ֵ��Χ
                    if (curCtrl.VisibleFunctionProperty != null && curCtrl.VisibleFunctionProperty.Count != 0)
                    {//�ؼ��������Դ������ж�Ϊ�㷨�ؼ�
                          int paracout = 0;//��ȷ�Ĳ�������

                        foreach (XProp element in curCtrl.VisibleFunctionProperty)
                        {
                            if (paras[i].Count <= paracout)
                            { //ʵ�ʱ��������� �������в��� ����������
                                paracout++;
                                continue;
                            }
                            if (!checkProperty(element.ValueType, paras[i][paracout++], element.EnumValue))
                            {
                                ErrorList.Add(new string[] { BlockLineNum[i], "�ؼ�ָ��" + curCtrl.CodeInfo[1] + "����" + element.VarName + ",ֵ" + element.TheValue.ToString() + "��Χ����", "error", curmark });
                            }
                        }

                        foreach (XProp element in curCtrl.UnvisibleFunctionProperty)
                        {
                            if (element.TheValue.ToString().Contains("array")//����Ϊ������� 
                                && !GenerateCode.JOUnuseArray.Contains(element.Name))//�Ҳ�Ϊ�û����ɼ�����
                            {
                                if (paras[i].Count <= paracout)
                                {//ʵ�ʱ��������� �������в��� ����������
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
                                    ErrorList.Add(new string[] { BlockLineNum[i], "�ؼ�ָ��" + curCtrl.CodeInfo[1] + "�������" + element.VarName + "����", "error", curmark });
                                    //break;
                                }
                            }
                        }
                        if (paracout > paras[i].Count)
                        {
                            ErrorList.Add(new string[] { BlockLineNum[i], curCtrl.CodeInfo[1] + "�Ĳ���������ƥ��(����)", "error", curmark });
                        }
                        if (paracout < paras[i].Count)
                        {
                            ErrorList.Add(new string[] { BlockLineNum[i], curCtrl.CodeInfo[1] + "�Ĳ���������ƥ��(����)", "error", curmark });
                        }
                    }
                    //�ж����������������
                    if (curCtrl.OutputInfo != null && curCtrl.OutputInfo.Count != 0)
                    {//�����
                        int unConnectNum = 0;
                        if (curCtrl.OutputInfo.Count != 1)
                        {
                            for (int j = 0; j < curCtrl.OutputInfo.Count; j++)
                            {
                                if (i + j + 1 >= editOpers.Count || (editOpers[i + j + 1] != "ST" && editOpers[i + j + 1] != "POP"))
                                {
                                    ErrorList.Add(new string[] { BlockLineNum[i], curCtrl.CodeInfo[1] + "�Ķ������������������ST������һ��", "error", curmark });
                                    break;
                                }
                                else if (paras[i + j + 1].Count == 0 || editOpers[i + j + 1] == "POP")
                                {//���û�ָ���ģʽ��û��������ô��ʾ����20090706
                                    unConnectNum++;
                                }
                            }
                            if (unConnectNum == curCtrl.OutputInfo.Count)
                            {
                                ErrorList.Add(new string[] { BlockLineNum[i], "�ؼ�ָ��" + curCtrl.CodeInfo[1] + "�����û����", "warning", curmark });
                            }
                        }
                        else
                        {//������������󣡣������������������Ƿ�Ҫ���20090707
                            unConnectNum = 1;
                            for (int j = unConnectNum; j + i < editOpers.Count; j++)
                            {//�����б������
                                if (CtrlPropertys.ContainsKey(editOpers[i + j]))
                                {//���в���ָ������
                                    ControlInfo NextCtrl = CtrlPropertys[editOpers[i + j]];
                                    unConnectNum = unConnectNum - NextCtrl.InputInfo.Count + NextCtrl.OutputInfo.Count;
                                }
                                else if (editOpers[i + j] == "POP")
                                {
                                    unConnectNum--;
                                }
                                if (unConnectNum <= 0)
                                {//��ǰ�ؼ�����������������������ѭ��
                                    break;
                                }
                            }
                            if (unConnectNum > 0)
                            {//����ǰ��·�����껹��δ����Ķ˿ڴ��������
                                ErrorList.Add(new string[] { BlockLineNum[i], "���ƻ�·" + curmark + "�еĿؼ�" + curCtrl.CodeInfo[1] + "ȱ�����ģ��", "error", curmark });
                            }
                        }
                    }
                    //�ж����������
                    if (curCtrl.InputInfo != null && curCtrl.InputInfo.Count != 0)
                    {
                        int unConnectNum = curCtrl.InputInfo.Count;
                        for (int j = 1; i - j >= 0; j++)
                        {//�����б������
                            if (CtrlPropertys.ContainsKey(editOpers[i - j]))
                            {//���в���ָ������
                                ControlInfo NextCtrl = CtrlPropertys[editOpers[i - j]];
                                unConnectNum = unConnectNum + NextCtrl.InputInfo.Count - NextCtrl.OutputInfo.Count;
                            }
                            else if (editOpers[i - j] == "POP")
                            {
                                unConnectNum++;
                            }
                            if (unConnectNum <= 0)
                            {//��ǰ�ؼ�����������������������ѭ��
                                break;
                            }
                        }
                        if (unConnectNum > 0)
                        {//����ǰ��·�����껹��δ����Ķ˿ڴ��������
                            ErrorList.Add(new string[] { BlockLineNum[i], "���ƻ�·" + curmark + "�еĿؼ�" + curCtrl.CodeInfo[1] + "ȱ������ģ��", "error", curmark });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ��ʽ����ǰָ��ҳ���������
        /// ������ʽ����
        /// </summary>
        public void FormateAllRow()
        {
            string allFormatedText = null;
            for (int lineNo = 0; lineNo < LineCount; lineNo++)
            {
                string formatedText = FormatRow(lineNo, false, true);    // �ȸ�ʽ��ÿһ��,����Ĳ���жϱ����ڸ�ʽ��������½���

                if (lineNo != LineCount - 1)
                    allFormatedText += formatedText + "\n";
                else
                    allFormatedText += formatedText;
            }
            //��ʽ����� ����
            Text = allFormatedText;
        }

        #endregion

        /// <summary>
        /// ʧȥ����,��ʾ��Ϣ��������˵�ʱ���ᣬvsҲ��������        
        /// </summary>
        /// <param name="e"></param> 
        protected override void OnLostFocus(EventArgs e)
        {
            this.commandTipSet.Items.Clear();
            this.commandTipSet.Hide();
            base.OnLostFocus(e);
        }

        /// <summary>
        /// ʧȥ�����¼�
        /// ������ʾ��
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
        /// ���������¼�
        /// ������ʾ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PLCCodeEditor_VScroll(object sender, ICodeSenseEvents_VScrollEvent e)
        {
            this.commandTipSet.HideALL();
        }

        /// <summary>
        /// ��������¼�
        /// ������ʾ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PLCCodeEditor_HScroll(object sender, ICodeSenseEvents_HScrollEvent e)
        {
            this.commandTipSet.HideALL();
        }

    }
}
