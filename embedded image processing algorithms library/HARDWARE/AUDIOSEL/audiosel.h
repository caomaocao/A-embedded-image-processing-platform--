#ifndef __AUDIOSEL_H
#define __AUDIOSEL_H
#include "sys.h"
//////////////////////////////////////////////////////////////////////////////////	 
//������ֻ��ѧϰʹ�ã�δ��������ɣ��������������κ���;
//ALIENTEKս��STM32������
//��Ƶѡ���� ��������	   
//����ԭ��@ALIENTEK
//������̳:www.openedv.com
//�޸�����:2012/9/14
//�汾��V1.0
//��Ȩ���У�����ؾ���
//Copyright(C) ������������ӿƼ����޹�˾ 2009-2019
//All rights reserved									  
//////////////////////////////////////////////////////////////////////////////////

//74HC4052�������ö˿�
#define AUDIO_SELB	PBout(7)
#define AUDIO_SELA	PDout(7)
//����ͨ��ѡ��
#define AUDIO_MP3 		0	//MP3ͨ��
#define AUDIO_RADIO		1	//������ͨ��
#define AUDIO_PWM		2	//PWM��Ƶͨ��
#define AUDIO_NONE		3	//����    						 
							    
void Audiosel_Init(void);	//��ʼ������
void Audiosel_Set(u8 ch);   //���ú���

#endif
















