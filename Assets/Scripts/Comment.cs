using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ichikara.YoutubeComment
{
    public class CommentParts
    {

        public string Id;
        public string displayName;
        public string displayMessage;

    }

    public class Comment : MonoBehaviour
    {
        [SerializeField]private Text displayName;

        [SerializeField]private Text displayMessage;

        /// <summary>
        /// youtubeライブのコメント表示
        /// </summary>
        /// <param name="comment">Comment.</param>
        public void SetComment(CommentParts comment, List<CommentParticle> particleList)
        {
            foreach(var particle in particleList)
            {
                if(particle.func(comment.displayMessage))
                {
                    particle.action();
                }
            }

            //uGUIのScaleを明示して大きさを固定
            this.gameObject.transform.localScale = Vector3.one;

            this.displayName.text = comment.displayName;
            this.displayMessage.text = comment.displayMessage;
        }

    }
}
