//---------------------------------------------------------------
// �\���}�l�[�W���[ [ SoloManager.cs ]
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
    // �t�B�[���h



    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        
    }

    public void OnBackMenu()
    {
        // �\�����[�h�J��
        SceneManager.LoadScene("02_MenuScene");
    }
}
