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
        /// ��ǰ�������Ե�����
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
        /// ��ǰ�������Ե�������
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
        /// ��ǰ�������Ե�Ӣ����
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
        /// �Զ������Եı�ѡֵ
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
        /// ��ǰ�������Ե�ֵ
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
        /// ���Ե�������Ϣ
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
        /// ���Ե��Ż�����
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
        /// ���Ե��Ż�ֵ
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
        /// ���Ե��Ż����
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
        /// ���ԵĹ�ϵ����
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
        /// �ڹ������Լ����ж��ض�����ֵ��ֵ
        /// </summary>
        /// <param name="value">��ֵ</param>
        /// <param name="FunName">Ŀ�����Ե�VarName</param>
        /// <param name="Functions">�������Լ���</param>
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
        /// �ӹ������Լ����л�ȡ���������ֵ
        /// </summary>
        /// <param name="FunName">Ŀ�����Ե�VarName</param>
        /// <param name="Functions">�������Լ���</param>
        /// <returns>��������ֵ</returns>
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
