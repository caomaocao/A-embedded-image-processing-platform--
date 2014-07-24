using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CaVeGen.DesginForm
{
    public partial class InspectionResultMark : UserControl
    {
        private int count_Pass = 0;  //pass个数
        private int count_Fail = 0;  //fail个数 

        public InspectionResultMark()
        {
            InitializeComponent();
        }

        //设置pass个数
        private void setPassCount()
        {
            this.rs_pass.Text = this.count_Pass.ToString();
        }
        //设置fail个数
        private void setFailCount()
        {
            this.rs_fail.Text = this.count_Fail.ToString();
        }

        /// <summary>
        /// 累加计数器
        /// </summary>
        /// <param name="type">pass or fail</param>
        private void AccCounter(string type, bool flag)
        {
            switch (type)
            {
                case "pass":
                    if (flag)
                        this.count_Pass++;
                    else
                        this.count_Pass = 1;
                    break;
                case "fail":
                    if (flag)
                        this.count_Fail++;
                    else
                        this.count_Fail = 1;
                    break;
            }

        }

        /// <summary>
        /// 设置匹配率，外部接口
        /// </summary>
        /// <param name="percent"></param>
        public void setMatch(int percent)
        {
            this.rs_match.Text = percent.ToString() + "%";
        }
        /// <summary>
        /// 显示计数器结果，外部接口
        /// </summary>
        public void ShowCounter()
        {
            this.setPassCount();
            this.setFailCount();
        }

        /// <summary>
        /// 清空描述状态
        /// </summary>
        public void ClearInspectionStatus()
        {
            this.rs_pass.Text = "";
            this.rs_fail.Text = "";
            this.rs_match.Text = "";
            //  this.statusPic

        }
        /// <summary>
        /// 清空计数器
        /// </summary>
        public void ClearCounter()
        {
            this.count_Pass = 0;
            this.count_Fail = 0;
        }

        /// <summary>
        /// 设置调试状态栏
        /// </summary>
        /// <param name="status">状态："null" or "pass" or "fail"</param>
        /// <param name="isLoop">bool</param>
        public void SetDebugStatus(string status, bool isLoop)
        {
            switch (status)
            {
                case "null":
                    this.statusPic.Visible = false;
                    break;
                case "pass":
                    this.statusPic.Image = global::CaVeGen.Properties.Resources.pass;
                    this.statusPic.Visible = true;
                    this.AccCounter("pass", isLoop);
                    //  this.isStepDebug = isStepDebug;
                    break;
                case "fail":
                    this.statusPic.Image = global::CaVeGen.Properties.Resources.fail;
                    this.statusPic.Visible = true;
                    this.AccCounter("fail", isLoop);
                    // this.isStepDebug = isStepDebug;
                    break;
            }
        }
        private void gBox_status_Resize(object sender, EventArgs e)
        {
            //gBox_status中各控件的间距
            int gWidth = this.gBox_status.Width;
            int gHeight = (this.gBox_status.Height - this.lb_Pass.Height * 3 - statusPic.Height) / 13 * 2;

            //pass count:xxx
            lb_Pass.Location = new Point(5, gHeight * 2);
            rs_pass.Location = new Point(this.lb_match.Width + 10, gHeight * 2);


            //fail count : xxx
            lb_Fail.Location = new Point(5, lb_Pass.Height + gHeight * 3);
            rs_fail.Location = new Point(this.lb_match.Width + 10, lb_Pass.Height + gHeight * 3);


            //match count :  xxx
            lb_match.Location = new Point(5, lb_Pass.Height + lb_Fail.Height + gHeight * 4);
            rs_match.Location = new Point(this.lb_match.Width + 10, lb_Pass.Height + lb_Fail.Height + gHeight * 4);


            //设置groupbox中状态图片的位置 pass or fail
            statusPic.Location = new Point(gWidth / 3, gHeight * 11 / 2 + this.rs_pass.Height * 3);
            statusPic.Size = new Size(64, 32);
        }
    }
}
