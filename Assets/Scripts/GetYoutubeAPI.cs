using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Ichikara.YoutubeComment
{
    public class GetYoutubeAPI : MonoBehaviour
    {
        private string apikey = APIKEY.API_KEY;//apikeyを入れる

        private string searchBaseURI = "https://www.googleapis.com/youtube/v3/search?key=";

        private string searchBaseParts = "&part=snippet&channelId=";

        private string searchBaseChannnel = "UCQINXHZqCU5i06HzxRkujfg";//ここを書き換えるYoutubeチャンネル

        private string searchBaseStr = "&eventType=live&type=video";

        private string videoId;

        private string youtubeAPIbase = "https://www.googleapis.com/youtube/v3/";

        private string channnelSearch = "videos?part=liveStreamingDetails&id=";

        private string chatId;

        private string pagetoken = "&pageToken=";

        private string chatURIUp = "liveChat/messages?liveChatId=";

        private bool connectionflag = false;

        private string nextPageTokenstr = null;

        private string jsontext;


        private string chatURIbottom2 = "&part=snippet,authorDetails&key=";

        private string[] deleteString = { " ", "\r", "\n", "\"", ":", "videoId", "activeLiveChatId" };

        private void Start()
        {
            videoId = null;
            chatId = null;
        }

        //public
        public void GetYoutubeURI()
        {
            StartCoroutine(GetURICoroutine());
        }


        //private
        private IEnumerator GetURICoroutine()
        {
            string uriStr = searchBaseURI + apikey + searchBaseParts + searchBaseChannnel + searchBaseStr;
            Debug.Log("uri: " + uriStr);

            UnityWebRequest liverequest = UnityWebRequest.Get(uriStr);
            yield return liverequest.SendWebRequest();

            if (liverequest.isHttpError || liverequest.isNetworkError)
            {
                Debug.LogError(liverequest.error);
            }
            else
            {
                Debug.Log(liverequest.downloadHandler.text);


                string[] split = { "\r\n", "\n" };
                string[] jsonData = liverequest.downloadHandler.text.Split(split, StringSplitOptions.None);

                foreach (var json in jsonData)
                {

                    //videoIdでなければcontinue
                    if (json.IndexOf("videoId") < 0)
                    {
                        Debug.Log("This is not videoId.");
                        continue;
                    }

                    //videoIdの整形
                    videoId = JsonStringFormatter.GetFormattedString(deleteString, json);
                    Debug.Log("videoId : " + videoId);

                    break;
                }

                // videoIdがない場合は処理を打ち切る
                if (videoId == null)
                {
                    Debug.LogWarning("videoId does not exist.");
                    yield break;
                }

                // chatIdを取得
                StartCoroutine(GetChatId());

            }
        }

        private IEnumerator GetChatId()
        {
            // TODO: JsonStringFormatterクラスにchannelを作らせる
            string channel = youtubeAPIbase + channnelSearch + videoId + "&key=" + apikey;
            Debug.Log(channel);
            UnityWebRequest channelRequest = UnityWebRequest.Get(channel);
            yield return channelRequest.SendWebRequest();

            if (channelRequest.isHttpError || channelRequest.isNetworkError)
            {
                Debug.LogError(channelRequest.error);
            }
            else
            {
                string[] split = { "\r\n", "\n" };
                string[] jsonData = channelRequest.downloadHandler.text.Split(split, StringSplitOptions.None);

                foreach(var json in jsonData)
                {
                    if(json.IndexOf("activeLiveChatId") < 0)
                    {
                        Debug.Log("This is not comment.");
                        continue;
                    }
                    Debug.Log("-----------------------");
                    Debug.Log(channelRequest.downloadHandler.text);

                    chatId = JsonStringFormatter.GetFormattedString(deleteString, json);
                    break;
                }

                StartCoroutine(GetComment());
            }

        }

        private IEnumerator GetComment()
        {
            var chatURI = youtubeAPIbase + chatURIUp + chatId + pagetoken + nextPageTokenstr + chatURIbottom2 + apikey;



            UnityWebRequest connectChatRequest = UnityWebRequest.Get(chatURI);
            yield return connectChatRequest.SendWebRequest();

            if(connectChatRequest.isHttpError || connectChatRequest.isNetworkError)
            {
                Debug.LogError(connectChatRequest.error);
            }
            else
            {
                string[] split = { "\r\n", "\n" };
                string[] jsonData = connectChatRequest.downloadHandler.text.Split(split, StringSplitOptions.None);

                Debug.Log("コメント--------------------");
                foreach(var json in jsonData)
                {
                    //Debug.Log(json);
                    //yield return null;

                }
            }

            //nextPageTokenstr = (string)commentlogjson["nextPageToken"];
            //Debug.Log(nextPageTokenstr);



            //var pageinfo = (IDictionary)commentlogjson["pageInfo"];
            ////var totalResults = (IDictionary)pageinfo[0];
            //int commentcount = int.Parse(pageinfo["totalResults"].ToString());
            //Debug.Log(commentcount + " : countnum");

            ////コメント分だけ描画
            //for (var i = 0; i < (int)commentcount; i++)
            //{
            //    GameObject cvn = Instantiate(canvas);

            //    var citems = (IList)commentlogjson["items"];
            //    var cslsd = (IDictionary)citems[i];
            //    var clad = (IDictionary)cslsd["snippet"];
            //    string message = (string)clad["displayMessage"];

            //    cvn.transform.Find("Description").gameObject.GetComponent<Text>().text = message;

            //    var author = (IDictionary)cslsd["authorDetails"];
            //    //var cslsd = (IDictionary)author[i];
            //    var dispName = (string)author["displayName"];

            //    cvn.transform.Find("Name").gameObject.GetComponent<Text>().text = dispName;

            //    float _x = UnityEngine.Random.Range(-400f, 400f);
            //    float _y = UnityEngine.Random.Range(-250f, 250f);
            //    cvn.transform.position = new Vector3(_x, _y, cvn.transform.position.z);
            //}




        }
    }
}