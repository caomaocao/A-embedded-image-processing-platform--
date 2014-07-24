using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using CaVeGen.CommonOperation;
namespace CaVeGen.MachineVision
{
    class AnalyseFeatherHeaderClass
    {
        private const string FeatherName = "\\feather.h";
        private const string FeatureAreaName = "\\featureArea.h";

        //读取图片坐标信息，生成featureArea.h
        public  string AnalyseFeatherHeaderFile(string savePath, string inputFilePath)
        {
            try
            {
                if (!File.Exists(savePath + FeatherName))
                {
                    // CassMessageBox.Information("当前工程不存在" + FeatherName + "文件！");
                    return "当前工程不存在" + FeatherName + "文件！";
                }

                //读取feather.h文件
                string fileContent = FileOperator.ReadFromFile2(savePath + FeatherName);

                string writeContent = null;

                if (fileContent == null)//当feather.h文件为空时，默认读取全图尺寸
                {
                    writeContent = GenerateDefaltFeatureAreaFile(inputFilePath);
                    CassMessageBox.Information("当前为默认状态，全图操作。");
                }
                else

                    writeContent = AnalyseFeatherContent(fileContent);

                FileOperator.WriteToFile(savePath + FeatureAreaName, writeContent, Encoding.UTF8);

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return null;

        }

        /// <summary>
        /// feather.h 文件为空时，默认生成全图的区域数组
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        private  string GenerateDefaltFeatureAreaFile(string inputFilePath)
        {
            string result = "space_number_info  local_area_array[0] = {0}";
            try
            {
                //默认生成全图
                FileStream fs = File.OpenRead(inputFilePath);
                int filelength = 0;
                filelength = (int)fs.Length; //获得文件长度 
                Byte[] imageByte = new Byte[filelength]; //建立一个字节数组 
                fs.Read(imageByte, 0, filelength); //按字节流读取
                Image image = Image.FromStream(fs);
                string width = image.Width.ToString();
                string length = image.Height.ToString();
                fs.Close();
                result = "space_number_info  local_area_array[1] = {0,0," + width + "," + length + " };";
            }
            catch (Exception)
            {

            }
            finally
            {

            }
            return result;
        }

        /// <summary>
        /// feather.h 文件不为空，解析feather.h内容 
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        private  string AnalyseFeatherContent(string fileContent)
        {
            string result = null;
            //处理
            //读取第一个大括号
            int startIndex = fileContent.IndexOf('{');

            string dataStr = fileContent.Substring(startIndex, fileContent.Length - startIndex);

            string[] dataArray = dataStr.Split(',').ToArray();
            //计算有几个待识别的区域

            if (dataArray.Length != 12054)
            {
                // CassMessageBox.Error("数组个数不足3054个");
                return "数组个数不足12054个";
            }
            List<AreaInfo> areaInfoList = new List<AreaInfo>();

            //前10个数据（1个版本信息位 + 1个是否灰度位 +8 个镜头参数）过滤，
            //第11到第14个数据是大区域信息，第15个开始，，有10组小区域信息，每304 个数据，前4个区域信息，后300个特征点
            int index = 14;
            for (int i = 0; i < 10; i++)   //默认十个区域，不足的填充0
            {
                AreaInfo areaInfo;

                areaInfo.Leftbound = Convert.ToUInt16(dataArray[index]);
                areaInfo.Topbound = Convert.ToUInt16(dataArray[index + 1]);
                areaInfo.Width = Convert.ToUInt16(dataArray[index + 2]);
                areaInfo.Height = Convert.ToUInt16(dataArray[index + 3]);

                if (areaInfo.Leftbound + areaInfo.Topbound + areaInfo.Width + areaInfo.Height != 0)
                    areaInfoList.Add(areaInfo);

                index += 300 * 4 + 4;
            }

            //生成featureArea.h
            result = "space_number_info  local_area_array[" + areaInfoList.Count + "]={";
            foreach (AreaInfo temp in areaInfoList)
            {
                //if (temp.Leftbound + temp.Topbound + temp.Width + temp.Height == 0)
                //    break;
                result += temp.Leftbound + "," + temp.Topbound + "," + temp.Width + "," + temp.Height + ",";
            }

            result = result.TrimEnd(',');
            result += "};";

            return result;
        }
    }
}
