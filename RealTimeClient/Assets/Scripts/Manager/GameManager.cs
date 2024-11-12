//---------------------------------------------------------------
// �Q�[���}�l�[�W���[ [ GameManager.cs ]
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
    // �t�B�[���h

    /// <summary>
    /// ���[�U�[ID
    /// </summary>
    [SerializeField] private Text nameText;

    [SerializeField] UserModel userModel;

    //-------------------------------------------------------
    // ���\�b�h

    // Update is called once per frame
    void Start()
    {

    }

    public async void OnInputName()
    {
        bool flag = await userModel.RegistUserAsync(nameText.text);

        if (flag)
        {
            Debug.Log("�o�^����");
        }
        else
        {
            Debug.Log("�o�^���s");
        }
    }
}
