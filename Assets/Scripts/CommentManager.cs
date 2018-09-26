using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ichikara.YoutubeComment
{
    public class CommentManager : MonoBehaviour
    {
        private static GameObject mInstance;

        [SerializeField] private GameObject contentParent;

        [SerializeField] private GameObject commentPrefab;

        [SerializeField] private ScrollRect scrollRect;

        private Queue<GameObject> queue;

        public static GameObject Instance
        {
            get
            {
                return mInstance;
            }
        }

        private void Awake()
        {
            if(mInstance == null)
            {
                DontDestroyOnLoad(gameObject);
                mInstance = gameObject;
            }
        }

        private void Start()
        {
            queue = new Queue<GameObject>();
        }

        public void CommentUpdate(CommentParts comment)
        {
            GameObject go = Instantiate(commentPrefab);
            go.transform.SetParent(contentParent.transform);
            go.GetComponent<Comment>().SetComment(comment);

            queue.Enqueue(go);

            scrollRect.verticalNormalizedPosition = 0;
        }

        public void CommentDequeue(int maxCount)
        {
            if(queue.Count > maxCount)
            {
                GameObject go = queue.Dequeue();
                Destroy(go);
                Debug.Log("Dequeue Comment.");
            }
        }
    }
}
