using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CaVeGen.DesignViewFiles.FilterProperty
{
    public class XProp
    {

        private string valueType = "";
        /// <summary>
        /// 当前功能属性的类型
        /// </summary>
        public string ValueType
        {
            get
            {
                return this.valueType;
            }
            set
            {
                this.valueType = value;
            }
        }

        private string name = "";
        /// <summary>
        /// 当前功能属性的中文名
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        private string varName = "";    
        /// <summary>
        /// 当前功能属性的英文名
        /// </summary>
        public string VarName
        {
            get
            {
                return this.varName;
            }
            set
            {
                this.varName = value;
            }
        }


        private string enumValue = "";
        /// <summary>
        /// 自定义属性的备选值
        /// </summary>
        public string EnumValue
        {
            get
            {
                return this.enumValue;
            }
            set
            {
                this.enumValue = value;
            }
        }

        private object theValue = null;
        /// <summary>
        /// 当前功能属性的值
        /// </summary>
        public object TheValue
        {
            get
            {
                return this.theValue;
            }
            set
            {
                this.theValue = value;
            }
        }

        public override string ToString()
        {
            return "Name: " + this.name + ", Value: " + this.theValue.ToString();
        }

        private string valueExplain = "";
        /// <summary>
        /// 属性的描述信息
        /// </summary>
        public string ValueExplain
        {
            get
            {
                return this.valueExplain;
            }
            set
            {
                this.valueExplain = value;
            }
        }

        private string optype = null;
        /// <summary>
        /// 属性的优化类型
        /// </summary>
        public string Optype
        {
            get
            {
                return this.optype;
            }
            set
            {
                this.optype = value;
            }
        }

        private string opvalue = null;
        /// <summary>
        /// 属性的优化值
        /// </summary>
        public string Opvalue
        {
            get
            {
                return this.opvalue;
            }
            set
            {
                this.opvalue = value;
            }
        }

        private string opnode = null;
        /// <summary>
        /// 属性的优化结点
        /// </summary>
        public string Opnode
        {
            get
            {
                return this.opnode;
            }
            set
            {
                this.opnode = value;
            }
        }

        private string relate = null;
        /// <summary>
        /// 属性的关系运算
        /// </summary>
        public string Relate
        {
            get
            {
                return this.relate;
            }
            set
            {
                this.relate = value;
            }
        }

        public XProp()
        {
        }

        /// <summary>
        /// 在功能属性集合中对特定属性值赋值
        /// </summary>
        /// <param name="value">赋值</param>
        /// <param name="FunName">目标属性的VarName</param>
        /// <param name="Functions">功能属性集合</param>
        public static void SetValue(string value, string FunName, List<XProp> Functions)
        {
            foreach (XProp function in Functions)
            {
                if (function.VarName == FunName)
                {
                    function.TheValue = value;
                    break;
                }
            }
        }

        /// <summary>
        /// 从功能属性集合中获取所需的属性值
        /// </summary>
        /// <param name="FunName">目标属性的VarName</param>
        /// <param name="Functions">功能属性集合</param>
        /// <returns>返回属性值</returns>
        public static string GetValue(string FunName, List<XProp> Functions)
        {
            foreach (XProp function in Functions)
            {
                if (function.VarName == FunName)
                {
                    return function.TheValue.ToString();                   
                }
            }
            return null;
        }
    }
}
