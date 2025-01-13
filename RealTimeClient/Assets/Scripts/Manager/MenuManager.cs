//---------------------------------------------------------------
// ���j���[�}�l�[�W���[ [ MenuManager.cs ]
// Author:Kenta Nakamoto
// Data:2024/12/10
// Update:2024/12/10
//---------------------------------------------------------------
using KanKikuchi.AudioManager;
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
    [SerializeField] private Button acountButton;   // �A�J�E���g
    [SerializeField] private Button shopButton;     // �V���b�v
    [SerializeField] private Button optionButton;   // �I�v�V����

    //=====================================
    // ���\�b�h

    /// <summary>
    /// ��������
    /// </summary>
    void Start()
    {
        // BGM�Đ�
        //�Đ�����BGM�̖��O��S�Ď擾
        var currentBGMNames = BGMManager.Instance.GetCurrentAudioNames();

        if (currentBGMNames[0] != "MainBGM")
        {   // MainBGM���ĊJ
            BGMManager.Instance.Stop(BGMPath.TIME_ATTACK);
            BGMManager.Instance.Stop(BGMPath.MULTI_PLAY);
            BGMManager.Instance.Play(BGMPath.MAIN_BGM, 0.75f, 0, 1, true, true);
        }

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
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);
        panel.SetActive(true);
    }

    /// <summary>
    /// �p�l����\��
    /// </summary>
    /// <param name="panel">����p�l��</param>
    public void OnClosePanel(GameObject panel)
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);
        panel.SetActive(false);
    }

    /// <summary>
    /// �\���{�^��������
    /// </summary>
    public void OnSoloButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �\���I�����[�h�J��
        SceneManager.LoadScene("03_SoloSelectScene");
    }

    /// <summary>
    /// �I�����C���{�^��������
    /// </summary>
    public void OnOnlineButton()
    {
        // SE�Đ�
        SEManager.Instance.Play(SEPath.TAP_BUTTON);

        // �I�����C�����[�h�J��
        SceneManager.LoadScene("05_MatchingScene");
    }
}
