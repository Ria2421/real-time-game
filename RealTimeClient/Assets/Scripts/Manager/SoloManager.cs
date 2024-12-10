//---------------------------------------------------------------
// ソロマネージャー [ SoloManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoloManager : MonoBehaviour
{
    //=====================================
    // フィールド



    //=====================================
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        
    }

    public void OnBackMenu()
    {
        // ソロモード遷移
        SceneManager.LoadScene("02_MenuScene");
    }
}
