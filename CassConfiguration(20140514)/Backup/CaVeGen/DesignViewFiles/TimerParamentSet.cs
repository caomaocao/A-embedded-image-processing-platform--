using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CaVeGen.CommonOperation;
using System.Security;

namespace CaVeGen.DesignViewFiles
{
    public partial class TimerParamentSet : Form
    {
        private bool timeEnabeFlag = false;        //表示主页面的定时器的始能状态，即是否开启
        private bool updateFlag = false;           //标志当前是否修改成功
        private int timeInterval = 0;              //主页面的定时器的时间间隔

        public TimerParamentSet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 构造函数，传进当前主页面的定时器的状态和时间
        /// </summary>
        /// <param name="timerEnable">定时器的状态,true or false</param>
        /// <param name="timeInterval">定时器所设置的间隔时间</param>
        public TimerParamentSet(bool timerEnable,int timerInterval)
        {
            InitializeComponent();

            timeEnabeFlag = timerEnable;  //获得当前定时器页面
            timeInterval = timerInterval; //获得当前定时器的定时时间

            //显示在页面上,先对是否开启的状态进行设置
            if (timeEnabeFlag == true)
            {
                timeEnablecheckBox.Checked = true;
            }
            else
            {
                timeEnablecheckBox.Checked = false;
            }

            //由于所设置的定时器的时间间隔已经在主页面上进行了设置，则无需再做时间范围的判断
            timetextBox.Text = timeInterval.ToString();
        }

        /// <summary>
        /// 确定后修改自动保存定时器的设置
        /// </summary>
        /// <param name="sender">buttonOk对象</param>
        /// <param name="e">所触发的Click事件</param>
        private void buttonOK_Click(object sender, EventArgs e)
        { 
            FileStream fStream = null;
            StreamWriter sWriter = null;

            try
            {
                timeInterval = Convert.ToInt16(timetextBox.Text);

                if (timeInterval > 0 && timeInterval <= 60)
                {
                    if (timeEnablecheckBox.CheckState == CheckState.Checked)
                    {
                        timeEnabeFlag = true;
                    }
                    else 
                    {
                        timeEnabeFlag = false;
                    }

                    //将当前配置保存到安装目录下的TimerSetParament.inf文件中
                    fStream = new FileStream(Path.Combine(CassViewGenerator.designerPath, PublicVariable.TimeSetFileName), FileMode.Create);
                    sWriter = new StreamWriter(fStream);
                    sWriter.WriteLine(timeEnabeFlag.ToString().ToLower());
                    sWriter.WriteLine(timeInterval);
                    sWriter.Close();
                    fStream.Close();

                    updateFlag = true;
                    this.Close();    //成功后关闭

                }
                else
                {
                    CassMessageBox.Warning("时间范围设置错误！");
                }
            }
            catch (FileNotFoundException ex)
            {
                CassMessageBox.Error("安装文件被损坏！");
            }
            catch (DirectoryNotFoundException ex)
            {
                CassMessageBox.Error("安装文件被损坏！");
            }
            catch (SecurityException ex)
            {
                CassMessageBox.Error("文件权限被修改！");
            }
            catch (Exception ex)
            {
                CassMessageBox.Error("保存过程发生异常，可能是输入的保存时间格示错误！");
            }
            finally
            {
                if (sWriter != null)
                {
                    sWriter.Dispose();
                }
                if (fStream != null)
                {
                    fStream.Dispose();
                }
                
            }
        }

        /// <summary>
        /// 关闭当前页面
        /// </summary>
        /// <param name="sender">buttonReturn对象</param>
        /// <param name="e">所触发的Click事件</param>
        private void buttonReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 标志是否已修改过定时器的设置
        /// </summary>
        public bool UpdateFlag
        {
            get
            {
                return updateFlag;
            }
        }

        /// <summary>
        /// 定时器是否开启状态
        /// </summary>
        public bool TimerEnable
        {
            get 
            {
                return timeEnabeFlag;
            }
        }

        public int TimerInterval
        {
            get
            {
                return timeInterval;
            }
        }
    }
}