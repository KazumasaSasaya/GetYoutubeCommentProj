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
        /// Jsonファイルのネスト情報を取り出す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object FromJson<T>(string json)
        {
            if (json.StartsWith("["))
            {
                json = @"{""Items"":" + json + "}";
                var obj = JsonUtility.FromJson<Wrapper<T>>(json);
                return (T[])(object)obj.Items;
            }
            else
            {
                T obj = JsonUtility.FromJson<T>(json);
                return json;
            }
        }

        /// <summary>
        /// ネストが存在するデータをJsonへシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(T obj)
        {
            if (obj is IList)
            {
                Wrapper<T> wrapper = new Wrapper<T>();
                wrapper.Items = obj;
                var json = JsonUtility.ToJson(wrapper);
                json = Regex.Replace(json, "^\\{\"Item\":", "");
                json = Regex.Replace(json, "\\}$", "");
                return json;
            }
            else
            {
                var json = JsonUtility.ToJson(obj);
                return json;
            }

        }

        [Serializable]
        private class Wrapper<T>
        {
            public T Items;
        }

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