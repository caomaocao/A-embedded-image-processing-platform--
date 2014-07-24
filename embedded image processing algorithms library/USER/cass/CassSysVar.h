////////////////////////////////////////////////////////////////////////////////////////////
// ��   ��   ��  : CassSysVar.h
// ��        ��  :                            
// Ŀ�ļ���Ҫ����: ϵͳȫ�ֱ����ͺ��������,����IO�˿ڣ����ڣ�IIC�ڶ�������ݲ�ͬCPU�����޸�
// �� �� ��  ��  : 2008.3.5
// �� �� ��  ��  : 2008.3.5                    
// ��        ��  : hfl
// ��   ��   ��  :                      
////////////////////////////////////////////////////////////////////////////////////////////

#ifndef  CASSSYSVAR_H
#define  CASSSYSVAR_H

#include "CassSysCfg.h"
#include "CassType.h"
//#include "CassAutoVar.h"

//ȫ�ֱ�����
#ifdef  _GLOBAL_VAR_DEFINE_
#define  EXTERN_VAR  
#else
#define  EXTERN_VAR    extern
#endif
//��ʱ��
#define M_TIMER_INTERVAL 1000  //��ʱ����С��ʱ�䵱������λ ����
//����������������

//�˿ڶ��壬����ݲ�ͬCPU���ж��壬����Ϊ���������ӳ��PLCIN000��PLCIN***�����������ӳ��PLCOUT000��PLCOUT***���������������ӳ��PLCINCFG000��PLCINCFG***���������������ӳ��PLCOUTCFG000��PLCOUTCFG***
#define PLCIN000 IO0PIN
#define PLCIN001 IO1PIN
#define PLCIN002 IO2PIN
#define PLCIN003 IO3PIN

#define PLCOUT000S0 IO0SET
#define PLCOUT000S1 IO0CLR
#define PLCOUT001S0 IO1SET
#define PLCOUT001S1 IO1CLR
#define PLCOUT002S0 IO2SET
#define PLCOUT002S1 IO2CLR
#define PLCOUT003S0 IO3SET
#define PLCOUT003S1 IO3CLR

#define PLCINCFG000 IO0DIR
#define PLCINCFG001 IO1DIR
#define PLCINCFG002 IO2DIR
#define PLCINCFG003 IO3DIR

#define PLCOUTCFG000 IO0DIR
#define PLCOUTCFG001 IO1DIR
#define PLCOUTCFG002 IO2DIR
#define PLCOUTCFG003 IO3DIR

#if defined MIDDLEMODE       //���������ϵͳ�����������CASS�˿ڶ���
#define PLCMIDIN000 7
#define PLCMIDIN001 6
#define PLCMIDIN002 5
#define PLCMIDIN003 4
#define PLCMIDIN004 3
#define PLCMIDIN005 2
#define PLCMIDIN006 1
#define PLCMIDIN007 0

#define PLCMIDOUT000 7
#define PLCMIDOUT001 6
#define PLCMIDOUT002 5
#define PLCMIDOUT003 4
#define PLCMIDOUT004 3
#define PLCMIDOUT005 2
#define PLCMIDOUT006 1
#define PLCMIDOUT007 0

#elif defined BIGMODE        //����Ǵ���ϵͳ�����������CASS�˿ڶ���
#define PLCBIGIN000 0x0
#define PLCBIGIN001 0x0
#define PLCBIGIN002 0x0
#define PLCBIGIN003 0x0
#define PLCBIGIN004 0x0
#define PLCBIGIN005 0x0
#define PLCBIGIN006 0x0

#define PLCBIGOUT000 0x0
#define PLCBIGOUT001 0x0
#define PLCBIGOUT002 0x0
#define PLCBIGOUT003 0x0
#define PLCBIGOUT004 0x0
#define PLCBIGOUT005 0x0
#define PLCBIGOUT006 0x0


#endif

//��ӳ״̬�ı���
EXTERN_VAR uint8 PLCRun;	//ϵͳ���п���״̬1 = stop,0 = run
EXTERN_VAR uint8 ErrorFlag ; //ϵͳ���г����־

#define COM_READ	3		//�����ܺ�
#define COM_WRITE	6		//д���ܺ�
EXTERN_VAR uint8  *pComInBuf ;				//���ܻ����������ַָ��
EXTERN_VAR uint8  *pComOutBuf ;			//���ͻ������߼���ַָ��
EXTERN_VAR uint8  *pComInBuf ;				//���ջ������߼���ַָ��
EXTERN_VAR uint8  *pComDataLen ;			//�������ݳ���
EXTERN_VAR uint8  *pComDataCur ;			//��������λ��ָ��
EXTERN_VAR uint8  *pComInTimeover ;			//���յ����ݺ�ȴ���һ�ַ��ĳ�ʱʱ��

//����ͨѶ��ض���
//����Ҫ��ѭ��ǰ�ķ�񣬴��ڶ����1��ʼ
#ifdef M_UART0
//����1�ڴ涨��
	EXTERN_VAR uint8 COMInBuf0[IN_BUFLEN_MAX_UART0] ;		//���뻺����
	EXTERN_VAR uint8 COMOutBuf0[IN_BUFLEN_MAX_UART0] ;	//���������
	EXTERN_VAR uint8 COMDataLen0 ;						//������ݳ���
	EXTERN_VAR uint8 COMDataCur0 ;						//���ݴ���ָ��
	EXTERN_VAR uint8 COMInTimeover0 ;					//���ݽ��ճ�ʱʱ��
	EXTERN_VAR uint8 COMInFrmFlag0 ;					//���ݽ��յ�һ֡�ı�־
#endif
#ifdef M_UART1
//����2�ڴ涨��
	EXTERN_VAR uint8 COMInBuf1[IN_BUFLEN_MAX_UART1] ;		//���뻺����
	EXTERN_VAR uint8 COMOutBuf1[IN_BUFLEN_MAX_UART1] ;	//���������
	EXTERN_VAR uint8 COMDataLen1 ;						//������ݳ���
	EXTERN_VAR uint8 COMDataCur1 ;						//���ݴ���ָ��
	EXTERN_VAR uint8 COMInTimeover1 ;					//���ݽ��ճ�ʱʱ��
	EXTERN_VAR uint8 COMInFrmFlag1 ;					//���ݽ��յ�һ֡�ı�־
#endif

#ifdef M_UART2
//����3�ڴ涨��
	EXTERN_VAR uint8 COMInBuf2[IN_BUFLEN_MAX_UART2] ;		//���뻺����
	EXTERN_VAR uint8 COMOutBuf2[IN_BUFLEN_MAX_UART2] ;	//���������
	EXTERN_VAR uint8 COMDataLen2 ;						//������ݳ���
	EXTERN_VAR uint8 COMDataCur2 ;						//���ݴ���ָ��
	EXTERN_VAR uint8 COMInTimeover2 ;					//���ݽ��ճ�ʱʱ��
	EXTERN_VAR uint8 COMInFrmFlag2 ;					//���ݽ��յ�һ֡�ı�־
#endif
//���г����д����ڴ涨��

EXTERN_VAR uint8 LastPLCRun;  //���п�����һ��״̬

EXTERN_VAR uint16 nEngineFirstCurTime; //���������߼��ĵ�ǰʱ��
EXTERN_VAR uint16 nEngineSecondCurTime;//��������ĵ�ǰʱ��

EXTERN_VAR uint8 Time1msCur ; //�����ۻ�10ms��ʱ��
EXTERN_VAR uint8 Time2msCur ; //�����ۻ�2ms��ʱ��
EXTERN_VAR uint8 Time4msCur ; //�����ۻ�4ms��ʱ��
EXTERN_VAR uint8 Time5msCur ; //�����ۻ�5ms��ʱ��
EXTERN_VAR uint8 Time10msCur ;//�����ۻ�100ms��ʱ��
EXTERN_VAR uint8 Time100msCur ;//�����ۻ�1000ms��ʱ��

EXTERN_VAR uint8 T10msFlag ;//10ms�¼������
EXTERN_VAR uint8 T100msFlag ;//100ms�¼������
EXTERN_VAR uint8 T1000msFlag ;//100ms�¼������

extern uint8 MCStack ;	//PLC����������õ���ĸ�߶�ջ
extern uint8 CurStatus ;	//PLC��������еĵ�ǰ״̬
extern uint8 WorkStack ;//PLC����������õ��Ĺ�������ջ
extern uint8 MPStack ;//PLC����������õ����߼���ջ
extern uint8 TempValue ;//PLC����������õ�����ʱ����

EXTERN_VAR uint8 RunLedStatus; //����ָʾ��״̬��1=on��0=off

//���ڶ˿ڲ������룬���迼�ǣ���ע��ֱ�Ӷ˿���PLC�˿ڵ�����
//������ֱ�Ӷ˿�PLCPORT���ó�����

#define CfgInputPort(PLCPORT) GPIO_InitStructure.GPIO_Pin = GPIO_Pin_All;\
                              GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;\
                              GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;\
                              GPIO_Init(PLCPORT, &GPIO_InitStructure)
//������ֱ�Ӷ˿�PLCPORT���ó����     
#define CfgOutputPort(PLCPORT) GPIO_InitStructure.GPIO_Pin = GPIO_Pin_All;\
                              GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;\
                              GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;\
                              GPIO_Init(PLCPORT, &GPIO_InitStructure)


//��ֱ�Ӷ˿�PLCPORT��PROTNUM���ܽ����ó�����
#define CfgInputPortBit(PLCPORT,PORTNUM)    GPIO_InitStructure.GPIO_Pin = PORTNUM;\
                                          GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;\
                                          GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;\
                                            GPIO_Init(PLCPORT, &GPIO_InitStructure) 
//��ֱ�Ӷ˿�PLCPORT��PROTNUM���ܽ����ó����
#define CfgOutputPortBit(PLCPORT,PORTNUM) GPIO_InitStructure.GPIO_Pin = PORTNUM;\
                              GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;\
                              GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;\
                              GPIO_Init(PLCPORT, &GPIO_InitStructure)

//��ֱ��CPU�˿�PLCPORTֵ���뵽�ڴ浥ԪMemData��
#define ReadSingleDirectPort(MemData,PLCPORT)  MemData = GPIO_ReadInputData(PLCPORT)   
//��ֵMemDataд�뵽ֱ�Ӷ˿�PLCPORT��
#define WriteSingleDirectPort(MemData,PLCPORT) GPIO_SetBits(PLCPORT,MemData);\
                                         GPIO_ResetBits(PLCPORT,~MemData)

//��ֱ�Ӷ˿�PLCPORT��PORTNUM�ܽŵ�ֵ���뵽MemData��
#define ReadSingleDirectPortBit(MemData,PLCPORT,PORTNUM) MemData = GPIO_ReadInputDataBit(PLCPORT,PORTNUM) 
//��ֵMemDataд�뵽ֱ�Ӷ˿�PLCPORT��PORTNUM�ܽ���
#define WriteSingleDirectPortBit(MemData,PLCPORT,PORTNUM) if(MemData != 0) \
							GPIO_SetBits(PLCPORT,PORTNUM);\
							else \
							GPIO_ResetBits(PLCPORT,PORTNUM);\

#if defined SMALLMODE            //�˴���дС������CASS�˿ڲ������� 
//��CASS�˿�PLCPORT��ֵ���뵽MemData��  
#define ReadSinglePort(MemData,PLCPORT)  MemData = GPIO_ReadInputData(PLCPORT)   
//��ֵMemDataд�뵽CASS�˿�PLCPORT��
#define WriteSinglePort(MemData,PLCPORT) GPIO_SetBits(PLCPORT,MemData);\
                                         GPIO_ResetBits(PLCPORT,~MemData)
              
#elif defined MIDDLEMODE         //�˴���дС������CASS�˿ڲ������� 
#define ReadSinglePort(MemData,PLCPORT)    if(PLCPORT & 1) \
												PLCCODELINEA##0 |= (1 << PLCNUMCODELINEA); \
										   else \
												PLCCODELINEA##1 |= (1 << PLCNUMCODELINEA); \
										   if(PLCPORT & (1 << 1)) \
												PLCCODELINEB##0 |= (1 << PLCNUMCODELINEB); \
										   else \
												PLCCODELINEB##1 |= (1 << PLCNUMCODELINEB); \
											if(PLCPORT & (1 << 2)) \
												PLCCODELINEC##0 |= (1 << PLCNUMCODELINEC); \
										   else \
												PLCCODELINEC##1 |= (1 << PLCNUMCODELINEC); \
										   PLCPULSE1 |= (1 << PLCNUMPULSE); \
										   MemData = ~PLCINPUTPORT;	
										          
//��ֵMemDataд�뵽PLC�˿�PLCPORT��
#define WriteSinglePort(MemData,PLCPORT)   	PLCOUTPUTPORT##0 = ~MemData; \
											PLCOUTPUTPORT##1 = MemData; \
										    if(PLCPORT & 1) \
												PLCCODELINEA##0 |= (1 << PLCNUMCODELINEA); \
										    else \
												PLCCODELINEA##1 |= (1 << PLCNUMCODELINEA); \
										    if(PLCPORT & (1 << 1)) \
												PLCCODELINEB##0 |= (1 << PLCNUMCODELINEB); \
										    else \
												PLCCODELINEB##1 |= (1 << PLCNUMCODELINEB); \
											if(PLCPORT & (1 << 2)) \
												PLCCODELINEC##0 |= (1 << PLCNUMCODELINEC); \
											else \
												PLCCODELINEC##1 |= (1 << PLCNUMCODELINEC); \
											PLCPULSE1 |= (1 << PLCNUMPULSE); \
											PLCPULSE1 |= (1 << PLCNUMPULSE); \
											PLCPULSE0 |= (1 << PLCNUMPULSE); 
											


#elif defined BIGMODE            //�˴���дС������CASS�˿ڲ������� 
//��CASS�˿�PLCPORT��ֵ���뵽MemData��  
#define ReadSinglePort(MemData,PLCPORT)     
//��ֵMemDataд�뵽CASS�˿�PLCPORT��
#define WriteSinglePort(MemData,PLCPORT)  
#endif

//IIC��,����ݲ�ͬCPU���Զ������޸�
#define IIC0Start()  I2CONCLR |= 0x1C;I2CONSET = 0x60//����START�ź�
#define IIC0Stop()   I2CONCLR |= 0x1C;I2CONSET = 0x50//����STOP�ź�
#define IIC0SndByt(x) I2DAT = x; I2CONCLR |= 0x20//����һ���ֽڵ�����

//...�����г�ÿ��IIC�ں궨��ʹ����������

//ʵʱʱ��оƬ��д��ַ
#define PCF8563R 0xA3    //��8563�ĵ�ַ
#define PCF8563W 0xA2    //д8563�ĵ�ַ

//IIC״̬�붨��
#define TW_START		0x08
#define TW_REP_START		0x10
/* Master Transmitter */
#define TW_MT_SLA_ACK		0x18
#define TW_MT_SLA_NACK		0x20
#define TW_MT_DATA_ACK		0x28
#define TW_MT_DATA_NACK		0x30
#define TW_MT_ARB_LOST		0x38
/* Master Receiver */
#define TW_MR_ARB_LOST		0x38
#define TW_MR_SLA_ACK		0x40
#define TW_MR_SLA_NACK		0x48
#define TW_MR_DATA_ACK		0x50
#define TW_MR_DATA_NACK		0x58
/* Slave Transmitter */
#define TW_ST_SLA_ACK		0xA8
#define TW_ST_ARB_LOST_SLA_ACK	0xB0
#define TW_ST_DATA_ACK		0xB8
#define TW_ST_DATA_NACK		0xC0
#define TW_ST_LAST_DATA		0xC8
/* Slave Receiver */
#define TW_SR_SLA_ACK		0x60
#define TW_SR_ARB_LOST_SLA_ACK	0x68
#define TW_SR_GCALL_ACK		0x70
#define TW_SR_ARB_LOST_GCALL_ACK 0x78
#define TW_SR_DATA_ACK		0x80
#define TW_SR_DATA_NACK		0x88
#define TW_SR_GCALL_DATA_ACK	0x90
#define TW_SR_GCALL_DATA_NACK	0x98
#define TW_SR_STOP		0xA0
/* Misc */
#define TW_NO_INFO		0xF8
#define TW_BUS_ERROR		0x00

EXTERN_VAR uint8 PIDTempArray[8]; //PIDָ���ʱ���õ�����ʱ�洢��



EXTERN_VAR uint8 StepFlag;
EXTERN_VAR uint16 StepLen;    //����
EXTERN_VAR uint8 FirstDebug;
EXTERN_VAR uint32 PointIndex[100]; //����ͼ����
EXTERN_VAR uint8 PointLen;
EXTERN_VAR uint32 AimIndex;
EXTERN_VAR uint32 CurIndex;
EXTERN_VAR uint8 StopDebug;
//EXTERN_VAR uint8 ComOutInfo[1000];
//EXTERN_VAR uint16 ComOutLen;
//EXTERN_VAR uint8 ComInInfo[100];
//EXTERN_VAR uint16 ComInLen;
EXTERN_VAR uint8 PlcFlag;//1���٣�2����
EXTERN_VAR uint32 tempDebug;
EXTERN_VAR uint32 NumDebug;
EXTERN_VAR uint8 RSTFlag;//��λ��־

EXTERN_VAR pFunction FirstProgramSub ;
EXTERN_VAR pFunction SecondProgramSub ;
EXTERN_VAR pFunction Time1ms_Sub ;
EXTERN_VAR pFunction Time10ms_Sub ;
EXTERN_VAR pFunction Time100ms_Sub ;
EXTERN_VAR pFunction Time1000ms_Sub ;
EXTERN_VAR pFunction InitAddr ;
EXTERN_VAR pFunction FraimProcessEMS ;
extern uint32 FirstProgram;
extern uint32 SecondProgram;
extern uint32 Time1MS;
extern uint32 Time10MS;
extern uint32 Time100MS;
extern uint32 Time1000MS;
extern uint32 FraimProcessAddr;
extern uint32 SendInfoAddr;
extern uint32 WritePortAddr;
extern uint32 ComProAddr;
extern uint8 ComOutInfo[71];
extern uint8 ComInInfo[71];
extern uint8 ComInLen;
extern uint8 ComOutLen;
extern uint8 ModeFlag;

extern uint32 DCSProgram;
extern uint32 ADConvertAddr;
extern uint32 DAConvertAddr;
extern uint32 DCS_TimerCount;
extern uint32 ConfigurationInitAddr; 


EXTERN_VAR pFunction DCSProgramSub ;
EXTERN_VAR pFunction Configuration_init ;

#define M_T2_PWM
#define TIM3CLKHZ 72000000
#endif
