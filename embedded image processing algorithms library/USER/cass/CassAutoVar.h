#ifndef CASSAUTOVAR_H
#define CASSAUTOVAR_H
#include "CassType.h"
#ifdef  _GLOBAL_VAR_DEFINE_
#define  EXTERN_VAR
#else
#define  EXTERN_VAR    extern
#endif
#define LocalAddr 1
#define F_CPU 7372800
#define M_UART2
#define UART2BAUT 115200
#define IN_BUFLEN_MAX_UART2 71
#define OUT_BUFLEN_MAX_UART2 71
#define EngineFirstTime 1
#define EngineSecondTime 2
#define EngineDcsTime 1000
#define CPUDATA32

#define M_T0_INT
#define M_T0_1MS
#define XStart 0 //��������ʼƫ��
#define XEnd 3 //����������ƫ��
EXTERN_VAR uint32 OldValue[1];//�˿���ʱ���ݴ����
#define YStart 8 //�������ʼƫ��
#define YEnd 11 //���������ƫ��
#define MStart 24 //M����ʼƫ��
#define MEnd 73 //M������ƫ��
#define SStart 224 //S����ʼƫ��
#define SEnd 224 //S������ƫ��
#define TStart 226 //T����ʼƫ��
#define TEnd 233 //T������ƫ��
#define TCurStart 258 //T����ǰֵ��ַ��ʼƫ��
#define TCurEnd 385 //T����ǰֵ��ַ����ƫ��
#define TSetStart 386 //T���趨ֵ��ַ��ʼƫ��
#define TSetEnd 513 //T���趨ֵ��ַ����ƫ��
#define CStart 514 //C����ʼƫ��
#define CEnd 514 //C������ƫ��
#define CCurStart 520 //C����ǰֵ��ַ��ʼƫ��
#define CCurEnd 535 //C����ǰֵ��ַ����ƫ��
#define CSetStart 536 //C���趨ֵ��ַ��ʼƫ��
#define CSetEnd 551 //C���趨ֵ��ַ����ƫ��
#define CDStart 552 //CD����ʼƫ��
#define CDEnd 552 //CD������ƫ��
#define CDCurStart 560 //C����ǰֵ��ַ��ʼƫ��
#define CDCurEnd 591 //C����ǰֵ��ַ����ƫ��
#define CDSetStart 592 //C���趨ֵ��ַ��ʼƫ��
#define CDSetEnd 623 //C���趨ֵ��ַ����ƫ��
#define CQStart 624 //CQ����ʼƫ��
#define CQEnd 623 //CQ������ƫ��
#define CQCurStart 624 //CQ����ǰֵ��ַ��ʼƫ��
#define CQCurEnd 624 //CQ����ǰֵ��ַ����ƫ��
#define CQSetStart 624 //CQ���趨ֵ��ַ��ʼƫ��
#define CQSetEnd 623 //CQ���趨ֵ��ַ����ƫ��
#define TCQStart 624 //TCQ����ʼƫ��
#define TCQEnd 623 //TCQ������ƫ��
#define TCQCurStart 624 //TCQ����ǰֵ��ַ��ʼƫ��
#define TCQCurEnd 623 //TCQ����ǰֵ��ַ����ƫ��
#define TCQSetStart 624 //TCQ���趨ֵ��ַ��ʼƫ��
#define TCQSetEnd 623 //TCQ���趨ֵ��ַ����ƫ��
#define DStart 624 //D����ʼƫ��
#define DEnd 1823 //D������ƫ��
#define CASSVALUEF 1824
//extern uint8 CassMem[1828];

#define SMALLMODE
#define CPUDATALEN 32


//���¶�����Ϊ�˸��ټ�������ʹ��
#define VALUEY0 8
#define VALUEY1 9
#define VALUEY2 10
#define VALUEY3 11

//Define C Function Flag 
#endif

