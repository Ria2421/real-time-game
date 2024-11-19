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
using UnityEditor.Rendering;
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
    /// プレイヤーを格納する親オブジェクト
    /// </summary>
    [SerializeField] private GameObject parentObj;

    /// <summary>
    /// プレイヤーごとのリス地点
    /// </summary>
    [SerializeField] private Transform[] respownList; 

    /// <summary>
    /// 接続IDをキーにキャラクタのオブジェクトを管理
    /// </summary>
    private Dictionary<Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();

    /// <summary>
    /// ゲームフラグ
    /// </summary>
    private bool isGame = false;

    //-------------------------------------------------------
    // メソッド

    // 初期処理
    void Start()
    {
        // ユーザーが入室したときにOnJoinUserメソッドを実行するよう、モデルに登録する。
        roomModel.OnJoinedUser += OnJoinedUser;
        // ユーザーが退出したときにOnExitUserメソッドを実行するよう、モデルに登録する。
        roomModel.OnExitedUser += OnExitedUser;
        // ユーザーが退出したときにOnMoveUserメソッドを実行するよう、モデルに登録する。
        roomModel.OnMovedUser += OnMovedUser;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void FixedUpdate()
    {
        if (!isGame) return; 

        var moveData = new MoveData() { ConnectionId = roomModel.ConnectionId,
                                        Position = characterList[roomModel.ConnectionId].transform.position,
                                        Rotation = characterList[roomModel.ConnectionId].transform.eulerAngles  };

        await roomModel.MoveAsync(moveData);
    }

    // 接続処理
    public async void OnConnect()
    {
        int userId = int.Parse(idText.text);    // 入力したユーザーIDの変換

        // 接続
        await roomModel.ConnectAsync();
        // 入室 (ルーム名とユーザーIDを渡して入室。最終的にはローカルデータのユーザーIDを使用)
        await roomModel.JoinAsync("sampleRoom", userId);

        Debug.Log("入室");
    }

    // 切断処理
    public async void OnDisconnect()
    {
        // 退出
        await roomModel.ExitAsync();
        // 切断
        await roomModel.DisconnectionAsync();
        // プレイヤーオブジェクトの削除
        foreach(Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }

        isGame = false;
        Debug.Log("退出");
    }

    // 入室したときの処理
    private void OnJoinedUser(JoinedUser user)
    {
        Debug.Log(user.JoinOrder + "P");

        // プレイヤーの生成
        GameObject characterObj = Instantiate(characterPrefab, respownList[user.JoinOrder-1].position, Quaternion.Euler(0,180,0));

        characterObj.transform.parent = parentObj.transform;    // 親の設定
        characterList[user.ConnectionId] = characterObj;        // フィールドで保存

        if(user.ConnectionId == roomModel.ConnectionId)
        {
            characterList[roomModel.ConnectionId].gameObject.AddComponent<PlayerManager>();
            isGame = true;
        }
    }

    // 退出したときの処理
    private void OnExitedUser(Guid connectionId)
    {
        // 位置情報の更新
        if (!characterList.ContainsKey(connectionId)) return;

        Destroy(characterList[connectionId]);   // オブジェクトの破棄
        characterList.Remove(connectionId);     // リストから削除
    }

    // プレイヤーが移動したときの処理
    private void OnMovedUser(MoveData moveData)
    {
        // 位置情報の更新
        if (!characterList.ContainsKey(moveData.ConnectionId)) return;

        characterList[moveData.ConnectionId].gameObject.transform.position = moveData.Position;
        characterList[moveData.ConnectionId].gameObject.transform.eulerAngles = moveData.Rotation; 
    }
}
