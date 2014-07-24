using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using CaVeGen.CommonOperation;
using System.Collections;
namespace CaVeGen.MachineVision
{
    //结构体定义

    //区域信息
    struct AreaInfo
    {
        public UInt16 Leftbound;  //左上角宽度坐标
        public UInt16 Topbound;   //左上角长度坐标
        public UInt16 Width;      //区域的宽度
        public UInt16 Height;    //区域的长度
    }
 

    public class MachineVisionMethod
    {
        private static AnalyseFeatherHeaderClass analyseFeatherHeaderClass = new AnalyseFeatherHeaderClass();

        public static void AnalyseFeatherHeaderHeaderFile(string projectPath,string inputFilePath)
        {
            analyseFeatherHeaderClass.AnalyseFeatherHeaderFile(projectPath, inputFilePath);
        }


      
    }
}
