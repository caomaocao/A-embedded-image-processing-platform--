/*******************************************************************************
           ** Copyright (C) 2007 CASS 版权所有
           ** 文件名：StringOperateHelp 
           ** 功能描述：
           **          字符串操作帮助
           ** 作者：雨晨
           ** 创始时间：2007-7-6
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace CaVeGen.CommonOperation
{
    public static class StringOperateHelp
    {
        /// <summary>
        /// 取出字符串中字符c 第1次 出现位置左边的子字符串
        /// </summary>
        /// <param name="src">字符串</param>
        /// <param name="c">找寻的字符</param>
        /// <returns>子字符串</returns>
        public static string LeftOf(string src, char c)
        {
            int idx = src.IndexOf(c);
            if (idx == -1)
            {
                return "";
            }
            return src.Substring(0, idx);
        }

        /// <summary>
        /// 取出字符串中字符c 第n次 出现位置左边的子字符串
        /// </summary>
        /// <param name="src">字符串</param>
        /// <param name="c">找寻的字符</param>
        /// <param name="n">次数</param>
        /// <returns>子字符串</returns>
        public static string LeftOf(string src, char c, int n)
        {
            int idx = -1;
            while (n != 0)
            {
                idx = src.IndexOf(c, idx + 1);
                if (idx == -1)
                {
                    return "";
                }
                --n;
            }
            return src.Substring(0, idx);
        }

        /// <summary>
        /// 取出字符串中字符c 第1次 出现位置右边的子字符串
        /// </summary>
        /// <param name="src">字符串</param>
        /// <param name="c">找寻的字符</param>
        /// <returns>子字符串</returns>
        public static string RightOf(string src, char c)
        {
            int idx = src.IndexOf(c);
            if (idx == -1)
            {
                return src;
            }

            return src.Substring(idx + 1);
        }

        /// <summary>
        /// 取出字符串中字符c 第n次 出现位置右边的子字符串
        /// </summary>
        /// <param name="src">字符串</param>
        /// <param name="c">找寻的字符</param>
        /// <param name="n">次数</param>
        /// <returns>子字符串</returns>
        public static string RightOf(string src, char c, int n)
        {
            int idx = -1;
            while (n != 0)
            {
                idx = src.IndexOf(c, idx + 1);
                if (idx == -1)
                {
                    return src;
                }
                --n;
            }

            return src.Substring(idx + 1);
        }

        /// <summary>
        /// 取出字符串中最右一个 字符c出现位置 左边的子字符串
        /// </summary>
        /// <param name="src">字符串</param>
        /// <param name="c">找寻的字符</param>
        /// <returns>子字符串</returns>
        public static string LeftOfRightmostOf(string src, char c)
        {
            int idx = src.LastIndexOf(c);
            if (idx == -1)
            {
                return src;
            }
            return src.Substring(0, idx);
        }

        /// <summary>
        /// 取出字符串中最右一个 字符c出现位置 右边的子字符串
        /// </summary>
        /// <param name="src">字符串</param>
        /// <param name="c">找寻的字符</param>
        /// <returns>子字符串</returns>
        public static string RightOfRightmostOf(string src, char c)
        {
            int idx = src.LastIndexOf(c);
            if (idx == -1)
            {
                return src;
            }
            return src.Substring(idx + 1);
        }

        /// <summary>
        /// 取出 字符start和字符end 左数第一次出现位置 中间的字符串(保证end必须在start右边)
        /// </summary>
        /// <param name="src">字符串</param>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns></returns>
        public static string Between(string src, char start, char end)
        {
            string res = String.Empty;
            int idxStart = src.IndexOf(start);
            if (idxStart != -1)
            {
                ++idxStart;
                int idxEnd = src.IndexOf(end, idxStart);
                if (idxEnd != -1)
                {
                    res = src.Substring(idxStart, idxEnd - idxStart);
                }
            }
            return res;
        }

        /// <summary>
        /// 返回find在字符串src中出现次数
        /// </summary>
        /// <param name="src">字符串</param>
        /// <param name="find">查找的字符</param>
        /// <returns>次数</returns>
        public static int Count(string src, char find)
        {
            int ret = 0;
            foreach (char s in src)
            {
                if (s == find)
                {
                    ++ret;
                }
            }
            return ret;
        }
    }
}
