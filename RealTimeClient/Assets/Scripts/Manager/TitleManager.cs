//---------------------------------------------------------------
// タイトルマネージャー [ TitleManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/05
// Update:2024/12/10
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    /// <summary>
    /// スタートボタン押下時
    /// </summary>
    public void OnStartButton()
    {
        SceneManager.LoadScene("02_MenuScene");
    }
}