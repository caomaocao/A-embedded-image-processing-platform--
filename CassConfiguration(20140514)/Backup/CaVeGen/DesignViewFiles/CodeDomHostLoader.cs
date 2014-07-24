/*******************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����
           ** �ļ�����CodeDomHostLoader.cs 
 * 
           ** ����������
           **         1�� �̳� CodeDomDesignerLoader�࣬��Ҫ���������������
            *          ���໹������CodeGen����������������������ռ䡢���͵ȣ�
 * 
 *                    2����ɱ���ҳ��ʹ�ҳ�湦�ܡ��ڱ��������ļ�ʱ��
 *                      �Կؼ������Խ����˹��ˣ�
 *                      �Ի�ȡ������ֵ�������͵�ת������ͼƬ�⣬����Ϊ�ַ���
 *                      ����ɫ���Խ����޸ģ��޸���ARGB��λ����ֵ��
 *                      ���������Ե�ֵ��ֻ��������������ƺʹ�С��
 *                      ���ڱ���ͼƬ��������ʱ�����Ƶ����ص��ֵ��
 * 
 *                    3����ҳ��ʱ���ȸ����������ɸ����͵Ŀؼ��������ݽڵ���Ϣ���ζԿؼ������Խ��и�ֵ��
 *                     ���Խڵ������ֵ���������ת���������������Ҫ�������
 * 
           ** ���ߣ��ⵤ�� 
           ** ��ʼʱ�䣺2007-3-20
           ** 
********************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Drawing;
using Microsoft.CSharp;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using CaVeGen.CommonOperation;
using System.Security;
using CaVeGen.DesignViewFiles;

namespace CaVeGen.DesignViewFiles
{
    /// <summary>
    /// �̳� CodeDomDesignerLoader�࣬CodeDomDesignerLoader�ṩ����ʵ�ֻ��� CodeDOM ����������س���Ļ��ࡣ
    ///  CodeCompileUnit:Ϊ CodeDOM ����ͼ���ṩ������ 
    /// 
    /// </summary>
    public class CodeDomHostLoader : CodeDomDesignerLoader
    {
        private CSharpCodeProvider csCodeProvider = new CSharpCodeProvider();
        private CodeCompileUnit codeCompileUnit = null;
        private TypeResolutionService typeResolutionService = null;
        private string currentForm = null;  //����ĵ�ǰҳ��
        private IDesignerLoaderHost hostLoader;
        private ArrayList readGroupList = new ArrayList();   //��ʾ����XML�ĵ�����Ͽؼ�������,ÿ��Ԫ��Ϊstring[]����
        private ArrayList readGroupInformation = new ArrayList();   //��ʾ�����xml�ĵ�����Ͽؼ�����Ϣ,ÿ��Ԫ��ΪGroupStruct�ṹ.����readGroupList�����е�����һһ��Ӧ
        public static readonly Attribute[] propertyAttributes = new Attribute[] {
			DesignOnlyAttribute.No
		};

        public CodeDomHostLoader()
        {
        }

        public CodeDomHostLoader(string formName)
        {
            typeResolutionService = new TypeResolutionService();
            currentForm = formName;
        }

        /// <summary>
        /// ��ȡҪ�����������س���һ��ʹ�õ����ͽ�������
        /// CodeDOM ���л������ڽ�������ʱ��ʹ�õ� ITypeResolutionService��
        /// �ڵ��� Initialize ����ʱ��CodeDomDesignerLoader �Զ����� ITypeResolutionService ��ӵ�����������
        /// ��Ȼ���ͽ������������������ǿ�ѡ�ģ������ڴ���������Ǳ���ģ�
        /// ��ΪԴ��������������ƣ����������������á�
        /// </summary>
        protected override ITypeResolutionService TypeResolutionService
        {
            get
            {
                return typeResolutionService;
            }
        }

        /// <summary>
        /// ��ȡ����������س���ʹ�õ� CodeDomProvider��
        /// </summary>
        protected override CodeDomProvider CodeDomProvider
        {
            get
            {
                return csCodeProvider;
            }
        }

        /// <summary>
        /// Parse�������ı����������ô洢�������� CodeCompileUnit��
        /// �� CodeDomDesignerLoader ��Ҫ����Դ����ʱ������� Parse ������
        /// ����ͨ��������ָ��Դ�����λ�ú͸�ʽ��
        /// ���������װ��Form�����Ե�ǰ��Ƹ��������������
        /// </summary>
        /// ����ֵ���ӷ����������ɵ� CodeCompileUnit��
        /// <returns></returns>
        protected override CodeCompileUnit Parse()
        {
            CodeCompileUnit compileUnit = null;
            DesignSurface designSurface = new DesignSurface();

            designSurface.BeginLoad(typeof(CassView));

            IDesignerHost designerHost = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
            designerHost.RootComponent.Site.Name = currentForm;

            compileUnit = GetCodeCompileUnit(designerHost);

            AssemblyName[] names = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            for (int i = 0; i < names.Length; i++)
            {
                Assembly assembly = Assembly.Load(names[i]);
                compileUnit.ReferencedAssemblies.Add(assembly.Location);
            }

            codeCompileUnit = compileUnit;
            return compileUnit;
        }

        /// <summary>
        /// Write ������ CodeCompileUnit ���浽���ô洢����
        /// �����ฺ�������Ӧ�ı���д���ϵ� ICodeGenerator �Ա�����롣
        /// CodeDomDesignerLoader ȷ�����ݵ� Write �� CodeDOM ������� Parse �������Ķ����ʵ����ͬ��
        /// �������л����̶Ըô�������˸��ġ�
        /// �������������Ż�����������س����ڴ���Ԫ�ص� UserData �����д洢�������ݡ�
        /// ���κ�û�б����л����������Ԫ�ص� Write �����У�������ʹ����Щ���ݡ�
        /// </summary>
        protected override void Write(CodeCompileUnit unit)
        {
            codeCompileUnit = unit;
        }

        /// <summary>
        /// ��View����ӵ�CodeCompileUnit�У�CodeCompileUnit��Ϊ CodeDOM ����ͼ���ṩ������
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public CodeCompileUnit GetCodeCompileUnit(IDesignerHost host)
        {           
            CodeNamespace nameSpace = new CodeNamespace();      //���������ռ�
            CodeTypeDeclaration myDesignerClass = new CodeTypeDeclaration();        //��������
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            IDesignerHost iDesignHost = (IDesignerHost)host.GetService(typeof(IDesignerHost));
            IComponent root = iDesignHost.RootComponent;        //��ȡ��ǰ������ĸ�����
            myDesignerClass = new CodeTypeDeclaration(root.Site.Name);      //��ָ�������Ƴ�ʼ�� CodeTypeDeclaration �����ʵ����
            myDesignerClass.BaseTypes.Add(typeof(CassView).FullName);       //�����Ա��ӵ���
            nameSpace.Types.Add(myDesignerClass);
            compileUnit.Namespaces.Add(nameSpace);
            return compileUnit;
        }


        /// <summary>
        /// ����ҳ���ʱ����ã������xml�ļ���ȡ��Ϣ
        /// ���ú���ReadFile���������ļ��ڵ�ľ�����Ϣ�������ݶ�ȡ����Ϣ����������ļ��ء�
        /// </summary>
        /// <param name="document">��Ҫ���ص�xml�ļ�</param>
        /// <returns></returns>
        public bool MyPerformLoad(XmlDocument document)
        {  
            ArrayList errors = new ArrayList();
            bool successful = true;        //��־���ع����Ƿ�ɹ�
            string baseClassName = null;
            this.hostLoader = this.LoaderHost;  //��ȡ���س������������� BasicDesignerLoader �̳С���

            if (hostLoader == null)
            {
                //CassMessageBox.Warning("����������쳣��");
                return false;               
            }

            //��Ŵ�����Ϣ,��������ڼ�������������뽫��Щ������Ϊ�쳣���ϴ��ݵ� errorCollection �����С�          
            baseClassName = ReadFile(document, errors);

            if (baseClassName == null)
            {
                return false;
            }

            if (errors.Count > 0)
            {
                successful = false;
            }

            // ������������ز�����
            //baseClassName �������������Ƶ��ĵ��Ļ������ȫ�޶����� 
            //successful ���������ѳɹ����أ���Ϊ true������Ϊ false��
            //errorCollection ���������ڼ������Ĵ�������еĻ����ļ��ϡ�
            hostLoader.EndLoad(baseClassName, successful, errors);
            return successful;
        }

        #region Helper methods �Ƿ��޸ĺͱ����,�Լ�����ת������
        /// <summary>
        ///���ظ�ת�����Ƿ����ʹ��ָ���������Ľ��������͵Ķ���ת��Ϊ��ת���������͡�
        /// �������ת����֧�ָ����͵�ת��,�򷵻�true
        /// </summary>
        private bool GetConversionSupported(TypeConverter converter, Type conversionType)
        {
            return (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType));
        }
        #endregion

        #region Serialize - Flush  дxml�ļ�����

        /// <summary>
        /// д����"Object",�ؼ��Ļ�����Ϣ
        /// ��ȡÿ���ؼ������ƣ�����PropertyConfig.xml�е��������ݣ��Կؼ������Խ��й��ˣ��õ������ļ��еĸ������Ժ���ӵ������ļ��Ľڵ��У�
        /// ��ȡCass���ԺͶ������õ�����ֵ����ӵ������ļ��С�
        /// 
        /// �ú����д��ڵݹ���á�
        /// </summary>
        /// <param name="document">XmlDocument�ṹ</param>
        /// <param name="value">Ҫ����Ķ���</param>
        /// <param name="designerPath">�������·��</param>
        /// <returns>ҳ�����Խڵ�</returns>
        private XmlNode WriteObject(XmlDocument document, object value, string designerPath)
        {
             int index = 0;    //��Ϊ��ʱ����ʹ��
             XmlNode node = document.CreateElement("Object");

             try
             {
                 //XmlDocument.CreateElement (String)  ��������ָ�����Ƶ�Ԫ�ء� XmlElement��ʾһ��Ԫ�ء�
                 //Ԫ��Ϊ��W3C �ĵ�����ģ�͡�(DOM) ����õĽڵ�֮һ��Ԫ�ؿ��Ծ�����֮���������ԡ�
                 //XmlElement �����������ڷ������Եķ�����GetAttribute��SetAttribute��RemoveAttribute��GetAttributeNode �ȣ���
                 //Ҳ��ʹ�� Attributes ���ԣ����᷵��һ�� XmlAttributeCollection��ʹ���ܹ������ƻ��������ʼ����е����ԡ�
                 //��������ָ�� Name �� XmlAttribute��XmlAttribute��ʾһ�����ԡ������Ե���Чֵ��Ĭ��ֵ���ĵ����Ͷ��� (DTD) ��ܹ��н��ж��塣
                 XmlAttribute typeAttr = document.CreateAttribute("type");

                 //Type.AssemblyQualifiedName ���Ի�ȡ Type �ĳ����޶������������м��� Type �ĳ��򼯵����ơ�
                 typeAttr.Value = value.GetType().AssemblyQualifiedName;
                 node.Attributes.Append(typeAttr);

                 if ((Control)value != null && ((Control)value).Site != null 
                     && ((Control)value).Site.Name.ToString() != null)   //д"name"����
                 {
                     XmlAttribute nameAttr = document.CreateAttribute("name");
                     nameAttr.Value = ((Control)value).Site.Name.ToString();
                     node.Attributes.Append(nameAttr);
                 }

                 if ((IComponent)value != null)
                 {
                     //PropertyDescriptorCollection �� ��ʾ PropertyDescriptor ����ļ��ϡ�PropertyDescriptor �� �ṩ���ϵ����Եĳ��󻯡�
                     //TypeDescriptor �� �ṩ�й�������� (Attribute) ����Ϣ������������� (Attribute)������ (Property) ���¼���
                     //TypeDescriptor.GetProperties ������������͵����� (Property) �ļ��ϡ� 
                     PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, propertyAttributes);  //���Attribute����
                     PropertyDescriptor controlProp = properties["Controls"];
                     if (controlProp != null)
                     {
                         PropertyDescriptor[] propArray = new PropertyDescriptor[properties.Count - 1];
                         index = 0;

                         foreach (PropertyDescriptor propDescriptor in properties)
                         {
                             if (propDescriptor != controlProp)
                             {
                                 propArray[index++] = propDescriptor;
                             }
                         }
                         properties = new PropertyDescriptorCollection(propArray);
                     }

                     XmlDocument xmlDocument = new XmlDocument();

                     ArrayList propList = new ArrayList();   //�����Ҫ�����Ե�����

                     //��ӿؼ�����������ڷ���"��������"�л��
                     PropertyDescriptorCollection cassPropertyDescriptors =
                         TypeDescriptor.GetProperties(value, new Attribute[] {
                             new CategoryAttribute(PublicVariable.ControlCategoryName)});

                     if (cassPropertyDescriptors != null || cassPropertyDescriptors.Count > 0)
                     {
                         foreach (PropertyDescriptor propertyDescriptor in cassPropertyDescriptors)
                         {
                             propList.Add(propertyDescriptor);
                         }
                     }
                     if (propList.Count > 0)
                     {
                         PropertyDescriptor[] propArray = new PropertyDescriptor[propList.Count];
                         for (index = 0; index < propList.Count; index++)
                         {
                             propArray[index] = (PropertyDescriptor)propList[index];
                         }
                         properties = new PropertyDescriptorCollection(propArray);
                     }
                     //����д���Ժ���WriteProperties
                     WriteProperties(document, properties, value, node, "Property");
                 }

                 if (((Control)value).Controls.Count > 0)
                 {
                     foreach (Control child in ((Control)value).Controls)
                     {
                         if (child.Site != null)        //��ȡ�ӿؼ���Ϣ 
                         {
                             node.AppendChild(WriteObject(document, child, designerPath));
                         }
                     }
                 }
             }
             catch (Exception ex) { }
             return node;
        }

        /// <summary>
        /// д����"Property"
        /// ��ÿؼ������ԣ�������ɫ������ֵ�����޸ģ������޸�ΪARGB��ʽ��
        /// ����������������޸ģ�ֻ������������ƺʹ�С
        /// </summary>
        /// <param name="document">XmlDocument�ṹ</param>
        /// <param name="properties">Ҫ���������Լ���</param>
        /// <param name="value">�ؼ�����</param>
        /// <param name="parent">���ڵ�</param>
        /// <param name="elementName">�ַ���"Property",������xml�ļ�ʱ��Ϊ�ڵ�����</param>
        public void WriteProperties(XmlDocument document, PropertyDescriptorCollection properties, object value, XmlNode parent, string elementName)
        {
            bool colorFlag = false;   //��־��ǰ����ֵ�Ƿ�Ϊ��ɫ

            try
            {
                //�����Լ����е�ÿ�����Զ��������²���
                foreach (PropertyDescriptor propDescriptor in properties)
                {
                    //PropertyDescriptor.ShouldSerializeValue ����,�����������б���дʱ��ȷ��һ��ֵ����ֵָʾ�Ƿ���Ҫ���ñ�������Ե�ֵ��
                    //ͨ����ͨ������ʵ�ִ˷�����
                    //�����������б���дʱ��������Եĵ�ǰֵ����Ĭ��ֵ��ͬ����˷������� true����ͨ���Ȳ��� DefaultValueAttribute ������Ĭ��ֵ��
                    //����˷����ҵ������� (Attribute)������������ (Attribute) ��ֵ������ (Property) �ĵ�ǰֵ���бȽϡ�
                    //����˷����Ҳ��� DefaultValueAttribute��������������Ҫʵ�ֵġ�ShouldSerializeMyProperty��������
                    //����ҵ��˷������� ShouldSerializeValue ����������
                    //����˷����Ҳ��� DefaultValueAttribute ��ShouldSerializeMyProperty���������������޷������Ż������ҷ��� true��
                    if (propDescriptor.ShouldSerializeValue(value) || (value.GetType().FullName.Equals(PublicVariable.viewName)))
                    {
                        string compName = parent.Name;   //��õ�ǰxml�ڵ�ĸ��ڵ�����
                        XmlNode node = document.CreateElement(elementName);
                        XmlAttribute attribute = document.CreateAttribute("name");

                        attribute.Value = propDescriptor.Name;
                        node.Attributes.Append(attribute);

                        //������ֵ��������ͳһת������Color��ֵͳһת����RGB��ʽ��
                        object propValue = propDescriptor.GetValue(value);
                        if (propValue != null && propValue.GetType().FullName.Equals("System.Drawing.Color"))
                        {
                            colorFlag = true;
                            Color myColor = (Color)propValue;
                            Color newColor = System.Drawing.Color.FromArgb(myColor.A, myColor.R, myColor.G, myColor.B);
                            propValue = newColor;
                        }
                        else if (propDescriptor.Name == "Font") //��Font��ֵͳһת����Լ������ʽ,ֻ�ṩ��������ƺʹ�С
                        {
                            Font myFont = (Font)propValue;
                            Font newFont = new Font(myFont.Name, myFont.Size);
                            propValue = newFont;
                        }

                        //DesignerSerializationVisibilityAttribute��,ָ�������ʱ���л�����ϵ����� (Property) ʱ��ʹ�õĳ־������͡�
                        //MemberDescriptor.Attributes ����,��ȡ�ó�Ա������ (Attribute) ���ϡ�
                        DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)propDescriptor.Attributes[typeof(DesignerSerializationVisibilityAttribute)];

                        //DesignerSerializationVisibilityAttribute.Visible ָ��Ӧ���������л��������л����� (Property) ��ֵ��
                        //DesignerSerializationVisibility ö��:
                        //Content ���������������������ݵĴ��룬�����Ƕ�����Ĵ��롣  
                        //Hidden ���������������ɶ���Ĵ��롣  
                        //Visible �������������ɶ���Ĵ��롣
                        if (visibility.Visibility == DesignerSerializationVisibility.Visible)
                        {
                            if (!propDescriptor.IsReadOnly && WriteValue(document, propValue, node, colorFlag))
                            {
                                parent.AppendChild(node);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// дֵ�����Եľ������
        /// ������ֵת��Ϊ�ַ��ͣ�������ͼƬ�Ķ������ļ����⣩
        /// </summary>
        /// <param name="document">XmlDocument�ṹ</param>
        /// <param name="value">ֵ�����Եľ������</param>
        /// <param name="parent">���ڵ�</param>
        /// <param name="colorFlag">��־��ǰ�Ƿ�����ɫ���ԣ������ɫ���Ե�ֵȱ����һλ������λARGB�������һλ255</param>
        /// <returns>��ֵ�Ƿ��ܹ���ת������Ҫ������</returns>
        private bool WriteValue(XmlDocument document, object value, XmlNode parent, bool colorFlag)
        {
            if (value == null)  //����ֵΪ��ʱ,����Ҫת������ֵ����
            {
                return true;
            }

            try
            {
            //  TypeConverter�ṩһ�ֽ�ֵ������ת��Ϊ���������Լ����ʱ�׼ֵ�������Ե�ͳһ������
            //  TypeDescriptor.GetConverter (Object) ����ָ��������͵�����ת������ 
                TypeConverter converter = TypeDescriptor.GetConverter(value);  //���ܻ�����쳣

                if (GetConversionSupported(converter, typeof(string)))
                {
                    //XmlNode.InnerText ��ȡ�����ýڵ㼰�������ӽڵ�Ĵ���ֵ�����ô����Խ��þ��������ĸ����ַ��������滻�����ӽڵ㡣����Ҷ�ڵ㣬InnerText �� Value ���Է�����ͬ�����ݡ� 
                    //TypeConverter.ConvertTo ���� (ITypeDescriptorContext, CultureInfo, Object, Type)ʹ��ָ���������ĺ���������Ϣ��������ֵ����ת��Ϊָ��������.
                    //  context ITypeDescriptorContext���ṩ��ʽ�����ġ�
                    //  culture CultureInfo��������� �����ã��� Visual Basic ��Ϊ Nothing��������õ�ǰ�����ԡ� 
                    //  value Ҫת���� Object��
                    //  destinationType value����Ҫת������ Type��
                    //CultureInfo�ṩ�й��ض������Ե���Ϣ���������Ե����ơ���дϵͳ��ʹ�õ��������Լ�����������ں������ַ����ĸ�ʽ��
                    
                    string valueString = (string)converter.ConvertTo(value, typeof(string));
                    if (colorFlag == true && valueString != null) //��ɫ����
                    {
                        string[] valueSplit = valueString.Split(',');
                        if (converter.GetType().FullName.Equals("System.Drawing.ColorConverter") && valueSplit.Length == 3)
                        {
                            valueString = "255, " + valueString;   //��λ��ʱ�����һλ����ֵ��255
                        }
                        colorFlag = false;
                    }
                    parent.InnerText = valueString;
                }
                else if (GetConversionSupported(converter, typeof(byte[])))
                {
                    byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(byte[]));
                    parent.AppendChild(WriteBinary(document, data));
                }
                else if (value.GetType().IsSerializable)
                {                   
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream();
                    formatter.Serialize(stream, value);
                    XmlNode binaryNode = WriteBinary(document, stream.ToArray());
                    parent.AppendChild(binaryNode);
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception ex) { }

            return true;
        }

        /// <summary>
        /// д���������ļ�.��:�ڶ�д����ͼƬʱ��Ҫ�ò���
        /// </summary>
        /// <param name="document">���ڵ�xml�ĵ�</param>
        /// <param name="value">ͼƬ���ֽ���</param>
        /// <returns></returns>
        private XmlNode WriteBinary(XmlDocument document, byte[] value)
        {
            XmlNode node = document.CreateElement("Binary");

            try
            {
                node.InnerText = Convert.ToBase64String(value);
            }
            catch(Exception ex)
            {
            }
             return node;         
        }

        #endregion

        #region DeSerialize - Load ��xml�ļ����з����л�����

        /// <summary>
        /// ��������ȡ��XML�ļ��ṹ��
        /// �������ĵ��еĿؼ�������Ϣ���д������ؼ�������ӵ�����combineObject��
        /// ���ú���ReadObject���������Ŀؼ��������Ե�ֵ��
        /// </summary>
        /// <param name="document">��Ҫ���صģ�xml�ļ�</param>
        /// <param name="errors">�����ڼ������Ĵ�������еĻ����ļ���</param>
        /// <returns>�������������Ƶ��ĵ��Ļ������ȫ�޶���</returns>
        private string ReadFile(XmlDocument document, ArrayList errors)
        {
            string baseClass = null;
            XmlNode node = null;

            try
            {
                for (int i = 0; i < document.DocumentElement.ChildNodes.Count; i++)  //xml���ڵ�
                {
                    node = document.DocumentElement.ChildNodes[i];
                    switch (node.Name)
                    {
                        case "Object":
                            {
                                if (baseClass == null)
                                {
                                    baseClass = node.Attributes["name"].Value;
                                }
                                ReadObject(node, errors);//����ReadObject��������ȡ"Object"��ǩ��������Ƶ�ʵ��:Object����
                            }
                            break;
                    }//switch
                }//end for
            }
            catch (Exception ex)
            {
                //CassMessageBox.Error("������Ч��");
                errors.Add(ex.Message);
            }

            return baseClass;

        }

        /// <summary>
        /// ��ȡ"Object"��ǩ��������Ƶ�ʵ��:Object����
        /// �������ͺ�����������Ӧ�Ŀؼ���
        /// �������ļ��пؼ�������Property�ڵ㣬����ú���ReadProperty���Կؼ�����Ӧ���Խ���ֵ���趨��
        /// �ú������ݹ���á�
        /// </summary>
        /// <param name="node">���ڵ�</param>
        /// <param name="errors">�����ڼ������Ĵ�������еĻ����ļ���</param>
        /// <returns>�ؼ�����</returns>
        private object ReadObject(XmlNode node, ArrayList errors)
        {
            //XmlNode.Attributes ��ȡһ�� XmlAttributeCollection���������ýڵ�����ԡ� 
            //�����ǻ��"type"������
            XmlAttribute typeAttr = node.Attributes["type"];
            object instance = null;

            if (typeAttr == null)
            {
                errors.Add("û��type�ڵ�");
                return null;
            }

            try
            {
                Type type = Type.GetType(typeAttr.Value);
                if (type == null)       //û���ҵ�ƥ���
                {
                    errors.Add(string.Format("type�ڵ������Ϊ��.", typeAttr.Value));
                    return null;
                }

                XmlAttribute nameAttr = node.Attributes["name"];

                if (typeof(IComponent).IsAssignableFrom(type))  //ȷ����ǰ�� Type ��ʵ���Ƿ���Դ�ָ�� Type ��ʵ�����䡣
                {
                    if (nameAttr == null)
                    {
                        instance = hostLoader.CreateComponent(type);
                    }
                    else if (nameAttr.Value.Equals(currentForm))
                    {
                        instance = hostLoader.RootComponent;   //��ǰ����ΪCassView,��Ϊ�������
                    }
                    else
                    {
                        instance = hostLoader.CreateComponent(type, nameAttr.Value);
                    }
                }
                else
                {
                    instance = Activator.CreateInstance(type);
                }
                IList childList = null;

                //ͨ����ָ�������� (Attribute) ��������ɸѡ����ʹ���Զ�������˵����������ָ����������� (Property) �ļ���
                //PropertyDescriptor childProp = TypeDescriptor.GetProperties(instance)[childAttr.Value];
                PropertyDescriptor childProp = TypeDescriptor.GetProperties(instance)["Controls"];

                if (childProp == null)
                {
                    errors.Add(string.Format(" ��ʵ����������Control����"));
                }
                else
                {
                    //PropertyDescriptor.GetValue ����,��ȡ����ϵ����Եĵ�ǰֵ,ͨ����ͨ������ʵ�ִ˷���
                    childList = childProp.GetValue(instance) as IList;
                    if (childList == null)
                    {
                        errors.Add(string.Format("���� {0} ���ҵ�����������Ч��������", childProp.Name));
                    }
                }

                //���ڿ�ʼ������Ԫ��
                object childInstance = null;
                ArrayList combineList = new ArrayList();    //��϶���
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name.Equals("Object"))
                    {
                        if (childList != null)
                        {
                            childInstance = ReadObject(childNode, errors);
                            if(childInstance != null)
                            {
                                combineList.Add(childInstance);
                                childList.Add(childInstance);
                            }
                        }
                    }
                    else if (childNode.Name.Equals("Property"))
                    {
                        ReadProperty(childNode, instance, errors);
                    }
                }         
            }
            catch (Exception ex)
            {
                //CassMessageBox.Error("��������ʧ�ܣ�");
                errors.Add(ex.Message);
            }
            return instance;
        }

        /// <summary>
        /// ��ȡ"Property"�ڵ㣬
        /// ���ݿؼ����������������ϣ������ݺ���ReadValue������������ֵ�����пؼ����Եĸ�ֵ������
        /// </summary>
        /// <param name="node">���ڵ�</param>
        /// <param name="instance">�ؼ�����</param>
        /// <param name="errors">�����ڼ������Ĵ�������еĻ����ļ���</param>
        private void ReadProperty(XmlNode node, object instance, ArrayList errors)
        {
            //XmlNode.Attributes ��ȡһ�� XmlAttributeCollection���������ýڵ�����ԡ� �����ǻ��"name"������
            XmlAttribute nameAttr = node.Attributes["name"];

            if (nameAttr != null && nameAttr.Value == "FunctionPropertyValue")
            {
                nameAttr.InnerText = "FunctionPropertyValue" + "";
            }

            if (nameAttr == null)
            {
                errors.Add("����û�����ƣ�");
                return;
            }
            try
            {
                //TypeDescriptor.GetProperties ������������͵����� (Property) �ļ��ϡ� 
                PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(instance)[nameAttr.Value];

                if (propDescriptor == null)
                {
                    errors.Add(string.Format("����{0} ���������� {1}��", nameAttr.Value, instance.GetType().FullName));
                    return;
                }

                object value = null;

                if (ReadValue(node, propDescriptor.Converter, errors, ref value) && value != null)
                {
                    //PropertyDescriptor.SetValue �����������б���дʱ���������ֵ����Ϊһ����ͬ��ֵ��
                    propDescriptor.SetValue(instance, value);
                }
            }
            catch (NotSupportedException e)
            {
                //CassMessageBox.Error("��ȡ��������ʧ�ܣ�");
                errors.Add(e.Message);
            }
        }

        /// <summary>
        /// ��ȡ"Value"�ڵ�
        /// �������ԣ��������ĵ��е�����ֵ�������͵�ת����ת��Ϊ��ǰ���Ե�ֵ����
        /// </summary>
        /// <param name="node">���ڵ�</param>
        /// <param name="converter">���͵�ת����</param>
        /// <param name="errors">�����ڼ������Ĵ�������еĻ����ļ���</param>
        /// <param name="value">����</param>
        /// <returns>��ֵ�Ƿ��ܹ���ת������Ҫ������</returns>
        private bool ReadValue(XmlNode node, TypeConverter converter, ArrayList errors, ref object value)
        {
            try
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    //XmlNodeTypeö��ָ���ڵ�����͡�.Text:�ڵ���ı����ݡ�Text �ڵ㲻�ܾ����κ��ӽڵ㡣
                    if (child.NodeType == XmlNodeType.Text)
                    {
                        value = converter.ConvertFromInvariantString(node.InnerText);
                        return true;
                    }
                    else if (child.Name.Equals("Binary"))
                    {
                        byte[] data = Convert.FromBase64String(child.InnerText);

                        if (GetConversionSupported(converter, typeof(byte[])))
                        {
                            value = converter.ConvertFrom(data);
                            return true;
                        }
                        else
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            MemoryStream stream = new MemoryStream(data);

                            value = formatter.Deserialize(stream);
                            return true;
                        }
                    }
                   
                    else
                    {
                        errors.Add(string.Format("��������{0}", child.Name));
                        value = null;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                value = null;
                return false;
            }
        }

        #endregion


        /// <summary>
        /// ���浥��xml�ļ�
        /// </summary>
        /// <param name="document">XmlDocument�ṹ</param>
        /// <param name="savefilename">Ҫ������ļ�����</param>
        /// <param name="value">Ҫ����Ķ���</param>
        /// <param name="designerPath">�������·��</param>
        /// <param name = "savePath">�����xml��·��</param>>
        public void SaveToFile(XmlDocument document, string savefilename, object value, string designerPath, string savePath)
        {
            //��ʾ����Ի��������ļ���
            SaveFileDialog dialog = new SaveFileDialog();
            try
            {
                //����WriteObject��������֯XmlDocument�ṹ��,���ڵ������
                document.FirstChild.AppendChild(WriteObject(document, value, designerPath));
                document.Save(savePath);    //����֯�õ�XmlDocument�ṹ�屣�浽�ļ���               
            }
            catch (Exception ex) { }
            finally
            {
                dialog.Dispose();
            }
        }

        /// <summary>
        /// �������̵ĸ��������ļ������浽��Ӧ�Ĵ����ļ��ϣ�һ�����ҳ�����һ�θú���
        /// </summary>
        /// <param name="document">XmlDocument�ṹ</param>
        /// <param name="savefilename">Ҫ������ļ�����</param>
        /// <param name="value">Ҫ����Ķ���</param>
        /// <param name="designerPath">�������·��</param>
        public void SaveProjectToFile(XmlDocument document, string savefilename, object value, string designerPath)
        {
            try
            {
                //����WriteObject��������֯XmlDocument�ṹ��,���ڵ������
                document.FirstChild.AppendChild(WriteObject(document, value, designerPath));
                document.Save(savefilename + ".xml");   //����֯�õ�XmlDocument�ṹ�屣�浽�ļ���
            }
            catch (Exception ex) { }
        }






    }// class
}// namespace
