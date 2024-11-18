//---------------------------------------------------------------
// ゲームマネージャー [ GameManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/11/18
// Update:2024/11/18
//---------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    //-------------------------------------------------------
    // フィールド

    /// <summary>
    /// ルームモデル格納用
    /// </summary>
    [SerializeField] private RoomModel roomModel;

    /// <summary>
    /// ユーザーID
    /// </summary>
    [SerializeField] private Text idText;

    /// <summary>
    /// 生成するキャラクタープレハブ
    /// </summary>
    [SerializeField] private GameObject characterPrefab;

    /// <summary>
    /// 接続IDをキーにキャラクタのオブジェクトを管理
    /// </summary>
    private Dictionary<Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();

    //-------------------------------------------------------
    // メソッド

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 接続処理
    public async void OnConnect()
    {
        int userId = int.Parse(idText.text);    // 入力したユーザーIDの変換

        // ユーザーが入室したときにOnJoinUserメソッドを実行するよう、モデルに登録する。
        roomModel.OnJoinedUser += OnJoinedUser;
        // 接続
        await roomModel.ConnectAsync();
        // 入室 (ルーム名とユーザーIDを渡して入室。最終的にはローカルデータのユーザーIDを使用)
        await roomModel.JoinAsync("sampleRoom", userId);
    }

    // 入室したときの処理
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObj = Instantiate(characterPrefab);    // インスタンスの生成
        characterObj.transform.position = Vector3.zero;
        characterList[user.ConnectionId] = characterObj;    // フィールドで保存
    }
}
