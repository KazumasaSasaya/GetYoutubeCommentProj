using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ichikara.YoutubeComment
{
    public class Comment : MonoBehaviour
    {

        public void SetComment(CommentParts comment)
        {
            this.gameObject.transform.localScale = Vector3.one;
            this.transform.GetChild(1).GetComponent<Text>().text = comment.displayName;
            this.transform.GetChild(2).GetComponent<Text>().text = comment.displayMessage;
        }
    }

    public class CommentParts
    {

        public string Id;
        public string displayName;
        public string displayMessage;

    }
}
