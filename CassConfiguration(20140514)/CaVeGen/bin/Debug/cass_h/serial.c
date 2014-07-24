#include <windows.h>
#include <stdio.h>
#include <stdlib.h>

//设备输入——串口参数
char DevName[10]="COM1";
char *cass_mv_cameraIn_serialportname = DevName ; //串口号
int cass_mv_cameraIn_serialBaudrate =115200;      //波特率
uint8 cass_mv_cameraIn_serialParity =NOPARITY;  ;       //奇偶校验
uint8 cass_mv_cameraIn_serialStopbits = ONESTOPBIT;       //停止位
uint8 cass_mv_cameraIn_serialDatabits = 8;  //数据位
HANDLE comPort = NULL;

int SeiralPortOpen(const char *portName,int baudrate, unsigned char stopbits, unsigned char parity)
{
   // instance an object of COMMTIMEOUTS.
    COMMTIMEOUTS comTimeOut;     
    DCB dcb;

    comPort = CreateFile(portName,      // Specify port device: default "COM1"
    GENERIC_READ | GENERIC_WRITE,       // Specify mode that open device.
    0,                                  // the devide isn't shared.
    NULL,                               // the object gets a default security.
    OPEN_EXISTING,                      // Specify which action to take on file. 
    0,                                  // default.
    NULL);                              // default.
  
    if (GetCommState(comPort,&dcb) == 0)
    {
      return -1;
    }
    dcb.BaudRate = baudrate;    // Specify buad rate of communicaiton.
    dcb.StopBits = stopbits;    // Specify stopbit of communication.
    dcb.Parity = parity;        // Specify parity of communication.
    dcb.fParity = 0; 
    dcb.fOutxDsrFlow = 0;
    dcb.fBinary = 1;
    dcb.fDtrControl = DTR_CONTROL_DISABLE;
    dcb.fRtsControl = RTS_CONTROL_DISABLE;
    dcb.fOutX = 0;
    dcb.fInX = 0;
    dcb.DCBlength = sizeof(dcb);
    dcb.ByteSize = 8;    // Specify  byte of size of communication.

    // Set current configuration of serial communication port.
    if (SetCommState(comPort,&dcb) == 0)
    {
       return -1;
    }
    memset(&comTimeOut,0,sizeof(comTimeOut));       
    // Specify time-out between charactor for receiving.
    comTimeOut.ReadIntervalTimeout = 1000;
    // Specify value that is multiplied 
    // by the requested number of bytes to be read. 
    comTimeOut.ReadTotalTimeoutMultiplier = 0;
    // Specify value is added to the product of the 
    // ReadTotalTimeoutMultiplier member
    comTimeOut.ReadTotalTimeoutConstant = 0;
    // Specify value that is multiplied 
    // by the requested number of bytes to be sent. 
    comTimeOut.WriteTotalTimeoutMultiplier = 3;
    // Specify value is added to the product of the 
    // WriteTotalTimeoutMultiplier member
    comTimeOut.WriteTotalTimeoutConstant = 0;
    // set the time-out parameter into device control.
   SetCommTimeouts(comPort,&comTimeOut);
   //memset(&comTimeOut,0,sizeof(comTimeOut));   
   //printf("%d:%d:%d\n",comTimeOut.ReadIntervalTimeout,comTimeOut.ReadTotalTimeoutMultiplier ,comTimeOut.ReadTotalTimeoutConstant);
   //GetCommTimeouts(comPort,&comTimeOut);
   //printf("%d:%d:%d\n",comTimeOut.ReadIntervalTimeout,comTimeOut.ReadTotalTimeoutMultiplier ,comTimeOut.ReadTotalTimeoutConstant);
    // Updata port's status.
    SetupComm(comPort,4096,4096);
   
    return 0;

}

int read_scc(char* inputData,
                    unsigned int sizeBuffer,
                    unsigned long* length)
{
   int dwErrorFlags;
   COMSTAT ComStat;
   ClearCommError(comPort,&dwErrorFlags,&ComStat);
  if (ReadFile(comPort,  // handle of file to read
    inputData,               // handle of file to read
    sizeBuffer,              // number of bytes to read
    length,                 // pointer to number of bytes read
    NULL) == 0)              // pointer to structure for data
  {
    return -1;
  }
  PurgeComm(comPort,PURGE_RXCLEAR|PURGE_TXCLEAR|PURGE_RXABORT|PURGE_TXABORT);
  return (int)length;
}


int write_scc(LPCVOID outputData,
                     unsigned int sizeBuffer,
                     unsigned long* length)
{
  if( sizeBuffer> 0)
  {
  
    if (WriteFile(comPort, 	// handle to file to write to
      outputData,              // pointer to data to write to file
      sizeBuffer,              // number of bytes to write
      length,NULL) == 0)      // pointer to number of bytes written
    {
      return -1;
    }
  }
  return (int)length;
}

void SerialPortClose()
{
    CloseHandle(comPort);
}
