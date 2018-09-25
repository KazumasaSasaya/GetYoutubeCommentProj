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

        private string[] splitString = { "\r\n", "\n" };

        private string[] deleteString = { " ", "\r", "\n", "\"", ":", "videoId", "activeLiveChatId", "displayMessage", "displayName", "nextPageToken" };

        private List<Comment> commentList;

        [Tooltip("コメント最大保持数")] [Range(20, 200)] [SerializeField] private int holdingNumber = 50;

        [Tooltip("コメント再取得インターバル(ミリ秒)")] [Range(4000, 10000)] [SerializeField] private int IntervalMillis = 5000;

        private void Start()
        {
            videoId = null;
            chatId = null;
            commentList = new List<Comment>();

            //debug用関数
            JsonStringFormatter.TextReset();
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

                string[] jsonData = liverequest.downloadHandler.text.Split(splitString, StringSplitOptions.None);

                foreach (var json in jsonData)
                {

                    //videoIdでなければcontinue
                    if (json.IndexOf("videoId") <= 0)
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
                string[] jsonData = channelRequest.downloadHandler.text.Split(splitString, StringSplitOptions.None);

                foreach(var json in jsonData)
                {
                    if(json.IndexOf("activeLiveChatId") <= 0)
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
            //コルーチンが２つ以上にならないように止める
            StopCoroutine(InvokeWait());
            //yield return new WaitForSeconds(5.0f);

            var commentURI = youtubeAPIbase + chatURIUp + chatId + pagetoken + nextPageTokenstr + chatURIbottom2 + apikey;

            Debug.Log("CommentURI : " + commentURI);

            UnityWebRequest connectCommentRequest = UnityWebRequest.Get(commentURI);
            yield return connectCommentRequest.SendWebRequest();

            if(connectCommentRequest.isHttpError || connectCommentRequest.isNetworkError)
            {
                Debug.LogError(connectCommentRequest.error);
            }
            else
            {
                string[] jsonData = connectCommentRequest.downloadHandler.text.Split(splitString, StringSplitOptions.None);

                Debug.Log("コメント--------------------");
                Debug.Log(connectCommentRequest.downloadHandler.text);
                Comment comment = new Comment();
                foreach (var json in jsonData)
                {
                    JsonStringFormatter.TextOutput(json);
                    if (json.IndexOf("nextPageToken") > 0)
                    {
                        string newToken = JsonStringFormatter.GetFormattedString(deleteString, json);

                        Debug.Log("newToken : " + newToken);
                        Debug.Log("nextPageTokenstr : " + nextPageTokenstr);
                        if (nextPageTokenstr == newToken)
                        {
                            Debug.Log("Same Token.");
                            break;
                        }
                        else
                        {
                            nextPageTokenstr = newToken;
                            Debug.Log("Change Token.");
                            continue;
                        }
                    }

                    if (json.IndexOf("displayMessage") <= 0 && json.IndexOf("displayName") <= 0)
                    {
                        continue;
                    }

                    if (json.IndexOf("displayMessage") > 0)
                    {
                        comment.displayMessage = JsonStringFormatter.GetFormattedString(deleteString, json);
                        Debug.Log("displayMessage : " + comment.displayMessage);
                    }
                    if (json.IndexOf("displayName") > 0)
                    {
                        comment.displayName = JsonStringFormatter.GetFormattedString(deleteString, json);
                        Debug.Log("displayName : " + comment.displayName);
                    }
                }
                StartCoroutine(InvokeWait());
            }
        }

        private IEnumerator InvokeWait()
        {

            yield return new WaitForSeconds(IntervalMillis);

            StartCoroutine(GetComment());
        }
    }
}