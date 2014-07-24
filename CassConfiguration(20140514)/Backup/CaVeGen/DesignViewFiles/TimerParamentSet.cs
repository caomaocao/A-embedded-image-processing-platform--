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
        private bool timeEnabeFlag = false;        //��ʾ��ҳ��Ķ�ʱ����ʼ��״̬�����Ƿ���
        private bool updateFlag = false;           //��־��ǰ�Ƿ��޸ĳɹ�
        private int timeInterval = 0;              //��ҳ��Ķ�ʱ����ʱ����

        public TimerParamentSet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ���캯����������ǰ��ҳ��Ķ�ʱ����״̬��ʱ��
        /// </summary>
        /// <param name="timerEnable">��ʱ����״̬,true or false</param>
        /// <param name="timeInterval">��ʱ�������õļ��ʱ��</param>
        public TimerParamentSet(bool timerEnable,int timerInterval)
        {
            InitializeComponent();

            timeEnabeFlag = timerEnable;  //��õ�ǰ��ʱ��ҳ��
            timeInterval = timerInterval; //��õ�ǰ��ʱ���Ķ�ʱʱ��

            //��ʾ��ҳ����,�ȶ��Ƿ�����״̬��������
            if (timeEnabeFlag == true)
            {
                timeEnablecheckBox.Checked = true;
            }
            else
            {
                timeEnablecheckBox.Checked = false;
            }

            //���������õĶ�ʱ����ʱ�����Ѿ�����ҳ���Ͻ��������ã�����������ʱ�䷶Χ���ж�
            timetextBox.Text = timeInterval.ToString();
        }

        /// <summary>
        /// ȷ�����޸��Զ����涨ʱ��������
        /// </summary>
        /// <param name="sender">buttonOk����</param>
        /// <param name="e">��������Click�¼�</param>
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

                    //����ǰ���ñ��浽��װĿ¼�µ�TimerSetParament.inf�ļ���
                    fStream = new FileStream(Path.Combine(CassViewGenerator.designerPath, PublicVariable.TimeSetFileName), FileMode.Create);
                    sWriter = new StreamWriter(fStream);
                    sWriter.WriteLine(timeEnabeFlag.ToString().ToLower());
                    sWriter.WriteLine(timeInterval);
                    sWriter.Close();
                    fStream.Close();

                    updateFlag = true;
                    this.Close();    //�ɹ���ر�

                }
                else
                {
                    CassMessageBox.Warning("ʱ�䷶Χ���ô���");
                }
            }
            catch (FileNotFoundException ex)
            {
                CassMessageBox.Error("��װ�ļ����𻵣�");
            }
            catch (DirectoryNotFoundException ex)
            {
                CassMessageBox.Error("��װ�ļ����𻵣�");
            }
            catch (SecurityException ex)
            {
                CassMessageBox.Error("�ļ�Ȩ�ޱ��޸ģ�");
            }
            catch (Exception ex)
            {
                CassMessageBox.Error("������̷����쳣������������ı���ʱ���ʾ����");
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
        /// �رյ�ǰҳ��
        /// </summary>
        /// <param name="sender">buttonReturn����</param>
        /// <param name="e">��������Click�¼�</param>
        private void buttonReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// ��־�Ƿ����޸Ĺ���ʱ��������
        /// </summary>
        public bool UpdateFlag
        {
            get
            {
                return updateFlag;
            }
        }

        /// <summary>
        /// ��ʱ���Ƿ���״̬
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