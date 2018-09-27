using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ichikara.YoutubeComment
{
    /// <summary>
    /// コメントにたいしてなにかしらのアクションを起こすための無名関数構造体
    /// </summary>
    public struct CommentParticle
    {
        public Func<string, bool> func;
        public Action action;
    }

    public class CommentManager : MonoBehaviour
    {
        private static GameObject mInstance;

        [SerializeField] private GameObject contentParent;

        [SerializeField] private GameObject commentPrefab;

        [SerializeField] private ScrollRect scrollRect;


        [Tooltip("コメント最大保持数")] [Range(20, 200)] [SerializeField] private int holdingNumber = 50;

        private Queue<GameObject> queue = new Queue<GameObject>();

        private List<CommentParticle> particleList = new List<CommentParticle>();


        /// <summary>
        /// CommentManagerシングルトン
        /// </summary>
        /// <value>The instance.</value>
        public static GameObject Instance
        {
            get
            {
                return mInstance;
            }
        }

        private void Awake()
        {
            // シングルトン生成チェック
            if(mInstance == null)
            {
                DontDestroyOnLoad(gameObject);
                mInstance = gameObject;
            }
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
            particleList.Add(commentParticle);
        }


        /// <summary>
        /// コメントのuGUI表示用関数
        /// </summary>
        /// <param name="comment">Comment.</param>
        public void CommentUpdate(CommentParts comment)
        {
            GameObject go = Instantiate(commentPrefab);
            go.transform.SetParent(contentParent.transform);
            go.GetComponent<Comment>().SetComment(comment, particleList);

            queue.Enqueue(go);

            this.CommentDequeue();

            //スクロールビューの一番下に強制移動
            scrollRect.verticalNormalizedPosition = 0;
        }

        /// <summary>
        /// 古いコメントuGUI情報の削除
        /// </summary>
        public void CommentDequeue()
        {
            if (queue.Count > holdingNumber)
            {
                GameObject go = queue.Dequeue();
                if(go != null)
                {
                    Destroy(go);
                }
            }
        }
    }
}
