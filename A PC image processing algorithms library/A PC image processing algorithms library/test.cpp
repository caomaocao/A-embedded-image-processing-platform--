#include<stdlib.h>
#include<string.h>
#include<stdio.h>
#include"math.h"
#include"image.h"

unsigned char raw_data[360000],out_data[360000];
unsigned char bmpinfoarray[54]={66,77,184,91,0,0,0,0,0,0,54,4,0,0,40,0,0,0,50,0,0,0,50,0,0,0,1,0,8,0,0,0,0,0,130,87,0,0,18,11,0,0,18,11,0,0,0,0,0,0,0,0,0,0};//bmp文件头信息头存放数组
unsigned char plltee[1024]={0,0,0,0,1,1,1,0,2,2,2,0,3,3,3,0,4,4,4,0,5,5,5,0,6,6,6,0,7,7,7,0,8,8,8,0,9,9,9,0,10,10,10,0,11,11,11,0,12,12,12,0,13,13,13,0,14,14,14,0,15,15,15,0,16,16,16,0,17,17,17,0,18,18,18,0,19,19,19,0,20,20,20,0,21,21,21,0,22,22,22,0,23,23,23,0,24,24,24,0,25,25,25,0,26,26,26,0,27,27,27,0,28,28,28,0,29,29,29,0,30,30,30,0,31,31,31,0,32,32,32,0,33,33,33,0,34,34,34,0,35,35,35,0,36,36,36,0,37,37,37,0,38,38,38,0,39,39,39,0,40,40,40,0,41,41,41,0,42,42,42,0,43,43,43,0,44,44,44,0,45,45,45,0,46,46,46,0,47,47,47,0,48,48,48,0,49,49,49,0,50,50,50,0,51,51,51,0,52,52,52,0,53,53,53,0,54,54,54,0,55,55,55,0,56,56,56,0,57,57,57,0,58,58,58,0,59,59,59,0,60,60,60,0,61,61,61,0,62,62,62,0,63,63,63,0,64,64,64,0,65,65,65,0,66,66,66,0,67,67,67,0,68,68,68,0,69,69,69,0,70,70,70,0,71,71,71,0,72,72,72,0,73,73,73,0,74,74,74,0,75,75,75,0,76,76,76,0,77,77,77,0,78,78,78,0,79,79,79,0,80,80,80,0,81,81,81,0,82,82,82,0,83,83,83,0,84,84,84,0,85,85,85,0,86,86,86,0,87,87,87,0,88,88,88,0,89,89,89,0,90,90,90,0,91,91,91,0,92,92,92,0,93,93,93,0,94,94,94,0,95,95,95,0,96,96,96,0,97,97,97,0,98,98,98,0,99,99,99,0,100,100,100,0,101,101,101,0,102,102,102,0,103,103,103,0,104,104,104,0,105,105,105,0,106,106,106,0,107,107,107,0,108,108,108,0,109,109,109,0,110,110,110,0,111,111,111,0,112,112,112,0,113,113,113,0,114,114,114,0,115,115,115,0,116,116,116,0,117,117,117,0,118,118,118,0,119,119,119,0,120,120,120,0,121,121,121,0,122,122,122,0,123,123,123,0,124,124,124,0,125,125,125,0,126,126,126,0,127,127,127,0,128,128,128,0,129,129,129,0,130,130,130,0,131,131,131,0,132,132,132,0,133,133,133,0,134,134,134,0,135,135,135,0,136,136,136,0,137,137,137,0,138,138,138,0,139,139,139,0,140,140,140,0,141,141,141,0,142,142,142,0,143,143,143,0,144,144,144,0,145,145,145,0,146,146,146,0,147,147,147,0,148,148,148,0,149,149,149,0,150,150,150,0,151,151,151,0,152,152,152,0,153,153,153,0,154,154,154,0,155,155,155,0,156,156,156,0,157,157,157,0,158,158,158,0,159,159,159,0,160,160,160,0,161,161,161,0,162,162,162,0,163,163,163,0,164,164,164,0,165,165,165,0,166,166,166,0,167,167,167,0,168,168,168,0,169,169,169,0,170,170,170,0,171,171,171,0,172,172,172,0,173,173,173,0,174,174,174,0,175,175,175,0,176,176,176,0,177,177,177,0,178,178,178,0,179,179,179,0,180,180,180,0,181,181,181,0,182,182,182,0,183,183,183,0,184,184,184,0,185,185,185,0,186,186,186,0,187,187,187,0,188,188,188,0,189,189,189,0,190,190,190,0,191,191,191,0,192,192,192,0,193,193,193,0,194,194,194,0,195,195,195,0,196,196,196,0,197,197,197,0,198,198,198,0,199,199,199,0,200,200,200,0,201,201,201,0,202,202,202,0,203,203,203,0,204,204,204,0,205,205,205,0,206,206,206,0,207,207,207,0,208,208,208,0,209,209,209,0,210,210,210,0,211,211,211,0,212,212,212,0,213,213,213,0,214,214,214,0,215,215,215,0,216,216,216,0,217,217,217,0,218,218,218,0,219,219,219,0,220,220,220,0,221,221,221,0,222,222,222,0,223,223,223,0,224,224,224,0,225,225,225,0,226,226,226,0,227,227,227,0,228,228,228,0,229,229,229,0,230,230,230,0,231,231,231,0,232,232,232,0,233,233,233,0,234,234,234,0,235,235,235,0,236,236,236,0,237,237,237,0,238,238,238,0,239,239,239,0,240,240,240,0,241,241,241,0,242,242,242,0,243,243,243,0,244,244,244,0,245,245,245,0,246,246,246,0,247,247,247,0,248,248,248,0,249,249,249,0,250,250,250,0,251,251,251,0,252,252,252,0,253,253,253,0,254,254,254,0,255,255,255,0};//Bmp调色板存放数组

int pic_count=0,all_threshold=0;//读入图片数量，图像阈值
int modules_width[10]={0},modules_height[10]={0};//存每个样本高，宽；应对样本大小不一致情况。
int small_leftbound=0,small_rightbound=0,small_topbound=0,small_downbound=0;//待检测区域内的框住数字最小框
int cass_mv_border_leftbound=0,cass_mv_border_topbound=0,cass_mv_border_width=0,cass_mv_border_height=0;
int cass_mv_match_result=0;
char *cass_mv_match_percent;
FILE *fp;



int Load_File(char filename[])
{
	FILE *source;
   	int  x,y,scanline;
	int width,height,fileSize,dwSize,current;
		
	if((source=fopen((char *)filename,"rb+"))==NULL)
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

    width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

	modules_width[0]=width;
	modules_height[0]=height;

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
			//out_data[(height-1-x)*scanline+y]=fgetc(source);
		}	
	}
	memcpy(out_data,raw_data,360000);
	fclose(source);
	return 1;
}	

int Compare(char txtname[])
{
	/*读取特征点列表，进行特征比对*/
	FILE *dotarray;
	int dotarraynumber[1000];//限制特征点不超过250个
	if((dotarray=fopen(txtname,"r"))==NULL)//读特征点
    {
        printf("can't open the source file!\n");
        return 0;
    }
	memset(dotarraynumber,-1,1000);
	
	int tempx,tempy,tempvalue,tempdot;
	int sum=0,sumvalue=0,x;
	int height=bmpinfo.height;
	int width=bmpinfo.width;
	int scanline;
	
	if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
	else
		scanline=width;


	x=0;//读取txt里特征点信息
	while(!feof(dotarray))
	{
		fscanf(dotarray,"%d,%d,%d,%d,",&dotarraynumber[x],&dotarraynumber[x+1],&dotarraynumber[x+2],&dotarraynumber[x+3]);
		x+=4;
	}
	
	x=3;//计算特征点总的权值
	while(1)
	{
		sum+=dotarraynumber[x];
		x+=4;
		if(dotarraynumber[x]==-1)
			break;
	}

	x=0;//把特征点和输入图做对比
	while(x<1000&&(dotarraynumber[x]!=-1))
	{
		tempx=dotarraynumber[x];
		tempy=dotarraynumber[x+1];
		tempdot=dotarraynumber[x+2];
		tempvalue=dotarraynumber[x+3];

		if(raw_data[tempx*scanline+tempy]==tempdot)
		{
			sumvalue+=tempvalue;
		}
		x+=4;
	}

	if(sumvalue>0.5*sum)
		printf("合格");
	else
		printf("不合格");
	fclose(dotarray);
	
	return 1;
}

int Save_File(int a,int height,int width)
{
	FILE *dst;
	unsigned char temp;
	int i=0,j=0,templength=0;
	double current;
	int scanline;
	char na[20];
	char dialog[30]=".\\out\\";
	char bmp[20]=".bmp";
	unsigned char *temppoint;
	unsigned char *temppointA=plltee;
	int heightarray[4]={0,0,0,0},widtharray[4]={0,0,0,0};

	temppoint=&bmpinfoarray[0];
	itoa(a,na,10);
	strcat(na,bmp);
	strcat(dialog,na);

	if((dst=fopen(dialog,"wb+"))==NULL)
	{
		printf("file cannot open\n");
		return 0;
	}

	i=3;
	templength=width;
	while(templength!=0)
	{
		widtharray[i--]=templength%16;
		templength/=16;
	}

	i=3;
	templength=height;
	while(templength!=0)
	{
		heightarray[i--]=templength%16;
		templength/=16;
	}
	bmpinfoarray[18]=widtharray[3]+widtharray[2]*16;
	bmpinfoarray[19]=widtharray[1]+widtharray[0]*16;
	bmpinfoarray[22]=heightarray[3]+heightarray[2]*16;
	bmpinfoarray[23]=heightarray[1]+heightarray[0]*16;
	
	if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
	else
		scanline=width;

	fseek(dst,0,SEEK_SET);//写bmp信息头
	i=0;
	while(i<54)
	{
		fwrite(temppoint,sizeof(unsigned char),1,dst);			
		temppoint++;
		i++;
	}
	fseek(dst,54,SEEK_SET);//写bmp调色板
	while(j<1024)
	{
		fwrite(temppointA,sizeof(unsigned char),1,dst);	
		temppointA++;
		j++;
	}
	
	fseek(dst,1078,SEEK_SET);//写数据区
	for(i=0;i<height;i++)
		for(j=0;j<scanline;j++)
		{			
			temp=raw_data[i*scanline+j];
			current=14+40+1024+(height-i-1)*scanline+j;
			fseek(dst,current,SEEK_SET);
			fwrite(&temp,sizeof(unsigned char),1,dst);			
		}
	fclose(dst);
	return 1;	
}

int Read_Bmp_Info(char filename[])
{
	FILE *fp;
	int x,y;
	
	if((fp=fopen(filename,"wb"))==NULL)
    {
        printf("can't open the source file!\n");
        return 0;
    }

	for(x=0;x<54;x++)
	{
		fprintf(fp,"%d,",bmpinfoarray[x]);
	}
	fprintf(fp,"\n");
	for(x=0;x<1024;x++)
	{
		fprintf(fp,"%d,",plltee[x]);	
	}
	fprintf(fp,"\n");

	return 1;
}

/*-------------------------------滤波*-------------------------------/
/*高斯*/
int GAUSSIANSMOOTHINGControl()
{
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

    for(y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			if(y==0||y==cass_mv_border_height-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else if(x==0||x==cass_mv_border_width-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else
			{
				v[0] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[1] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[2] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[3] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[4] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[5] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[6] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[7] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[8] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];


				sum = win[0]*v[0] + win[1]*v[1] + win[2]*v[2]
				+ win[3]*v[3] + win[4]*v[4] + win[5]*v[5]
				+ win[6]*v[6] + win[7]*v[7] + win[8]*v[8];

				sum = (int)(sum/16);
				if(sum>255)  
					sum=255;
				if(sum<0)    
					sum=0;
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=(unsigned char)sum;
			}		
		}
    }

	memcpy(raw_data,out_data,360000);
    Save_File(1, height,width);
	return 1;
}

/*索贝尔*/
int SOBELControl()
{
	int x,y,scanline;
    int cur_value,thresh_value,sumx,sumy,sum;
    int width,height;
    int v[9];
        
	static int winy[9] = { -1,-2,-1,0,0,0,1,2,1};
	static int winx[9] = { -1,0,1,-2,0,2,-1,0,1};
    width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

    for(y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			if(y==0||y==cass_mv_border_height-1)
				sum=0;
			else if(x==0||x==cass_mv_border_width-1)
				sum=0;
			else
			{
				v[0] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[1] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[2] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[3] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[4] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[5] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[6] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[7] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[8] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];

				sumx = winx[0]*v[0] + winx[1]*v[1] + winx[2]*v[2]
				+ winx[3]*v[3] + winx[4]*v[4] + winx[5]*v[5]
				+ winx[6]*v[6] + winx[7]*v[7] + winx[8]*v[8];

				sumx = (int)(sumx/16);
				if(sumx>255)  
					sumx=255;
				if(sumx<0)    
					sumx=0;

				sumy = winy[0]*v[0] + winy[1]*v[1] + winy[2]*v[2]
				+ winy[3]*v[3] + winy[4]*v[4] + winy[5]*v[5]
				+ winy[6]*v[6] + winy[7]*v[7] + winy[8]*v[8];
				sumy = (int)(sumy/16);
				if(sumy>255)  
					sumx=255;
				if(sumy<0)    
					sumx=0;

				//sum=abs(sumx)+abs(sumy);
				sum=(int)sqrt((float)(sumx*sumx+sumy*sumy));				
			}	
			out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=255-(unsigned char)sum;
			//if(sum>cass_mv_sobel_threshold)
			//	out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=255;
			//else
			//	out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=0;
		}
    }
	memcpy(raw_data,out_data,360000);
    Save_File(2, height,width);
	return 1;
}

/*普锐维特*/
int PREWITTControl()
{
	int x,y,scanline;
    int cur_value,thresh_value,sumx,sumy,sum;
    int width,height;
    int v[9];
        
	static int winy[9] = { -1,0,1,-1,0,1,-1,0,1};
	static int winx[9] = { -1,-1,1,0,0,0,1,1,1};
    width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

    for(y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			if(y==0||y==cass_mv_border_height-1)
				sum=0;
			else if(x==0||x==cass_mv_border_width-1)
				sum=0;
			else
			{
				v[0] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[1] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[2] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[3] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[4] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[5] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[6] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[7] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[8] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];

				sumx = winx[0]*v[0] + winx[1]*v[1] + winx[2]*v[2]
				+ winx[3]*v[3] + winx[4]*v[4] + winx[5]*v[5]
				+ winx[6]*v[6] + winx[7]*v[7] + winx[8]*v[8];

				sumx = (int)(sumx/16);
				if(sumx>255)  
					sumx=255;
				if(sumx<0)    
					sumx=0;

				sumy = winy[0]*v[0] + winy[1]*v[1] + winy[2]*v[2]
				+ winy[3]*v[3] + winy[4]*v[4] + winy[5]*v[5]
				+ winy[6]*v[6] + winy[7]*v[7] + winy[8]*v[8];
				sumy = (int)(sumy/16);
				if(sumy>255)  
					sumx=255;
				if(sumy<0)    
					sumx=0;

				//sum=abs(sumx)+abs(sumy);
				sum=(int)sqrt((float)(sumx*sumx+sumy*sumy));
			}
			//if(sum>cass_mv_prewitt_threshold)
			//	out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)] = 255;
			//else
			//	out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)] = 0;
			out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=255-(unsigned char)sum;
		}
    }
	memcpy(raw_data,out_data,360000);
    Save_File(3, height,width);
	return 1;
}

/*中值*/
int MEDIANFILTERControl()
{
	int x,y,scanline;
    int width,height;
    int v[9];

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

    for(y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			if(y==0||y==cass_mv_border_height-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else if(x==0||x==cass_mv_border_width-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else
			{
				int k = 0;  
				//unsigned char window[9];  
           
				for (int jj = y - 1; jj < y + 2; ++jj)  
					for (int ii = x - 1; ii < x + 2; ++ii)  
					{
						v[k++] = raw_data[(jj+cass_mv_border_topbound) * scanline + (ii+cass_mv_border_leftbound)];  
					}

				for (int m = 0; m < 5; ++m)  
				{  
					int min = m;  
					for (int n = m + 1; n < 9; ++n) 
					{
						if (v[n] < v[min])  
							min = n;  
					}
					unsigned char temp = v[m];  
					v[m] = v[min];  
					v[min] = temp;  
				}  
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)] = v[4];  
			}  
		}
    } 

	memcpy(raw_data,out_data,360000);
    Save_File(6, height,width);

	return 1;
}

/*均值*/
int MEANFILTERControl()
{
	int x,y,sum;
    int width,height,scanline;
    int v[9];

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

    for(y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			if(y==0||y==cass_mv_border_height-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else if(x==0||x==cass_mv_border_width-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else
			{
				v[0] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[1] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[2] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[3] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[4] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[5] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[6] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[7] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[8] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];

				sum = v[0] + v[1] + v[2]+ v[3] + v[4] + v[5]+ v[6] + v[7] + v[8];
				sum = (int)(sum/9);

				if(sum>255)  
					sum=255;
				if(sum<0)    
					sum=0;
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=(unsigned char)sum;
			}
			
		}
    }

	memcpy(raw_data,out_data,360000);
    Save_File(6, height,width);

	return 1;
}

int LAPLACEControl()
{
	int x,y,scanline;
    int cur_value,thresh_value,sum;
    int width,height;
    int v[9];
        
	static int win[9] = {0,1,0,1,-4,1,0,1,0};
    
	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

    for(y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			if(y==0||y==cass_mv_border_height-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else if(x==0||x==cass_mv_border_width-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else
			{
				v[0] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[1] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[2] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[3] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[4] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[5] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
				v[6] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
				v[7] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
				v[8] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];


				sum = win[0]*v[0] + win[1]*v[1] + win[2]*v[2]
				+ win[3]*v[3] + win[4]*v[4] + win[5]*v[5]
				+ win[6]*v[6] + win[7]*v[7] + win[8]*v[8];

				sum = (int)(sum/16);
				if(sum>255)  
					sum=255;
				if(sum<0)    
					sum=0;
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=(unsigned char)sum;
			}		
		}
    }

	memcpy(raw_data,out_data,360000);
    Save_File(1, height,width);
	return 1;
}

/*-------------------------------色彩-------------------------------*/
/*大津法二值化*/
int Ostu_Process()
{
	int x,y,i,j;
   int height,width,scanline,thresh_value;
   double piexlsum,greyscale;
   float piexlcount[255];
   float piexlpercent[255];
   float w0,w1,u0tmp,u1tmp,u0,u1,u,deltaTmp,deltaMax;
   unsigned char *tempspace=NULL;

   thresh_value=0;
   greyscale=255;
   deltaMax=0;
   
   //width=modules_width[0];
   //height=modules_height[0];

   	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
	height=bmpinfoarray[22]+bmpinfoarray[23]*256;

   piexlsum=cass_mv_border_width*cass_mv_border_height;//总像素

   memset(piexlcount,0,sizeof(piexlcount));
   memset(piexlpercent,0,sizeof(piexlpercent));
	
   if(width%4)
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

   for (y=0;y<cass_mv_border_height;y++)  
      for (x=0;x<cass_mv_border_width;x++)
	  { 
	    piexlcount[(int)raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]]++;//每级灰度的像素个数
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
   
   for (y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{ 		
			if(raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]<thresh_value)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=0;
			else
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=255;
        }
	} 

    memcpy(raw_data,out_data,360000);
    Save_File(2,240,320);
	return 1;
}

/*二值化*/
int BINARYControl()
{
	int x,y,scanline,width,height;

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
	height=bmpinfoarray[22]+bmpinfoarray[23]*256;
	
	if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
	else
		scanline=width;

	for(y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{ 		
			if(raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]<70)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=0;
			else
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=255;
        }
	} 

	 memcpy(raw_data,out_data,360000);
	 Save_File(3,240,320);
	 return 1;
}

/*直方图均衡化*/
int HiagramControl()
{
	int x,y,scanline;
	int width,height,max,min;
	float p[256]={0},c[256]={0},count=0;
	int n[256]={0};

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
    else
		scanline=width;
	
	for (y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			n[raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]]++;
			count++;
		}
	}
    for(x=0;x<256;x++)
	{
        p[x]=(float)n[x]/(float)(cass_mv_border_height*cass_mv_border_width);
    }

	for(x=0;x<256;x++)
	{
       for(y=0;y<=x;y++)
	   {
          c[x]+=p[y];
       }
    }

	max=min=raw_data[y+cass_mv_border_topbound*scanline+cass_mv_border_leftbound];
    for (y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
           if(max<raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)])
		   {
               max=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
           }
		   else if(min>raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)])
		   {
               min=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
           }
       }
    }

	for (y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
	   {
		  out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=c[raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]]*(max-min)+min;
	   }
	}

	memcpy(raw_data,out_data,360000);
    Save_File(5, height,width);
	return 1;

}

/*反色*/
int ANTICOLORControl()
{
	int x,y,sum;
    int width,height,scanline;

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

	for (y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=255-raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
		}
	 }
	 memcpy(raw_data,out_data,360000);
	 Save_File(5, height,width);
	 return 1;
}

/*灰度拉伸*/
int GRAYSCALESTRETCHControl()
{
	int x,y,sum;
    int width,height,scanline;
	unsigned char map[256]={0};
	double dTemp;
	float cass_mv_left_gray_in=50,cass_mv_right_gray_in=100,cass_mv_left_gray_out=20,cass_mv_right_gray_out=200;

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

	for(x=0;x<256;x++)
	{
		// 如果在第一条直线上
		if ( x < cass_mv_left_gray_in)
		{
			if ( fabs(cass_mv_left_gray_in - 0) > 0.0001 )
				dTemp = (float)cass_mv_left_gray_out / (float)cass_mv_left_gray_in * x;
			else
				dTemp = 0;
		}
		// 如果在第二条直线上
		else if ( x <= cass_mv_right_gray_in )
		{
			if ( fabs(cass_mv_right_gray_in - cass_mv_left_gray_in) > 0.0001 )
				dTemp = (float)(cass_mv_right_gray_out - cass_mv_left_gray_out) / (float)(cass_mv_right_gray_in - cass_mv_left_gray_in) * (x - cass_mv_left_gray_in) + cass_mv_left_gray_out;
			else
				dTemp = x;
		}
		// 如果在第三条直线上
		else
		{
			if ( fabs(cass_mv_right_gray_out - 255) > 0.0001 )
				dTemp = (float)(255 - cass_mv_right_gray_out) / (float)(255 - cass_mv_right_gray_in) * (x - cass_mv_right_gray_in) + cass_mv_right_gray_out;
			else
				dTemp = 255;
		}

		// 四舍五入
		map[x] = int(dTemp + 0.5);
		
        //  map[x] = (int)dTemp;
	}

	for (y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=map[raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]];
		}
	}

	memcpy(raw_data,out_data,360000);

    Save_File(5, height,width);
	return 1;
}

/*-------------------------------形态学-------------------------------*/
/*细化*/
int ZHANGTHINControl()
{
    int neighbor[8];
	int x,y,k;
    int width,height,scanline;
	bool loop=true;
    int markNum=0;
	
	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)//bmp宽度不为4要补0
		scanline=(width*8+31)/32*4;
    else
		scanline=width;


	//                                        p3  p2  p1
	//**********使用zhang并行快速算法进行细化 p4  p   p0
	//                                        p5  p6  p7
    while(loop)
    {
       loop=false;
 
       //第一步
       markNum=0;
       for (y=1;y<cass_mv_border_height-1;y++)  
		{
		for (x=1;x<cass_mv_border_width-1;x++)
           {
              //条件1：p必须是前景点
              if(raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]==0 ) 
				  continue;
 
              neighbor[0]= raw_data[(y+cass_mv_border_topbound)*scanline+x+1+cass_mv_border_leftbound] ;
              neighbor[1]= raw_data[(y-1+cass_mv_border_topbound)*scanline+x+1+cass_mv_border_leftbound];
              neighbor[2]= raw_data[(y-1+cass_mv_border_topbound)*scanline+x+cass_mv_border_leftbound];
              neighbor[3]= raw_data[(y-1+cass_mv_border_topbound)*scanline+x-1+cass_mv_border_leftbound];
              neighbor[4]= raw_data[(y+cass_mv_border_topbound)*scanline+x-1+cass_mv_border_leftbound];
              neighbor[5]= raw_data[(y+1+cass_mv_border_topbound)*scanline+x-1+cass_mv_border_leftbound];
              neighbor[6]= raw_data[(y+1+cass_mv_border_topbound)*scanline+x+cass_mv_border_leftbound];
              neighbor[7]= raw_data[(y+1+cass_mv_border_topbound)*scanline+x+1+cass_mv_border_leftbound];
 
              //条件2：2<=N(p）<=6
              int np=(neighbor[0]+neighbor[1]+neighbor[2]+neighbor[3]+neighbor[4]+neighbor[5]+neighbor[6]+neighbor[7])/255;
              if(np<2 || np>6) continue;
 
              //条件3：S(p）=1
              int sp=0;
              for(int i=1;i<8;i++)
              {
                  if(neighbor[i]-neighbor[i-1]==255)
                     sp++;
              }
              if(neighbor[0]-neighbor[7]==255)
                  sp++;            
              if(sp!=1) continue;
 
              //条件4：p2*p0*p6=0
              if(neighbor[2]&neighbor[0]&neighbor[6]!=0)
                  continue;
                //条件5：p0*p6*p4=0
              if(neighbor[0]&neighbor[6]&neighbor[4]!=0)
                  continue;
 
 
              //标记删除
              out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=1;   
              markNum++;
              loop=true;
           }
       }
 
       //将标记删除的点置为背景色
       if(markNum>0)
       {
        for (y=0;y<cass_mv_border_height;y++)  
		{
			for (x=0;x<cass_mv_border_width;x++) 
			{
                 // k=y*w+x;
                  if(out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]==1)
                  {
                     raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=0;
                  }
              }
           }
       }
      
 
       //第二步
        markNum=0;
       for (y=1;y<cass_mv_border_height-1;y++)  
		{
			for (x=1;x<cass_mv_border_width-1;x++) 
			{
              //条件1：p必须是前景点
              if(raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]==0 ) 
				  continue;
 
              neighbor[0]= raw_data[(y+cass_mv_border_topbound)*scanline+x+1+cass_mv_border_leftbound] ;
              neighbor[1]= raw_data[(y-1+cass_mv_border_topbound)*scanline+x+1+cass_mv_border_leftbound];
              neighbor[2]= raw_data[(y-1+cass_mv_border_topbound)*scanline+x+cass_mv_border_leftbound];
              neighbor[3]= raw_data[(y-1+cass_mv_border_topbound)*scanline+x-1+cass_mv_border_leftbound];
              neighbor[4]= raw_data[(y+cass_mv_border_topbound)*scanline+x-1+cass_mv_border_leftbound];
              neighbor[5]= raw_data[(y+1+cass_mv_border_topbound)*scanline+x-1+cass_mv_border_leftbound];
              neighbor[6]= raw_data[(y+1+cass_mv_border_topbound)*scanline+x+cass_mv_border_leftbound];
              neighbor[7]= raw_data[(y+1+cass_mv_border_topbound)*scanline+x+1+cass_mv_border_leftbound];
 
              //条件2：<=N(p)<=6
              int np=(neighbor[0]+neighbor[1]+neighbor[2]+neighbor[3]+neighbor[4]+neighbor[5]+neighbor[6]+neighbor[7])/255;
              if(np<2 || np>6) continue;
 
              //条件3：S(p)=1
              int sp=0;
              for(int i=1;i<8;i++)
              {
                  if(neighbor[i]-neighbor[i-1]==255)
                     sp++;
              }
              if(neighbor[0]-neighbor[7]==255)
                  sp++;
              if(sp!=1) continue;
 
              //条件4：p2*p0*p4==0
              if(neighbor[2]&neighbor[0]&neighbor[4]!=0)
                  continue;
              //条件5：p2*p6*p4==0
              if(neighbor[2]&neighbor[6]&neighbor[4]!=0)
                  continue;
 
              //标记删除
             out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=1;   
              markNum++;
              loop=true;
           }
       }
 
       //将标记删除的点置为背景色
        if(markNum>0)
       {
         for (y=0;y<cass_mv_border_height;y++)  
		{
			for (x=0;x<cass_mv_border_width;x++) 
              {
                 // k=y*w+x;
                  if(out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]==1)
                  {
                     raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=0;
                  }
              }
           }
       }
     
    } 
	Save_File(11, height,width);
	return 1;
}

/*去噪*/
int DENOSINGControl()
{
	int x,y,sum,m,n;
    int width,height,scanline;
	int v[9];

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

	for(y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			if(y==0||y==cass_mv_border_height-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else if(x==0||x==cass_mv_border_width-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else
			{
				if(raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]==0)
				{
					for( m = y-1; m<y+2; m++)
					{
						for( n = x-1;n<x+2; n++ )
						{
							if( raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]==0)
							{  
							   sum++;
							}
						}
					}                             //sum记录黑点的个数
					if(sum < 5 )   
						out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=255;
				}
			}
		}
	}
	memcpy(raw_data,out_data,360000);
	Save_File(6, height,width);
	return 1;
}

/*腐蚀*/
int ERODEControl()
{
	int scanline,k,i,j;
	int flag;
	unsigned char pix[5];

	int width=bmpinfoarray[18]+bmpinfoarray[19]*255;
	int height=bmpinfoarray[22]+bmpinfoarray[23]*255;

	if(width%4)
		scanline=(width*8+31)/32*4;
	else
		scanline=width;

	for (i=1;i<cass_mv_border_height-1;i++)  
	{
		for (j=1;j<cass_mv_border_width-1;j++)
		{
			flag=1;
			pix[0]=raw_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound];
			pix[1]=raw_data[(i+cass_mv_border_topbound)*scanline+j-2+cass_mv_border_leftbound];
			pix[2]=raw_data[(i+cass_mv_border_topbound)*scanline+j-1+cass_mv_border_leftbound];
			pix[3]=raw_data[(i+cass_mv_border_topbound)*scanline+j+1+cass_mv_border_leftbound];
			pix[4]=raw_data[(i+cass_mv_border_topbound)*scanline+j+2+cass_mv_border_leftbound];

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
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=0;
			else
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=255;
		}
	}
	memcpy(raw_data,out_data,360000);
	Save_File(10,height,width);
	return 1;
}

/*膨胀*/
int DIALATEControl()
{
	int scanline,k,i,j;
	int flag;
	unsigned char pix[5];

	int width=bmpinfoarray[18]+bmpinfoarray[19]*255;
	int height=bmpinfoarray[22]+bmpinfoarray[23]*255;

	if(width%4)
		scanline=(width*8+31)/32*4;
	else
		scanline=width;

	for (i=1;i<cass_mv_border_height-1;i++)  
	{
		for (j=1;j<cass_mv_border_width-1;j++)
		{
			flag=1;
			pix[0]=raw_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound];
			pix[1]=raw_data[(i+cass_mv_border_topbound)*scanline+j-2+cass_mv_border_leftbound];
			pix[2]=raw_data[(i+cass_mv_border_topbound)*scanline+j-1+cass_mv_border_leftbound];
			pix[3]=raw_data[(i+cass_mv_border_topbound)*scanline+j+1+cass_mv_border_leftbound];
			pix[4]=raw_data[(i+cass_mv_border_topbound)*scanline+j+2+cass_mv_border_leftbound];

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
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=255;
			else
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=0;
		}
	}
	memcpy(raw_data,out_data,360000);
	Save_File(9,height,width);
	return 1;
}

/*开操作*/
int OPENControl()
{
	int scanline,k,i,j;
	int flag;
	unsigned char pix[5];

	int width=bmpinfoarray[18]+bmpinfoarray[19]*255;
	int height=bmpinfoarray[22]+bmpinfoarray[23]*255;

	if(width%4)
		scanline=(width*8+31)/32*4;
	else
		scanline=width;

	for (i=1;i<cass_mv_border_height-1;i++)  
	{
		for (j=1;j<cass_mv_border_width-1;j++)
		{
			flag=1;
			pix[0]=raw_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound];
			pix[1]=raw_data[(i+cass_mv_border_topbound)*scanline+j-2+cass_mv_border_leftbound];
			pix[2]=raw_data[(i+cass_mv_border_topbound)*scanline+j-1+cass_mv_border_leftbound];
			pix[3]=raw_data[(i+cass_mv_border_topbound)*scanline+j+1+cass_mv_border_leftbound];
			pix[4]=raw_data[(i+cass_mv_border_topbound)*scanline+j+2+cass_mv_border_leftbound];

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
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=0;
			else
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=255;
		}
	}
	memcpy(raw_data,out_data,360000);

	for (i=1;i<cass_mv_border_height-1;i++)  
	{
		for (j=1;j<cass_mv_border_width-1;j++)
		{
			flag=1;
			pix[0]=raw_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound];
			pix[1]=raw_data[(i+cass_mv_border_topbound)*scanline+j-2+cass_mv_border_leftbound];
			pix[2]=raw_data[(i+cass_mv_border_topbound)*scanline+j-1+cass_mv_border_leftbound];
			pix[3]=raw_data[(i+cass_mv_border_topbound)*scanline+j+1+cass_mv_border_leftbound];
			pix[4]=raw_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound];

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
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=255;
			else
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=0;
		}
	}
	memcpy(raw_data,out_data,360000);
	Save_File(11,height,width);
	return 1;
}

/*闭操作*/
int CLOSEControl()
{
	int scanline,k,i,j;
	int flag;
	unsigned char pix[5];

	int width=bmpinfoarray[18]+bmpinfoarray[19]*255;
	int height=bmpinfoarray[22]+bmpinfoarray[23]*255;

	if(width%4)
		scanline=(width*8+31)/32*4;
	else
		scanline=width;

		for (i=1;i<cass_mv_border_height-1;i++)  
	{
		for (j=1;j<cass_mv_border_width-1;j++)
		{
			flag=1;
			pix[0]=raw_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound];
			pix[1]=raw_data[(i+cass_mv_border_topbound)*scanline+j-2+cass_mv_border_leftbound];
			pix[2]=raw_data[(i+cass_mv_border_topbound)*scanline+j-1+cass_mv_border_leftbound];
			pix[3]=raw_data[(i+cass_mv_border_topbound)*scanline+j+1+cass_mv_border_leftbound];
			pix[4]=raw_data[(i+cass_mv_border_topbound)*scanline+j+2+cass_mv_border_leftbound];

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
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=255;
			else
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=0;
		}
	}
	memcpy(raw_data,out_data,360000);

	for (i=1;i<cass_mv_border_height-1;i++)  
	{
		for (j=1;j<cass_mv_border_width-1;j++)
		{
			flag=1;
			pix[0]=raw_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound];
			pix[1]=raw_data[(i+cass_mv_border_topbound)*scanline+j-2+cass_mv_border_leftbound];
			pix[2]=raw_data[(i+cass_mv_border_topbound)*scanline+j-1+cass_mv_border_leftbound];
			pix[3]=raw_data[(i+cass_mv_border_topbound)*scanline+j+1+cass_mv_border_leftbound];
			pix[4]=raw_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound];

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
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=0;
			else
				out_data[(i+cass_mv_border_topbound)*scanline+j+cass_mv_border_leftbound]=255;
		}
	}
	memcpy(raw_data,out_data,360000);
	Save_File(12,height,width);
	return 1;
}

/*-------------------------------轮廓-------------------------------*/
/*掏空*/
int HOLLOWEDControl()
{
	int x,y,sum;
    int width,height,scanline;
	int v[9];

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

	for (y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			if(y==0||y==cass_mv_border_height-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else if(x==0||x==cass_mv_border_width-1)
				out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			else
			{
				if(raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]==0)
				{
					v[0] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
					v[1] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
					v[2] = raw_data[(y-1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
					v[3] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
					v[4] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
					v[5] = raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];
					v[6] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)-1];
					v[7] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
					v[8] = raw_data[(y+1+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)+1];

					if(v[0]+v[1]+v[2]+v[3]+v[5]+v[6]+v[7]+v[8]==0)
						out_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)]=255;
				}
			}
		}
	}
	memcpy(raw_data,out_data,360000);
    Save_File(6, height,width);
	return 1;
}

/*-------------------------------数值计算-------------------------------*/
/*计算图片重心*/
int CENTEROFGRAVITYControl()
{
	int x,y,sum;
    int width,height,scanline;
	double s00=0,s10=0,s01=0;//0阶矩和1阶矩
	int Center_x=0,Center_y=0;//重心坐标

	width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    height=bmpinfoarray[22]+bmpinfoarray[23]*256;

    if(width%4)
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

	for (y=0;y<cass_mv_border_height;y++)  
	{
		for (x=0;x<cass_mv_border_width;x++)
		{
			s00+=raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			s10+=x*raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
			s01+=y*raw_data[(y+cass_mv_border_topbound)*scanline+(x+cass_mv_border_leftbound)];
		}
	}

	Center_x=(s10/s00+0.5);
    Center_y=(s01/s00+0.5);

	//将重心周五3×3区域灰度置于128
	raw_data[(Center_y-1+cass_mv_border_topbound)*scanline+Center_x-1+cass_mv_border_leftbound]=128;
	raw_data[(Center_y-1+cass_mv_border_topbound)*scanline+Center_x+cass_mv_border_leftbound]=128;
	raw_data[(Center_y-1+cass_mv_border_topbound)*scanline+Center_x+1+cass_mv_border_leftbound]=128;
	raw_data[(Center_y+cass_mv_border_topbound)*scanline+Center_x-1+cass_mv_border_leftbound]=128;
	raw_data[(Center_y+cass_mv_border_topbound)*scanline+Center_x+cass_mv_border_leftbound]=128;
	raw_data[(Center_y+cass_mv_border_topbound)*scanline+Center_x+1+cass_mv_border_leftbound]=128;
	raw_data[(Center_y+1+cass_mv_border_topbound)*scanline+Center_x-1+cass_mv_border_leftbound]=128;
	raw_data[(Center_y+1+cass_mv_border_topbound)*scanline+Center_x+cass_mv_border_leftbound]=128;
	raw_data[(Center_y+1+cass_mv_border_topbound)*scanline+Center_x+1+cass_mv_border_leftbound]=128;

	Save_File(9, height,width);
	return 1;
}


/*识别*/
void NUMMATCHControl()
{
	int tempx,tempy,tempvalue,tempdot,scanline,temp;
	int sum=0,sumvalue=0,x=0,count=0;
	int space_count=0;//待识别区域个数
	int compare_sum[10]={0},correct_sum[10]={0};
	int small_leftbound=0,small_rightbound=0,small_topbound=0,small_downbound=0;//待检测区域内的框住数字最小框
	int temp_topbound,temp_leftbound;
	int i,j;
	
	int width=bmpinfoarray[18]+bmpinfoarray[19]*256;
    int height=bmpinfoarray[22]+bmpinfoarray[23]*256;
	
	if(width%4)
		scanline=(width*8+31)/32*4;
    else
		scanline=width;

	for(i=0;i<10;i++)
	{
		int temp_sum=0;
		for( j=0;j<30;j++)
		{
			temp=abs(image_data_packet_desc.space[0].point[i*30+j].w);
			if(temp!=100)//权值为100的点为模板中数字的左上，右下点，
				temp_sum+=temp;
		}
		correct_sum[i]=temp_sum;
	}
	
	for(i=cass_mv_border_leftbound;i<cass_mv_border_leftbound+cass_mv_border_width;i++)
	{	
		count=0;
		for(j=cass_mv_border_topbound;j<cass_mv_border_topbound+cass_mv_border_height;j++)
		{
			if(raw_data[j*scanline+i]==255)
				count++;
		}
		if(count>0)
		{
			small_leftbound=i;
			break;
		}
	}
	for(i=cass_mv_border_leftbound+cass_mv_border_width-1;i>=cass_mv_border_leftbound;i--)
	{
		count=0;
		for(j=cass_mv_border_topbound;j<cass_mv_border_topbound+cass_mv_border_height;j++)
		{
			if(raw_data[j*scanline+i]==255)
				count++;
		}
		if(count>0)
		{
			small_rightbound=i;//LCD下边
			break;
		}
	}
	for(j=cass_mv_border_topbound;j<cass_mv_border_topbound+cass_mv_border_height;j++)
	{
		count=0;
		for(i=cass_mv_border_leftbound;i<cass_mv_border_leftbound+cass_mv_border_width;i++)
		{
			
			if(raw_data[j*scanline+i]==255)
				count++;
		}
		if(count>0)
		{
			small_topbound=j;//LCD左边
			break;
		}
	}
	for(j=cass_mv_border_topbound+cass_mv_border_height-1;j>=cass_mv_border_topbound;j--)
	{
		count=0;
		for(i=cass_mv_border_leftbound;i<cass_mv_border_leftbound+cass_mv_border_width;i++)
		{
			if(raw_data[j*scanline+i]==255)
				count++;
		}
		if(count>0)
		{
			small_downbound=j;//LCD右边
			break;
		}
	}
	
	if(((small_rightbound-small_leftbound)/(small_downbound-small_topbound))>3)
		cass_mv_match_result=1;//最小框长宽比大于3，直接识别为数字1
	else
	{
		int small_temp_leftbound,small_temp_topbound;
	    int max=compare_sum[0];
		int maxnumber;
		for( i=0;i<10;i++)
		{
			int value_sum=0;
			/*模板内的数字最小框左上角*/
			if(image_data_packet_desc.space[0].point[i*30].w==100)//100是确保坐标是最小框左上角
			{
				small_temp_topbound=image_data_packet_desc.space[0].point[i*30].y;
				small_temp_leftbound=image_data_packet_desc.space[0].point[i*30].x;	 
			}

			if(i==1)
			{
				compare_sum[1]=0;
				continue;
			}

			for( j=1;j<30;j++)
			{
			
				tempx=image_data_packet_desc.space[0].point[i*30+j].x-small_temp_leftbound;//相对坐标：坐标系为模板中数字最小框
				tempy=image_data_packet_desc.space[0].point[i*30+j].y-small_temp_topbound;
				tempdot=image_data_packet_desc.space[0].point[i*30+j].v;
				tempvalue=abs(image_data_packet_desc.space[0].point[i*30+j].w);
				
				if((image_data_packet_desc.space[0].point[i*30+j].w)!=0)//不是空点
					if(raw_data[(small_topbound+tempy)*scanline+(small_leftbound+tempx)]==tempdot)//待识别区域中 数字最小框左上角 加上 特征点相对坐标
						value_sum+=tempvalue;
			}
			compare_sum[i]=value_sum;
		}

		for( i=0;i<10;i++)//算百分比
		{
			if(correct_sum[i])
				compare_sum[i]=compare_sum[i]*100/correct_sum[i];
			else
				getchar();
			
		}

		for( i=0;i<10;i++)
			if(max<=compare_sum[i])
			{
				max=compare_sum[i];
				maxnumber=i;
			}
		
		cass_mv_match_result=maxnumber;
		cass_mv_match_percent="UINT";
	}
}

int main()
{
	char filename[255]="2.bmp",txtname[255]="bmpinfo.txt",savename[255]="out.bmp";

	//Read_Bmp_Info();
	Load_File(filename);
	Save_File(1,240,320);
	
	cass_mv_border_leftbound=local_area_array[0].leftbound;
	cass_mv_border_topbound=local_area_array[0].topbound;
	cass_mv_border_height=local_area_array[0].height;
	cass_mv_border_width=local_area_array[0].width;

	//GRAYSCALESTRETCHControl();
	//GAUSSIANSMOOTHINGControl();
	//PREWITTControl();
	//MEDIANFILTERControl();
	//MEANFILTERControl();

	//HiagramControl();
	//GRAYSCALESTRETCHControl();
	//BINARYControl();
	Ostu_Process();
	//ANTICOLORControl();
	
	//DIALATEControl();
	//ERODEControl();
	//OPENControl();
	//CLOSEControl();
	//ZHANGTHINControl();
	//HOLLOWEDControl();
	CENTEROFGRAVITYControl();


	//NUMMATCHControl();
	return 1;
}