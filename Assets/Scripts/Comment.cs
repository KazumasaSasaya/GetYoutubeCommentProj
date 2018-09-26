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

    public struct CommentParticle
    {
        public Func<string, bool> func;
        public Action action;
    }

    public class Comment : MonoBehaviour
    {
        [SerializeField]private Text displayName;

        [SerializeField]private Text displayMessage;

        private List<CommentParticle> particleList = new List<CommentParticle>();

        /// <summary>
        /// youtubeライブのコメント表示
        /// </summary>
        /// <param name="comment">Comment.</param>
        public void SetComment(CommentParts comment)
        {
            foreach(var particle in particleList)
            {
                if(particle.func(comment.displayMessage))
                {
                    particle.action();
                }
            }

            this.gameObject.transform.localScale = Vector3.one;
            this.displayName.text = comment.displayName;
            this.displayMessage.text = comment.displayMessage;
        }

        /// <summary>
        /// 特定コメントのアクション登録関数
        /// </summary>
        /// <param name="func">Func.</param>
        /// <param name="action">Action.</param>
        public void SetParticleAction(Func<string, bool> func, Action action)
        {
            CommentParticle commentParticle;
            commentParticle.func = func;
            commentParticle.action = action;
            this.SetParticleAction(commentParticle);
        }
        /// <summary>
        /// 特定コメントのアクション登録関数
        /// </summary>
        /// <param name="commentParticle">Comment particle.</param>
        public void SetParticleAction(CommentParticle commentParticle)
        {
            this.particleList.Add(commentParticle);
        }
    }
}
