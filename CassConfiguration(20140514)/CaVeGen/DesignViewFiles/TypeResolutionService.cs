/*******************************************************************************
           ** Copyright (C) 2006 CASS ��Ȩ����
           ** �ļ�����TypeResolutionService.cs
           ** ����������
           **          �������ͣ�ΪCodeDomHostLoader�����
           **          ����������Ŀ���������漰���ĳ��򼯵��������͡�
           ** ���ߣ��ⵤ��
           ** ��ʼʱ�䣺2006-11-11
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
    /// ���ڽ������ͣ���CodeDomHostLoader�е��á�ITypeResolutionService:�ṩ�����Ƽ������򼯻����͵Ľӿ�
    /// </summary>
    public class TypeResolutionService : ITypeResolutionService
    {
        private Hashtable hashTable = new Hashtable();

        public TypeResolutionService()
        {
        }

        /// <summary>
        /// GetAssembly����ȡ����ĳ��򼯡�
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
        /// ��ȡ���м��س��򼯵��ļ���·��
        /// </summary>
        public string GetPathOfAssembly(System.Reflection.AssemblyName name)
        {
            return null;
        }

        /// <summary>
        /// ��ָ�������Ƽ������͡�
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
        ///�ӹ������н��ؼ��϶�����ӵ������ʱ��������CodeDomHostLoader�࣬�ڸ����н����÷����Խ�������
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
                //��ȥ����Ҫ�ĳ���
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
            //һ�����Գ��򼯵�������ӵ��˷��񣬴˷���Ϳ���ͨ����ָ�����򼯵��������������͡�
        }

    }// class
}// namespace

