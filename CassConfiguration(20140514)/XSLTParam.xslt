<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration="yes"/>
<!--变量前缀-->
<xsl:variable name="PredixVar">configuration<xsl:value-of select="/DCS/ProjectIndex"/>_</xsl:variable>
<!--函数前缀-->
<xsl:variable name="PredixFun">Configuration<xsl:value-of select="/DCS/ProjectIndex"/>_</xsl:variable>
<!--根匹配-->
<!--主函数开始-->
<xsl:template match="/"><xsl:text disable-output-escaping="yes">#include "Configuration_main.h"
</xsl:text>
<!--全局变量设置-->    
    <xsl:apply-templates select="/DCS/EntireVarRegion"/>
    <xsl:apply-templates select="/DCS/Parameters"/>   
    <xsl:apply-templates select="/DCS/Calculators"/>
    <xsl:apply-templates select="/DCS/ConditionActions"/>
    <xsl:apply-templates select="/DCS/Function"/>      
    <xsl:apply-templates select="/DCS/Main"/>
  </xsl:template>

  <!--// 多输出的中间变量-->
  <xsl:template match="/DCS/EntireVarRegion">
// 多输出的中间变量
<xsl:for-each select="item">
<xsl:value-of select="@type"/><xsl:text> </xsl:text><xsl:value-of select="."/>;
</xsl:for-each>
  </xsl:template>
  
  <!--初始化参数函数匹配,这个要放到最先-->
  <xsl:template match="/DCS/Parameters">
// 全局变量
<xsl:text></xsl:text>
    <xsl:for-each select="Parameter">
<xsl:value-of select="@type"/>Struct <xsl:value-of select="@name"/>;
<xsl:text></xsl:text>
    </xsl:for-each>
    <!--数组声明定义-->
    <xsl:for-each select="Arrays/item">
      <xsl:value-of select="@type"/><xsl:text> </xsl:text>
      <xsl:value-of select="@name"/>[<xsl:value-of select="@dimension"/>] = <xsl:value-of select="."/>;
    </xsl:for-each>
    <!--子函数的向前声明-->
    <xsl:text>
// 子函数的向前声明</xsl:text>
    <xsl:for-each select="/DCS/Function">      
uint8 <xsl:value-of select="@name"/>(void);</xsl:for-each>
// 初始化参数
void <xsl:value-of select="$PredixFun"/>InitParam(void)
{
    <!--参数列表-->
    <xsl:apply-templates select="Parameter"/>  <xsl:text>
</xsl:text>    
    <!--中间变量初始化为0-->    // 中间变量初始化
    <xsl:for-each select="/DCS/EntireVarRegion/item">    
    <xsl:value-of select="."/> = 0;
    </xsl:for-each>
}  </xsl:template> 
  
  <!--各个参数匹配-->
  <xsl:template match="Parameter">
    <xsl:param name="name" select="@name"/>
    // <xsl:value-of select="$name"/>
    <xsl:text>
    </xsl:text>
    <!--E2PROM参数-->
    <xsl:for-each select="E2PROM/item">
      <xsl:value-of select="$name"/>.<xsl:value-of select="@name"/> = Configuration_ReadPara<xsl:value-of select="@type"/>(<xsl:value-of select="."/>);
    </xsl:for-each>
    <!--其它参数-->
    <xsl:for-each select="others/item">
      <xsl:if test="@optimise">
    #if <xsl:value-of select="@optimise"/><xsl:text>  
       </xsl:text>        
      </xsl:if>
          <xsl:value-of select="$name"/>.<xsl:value-of select="@name"/> = <xsl:value-of select="."/>;
    <xsl:if test="@optimise">#endif
    </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <!--计算器-->
  <xsl:template match="/DCS/Calculators">
    <xsl:for-each select="Calculator">
      <xsl:variable name="varCalcu" select="@name"/>
// 计算器
uint8 <xsl:value-of select="$PredixFun"/><xsl:value-of select="$varCalcu"/>Control(CALCUStruct *CACUL)
{
      fp32 X0,X1,X2,X3,X4,X5,X6,X7;
      fp32 Y0,Y1,Y2,Y3;

      <xsl:text disable-output-escaping="yes">Pop(&amp;paramStack,&amp;X7);
      Pop(&amp;paramStack,&amp;X6);
      Pop(&amp;paramStack,&amp;X5);
      Pop(&amp;paramStack,&amp;X4);
      Pop(&amp;paramStack,&amp;X3);
      Pop(&amp;paramStack,&amp;X2);
      Pop(&amp;paramStack,&amp;X1);
      Pop(&amp;paramStack,&amp;X0);
      Y0 = 0;
      Y1 = 0;
      Y2 = 0;
      Y3 = 0;
</xsl:text>
      <!--每个条件-->
      <xsl:for-each select="item">
        <xsl:if test="./condition">
       if(<xsl:value-of select="./condition" disable-output-escaping="yes"/>)
       {
        </xsl:if>
        <xsl:if test="./expression">
          <xsl:value-of select="./expression" disable-output-escaping="yes"/>;
        </xsl:if>
        <xsl:if test="./condition">
       }
        </xsl:if>

      </xsl:for-each>
      <xsl:text disable-output-escaping="yes">
      Push(&amp;paramStack,Y0);
      Push(&amp;paramStack,Y1);
      Push(&amp;paramStack,Y2);
      Push(&amp;paramStack,Y3);
      return 1;
      </xsl:text>
      
}

    </xsl:for-each>
  </xsl:template>


  <!--条件动作表-->
  <xsl:template match="/DCS/ConditionActions">
    <xsl:for-each select="ConditionAction">
      <xsl:variable name="varCalcu" select="@name"/>
// 条件动作表
uint8 <xsl:value-of select="$PredixFun"/><xsl:value-of select="$varCalcu"/>Control(PROCESSStruct *PROCESS)
{
   <xsl:text disable-output-escaping="yes">uint8 condition[32]= {0,0,0,0,0,0,0,0,0,0,
                   0,0,0,0,0,0,0,0,0,0,
                   0,0};
   fp32 isEn = 0;
   Pop(&amp;paramStack,&amp;isEn);
   if(isEn != 0)
   {
</xsl:text>
      <!--每个条件-->
      <xsl:for-each select="item">
        <xsl:if test="./condition">
          <xsl:text></xsl:text>      if(<xsl:for-each select="./condition">            
              <xsl:value-of select="." disable-output-escaping="yes"/>
            <xsl:if test="position() != last()">
              <xsl:text disable-output-escaping="yes"> &amp;&amp; </xsl:text>
            </xsl:if>
            <xsl:if test="position() = last()">)
      {
      <xsl:text>   </xsl:text>
            </xsl:if>
          </xsl:for-each>
        </xsl:if>
        <xsl:if test="./expression">        
             <xsl:value-of select="./expression" disable-output-escaping="yes"/>;<xsl:text></xsl:text>
        </xsl:if>
        <xsl:if test="./function">
           if(!<xsl:value-of select="./function" disable-output-escaping="yes"/>()){return 0;}<xsl:text></xsl:text>
        </xsl:if>
        <xsl:if test="./condition"><xsl:text></xsl:text>          
      }
<xsl:text></xsl:text>
      </xsl:if>
      </xsl:for-each>
      <xsl:text disable-output-escaping="yes">      PROCESS-&gt;isEn = 1;
    }
    else
    {
      PROCESS-&gt;isEn = 0;
    }      

    return 1;
}</xsl:text>  
    </xsl:for-each>
  </xsl:template>

  <!--子策略或子块-->
  <xsl:template match="/DCS/Function">
// 子策略或子块函数
uint8 <xsl:value-of select="@name"/>(void)
{
<xsl:text></xsl:text>
    <xsl:apply-templates select="instruction"/>
    <xsl:text>    return 1;</xsl:text>
}
  </xsl:template>


  <!--主策略-->
  <xsl:template match="/DCS/Main">
// 组态控制
uint8 <xsl:value-of select="$PredixFun"/>Control()
{
<xsl:apply-templates select="instruction"/>
<xsl:text>        return 1;</xsl:text>
}
  </xsl:template>

  <!--处理每条指令-->
  <xsl:template match="instruction">
      <xsl:text>    if(!</xsl:text>
      <xsl:value-of select="@name"/>
      <xsl:text>(</xsl:text>
      <!--循环每个参数-->
      <xsl:for-each select="param">
        <xsl:value-of select="." disable-output-escaping="yes"/>
        <xsl:if test="position()!=last()">,</xsl:if>
      </xsl:for-each>
      <xsl:text>)){return 0;}
</xsl:text>

  </xsl:template>


</xsl:stylesheet>