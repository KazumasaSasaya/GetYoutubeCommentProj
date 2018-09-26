﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ichikara.YoutubeComment;


public class InputApiKey : MonoBehaviour {
    [SerializeField] private GetYoutubeAPI youtubeApi;
    [SerializeField] private InputField field;
	
    /// <summary>
    /// InputFieldを取得して初期化する
    /// </summary>
	void Start () {
        if (youtubeApi == null)
        {
            youtubeApi = this.GetComponent<GetYoutubeAPI>();
            Debug.Log("youtube api is null.");
        }

        if(field == null)
        {
            field = this.GetComponent<InputField>();
            Debug.Log("input field is null.");
        }

        InitInputField();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Log出力用メソッド
    /// 入力値を取得してLogに出力し、初期化
    /// </summary>
    public void InputLogger()
    {

        string inputValue = field.text;
        if(inputValue == null)
        {
            return;
        }
        Debug.Log("input keys:" + inputValue);

        StartCoroutine(UnactiveInputField(inputValue));

        InitInputField();
    }


    /// <summary>
    /// InputFieldの初期化用メソッド
    /// 入力値をリセットして、フィールドにフォーカスする
    /// </summary>
    private void InitInputField()
    {
        field.text = "";
        field.ActivateInputField();
    }

    /// <summary>
    /// 入力を確認した後文字入力画面を非アクティブにする
    /// </summary>
    IEnumerator UnactiveInputField(string key)
    {
        youtubeApi.GetYoutubeURI();
        yield return null;
        //this.gameObject.enabled = false;


    }
}
