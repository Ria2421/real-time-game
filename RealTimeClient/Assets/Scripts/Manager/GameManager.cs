//---------------------------------------------------------------
// ゲームマネージャー [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/12
// Update:2024/11/12
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //-------------------------------------------------------
    // フィールド

    /// <summary>
    /// ユーザーID
    /// </summary>
    [SerializeField] private Text nameText;

    [SerializeField] UserModel userModel;

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
