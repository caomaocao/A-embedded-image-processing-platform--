////////////////////////////////////////////////////////////////////////////////////////////
// ��   ��   ��  : CassSysVar.h
// ��        ��  :                            
// Ŀ�ļ���Ҫ����: ���ϵͳ�������͵�������Ŀ����Ϊ���������������޹���
// �� �� ��  ��  : 2008.3.5
// �� �� ��  ��  : 2008.3.5                    
// ��        ��  : hfl
// ��   ��   ��  :                      
////////////////////////////////////////////////////////////////////////////////////////////

#ifndef CASSTYPE_H
#define CASSTYPE_H

typedef unsigned char uint8;
typedef unsigned int uint16;
typedef unsigned long uint32;
typedef signed char int8;
typedef signed int int16;
typedef signed long int32;
typedef float fp32;
typedef double fp64;
typedef uint8 (*pFunction)(void) ;
typedef uint8 (*pFun)(uint32) ;
typedef uint8 (*ppFun)(uint8,uint32) ;
typedef uint8 (*pppFun)(uint32*) ;

#endif
