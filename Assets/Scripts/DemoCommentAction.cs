using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ichikara.YoutubeComment
{
    public class DemoCommentAction : MonoBehaviour
    {
        private GameObject commentManager;

        private void Start()
        {
            //CommentManagerクラスを取得
            commentManager = CommentManager.Instance;

            CompareInfo ci = CultureInfo.CurrentCulture.CompareInfo;

            //特定条件の無名関数登録
            Func<string, bool> func = x =>
            {
                if(Regex.Match(x, @"[*\(*]").Success)
                {
                    return true;
                }
                return false;
            };

            //アクションの無名関数登録
            Action action = () => Debug.LogWarning("対応文字列入力確認");

            //条件とアクションの登録
            commentManager.GetComponent<CommentManager>().SetParticleAction(func, action);

        }
    }
}