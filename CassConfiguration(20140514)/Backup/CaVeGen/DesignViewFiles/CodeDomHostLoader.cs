/*******************************************************************************
           ** Copyright (C) 2006 CASS 版权所有
           ** 文件名：CodeDomHostLoader.cs 
 * 
           ** 功能描述：
           **         1、 继承 CodeDomDesignerLoader类，主要用来加载设计器，
            *          该类还调用了CodeGen类用来生成设计器的命名空间、类型等；
 * 
 *                    2、完成保存页面和打开页面功能。在保存描述文件时，
 *                      对控件的属性进行了过滤，
 *                      对获取的属性值进行类型的转换，除图片外，其他为字符型
 *                      对颜色属性进行修改，修改了ARGB四位数据值；
 *                      对字体属性的值，只保留了字体的名称和大小；
 *                      对于背景图片，则保留的时二进制的象素点的值；
 * 
 *                    3、打开页面时，先根据类型生成该类型的控件，并根据节点信息依次对控件的属性进行赋值。
 *                     　对节点的属性值需进行类型转换，将其符合属性要求的类型
 * 
           ** 作者：吴丹红 
           ** 创始时间：2007-3-20
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
    /// 继承 CodeDomDesignerLoader类，CodeDomDesignerLoader提供用于实现基于 CodeDOM 的设计器加载程序的基类。
    ///  CodeCompileUnit:为 CodeDOM 程序图形提供容器。 
    /// 
    /// </summary>
    public class CodeDomHostLoader : CodeDomDesignerLoader
    {
        private CSharpCodeProvider csCodeProvider = new CSharpCodeProvider();
        private CodeCompileUnit codeCompileUnit = null;
        private TypeResolutionService typeResolutionService = null;
        private string currentForm = null;  //读入的当前页面
        private IDesignerLoaderHost hostLoader;
        private ArrayList readGroupList = new ArrayList();   //表示读入XML文档的组合控件的名称,每个元素为string[]类型
        private ArrayList readGroupInformation = new ArrayList();   //表示读入的xml文档的组合控件的信息,每个元素为GroupStruct结构.即与readGroupList链表中的索引一一对应
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
        /// 获取要与此设计器加载程序一起使用的类型解析服务。
        /// CodeDOM 序列化程序在解析类型时将使用的 ITypeResolutionService。
        /// 在调用 Initialize 方法时，CodeDomDesignerLoader 自动将此 ITypeResolutionService 添加到服务容器。
        /// 虽然类型解析程序在许多情况下是可选的，但对于代码解释它是必需的，
        /// 因为源代码包含类型名称，但不包含程序集引用。
        /// </summary>
        protected override ITypeResolutionService TypeResolutionService
        {
            get
            {
                return typeResolutionService;
            }
        }

        /// <summary>
        /// 获取此设计器加载程序将使用的 CodeDomProvider。
        /// </summary>
        protected override CodeDomProvider CodeDomProvider
        {
            get
            {
                return csCodeProvider;
            }
        }

        /// <summary>
        /// Parse：分析文本或其他永久存储区并返回 CodeCompileUnit。
        /// 当 CodeDomDesignerLoader 需要分析源代码时，会调用 Parse 方法。
        /// 必须通过派生类指定源代码的位置和格式。
        /// 在设计器中装载Form，并对当前设计根组件进行命名。
        /// </summary>
        /// 返回值：从分析操作生成的 CodeCompileUnit。
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
        /// Write 方法将 CodeCompileUnit 保存到永久存储区。
        /// 派生类负责调用相应文本编写器上的 ICodeGenerator 以保存代码。
        /// CodeDomDesignerLoader 确保传递到 Write 的 CodeDOM 对象与从 Parse 检索到的对象的实例相同，
        /// 除非序列化进程对该代码进行了更改。
        /// 这样就允许经过优化的设计器加载程序在代码元素的 UserData 属性中存储其他数据。
        /// 在任何没有被序列化进程替代的元素的 Write 方法中，将可以使用这些数据。
        /// </summary>
        protected override void Write(CodeCompileUnit unit)
        {
            codeCompileUnit = unit;
        }

        /// <summary>
        /// 将View类添加到CodeCompileUnit中，CodeCompileUnit：为 CodeDOM 程序图形提供容器。
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public CodeCompileUnit GetCodeCompileUnit(IDesignerHost host)
        {           
            CodeNamespace nameSpace = new CodeNamespace();      //定义命名空间
            CodeTypeDeclaration myDesignerClass = new CodeTypeDeclaration();        //定义类型
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            IDesignerHost iDesignHost = (IDesignerHost)host.GetService(typeof(IDesignerHost));
            IComponent root = iDesignHost.RootComponent;        //获取当前设计器的根主键
            myDesignerClass = new CodeTypeDeclaration(root.Site.Name);      //用指定的名称初始化 CodeTypeDeclaration 类的新实例。
            myDesignerClass.BaseTypes.Add(typeof(CassView).FullName);       //将类成员添加到类
            nameSpace.Types.Add(myDesignerClass);
            compileUnit.Namespaces.Add(nameSpace);
            return compileUnit;
        }


        /// <summary>
        /// 载入页面的时候调用，负责从xml文件读取信息
        /// 调用函数ReadFile进行描述文件节点的具体信息处理，根据读取的信息进行设计器的加载。
        /// </summary>
        /// <param name="document">需要加载的xml文件</param>
        /// <returns></returns>
        public bool MyPerformLoad(XmlDocument document)
        {  
            ArrayList errors = new ArrayList();
            bool successful = true;        //标志加载过程是否成功
            string baseClassName = null;
            this.hostLoader = this.LoaderHost;  //获取加载程序宿主。（从 BasicDesignerLoader 继承。）

            if (hostLoader == null)
            {
                //CassMessageBox.Warning("设计器加载异常！");
                return false;               
            }

            //存放错误信息,如果加载期间遇到错误，则必须将这些错误作为异常集合传递到 errorCollection 参数中。          
            baseClassName = ReadFile(document, errors);

            if (baseClassName == null)
            {
                return false;
            }

            if (errors.Count > 0)
            {
                successful = false;
            }

            // 结束设计器加载操作。
            //baseClassName 此设计器正在设计的文档的基类的完全限定名。 
            //successful 如果设计器已成功加载，则为 true；否则为 false。
            //errorCollection 包含加载期间遇到的错误（如果有的话）的集合。
            hostLoader.EndLoad(baseClassName, successful, errors);
            return successful;
        }

        #region Helper methods 是否修改和保存过,以及类型转换函数
        /// <summary>
        ///返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型。
        /// 如果类型转换器支持该类型的转换,则返回true
        /// </summary>
        private bool GetConversionSupported(TypeConverter converter, Type conversionType)
        {
            return (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType));
        }
        #endregion

        #region Serialize - Flush  写xml文件操作

        /// <summary>
        /// 写对象"Object",控件的基本信息
        /// 获取每个控件的名称，根据PropertyConfig.xml中的描述内容，对控件的属性进行过滤，得到描述文件中的各个属性后，添加到描述文件的节点中；
        /// 获取Cass属性和动作设置的属性值，添加到描述文件中。
        /// 
        /// 该函数中存在递归调用。
        /// </summary>
        /// <param name="document">XmlDocument结构</param>
        /// <param name="value">要保存的对象</param>
        /// <param name="designerPath">设计器的路径</param>
        /// <returns>页面属性节点</returns>
        private XmlNode WriteObject(XmlDocument document, object value, string designerPath)
        {
             int index = 0;    //作为临时索引使用
             XmlNode node = document.CreateElement("Object");

             try
             {
                 //XmlDocument.CreateElement (String)  创建具有指定名称的元素。 XmlElement表示一个元素。
                 //元素为“W3C 文档对象模型”(DOM) 中最常用的节点之一。元素可以具有与之关联的属性。
                 //XmlElement 类具有许多用于访问属性的方法（GetAttribute、SetAttribute、RemoveAttribute、GetAttributeNode 等）。
                 //也可使用 Attributes 属性，它会返回一个 XmlAttributeCollection，使您能够按名称或索引访问集合中的属性。
                 //创建具有指定 Name 的 XmlAttribute。XmlAttribute表示一个属性。此属性的有效值和默认值在文档类型定义 (DTD) 或架构中进行定义。
                 XmlAttribute typeAttr = document.CreateAttribute("type");

                 //Type.AssemblyQualifiedName 属性获取 Type 的程序集限定名，包括从中加载 Type 的程序集的名称。
                 typeAttr.Value = value.GetType().AssemblyQualifiedName;
                 node.Attributes.Append(typeAttr);

                 if ((Control)value != null && ((Control)value).Site != null 
                     && ((Control)value).Site.Name.ToString() != null)   //写"name"属性
                 {
                     XmlAttribute nameAttr = document.CreateAttribute("name");
                     nameAttr.Value = ((Control)value).Site.Name.ToString();
                     node.Attributes.Append(nameAttr);
                 }

                 if ((IComponent)value != null)
                 {
                     //PropertyDescriptorCollection 类 表示 PropertyDescriptor 对象的集合。PropertyDescriptor 类 提供类上的属性的抽象化。
                     //TypeDescriptor 类 提供有关组件属性 (Attribute) 的信息，如组件的属性 (Attribute)、属性 (Property) 和事件。
                     //TypeDescriptor.GetProperties 返回组件或类型的属性 (Property) 的集合。 
                     PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, propertyAttributes);  //获得Attribute属性
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

                     ArrayList propList = new ArrayList();   //存放需要的属性的链表

                     //添加控件自身的属性在分类"基本属性"中获得
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
                     //调用写属性函数WriteProperties
                     WriteProperties(document, properties, value, node, "Property");
                 }

                 if (((Control)value).Controls.Count > 0)
                 {
                     foreach (Control child in ((Control)value).Controls)
                     {
                         if (child.Site != null)        //读取子控件信息 
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
        /// 写属性"Property"
        /// 获得控件的属性，并对颜色的属性值进行修改，将其修改为ARGB形式，
        /// 并对字体进行重新修改，只保留字体的名称和大小
        /// </summary>
        /// <param name="document">XmlDocument结构</param>
        /// <param name="properties">要描述的属性集合</param>
        /// <param name="value">控件对象</param>
        /// <param name="parent">父节点</param>
        /// <param name="elementName">字符串"Property",在生成xml文件时作为节点名称</param>
        public void WriteProperties(XmlDocument document, PropertyDescriptorCollection properties, object value, XmlNode parent, string elementName)
        {
            bool colorFlag = false;   //标志当前属性值是否为颜色

            try
            {
                //对属性集合中的每个属性都进行以下操作
                foreach (PropertyDescriptor propDescriptor in properties)
                {
                    //PropertyDescriptor.ShouldSerializeValue 方法,当在派生类中被重写时，确定一个值，该值指示是否需要永久保存此属性的值。
                    //通常，通过反射实现此方法。
                    //当在派生类中被重写时，如果属性的当前值与其默认值不同，则此方法返回 true。它通过先查找 DefaultValueAttribute 来查找默认值。
                    //如果此方法找到该属性 (Attribute)，它将该属性 (Attribute) 的值与属性 (Property) 的当前值进行比较。
                    //如果此方法找不到 DefaultValueAttribute，它将查找您需要实现的“ShouldSerializeMyProperty”方法。
                    //如果找到此方法，则 ShouldSerializeValue 将调用它。
                    //如果此方法找不到 DefaultValueAttribute 或“ShouldSerializeMyProperty”方法，则它将无法创建优化，并且返回 true。
                    if (propDescriptor.ShouldSerializeValue(value) || (value.GetType().FullName.Equals(PublicVariable.viewName)))
                    {
                        string compName = parent.Name;   //获得当前xml节点的父节点名称
                        XmlNode node = document.CreateElement(elementName);
                        XmlAttribute attribute = document.CreateAttribute("name");

                        attribute.Value = propDescriptor.Name;
                        node.Attributes.Append(attribute);

                        //对属性值进行类型统一转换，把Color的值统一转换成RGB形式。
                        object propValue = propDescriptor.GetValue(value);
                        if (propValue != null && propValue.GetType().FullName.Equals("System.Drawing.Color"))
                        {
                            colorFlag = true;
                            Color myColor = (Color)propValue;
                            Color newColor = System.Drawing.Color.FromArgb(myColor.A, myColor.R, myColor.G, myColor.B);
                            propValue = newColor;
                        }
                        else if (propDescriptor.Name == "Font") //把Font的值统一转换成约定的形式,只提供字体的名称和大小
                        {
                            Font myFont = (Font)propValue;
                            Font newFont = new Font(myFont.Name, myFont.Size);
                            propValue = newFont;
                        }

                        //DesignerSerializationVisibilityAttribute类,指定在设计时序列化组件上的属性 (Property) 时所使用的持久性类型。
                        //MemberDescriptor.Attributes 属性,获取该成员的属性 (Attribute) 集合。
                        DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)propDescriptor.Attributes[typeof(DesignerSerializationVisibilityAttribute)];

                        //DesignerSerializationVisibilityAttribute.Visible 指定应该允许序列化程序序列化属性 (Property) 的值。
                        //DesignerSerializationVisibility 枚举:
                        //Content 代码生成器产生对象内容的代码，而不是对象本身的代码。  
                        //Hidden 代码生成器不生成对象的代码。  
                        //Visible 代码生成器生成对象的代码。
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
        /// 写值，属性的具体参数
        /// 将属性值转化为字符型，（背景图片的二进制文件除外）
        /// </summary>
        /// <param name="document">XmlDocument结构</param>
        /// <param name="value">值，属性的具体参数</param>
        /// <param name="parent">父节点</param>
        /// <param name="colorFlag">标志当前是否是颜色属性，如果颜色属性的值缺少了一位，即三位ARGB，则添加一位255</param>
        /// <returns>该值是否能够被转化成需要的类型</returns>
        private bool WriteValue(XmlDocument document, object value, XmlNode parent, bool colorFlag)
        {
            if (value == null)  //属性值为空时,则不需要转换属性值类型
            {
                return true;
            }

            try
            {
            //  TypeConverter提供一种将值的类型转换为其他类型以及访问标准值和子属性的统一方法。
            //  TypeDescriptor.GetConverter (Object) 返回指定组件类型的类型转换器。 
                TypeConverter converter = TypeDescriptor.GetConverter(value);  //可能会产生异常

                if (GetConversionSupported(converter, typeof(string)))
                {
                    //XmlNode.InnerText 获取或设置节点及其所有子节点的串联值。设置此属性将用经过分析的给定字符串内容替换所有子节点。对于叶节点，InnerText 与 Value 属性返回相同的内容。 
                    //TypeConverter.ConvertTo 方法 (ITypeDescriptorContext, CultureInfo, Object, Type)使用指定的上下文和区域性信息将给定的值对象转换为指定的类型.
                    //  context ITypeDescriptorContext，提供格式上下文。
                    //  culture CultureInfo。如果传递 空引用（在 Visual Basic 中为 Nothing），则采用当前区域性。 
                    //  value 要转换的 Object。
                    //  destinationType value参数要转换到的 Type。
                    //CultureInfo提供有关特定区域性的信息（如区域性的名称、书写系统和使用的日历）以及如何设置日期和排序字符串的格式。
                    
                    string valueString = (string)converter.ConvertTo(value, typeof(string));
                    if (colorFlag == true && valueString != null) //颜色处理
                    {
                        string[] valueSplit = valueString.Split(',');
                        if (converter.GetType().FullName.Equals("System.Drawing.ColorConverter") && valueSplit.Length == 3)
                        {
                            valueString = "255, " + valueString;   //三位的时候添加一位Ａ的值是255
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
        /// 写二进制流文件.即:在读写背景图片时需要该操作
        /// </summary>
        /// <param name="document">所在的xml文档</param>
        /// <param name="value">图片的字节流</param>
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

        #region DeSerialize - Load 读xml文件进行反序列化操作

        /// <summary>
        /// 解析所读取的XML文件结构。
        /// 对描述文档中的控件叠加信息进行处理，将控件名称添加到数组combineObject中
        /// 调用函数ReadObject，处理具体的控件及其属性的值；
        /// </summary>
        /// <param name="document">需要加载的．xml文件</param>
        /// <param name="errors">加载期间遇到的错误（如果有的话）的集合</param>
        /// <returns>此设计器正在设计的文档的基类的完全限定名</returns>
        private string ReadFile(XmlDocument document, ArrayList errors)
        {
            string baseClass = null;
            XmlNode node = null;

            try
            {
                for (int i = 0; i < document.DocumentElement.ChildNodes.Count; i++)  //xml根节点
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
                                ReadObject(node, errors);//调用ReadObject函数，读取"Object"标签，返回设计的实例:Object对象
                            }
                            break;
                    }//switch
                }//end for
            }
            catch (Exception ex)
            {
                //CassMessageBox.Error("参数无效！");
                errors.Add(ex.Message);
            }

            return baseClass;

        }

        /// <summary>
        /// 读取"Object"标签，返回设计的实例:Object对象。
        /// 根据类型和名称生成相应的控件。
        /// 对描述文件中控件的属性Property节点，则调用函数ReadProperty，对控件的相应属性进行值的设定。
        /// 该函数被递归调用。
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="errors">加载期间遇到的错误（如果有的话）的集合</param>
        /// <returns>控件对象</returns>
        private object ReadObject(XmlNode node, ArrayList errors)
        {
            //XmlNode.Attributes 获取一个 XmlAttributeCollection，它包含该节点的属性。 
            //这里是获得"type"的属性
            XmlAttribute typeAttr = node.Attributes["type"];
            object instance = null;

            if (typeAttr == null)
            {
                errors.Add("没有type节点");
                return null;
            }

            try
            {
                Type type = Type.GetType(typeAttr.Value);
                if (type == null)       //没有找到匹配的
                {
                    errors.Add(string.Format("type节点的内容为空.", typeAttr.Value));
                    return null;
                }

                XmlAttribute nameAttr = node.Attributes["name"];

                if (typeof(IComponent).IsAssignableFrom(type))  //确定当前的 Type 的实例是否可以从指定 Type 的实例分配。
                {
                    if (nameAttr == null)
                    {
                        instance = hostLoader.CreateComponent(type);
                    }
                    else if (nameAttr.Value.Equals(currentForm))
                    {
                        instance = hostLoader.RootComponent;   //当前对象为CassView,即为根设计器
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

                //通过将指定的属性 (Attribute) 数组用作筛选器并使用自定义类型说明符来返回指定组件的属性 (Property) 的集合
                //PropertyDescriptor childProp = TypeDescriptor.GetProperties(instance)[childAttr.Value];
                PropertyDescriptor childProp = TypeDescriptor.GetProperties(instance)["Controls"];

                if (childProp == null)
                {
                    errors.Add(string.Format(" 该实例并不包含Control属性"));
                }
                else
                {
                    //PropertyDescriptor.GetValue 方法,获取组件上的属性的当前值,通常，通过反射实现此方法
                    childList = childProp.GetValue(instance) as IList;
                    if (childList == null)
                    {
                        errors.Add(string.Format("属性 {0} 已找到但并不是有效属性链表", childProp.Name));
                    }
                }

                //现在开始遍历子元素
                object childInstance = null;
                ArrayList combineList = new ArrayList();    //组合队列
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
                //CassMessageBox.Error("创建对象失败！");
                errors.Add(ex.Message);
            }
            return instance;
        }

        /// <summary>
        /// 读取"Property"节点，
        /// 根据控件的属性描述符集合，并根据函数ReadValue或许具体的属性值，进行控件属性的赋值操作。
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="instance">控件对象</param>
        /// <param name="errors">加载期间遇到的错误（如果有的话）的集合</param>
        private void ReadProperty(XmlNode node, object instance, ArrayList errors)
        {
            //XmlNode.Attributes 获取一个 XmlAttributeCollection，它包含该节点的属性。 这里是获得"name"的属性
            XmlAttribute nameAttr = node.Attributes["name"];

            if (nameAttr != null && nameAttr.Value == "FunctionPropertyValue")
            {
                nameAttr.InnerText = "FunctionPropertyValue" + "";
            }

            if (nameAttr == null)
            {
                errors.Add("属性没有名称！");
                return;
            }
            try
            {
                //TypeDescriptor.GetProperties 返回组件或类型的属性 (Property) 的集合。 
                PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(instance)[nameAttr.Value];

                if (propDescriptor == null)
                {
                    errors.Add(string.Format("属性{0} 并不存在于 {1}中", nameAttr.Value, instance.GetType().FullName));
                    return;
                }

                object value = null;

                if (ReadValue(node, propDescriptor.Converter, errors, ref value) && value != null)
                {
                    //PropertyDescriptor.SetValue 当在派生类中被重写时，将组件的值设置为一个不同的值。
                    propDescriptor.SetValue(instance, value);
                }
            }
            catch (NotSupportedException e)
            {
                //CassMessageBox.Error("获取属性描述失败！");
                errors.Add(e.Message);
            }
        }

        /// <summary>
        /// 读取"Value"节点
        /// 根据属性，将描述文档中的属性值进行类型的转换，转换为当前属性的值类型
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="converter">类型的转换器</param>
        /// <param name="errors">加载期间遇到的错误（如果有的话）的集合</param>
        /// <param name="value">对象</param>
        /// <returns>该值是否能够被转化成需要的类型</returns>
        private bool ReadValue(XmlNode node, TypeConverter converter, ArrayList errors, ref object value)
        {
            try
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    //XmlNodeType枚举指定节点的类型。.Text:节点的文本内容。Text 节点不能具有任何子节点。
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
                        errors.Add(string.Format("意外类型{0}", child.Name));
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
        /// 保存单个xml文件
        /// </summary>
        /// <param name="document">XmlDocument结构</param>
        /// <param name="savefilename">要保存的文件名字</param>
        /// <param name="value">要保存的对象</param>
        /// <param name="designerPath">设计器的路径</param>
        /// <param name = "savePath">保存的xml的路径</param>>
        public void SaveToFile(XmlDocument document, string savefilename, object value, string designerPath, string savePath)
        {
            //显示保存对话框，设置文件名
            SaveFileDialog dialog = new SaveFileDialog();
            try
            {
                //调用WriteObject函数来组织XmlDocument结构体,根节点下添加
                document.FirstChild.AppendChild(WriteObject(document, value, designerPath));
                document.Save(savePath);    //把组织好的XmlDocument结构体保存到文件中               
            }
            catch (Exception ex) { }
            finally
            {
                dialog.Dispose();
            }
        }

        /// <summary>
        /// 整个工程的各个描述文件到保存到相应的磁盘文件上，一个设计页面调用一次该函数
        /// </summary>
        /// <param name="document">XmlDocument结构</param>
        /// <param name="savefilename">要保存的文件名字</param>
        /// <param name="value">要保存的对象</param>
        /// <param name="designerPath">设计器的路径</param>
        public void SaveProjectToFile(XmlDocument document, string savefilename, object value, string designerPath)
        {
            try
            {
                //调用WriteObject函数来组织XmlDocument结构体,根节点下添加
                document.FirstChild.AppendChild(WriteObject(document, value, designerPath));
                document.Save(savefilename + ".xml");   //把组织好的XmlDocument结构体保存到文件中
            }
            catch (Exception ex) { }
        }






    }// class
}// namespace
