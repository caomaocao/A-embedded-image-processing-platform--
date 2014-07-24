using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace CaVeGen.CommonOperation
{
    public sealed  class FileOperator
    {
        public static ArrayList ReadFromFile(string FilePath)
        {
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

        public static ArrayList ReadFromFile(string FilePath, Encoding encoding)
        {
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
    }
}
