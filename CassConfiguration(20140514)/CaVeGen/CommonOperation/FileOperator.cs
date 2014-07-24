using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace CaVeGen.CommonOperation
{
    public sealed  class FileOperator
    {
       
        public static ArrayList ReadFromFile(string FilePath)
        {
            if (!File.Exists(FilePath))
                return null;
            ArrayList arrReturnValue = new ArrayList();
            string lineData = null;
            FileStream fileStream = new FileStream(FilePath, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            while ((lineData = streamReader.ReadLine()) != null)
            {
                arrReturnValue.Add(lineData);
            }
            streamReader.Close();
            fileStream.Close();
            streamReader.Dispose();
            fileStream.Dispose();
            return arrReturnValue;
        }

        /// <summary>
        /// 从指定文件路径读取内容
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns>字符串</returns>
        public static string ReadFromFile2(string FilePath)
        {
            string resultStr = null;
            string lineData = null;
            if (!File.Exists(FilePath))
                return resultStr;
            FileStream fileStream = new FileStream(FilePath,FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            while ((lineData = streamReader.ReadLine()) != null)
            {
                resultStr += lineData + "\n";
            }
            streamReader.Close();
            fileStream.Close();
            streamReader.Dispose();
            fileStream.Dispose();
            return resultStr;
            
        }
        /// <summary>
        /// 从指定文件路径读取内容
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns>字符串</returns>
        public static string ReadFromFile2(string FilePath, Encoding encoding)
        {
            string resultStr = null;
            string lineData = null;
            if (!File.Exists(FilePath))
                return resultStr;
            FileStream fileStream = new FileStream(FilePath, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream, encoding);
            while ((lineData = streamReader.ReadLine()) != null)
            {
                resultStr += lineData + "\n";
            }
            streamReader.Close();
            fileStream.Close();
            streamReader.Dispose();
            fileStream.Dispose();
            return resultStr;

        }

        public static ArrayList ReadFromFile(string FilePath, Encoding encoding)
        {
            if (!File.Exists(FilePath))
                return null;
            ArrayList arrReturnValue = new ArrayList();
            string lineData = null;
            FileStream fileStream = new FileStream(FilePath, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream, encoding);
            while ((lineData = streamReader.ReadLine()) != null)
            {
                arrReturnValue.Add(lineData);
            }
            streamReader.Close();
            fileStream.Close();
            streamReader.Dispose();
            fileStream.Dispose();
            return arrReturnValue;
        }

        public static void WriteToFile(string FilePath, ArrayList arrWriteValue)
        {
            try
            {
                FileStream fileStream = null;
                if (File.Exists(FilePath))
                {
                    fileStream = new FileStream(FilePath, FileMode.Truncate);
                }
                else
                {
                    fileStream = new FileStream(FilePath, FileMode.CreateNew);
                }
                StreamWriter streamWriter = new StreamWriter(fileStream);
                foreach (string lineData in arrWriteValue)
                {
                    streamWriter.WriteLine(lineData);
                }
                streamWriter.Close();
                fileStream.Close();
                streamWriter.Dispose();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void WriteToFile(string FilePath, ArrayList arrWriteValue, Encoding encoding)
        {
            try
            {
                FileStream fileStream;
                if (File.Exists(FilePath))
                {
                    fileStream = new FileStream(FilePath, FileMode.Truncate);
                }
                else
                {
                    fileStream = new FileStream(FilePath, FileMode.CreateNew);
                }
                StreamWriter streamWriter = new StreamWriter(fileStream, encoding);
                foreach (string lineData in arrWriteValue)
                {
                    streamWriter.WriteLine(lineData);
                }
                streamWriter.Close();
                fileStream.Close();
                streamWriter.Dispose();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void WriteToFile(string FilePath, string strWriteValue, Encoding encoding)
        {
            try
            {
                FileStream fileStream = null;
                if (File.Exists(FilePath))
                {
                    fileStream = new FileStream(FilePath, FileMode.Truncate);
                }
                else
                {
                    fileStream = new FileStream(FilePath, FileMode.CreateNew);
                }
                StreamWriter streamWriter = new StreamWriter(fileStream,encoding);

                streamWriter.WriteLine(strWriteValue);
                
                streamWriter.Close();
                fileStream.Close();
                streamWriter.Dispose();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void AppendToFile(string FilePath, ArrayList arrWriteValue)
        {
            try
            {
                FileStream fileStream = null;
                if (File.Exists(FilePath))
                {
                    fileStream = new FileStream(FilePath, FileMode.Append);
                }
                else
                {
                    fileStream = new FileStream(FilePath, FileMode.CreateNew);
                }
                StreamWriter streamWriter = new StreamWriter(fileStream);
                foreach (string lineData in arrWriteValue)
                {
                    streamWriter.WriteLine(lineData);
                }
                streamWriter.Close();
                fileStream.Close();
                streamWriter.Dispose();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void AppendToFile(string FilePath, ArrayList arrWriteValue, Encoding encoding)
        {
            try
            {
                FileStream fileStream = null;
                if (File.Exists(FilePath))
                {
                    fileStream = new FileStream(FilePath, FileMode.Append);
                }
                else
                {
                    fileStream = new FileStream(FilePath, FileMode.CreateNew);
                }
                StreamWriter streamWriter = new StreamWriter(fileStream, encoding);
                foreach (string lineData in arrWriteValue)
                {
                    streamWriter.WriteLine(lineData);
                }
                streamWriter.Close();
                fileStream.Close();
                streamWriter.Dispose();
                fileStream.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 删除指定目录下的所有指定类型文件（不支持删除子目录中的文件）
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="fileType"></param>
        public static void DeleteFiles(string dirPath, string fileType)
        {
           
            DirectoryInfo di = new DirectoryInfo(dirPath);
            if (di.Exists)
            {
                FileInfo[] fi = di.GetFiles();
                foreach (FileInfo temp in fi)
                {
                    if (temp.Extension == fileType)
                    {
                        File.Delete(dirPath + "\\" + temp);
                    }

                }
            }
        }


        public static void DeleteFile(string filePath)
        {
            try
            {

            }
            catch
            {
                //有其他进程调用此文件
                foreach (var process in Process.GetProcesses())
                {
                    var files = GetFilesLockedBy(process);
                   // if (files.Contains(filePath))
                    //{
                        
                        
                    //}
                }

            }
        }

        private static object GetFilesLockedBy(Process process)
        {
            throw new NotImplementedException();
        }
    
    }
}
