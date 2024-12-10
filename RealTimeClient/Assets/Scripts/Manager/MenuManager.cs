//---------------------------------------------------------------
// ���j���[�}�l�[�W���[ [ MenuManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    //=====================================
    // �t�B�[���h

    [Header("---- Button ----")]

    // ���j���[�{�^��
    [SerializeField] private Button profileButton;  // �v���t�B�[��
    [SerializeField] private Button shopButton;     // �V���b�v
    [SerializeField] private Button optionButton;   // �I�v�V����

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        //++ �v���t�B�[�����擾�EUI���f

        //++ �V���b�v���擾�EUI���f
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        
    }

    //-----------------------------
    // �{�^����������

    /// <summary>
    /// �p�l���\��
    /// </summary>
    /// <param name="panel">�\���p�l��</param>
    public void OnDisplayPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    /// <summary>
    /// �p�l����\��
    /// </summary>
    /// <param name="panel">����p�l��</param>
    public void OnClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// �\���{�^��������
    /// </summary>
    public void OnSoloButton()
    {
        // �\�����[�h�J��
        SceneManager.LoadScene("03_SoloScene");
    }

    /// <summary>
    /// �I�����C���{�^��������
    /// </summary>
    public void OnOnlineButton()
    {
        // �I�����C�����[�h�J��
        SceneManager.LoadScene("04_MatchingScene");
    }
}
