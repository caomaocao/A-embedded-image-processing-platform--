/*******************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����
           ** �ļ�����XmlSerializerSectionHandle.cs 
           ** ����������
           **          ��Ҫ���ڴ������ýڵķ��ʡ�
           ** ���ߣ��ⵤ��
           ** ��ʼʱ�䣺2006-11-8
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
   /// ΪApp.config�е�XML�ļ����񣬶������ýڵ������
   /// </summary>
    public class XmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        //������ض�����ǰ���ýڵķ���
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {

                //XPathNavigator��ʹ���α�ģ�ʹ��������ݴ洢����ȡ���ݡ�
            XPathNavigator navigator = section.CreateNavigator();  
            string text = (string)navigator.Evaluate("string(@type)");
            Type type = Type.GetType(text);
            XmlSerializer serializer = new XmlSerializer(type);

            return serializer.Deserialize(new XmlNodeReader(section));
        }
    }
}
