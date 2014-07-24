/////////////////////////////////
// ������
////////////////////////////////
#ifndef _CONFIGURATION_MYQUEUE_H
#define _CONFIGURATION_MYQUEUE_H

#include "CassType.h"

typedef struct QNode
{
	fp32 fData;
	struct QNode *next;
}QNode,*QueuePtr;

typedef struct
{
	QueuePtr front;  // ��ָ��
	QueuePtr rear;   // βָ��
}LinkQueue;

void InitQueue(LinkQueue* *a);
void DestroyQueue(LinkQueue *);
void EnQueue(LinkQueue* *,fp32);
void DeQueue(LinkQueue* *,fp32 *);
int QueueLength(LinkQueue *);
char QueueEmpty(LinkQueue );		    
float GetQueueElement(LinkQueue *);	  

#endif

