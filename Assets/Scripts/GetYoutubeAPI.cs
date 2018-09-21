using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Ichikara.YoutubeComment
{
    public class GetYoutubeAPI : MonoBehaviour
    {
        private string apikey = "xxx";//apikeyを入れる

        private string searchBaseURI = "https://www.googleapis.com/youtube/v3/search?key=";

        private string searchBaseParts = "&part=snippet&channelId=";

        private string searchBaseChannnel = "UCpWBCGo4gEtrhMeB-IEwbdw";//ここを書き換えるYoutubeチャンネル

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


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
                YoutubeAPIProfile ss = new YoutubeAPIProfile();
                ss = JsonUtility.FromJson<YoutubeAPIProfile>(liverequest.downloadHandler.text);
                Debug.Log("profile.kind:" + ss.kind);
                Debug.Log("profile.nextpagetoken" + ss.nextPageToken);
                Debug.Log("profile.prevpagetoken" + ss.prevPageToken);
                Debug.Log("profile.etag" + ss.etag);
                Debug.Log("profile.pageinfo.totalResults" + ss.pageInfo.totalResults);
                Debug.Log("profile.pageinfo.resultsPerPage" + ss.pageInfo.resultsPerPage);
                Debug.Log("profile.items" + ss.items);
                Debug.Log("response: " + liverequest.downloadHandler.text);




                //jsontext = liverequest.downloadHandler.text;
                ////MiniJSON　つかうー！！！！
                //var mjson = (IDictionary)MiniJSON.Json.Deserialize(jsontext);
                //Debug.Log("miniJson 1 : " + jsontext);

                //var mitems = (IList)mjson["items"];
                //var mid = (IDictionary)mitems[0];
                //var sid = (IDictionary)mid["id"];
                //string mvideoId = (string)sid["videoId"];
                //Debug.Log(mvideoId);
                ////videoIdを取得
                //videoId = (string)sid["videoId"];


                /*vs2017ならできる？？
                var chatJsonObj = JsonConvert.DeserializeObject<dynamic>(jsontext);
                string videoId2 = chatJsonObj.items[0].id.videoId;
                Debug.Log("videoID2017 : " + videoId2);
                */


                //StartCoroutine(GetChatId());

            }

        }
    }
}