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
    /// �̳��˱�׼PropertyGrid��
    /// ����ʵ����������ѡ�������˲������� ������Attribute��Property�ķ�����Ϣ
    /// </summary>
    public partial class FilteredPropertyGrid : PropertyGrid
    {
        //���Կؼ��еĶ������Ե���������,Ĭ������£�propertyDescriptors��������������ѡ�������������
        List<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();
        //�����غ���ʾ�����Լ���Attribute��Ϣ,����������Ϣ��Attribute���Զ������ԵĻ��� 
        private AttributeCollection hiddenAttributes = null, browsableAttributes = null;
        //��Ҫ���غ���ʾ�����Լ���Property��Ϣ�����������еľ�����Ϣ
        private string[] browsableProperties = null, hiddenProperties = null;
        private ObjectWrapper wrapper = null;   //��ǰ��ѡ�еĶ���

        /// <summary>
        /// FilteredPropertyGrid �๹�캯��
        /// </summary>
        public FilteredPropertyGrid()
        {
            InitializeComponent();
            base.SelectedObject = this.wrapper;
        }

        /// <summary>
        /// ��÷������Լ���
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
        /// ��Ҫ���صķ�����Ϣ
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
        /// ��Ҫ��ʾ�ľ���������Ϣ
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
        /// ������Ҫ���ص����Ե�����
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
        /// ���ѡ�ж���(��ȡ�����õ�ǰѡ���Ķ���).
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
                    this.wrapper.PropertyDescriptors = this.propertyDescriptors;      // �����Ҫ��װ���������������                    
                    base.SelectedObject = this.wrapper;      // ��õ�ǰ�����Ķ���
                }
                else //�������Ϊ�գ�����յ�ǰ�Ŀؼ�����
                {
                    if (this.wrapper != null)
                    {
                        this.wrapper.SelectedObject = null;
                        this.wrapper.PropertyDescriptorClear();  //������Լ�
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
        /// ��д��ѡ���������ԣ��������������ص������ԵĶ���(��ȡ����������������ʾ���ԵĶ���)
       /// ��������һ���µĶ��󣬸ö����ԭ�ж��������һ�¡��ٶԸ��¶�������Խ��и�д
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
                    this.wrapper.PropertyDescriptors = this.propertyDescriptors;      // �����Ҫ��װ���������������                    
                    base.SelectedObject = this.wrapper;      // ��õ�ǰ�����Ķ���
                }
                else //�������Ϊ�գ�����յ�ǰ�Ŀؼ�����
                {
                    if (this.wrapper != null)
                    {
                        this.wrapper.SelectedObject = null;
                        this.wrapper.PropertyDescriptorClear();  //������Լ�
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
        /// ��������Է����仯ʱ���øú���
        /// </summary>
        private void OnBrowsablePropertiesChanged()
        {
            if (this.wrapper == null)
            {
                return;
            } 
        }

        /// <summary>
        /// ��������������غ���Ҫ��ʾ�����Ե���Ϣ������������������ɸѡ��
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
                //��ʾ��Ҫ��ʾ��������Ϣ
                foreach (Attribute attribute in this.browsableAttributes)
                { 
                    ShowAttribute(attribute); 
                }
            }
            else
            {
                // ��Ӷ����ȫ������ ������Ҫ���صķ���������ز���
                PropertyDescriptorCollection originalpropertydescriptors
                    = TypeDescriptor.GetProperties(wrapper.SelectedObject);
                foreach (PropertyDescriptor propertyDescriptor in originalpropertydescriptors)
                {
                     this.propertyDescriptors.Add(propertyDescriptor);
                }
                
                if (this.hiddenAttributes != null)      // ����Ҫ���ص��Զ���������Ϣ��������
                {
                    foreach (Attribute attribute in this.hiddenAttributes)
                    {
                        HideAttribute(attribute);
                    }
                }
            }

            // �õ���ѡ�������������
            PropertyDescriptorCollection allproperties = TypeDescriptor.GetProperties(wrapper.SelectedObject);
            if (this.hiddenProperties != null && this.hiddenProperties.Length > 0)        //��������
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

            //����Ҫ��ʾ�ľ�����Ϣ���в���
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
      /// ���ط���������Ϣ����
      /// </summary>
      /// <param name="attribute">��������</param>
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
        /// ��ʾ����������Ϣ
        /// </summary>
        /// <param name="attribute">���Է�����Ϣ</param>
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
        /// ��ʾ��������
        /// </summary>
        /// <param name="property">��������</param>
        private void ShowProperty(PropertyDescriptor property)
        {
            if (!propertyDescriptors.Contains(property)) 
            {
                this.propertyDescriptors.Add(property);
            } 
        }
       
        /// <summary>
        /// ɾ����Ҫ���ص�����
        /// </summary>
        /// <param name="property">��������</param>
        private void HideProperty(PropertyDescriptor property)
        {
            if (propertyDescriptors.Contains(property))
            {
                propertyDescriptors.Remove(property);
            }
        }
    }
}
