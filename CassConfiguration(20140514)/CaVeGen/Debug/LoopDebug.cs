using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace CaVeGen.Debug
{
    class DoBackgroundWorker
    {
        private BackgroundWorker _BackGroundWorker;  //后台进程

        public DoBackgroundWorker()
        {
            this._BackGroundWorker = new BackgroundWorker();
            this._BackGroundWorker.WorkerReportsProgress = true;
            this._BackGroundWorker.WorkerSupportsCancellation = true;
            this._BackGroundWorker.DoWork +=new DoWorkEventHandler(_BackGroundWorker_DoWork);
            this._BackGroundWorker.ProgressChanged += new ProgressChangedEventHandler(_BackGroundWorker_ProgressChanged);
            this._BackGroundWorker.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(_BackGroundWorker_RunWorkerCompleted);
            
        }


        private void _BackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
           
        }

        //进程对应的进度条(异步)
        private void _BackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

          

        }

        //进程完成后调用
        private void _BackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           

        }


        /// <summary>
        /// 开始循环
        /// </summary>
        internal void Start()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 停止循环
        /// </summary>
        internal void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
