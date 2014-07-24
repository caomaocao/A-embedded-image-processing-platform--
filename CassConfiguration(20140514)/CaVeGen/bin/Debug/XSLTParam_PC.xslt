<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration="yes"/>
<!--变量前缀-->
<xsl:variable name="PredixVar">configuration<xsl:value-of select="/DCS/ProjectIndex"/>_</xsl:variable>
<!--函数前缀-->
<xsl:variable name="PredixFun">Configuration<xsl:value-of select="/DCS/ProjectIndex"/>_</xsl:variable>
<!--根匹配-->
<!--主函数开始-->
<xsl:template match="/"><xsl:text disable-output-escaping="yes">#include "configuration_control.h"
#include "configuration_system.h"
#include "configuration_address.h"


#define CASS_MV_PC_VERSION

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
  <xsl:text></xsl:text>
<xsl:for-each select="Parameter">
<xsl:value-of select="@type"/>Struct <xsl:value-of select="@name"/>;
<xsl:text></xsl:text>
</xsl:for-each>
    unsigned char raw_data[360000],out_data[360000];//临时图像数组空间
    unsigned char bmpinfoarray[54]={66,77,184,91,0,0,0,0,0,0,54,4,0,0,40,0,0,0,50,0,0,0,50,0,0,0,1,0,8,0,0,0,0,0,130,87,0,0,18,11,0,0,18,11,0,0,0,0,0,0,0,0,0,0};//bmp文件头信息头存放数组
    unsigned char plltee[1024]= {0,0,0,0,1,1,1,0,2,2,2,0,3,3,3,0,4,4,4,0,5,5,5,0,6,6,6,0,7,7,7,0,8,8,8,0,9,9,9,0,10,10,10,0,11,11,11,0,12,12,12,0,13,13,13,0,14,14,14,0,15,15,15,0,16,16,16,0,17,17,17,0,18,18,18,0,19,19,19,0,20,20,20,0,21,21,21,0,22,22,22,0,23,23,23,0,24,24,24,0,25,25,25,0,26,26,26,0,27,27,27,0,28,28,28,0,29,29,29,0,30,30,30,0,31,31,31,0,32,32,32,0,33,33,33,0,34,34,34,0,35,35,35,0,36,36,36,0,37,37,37,0,38,38,38,0,39,39,39,0,40,40,40,0,41,41,41,0,42,42,42,0,43,43,43,0,44,44,44,0,45,45,45,0,46,46,46,0,47,47,47,0,48,48,48,0,49,49,49,0,50,50,50,0,51,51,51,0,52,52,52,0,53,53,53,0,54,54,54,0,55,55,55,0,56,56,56,0,57,57,57,0,58,58,58,0,59,59,59,0,60,60,60,0,61,61,61,0,62,62,62,0,63,63,63,0,64,64,64,0,65,65,65,0,66,66,66,0,67,67,67,0,68,68,68,0,69,69,69,0,70,70,70,0,71,71,71,0,72,72,72,0,73,73,73,0,74,74,74,0,75,75,75,0,76,76,76,0,77,77,77,0,78,78,78,0,79,79,79,0,80,80,80,0,81,81,81,0,82,82,82,0,83,83,83,0,84,84,84,0,85,85,85,0,86,86,86,0,87,87,87,0,88,88,88,0,89,89,89,0,90,90,90,0,91,91,91,0,92,92,92,0,93,93,93,0,94,94,94,0,95,95,95,0,96,96,96,0,97,97,97,0,98,98,98,0,99,99,99,0,100,100,100,0,101,101,101,0,102,102,102,0,103,103,103,0,104,104,104,0,105,105,105,0,106,106,106,0,107,107,107,0,108,108,108,0,109,109,109,0,110,110,110,0,111,111,111,0,112,112,112,0,113,113,113,0,114,114,114,0,115,115,115,0,116,116,116,0,117,117,117,0,118,118,118,0,119,119,119,0,120,120,120,0,121,121,121,0,122,122,122,0,123,123,123,0,124,124,124,0,125,125,125,0,126,126,126,0,127,127,127,0,128,128,128,0,129,129,129,0,130,130,130,0,131,131,131,0,132,132,132,0,133,133,133,0,134,134,134,0,135,135,135,0,136,136,136,0,137,137,137,0,138,138,138,0,139,139,139,0,140,140,140,0,141,141,141,0,142,142,142,0,143,143,143,0,144,144,144,0,145,145,145,0,146,146,146,0,147,147,147,0,148,148,148,0,149,149,149,0,150,150,150,0,151,151,151,0,152,152,152,0,153,153,153,0,154,154,154,0,155,155,155,0,156,156,156,0,157,157,157,0,158,158,158,0,159,159,159,0,160,160,160,0,161,161,161,0,162,162,162,0,163,163,163,0,164,164,164,0,165,165,165,0,166,166,166,0,167,167,167,0,168,168,168,0,169,169,169,0,170,170,170,0,171,171,171,0,172,172,172,0,173,173,173,0,174,174,174,0,175,175,175,0,176,176,176,0,177,177,177,0,178,178,178,0,179,179,179,0,180,180,180,0,181,181,181,0,182,182,182,0,183,183,183,0,184,184,184,0,185,185,185,0,186,186,186,0,187,187,187,0,188,188,188,0,189,189,189,0,190,190,190,0,191,191,191,0,192,192,192,0,193,193,193,0,194,194,194,0,195,195,195,0,196,196,196,0,197,197,197,0,198,198,198,0,199,199,199,0,200,200,200,0,201,201,201,0,202,202,202,0,203,203,203,0,204,204,204,0,205,205,205,0,206,206,206,0,207,207,207,0,208,208,208,0,209,209,209,0,210,210,210,0,211,211,211,0,212,212,212,0,213,213,213,0,214,214,214,0,215,215,215,0,216,216,216,0,217,217,217,0,218,218,218,0,219,219,219,0,220,220,220,0,221,221,221,0,222,222,222,0,223,223,223,0,224,224,224,0,225,225,225,0,226,226,226,0,227,227,227,0,228,228,228,0,229,229,229,0,230,230,230,0,231,231,231,0,232,232,232,0,233,233,233,0,234,234,234,0,235,235,235,0,236,236,236,0,237,237,237,0,238,238,238,0,239,239,239,0,240,240,240,0,241,241,241,0,242,242,242,0,243,243,243,0,244,244,244,0,245,245,245,0,246,246,246,0,247,247,247,0,248,248,248,0,249,249,249,0,250,250,250,0,251,251,251,0,252,252,252,0,253,253,253,0,254,254,254,0,255,255,255,0};//Bmp调色板存放数组
    int dotarraynumber[1000];//限制特征点不超过250个

    //全局变量
    uint16 cass_mv_border_width;
    uint16 cass_mv_border_height;
    uint16 cass_mv_border_leftbound;
    uint16 cass_mv_border_topbound;
    uint8  cass_mv_feature_point_number;

    //特征点匹配百分比，最大100最小0
    char* cass_mv_match_percent;  //系统匹配
    uint8 cass_mv_match_result;   //当前匹配结果


    //extern char *cass_mv_cameraIn_serialportname ; //串口号
    //extern int cass_mv_cameraIn_serialBaudrate;      //波特率
    //extern uint8 cass_mv_cameraIn_serialParity;  ;    //奇偶校验
    //extern uint8 cass_mv_cameraIn_serialStopbits;     //停止位
    //extern uint8 cass_mv_cameraIn_serialDatabits;  //数据位



    BMPINFO bmpinfo;
    #include "feather.h"
    #include"saveimg.c"
    #include "serial.c"
    #include "configuration_ControlFuns.c"
    #include "featureArea.h"

    // 初始化参数
    void <xsl:value-of select="$PredixFun"/>InitParam(void)
{<xsl:apply-templates select="Parameter"/>        
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
void <xsl:value-of select="$PredixFun"/><xsl:value-of select="$varCalcu"/>Control(CACULStruct *CACUL)
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
void <xsl:value-of select="$PredixFun"/><xsl:value-of select="$varCalcu"/>Control()
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
void <!--xsl:value-of select="$PredixFun"/--><xsl:value-of select="@name"/>(void)
{
<xsl:text></xsl:text>
    <xsl:apply-templates select="instruction"/>
}
  </xsl:template>


  <!--主策略-->
  <xsl:template match="/DCS/Main">
// 组态控制
void <xsl:value-of select="$PredixFun"/>Control()
{
      <xsl:text disable-output-escaping="yes">
	    int i=0;
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
// 主函数
int main(void)
{
    <xsl:value-of select="$PredixFun"/>InitParam();
  	<xsl:value-of select="$PredixFun"/>Control();
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