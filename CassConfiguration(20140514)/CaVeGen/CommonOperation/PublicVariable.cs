/******************************************************************************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����

           ** �ļ�����PublicVariable.cs 
           ** ����������
 *              �����¼������ϵͳ������Ҫ�Ĺ����ַ���������ʶ�Լ����ڴ�ӳ�������dll���������á�
* 
           ** ���ߣ��ⵤ��
 *         ** ��ʼʱ�䣺2007-4-10


******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;


namespace CaVeGen.CommonOperation
{
    public class PublicVariable
    {
        //������Ϣ��
        public const int KeyUp = 0x26;      //��
        public const int KeyDown = 0x28;        //��
        public const int KeyLeft = 0x25;        //��
        public const int KeyRight = 0x27;       //��
        public const int KeyPageDown = 0x22;        //Page Down
        public const int KeyPageUp = 0x21;        //Page Up
        public const int KeyDelete = 0x2E;      //Delete
        public const int KeyPress = 0x100;      //����

        //������
        public const int DClick = 2;   //˫��

        //�����Ϣ��
        public const int MouseMove = 0x200;     //����ƶ�
        public const int MouseLeftDown = 0x201;     //����������
        public const int MouseLeftUp = 0x202;       //����������
        public const int MouseDoubleClick = 0x203;//���˫��
        public const int MoustRight = 0x205;        //����Ҽ�

        //��������
        public const string strSystem = "System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        public const string strData = "System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        public const string strXml = "System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        public const string strDesign = "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        public const string strJanus = "Janus.Windows.Common.v3, Version=3.0.0.0, Culture=neutral, PublicKeyToken=21d5517571b185bf";
        public const string strExporerBar = "Janus.Windows.ExplorerBar.v3, Version=3.0.0.0, Culture=neutral, PublicKeyToken=dc38f5dfc6212b78";
        public const string strConfiguration = "System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        public const string strResource = "System.resources, Version=2.0.0.0, Culture=zh-CHS, PublicKeyToken=b77a5c561934e089";
        public const string strAccessibility = "Accessibility, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        //����ȡ��XML�е�View��ȫ��
        public const string viewName = "CaVeGen.DesignViewFiles.CassView";

        //�Զ���ؼ��ķ�������
        public const string ControlCategoryName = "��������";
        public const string ControlFuntionName = "��������";
        public const string ToolConfigFileName = "sys\\ToolConfig_PC.xml";
     //   public const string ToolConfigFileName = "ToolConfig.xml";
        public const string TimeSetFileName = "TimerSetParament.inf";       //�Զ����湤�̵Ĳ����ļ�����
        public const string DesignViewName = "tactic";      //��Դ�����������ҳ�����Ƶ���ʼ�ַ���
        public const string DefaultWorkSpacePath = "D:\\CassWorkSpace"; //ϵͳĬ�Ϲ���Ŀ¼
        public const string sysInfoFileName = "sys\\sysInfo.txt"; //ϵͳ����Ŀ¼����ļ�      
        public const string HardwareConfigFileName = "sys\\hardware config.txt"; //Ӳ�������ļ�
        public const string NegativePointsFileName = "sys\\negative_points.txt";   
        public const string NumberConfigExePath = "sys\\NumberConfig.exe"; //����ʶ��������.exe
        public const string FeatherPointsExePath = "sys\\FeaturePoints.exe";
        public const string CompileFileName = "ComFile.txt";   //���������ļ�(�û�����Ŀ¼)
        public const string GenDataConfigFileName = "GenDataConfig.txt";  //�����㷨���������������ļ����û�����Ŀ¼��

        #region Memory Mapped Files �ڴ�ӳ�����

        //���干���ڴ�ʱ������Ҫ�����API����
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(
            IntPtr hFile, IntPtr lpAttributes, int flProtect, int dwMaximumSizeLow, int dwMaximumSizeHigh, String lpName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(
            int dwDesiredAccess, bool bInheritHandle, String lpName);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh,
            int dwFileOffsetLow, int dwNumBytesToMap);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        //�ڴ������򿪺Ͳ쿴�ڴ湲���ļ�ʱ������Ҫ�Ĳ�������

        [Flags]  //�����ڴ������Է�ʽ,��CreateFileMapping �� OpenFileMapping����ʹ�� 
        public enum MapProtection
        {
            PageNone = 0x00000000,
            // protection
            PageReadOnly = 0x00000002,
            PageReadWrite = 0x00000004,
            PageWriteCopy = 0x00000008,
        }

        public enum MapAccess //���ڴ��ķ�ʽ,��ViewFileMapping����ʹ�� 
        {
            FileMapCopy = 0x00000001,
            FileMapWrite = 0x00000002,
            FileMapRead = 0x00000004,
            FileMapAllAccess = 0x0000001f,
        }

        #endregion

        # region ֻ��������һ����ִ���ļ���API ����

        [DllImport("User32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion
    }
}

