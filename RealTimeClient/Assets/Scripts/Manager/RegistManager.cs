//---------------------------------------------------------------
// 登録マネージャー [ RegistManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/12
// Update:2024/11/18
//---------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistManager : MonoBehaviour
{
    //-------------------------------------------------------
    // フィールド

    /// <summary>
    /// ユーザーモデル格納用
    /// </summary>
    [SerializeField] UserModel userModel;

    /// <summary>
    /// ユーザー名
    /// </summary>
    [SerializeField] private Text nameText;

    //-------------------------------------------------------
    // メソッド

    // Update is called once per frame
    void Start()
    {

    }

    public async void OnInputName()
    {
        bool flag = await userModel.RegistUserAsync(nameText.text);

        if (flag)
        {
            Debug.Log("登録成功");
        }
        else
        {
            Debug.Log("登録失敗");
        }
    }
}
