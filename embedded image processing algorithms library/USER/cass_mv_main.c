#include "cass\configuration_control.h"
#include "cass\configuration_system.h"
#include "cass\configuration_address.h"
#include "stm32f10x_type.h"


GAUSSIANSMOOTHINGStruct configuration0_GAUSSIANSMOOTHING0;
OSTUBINARYStruct configuration0_OSTUBINARY0;
ANTICOLORStruct configuration0_ANTICOLOR0;
ERODEStruct configuration0_ERODE0;
HOLLOWEDStruct configuration0_HOLLOWED0;


extern uint8 raw_data[];
extern uint8 out_data[];
uint16 cass_mv_boarder_leftbound,cass_mv_boarder_topbound;

uint16 cass_mv_boarder_width;
uint16 cass_mv_boarder_height;
uint8  cass_mv_feature_point_number;

uint8 cass_mv_match_result;
//feather.h
//begin
//end


uint8 cass_mv_binary_threshold;

uint8 cass_mv_gaussian_length;

uint8 cass_mv_laplace_length;

uint8 cass_mv_match_percent;

uint8 cass_mv_erode_strenth;

//configuration_ControlFuns.c
//begin
		
// 文件输入
uint8 FILEINControl(char *filename)
{
#ifdef CASS_MV_PC_VERSION
	FILE *source;
   	int  x,y,scanline;
	int width,height,fileSize,dwSize,current;
	
	memset(raw_data,0,360000);
	memset(out_data,0,360000);
	
	if((source=fopen((char *)filename,"r"))==NULL)
    	{
        	printf("can't open the source file!\n");
        	return 0;
    	}
	
	/*读取bmp文件头和信息头存到数组里*/
	fseek(source,0,SEEK_SET);
	fread(bmpinfoarray,sizeof(unsigned char),54,source);
	fseek(source,54,SEEK_SET);
	fread(plltee,sizeof(unsigned char),1024,source);
	
	fseek(source,2,0);
	fread(&fileSize,sizeof(long),1,source);//读取bmp数据区大小
	
	fseek(source,14,0);
	fread(&dwSize,sizeof(long),1,source);

          //bmpinfo.width=bmpinfoarray[18]+bmpinfoarray[19]*255;
          //bmpinfo.height=bmpinfoarray[22]+bmpinfoarray[23]*255;
          //bmpinfo.fileSize=fileSize;
          //bmpinfo.dwSize=dwSize;

          width=bmpinfoarray[18]+bmpinfoarray[19]*255;
          height=bmpinfoarray[22]+bmpinfoarray[23]*255;

          if(width%4)//bmp宽度不为4要补0
          	scanline=(width*8+31)/32*4;
          else
          	scanline=width;

 	for(x=0;x<height;x++)//读取bmp进数组		
	{	
		for(y=0;y<scanline;y++)
		{
			current=14+dwSize+1024+x*scanline+y;
			fseek(source,current,0); 
			raw_data[(height-1-x)*scanline+y]=fgetc(source);
		}	
	}		
	fclose(source);
#else

#endif
	return 1;
}

          // 高斯函数
          uint8 GAUSSIANSMOOTHINGControl(GAUSSIANSMOOTHINGStruct *GAUSSIANSMOOTHING)
          {
          #ifdef CASS_MV_PC_VERSION
          int x,y,scanline;
          int cur_value,thresh_value,sum;
          int width,height;
          int v[9];

          static int win[9] = { 1,2,1,2,4,2,1,2,1};

          width=bmpinfoarray[18]+bmpinfoarray[19]*256;
          height=bmpinfoarray[22]+bmpinfoarray[23]*256;

          if(width%4)//bmp宽度不为4要补0
          scanline=(width*8+31)/32*4;
          else
          scanline=width;

          for (y=0 ;y<height;y++)
          {
          for (x=0;x<width;x++)
          {
          sum=0;

          if(y==0||y==height-1)
          out_data[y*scanline+x]=raw_data[y*scanline+x];
          else if(x==0||x==width-1)
          out_data[y*scanline+x]=raw_data[y*scanline+x];
          else
          {
          v[0] = raw_data[(y-1)*scanline+x-1];
          v[1] = raw_data[(y-1)*scanline+x];
          v[2] = raw_data[(y-1)*scanline+x+1];
          v[3] = raw_data[y*scanline+x-1];
          v[4] = raw_data[y*scanline+x];
          v[5] = raw_data[y*scanline+x+1];
          v[6] = raw_data[(y+1)*scanline+x-1];
          v[7] = raw_data[(y+1)*scanline+x];
          v[8] = raw_data[(y+1)*scanline+x+1];

          sum = win[0]*v[0] + win[1]*v[1] + win[2]*v[2]
          + win[3]*v[3] + win[4]*v[4] + win[5]*v[5]
          + win[6]*v[6] + win[7]*v[7] + win[8]*v[8];

          sum = (int)(sum/16);
          if(sum>255)
          sum=255;
          if(sum<0)
          sum=0;
          out_data[y*scanline+x]=(unsigned char)sum;
          }
          }
          }
          memcpy(raw_data,out_data,360000);
          Save_File(GAUSSIANSMOOTHING->index, height,width);
          #else
          u16 x=0,y=0,scanline=320;
	u16 cur_value=0;
	u8	value=0;
	u8  v[9];
	static u8 win[9] = {1,2,1,2,4,2,1,2,1};
	
	for(y=0;y<cass_mv_boarder_height;y++)  
		for (x=0;x<cass_mv_boarder_width;x++)
		{ 		
			v[0] = raw_data[(y+cass_mv_boarder_topbound-1)*scanline+x+cass_mv_boarder_leftbound-1];//有边缘隐患，如果是对全图或者边缘处理有危险。
			v[1] = raw_data[(y+cass_mv_boarder_topbound-1)*scanline+x+cass_mv_boarder_leftbound];
			v[2] = raw_data[(y+cass_mv_boarder_topbound-1)*scanline+x+cass_mv_boarder_leftbound+1];
			v[3] = raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound-1];
			v[4] = raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound];
			v[5] = raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound+1];
			v[6] = raw_data[(y+cass_mv_boarder_topbound+1)*scanline+x+cass_mv_boarder_leftbound-1];
			v[7] = raw_data[(y+cass_mv_boarder_topbound+1)*scanline+x+cass_mv_boarder_leftbound];
			v[8] = raw_data[(y+cass_mv_boarder_topbound+1)*scanline+x+cass_mv_boarder_leftbound+1];
			
			cur_value = win[0]*v[0] + win[1]*v[1] + win[2]*v[2]
          + win[3]*v[3] + win[4]*v[4] + win[5]*v[5]
          + win[6]*v[6] + win[7]*v[7] + win[8]*v[8];
			value = (u8)(cur_value>>4);
			out_data[(y+cass_mv_boarder_topbound)*scanline+(x+cass_mv_boarder_leftbound)]=value;
    }
	 
	memcpy(raw_data,out_data,240*scanline);//可以改为就复制选取区域的一部分，会更快
	
          #endif
          return 0;
          }


          // 最优二值化函数
          uint8 OSTUBINARYControl(OSTUBINARYStruct *OSTUBINARY)
          {
          #ifdef CASS_MV_PC_VERSION
          int x,y,i,j;
          int height,width,scanline,thresh_value;
          double piexlsum,greyscale;
          float piexlcount[255]={0};
          float piexlpercent[255]={0};
          float w0,w1,u0tmp,u1tmp,u0,u1,u,deltaTmp,deltaMax;

          thresh_value=0;
          greyscale=255;
          deltaMax=0;

          width=bmpinfoarray[18]+bmpinfoarray[19]*256;
          height=bmpinfoarray[22]+bmpinfoarray[23]*256;
          piexlsum=width*height;

          if(width%4)
          scanline=(width*8+31)/32*4;
          else
          scanline=width;

          for (y=0;y<height;y++)
          for (x=0;x<width;x++)
          {
          piexlcount[(int)raw_data[y*scanline+x]]++;//每级灰度的像素个数
          }

          for(i=0;i<greyscale;i++)
          {
          piexlpercent[i]=(1.0*piexlcount[i])/(1.0*piexlsum);//每级灰度的像素占总像素数比率
          }

          for(i=0;i<greyscale;i++)//阈值扫描
          {
          w0=w1=u0tmp=u1tmp=u0=u1=u=deltaTmp=0;
          for(j=0;j<greyscale;j++)
          {
          if(j<i)
          {
          w0+=piexlpercent[j];//前景像素占总像素比率
          u0tmp+=j*piexlpercent[j];//前景像素灰度平均值
          }
          else
          {
          w1+=piexlpercent[j];//背景像素占总像素比率
          u1tmp+=j*piexlpercent[j];//背景像素灰度平均值
          }
          }
          u0=u0tmp/w0;//前景像素灰度平均值
          u1=u1tmp/w1;//背景像素灰度平均值
          u=u0tmp+u1tmp;//所有像素灰度平均值
          deltaTmp=w0*pow((u0-u),2)+w1*pow((u1-u),2);//前景背景差距=w0(u0-u)^2+w1(u1-u)^2

          if(deltaTmp>deltaMax)//最大的deltaMax时，前景背景差异最大
          {
          deltaMax=deltaTmp;
          thresh_value=i;
          }
          }

          for (y=0;y<height;y++)
          {
          for (x=0;x<width;x++)
          {
          if(raw_data[y*scanline+x]<thresh_value)
          out_data[y*scanline+x]=0;
          else
          out_data[y*scanline+x]=255;
          }
          }

          memcpy(raw_data,out_data,360000);
          Save_File(OSTUBINARY->index,height,width);
          #else
u8 x,y,i,j;
   u16 scanline,thresh_value;
   double piexlsum,greyscale;
   float piexlcount[255];
   float piexlpercent[255];
   float w0,w1,u0tmp,u1tmp,u0,u1,u,deltaTmp,deltaMax;

   thresh_value=0;
   greyscale=255;
   deltaMax=0;
   
   scanline=320;
   //height=240;

   piexlsum=cass_mv_boarder_width*cass_mv_boarder_height;//总像素

   memset(piexlcount,0,sizeof(piexlcount));
   memset(piexlpercent,0,sizeof(piexlpercent));

   for (y=0;y<cass_mv_boarder_height;y++)  
      for (x=0;x<cass_mv_boarder_width;x++)
	  { 
			piexlcount[(int)raw_data[(y+cass_mv_boarder_topbound)*scanline+(x+cass_mv_boarder_leftbound)]]++;//每级灰度的像素个数
      }
      
   for(i=0;i<greyscale;i++)
   {
      piexlpercent[i]=(float)piexlcount[i]/piexlsum;//每级灰度的像素占总像素数比率
   }
   
   for(i=0;i<greyscale;i++)//阈值扫描
   {
     w0=w1=u0tmp=u1tmp=u0=u1=u=deltaTmp=0;
     for(j=0;j<greyscale;j++)
     {
      if(j<i)
      {
        w0+=piexlpercent[j];//前景像素占总像素比率
        u0tmp+=j*piexlpercent[j];//前景像素灰度平均值
      }
      else
      {
        w1+=piexlpercent[j];//背景像素占总像素比率
        u1tmp+=j*piexlpercent[j];//背景像素灰度平均值
      }
     }
     u0=u0tmp/w0;//前景像素灰度平均值
     u1=u1tmp/w1;//背景像素灰度平均值
     u=u0tmp+u1tmp;//所有像素灰度平均值
     deltaTmp=w0*pow((u0-u),2)+w1*pow((u1-u),2);//前景背景差距=w0(u0-u)^2+w1(u1-u)^2
     if(deltaTmp>deltaMax)//最大的deltaMax时，前景背景差异最大
     {
       deltaMax=deltaTmp;
       thresh_value=i;
     }   
   }  
   
   for (y=0;y<cass_mv_boarder_height;y++)  
	{
		for (x=0;x<cass_mv_boarder_width;x++)
		{ 		
			if(raw_data[(y+cass_mv_boarder_topbound)*scanline+(x+cass_mv_boarder_leftbound)]<thresh_value)
				out_data[(y+cass_mv_boarder_topbound)*scanline+(x+cass_mv_boarder_leftbound)]=0;
			else
				out_data[(y+cass_mv_boarder_topbound)*scanline+(x+cass_mv_boarder_leftbound)]=255;
    }
	} 

	memcpy(raw_data,out_data,240*scanline);//可以改为就复制选取区域的一部分，会更快
          #endif
          return 0;
          }
		  

		// 反转颜色函数
uint8 ANTICOLORControl(ANTICOLORStruct *ANTICOLOR)
{
#ifdef CASS_MV_PC_VERSION
	int x,y,sum;
    int width,height,scanline;

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

	for (y=0 ;y<height;y++)
    {
		for (x=0;x<width;x++)
          {
          raw_data[y*scanline+x]=255-raw_data[y*scanline+x];
          }
          }
          Save_File(ANTICOLOR->index,height,width);
          #else
u16 x=0,y=0,scanline=320;
	
	for(y=0;y<cass_mv_boarder_height;y++)  
		for (x=0;x<cass_mv_boarder_width;x++)	
		{
			out_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound]=255-raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound];
		}

	memcpy(raw_data,out_data,240*scanline);//可以改为就复制选取区域的一部分，会更快
          #endif
          return 0;
          }
       

        
uint8 ERODEControl(ERODEStruct *ERODE)
{
#ifdef CASS_MV_PC_VERSION
	int scanline,k,i,j;
	int flag;
	unsigned char pix[5];

	int width=bmpinfoarray[18]+bmpinfoarray[19]*255;
	int height=bmpinfoarray[22]+bmpinfoarray[23]*255;

	if(width%4)
		scanline=(width*8+31)/32*4;
	else
		scanline=width;

	for(i=1;i<height-1;i++)                                     
	{
		for(j=1;j<width-1;j++)
		{
			flag=1;
			pix[0]=raw_data[i*scanline+j];
			pix[1]=raw_data[i*scanline+j-2];
			pix[2]=raw_data[i*scanline+j-1];
			pix[3]=raw_data[i*scanline+j+1];
			pix[4]=raw_data[i*scanline+j];

			for(k=0;k<5;k++)
			{
				if((pix[0]==0)||(pix[k]==0))
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
				out_data[i*scanline+j]=0;
			else
				out_data[i*scanline+j]=255;
		}
	}
	memcpy(raw_data,out_data,360000);
	Save_File(ERODE->index,height,width);
#else
    u16 scanline=320,k,x,y;
	u8 flag;
  u8 pix[5];

	for(y=1;y<cass_mv_boarder_height-1;y++)                                      
	{
    for(x=1;x<cass_mv_boarder_width-1;x++)
		{
		  flag=1;
		  pix[0]=raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound];
		  pix[1]=raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound-2];
		  pix[2]=raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound-1];
		  pix[3]=raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound+1];
		  pix[4]=raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound+2];
        
		for(k=0;k<5;k++)
		{
			if((pix[0]==0)||(pix[k]==0))
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
			out_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound]=0;		
		else	
			out_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound]=255;			
		}
	}
	
 	memcpy(raw_data,out_data,scanline*240);
#endif
    return 1;	
}
     

		// 掏空函数
uint8 HOLLOWEDControl(HOLLOWEDStruct *HOLLOWED)
{
#ifdef CASS_MV_PC_VERSION
	int x,y,sum;
    int width,height,scanline;
	int v[9];

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

	for (y=0 ;y<height;y++)
    {
		for (x=0;x<width;x++)
          {
          if(y==0||y==height-1)
          out_data[y*scanline+x]=raw_data[y*scanline+x];
          else if(x==0||x==width-1)
          out_data[y*scanline+x]=raw_data[y*scanline+x];
          else
          {
          if(raw_data[y*scanline+x]==0)
          {
          v[0] = raw_data[(y-1)*scanline+x-1];
          v[1] = raw_data[(y-1)*scanline+x];
          v[2] = raw_data[(y-1)*scanline+x+1];
          v[3] = raw_data[y*scanline+x-1];
          v[4] = raw_data[y*scanline+x];
          v[5] = raw_data[y*scanline+x+1];
          v[6] = raw_data[(y+1)*scanline+x-1];
          v[7] = raw_data[(y+1)*scanline+x];
          v[8] = raw_data[(y+1)*scanline+x+1];

          if(v[0]+v[1]+v[2]+v[3]+v[5]+v[6]+v[7]+v[8]==0)
          out_data[y*scanline+x]=255;
          }
          }
          }
          }
          memcpy(raw_data,out_data,360000);
          Save_File(HOLLOWED->index,height,width);
          #else
	u16 x,y,sum,scanline=320;
  u8 v[9];

	for (y=0 ;y<cass_mv_boarder_height;y++)
		for (x=0;x<cass_mv_boarder_width;x++)
    {
			  if(y==0||y==cass_mv_boarder_height-1)
				out_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound]=raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound];
			  else if(x==0||x==cass_mv_boarder_width-1)
				out_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound]=raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound];
			  else
			  {
				if(raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound]==0)
				{
				  v[0] = raw_data[(y+cass_mv_boarder_topbound-1)*scanline+x+cass_mv_boarder_leftbound-1];
				  v[1] = raw_data[(y+cass_mv_boarder_topbound-1)*scanline+x+cass_mv_boarder_leftbound];
				  v[2] = raw_data[(y+cass_mv_boarder_topbound-1)*scanline+x+cass_mv_boarder_leftbound+1];
				  v[3] = raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound-1];
				  v[4] = raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound];
				  v[5] = raw_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound+1];
				  v[6] = raw_data[(y+cass_mv_boarder_topbound+1)*scanline+x+cass_mv_boarder_leftbound-1];
				  v[7] = raw_data[(y+cass_mv_boarder_topbound+1)*scanline+x+cass_mv_boarder_leftbound];
				  v[8] = raw_data[(y+cass_mv_boarder_topbound+1)*scanline+x+cass_mv_boarder_leftbound+1];

				  if(v[0]+v[1]+v[2]+v[3]+v[5]+v[6]+v[7]+v[8]==0)
					out_data[(y+cass_mv_boarder_topbound)*scanline+x+cass_mv_boarder_leftbound]=255;
				}
			  }
		}
    
		memcpy(raw_data,out_data,scanline*240);		  
          #endif
          return 0;
          }
       
			
// 输出
void FILEOUTControl(char *folderName)
{
#ifdef CASS_MV_PC_VERSION
        FILE *fp;
        char pass[4];
		if((fp=fopen("result.txt","w"))==NULL)
    		{
        		printf("can't open the source file!\n");
        		return;
    		}
                itoa(cass_mv_match_percent,pass,10);
                fprintf(fp,"%s\n",pass);
		itoa(cass_mv_match_result,pass,10);
                fprintf(fp,"%s\n",pass);                				
	        fclose(fp);
#endif
}

//end


static void Configuration0_InitParam(void)
{

    configuration0_GAUSSIANSMOOTHING0.index = 1;
    configuration0_GAUSSIANSMOOTHING0.length = 3;  
	  
    configuration0_OSTUBINARY0.index = 2;
    
    configuration0_ANTICOLOR0.index = 4;
    
    configuration0_ERODE0.index = 5;
    configuration0_ERODE0.length = 3;  
	  
    configuration0_HOLLOWED0.index = 6;
       
     
}  
static void Configuration0_mainLoop(void)
{
    FILEINControl("2.bmp");
    GAUSSIANSMOOTHINGControl(&configuration0_GAUSSIANSMOOTHING0);
    OSTUBINARYControl(&configuration0_OSTUBINARY0);
    ANTICOLORControl(&configuration0_ANTICOLOR0);
    ERODEControl(&configuration0_ERODE0);
    HOLLOWEDControl(&configuration0_HOLLOWED0);
    FILEOUTControl("null");

}
  
static void Configuration0_mainPage(void)
{
    Configuration0_mainLoop();

}
  
static void Configuration0_Control()
{
    Configuration0_mainPage();

}

/********************************************************
提供给CASS平台的接口函数
*********************************************************/
int cass_mv_main_entry(void)
{
    Configuration0_Control();
    return 1;
}
int cass_mv_main_init(void)
{
    Configuration0_InitParam();
    return 1;
}
