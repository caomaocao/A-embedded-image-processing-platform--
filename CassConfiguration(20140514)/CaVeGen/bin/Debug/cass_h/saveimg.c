
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
	bmpinfoarray[18]=widtharray[3]+widtharray[2]*16;
	bmpinfoarray[19]=widtharray[1]+widtharray[0]*16;

	i=3;
	templength=height;
	while(templength!=0)
	{
		heightarray[i--]=templength%16;
		templength/=16;
	}


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