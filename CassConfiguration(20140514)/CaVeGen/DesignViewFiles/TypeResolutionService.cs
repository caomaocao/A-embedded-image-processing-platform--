/*******************************************************************************
           ** Copyright (C) 2006 CASS 版权所有
           ** 文件名：TypeResolutionService.cs
           ** 功能描述：
           **          解析类型，为CodeDomHostLoader类服务。
           **          它解析了项目引用中所涉及到的程序集的所有类型。
           ** 作者：吴丹红
           ** 创始时间：2006-11-11
           ** 
********************************************************************************/

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.CodeDom;
using CaVeGen.CommonOperation;
using System.Security.Policy;

namespace CaVeGen.DesignViewFiles
{
    /// <summary>
    /// 用于解析类型，在CodeDomHostLoader中调用。ITypeResolutionService:提供按名称检索程序集或类型的接口
    /// </summary>
    public class TypeResolutionService : ITypeResolutionService
    {
        private Hashtable hashTable = new Hashtable();

        public TypeResolutionService()
        {
        }

        /// <summary>
        /// GetAssembly：获取请求的程序集。
        /// </summary>
        public System.Reflection.Assembly GetAssembly(System.Reflection.AssemblyName name)
        {
            return GetAssembly(name, true);
        }

        public System.Reflection.Assembly GetAssembly(System.Reflection.AssemblyName name, bool throwOnErrors)
        {
            return Assembly.GetAssembly(typeof(UserControl));
        }

        /// <summary>
        /// 获取从中加载程序集的文件的路径
        /// </summary>
        public string GetPathOfAssembly(System.Reflection.AssemblyName name)
        {
            return null;
        }

        /// <summary>
        /// 用指定的名称加载类型。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type GetType(string name)
        {
            return this.GetType(name, true);
        }

        public Type GetType(string name, bool throwOnError)
        {
            return this.GetType(name, throwOnError, false);
        }

        /// <summary>
        ///从工具箱中将控件拖动后添加到设计器时，将调用CodeDomHostLoader类，在该类中将调用服务以解析类型
        /// </summary>
        public Type GetType(string name, bool throwOnError, bool ignoreCase)
        {

            if (hashTable.ContainsKey(name))
            {
                return (Type)hashTable[name];
            }

            Assembly[] windowsControls = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly winControl in windowsControls)
            {
                //除去不需要的程序集
                if (
                    winControl.ToString() == PublicVariable.strSystem
                    ||
                    winControl.ToString() == PublicVariable.strData
                    || winControl.ToString() == PublicVariable.strXml
                    || winControl.ToString() == PublicVariable.strDesign
                    || winControl.ToString() == PublicVariable.strJanus
                    || winControl.ToString() == PublicVariable.strExporerBar
                    || winControl.ToString() == PublicVariable.strConfiguration
                    || winControl.ToString() == PublicVariable.strResource
                    || winControl.ToString() == PublicVariable.strAccessibility
                    )
                    continue;

                Type[] types = winControl.GetTypes();
                string typeName = String.Empty;
                foreach (Type type in types)
                {
                    typeName = type.FullName;
                    if (typeName == name)
                    {
                        hashTable[name] = type;
                        return type;
                    }
                }
            }

            return Type.GetType(name);
        }

        public void ReferenceAssembly(System.Reflection.AssemblyName name)
        {
            //一旦将对程序集的引用添加到此服务，此服务就可以通过不指定程序集的名称来加载类型。
        }

    }// class
}// namespace

