using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

/***
 * UnityJsonUtilityラッパークラス
 * ネストされているデータのシリアライズ・デシリアライズを行う
 */
namespace Ichikara.YoutubeComment
{
    public static class JsonStringFormatter
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
    }
}