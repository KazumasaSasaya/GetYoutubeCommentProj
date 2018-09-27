using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace Ichikara.YoutubeComment
{
    /// <summary>
    /// Json string formatter.
    /// </summary>
    public static class StringFormatter
    {
        /// <summary>
        /// 第二引数の文字列から第一引数にある文字列を削除
        /// </summary>
        /// <param name="delete">消したい文字列</param>
        /// <param name="data">整形したいデータ</param>
        /// <returns>整形データ</returns>
        public static string GetFormattedString(string[] delete, string data)
        {
            string ret = data;
            foreach (string del in delete)
            {
                ret = ret.Replace(del, String.Empty);
            }
            return ret;
        }



        private const string LOG_URL = "./LogData.txt";
        /// <summary>
        /// テキスト出力
        /// PCdebugのみ動作可能
        /// プロジェクトフォルダ直下に生成
        /// </summary>
        /// <param name="txt">Text.</param>
        public static void TextOutput(string txt)
        {
#if DEBUG
            StreamWriter sw = new StreamWriter(LOG_URL, true);//ファイル内容の末尾に追記
            sw.WriteLine(txt);
            sw.Flush();
            sw.Close();
#endif
        }

        /// <summary>
        /// テキスト出力されたログをリセット
        /// PCdebugのみ動作可能
        /// </summary>
        public static void TextReset()
        {
#if DEBUG
            StreamWriter sw = new StreamWriter(LOG_URL, false);//ファイル内容を上書き
            sw.WriteLine("");
            sw.Flush();
            sw.Close();
#endif
        }
    }
}