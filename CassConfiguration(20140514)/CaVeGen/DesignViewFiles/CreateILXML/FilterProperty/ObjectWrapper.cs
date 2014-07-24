using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CaVeGen.DesignViewFiles.FilterProperty
{
    // ICustomTypeDescriptor：提供为对象提供动态自定义类型信息的接口。
    internal class ObjectWrapper : ICustomTypeDescriptor
    {
        private object selectedObject = null;  //当前选中的对象

        //属性控件中的对象属性的所有描述,默认情况下，propertyDescriptors描述符包括了所选对象的所有属性
        List<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();

        /// <summary>
        /// ObjectWrapper 类构造函数
        /// </summary>
        /// <param name="obj">信息被显示的组件</param>
        internal ObjectWrapper(object obj)
        {
            this.selectedObject = obj;
        }

        /// <summary>
        /// 获得所选对象
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
        /// 获得所选对象的所有描述符
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
        /// 清除描述符
        /// </summary>
        public void PropertyDescriptorClear()
        {
            this.propertyDescriptors.Clear();
        }

        #region ICustomTypeDescriptor接口的方法

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(this.propertyDescriptors.ToArray(), true);
        }

        /// <summary>
        /// 返回此组件实例的自定义属性的集合。
        /// </summary>
        /// <returns>组件属性信息中属性</returns>
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
        /// 返回此组件实例的类名
        /// </summary>
        /// <returns>组件属性信息中类名</returns>
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
        /// 返回此组件实例的名称。 
        /// </summary>
        /// <returns>组件属性信息中组件名</returns>
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
        /// 返回此组件实例的类型转换器。 
        /// </summary>
        /// <returns>组件属性信息中类型</returns>
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
        /// 返回此组件实例的默认事件。
        /// </summary>
        /// <returns>组件属性信息中的默认事件</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this.selectedObject, true);
        }

        /// <summary>
        /// 返回此组件实例的默认属性。
        /// </summary>
        /// <returns>组件属性信息中的默认属性</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this.selectedObject, true);
        }

        /// <summary>
        /// 返回此组件实例的指定类型的编辑器。
        /// </summary>
        /// <param name="editorBaseType">编辑器类型</param>
        /// <returns>组件属性信息中的编辑器</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// 将指定的属性数组用作筛选器来返回此组件实例的事件。
        /// </summary>
        /// <param name="attributes">属性数组</param>
        /// <returns>组件实例的事件</returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this.selectedObject, attributes, true);
        }

        /// <summary>
        /// 返回此组件实例的事件
        /// </summary>
        /// <returns>组件实例的事件</returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this.selectedObject, true);
        }

        /// <summary>
        /// 返回包含指定的属性描述符所描述的属性的对象。
        /// </summary>
        /// <param name="pd">对象的属性描述符</param>
        /// <returns>所选择的对象</returns>
        public object GetPropertyOwner(PropertyDescriptor propertyDescriptor)
        {
            return this.selectedObject;
        }

        #endregion
    }
}


