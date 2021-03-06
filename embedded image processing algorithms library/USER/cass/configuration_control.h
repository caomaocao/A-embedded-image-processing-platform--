#ifndef _CONFIGURATION_CONTROL_H
#define _CONFIGURATION_CONTROL_H

#include "configuration_stack.h"
#include "configuration_queue.h"
#include "configuration_system.h"
#include <math.h>
#include <stdlib.h>
#include<string.h>
#include<stdio.h>
#include "CassType.h"
#include "CassSysVar.h"

extern stack paramStack;

//感兴趣区域参数
typedef struct region_of_interest_desc_type_
{
	int x_start;
	int y_start;
	int x_end;
	int y_end;
} region_of_interest_desc_type;  //(0,0,0,0)表示选中整个图像

//镜头参数
typedef struct camera_param_desc_type_
{
		int chroma;
		int luminance;
		int contrast;
		int saturation;
		int rsd[4];
} camera_param_desc_type;

/*特征点结构*/
typedef struct point_feature_desc_type_
{
	int x;          //x坐标
	int y;          //y坐标
	int v;          //像素值  
	int w;           //权重
} point_feature_desc_type;

/*每套区域包含的信息：小框位置，10个数字共300个特征点信息*/
typedef struct small_point_space_desc_type_
{
	int leftbound;          //左上角宽度坐标
	int topbound;
	int width;				//宽	       
	int height;
	point_feature_desc_type  point[300];
} small_point_feature_desc_type;

//工程数据包
typedef struct image_data_packet_desc_type_
{
	int updated;  //版本信息version    
	int isGray;
	camera_param_desc_type     camera_desc;//镜头参数
	region_of_interest_desc_type  roi_desc;	//截图区域信息
	small_point_feature_desc_type space[10] ;//区域模板信息
} image_data_packet_desc_type;
 

/*bmp信息结构体*/
typedef struct          
{
  long fileSize;
	long width;
	long height;	
	long dwSize;
} BMPINFO;

//参数结构体

//识别处理
typedef struct
{
	uint8 index;
	uint8 percent;
}MATCHStruct;

/*typedef struct
{
    uint8 index;
}FILEOUTStruct;*/


//设备输入
typedef struct
{
	uint8 index;
	char  *portName; //串口号
    int baudRate; //波特率
	uint8 parity;  //奇偶校验
	uint8 stopBits; //停止位
	uint8 dataBits;  //数据位
}CAMERAINStruct;

//高斯滤波
typedef struct
{
	uint8 index;
	uint8 length;
}GAUSSIANSMOOTHINGStruct;

//中值滤波
typedef struct
{
	uint8 index;
	uint8 length;
}MEDIANFILTERStruct;

//均值滤波
typedef struct
{
	uint8 index;
	uint8 length;
}MEANFILTERStruct;

//索贝尔处理
typedef struct
{
	uint8 index;
	uint8 threshold;
}SOBELStruct;

//普锐维特处理
typedef struct
{
	uint8 index;
	uint8 threshold;
}PREWITTStruct;

//拉普拉斯处理
typedef struct
{
	uint8 index;

}LAPLACEStruct;


//二值化
typedef struct
{
	uint8 index;
	uint8 threshold;
}BINARYStruct;

//最优二值化
typedef struct
{
	uint8 index;
}OSTUBINARYStruct;

//反转颜色
typedef struct
{
	uint8 index;
}ANTICOLORStruct;

//灰度拉伸
typedef struct
{
	uint8 index;
	uint8 left_gray_in;
	uint8 left_gray_out;
	uint8 right_gray_in;
	uint8 right_gray_out;
}GRAYSCALESTRETCHStruct;

//直方图均衡化
typedef struct
{
	uint8 index;
}HIAGRAMStruct;


//去噪
typedef struct
{
	uint8 index;
	uint8 noise_size;
}DENOSINGStruct;

//细化
typedef struct
{
	uint8 index;
}ZHANGTHINStruct;

//膨胀
typedef struct
{
	uint8 index;
	uint8 length;
}DIALATEStruct;


//腐蚀
typedef struct
{
	uint8 index;
	uint8 length;
}ERODEStruct;

//开操作
typedef struct
{
	uint8 index;
	uint8 length;
}OPENOPERATIONStruct;

//闭操作
typedef struct
{
	uint8 index;
	uint8 length;
}CLOSEOPERATIONStruct;

//卡尼边缘检测
typedef struct
{
	uint8 index;
	uint8 low_threshold;
	uint8 high_threshold;
}CANNYStruct;

//霍夫直线检测
typedef struct
{
	uint8 index;
}HOUGHLINESStruct;

//掏空
typedef struct
{
	uint8 index;
}HOLLOWEDStruct;

//哈里斯角点
typedef struct
{
	uint8 index;
	uint8 low_threshold;
	uint8 high_threshold;
}HARRISStruct;

//区域重心
typedef struct
{
	uint8 index;
}CENTEROFGRAVITYStruct;

// 参数结构体
// "PID"

typedef struct
{
	fp32 Kp;	 // 比例
	#if defined PID_Ki_NOT0
	fp32 Ki;	 // 积分
	#endif
	fp32 Kd;	 // 微分

	uint32 Ts;	 // 周期
	fp32 ei;	 // 偏差
	fp32 ei_1;
	fp32 ei_2;
	// 紧急状态
	//uint8 ucIsEmergency;
	// 安全值
	//fp32 fSaveValue;
	// 给定值处理
	fp32 fSVL;  // 内给定值,由操作员给定
	#if defined PID_fSR_NOT0
	fp32 fSR;  // 给定值的变化率,正值	
	fp32 fSV;	// 记录上次的给定值
	#endif
	// 被控量处理
	fp32 fPH;  // 上限报警
	fp32 fPL;  // 下限报警
	uint8 cPHA;  // 上限状态
	uint8 cPLA;  // 下限状态
	fp32 fHY;  // 报警死区	   10
	#if defined PID_cSelect_3 || defined PID_cSelect_4 || defined PID_fPR_NOT0
	fp32 fPV; 	// 被控量
	#endif
	#if defined PID_fPR_NOT0
	fp32 fPR;  // 被控量的变化率
	#endif
	// 偏差处理	
	uint8 cDorR; // 0 正作用; 1 反作用
	fp32 fDL;  // 偏差报警值
	uint8 cDLA;  // 偏差报警状态
	fp32 fBSGP; // 偏差死区
	//fp32 fBSGN; // 偏差增益
    #if (defined PID_cICM_1 || defined PID_cICM_2 || defined PID_cICM_3)
	fp32 fICV;  // 输入补偿量 
	uint8 cICM;  // 输入补偿方式
	#endif
	// PID计算
	uint8 cSelect; // 算法选择	10
	fp32 fMV; // 输出值
	#if defined PID_cSelect_1	 
	fp32 fE0; // 积分分离因子
	#endif
	fp32 fMH; // 控制量上限
	fp32 fML; // 控制量下限
	#if defined PID_cSelect_3 || defined PID_cSelect_4
	fp32 fPV_1; // 上次的被控量
	fp32 fPV_2; // 上上次的被控量
	#endif
	// 输出处理
	#if defined PID_cOCM_1 || defined PID_cOCM_2 || defined PID_cOCM_3
	fp32 fOCV;  // 输出补偿量	
	uint8 cOCM;  // 输出补偿方式
	#endif
	#if defined PID_fMR_NOT0
	fp32 fMR;  // 变化率限制
	#endif
	#if defined PID_cFHorNH_1
	uint8 cFHorNH;  // 输出保持	10
	#endif
	#if defined PID_cFSorNS_1
	uint8 cFSorNS;  // 安全输出
	fp32 fMS;  // 安全输出量
	#endif
	// 自动／手动切换
	uint8 cCtrlMode; // ０：自动	１：手动
}PIDStruct;

// 纯滞后补偿
typedef struct
{
	uint32 ulT1;  // 时间常数1
	uint32 ulT2;  // 时间常数2
	uint32 ulLagT;  //滞后时间
	fp32 fK;  // 补偿比例
	uint32 ulTs;  // 采样周期
	fp32 fPV;  // 输出
	fp32 fMV_1; // 上上次输入
	LinkQueue *queueIn;  // 输入队列
	fp32 fMV;	 // 上次输入
	
	 	
}SMITHStruct;

// 最少拍控制
typedef struct
{
	fp32 fK;   // 被控对象的放大系数
	uint32 ulT1;  // 一阶被控对象的时间常数
	uint32 ulTao; // 被控对象的纯滞后时间
	uint32 ulTs;  // 采样周期
	fp32 fMH;  // 输出高限
	fp32 fML;  // 输出低限
	uint8 cCtrlMod;  // 0自动/1手动
	LinkQueue *queueMV;  // 输出队列
	fp32 fei;  // 上次的偏差
	fp32 fMV;  // 输出
	fp32 fSV;  // 设定值
}LEASTStruct;

// 大林控制
typedef struct
{
	fp32 fK;  // 被控对象的放大系数
	uint32 ulT1;  // 时间常数
	uint32 ulT2;  // 时间常数
	uint32 ulTs;  // 采样周期
	uint32 ulTTao;  // 闭环系统的时间常数 
	uint32 ulTao;  // 滞后时间
	fp32 fMH; // 输出高限
	fp32 fML; // 输出低限
	uint8 cCtrlMod; // 0自动/1手动
	LinkQueue *queueMV;  // 输出队列
	fp32 fMV; // 上次的输出,也存放着手动修改的值
	fp32 fMV_1; 
	fp32 fei;  // 上次的偏差
	fp32 fei_1; // 上上次的偏差
	fp32 fSV;  // 设定值
}DARLINStruct;

// 前馈控制器
typedef struct
{
	fp32 fK1;  // 扰动对象的放大系数
	fp32 fK2;  // 被控对象的放大系数
	uint32 ulT1;  // 扰动通道的时间常数
	uint32 ulT2;  // 被控对象的时间常数
	uint32 ulTs;  // 采样周期
	uint32 ulTTao1;  // 扰动通道的时间常数
	uint32 ulTTao2;  // 被控对象的时间常数
	fp32 fMH; // 输出高限
	fp32 fML; // 输出低限
	uint8 cCtrlMod; // 0自动/1手动
	fp32 fMV; // 上次的输出
	LinkQueue *queuePV;  // 输入队列   
}PREStruct;

// 解耦算法
typedef struct
{
	uint8 iN;  // 几路,最多255路
	uint8 hasMatrix; // 是否计算过解耦,开机时计算一次
	fp32 *paramMatrix;  // 解耦矩阵的参数
	fp32 *jieOuMatrix; // 解耦矩阵的结果
	fp32 *ei;   // 上次偏差数组
	fp32 *ei_1; // 上上次偏差数组
	fp32 *fUk;  // 上次控制器输出
	// 解耦中的PID参数,上面有几路,下面数组中就有几个
	fp32 *fKp;
	fp32 *fKi;
	fp32 *fKd;
	fp32 *fSV;	
	uint8  *cCtrlMod;
	fp32 *fMH; // 输出高限
	fp32 *fML; // 输出低限
	fp32 *fMV; // 输出
	uint32 ulTs; // 一个周期

}JIEOUStruct;

// 模糊控制
typedef struct
{
	fp32 *fuzzyControlList; // 模糊控制表,离线计算
	fp32 fEH; // 偏差最大值
	fp32 fECH; // 偏差变化率最大值
	fp32 fEi;  // 偏差
	fp32 fUH;  // 输出最值
	uint32 ulL;  // 偏差模糊论域
	uint32 ulM;  // 偏差变化率模糊论域
	uint32 ulN;  // 控制量模糊论域
	uint8 cCtrlMod;       // 自动/手动
	uint32 ulTs; // 周期
	fp32 fMV;  // 输出
	fp32 fSV;  // 设定值 
}FUZZYStruct;

// "与"
typedef struct
{
	uint8 type;  // 0: 位类型 1:字节类型 0xFF 2:字类型 0xFFFF 3:双字类型 0xFFFFFFFF
	fp32 fLastVal; // 上次的值
}ANDStruct;

// "或"
typedef struct
{
	uint8 type;  // 0: 位类型 1:字节类型 0xFF 2:字类型 0xFFFF 3:双字类型 0xFFFFFFFF
	fp32 fLastVal; // 上次的值
}ORStruct;

// "异或"
typedef struct
{
	uint8 type;  // 0: 位类型 1:字节类型 0xFF 2:字类型 0xFFFF 3:双字类型 0xFFFFFFFF
	  fp32 fLastVal; // 上次的值
}XORStruct;

// "非"
typedef struct
{
	uint8 type;  // 0: 位类型 1:字节类型 0xFF 2:字类型 0xFFFF 3:双字类型 0xFFFFFFFF
	fp32 fLastVal; // 上次的值
}NOTStruct;

// "与非"
typedef struct
{
	uint8 type;  // 0: 位类型 1:字节类型 0xFF 2:字类型 0xFFFF 3:双字类型 0xFFFFFFFF
	fp32 fLastVal; // 上次的值
}NANDStruct;

// "或非"
typedef struct
{
	uint8 type;  // 0: 位类型 1:字节类型 0xFF 2:字类型 0xFFFF 3:双字类型 0xFFFFFFFF
	fp32 fLastVal; // 上次的值
}NORStruct;

// "三输入不一致"
typedef struct
{
	uint8 type;  // 0: 位类型 1:字节类型 0xFF 2:字类型 0xFFFF 3:双字类型 0xFFFFFFFF
	//uint32 cycle;	
	uint8 lastValue;  // 上次的值,持续一个周期
}DISP3Struct;

// "开关"
typedef struct
{
	uint8 type;  // 0: 位类型 1:字节类型 0xFF 2:字类型 0xFFFF 3:双字类型 0xFFFFFFFF
	fp32 fLastVal; // 上次的值
}SWITStruct;

// "定长脉冲"
typedef struct
{
	uint32 cycle;  // 定长脉冲
	uint32 lastTimeRec;
	uint8 lastInValue;  // 上次的输入值
	uint8 lastOutValue;  // 上次的输出值
}PULSEStruct;

// "最大脉冲"
typedef struct
{
	uint32 cycle;  // 周期
	uint32 lastTimeRec;
	uint8 lastInValue;  // 上次的输入值
	uint8 lastOutValue;  // 上次的输出值
}MAXPLStruct;

// 最小脉冲
typedef struct
{
	uint32 cycle;  // 周期
	uint32 lastTimeRec;
	uint8 lastInValue;  // 上次的输入值
	uint8 lastOutValue;  // 上次的输出值
}MINPLStruct;

// RS
typedef struct
{
	uint8 lastValue;  // 上次的值
}RSStruct;

// DELAY
typedef struct
{
	uint32 cycle;
	uint32 lastTimeRec;
	uint8 lastOutValue;  
}DELAYStruct;

// ON延时
typedef struct
{
	uint32 cycle;
	uint32 lastTimeRec;
	uint8 lastOutValue;
	uint8 lastInValue;
	uint8 onSign;   // OFF->ON标志  
}ONDLYStruct;

// OFF延时
typedef struct
{
	uint32 cycle;
	uint32 lastTimeRec;
	uint8 lastOutValue;
	uint8 lastInValue;
	uint8 offSign;   // ON->OFF标志  
}OFFDLYStruct;

// 变化检测
typedef struct
{
	uint8 lastIn1;
	uint8 lastIn2;
	uint8 lastIn3; 
	fp32 fLastVal; // 上次的值 
}CHDCTStruct;
// 看门狗
typedef struct
{
	uint32 cycle;
	uint32 lastTimeRec;	
	uint8 lastInValue;
	uint8 isTrigger;  // 是否触发
	fp32 fLastVal; // 上次的值	  
}WTDOGStruct;

// 选通器
typedef struct
{
	uint8 cIsMin1;  // 是否开启选能低限1功能
	uint8 cIsMax1;  // 是否开启选能高限1功能	
	fp32 fMin1;   // 选通1低限
	fp32 fMax1;   // 选能1高限
	
	uint8 cIsMin2;  // 是否开启选能低限2功能
	uint8 cIsMax2;  // 是否开启选能高限2功能	
	fp32 fMin2;   // 选通2低限
	fp32 fMax2;   // 选能2高限
	
	uint8 cIsMin3;  // 是否开启选能低限3功能
	uint8 cIsMax3;  // 是否开启选能高限3功能	
	fp32 fMin3;   // 选通3低限
	fp32 fMax3;   // 选能3高限
	
	uint8 cIsMin4;  // 是否开启选能低限4功能
	uint8 cIsMax4;  // 是否开启选能高限4功能	
	fp32 fMin4;   // 选通4低限
	fp32 fMax4;   // 选能4高限
	
	uint8 cIsMin5;  // 是否开启选能低限5功能
	uint8 cIsMax5; // 是否开启选能高限5功能	
	fp32 fMin5;   // 选通5低限
	fp32 fMax5;   // 选能5高限
	
	uint8 cIsMin6;  // 是否开启选能低限6功能
	uint8 cIsMax6;  // 是否开启选能高限6功能	
	fp32 fMin6;   // 选通6低限
	fp32 fMax6;   // 选能6高限
	
	uint8 cIsMin7;  // 是否开启选能低限7功能
	uint8 cIsMax7;  // 是否开启选能高限7功能	
	fp32 fMin7;   // 选通7低限
	fp32 fMax7;   // 选能7高限
	
	uint8 cIsMin8;  // 是否开启选能低限8功能
	uint8 cIsMax8;  // 是否开启选能高限8功能	
	fp32 fMin8;   // 选通8低限
	fp32 fMax8;   // 选能8高限

	fp32 fLastVal1; // 上次的值
	fp32 fLastVal2; // 上次的值
	fp32 fLastVal3; // 上次的值
	fp32 fLastVal4; // 上次的值
	fp32 fLastVal5; // 上次的值
	fp32 fLastVal6; // 上次的值
	fp32 fLastVal7; // 上次的值
	fp32 fLastVal8; // 上次的值
}SELECTStruct;
// 脉冲输入
typedef struct 
{
	fp32 fC1;   // 工程单位转换因子
	fp32 fC2;   // 仪表系数
	uint8  ucTBase;  // 基本时间单位
	
}PIStruct;

// 滤波
typedef struct 
{
	uint32  ulTs;  // 采样周期
	uint32  ulTF;	// 滤波时间常数
	fp32 fLastPV;  // 上次的输出	
}FILTStruct;

// 一阶传递函数
typedef struct
{
	// 四个参数
	fp32 A;
	fp32 B;
	fp32 C;
	fp32 D;
	fp32 Xk; // 中间临时变量
	fp32 Uk; // 上次输入	
	uint32 Ts;
}TRANSStruct;

// 计数器点
typedef struct 
{
	uint32 ulCount;   // 计数器当前的计数值
	uint32 ulSetValue;   // 计数的设定值
	uint8 ucType;  // 计数的类型: 0,上升; 1,下降; 2,高电平; 3,低电平
	uint8 ucIsComplete;  // 计数是否完成
	uint8 ucIsAutoReset;  // 是否自动复位
	fp32 fLastInValue;  // 上次的输入值
}CNTStruct;

// 计时器点
typedef struct
{
	uint32 ulTimer;   // 当前的计时值
	uint32 ulSetTimer;  // 预设值
	uint8 ucIsFinish;  // 是否完成
	uint8 ucAutoReset;  // 是否自动复位
	uint32 ulLastTimeRec; // 上次记录的时间
}TIMStruct;

// 累计点
typedef struct
{
	fp32 accValue;  // 累计值
	fp32 setValue;  // 预设值
	uint32 TBase;     // 时间基数,以秒为单位
	uint8 isTot;  // 是否累计
	uint8 isComplete; // 累计是否完成
	fp32 valveValue;  // 阀值,也就是小信号切除
	uint8 isAuto;  // 是否自动复位
	uint32 lastTimeRec; // 上次采集的时间
}TOTStruct;

// 限值器
typedef struct
{
	fp32 fMin;  // 低限
	fp32 fMax;  // 高限
}LIMITStruct;

// 比例器
typedef struct
{
	fp32 fMin;  // 低限
	fp32 fMax;  // 高限
}PERCENTStruct;

// 比值控制器
typedef struct
{
	fp32 fInMin;  // 输入低限
	fp32 fInMax;  // 输入高限
	fp32 fOutMin;  // 输出低限
	fp32 fOutMax;  // 输出高限
	fp32 fRation;  // 比值设定
	fp32 fClimbCon;  // 爬坡常数
	fp32 fBias;   // 偏差
	uint8 ucCtrlMod;  // 是否是自动
	fp32 fPV;  // 当手动时,输入值
	fp32 fMV;   // 当手动时,输出值
}RATIONStruct;

// 斜坡控制点
typedef struct
{
	uint8 sectionNum;  // 有效段数,上位机要算出
	uint8 outNum;  // 输出段数
	uint32 controlTime;  // 控制时间,最长控制时间1193小时,即49天,上位机控制
	//uint8 timeUnit;  // 时间单位
	uint8 ctrlMod;
	#if defined RAMP_manStyle_1
	uint8 manStyle;  // 手动时计时方式,0表示停止,1表示继续计时
	#endif
	uint8 terminateStyle;  // 终止方式,0表示一个时间段完成后重新计时,非0表示
								   // 时间段完成后停止控制,输出控制终值;是否是循环控制
								   // 还是说控制一遍后不再控制
	fp32  initOutValue;   // 初始的输出值
	fp32 endOutValue;   // 终止值
	uint32 time[24];  // 没放入起始时间0,时间段,单位为ms
	fp32 outValue[24];   // 没放入初值和末值,输出段
	fp32 fMV;  // 手动输出
	uint32 lastTimeRec;
}RAMPStruct;

// 开关控制点51
typedef struct
{
	uint8 ucSelect; // 选择
	//fp32 fPV; // 输出
}SW51Struct;

 // 温压补偿器
typedef struct
{
	uint8 ucType;  // 补偿类型 0:液体 1:气体与蒸汽 
						   // 2:气体与蒸汽(比重) 3:气体和蒸汽体积流量
						   // 4:蒸汽
	fp32 fRG;  // 设计比重
	fp32 fRP;  // 设计压力
	fp32 fRQ;  // 设计蒸汽参数因子
	fp32 fRT;  // 设计温度
	fp32 fP0;  // 表压转换系数
	fp32 fT0;  // 温度转换系数
	fp32 fRX;  // 设计蒸汽压缩系数
}FLCPStruct;

// 点的结构体
typedef struct
{
	fp32 x;
	fp32 y;
}PointStruct;
// 通用线性化点
typedef struct
{
	fp32 fInMin; // 输入的最值
	fp32 fInMax;
	fp32 fOutMin; // 输出的最值
	fp32 fOutMax;

	PointStruct pt[12]; 	

	uint8 effNum; // 有效点的个数
}GLINStruct;

// 线性变换器
typedef struct
{
	fp32 fCoeffient;// 系数
	fp32 fBias;  //偏差
}LICHStruct; 

// 高低选择器
typedef struct
{
	uint8 ucSelect; // 0:max 1:min 2:average
	uint8 ucEffNum; // 有效个数
}SWHLStruct; 

// 三者取中器
typedef struct
{
	#if defined SWMID_ucEffNum_2 || defined SWMID_ucEffNum_3
	uint8 ucSelect;// 0:max 1:min 2:average
	#endif 
	uint8 ucEffNum;  // 有效个数
}SWMIDStruct;

// 开关控制器13
typedef struct
{
	uint8 ucSelect;  // 0:选择1, 1:选择2, 2:选择3	
}SW13Struct;

// 偏差限值器
typedef struct
{
 	fp32 fMaxBias;  // 最大偏差
	fp32 fLastOut;	 // 上次输出
    uint32 ulCycle;   // 周期
	uint32 ulLastTimeRec; // 上次控制的时间
}BIASLMTStruct;

// 变化率器
typedef struct
{
	uint32 ulCycle;
	fp32 fLastIn;  // 上次的输入
}CHRATStruct;

// 加权平均滤波器
typedef struct
{
	fp32 *fCoefficient;  // 系数
	fp32 *fSavedIn;  // 最多前7次的输入
}AVEFLTStruct;

// 计算器
typedef struct
{
	fp32 fM[8];  // 八个中间变量

}CALCUStruct;

// 模拟量输入
typedef struct
{
	fp32 fPV; // 输出值
	fp32 fSV; // 设定值
	//fp32 fLastVal; // 上次值
	fp32 fRawVal; // 原始值
	uint8 isChange; // 是否进行量程转换
	fp32 fPVMax; // 工程单位高限
	fp32 fPVMin; // 工程单位低限
	fp32 fRawMax; // 原始数据高限
	fp32 fRawMin; // 原始数据低限
	fp32 fWarnLL;  // 报警低低限值
	uint8 isWarnLL; // 是否发生低低限报警
	fp32 fWarnL; // 报警低限值
	uint8 isWarnL; // 是否发生低限报警
	fp32 fWarnHH;  // 报警高高限
	uint8 isWarnHH; // 是否发生高高限报警
	fp32 fWarnH; // 报警高限
	uint8 isWarnH; // 是否发生高限报警
	fp32 fRation; // 变化率
	uint8 isRationWarn; // 是否发生进行变化率报警
	fp32 fBias;  // 偏差
	uint8 isBiasWarn; // 是否发生偏差报警
	uint32 ulCycle; // 周期	
}AIStruct;

// 模拟量输出
typedef struct
{
	fp32 fPV; // 输出值
	fp32 fSV; // 设定值
	//fp32 fLastVal; // 上次值
	fp32 fRawVal; // 原始值
	uint8 isChange; // 是否进行量程转换
	fp32 fPVMax; // 工程单位高限
	fp32 fPVMin; // 工程单位低限
	fp32 fRawMax; // 原始数据高限
	fp32 fRawMin; // 原始数据低限
	fp32 fWarnLL;  // 报警低低限值
	uint8 isWarnLL; // 是否发生低低限报警
	fp32 fWarnL; // 报警低限值
	uint8 isWarnL; // 是否发生低限报警
	fp32 fWarnHH;  // 报警高高限
	uint8 isWarnHH; // 是否发生高高限报警
	fp32 fWarnH; // 报警高限
	uint8 isWarnH; // 是否发生高限报警
	fp32 fRation; // 变化率
	uint8 isRationWarn; // 是否发生进行变化率报警
	fp32 fBias;  // 偏差
	uint8 isBiasWarn; // 是否发生偏差报警
	uint32 ulCycle; // 周期	

}AOStruct;
// 数字输入
typedef struct
{
   uint8 cPV; // 输出
   uint8 isInvert; // 正反动作
   uint8 cNormalVal; // 正常值
   uint8 isWarn;  // 报警值
   uint8 cRawVal; // 原始值

}DIStruct;
// 数字输出
typedef struct
{
  uint8 cPV;	 // 输出值
  uint8 isInvert;// 正/反动作

}DOStruct;
//脉宽调制输出
typedef struct
{
  fp32 fLastMV; // 上次的输入值
  uint8 cPV;	 // 输出值
  uint32 lCycle;// 控制周期
  uint8 cReset; // 是否重置
  uint8 cStyle; // 方式
  uint32 preCycle; // 前段时间
  uint32 aftCycle; // 后段时间
  uint32 timeRec; // 时间记录
}PVMStruct;
//AD输入
typedef struct
{
  uint8 ucCN; // AD通道号
  //uint32 ulCycle; // 采样周期  
}ADStruct;
//DA输出
typedef struct
{
  uint8 ucCN; // DA通道号
  //uint32 ulCycle; // 采样周期  
}DAStruct;
// 具有使能端的模块有默认参数:上次值
//ADD加法
typedef struct
{
  fp32 fLastVal; // 上次的值
}ADDStruct;
//SUB减法
typedef struct
{
  fp32 fLastVal; // 上次的值
}SUBStruct;
//MUL乘法
typedef struct
{
  fp32 fLastVal; // 上次的值
}MULStruct;
//DIV除法
typedef struct
{
  fp32 fLastVal; // 上次的值
}DIVStruct;
//POWER乘方
typedef struct
{
  fp32 fLastVal; // 上次的值
}POWERStruct;
//MOD求余
typedef struct
{
  fp32 fLastValPV; // 上次的值商
  fp32 fLastValQuat; // 上次的值余数
}MODStruct;
//ABS绝对值
typedef struct
{
  fp32 fLastVal; // 上次的值
}ABSStruct;
//COS余弦
typedef struct
{
  fp32 fLastVal; // 上次的值
}COSStruct;
//SIN正弦
typedef struct
{
  fp32 fLastVal; // 上次的值
}SINStruct;
//TAN正切
typedef struct
{
  fp32 fLastVal; // 上次的值
}TANStruct;
//ASIN反正弦
typedef struct
{
  fp32 fLastVal; // 上次的值
}ASINStruct;

//ACOS反余弦
typedef struct
{
  fp32 fLastVal; // 上次的值
}ACOSStruct;
//ATAN反正切
typedef struct
{
  fp32 fLastVal; // 上次的值
}ATANStruct;
//EXP指数
typedef struct
{
  fp32 fLastVal; // 上次的值
}EXPStruct;
//LG常用对数
typedef struct
{
  fp32 fLastVal; // 上次的值
}LGStruct;
//LN自然对数
typedef struct
{
  fp32 fLastVal; // 上次的值
}LNStruct;
//SQRT平方根
typedef struct
{
  fp32 fLastVal; // 上次的值
}SQRTStruct;
//TRUNC 取整
typedef struct
{
  fp32 fLastVal; // 上次的值
}TRUNCStruct;
//两输入ON有效或门
typedef struct
{
  fp32 fLastVal; // 上次的值
}QOR2Struct;
// 三输入ON有效或门
typedef struct
{
  fp32 fLastVal; // 上次的值
}QOR3Struct;
// 等于比较
typedef struct
{
  fp32 fLastVal; // 上次的值
}EQUStruct;
// 不等于比较
typedef struct
{
  fp32 fLastVal; // 上次的值
}UEQUStruct;
// 大于比较
typedef struct
{
  fp32 fLastVal; // 上次的值
}GTStruct;
// 小于比较
typedef struct
{
  fp32 fLastVal; // 上次的值
}LTStruct; // 157个struct-3 (154 / 2 = 77)
// 比较
typedef struct
{
  fp32 fResult; // 比较结果
  uint8 isGT;// 大于
  uint8 isLT;// 小于
  uint8 isEQ;// 等于
}COMPAREStruct;
// 数字组合点
typedef struct
{
  uint8 inputStatus;// 输入状态
  uint8 outputStatus;// 输出状态
}DCStruct;
// 条件动作表
typedef struct
{
  uint8 isEn;  // 使能
}PROCESSStruct;
////////////////////////////////////////////////////////////
//  2013.11.21 图像处理
//extern uint8 FILEOUTControl(FILEOUTStruct *FILEOUT);
//滤波
extern uint8 GAUSSIANSMOOTHINGControl(GAUSSIANSMOOTHINGStruct *GAUSSIANSMOOTHING);
extern uint8 MEDIANFILTERControl(MEDIANFILTERStruct  *MEDIANFILTER);
extern uint8 MEANFILTERControl(MEANFILTERStruct  *MEANFILTER);
extern uint8 SOBELControl(SOBELStruct *SOBEL);
extern uint8 PREWITTControl(PREWITTStruct  *PREWITT);
extern uint8 LAPLACEControl(LAPLACEStruct *LAPLACE);
//色彩
extern uint8 BINARYControl(BINARYStruct *BINARY);
extern uint8 OSTUBINARYControl(OSTUBINARYStruct  *OSTUBINARY);
extern uint8 ANTICOLORControl(ANTICOLORStruct  *ANTICOLOR);
extern uint8 GRAYSCALESTRETCHControl(GRAYSCALESTRETCHStruct *GRAYSCALESTRETCH);
extern uint8 HIAGRAMControl(HIAGRAMStruct *HIAGRAM);
//形态学
extern uint8 DENOSINGControl(DENOSINGStruct  *DENOSING);
extern uint8 ZHANGTHINControl(ZHANGTHINStruct  *ZHANGTHIN);
extern uint8 DIALATEControl(DIALATEStruct *DIALATE);
extern uint8 ERODEControl(ERODEStruct *ERODE);
extern uint8 OPENOPERATIONControl(OPENOPERATIONStruct *OPENOPERATION);
extern uint8 CLOSEOPERATIONControl(CLOSEOPERATIONStruct  *CLOSEOPERATION);
//轮廓检测
extern uint8 CANNYControl (CANNYStruct  *CANNY);
extern uint8 HOUGHLINESControl(HOUGHLINESStruct  *HOUGHLINES);
extern uint8 HOLLOWEDControl(HOLLOWEDStruct  *HOLLOWED);
//特征点检测
extern uint8 HARRISControl(HARRISStruct  *HARRIS);
//数值计算
extern uint8 CENTEROFGRAVITYControl(CENTEROFGRAVITYStruct  *CENTEROFGRAVITY);
//匹配
extern uint8 MATCHControl(MATCHStruct *MATCH);

//
extern uint8 CONControl(fp32 a);
extern uint8 POPControl(void);
extern uint8 LDControl(void *a);
extern uint8 STControl(void *a);
extern uint8 ADDControl(ADDStruct *ADD);
extern uint8 SUBControl(SUBStruct *SUB);
extern uint8 MULControl(MULStruct *MUL);
extern uint8 DIVControl(DIVStruct *DIV);
extern uint8 POWERControl(POWERStruct *POWER);
extern uint8 MODControl(MODStruct *MOD);
extern uint8 ABSControl(ABSStruct *ABS);
extern uint8 COSControl(COSStruct *COS);
extern uint8 SINControl(SINStruct *SIN);
extern uint8 TANControl(TANStruct *TAN);
extern uint8 ASINControl(ASINStruct *ASIN);
extern uint8 ACOSControl(ACOSStruct *ACOS);
extern uint8 ATANControl(ATANStruct *ATAN);
extern uint8 EXPControl(EXPStruct *EXP);
extern uint8 LGControl(LGStruct *LG);
extern uint8 LNControl(LNStruct *LN);
extern uint8 SQRTControl(SQRTStruct *SQRT);
extern uint8 TRUNCControl(TRUNCStruct *TRUNC);
extern uint8 ANDControl(ANDStruct *AND);
extern uint8 ORControl(ORStruct *OR);
extern uint8 XORControl(XORStruct *XOR);
extern uint8 NOTControl(NOTStruct *NOT);
extern uint8 NANDControl(NANDStruct *NAND);
extern uint8 NORControl(NORStruct *NOR);
extern uint8 QOR2Control(QOR2Struct *QOR2);
extern uint8 QOR3Control(QOR3Struct *QOR3);
extern uint8 DISP3Control(DISP3Struct *DISP3);
extern uint8 SWITControl(SWITStruct *SWIT);
extern uint8 PULSEControl(PULSEStruct *PULSE);
extern uint8 MAXPLControl(MAXPLStruct *MAXPL);
extern uint8 MINPLControl(MINPLStruct *MINPL);
extern uint8 EQUControl(EQUStruct *EQU);
extern uint8 UEQUControl(UEQUStruct *UEQU);
extern uint8 GTControl(GTStruct *GT);
extern uint8 LTControl(LTStruct *LT);
extern uint8 RSControl(RSStruct *RS);
extern uint8 DELAYControl(DELAYStruct *DELAY);
extern uint8 ONDLYControl(ONDLYStruct *ONDLY);
extern uint8 OFFDLYControl(OFFDLYStruct *OFFDLY);
extern uint8 WTDOGControl(WTDOGStruct *WTDOG);
extern uint8 SELECTControl(SELECTStruct *SELECT);
extern uint8 CHDCTControl(CHDCTStruct *CHDCT);
extern uint8 RETControl(void);
extern uint8 PIControl(PIStruct *PI);
extern uint8 SMITHControl(SMITHStruct *SMITH);
extern uint8 LEASTControl(LEASTStruct *LEAST);
extern uint8 DARLINControl(DARLINStruct *DARLIN);
extern uint8 PREControl(PREStruct *PRE);
extern uint8 JIEOUControl(JIEOUStruct *JIEOU);
extern uint8 FUZZYControl(FUZZYStruct *FUZZY);
extern uint8 FILTControl(FILTStruct *FILT);
extern uint8 TRANSControl(TRANSStruct *TRANS);
extern uint8 CNTControl(CNTStruct *CNT);
extern uint8 TIMControl(TIMStruct *TIM);
extern uint8 TOTControl(TOTStruct *TOT);
extern uint8 LIMITControl(LIMITStruct *LIMIT);
extern uint8 PERCENTControl(PERCENTStruct *PERCENT);
extern uint8 RATIONControl(RATIONStruct *RATION);
extern uint8 RAMPControl(RAMPStruct *RAMP);
extern uint8 SW51Control(SW51Struct *SW51);
extern uint8 FLCPControl(FLCPStruct *FLCP);
extern fp32 Linearize(PointStruct pt1,PointStruct pt2,fp32 x);
extern uint8 GLINControl(GLINStruct *GLIN);
extern uint8 LICHControl(LICHStruct *LICH);
extern uint8 SWHLControl(SWHLStruct *SWHL);
extern uint8 SWMIDControl(SWMIDStruct *SWMID);
extern uint8 SW13Control(SW13Struct *SW13);
extern uint8 BIASLMTControl(BIASLMTStruct *BIASLMT);
extern uint8 DCControl(DCStruct *DC);
extern uint8 CHRATControl(CHRATStruct *CHRAT);
extern uint8 COMPAREControl(COMPAREStruct *COMPARE);
extern uint8 AVEFLTControl(AVEFLTStruct *AVEFLT);
extern uint8 PIDControl(PIDStruct* PID);
extern uint8 DAControl(DAStruct * DA);
extern uint8 ADControl(ADStruct * AD);
extern uint8 AOControl(AOStruct *AO);
extern uint8 DIControl(DIStruct* DI);
extern uint8 DOControl(DOStruct* DO);
extern uint8 AIControl(AIStruct *AI);
extern uint8 PVMControl(PVMStruct *PVM);

#endif





