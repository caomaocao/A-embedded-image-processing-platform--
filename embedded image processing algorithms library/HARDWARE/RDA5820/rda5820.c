#include "rda5820.h"
#include "delay.h"
//////////////////////////////////////////////////////////////////////////////////	 
//������ֻ��ѧϰʹ�ã�δ��������ɣ��������������κ���;
//ALIENTEKս��STM32������
//RDA5820 ��������	   
//����ԭ��@ALIENTEK
//������̳:www.openedv.com
//�޸�����:2012/9/14
//�汾��V1.0
//��Ȩ���У�����ؾ���
//Copyright(C) ������������ӿƼ����޹�˾ 2009-2019
//All rights reserved									  
//////////////////////////////////////////////////////////////////////////////////

//��ʼ��
//0,��ʼ���ɹ�;
//����,��ʼ��ʧ��.
u8 RDA5820_Init(void)
{
	u16 id;
	IIC_Init();						//��ʼ��IIC��	
	id=RDA5820_RD_Reg(RDA5820_R00);	//��ȡID =0X5805
	if(id==0X5805)					//��ȡID��ȷ
	{
	 	RDA5820_WR_Reg(RDA5820_R02,0x0002);	//��λ
		delay_ms(50);
	 	RDA5820_WR_Reg(RDA5820_R02,0xC001);	//������,�ϵ�
		delay_ms(600);						//�ȴ�ʱ���ȶ� 
	 	RDA5820_WR_Reg(RDA5820_R05,0X884F);	//����ǿ��8,LNAN,1.8mA,VOL���
 	 	RDA5820_WR_Reg(0X07,0X7800);		// 
	 	RDA5820_WR_Reg(0X13,0X0008);		// 
	 	RDA5820_WR_Reg(0X15,0x1420);		//VCO����  0x17A0/0x1420 
	 	RDA5820_WR_Reg(0X16,0XC000);		//  
	 	RDA5820_WR_Reg(0X1C,0X3126);		// 
	 	RDA5820_WR_Reg(0X22,0X9C24);		//fm_true 
	 	RDA5820_WR_Reg(0X47,0XF660) ;		//tx rds 
 	}else return 1;//��ʼ��ʧ��
	return 0;
}
//дRDA5820�Ĵ���						 				    
void RDA5820_WR_Reg(u8 addr,u16 val)
{
    IIC_Start();  			   
	IIC_Send_Byte(RDA5820_WRITE);	//����д����   	 
	IIC_Wait_Ack();	   
    IIC_Send_Byte(addr);   			//���͵�ַ
	IIC_Wait_Ack(); 	 										  		   
	IIC_Send_Byte(val>>8);     		//���͸��ֽ�							   
	IIC_Wait_Ack();  		    	   
 	IIC_Send_Byte(val&0XFF);     	//���͵��ֽ�							   
 	IIC_Wait_Ack();  		    	   
    IIC_Stop();						//����һ��ֹͣ���� 	 
}
//��RDA5820�Ĵ���	
u16 RDA5820_RD_Reg(u8 addr)
{
	u16 res;
    IIC_Start();  			   
	IIC_Send_Byte(RDA5820_WRITE);	//����д����   	 
	IIC_Wait_Ack();	   
    IIC_Send_Byte(addr);   			//���͵�ַ
	IIC_Wait_Ack(); 	 										  		   
    IIC_Start();  			   
	IIC_Send_Byte(RDA5820_READ);    //���Ͷ�����							   
	IIC_Wait_Ack();  		    	   
 	res=IIC_Read_Byte(1);     		//�����ֽ�,����ACK	  
  	res<<=8;
  	res|=IIC_Read_Byte(0);     		//�����ֽ�,����NACK							   
  	IIC_Stop();						//����һ��ֹͣ���� 
	return res;						//���ض���������
}
//����RDA5820ΪRXģʽ
void RDA5820_RX_Mode(void)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X40);		//��ȡ0X40������
	temp&=0xfff0;					//RX ģʽ   
	RDA5820_WR_Reg(0X40,temp) ;		//FM RXģʽ 
}			
//����RDA5820ΪTXģʽ
void RDA5820_TX_Mode(void)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X40);		//��ȡ0X40������
	temp&=0xfff0;
	temp|=0x0001;				    //TX ģʽ
	RDA5820_WR_Reg(0X40,temp) ;		//FM TM ģʽ 
}
//�õ��ź�ǿ��
//����ֵ��Χ:0~127
u8 RDA5820_Rssi_Get(void)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X0B);		//��ȡ0X0B������
	return temp>>9;                 //�����ź�ǿ��
}
//��������ok
//vol:0~15;
void RDA5820_Vol_Set(u8 vol)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X05);		//��ȡ0X05������
	temp&=0XFFF0;
	temp|=vol&0X0F;				   		 
	RDA5820_WR_Reg(0X05,temp) ;		//��������	
}
//��������
//mute:0,������;1,����
void RDA5820_Mute_Set(u8 mute)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X02);		//��ȡ0X02������
	if(mute)temp|=1<<14;
	else temp&=~(1<<14);	    		   		 
	RDA5820_WR_Reg(0X02,temp) ;		//����MUTE	
}
//����������
//rssi:0~127;
void RDA5820_Rssi_Set(u8 rssi)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X05);		//��ȡ0X05������
	temp&=0X80FF;
	temp|=(u16)rssi<<8;				   		 
	RDA5820_WR_Reg(0X05,temp) ;		//����RSSI	
}
//����TX���͹���
//gain:0~63
void RDA5820_TxPAG_Set(u8 gain)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X42);		//��ȡ0X42������
	temp&=0XFFC0;
	temp|=gain;				   		//GAIN
	RDA5820_WR_Reg(0X42,temp) ;		//����PA�Ĺ���
}
//����TX �����ź�����
//gain:0~7
void RDA5820_TxPGA_Set(u8 gain)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X42);		//��ȡ0X42������
	temp&=0XF8FF;
	temp|=gain<<8;			    	//GAIN
	RDA5820_WR_Reg(0X42,temp) ;		//����PGA
}
//����RDA5820�Ĺ���Ƶ��
//band:0,87~108Mhz;1,76~91Mhz;2,76~108Mhz;3,�û��Զ���(53H~54H)
void RDA5820_Band_Set(u8 band)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X03);		//��ȡ0X03������
	temp&=0XFFF3;
	temp|=band<<2;			     
	RDA5820_WR_Reg(0X03,temp) ;		//����BAND
}
//����RDA5820�Ĳ���Ƶ��
//band:0,100Khz;1,200Khz;3,50Khz;3,����
void RDA5820_Space_Set(u8 spc)
{
	u16 temp;
	temp=RDA5820_RD_Reg(0X03);		//��ȡ0X03������
	temp&=0XFFFC;
	temp|=spc;			     
	RDA5820_WR_Reg(0X03,temp) ;		//����BAND
}
//����RDA5820��Ƶ��
//freq:Ƶ��ֵ(��λΪ10Khz),����10805,��ʾ108.05Mhz
void RDA5820_Freq_Set(u16 freq)
{
	u16 temp;
	u8 spc=0,band=0;
	u16 fbtm,chan;
	temp=RDA5820_RD_Reg(0X03);	//��ȡ0X03������
	temp&=0X001F;
	band=(temp>>2)&0x03;		//�õ�Ƶ��
	spc=temp&0x03; 				//�õ��ֱ���
	if(spc==0)spc=10;
	else if(spc==1)spc=20;
	else spc=5;
	if(band==0)fbtm=8700;
	else if(band==1||band==2)fbtm=7600;
	else 
	{
		fbtm=RDA5820_RD_Reg(0X53);//�õ�bottomƵ��
		fbtm*=10;
	}
	if(freq<fbtm)return;
	chan=(freq-fbtm)/spc;		//�õ�CHANӦ��д���ֵ
	chan&=0X3FF;				//ȡ��10λ	  
	temp|=chan<<6;
	temp|=1<<4;					//TONE ENABLE			     
	RDA5820_WR_Reg(0X03,temp) ;	//����Ƶ��
	delay_ms(20);				//�ȴ�20ms
	while((RDA5820_RD_Reg(0X0B)&(1<<7))==0);//�ȴ�FM_READY
	
}
//�õ���ǰƵ��
//����ֵ:Ƶ��ֵ(��λ10Khz)
u16 RDA5820_Freq_Get(void)
{
	u16 temp;
	u8 spc=0,band=0;
	u16 fbtm,chan;
	temp=RDA5820_RD_Reg(0X03);		//��ȡ0X03������
	chan=temp>>6;   
	band=(temp>>2)&0x03;		//�õ�Ƶ��
	spc=temp&0x03; 				//�õ��ֱ���
	if(spc==0)spc=10;
	else if(spc==1)spc=20;
	else spc=5;
	if(band==0)fbtm=8700;
	else if(band==1||band==2)fbtm=7600;
	else 
	{
		fbtm=RDA5820_RD_Reg(0X53);//�õ�bottomƵ��
		fbtm*=10;
	}
 	temp=fbtm+chan*spc;				 
	return temp;//����Ƶ��ֵ
}








































