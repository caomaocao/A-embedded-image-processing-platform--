using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CaVeGen.DesignViewFiles.FilterProperty
{
    public class XPropDescriptor : PropertyDescriptor
    {
        private XProp xProp = null;

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return this.GetType(); }
        }

        public override object GetValue(object component)
        {
            return this.xProp.TheValue; 
        }

        public override object GetEditor(Type editorBaseType)
        {
            if (this.xProp.ValueType != "MyEnum")
            {
                return base.GetEditor(editorBaseType);
            }
            else
            {
                return new EditControl();
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get 
            {
                if (this.xProp.ValueType != "MyEnum")
                {
                    //return this.xProp.Value.GetType();
                    return Type.GetType(xProp.ValueType);
                }
                else
                {
                    return this.xProp.EnumValue.GetType();
                }
            }
        }

        public override void ResetValue(object component)
        {
            ;
        }

        public override void SetValue(object component, object value)
        {
            this.xProp.TheValue = value;   
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public XPropDescriptor(XProp prop, Attribute[] attrs)
            : base(prop.Name, attrs)
        {
            this.xProp = prop;
        }
    }
}
