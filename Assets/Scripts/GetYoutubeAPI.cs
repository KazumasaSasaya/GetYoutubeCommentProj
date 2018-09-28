using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Ichikara.YoutubeComment;

namespace Ichikara.YoutubeComment
{
    public class GetYoutubeAPI : MonoBehaviour
    {
        private string apikey = APIKEY.API_KEY;//apikeyを入れる

        private string searchBaseURI = "https://www.googleapis.com/youtube/v3/search?key=";

        private string searchBaseParts = "&part=snippet&channelId=";

        private string searchBaseChannnel = "UCQINXHZqCU5i06HzxRkujfg";//ここを書き換えるYoutubeチャンネル

        private string searchBaseStr = "&eventType=live&type=video";

        private string videoId = null;

        private string youtubeAPIbase = "https://www.googleapis.com/youtube/v3/";

        private string channnelSearch = "videos?part=liveStreamingDetails&id=";

        private string chatId = null;

        private string pagetoken = "&pageToken=";

        private string chatURIUp = "liveChat/messages?liveChatId=";

        private string nextPageTokenstr = null;

        private string chatURIbottom2 = "&part=snippet,authorDetails&key=";

        private string[] splitString = { "\r\n", "\n" };

        private string[] deleteString = { " ", "\r", "\n", "\"", ":", "videoId", "activeLiveChatId", "displayMessage", "displayName", "nextPageToken" };

        private GameObject commentManager;//シングルトンクラス

        [Tooltip("コメント再取得インターバル(ミリ秒)")] [Range(4000, 10000)] [SerializeField] private int IntervalMillis = 5000;

        private void Start()
        {
            commentManager = CommentManager.Instance;

            //debug用関数
            StringFormatter.TextReset();
        }

        //public
        public void GetYoutubeURI()
        {
            StartCoroutine(this.GetURICoroutine());

            //debug
            StartCoroutine(this.GetYoutubeSuperchat());
        }

        private IEnumerator GetYoutubeSuperchat()
        {
            string uri = "https://www.googleapis.com/youtube/v3/superChatEvents?key=" + APIKEY.API_KEY + "&part=snippet";

            Debug.Log("superChat : " + uri);

            UnityWebRequest liveRequest = UnityWebRequest.Get(uri);
            yield return liveRequest.SendWebRequest();

            if(liveRequest.isNetworkError || liveRequest.isHttpError)
            {
                Debug.LogError(liveRequest.error);
            }
        }


        /// <summary>
        /// チャンネルIDから現在のライブIDを取得するコルーチン
        /// </summary>
        /// <returns>The URIC oroutine.</returns>
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
                        Debug.LogWarning("This is not videoId.");
                        continue;
                    }

                    //videoIdの整形
                    videoId = StringFormatter.GetFormattedString(deleteString, json);
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
                StartCoroutine(this.GetChatId());

            }
        }

        /// <summary>
        /// YoutubeライブのチャットIDを取得するコルーチン
        /// </summary>
        /// <returns>The chat identifier.</returns>
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

                Debug.Log("-----------------------");
                Debug.Log(channelRequest.downloadHandler.text);

                foreach (var json in jsonData)
                {
                    if(json.IndexOf("activeLiveChatId") <= 0)
                    {
                        Debug.LogWarning("This is not comment.");
                        continue;
                    }

                    chatId = StringFormatter.GetFormattedString(deleteString, json);
                    break;
                }

                StartCoroutine(this.GetComment());
            }

        }

        /// <summary>
        /// Youtubeライブのコメントを取得するコルーチン
        /// </summary>
        /// <returns>The comment.</returns>
        private IEnumerator GetComment()
        {
            //コルーチンが２つ以上にならないように止める
            StopCoroutine(this.InvokeWait());

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

                CommentParts comment = new CommentParts();
                foreach (var json in jsonData)
                {
                    StringFormatter.TextOutput(json);
                    if (json.IndexOf("nextPageToken") > 0)
                    {
                        string newToken = StringFormatter.GetFormattedString(deleteString, json);

                        Debug.Log("newToken : " + newToken);
                        Debug.Log("nextPageTokenstr : " + nextPageTokenstr);
                        if (nextPageTokenstr == newToken)
                        {
                            Debug.Log("Same Token.");
                        }
                        else
                        {
                            nextPageTokenstr = newToken;
                            Debug.Log("Change Token.");
                            continue;
                        }
                    }

                    if (json.IndexOf("displayMessage") > 0)
                    {
                        comment.displayMessage = StringFormatter.GetFormattedString(deleteString, json);
                        Debug.Log("displayMessage : " + comment.displayMessage);
                    }
                    if (json.IndexOf("displayName") > 0)
                    {
                        comment.displayName = StringFormatter.GetFormattedString(deleteString, json);
                        Debug.Log("displayName : " + comment.displayName);
                    }

                    if(comment.displayName != null && comment.displayMessage != null)
                    {
                        commentManager.GetComponent<CommentManager>().CommentUpdate(comment);
                        comment = new CommentParts();
                        yield return null;
                    }
                }
                StartCoroutine(this.InvokeWait());
            }

        }

        /// <summary>
        /// 一定時間ごとにコメントを取得させるコルーチン
        /// </summary>
        /// <returns>The wait.</returns>
        private IEnumerator InvokeWait()
        {

            yield return new WaitForSeconds(IntervalMillis / 1000.0f);

            StartCoroutine(this.GetComment());
        }
    }
}