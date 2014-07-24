/************************************************************************************
           ** Copyright (C) 2006 CASS 版权所有
           ** 文件名：HostDesign.cs 
           ** 功能描述：
           **          主要用来管理设计器中多个页面，每个页面都为带有HostDesign类的
           **          自定义控件HostControl,继承DesignSurfaceManager类。
 * 
                     * 该类实现了新建设计页面和打开设计页面时根据设计器页面名称创建HostControl类型变量的函数。
                     * 在打开已有的描述文件时，先对描述文件的格式进行判断是否为约定的xml格式文档，并且对控件叠加信息进添加到设计器的特殊处理
 * 
           ** 作者：吴丹红
           ** 创始时间：2006-11-11
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
    ///管理多个 HostDesign. 任何在 HostDesigneManager
    ///中添加的服务都能在HostDesign中实现。
    /// </summary>
    public class HostDesignManage : DesignSurfaceManager
    {

        public HostDesignManage()
            : base()
        {
            //
        }

        /// <summary>
        /// 重写了创建设计图面的实例。
        /// </summary>
        /// <param name="parentProvider">服务对象</param>
        /// <returns>包含了服务的HostDesign类型设计器对象</returns>
        protected override DesignSurface CreateDesignSurfaceCore(IServiceProvider parentProvider) 
        {
            return new HostDesign(parentProvider);
        }

        /// <summary>
        /// 生成一个新的HostDesign,并且进行加载。
        /// </summary>
        /// <param name="formName">所创建的设计页面的名称</param>
        /// <returns> 返回一个加载了设计器的HostControl对象</returns>
        public HostControl GetNewHost(string formName)
        {
            try
            {
                HostDesign hostSurface = (HostDesign)this.CreateDesignSurface(this.ServiceContainer);
                CodeDomHostLoader codeDomHostLoader = new CodeDomHostLoader(formName);

                hostSurface.BeginLoad(codeDomHostLoader);   //使用给定的设计器加载程序开始加载过程。
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
        /// 读入新的Xml文件时创建新的设计器，
        /// 对读入的xml文档进行处理，如果xml文档的名称和设计器Form的名称不一致，则将设计器的名称设为和xml文档的名称一致。
        /// 同时，判断输入的xml文档的格式是否为正确的描述文件；
        /// 对于描述文件中有控件叠加的信息，则将需叠加的控件的信息添加到设计器中。
        /// </summary>
        /// <param name="fileName">要读入的xml文件的名称/param>
        /// <returns></returns>
        public HostControl GetNewDesignHost(string fileName)
        {
            HostDesign hostSurface = (HostDesign)this.CreateDesignSurface(this.ServiceContainer);

            //判断文件名称和文件中的CassView对象是否一致
            string designName = Path.GetFileNameWithoutExtension(fileName);
            string documentDesignName = designName;

            XmlDocument document = new XmlDocument();

            try
            {
                document.Load(fileName);
                XmlNode node = document.FirstChild;
                for (int i = 0; i < node.ChildNodes.Count; i++) // 对读入的xml文档进行处理，如果xml文档的名称和设计器Form的名称不一致，则将设计器的名称设为和xml文档的名称一致。
                {
                    if (node.ChildNodes[i].Name.Equals("Object"))
                    {
                        if (node.ChildNodes[i].Attributes["type"] == null)
                        {
                            //CassMessageBox.Warning("描述文件格式错误！");
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
                                    if (!documentDesignName.Equals(designName))//如果名字不相同,则进行修改
                                    {
                                        node.ChildNodes[i].Attributes["name"].Value = designName;
                                        for (int j = 0; j < node.ChildNodes[i].ChildNodes.Count; j++)  //修改属性节点中的名称值
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
                            }//cassView节点
                            else
                            {
                                return null;
                            }
                        }//type != null

                    }//object
                }//修改名称问题 
                if (document != null)
                {
                    CodeDomHostLoader codeDomHostLoader = new CodeDomHostLoader(designName);

                    hostSurface.BeginLoad(codeDomHostLoader);
                    if (!codeDomHostLoader.MyPerformLoad(document)) //false(加载失败),跳出
                    {
                        return null;
                    }

                    hostSurface.Initialize(); //已加载后才可进行初始化操作
                    IDesignerHost host = (IDesignerHost)hostSurface.GetService(typeof(IDesignerHost));

                }//xml文档不为空

                return new HostControl(hostSurface);
            }
            catch (Exception ex)
            {
            }
            return null;          
        }
    }//class
}
