using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ichikara.YoutubeComment
{
    public static class JsonStringFormatter
    {
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

        [Serializable]
        private class Wrapper<T>
        {
            public T Items;
        }
    }
}