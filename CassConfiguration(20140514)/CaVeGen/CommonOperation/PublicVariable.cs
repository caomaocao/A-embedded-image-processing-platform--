/******************************************************************************************************************************************
           ** Copyright (C) 2006 CASS 版权所有

           ** 文件名：PublicVariable.cs 
           ** 功能描述：
 *              该类记录了整个系统的所需要的公共字符常量、标识以及对内存映射操作的dll函数的引用。
* 
           ** 作者：吴丹红
 *         ** 创始时间：2007-4-10


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
        //键盘消息号
        public const int KeyUp = 0x26;      //上
        public const int KeyDown = 0x28;        //下
        public const int KeyLeft = 0x25;        //左
        public const int KeyRight = 0x27;       //右
        public const int KeyPageDown = 0x22;        //Page Down
        public const int KeyPageUp = 0x21;        //Page Up
        public const int KeyDelete = 0x2E;      //Delete
        public const int KeyPress = 0x100;      //按键

        //鼠标操作
        public const int DClick = 2;   //双击

        //鼠标消息号
        public const int MouseMove = 0x200;     //鼠标移动
        public const int MouseLeftDown = 0x201;     //鼠标左键按下
        public const int MouseLeftUp = 0x202;       //鼠标左键弹起
        public const int MouseDoubleClick = 0x203;//鼠标双击
        public const int MoustRight = 0x205;        //鼠标右键

        //程序集名称
        public const string strSystem = "System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        public const string strData = "System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        public const string strXml = "System.Xml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        public const string strDesign = "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        public const string strJanus = "Janus.Windows.Common.v3, Version=3.0.0.0, Culture=neutral, PublicKeyToken=21d5517571b185bf";
        public const string strExporerBar = "Janus.Windows.ExplorerBar.v3, Version=3.0.0.0, Culture=neutral, PublicKeyToken=dc38f5dfc6212b78";
        public const string strConfiguration = "System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        public const string strResource = "System.resources, Version=2.0.0.0, Culture=zh-CHS, PublicKeyToken=b77a5c561934e089";
        public const string strAccessibility = "Accessibility, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        //所读取的XML中的View的全称
        public const string viewName = "CaVeGen.DesignViewFiles.CassView";

        //自定义控件的分类名称
        public const string ControlCategoryName = "基本属性";
        public const string ControlFuntionName = "功能属性";
        public const string ToolConfigFileName = "sys\\ToolConfig_PC.xml";
     //   public const string ToolConfigFileName = "ToolConfig.xml";
        public const string TimeSetFileName = "TimerSetParament.inf";       //自动保存工程的参数文件名称
        public const string DesignViewName = "tactic";      //资源管理器中设计页面名称的起始字符串
        public const string DefaultWorkSpacePath = "D:\\CassWorkSpace"; //系统默认工作目录
        public const string sysInfoFileName = "sys\\sysInfo.txt"; //系统工作目录存放文件      
        public const string HardwareConfigFileName = "sys\\hardware config.txt"; //硬件配置文件
        public const string NegativePointsFileName = "sys\\negative_points.txt";   
        public const string NumberConfigExePath = "sys\\NumberConfig.exe"; //数字识别特征点.exe
        public const string FeatherPointsExePath = "sys\\FeaturePoints.exe";
        public const string CompileFileName = "ComFile.txt";   //串口配置文件(用户工作目录)
        public const string GenDataConfigFileName = "GenDataConfig.txt";  //智能算法生成特征点配置文件（用户工作目录）

        #region Memory Mapped Files 内存映射操作

        //定义共享内存时，所需要引入的API函数
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

        //在创建、打开和察看内存共享文件时，所需要的参数定义

        [Flags]  //创建内存块的属性方式,供CreateFileMapping 和 OpenFileMapping参数使用 
        public enum MapProtection
        {
            PageNone = 0x00000000,
            // protection
            PageReadOnly = 0x00000002,
            PageReadWrite = 0x00000004,
            PageWriteCopy = 0x00000008,
        }

        public enum MapAccess //读内存块的方式,供ViewFileMapping参数使用 
        {
            FileMapCopy = 0x00000001,
            FileMapWrite = 0x00000002,
            FileMapRead = 0x00000004,
            FileMapAllAccess = 0x0000001f,
        }

        #endregion

        # region 只允许运行一个可执行文件的API 函数

        [DllImport("User32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion
    }
}

