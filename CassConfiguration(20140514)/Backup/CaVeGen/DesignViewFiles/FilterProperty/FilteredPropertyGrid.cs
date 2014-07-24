using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CaVeGen.CommonOperation;

namespace  CaVeGen.DesignViewFiles.FilterProperty
{
    /// <summary>
    /// 继承了标准PropertyGrid类
    /// 该类实现了允许所选择对象过滤部分属性 ，其中Attribute是Property的分类信息
    /// </summary>
    public partial class FilteredPropertyGrid : PropertyGrid
    {
        //属性控件中的对象属性的所有描述,默认情况下，propertyDescriptors描述符包括了所选对象的所有属性
        List<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();
        //需隐藏和显示的属性集的Attribute信息,即：分类信息，Attribute：自定义属性的基类 
        private AttributeCollection hiddenAttributes = null, browsableAttributes = null;
        //需要隐藏和显示的属性集的Property信息，即：分类中的具体信息
        private string[] browsableProperties = null, hiddenProperties = null;
        private ObjectWrapper wrapper = null;   //当前所选中的对象

        /// <summary>
        /// FilteredPropertyGrid 类构造函数
        /// </summary>
        public FilteredPropertyGrid()
        {
            InitializeComponent();
            base.SelectedObject = this.wrapper;
        }

        /// <summary>
        /// 获得分类属性集合
        /// </summary>
        public new AttributeCollection BrowsableAttributes
        {
            get
            {
                return this.browsableAttributes;
            }
            set
            {
                if (this.browsableAttributes != value)
                {
                    this.browsableAttributes = value;
                    RefreshProperties();
                }
            }
        }

        /// <summary>
        /// 需要隐藏的分类信息
        /// </summary>
        public AttributeCollection HiddenAttributes
        {
            get
            {
                return this.hiddenAttributes;
            }
            set
            {
                if (value != this.hiddenAttributes)
                {
                    this.hiddenAttributes = value;
                    RefreshProperties();
                }
            }
        }
        
        /// <summary>
        /// 需要显示的具体属性信息
        /// </summary>
        public string[] BrowsableProperties
        {
            get 
            {
                return this.browsableProperties; 
            }
            set
            {
                if (value != this.browsableProperties)
                {
                    this.browsableProperties = value;
                    RefreshProperties();
                }
            }
        }

        //private

        /// <summary>
        /// 设置需要隐藏的属性的名称
        /// </summary>
        public string[] HiddenProperties
        {
            get 
            {
                return this.hiddenProperties; 
            }
            set
            {
                if (value != this.hiddenProperties)
                {
                    this.hiddenProperties = value;
                    RefreshProperties();
                }
            }
        }

        /// <summary>
        /// 获得选中对象集(获取或设置当前选定的对象).
        /// </summary>
        public new  object[] SelectedObjects
        {            
            set
            {
                base.SelectedObjects = value;
                if (value != null)
                {
                    if (this.wrapper == null || this.wrapper.SelectedObject == null)
                    {
                        this.wrapper = new ObjectWrapper(value[0]);
                        RefreshProperties();
                    }
                    else
                    {
                        if (this.wrapper.SelectedObject != null
                            && this.wrapper.SelectedObject != value[0])
                        {
                            this.wrapper.SelectedObject = value[0];
                            if (value[0].GetType() != this.wrapper.SelectedObject.GetType())
                            {
                                RefreshProperties();
                            }
                        }
                    }

                    Control selectControl = (Control)(this.wrapper.SelectedObject);

                    if (!selectControl.GetType().ToString().Equals(PublicVariable.viewName))
                    {
                        CassView cassView = (CassView)(selectControl.Parent);

                        int controlIndex = cassView.FindControlName(selectControl.Site.Name);
                        if (controlIndex != -1 && cassView.ctrlsInfo[controlIndex].VisibleFunctionProperty != null)
                        {
                            foreach (XProp xprop in cassView.ctrlsInfo[controlIndex].VisibleFunctionProperty)
                            {
                                XPropDescriptor propDescriptor = new XPropDescriptor(
                                    xprop, CassViewGenerator.functionAttribute);
                                this.propertyDescriptors.Add(propDescriptor);
                            }
                        }
                    }
                    this.wrapper.PropertyDescriptors = this.propertyDescriptors;      // 获得所要封装对象的属性描述集                    
                    base.SelectedObject = this.wrapper;      // 获得当前创建的对象
                }
                else //如果对象为空，则清空当前的控件内容
                {
                    if (this.wrapper != null)
                    {
                        this.wrapper.SelectedObject = null;
                        this.wrapper.PropertyDescriptorClear();  //清空属性集
                        this.propertyDescriptors.Clear();
                        this.wrapper = null;
                        base.SelectedObject = null;
                        base.SelectedObjects = null;
                        base.Refresh();
                    }
                }
            }
        }

       /// <summary>
        /// 重写所选择对象的属性，创建不包括隐藏掉的属性的对象。(获取或设置在网格中显示属性的对象。)
       /// 即：创建一个新的对象，该对象和原有对象的属性一致。再对该新对象的属性进行改写
       /// </summary>
        public new object SelectedObject
        {
            get
            {
                if (base.SelectedObject != null )
                {
                    if (base.SelectedObject is ObjectWrapper)
                    {
                        return this.wrapper != null ? ((ObjectWrapper)base.SelectedObject).SelectedObject : null;
                    }
                    else 
                    {
                        if (this.wrapper != null && base.SelectedObjects.Length > 0)
                        {
                            this.wrapper = new ObjectWrapper(base.SelectedObjects[0]);
                            this.wrapper.SelectedObject = base.SelectedObjects[0];
                            RefreshProperties();
                            this.wrapper.PropertyDescriptors = propertyDescriptors;
                            base.SelectedObject = wrapper;
                            return wrapper.SelectedObject;
                        }
                        else
                        {
                            return null;
                        }
                    }                    
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    if (this.wrapper == null || this.wrapper.SelectedObject == null)
                    {
                        this.wrapper = new ObjectWrapper(value);
                        RefreshProperties();
                    }
                    else
                    {
                        if (this.wrapper.SelectedObject != null && this.wrapper.SelectedObject != value)
                        {
                            this.wrapper.SelectedObject = value;
                            if (value.GetType() != this.wrapper.SelectedObject.GetType())
                            {
                                RefreshProperties();
                            }
                        }
                    }
                    this.wrapper.PropertyDescriptors = this.propertyDescriptors;      // 获得所要封装对象的属性描述集                    
                    base.SelectedObject = this.wrapper;      // 获得当前创建的对象
                }
                else //如果对象为空，则清空当前的控件内容
                {
                    if (this.wrapper != null)
                    {
                        this.wrapper.SelectedObject = null;
                        this.wrapper.PropertyDescriptorClear();  //清空属性集
                        this.propertyDescriptors.Clear();
                        this.wrapper = null;
                        base.SelectedObject = null;
                        base.SelectedObjects = null;
                        base.Refresh();
                    }
                }               
            }
        }

        /// <summary>
        /// 可浏览属性发生变化时调用该函数
        /// </summary>
        private void OnBrowsablePropertiesChanged()
        {
            if (this.wrapper == null)
            {
                return;
            } 
        }

        /// <summary>
        /// 根据所定义的隐藏和所要显示的属性的信息，对属性描述集进行筛选。
        /// </summary>
        private void RefreshProperties()
        {
            if (this.wrapper == null)
            {
                return;
            }
            this.propertyDescriptors.Clear();

            if (this.browsableAttributes != null && this.browsableAttributes.Count > 0)
            {
                //显示所要显示的属性信息
                foreach (Attribute attribute in this.browsableAttributes)
                { 
                    ShowAttribute(attribute); 
                }
            }
            else
            {
                // 添加对象的全部属性 ，对需要隐藏的分类进行隐藏操作
                PropertyDescriptorCollection originalpropertydescriptors
                    = TypeDescriptor.GetProperties(wrapper.SelectedObject);
                foreach (PropertyDescriptor propertyDescriptor in originalpropertydescriptors)
                {
                     this.propertyDescriptors.Add(propertyDescriptor);
                }
                
                if (this.hiddenAttributes != null)      // 对需要隐藏的自定义属性信息进行隐藏
                {
                    foreach (Attribute attribute in this.hiddenAttributes)
                    {
                        HideAttribute(attribute);
                    }
                }
            }

            // 得到所选对象的所有属性
            PropertyDescriptorCollection allproperties = TypeDescriptor.GetProperties(wrapper.SelectedObject);
            if (this.hiddenProperties != null && this.hiddenProperties.Length > 0)        //隐藏属性
            {
                foreach (string propertyname in this.hiddenProperties)
                {
                    try
                    {
                        PropertyDescriptor property = allproperties[propertyname];
                        HideProperty(property);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(ex.Message);
                    }
                }
            }

            //对需要显示的具体信息进行操作
            if (this.browsableProperties != null && this.browsableProperties.Length > 0)
            {
                foreach (string propertyname in this.browsableProperties)
                {
                    try
                    {
                        ShowProperty(allproperties[propertyname]);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Property not found", propertyname);
                    }
                }
            }
        }

      /// <summary>
      /// 隐藏分类属性信息操作
      /// </summary>
      /// <param name="attribute">分类属性</param>
        private void HideAttribute(Attribute attribute)
        {
            try
            {
                PropertyDescriptorCollection filteredOriginalPropertyDescriptors
                    = TypeDescriptor.GetProperties(wrapper.SelectedObject, new Attribute[] { attribute });
                if (filteredOriginalPropertyDescriptors == null
                    || filteredOriginalPropertyDescriptors.Count == 0)
                {
                    return;
                }
                foreach (PropertyDescriptor propertyDescriptor in filteredOriginalPropertyDescriptors)
                {
                    HideProperty(propertyDescriptor);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        /// <summary>
        /// 显示分类属性信息
        /// </summary>
        /// <param name="attribute">属性分类信息</param>
        private void ShowAttribute(Attribute attribute)
        {
            try
            {
                PropertyDescriptorCollection filteredOriginalPropertyDescriptors
                    = TypeDescriptor.GetProperties(wrapper.SelectedObject, new Attribute[] { attribute });
                if (filteredOriginalPropertyDescriptors == null
                    || filteredOriginalPropertyDescriptors.Count == 0)
                { 
                    //throw new ArgumentException("Attribute not found", attribute.ToString());
                    return;
                }
                foreach (PropertyDescriptor propertyDescriptor in filteredOriginalPropertyDescriptors)
                { 
                    ShowProperty(propertyDescriptor); 
                }
            }
            catch(Exception ex)
            {
                //throw new ArgumentException(ex.Message);
            }
        }
        
        /// <summary>
        /// 显示具体属性
        /// </summary>
        /// <param name="property">属性描述</param>
        private void ShowProperty(PropertyDescriptor property)
        {
            if (!propertyDescriptors.Contains(property)) 
            {
                this.propertyDescriptors.Add(property);
            } 
        }
       
        /// <summary>
        /// 删除需要隐藏的属性
        /// </summary>
        /// <param name="property">属性描述</param>
        private void HideProperty(PropertyDescriptor property)
        {
            if (propertyDescriptors.Contains(property))
            {
                propertyDescriptors.Remove(property);
            }
        }
    }
}
