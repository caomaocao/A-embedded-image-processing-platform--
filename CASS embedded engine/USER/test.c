#include "sys.h"
#include "usart.h"		
#include "delay.h"	
#include "led.h" 
#include "beep.h"	 	 
#include "key.h"	 	 
#include "exti.h"	 	 
#include "wdg.h" 	 
#include "timer.h"		 	 
#include "tpad.h"
#include "oled.h"		 	 
#include "lcd.h"
#include "usmart.h"	
#include "rtc.h"	 	 
#include "wkup.h"	
#include "adc.h" 	 
#include "dac.h" 	 
#include "dma.h" 	 
#include "24cxx.h" 	 
#include "flash.h" 	 
#include "rs485.h" 	 
#include "can.h" 	 
#include "touch.h" 	 
#include "remote.h" 	 
#include "joypad.h"
#include "adxl345.h"
#include "ds18b20.h"
#include "dht11.h"
#include "24l01.h"
#include "mouse.h"
#include "stmflash.h"
#include "rda5820.h"
#include "audiosel.h"
#include "ov7670.h"
#include "malloc.h"
#include "sram.h"
#include "cass/configuration_control.h"
#include "cass/configuration_system.h"
#include "cass/configuration_address.h"

const u8*LMODE_TBL[5]={"Auto","Sunny","Cloudy","Office","Home"};							//5种光照模式	    
const u8*EFFECTS_TBL[7]={"Normal","Negative","B&W","Redish","Greenish","Bluish","Antique"};	//7种特效 
u8 lightmode=0,saturation=0,brightness=0,contrast=0,effect=0;

extern u8 ov_sta;	//在exit.c里面定义
extern u8 ov_frame;	//在timer.c里面定义		 

u8 CassMem[4096];
extern u8 raw_data[];
extern u8 out_data[];
extern u8 ComInDataOk;

u16 cass_mv_boarder_width;
u16 cass_mv_boarder_height;
u16 cass_mv_feature_point_number;
u16 startx,endx,starty,endy;

//feather.h
//begin
image_data_packet_desc_type  image_data_packet_desc={0, 0,0,320,240, 0,0,0,0,0,0,120,150, 
																										25,10,0,1, 30,12,0,1, 170,50,0,1, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
//end
//第三个坐标点错误
	
u8 cass_mv_binary_threshold=110;

u8 cass_mv_gussian_length;

u8 cass_mv_laplace_length;

u8 cass_mv_match_percent;

u8 cass_mv_erode_strenth;

	
static void sys_image_var_init()
{
	 //需要判断(x,y)的相对大小
  cass_mv_boarder_width = image_data_packet_desc.roi_desc.x_end - image_data_packet_desc.roi_desc.x_start;
	cass_mv_boarder_height = image_data_packet_desc.roi_desc.y_end - image_data_packet_desc.roi_desc.y_start;
	
	startx=image_data_packet_desc.roi_desc.x_start;
	endx=image_data_packet_desc.roi_desc.x_end;
	starty=image_data_packet_desc.roi_desc.y_start;
	endy=image_data_packet_desc.roi_desc.y_end;
	//cass_mv_boarder_width=320;
	//cass_mv_boarder_height=240;
}

void Camera_refresh()
{
	u32 x,y;
	u16 color,gray,red,green,blue;
	
	if(ov_sta==2)
	{			
		OV7670_RRST=0;				//开始复位读指针 
		OV7670_RCK=0;
		OV7670_RCK=1;
		OV7670_RCK=0;
		OV7670_RRST=1;				//复位读指针结束 
		OV7670_RCK=1;  	
		
 		for(y=0;y<240;y++)//宽
		{
			for(x=0;x<320;x++)//高
			{
				OV7670_RCK=0;
				color=GPIOC->IDR&0XFF;	//读数据
				OV7670_RCK=1; 
				color<<=8;  
				OV7670_RCK=0;
				color|=GPIOC->IDR&0XFF;	//读数据
				OV7670_RCK=1; 
				
				red = (color>>11) & 0x1f;
				green = (color>>5) & 0x3f;
				blue = color & 0x1f;					
				red   = (red<<3) | (red & 0x7);
				green = (green<<2) | (green & 0x3);
				blue  = (blue<<3) | (blue & 0x7);
				
				gray = (u16)((red*1224+green*2404+blue*467)>>12);
				raw_data[y*320+x]=gray;
				//out_data[y*320+x]=gray;
			}
		}		
 		EXTI->PR=1<<8;     			//清除LINE8上的中断标志位
		ov_sta=0;					//开始下一次采集
 		ov_frame++; 
			
		for(y=starty;y<endy;y++)
			for(x=startx;x<endx;x++)
			{
					out_data[(y-starty)*cass_mv_boarder_width+x-startx]=raw_data[y*320+x];
			}		
	} 
}	


//高斯模糊
uint8 GAOSIControl(GAOSIStruct *GAOSI)
{
		u16 x=0,y=0;
		u16 cur_value=0;
		u8	value=0;
		u8  v[9];

		static int win[9] = { 1,2,1,2,4,2,1,2,1	};

			for (y=1 ;y<cass_mv_boarder_height - 1;y++)
			{
				for (x=1;x<cass_mv_boarder_width - 1;x++)
				{
				v[0] = raw_data[(y-1)*cass_mv_boarder_width+x-1];
				v[1] = raw_data[(y-1)*cass_mv_boarder_width+x];
				v[2] = raw_data[(y-1)*cass_mv_boarder_width+x+1];
				v[3] = raw_data[y*cass_mv_boarder_width+x-1];
				v[4] = raw_data[y*cass_mv_boarder_width+x];
				v[5] = raw_data[y*cass_mv_boarder_width+x+1];
				v[6] = raw_data[(y+1)*cass_mv_boarder_width+x-1];
				v[7] = raw_data[(y+1)*cass_mv_boarder_width+x];
				v[8] = raw_data[(y+1)*cass_mv_boarder_width+x+1];

				cur_value = win[0]*v[0] + win[1]*v[1] + win[2]*v[2]
				+ win[3]*v[3] + win[4]*v[4] + win[5]*v[5]
				+ win[6]*v[6] + win[7]*v[7] + win[8]*v[8];

				value = (u8)(cur_value>>4);

				out_data[y*cass_mv_boarder_width+x] = value;
				}
			}		
			return 1;
}

// 拉普拉斯函数
uint8 LAPLACEControl(LAPLACEStruct *LAPLACE)
{
		u16 x,y;
		u16 cur_value=0;
		u8 v[9];      
		static int win[9] = {0,1,0,1,-4,1,0,1,0};

			for (y=1 ;y<cass_mv_boarder_height - 1;y++)
			{
				for (x=1;x<cass_mv_boarder_width - 1;x++)
				{
				v[0] = raw_data[(y-1)*cass_mv_boarder_width+x-1];
				v[1] = raw_data[(y-1)*cass_mv_boarder_width+x];
				v[2] = raw_data[(y-1)*cass_mv_boarder_width+x+1];
				v[3] = raw_data[y*cass_mv_boarder_width+x-1];
				v[4] = raw_data[y*cass_mv_boarder_width+x];
				v[5] = raw_data[y*cass_mv_boarder_width+x+1];
				v[6] = raw_data[(y+1)*cass_mv_boarder_width+x-1];
				v[7] = raw_data[(y+1)*cass_mv_boarder_width+x];
				v[8] = raw_data[(y+1)*cass_mv_boarder_width+x+1];

				cur_value = win[0]*v[0] + win[1]*v[1] + win[2]*v[2]
				+ win[3]*v[3] + win[4]*v[4] + win[5]*v[5]
				+ win[6]*v[6] + win[7]*v[7] + win[8]*v[8];

				cur_value = (u8)(cur_value>>4);

				out_data[y*cass_mv_boarder_width+x] = cur_value;
				}
			}		
	return 1;
}

//腐蚀
uint8 ERODEControl(ERODEStruct *ERODE)
{
	u8 k;
	u16 i,j;
	u8 flag;
  u8 pix[5];

	for(i=1;i<cass_mv_boarder_width-1;i++)                                     
  {
		for(j=1;j<cass_mv_boarder_width-1;j++)
		{
			flag=1;
			pix[0]=raw_data[i*cass_mv_boarder_width+j];
			pix[1]=raw_data[i*cass_mv_boarder_width+j-2];
			pix[2]=raw_data[i*cass_mv_boarder_width+j-1];
			pix[3]=raw_data[i*cass_mv_boarder_width+j+1];
			pix[4]=raw_data[i*cass_mv_boarder_width+j];
                   
			for(k=0;k<5;k++)
			{
				if((pix[0]==255)||(pix[k]==255))
				{
					flag=0;
					break;
				}
				if(flag==0)
				{
					break;
				}
			}
			if(flag==0)
				out_data[i*cass_mv_boarder_width+j]=255;		
			else	
				out_data[i*cass_mv_boarder_width+j]=0;			
		}
	}
  return 1;	
}

//二值化
uint8 BINARYControl(BINARYStruct *BINARY)
{
	u32 x;

	for(x=0;x<cass_mv_boarder_width*cass_mv_boarder_height;x++)
	{
		if(raw_data[x]<cass_mv_binary_threshold)
		{
			out_data[x]=0;
			raw_data[x]=0;
		}
		else
		{
			out_data[x]=255;
			raw_data[x]=255;
		}
	}
	return 1;
}


//匹配函数
uint8 MATCHControl(MATCHStruct *MATCH)
{
  u16 tempx,tempy,tempvalue,tempdot;
	u16 sum=0,sumvalue=0,x=0;

	while((x<256)&&(image_data_packet_desc.point[x].w!=0))//统计特征点权值和
	{
		sum+=image_data_packet_desc.point[x].w;
		x++;
	}
	
	x=0;
	while(x<256&&(image_data_packet_desc.point[x].w!=0))//待测图像特征点位置检测
	{
		tempx=image_data_packet_desc.point[x].x;
		tempy=image_data_packet_desc.point[x].y;
		tempdot=image_data_packet_desc.point[x].v;//像素值
		tempvalue=image_data_packet_desc.point[x].w;//权值

		if(out_data[tempy*cass_mv_boarder_width+tempx]==tempdot)
		{
			sumvalue+=tempvalue;
		}
		x++;
	}

	return ( sumvalue * 100/sum);//精确到个位
}

void Make_Points()
{
	out_data[10*cass_mv_boarder_width+25]=0;//高,宽
	out_data[12*cass_mv_boarder_width+30]=0;
	out_data[50*cass_mv_boarder_width+170]=0;//高度宽度 和结构体对调
	out_data[170*cass_mv_boarder_width+40]=0;
	out_data[62*cass_mv_boarder_width+210]=0;	
}

int main(void)
{
	u32 i,j;
	u8 tempsaturation=0,tempcontrast=0,tempbrightness;
		
	GAOSIStruct *gaosi;
	BINARYStruct *binary;
	ERODEStruct  *erode;
	LAPLACEStruct *laplace;
	MATCHStruct *match;

	Stm32_Clock_Init(9);	//系统时钟设置
	
	sys_image_var_init();
	
	uart_init(72,115200); 	//串口1初始化  
	delay_init(72);			//延时初始化	
	LCD_Init();				//初始化液晶 
	FSMC_SRAM_Init();
	if(lcddev.id==0X6804) 	//强制设置屏幕分辨率为320*240.以支持3.5寸大屏
	{
		lcddev.width=240;
		lcddev.height=320; 
	} 
  													   		 	
	while(OV7670_Init())//初始化OV7670
	{
 		delay_ms(200);
 	  LCD_Fill(60,190,239,206,WHITE);
 		delay_ms(200);
	}
	
	/*调整OV7670效果*/
	OV7670_Light_Mode(lightmode);
	OV7670_Color_Saturation(saturation);
	OV7670_Brightness(brightness);
	OV7670_Contrast(contrast);
 	OV7670_Special_Effects(effect);	 	
	
	TIM3_Int_Init(9,7199);//10Khz的计数频率，计数到10为1ms  
	TIM6_Int_Init(10000,7199);			//10Khz计数频率,1秒钟中断									  
	EXTI8_Init();						//使能定时器捕获
	OV7670_Window_Set(10,174,240,320);	//设置窗口	  
  OV7670_CS=0;				    		    
	ov_sta=2;//使能OV7670
	
	while(1)//识别每一帧
	{		
		tempbrightness=brightness;
		tempcontrast=contrast;
		tempsaturation=saturation;
		
		if(ComInDataOk)
		{
			 UartRecvHandle(0);
			 ComInDataOk = 0;
		}
		
		/*调整镜头参数*/
		brightness=CassMem[167];
		contrast=CassMem[168];
		saturation=CassMem[169];
		if((brightness!=tempbrightness)||(contrast!=tempcontrast)||(saturation!=tempsaturation))
		{	
			OV7670_Brightness(brightness);	
			OV7670_Contrast(contrast);
			OV7670_Color_Saturation(saturation);
			tempbrightness=brightness;
			tempcontrast=contrast;
			tempsaturation=saturation;
		}
		
			
		Camera_refresh();//OV7670刷新   
		Make_Points();
		
		//GAOSIControl(gaosi);	
		//BINARYControl(binary);
		//LAPLACEControl(laplace);
		//ERODEControl(erode);
		
		//MATCHControl(match);
			
// 	同步显示到LCD		
// 		for(x=0;x<(int)cass_mv_boarder_width*(int)cass_mv_boarder_height;x++)		
// 		{
// 			raw_data[x]=out_data[x];
// 		}
		
		LCD_Scan_Dir(U2D_L2R);		//设定LCD刷新方向
		LCD_SetCursor(0x00,0x0000);	//设置光标位置 
		LCD_WriteRAM_Prepare();     //开始写入GRAM					
		for(j=0;j<240;j++)
		{
			for(i=0;i<320;i++)
			{	
				LCD->LCD_RAM=(u16)((raw_data[j*320+i]>>3)<<11|((raw_data[j*320+i]>>2)<<5)|(raw_data[j*320+i]>>3)<<0);			
			}
		}
	}

}









