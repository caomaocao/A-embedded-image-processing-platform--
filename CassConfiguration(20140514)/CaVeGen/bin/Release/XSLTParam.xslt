<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration="yes" indent="yes"/>

  <!--根匹配-->
  <xsl:template match="/">
    <!--主函数开始-->
    <xsl:text disable-output-escaping="yes">
#include "Control.h"
#include "string.h"
#include "system.h";     
stack paramStack;
</xsl:text>
    <!--全局变量设置-->
    <xsl:text>// 定时器变量设置    
unsigned long timerCount = 0;
// 采样周期
unsigned long baseTimeInterval = </xsl:text>
    <xsl:value-of select="/DCS/Cycle"/>;<xsl:text>
// 上次的记录时间
unsigned long lastTimeRec = 0;</xsl:text>
    
    <xsl:apply-templates select="/DCS/Parameters"/>   
    <xsl:apply-templates select="/DCS/Calculators"/>
    <xsl:apply-templates select="/DCS/ConditionActions"/>
    <xsl:apply-templates select="/DCS/Function"/>
    <xsl:apply-templates select="/DCS/Main"/>
  </xsl:template>

  <!--初始化参数函数匹配,这个要放到最先-->
  <xsl:template match="/DCS/Parameters">
// 全局变量
<xsl:text></xsl:text>
    <xsl:for-each select="Parameter">
<xsl:value-of select="@type"/>Struct <xsl:value-of select="@name"/>;
<xsl:text></xsl:text>
    </xsl:for-each>
    <xsl:text>
// 初始化参数
void InitParam(void)
{
	LinkList L;
	L = CreateLinkList(L);
  </xsl:text>
    <xsl:apply-templates select="Parameter"/>
    // 写入到E2PROM
    WriteListE2PROM(L);
}
  </xsl:template>

  <!--各个参数匹配-->
  <xsl:template match="Parameter">
    <xsl:param name="name" select="@name"/>
    // <xsl:value-of select="$name"/>
    <xsl:text>
</xsl:text>
    <!--E2PROM参数-->
    <xsl:for-each select="E2PROM/item">
      <xsl:text>    Add</xsl:text>
      <xsl:value-of select="@type"/>
      <xsl:text disable-output-escaping="yes">(&amp;L,</xsl:text>
      <xsl:value-of select="."/>
      <xsl:text disable-output-escaping="yes">,&amp;</xsl:text>
      <xsl:value-of select="$name"/>.<xsl:value-of select="@name"/><xsl:text>);
</xsl:text>
    </xsl:for-each>
    <!--其它参数-->
    <xsl:for-each select="others/item">
      <xsl:text>    </xsl:text>
      <xsl:value-of select="$name"/>.<xsl:value-of select="@name"/> = <xsl:value-of select="."/>
      <xsl:text>;
</xsl:text>
    </xsl:for-each>
  </xsl:template>

  <!--计算器-->
  <xsl:template match="/DCS/Calculators">
    <xsl:for-each select="Calculator">
      <xsl:variable name="varCalcu" select="@name"/>
// 计算器
void <xsl:value-of select="$varCalcu"/>Control(CALCUStruct *CACUL/>)
{
      float X0,X1,X2,X3,X4,X5,X6,X7;
      float Y0,Y1,Y2,Y3;

      <xsl:text disable-output-escaping="yes">Pop(&amp;paramStack,X0);
      Pop(&amp;paramStack,X1);
      Pop(&amp;paramStack,X2);
      Pop(&amp;paramStack,X3);
      Pop(&amp;paramStack,X4);
      Pop(&amp;paramStack,X5);
      Pop(&amp;paramStack,X6);
      Pop(&amp;paramStack,X7);
      Y0 = 0f;
      Y1 = 0f;
      Y2 = 0f;
      Y3 = 0f;
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
void <xsl:value-of select="$varCalcu"/>Control()
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
          <xsl:value-of select="./function" disable-output-escaping="yes"/>Fun();<xsl:text></xsl:text>
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
void <xsl:value-of select="@name"/>Fun(void)
{
<xsl:text></xsl:text>
    <xsl:apply-templates select="instruction"/>
}
  </xsl:template>


  <!--主策略-->
  <xsl:template match="/DCS/Main">
    <xsl:text disable-output-escaping="yes">
// 组态控制
void Control()
{
</xsl:text>
    <xsl:apply-templates select="instruction"/>
<xsl:text disable-output-escaping="yes">
}
// 主函数
int main(void)
{    
     // 设置晶振为系统时钟
     SysCtlClockSet(SYSCTL_SYSDIV_1 | SYSCTL_USE_OSC | SYSCTL_OSC_MAIN |
                   SYSCTL_XTAL_6MHZ);
	

	  // 使能本例所使用的外设。
    SysCtlPeripheralEnable(SYSCTL_PERIPH_GPIOB);

	  // 使能处理器中断
    IntMasterEnable();	

	  // 定时器0:1ms
	  ConfigTimer1ms();	

   	InitStack(&amp;paramStack);

	  InitParam();

    while(1)
	  {
	 	  if(timerCount - lastTimeRec >= baseTimeInterval)
		  { // 时间差到点
			   Control();
			   lastTimeRec = timerCount;
		   }	
	  }
}
</xsl:text>
  </xsl:template>

  <!--处理每条指令-->
  <xsl:template match="instruction">
    <xsl:choose>
      <!--CALL指令的特殊处理-->
      <xsl:when test="@name='CALL'">
        <xsl:text>    </xsl:text><xsl:value-of select="./param"/>Fun();
<xsl:text></xsl:text>
      </xsl:when>
      <!--CALL指令的特殊处理-->
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