#include "audiosel.h"	  
//////////////////////////////////////////////////////////////////////////////////	 
//������ֻ��ѧϰʹ�ã�δ���������ɣ��������������κ���;
//ALIENTEKս��STM32������
//��Ƶѡ���� ��������	   
//����ԭ��@ALIENTEK
//������̳:www.openedv.com
//�޸�����:2012/9/14
//�汾��V1.0
//��Ȩ���У�����ؾ���
//Copyright(C) �������������ӿƼ����޹�˾ 2009-2019
//All rights reserved									  
//////////////////////////////////////////////////////////////////////////////////

//������ʼ��
void Audiosel_Init(void)
{
	RCC->APB2ENR|=1<<3;    //ʹ��PORTBʱ��	   	 
	RCC->APB2ENR|=1<<5;    //ʹ��PORTDʱ��	
 	GPIOB->CRL&=0X0FFFFFFF; 
	GPIOB->CRL|=0X30000000;//PB.7 ������� 
 	GPIOD->CRL&=0X0FFFFFFF; 
	GPIOD->CRL|=0X30000000;//PD.7 ������� 
}
//����4052��ѡ��ͨ��
//����ͨ��ѡ��
//0	//MP3ͨ��
//1	//������ͨ��
//2	//PWM��Ƶͨ��
//3	//����  
void Audiosel_Set(u8 ch)
{
	AUDIO_SELA=ch&0X01;
 	AUDIO_SELB=(ch>>1)&0X01;	 
}



