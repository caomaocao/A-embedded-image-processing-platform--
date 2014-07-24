/*******************************************************************************
           ** Copyright (C) 2006 CASS 版权所有
           ** 文件名：XmlSerializerSectionHandle.cs 
           ** 功能描述：
           **          主要用于处理配置节的访问。
           ** 作者：吴丹红
           ** 创始时间：2006-11-8
           ** 
********************************************************************************/


using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Configuration;

namespace CaVeGen
{
   /// <summary>
   /// 为App.config中的XML文件服务，定义配置节点的类型
   /// </summary>
    public class XmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        //处理对特定的以前配置节的访问
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {

                //XPathNavigator：使用游标模型从所有数据存储区读取数据。
            XPathNavigator navigator = section.CreateNavigator();  
            string text = (string)navigator.Evaluate("string(@type)");
            Type type = Type.GetType(text);
            XmlSerializer serializer = new XmlSerializer(type);

            return serializer.Deserialize(new XmlNodeReader(section));
        }
    }
}
