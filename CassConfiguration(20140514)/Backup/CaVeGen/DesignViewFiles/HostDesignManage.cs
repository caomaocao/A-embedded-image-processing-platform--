/************************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����
           ** �ļ�����HostDesign.cs 
           ** ����������
           **          ��Ҫ��������������ж��ҳ�棬ÿ��ҳ�涼Ϊ����HostDesign���
           **          �Զ���ؼ�HostControl,�̳�DesignSurfaceManager�ࡣ
 * 
                     * ����ʵ�����½����ҳ��ʹ����ҳ��ʱ���������ҳ�����ƴ���HostControl���ͱ����ĺ�����
                     * �ڴ����е������ļ�ʱ���ȶ������ļ��ĸ�ʽ�����ж��Ƿ�ΪԼ����xml��ʽ�ĵ������ҶԿؼ�������Ϣ����ӵ�����������⴦��
 * 
           ** ���ߣ��ⵤ��
           ** ��ʼʱ�䣺2006-11-11
           ** 
*************************************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.Windows.Forms;
using System.IO;
using CaVeGen.CommonOperation;
using System.Xml;

namespace CaVeGen.DesignViewFiles
{
    /// <summary>
    ///������ HostDesign. �κ��� HostDesigneManager
    ///����ӵķ�������HostDesign��ʵ�֡�
    /// </summary>
    public class HostDesignManage : DesignSurfaceManager
    {

        public HostDesignManage()
            : base()
        {
            //
        }

        /// <summary>
        /// ��д�˴������ͼ���ʵ����
        /// </summary>
        /// <param name="parentProvider">�������</param>
        /// <returns>�����˷����HostDesign�������������</returns>
        protected override DesignSurface CreateDesignSurfaceCore(IServiceProvider parentProvider) 
        {
            return new HostDesign(parentProvider);
        }

        /// <summary>
        /// ����һ���µ�HostDesign,���ҽ��м��ء�
        /// </summary>
        /// <param name="formName">�����������ҳ�������</param>
        /// <returns> ����һ���������������HostControl����</returns>
        public HostControl GetNewHost(string formName)
        {
            try
            {
                HostDesign hostSurface = (HostDesign)this.CreateDesignSurface(this.ServiceContainer);
                CodeDomHostLoader codeDomHostLoader = new CodeDomHostLoader(formName);

                hostSurface.BeginLoad(codeDomHostLoader);   //ʹ�ø�������������س���ʼ���ع��̡�
                hostSurface.Initialize();

                return new HostControl(hostSurface);  
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public void AddService(Type type, object serviceInstance)
        {
            this.ServiceContainer.AddService(type, serviceInstance);
        }

        /// <summary>
        /// �����µ�Xml�ļ�ʱ�����µ��������
        /// �Զ����xml�ĵ����д������xml�ĵ������ƺ������Form�����Ʋ�һ�£����������������Ϊ��xml�ĵ�������һ�¡�
        /// ͬʱ���ж������xml�ĵ��ĸ�ʽ�Ƿ�Ϊ��ȷ�������ļ���
        /// ���������ļ����пؼ����ӵ���Ϣ��������ӵĿؼ�����Ϣ��ӵ�������С�
        /// </summary>
        /// <param name="fileName">Ҫ�����xml�ļ�������/param>
        /// <returns></returns>
        public HostControl GetNewDesignHost(string fileName)
        {
            HostDesign hostSurface = (HostDesign)this.CreateDesignSurface(this.ServiceContainer);

            //�ж��ļ����ƺ��ļ��е�CassView�����Ƿ�һ��
            string designName = Path.GetFileNameWithoutExtension(fileName);
            string documentDesignName = designName;

            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(fileName);
                XmlNode node = document.FirstChild;
                for (int i = 0; i < node.ChildNodes.Count; i++) // �Զ����xml�ĵ����д������xml�ĵ������ƺ������Form�����Ʋ�һ�£����������������Ϊ��xml�ĵ�������һ�¡�
                {
                    if (node.ChildNodes[i].Name.Equals("Object"))
                    {
                        if (node.ChildNodes[i].Attributes["type"] == null)
                        {
                            //CassMessageBox.Warning("�����ļ���ʽ����");
                            return null;
                        }
                        else
                        {
                            string[] type = node.ChildNodes[i].Attributes["type"].Value.Split(',');
                            if (type.Length > 0 && type[0].Equals(PublicVariable.viewName))
                            {
                                if (node.ChildNodes[i].Attributes["name"] != null)
                                {
                                    documentDesignName = node.ChildNodes[i].Attributes["name"].Value;
                                    if (!documentDesignName.Equals(designName))//������ֲ���ͬ,������޸�
                                    {
                                        node.ChildNodes[i].Attributes["name"].Value = designName;
                                        for (int j = 0; j < node.ChildNodes[i].ChildNodes.Count; j++)  //�޸����Խڵ��е�����ֵ
                                        {
                                            if (node.ChildNodes[i].ChildNodes[j].Name.Equals("Property") && node.ChildNodes[i].ChildNodes[j].Attributes["name"] != null
                                                && node.ChildNodes[i].ChildNodes[j].Attributes["name"].Value.Equals("Name"))
                                            {
                                                node.ChildNodes[i].ChildNodes[j].InnerText = designName;
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            }//cassView�ڵ�
                            else
                            {
                                return null;
                            }
                        }//type != null

                    }//object
                }//�޸��������� 
                if (document != null)
                {
                    CodeDomHostLoader codeDomHostLoader = new CodeDomHostLoader(designName);

                    hostSurface.BeginLoad(codeDomHostLoader);
                    if (!codeDomHostLoader.MyPerformLoad(document)) //false(����ʧ��),����
                    {
                        return null;
                    }

                    hostSurface.Initialize(); //�Ѽ��غ�ſɽ��г�ʼ������
                    IDesignerHost host = (IDesignerHost)hostSurface.GetService(typeof(IDesignerHost));

                }//xml�ĵ���Ϊ��

                return new HostControl(hostSurface);
            }
            catch (Exception ex)
            {
            }
            return null;          
        }
    }//class
}
