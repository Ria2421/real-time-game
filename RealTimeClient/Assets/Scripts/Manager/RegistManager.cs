//---------------------------------------------------------------
// �o�^�}�l�[�W���[ [ RegistManager.cs ]
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
    // �t�B�[���h

    /// <summary>
    /// ���[�U�[���f���i�[�p
    /// </summary>
    [SerializeField] UserModel userModel;

    /// <summary>
    /// ���[�U�[��
    /// </summary>
    [SerializeField] private Text nameText;

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
