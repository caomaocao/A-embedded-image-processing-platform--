using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using CaVeGen.CommonOperation;

namespace CaVeGen
{
    static class Program
    {
         //0不可见但仍然运行,1居中,2最小化,3最大化
         private const int WS_SHOWNORMAL = 3;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
           
            //得到正在运行的例程

            Process instance = RunningInstance();

            if (instance == null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new CassViewGenerator());
                //Application.Run(new DesignViewFiles.MainScript());
            }

            else
            {
                //处理发现的例程
                HandleRunningInstance(instance);

                //MessageBox.Show("当前程序已经运行了!");

            }
        }


        /// <summary>
        /// 不允许有两个程序同时启动
        /// </summary>
        /// <returns></returns>
        public static Process RunningInstance()
        {

            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            //遍历正在有相同名字运行的例程

            foreach (Process process in processes)
            {

                //忽略现有的例程
                if (process.Id != current.Id)
                {

                    //确保例程从EXE文件运行
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName) //关联进程主模块的文件名称进行比较
                    {
                        //返回另一个例程实例
                        return process;

                    }

                }

            }

            //没有其它的例程，返回Null
            return null;

        }

       public static void HandleRunningInstance(Process instance)

         {

              //确保窗口没有被最小化或最大化
              PublicVariable.ShowWindowAsync (instance.MainWindowHandle , WS_SHOWNORMAL);

              //设置真实例程为foreground window
              PublicVariable.SetForegroundWindow (instance.MainWindowHandle);

         }

      
     }

}