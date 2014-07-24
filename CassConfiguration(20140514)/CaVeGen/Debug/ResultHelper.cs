using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using CaVeGen.CommonOperation;

namespace CaVeGen.Debug
{
    //数据类型 —— 枚举
    public enum DataType
    {
        BOOL,
        NUMBER,
        INT,
        FLOAT
    }

    //public struct BoolInfo
    //{
    //    public string TargetValue;
    //    public string ActualValue;
    //    public bool Result;

    //}

    //public struct NumberInfo
    //{
    //    public string Result;

    //}


    //public struct IntInfo
    //{

    //}

    //public struct FloatInfo
    //{

    //}
    public struct ResultInfo
    {
        public string DataType;
        public string Result;
    }
   
    public class ResultHelper
    {

        private const string _ResultFile = "Result.txt";

        private string _Path = null;

        public ResultHelper(string path)
        {
            this._Path = Path.Combine(path,_ResultFile);
        }
        /// <summary>
        /// 读取结果
        /// 文件结构
        /// 每一行：数据类型，结果
        /// 
        /// </summary>
        /// <param name="isLoopDebug"></param>
        /// <returns></returns>
        public List<ResultInfo> ReadFromFile2(bool isLoopDebug)
        {
            List<ResultInfo> resultList = new List<ResultInfo>();
            try
            {
                ArrayList contentList = FileOperator.ReadFromFile(_Path);
             
                foreach (string content in contentList)
                {

                    if (content != null)
                    {

                        string[] resMatch = content.Split(',');

                        if (resMatch.Length >= 2)
                        {
                            ResultInfo result = new ResultInfo();
                            result.DataType = resMatch[0];
                            result.Result = resMatch[1];
                            resultList.Add(result);
                        }

                    }
                }
            }
            catch
            {

            }
            finally
            {
                if (isLoopDebug)
                {
                    File.Delete(_Path);
                }
            }
            return resultList;
        }

        /// <summary>
        /// 读取匹配结果
        /// 文件结构
        /// 数据类型
        /// 结果
        /// </summary>
        /// <param name="isLoopDebug"></param>
        /// <returns></returns>
        public ResultInfo ReadFromFile(bool isLoopDebug)
        {
            ResultInfo result = new ResultInfo();
            try
            {
                string content = FileOperator.ReadFromFile2(_Path);
               
                if (content != null)
                {

                    string[] resMatch = content.Split('\n');

                    if (resMatch.Length >= 2)
                    {

                        result.DataType = resMatch[0];
                        result.Result = resMatch[1];

                    }

                    //   this.picWin.setMatch(curPercent);
                    //   this.picWin.ShowCounter();

                }
            }
            catch
            {

            }
            finally
            {
                 if (isLoopDebug)
                    {
                        File.Delete(_Path);
                    }
            }
            return result;
        }
    }


}
