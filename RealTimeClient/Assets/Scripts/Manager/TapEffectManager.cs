//---------------------------------------------------------------
// �^�b�v�G�t�F�N�g�}�l�[�W���[ [ TapEffectManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/17
// Update:2024/12/17
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffectManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    /// <summary>
    /// ���C���J����
    /// </summary>
    [SerializeField] private Camera mainCamera;

    /// <summary>
    /// �����G�t�F�N�g
    /// </summary>
    [SerializeField] private GameObject tapEffect;

    //=====================================
    // ���\�b�h

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ��ʂ��^�b�v�����Ƃ��̏���
            Vector2 tapPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition + mainCamera.transform.forward * 10);

            // �G�t�F�N�g�𐶐�
            GameObject effect = Instantiate(tapEffect, tapPosition, Quaternion.identity, this.transform);
        }
    }
}
