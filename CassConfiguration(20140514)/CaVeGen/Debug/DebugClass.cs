using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using CaVeGen.CommonOperation;

namespace CaVeGen.Debug
{
    class DebugClass
    {

        private DoBackgroundWorker _LoopDebug;
        private StepDebug _StepDebug;
        private string CurrentProjectPath; 
        
        private  List<string> _CurControlsNum; //当前控件序号列表
        public DebugClass(string path)
        {
            this.CurrentProjectPath = path;
            this._LoopDebug = new DoBackgroundWorker();
            this._StepDebug = new StepDebug();
        }


       
        public void StartDebug(bool flag,List<string> curControlList)
        {
            if (flag == true)   //循环调试
            {
                this._LoopDebug.Start();
            }
            else   //单步调试
            {

                this._StepDebug.Start();
                
            }

        }

        public void StopDebug()
        {
            this._LoopDebug.Stop();
        }


        /// <summary>
        /// 调用批处理，生成.exe
        /// </summary>
        private bool PCMakeBat(string batName,string arg)
        {
            try
            {

                Process p = new Process();
                p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + batName;
                p.StartInfo.Arguments = arg;// currentProjectPath + "  " + ProjectName + "  " + ProgramPath;//传入参数（工程路径、工程名、系统路径）
                p.StartInfo.UseShellExecute = false;  //以管理员身份打开
                p.StartInfo.CreateNoWindow = true; //设置不显示示dos窗口

                //    p.StartInfo.RedirectStandardInput = true;  // 重定向输入
                p.StartInfo.RedirectStandardOutput = true;  //   重定向输出
                p.StartInfo.RedirectStandardError = true;    //  重定向输出错误

                //启动进程
                p.Start();


                //   StreamReader reader = p.StandardOutput;  //截取输出流

                //     string errorInfo =p.StandardError.ReadToEnd();//截取错误流
                //     if(errorInfo!="")
                //         CassMessageBox.Warning(errorInfo);

                //       this.errorForm.errorList.Add(new string[] { null, errorInfo, "error", null });
                //string line = null;
                //while (!reader.EndOfStream)
                //{
                //    line = reader.ReadLine();
                //    this.errorForm.errorList.Add(new string[] { null, line, "info", null });

                //}

                //while (!readerErr.EndOfStream)
                //{
                //    line = reader.ReadLine();
                //    this.errorForm.errorList.Add(new string[] { null, line, "error", null });
                //}



                //添加重定向输出 事件
                //   p.OutputDataReceived += new DataReceivedEventHandler(PCMakeBat_OutputDataReceived);
                //   p.ErrorDataReceived += new DataReceivedEventHandler(PCMakeBat_ErrorDataReceived);
                //    p.BeginOutputReadLine();
                //  p.BeginErrorReadLine();


                p.WaitForExit(); //等待结束（很重要！）
                p.Close();
                p.Dispose();

                return true;

            }
            catch (Exception e)
            {
              //  MessageBox.Show("批处理错误：" + e.ToString());
                return false;
            }

        }


        /// <summary>
        /// 删除上一次调试的缓存结果
        /// </summary>
        private void ClearDebugResult()
        {
            File.Delete(this.CurrentProjectPath + "\\result.txt");

            FileOperator.DeleteFiles(this.CurrentProjectPath + "\\out", ".bmp");
        }

    }
}
