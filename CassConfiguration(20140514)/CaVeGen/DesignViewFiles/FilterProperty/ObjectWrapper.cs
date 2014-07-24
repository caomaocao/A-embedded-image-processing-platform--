using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CaVeGen.DesignViewFiles.FilterProperty
{
    // ICustomTypeDescriptor���ṩΪ�����ṩ��̬�Զ���������Ϣ�Ľӿڡ�
    internal class ObjectWrapper : ICustomTypeDescriptor
    {
        private object selectedObject = null;  //��ǰѡ�еĶ���

        //���Կؼ��еĶ������Ե���������,Ĭ������£�propertyDescriptors��������������ѡ�������������
        List<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();

        /// <summary>
        /// ObjectWrapper �๹�캯��
        /// </summary>
        /// <param name="obj">��Ϣ����ʾ�����</param>
        internal ObjectWrapper(object obj)
        {
            this.selectedObject = obj;
        }

        /// <summary>
        /// �����ѡ����
        /// </summary>
        public object SelectedObject
        {
            get 
            {
                return this.selectedObject; 
            }
            set 
            {
                if (this.selectedObject != value)
                {
                    this.selectedObject = value;
                }
            }
        }

        /// <summary>
        /// �����ѡ���������������
        /// </summary>
        public List<PropertyDescriptor> PropertyDescriptors
        {
            get
            {
                return this.propertyDescriptors; 
            }
            set 
            {
                this.propertyDescriptors = value; 
            }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public void PropertyDescriptorClear()
        {
            this.propertyDescriptors.Clear();
        }

        #region ICustomTypeDescriptor�ӿڵķ���

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(this.propertyDescriptors.ToArray(), true);
        }

        /// <summary>
        /// ���ش����ʵ�����Զ������Եļ��ϡ�
        /// </summary>
        /// <returns>���������Ϣ������</returns>
        public AttributeCollection GetAttributes()
        {
            if (this.selectedObject != null)
            {
                return TypeDescriptor.GetAttributes(this.selectedObject, true);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ���ش����ʵ��������
        /// </summary>
        /// <returns>���������Ϣ������</returns>
        public String GetClassName()
        {
            if (this.selectedObject != null)
            {
                return TypeDescriptor.GetClassName(this.selectedObject, true);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ���ش����ʵ�������ơ� 
        /// </summary>
        /// <returns>���������Ϣ�������</returns>
        public String GetComponentName()
        {
            if (this.selectedObject != null)
            {
                return TypeDescriptor.GetComponentName(this.selectedObject, true);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ���ش����ʵ��������ת������ 
        /// </summary>
        /// <returns>���������Ϣ������</returns>
        public TypeConverter GetConverter()
        {
            if (this.selectedObject != null)
            {
                return TypeDescriptor.GetConverter(this.selectedObject, true);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ���ش����ʵ����Ĭ���¼���
        /// </summary>
        /// <returns>���������Ϣ�е�Ĭ���¼�</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this.selectedObject, true);
        }

        /// <summary>
        /// ���ش����ʵ����Ĭ�����ԡ�
        /// </summary>
        /// <returns>���������Ϣ�е�Ĭ������</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this.selectedObject, true);
        }

        /// <summary>
        /// ���ش����ʵ����ָ�����͵ı༭����
        /// </summary>
        /// <param name="editorBaseType">�༭������</param>
        /// <returns>���������Ϣ�еı༭��</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// ��ָ����������������ɸѡ�������ش����ʵ�����¼���
        /// </summary>
        /// <param name="attributes">��������</param>
        /// <returns>���ʵ�����¼�</returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this.selectedObject, attributes, true);
        }

        /// <summary>
        /// ���ش����ʵ�����¼�
        /// </summary>
        /// <returns>���ʵ�����¼�</returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this.selectedObject, true);
        }

        /// <summary>
        /// ���ذ���ָ�������������������������ԵĶ���
        /// </summary>
        /// <param name="pd">���������������</param>
        /// <returns>��ѡ��Ķ���</returns>
        public object GetPropertyOwner(PropertyDescriptor propertyDescriptor)
        {
            return this.selectedObject;
        }

        #endregion
    }
}


