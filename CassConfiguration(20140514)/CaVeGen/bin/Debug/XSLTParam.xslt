<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output omit-xml-declaration="yes"/>
<!--变量前缀-->
<xsl:variable name="PredixVar">configuration<xsl:value-of select="/DCS/ProjectIndex"/>_</xsl:variable>
<!--函数前缀-->
<xsl:variable name="PredixFun">Configuration<xsl:value-of select="/DCS/ProjectIndex"/>_</xsl:variable>
<!--根匹配-->
<!--主函数开始-->
<xsl:template match="/"><xsl:text disable-output-escaping="yes">#include "cass\configuration_control.h"
#include "cass\configuration_system.h"
#include "cass\configuration_address.h"
#include "stm32f10x_type.h"


</xsl:text>
    <!--全局变量设置-->
    <xsl:apply-templates select="/DCS/EntireVarRegion"/>
    <xsl:apply-templates select="/DCS/Parameters"/>   
    <xsl:apply-templates select="/DCS/Calculators"/>
    <xsl:apply-templates select="/DCS/Function"/>
    <xsl:apply-templates select="/DCS/ConditionActions"/>    
    <xsl:apply-templates select="/DCS/Main"/>
  </xsl:template>

  <!-- 多输出的中间变量-->
  <xsl:template match="/DCS/EntireVarRegion">
    <!--// 多输出的中间变量-->
<xsl:for-each select="item">
<xsl:value-of select="@type"/><xsl:text> </xsl:text><xsl:value-of select="."/>;
</xsl:for-each>
  </xsl:template>
  
<!--初始化参数函数匹配,这个要放到最先-->
<xsl:template match="/DCS/Parameters">
// 全局变量
//自动部分
<xsl:text></xsl:text>
<xsl:for-each select="Parameter">
<xsl:value-of select="@type"/>Struct <xsl:value-of select="@name"/>;
<xsl:text></xsl:text>
</xsl:for-each>

  //引擎里定义
  extern uint8 raw_data[];
  extern uint8 out_data[];


  //手动部分
  //图像全局变量
  uint16 cass_mv_boarder_width;
  uint16 cass_mv_boarder_height;
  uint16 cass_mv_border_leftbound;
  uint16 cass_mv_border_topbound;
  uint8  cass_mv_feature_point_number;
  uint8 cass_mv_match_result;


  #include "featureArea.h"


  //工程数据
  //读取main.c，包含有特殊字符串的这一行，把feature.c(周红杰提供)里定义的工程数据变量内容插到这里
  //插入工程数据变量
  //feather.h

  //插入结束

  //模块变量
  //二值化阈值  0-255
  uint8 cass_mv_binary_threshold;

  //高斯核算子大小，3，5，7
  uint8 cass_mv_gaussian_length;

  //拉普拉斯算子大小，3，5，7
  uint8 cass_mv_laplace_length;

  //特征点匹配百分比，最大100最小0
  uint8 cass_mv_match_percent;

  //腐蚀强度,5,9,11
  uint8 cass_mv_erode_strenth;

  //在生成main.c和configuration_ControlFuns.c后，在main.c里找到包含有特殊字符串的这一行，然后把
  //configuration_ControlFuns.c整个文件的内容插入到这一行之后，生成新的cass_mv_main.c。原来的两个.c文件删掉。
  //插入模块函数
  //configuration_ControlFuns.c

  //插入结束

  //初始化参数，嵌入式版在VD区初始化时完成；
  static void <xsl:value-of select="$PredixFun"/>InitParam(void)
{
<xsl:apply-templates select="Parameter"/>   
     
}  </xsl:template> 
  
  <!--各个参数匹配-->
  <xsl:template match="Parameter">
    <xsl:param name="name" select="@name"/>
    <xsl:param name="type" select="@type"/>
    <xsl:param name ="index" select="@index"/>
    <xsl:text>
    </xsl:text>
	 <!--各控件序号-->
	 <xsl:value-of select="$name"/>.index = <xsl:value-of select="$index"/>;
    <!--E2PROM参数-->
    <xsl:for-each select="E2PROM/item">
        <xsl:text></xsl:text>
      <xsl:value-of select="$name"/>.<xsl:value-of select="@name"/> = Configuration_ReadPara<xsl:value-of select="@type"/>(<xsl:value-of select="$name"/>_<xsl:value-of select="@name"/>_Addr);
    </xsl:for-each>
    <!--其它参数-->
    <xsl:for-each select="others/item">  
      <xsl:value-of select="$name"/>.<xsl:value-of select="@name"/> = <xsl:value-of select="."/>;  
	  <xsl:text></xsl:text>
    </xsl:for-each>
  </xsl:template>

  <!--计算器-->
  <xsl:template match="/DCS/Calculators">
    <xsl:for-each select="Calculator">
      <xsl:variable name="varCalcu" select="@name"/>
// 计算器
static void <xsl:value-of select="$PredixFun"/><xsl:value-of select="$varCalcu"/>Control(CACULStruct *CACUL)
{
      fp32 X0,X1,X2,X3,X4,X5,X6,X7;
      fp32 Y0,Y1,Y2,Y3;

      <xsl:text disable-output-escaping="yes">Pop(&amp;paramStack,&amp;X0);
      Pop(&amp;paramStack,&amp;X1);
      Pop(&amp;paramStack,&amp;X2);
      Pop(&amp;paramStack,&amp;X3);
      Pop(&amp;paramStack,&amp;X4);
      Pop(&amp;paramStack,&amp;X5);
      Pop(&amp;paramStack,&amp;X6);
      Pop(&amp;paramStack,&amp;X7);
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
      </xsl:text>
}

    </xsl:for-each>
  </xsl:template>


  <!--条件动作表-->
  <xsl:template match="/DCS/ConditionActions">
    <xsl:for-each select="ConditionAction">
      <xsl:variable name="varCalcu" select="@name"/>
// 条件动作表
static void <xsl:value-of select="$PredixFun"/><xsl:value-of select="$varCalcu"/>Control()
{
<xsl:text></xsl:text>
      <!--每个条件-->
      <xsl:for-each select="item">
        <xsl:if test="./condition">
          <xsl:text></xsl:text>   if(<xsl:for-each select="./condition">            
              <xsl:value-of select="." disable-output-escaping="yes"/>
            <xsl:if test="position() != last()">
              <xsl:text disable-output-escaping="yes">&amp;&amp;</xsl:text>
            </xsl:if>
            <xsl:if test="position() = last()">)
   {
      <xsl:text></xsl:text>
            </xsl:if>
          </xsl:for-each>
        </xsl:if>
        <xsl:if test="./expression">        
          <xsl:value-of select="./expression" disable-output-escaping="yes"/>;<xsl:text></xsl:text>
        </xsl:if>
        <xsl:if test="./function">
          <xsl:value-of select="$PredixFun"/><xsl:value-of select="./function" disable-output-escaping="yes"/>();<xsl:text></xsl:text>
        </xsl:if>
        <xsl:if test="./condition"><xsl:text></xsl:text>          
   }
<xsl:text></xsl:text>
      </xsl:if>
      </xsl:for-each>
}
    </xsl:for-each>
  </xsl:template>

  <!--子策略-->
  <xsl:template match="/DCS/Function">
// 子策略函数
static void <!--xsl:value-of select="$PredixFun"/--><xsl:value-of select="@name"/>(void)
{
<xsl:text></xsl:text>
    <xsl:apply-templates select="instruction"/>
}
  </xsl:template>


<!--主策略-->
<xsl:template match="/DCS/Main">
// 组态控制
static void <xsl:value-of select="$PredixFun" disable-output-escaping="no"/>Control()
{
   <xsl:text disable-output-escaping="yes">
    int i = 0;
    for(i=0;i&lt;(sizeof(local_area_array)/sizeof(space_number_info));i++)
    {
            //初始化
			  cass_mv_border_leftbound =local_area_array[i].leftbound;	
		    cass_mv_border_topbound=local_area_array[i].topbound;
		    cass_mv_border_width=local_area_array[i].width;
		    cass_mv_border_height=local_area_array[i].height;
   </xsl:text>
		<xsl:apply-templates select="instruction"/>
	}

}

/********************************************************
提供给CASS平台的接口函数
*********************************************************/
int cass_mv_main_entry(void)
{
    <xsl:value-of select="$PredixFun"/>Control();
    return 1;
}
int cass_mv_main_init(void)
{
    <xsl:value-of select="$PredixFun"/>InitParam();
    return 1;
}
</xsl:template>

  <!--处理每条指令-->
  <xsl:template match="instruction">
    <xsl:choose>
      <!--CALL指令的特殊处理-->
      <xsl:when test="@name='CALLControl'">
        <xsl:text>    </xsl:text><xsl:value-of select="./param"/>();
<xsl:text></xsl:text>
      </xsl:when>
      <!--RETURN指令的特殊处理-->
      <xsl:when test="@name='RETControl'">
        <xsl:text></xsl:text>    if(RETControl())
    {
        return;
    }
<xsl:text></xsl:text>
      </xsl:when>
      <!--其它指令-->
      <xsl:otherwise>
        <xsl:text>    </xsl:text>
        <xsl:value-of select="@name"/>
        <xsl:text>(</xsl:text>
        <!--循环每个参数-->
        <xsl:for-each select="param">
          <xsl:value-of select="." disable-output-escaping="yes"/>
          <xsl:if test="position()!=last()">,</xsl:if>
        </xsl:for-each>
        <xsl:text>);
</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>


</xsl:stylesheet>